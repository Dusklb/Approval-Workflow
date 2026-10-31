using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Office.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Data;
using System.Drawing;
using Trinhky.Module;
using Trinhky.Module.BusinessObjects;
using static Trinhky.Module.BusinessObjects.TCom;

namespace Trinhky.Blazor.Server.Controllers
{
    public static class BCom
    {
        public static string webRootPath;
        public static void UpdateTrangthaiPhieu(int phieuiOid)
        {
            if (phieuiOid > 0)
            {
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(Phieudexuat));
                Session session = ((XPObjectSpace)objectSpace).Session;
                Phieudexuat phieudexuat = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieuiOid));
                if (phieudexuat != null)
                {
                    phieudexuat.Trangthai = TCom.EStatusVB.choduyet;
                    DateTime ngay = DateTime.MinValue;
                    string sql = "SELECT TOP (100) PERCENT Trangthai, Ngayduyet FROM     dbo.DuyetDexuat WHERE (Boqua=0) and (Phieu = " + phieudexuat.Oid + ") ORDER BY SoTT";
                    SelectedData results = session.ExecuteQuery(sql);
                    int lan = 0;
                    bool huy = false;
                    foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                    {
                        int trangthai = tmLib.CommonLib.CInt(row.Values[0]);
                        if (trangthai == 0)
                        {
                            phieudexuat.Trangthai = TCom.EStatusVB.choduyet;
                        }
                        else if (trangthai == 1)
                        {
                            phieudexuat.Trangthai = TCom.EStatusVB.dahuy;
                            huy = true;
                            break;
                        }
                        else
                        {
                            lan++;
                            if (row.Values[1] != null)
                            {
                                ngay = tmLib.CommonLib.CDate(row.Values[1]);
                            }
                        }
                    }
                    if (!huy)
                    {
                        int TongBuoc = 0;
                        foreach (DuyetDexuat buoc in phieudexuat.DuyetDexuats)
                        {
                            if (buoc.Boqua == false)
                                TongBuoc += 1;
                        }

                        phieudexuat.TyleDuyet = (int)(lan * 100 / TongBuoc);
                        if (lan == TongBuoc)
                        {
                            phieudexuat.Trangthai = TCom.EStatusVB.daduyet;
                            if (ngay != DateTime.MinValue)
                                phieudexuat.Ngayduyet = ngay;
                            CreatePdf(phieudexuat);
                            CreateLog(phieudexuat);
                            //update trang thái hoàn thành của phieu lien quan
                            if (phieudexuat.LinkID > 0)
                            {
                                Phieudexuat phieuLink = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieudexuat.LinkID));
                                if (phieuLink != null)
                                {
                                    phieuLink.Daxong = true;
                                    phieuLink.Ngayxong = GetServerDateTime();
                                    phieuLink.Save();
                                }
                            }
                        }
                        else if (lan > 0)
                        {
                            phieudexuat.Trangthai = TCom.EStatusVB.dangduyet;
                        }
                    }
                    else
                        phieudexuat.TyleDuyet = 0;

                    if (phieudexuat.Daxong)
                    {
                        phieudexuat.Ngayxong = TCom.GetServerDateTime();
                    }
                    else
                        phieudexuat.Ngayxong = DateTime.MinValue;
                    objectSpace.CommitChanges();
                    if(phieudexuat.Trangthai == TCom.EStatusVB.daduyet)
                    {
                        TCom.SendEmailXuly(phieudexuat);
                    }
                }
            }
        }
        public static void CreatePdf(Phieudexuat phieuxong)
        {
            if (phieuxong != null)
            {
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieudexuat = objectSpace.GetObject(phieuxong);
                //Ghep file
                RichEditDocumentServer KyProcessor = new()
                {
                    RtfText = phieudexuat.Kyduyet
                };

                using RichEditDocumentServer wordProcessor = new();
                wordProcessor.RtfText = phieudexuat.Noidung;

                var pos = KyProcessor.Document.CreatePosition(0);
                KyProcessor.Document.InsertRtfText(pos, wordProcessor.RtfText);
                //Thay chu ky vao KyProcessor
                string hoten = phieudexuat.Nhanvien.HoTen;
                KyProcessor.Document.ReplaceAll("{Hoten}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);

                if (phieudexuat.Nhanvien.AnhCK != null)
                {
                    using var ms = new MemoryStream(phieudexuat.Nhanvien.AnhCK);
                    Image img = Image.FromStream(ms);
                    DocumentRange[] ranges = KyProcessor.Document.FindAll("{KyHoten}", SearchOptions.None, KyProcessor.Document.Range);
                    if (ranges.Length > 0)
                    {
                        DocumentRange range = ranges[0];
                        DocumentPosition startPosition = range.Start;
                        KyProcessor.Document.Delete(range);
                        KyProcessor.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                    }
                }
                //Ngay thang nam
                if (phieudexuat.Ngay != DateTime.MinValue)
                {
                    string ngay = phieudexuat.Ngay.Day.ToString();
                    string thang = phieudexuat.Ngay.Month.ToString();
                    string nam = phieudexuat.Ngay.Year.ToString("####");
                    KyProcessor.Document.ReplaceAll("{dd}", ngay, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    KyProcessor.Document.ReplaceAll("{mm}", thang, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    KyProcessor.Document.ReplaceAll("{yy}", nam, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                //them cac chu ky 
                int ketthucOid = 0;
                int ketthucSott = 0;
                foreach (DuyetDexuat duyetDexuat in phieudexuat.DuyetDexuats)
                {
                    if (duyetDexuat.Ketthuc)
                    {
                        ketthucOid = duyetDexuat.Nguoiduyet.Oid;
                        ketthucSott = duyetDexuat.SoTT;
                    }
                }
                foreach (DuyetDexuat duyetDexuat in phieudexuat.DuyetDexuats)
                {
                    if (duyetDexuat.DuyetUQ == false)
                    {
                        if (ketthucOid == 0)//duyet binh thuong
                        {
                            if (duyetDexuat.Boqua)
                            {
                                string ma = duyetDexuat.Maduyet;
                                string machuky = duyetDexuat.MaChuky;
                                if (!string.IsNullOrEmpty(ma))
                                    KyProcessor.Document.ReplaceAll(ma, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                if (!string.IsNullOrEmpty(machuky))
                                    KyProcessor.Document.ReplaceAll(machuky, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                            }
                            else
                            {
                                Nhansu nguoiky = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", duyetDexuat.Nguoiduyet));
                                if (nguoiky != null)
                                {
                                    string ma = duyetDexuat.Maduyet;
                                    string machuky = duyetDexuat.MaChuky;
                                    string kyten = nguoiky.HoTen;
                                    if (!string.IsNullOrEmpty(ma))
                                        KyProcessor.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                    string makt = duyetDexuat.MaKythay;
                                    if (!string.IsNullOrEmpty(makt))
                                        KyProcessor.Document.ReplaceAll(makt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                    //Thay chu ky vao KyProcessor
                                    if (nguoiky.AnhCK != null)
                                    {
                                        using var ms = new MemoryStream(nguoiky.AnhCK);
                                        Image img = Image.FromStream(ms);
                                        DocumentRange[] ranges = KyProcessor.Document.FindAll(machuky, SearchOptions.None, KyProcessor.Document.Range);
                                        if (ranges.Length > 0)
                                        {
                                            DocumentRange range = ranges[0];
                                            DocumentPosition startPosition = range.Start;
                                            KyProcessor.Document.Delete(range);
                                            KyProcessor.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                                        }
                                    }
                                }

                            }
                        }
                        else//co ket thuc
                        {
                            if (duyetDexuat.Boqua && duyetDexuat.SoTT < ketthucSott)
                            {
                                string ma = duyetDexuat.Maduyet;
                                string machuky = duyetDexuat.MaChuky;
                                if (!string.IsNullOrEmpty(ma))
                                    KyProcessor.Document.ReplaceAll(ma, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                if (!string.IsNullOrEmpty(machuky))
                                    KyProcessor.Document.ReplaceAll(machuky, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                            }
                            else
                            {
                                if (duyetDexuat.SoTT <= ketthucSott)//ky binh thuong
                                {
                                    Nhansu nguoiky = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", duyetDexuat.Nguoiduyet));
                                    if (nguoiky != null)
                                    {
                                        string ma = duyetDexuat.Maduyet;
                                        string machuky = duyetDexuat.MaChuky;
                                        string kyten = nguoiky.HoTen;
                                        if (!string.IsNullOrEmpty(ma))
                                            KyProcessor.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                        string makt = duyetDexuat.MaKythay;
                                        if (!string.IsNullOrEmpty(makt))
                                            KyProcessor.Document.ReplaceAll(makt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                        //Thay chu ky vao KyProcessor
                                        if (nguoiky.AnhCK != null)
                                        {
                                            using var ms = new MemoryStream(nguoiky.AnhCK);
                                            Image img = Image.FromStream(ms);
                                            DocumentRange[] ranges = KyProcessor.Document.FindAll(machuky, SearchOptions.None, KyProcessor.Document.Range);
                                            if (ranges.Length > 0)
                                            {
                                                DocumentRange range = ranges[0];
                                                DocumentPosition startPosition = range.Start;
                                                KyProcessor.Document.Delete(range);
                                                KyProcessor.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                                            }
                                        }
                                    }
                                }
                                else//ky ket thuc
                                {
                                    Nhansu nguoiky = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", ketthucOid));
                                    if (nguoiky != null)
                                    {
                                        string ma = duyetDexuat.Maduyet;
                                        string machuky = duyetDexuat.MaChuky;
                                        string kyten = nguoiky.HoTen;
                                        if (!string.IsNullOrEmpty(ma))
                                            KyProcessor.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                        string makt = duyetDexuat.MaKythay;
                                        if (!string.IsNullOrEmpty(makt))
                                            KyProcessor.Document.ReplaceAll(makt, "TUQ-", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                                        //Thay chu ky vao KyProcessor
                                        if (nguoiky.AnhCK != null)
                                        {
                                            using var ms = new MemoryStream(nguoiky.AnhCK);
                                            Image img = Image.FromStream(ms);
                                            DocumentRange[] ranges = KyProcessor.Document.FindAll(machuky, SearchOptions.None, KyProcessor.Document.Range);
                                            if (ranges.Length > 0)
                                            {
                                                DocumentRange range = ranges[0];
                                                DocumentPosition startPosition = range.Start;
                                                KyProcessor.Document.Delete(range);
                                                KyProcessor.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Nhansu nguoiky = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", duyetDexuat.NguoiUQ));
                        if (nguoiky != null)
                        {
                            string ma = duyetDexuat.Maduyet;
                            string machuky = duyetDexuat.MaChuky;
                            string kyten = nguoiky.HoTen;
                            if (!string.IsNullOrEmpty(ma))
                                KyProcessor.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                            string makt = duyetDexuat.MaKythay;
                            if (!string.IsNullOrEmpty(makt))
                                KyProcessor.Document.ReplaceAll(makt, "TUQ-", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                            //Thay chu ky vao KyProcessor
                            if (nguoiky.AnhCK != null)
                            {
                                using var ms = new MemoryStream(nguoiky.AnhCK);
                                Image img = Image.FromStream(ms);
                                DocumentRange[] ranges = KyProcessor.Document.FindAll(machuky, SearchOptions.None, KyProcessor.Document.Range);
                                if (ranges.Length > 0)
                                {
                                    DocumentRange range = ranges[0];
                                    DocumentPosition startPosition = range.Start;
                                    KyProcessor.Document.Delete(range);
                                    KyProcessor.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                                }
                            }
                        }
                    }
                }

                // Specify export options.

                PdfExportOptions options = new();
                options.DocumentOptions.Author = phieudexuat.Nhanvien.HoTen;
                options.Compressed = false;
                options.ImageQuality = PdfJpegImageQuality.Highest;
                options.ShowPrintDialogOnOpen = true;
                // Export the document to a stream.

                //string path = System.IO.Path.GetTempPath();
                string fName = phieudexuat.Oid.ToString() + "_" + phieudexuat.Nhanvien.HoTen + ".pdf";
                string tepurl = @"Files\" + phieudexuat.Nhanvien.Sothe + @"\" + fName;
                string path = Path.Combine(BCom.webRootPath, @"Files\" + phieudexuat.Nhanvien.Sothe);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string combined = Path.Combine(path, fName);

                if (System.IO.File.Exists(combined))
                    System.IO.File.Delete(combined);

                using (FileStream pdfFileStream = new(combined, FileMode.Create))
                {
                    KyProcessor.ExportToPdf(pdfFileStream, options);
                }

                Phieudexuat phieu = objectSpace.GetObject(phieudexuat);
                if (phieu != null)
                {
                    PhieudexuatFile kem = objectSpace.FindObject<PhieudexuatFile>(CriteriaOperator.Parse("Phieu=? && Loai=?", phieudexuat, 1));
                    if (kem == null)
                    {
                        kem = objectSpace.CreateObject<PhieudexuatFile>();
                        kem.Phieu = phieu;
                        kem.Tailieu = "Đề xuất đã duyệt số " + phieu.Oid.ToString();
                        kem.TepTen = fName;
                        kem.TepUrl = tepurl;
                        kem.Loai = 1;
                        kem.Save();
                        phieu.PhieudexuatFiles.Add(kem);
                        objectSpace.CommitChanges();
                    }
                    else
                    {
                        kem.TepTen = fName;
                        kem.TepUrl = tepurl;
                        kem.Save();
                        objectSpace.CommitChanges();
                    }
                }
            }
        }

        public static void CreateLog(Phieudexuat phieu)
        {
            XafApplication app = ApplicationHelper.Instance.Application;
            IObjectSpace objectSpace = app.CreateObjectSpace(typeof(Phieudexuat));
            Session session = ((XPObjectSpace)objectSpace).Session;
            Phieudexuat phieudexuat = objectSpace.GetObject(phieu);
            //Tao file
            RichEditDocumentServer KyProcessor = new();
            DevExpress.XtraRichEdit.API.Native.Document document = KyProcessor.Document;

            //ParagraphStyle tdParastyle = document.ParagraphStyles.CreateNew();
            //tdParastyle.Name = "tdParastyle";
            ////pstyle.LineSpacingType = ParagraphLineSpacing.Double;
            //tdParastyle.Alignment = ParagraphAlignment.Center;
            //document.ParagraphStyles.Add(tdParastyle);

            //CharacterStyle tdFontstyle = document.CharacterStyles.CreateNew();
            //tdFontstyle.Name = "tdFontstyle";
            ////cstyle.Parent = document.CharacterStyles["Default Paragraph Font"];
            ////cstyle.ForeColor = System.Drawing.Color.DarkOrange;
            ////cstyle.Strikeout = StrikeoutType.Double;
            //tdFontstyle.FontName = "Times New Roman";
            //tdFontstyle.FontSize = 16;
            //tdFontstyle.Bold = true;
            //document.CharacterStyles.Add(tdFontstyle);

            //Paragraph paragraph = document.Paragraphs.Append();
            //paragraph.Style= tdParastyle;

            document.BeginUpdate();
            //Tieu de
            document.AppendText("NHẬT KÝ DUYỆT ĐỀ XUẤT\n");
            document.AppendText(phieudexuat.TenDexuat + "\n");
            document.AppendText("Số: " + phieudexuat.Oid.ToString() + ", ngày: " + phieudexuat.Ngay.ToString("dd/MM/yyyy HH:mm") + "\n");
            string nguoilap = phieudexuat.Nhanvien.HoTen;
            if (phieudexuat.Nhanvien.Donvi != null)
                nguoilap += ", Bộ phận: " + phieudexuat.Nhanvien.Donvi.Name;
            if (phieudexuat.Nhanvien.CS != null)
                nguoilap += ", Cơ sở: " + phieudexuat.Nhanvien.CS.TenCoso;
            document.AppendText("Người lập: " + nguoilap + "\n");

            string xuly = "Xử lý sau duyệt: ";
            if (phieudexuat.NguoiXuly != null)
            {
                xuly += phieudexuat.NguoiXuly.HoTen + " - Hoàn thành: ";
                if (phieudexuat.Ngayxong != DateTime.MinValue)
                    xuly += phieudexuat.Ngayxong.ToShortDateString();
            }
            document.AppendText(xuly + "\n");
            document.AppendText("\n");
            document.EndUpdate();

            DocumentRange range = document.Paragraphs[0].Range;
            CharacterProperties titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 16;
            titleFormatting.Bold = true;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            ParagraphProperties titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Center;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);

            range = document.Paragraphs[1].Range;
            titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 14;
            titleFormatting.Bold = false;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Center;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);
            for (int i = 2; i < 5; i++)
            {
                range = document.Paragraphs[i].Range;
                titleFormatting = document.BeginUpdateCharacters(range);
                //Set the character size, font name and color
                titleFormatting.FontSize = 12;
                titleFormatting.Bold = false;
                titleFormatting.FontName = "Times New Roman";
                titleFormatting.ForeColor = Color.Black;
                document.EndUpdateCharacters(titleFormatting);

                titleParagraphFormatting = document.BeginUpdateParagraphs(range);
                titleParagraphFormatting.Alignment = ParagraphAlignment.Left;
                document.EndUpdateParagraphs(titleParagraphFormatting);
            }

            DataTable dataTable;
            DataColumn dtColumn;
            DataRow myDataRow;

            //Tai lieu dinh kem
            document.BeginUpdate();
            range = document.AppendText("Tài liệu kèm theo");
            document.EndUpdate();
            //range = document.Paragraphs[6].Range;
            titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 12;
            titleFormatting.Bold = true;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Left;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);

            dataTable = new("tailieu");
            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ngay",
                Caption = "Ngày",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Hoten",
                Caption = "Người gửi",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Tailieu",
                Caption = "Tên tài liệu",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Tep",
                Caption = "Tên tệp",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTable.Columns.Add(dtColumn);

            XPCollection<PhieudexuatFile> ds = new(session)
            {
                Criteria = CriteriaOperator.Parse("Phieu.Oid=? and Loai=?", phieu.Oid, 0),
                Sorting = new SortingCollection(new SortProperty("Ngay", SortingDirection.Ascending))
            };
            if (ds.Count > 0)
            {
                foreach (PhieudexuatFile p in ds)
                {
                    myDataRow = dataTable.NewRow();
                    myDataRow["Ngay"] = p.Ngay.ToString("dd/MM/yyyy HH:mm");
                    if (p.Nhanvien != null)
                        myDataRow["Hoten"] = p.Nhanvien.HoTen;
                    myDataRow["Tailieu"] = p.Tailieu;
                    myDataRow["Tep"] = p.TepTen;

                    dataTable.Rows.Add(myDataRow);
                }
            }
            int dataTableRows = dataTable.Rows.Count;
            int dataTableColumns = dataTable.Columns.Count;
            List<string> columnsToDisplay = new();
            List<string> columnsCaption = new();

            for (int i = 0; i < dataTableColumns; i++)
            {
                columnsToDisplay.Add(dataTable.Columns[i].ColumnName);
                columnsCaption.Add(dataTable.Columns[i].Caption);
            }

            document.BeginUpdate();

            Table table = document.Tables.Create(document.Range.End, dataTableRows + 1, columnsToDisplay.Count, AutoFitBehaviorType.AutoFitToContents);
            //            table.BeginUpdate();
            //            table.TableLayout = TableLayoutType.Autofit;
            //table.Borders.InsideHorizontalBorder.LineColor = Color.DarkBlue;
            //table.Borders.InsideVerticalBorder.LineColor = Color.DarkBlue;
            //table.Borders.InsideHorizontalBorder.LineThickness = 0.5f;
            //table.Borders.InsideHorizontalBorder.LineStyle = TableBorderLineStyle.Single;
            //table.Borders.InsideVerticalBorder.LineThickness = 0.5f;
            //table.Borders.InsideVerticalBorder.LineStyle = TableBorderLineStyle.Single;
            //table.LeftPadding = Units.InchesToDocumentsF(0.01f);
            table.FirstRow.Height = Units.InchesToDocumentsF(0.1f);
            table.FirstRow.HeightType = HeightType.Exact;

            //TableCell firstCell = table.Rows[0].FirstCell;
            //firstCell.PreferredWidthType = WidthType.Fixed;
            //firstCell.PreferredWidth = Units.InchesToDocumentsF(0.5f);

            //TableCell nguoiColumn = table[0, 1];
            //nguoiColumn.PreferredWidthType = WidthType.Fixed;
            //nguoiColumn.PreferredWidth = Units.InchesToDocumentsF(0.5f);
            ////nguoiColumn.HeightType = HeightType.Auto;
            ////nguoiColumn.Height = Units.InchesToDocumentsF(0.5f);

            //TableCell TlColumn = table[0, 2];
            //TlColumn.PreferredWidthType = WidthType.Auto;
            ////TlColumn.HeightType = HeightType.Auto;

            //TableCell lastCell = table.Rows[0].LastCell;
            //lastCell.PreferredWidthType = WidthType.Fixed;
            //lastCell.PreferredWidth = Units.InchesToDocumentsF(0.5f);

            table.Rows[0].FirstCell.PreferredWidthType = WidthType.Fixed;
            table.Rows[0].FirstCell.PreferredWidth = Units.InchesToDocumentsF(1.2f);


            //Set the second column width and cell height
            table[0, 1].PreferredWidthType = WidthType.Fixed;
            table[0, 1].PreferredWidth = Units.InchesToDocumentsF(1.2f);
            table[0, 1].HeightType = HeightType.Exact;
            table[0, 1].Height = Units.InchesToDocumentsF(0.5f);

            //Set the second column width and cell height
            table[0, 2].PreferredWidthType = WidthType.Fixed;
            table[0, 2].PreferredWidth = Units.InchesToDocumentsF(4f);
            table[0, 2].HeightType = HeightType.Exact;
            table[0, 2].Height = Units.InchesToDocumentsF(0.5f);

            //Set the third column width 
            table.Rows[0].LastCell.PreferredWidthType = WidthType.Fixed;
            table.Rows[0].LastCell.PreferredWidth = Units.InchesToDocumentsF(1.2f);

            //for (int i = 1; i < table.Rows.Count; i++)
            //{
            //    table.Rows[i].Height = Units.InchesToDocumentsF(0.2f);
            //    table.Rows[i].HeightType=HeightType.Auto;
            //}

            //table.EndUpdate();

            ParagraphProperties pp = document.BeginUpdateParagraphs(table.FirstRow.Range);
            pp.Alignment = ParagraphAlignment.Center;
            pp.SpacingBefore = 0;
            pp.SpacingAfter = 0;
            document.EndUpdateParagraphs(pp);

            CharacterProperties cp = document.BeginUpdateCharacters(table.FirstRow.Range);
            cp.FontName = "Times New Roman";
            cp.FontSize = 12;
            cp.Bold = true;
            cp.ForeColor = Color.Black;
            document.EndUpdateCharacters(cp);

            // Fill table header with column names
            for (int i = 0; i < columnsToDisplay.Count; i++)
            {
                document.InsertText(table[0, i].Range.Start, columnsCaption[i]);
            }
            for (int i = 0; i < table.FirstRow.Cells.Count; i++)
            {
                table.FirstRow.Cells[i].BackgroundColor = Color.LightGray;
                table.FirstRow.Cells[i].VerticalAlignment = TableCellVerticalAlignment.Center;
            }

            // Fill table body with data
            if (dataTable.Rows.Count > 0)
            {
                table.ForEachCell(delegate (TableCell cell, int rowIndex, int cellIndex)
                {
                    if (rowIndex > 0)
                    {
                        document.InsertText(cell.Range.Start, dataTable.Rows[rowIndex - 1][columnsToDisplay[cellIndex]].ToString());
                        CharacterProperties cp = document.BeginUpdateCharacters(cell.Range);
                        cp.FontName = "Times New Roman";
                        cp.FontSize = 12;
                        cp.Bold = false;
                        cp.ForeColor = Color.Black;
                        document.EndUpdateCharacters(cp);
                    }
                });
            }
            //document.EndUpdate();

            //Y kien
            //document.BeginUpdate();
            //Paragraph para= document.Paragraphs.Append();
            range = document.AppendText("Ý kiến");
            document.EndUpdate();
            //range = document.Paragraphs[7].Range;
            titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 12;
            titleFormatting.Bold = true;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Left;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingBefore = Units.InchesToDocumentsF(0.2f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);

            DataTable dataTableyk = new("ykien");
            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ngay",
                Caption = "Ngày",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableyk.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Hoten",
                Caption = "Người gửi",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableyk.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ykien",
                Caption = "Ý kiến",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableyk.Columns.Add(dtColumn);

            XPCollection<Quatrinh> dsyk = new(session)
            {
                Criteria = CriteriaOperator.Parse("Phieu.Oid=?", phieu.Oid),
                Sorting = new SortingCollection(new SortProperty("Ngay", SortingDirection.Ascending))
            };
            if (dsyk.Count > 0)
            {
                foreach (Quatrinh p in dsyk)
                {
                    myDataRow = dataTableyk.NewRow();
                    myDataRow["Ngay"] = p.Ngay.ToString("dd/MM/yyyy HH:mm");
                    if (p.Nhanvien != null)
                        myDataRow["Hoten"] = p.Nhanvien.HoTen;
                    myDataRow["Ykien"] = p.Ykien;
                    dataTableyk.Rows.Add(myDataRow);
                }
            }
            dataTableRows = dataTableyk.Rows.Count;
            dataTableColumns = dataTableyk.Columns.Count;
            columnsToDisplay = new();
            columnsCaption = new();

            for (int i = 0; i < dataTableColumns; i++)
            {
                columnsToDisplay.Add(dataTableyk.Columns[i].ColumnName);
                columnsCaption.Add(dataTableyk.Columns[i].Caption);
            }

            document.BeginUpdate();

            Table tableyk = document.Tables.Create(document.Range.End, dataTableRows + 1, columnsToDisplay.Count, AutoFitBehaviorType.AutoFitToContents);
            //tableyk.BeginUpdate();
            //tableyk.TableLayout = TableLayoutType.Fixed;
            //tableyk.Borders.InsideHorizontalBorder.LineColor = Color.DarkBlue;
            //tableyk.Borders.InsideVerticalBorder.LineColor = Color.DarkBlue;
            //tableyk.Borders.InsideHorizontalBorder.LineThickness = 0.5f;
            //tableyk.Borders.InsideHorizontalBorder.LineStyle = TableBorderLineStyle.Single;
            //tableyk.Borders.InsideVerticalBorder.LineThickness = 0.5f;
            //tableyk.Borders.InsideVerticalBorder.LineStyle = TableBorderLineStyle.Single;
            //tableyk.LeftPadding = Units.InchesToDocumentsF(0.01f);
            tableyk.FirstRow.Height = Units.InchesToDocumentsF(0.1f);
            tableyk.FirstRow.HeightType = HeightType.Exact;

            tableyk.Rows[0].FirstCell.PreferredWidthType = WidthType.Fixed;
            tableyk.Rows[0].FirstCell.PreferredWidth = Units.InchesToDocumentsF(1.2f);


            //Set the second column width and cell height
            tableyk[0, 1].PreferredWidthType = WidthType.Fixed;
            tableyk[0, 1].PreferredWidth = Units.InchesToDocumentsF(1.2f);
            tableyk[0, 1].HeightType = HeightType.Exact;
            tableyk[0, 1].Height = Units.InchesToDocumentsF(0.5f);

            //Set the third column width 
            tableyk.Rows[0].LastCell.PreferredWidthType = WidthType.Fixed;
            tableyk.Rows[0].LastCell.PreferredWidth = Units.InchesToDocumentsF(5.2f);

            pp = document.BeginUpdateParagraphs(tableyk.FirstRow.Range);
            pp.Alignment = ParagraphAlignment.Center;
            pp.SpacingBefore = 0;
            pp.SpacingAfter = 0;
            document.EndUpdateParagraphs(pp);

            cp = document.BeginUpdateCharacters(tableyk.FirstRow.Range);
            cp.FontName = "Times New Roman";
            cp.FontSize = 12;
            cp.Bold = true;
            cp.ForeColor = Color.Black;
            document.EndUpdateCharacters(cp);

            // Fill table header with column names
            for (int i = 0; i < columnsToDisplay.Count; i++)
            {
                document.InsertText(tableyk[0, i].Range.Start, columnsCaption[i]);
            }
            for (int i = 0; i < tableyk.FirstRow.Cells.Count; i++)
            {
                tableyk.FirstRow.Cells[i].BackgroundColor = Color.LightGray;
                tableyk.FirstRow.Cells[i].VerticalAlignment = TableCellVerticalAlignment.Center;
            }

            // Fill table body with data
            if (dataTableyk.Rows.Count > 0)
            {
                tableyk.ForEachCell(delegate (TableCell cell, int rowIndex, int cellIndex)
                {
                    if (rowIndex > 0 && rowIndex <= dataTableyk.Rows.Count && cellIndex < columnsToDisplay.Count)
                    {
                        document.InsertText(cell.Range.Start, dataTableyk.Rows[rowIndex - 1][columnsToDisplay[cellIndex]].ToString());
                        CharacterProperties cp = document.BeginUpdateCharacters(cell.Range);
                        cp.FontName = "Times New Roman";
                        cp.FontSize = 12;
                        cp.Bold = false;
                        cp.ForeColor = Color.Black;
                        document.EndUpdateCharacters(cp);
                    }
                });
            }
            //document.EndUpdate();

            //Chia se
            //document.BeginUpdate();
            //Paragraph para= document.Paragraphs.Append();
            range = document.AppendText("Chia sẻ");
            document.EndUpdate();
            titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 12;
            titleFormatting.Bold = true;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Left;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingBefore = Units.InchesToDocumentsF(0.2f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);

            DataTable dataTableCS = new("chiase");
            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ngay",
                Caption = "Ngày",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableCS.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Hoten",
                Caption = "Người chia sẻ",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableCS.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Hoten2",
                Caption = "Ng. được chia sẻ",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableCS.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ghichu",
                Caption = "Ghi chú",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableCS.Columns.Add(dtColumn);

            XPCollection<Chiase> dscs = new(session)
            {
                Criteria = CriteriaOperator.Parse("Phieu.Oid=?", phieu.Oid),
                Sorting = new SortingCollection(new SortProperty("Ngay", SortingDirection.Ascending))
            };
            if (dscs.Count > 0)
            {
                foreach (Chiase p in dscs)
                {
                    myDataRow = dataTableCS.NewRow();
                    myDataRow["Ngay"] = p.Ngay.ToString("dd/MM/yyyy HH:mm");
                    if (p.NhanvienShare != null)
                        myDataRow["Hoten"] = p.NhanvienShare.HoTen;
                    if (p.NhanvienShared != null)
                        myDataRow["Hoten2"] = p.NhanvienShared.HoTen;
                    myDataRow["Ghichu"] = p.Ykien;
                    dataTableCS.Rows.Add(myDataRow);
                }
            }
            dataTableRows = dataTableCS.Rows.Count;
            dataTableColumns = dataTableCS.Columns.Count;
            columnsToDisplay = new();
            columnsCaption = new();

            for (int i = 0; i < dataTableColumns; i++)
            {
                columnsToDisplay.Add(dataTableCS.Columns[i].ColumnName);
                columnsCaption.Add(dataTableCS.Columns[i].Caption);
            }

            document.BeginUpdate();

            Table tableCS = document.Tables.Create(document.Range.End, dataTableRows + 1, columnsToDisplay.Count, AutoFitBehaviorType.AutoFitToContents);
            tableCS.FirstRow.Height = Units.InchesToDocumentsF(0.1f);
            tableCS.FirstRow.HeightType = HeightType.Exact;

            tableCS.Rows[0].FirstCell.PreferredWidthType = WidthType.Fixed;
            tableCS.Rows[0].FirstCell.PreferredWidth = Units.InchesToDocumentsF(1.2f);


            //Set the second column width and cell height
            tableCS[0, 1].PreferredWidthType = WidthType.Fixed;
            tableCS[0, 1].PreferredWidth = Units.InchesToDocumentsF(1.2f);
            tableCS[0, 1].HeightType = HeightType.Exact;
            tableCS[0, 1].Height = Units.InchesToDocumentsF(0.5f);

            tableCS[0, 2].PreferredWidthType = WidthType.Fixed;
            tableCS[0, 2].PreferredWidth = Units.InchesToDocumentsF(1.2f);
            tableCS[0, 2].HeightType = HeightType.Exact;
            tableCS[0, 2].Height = Units.InchesToDocumentsF(0.5f);

            //Set the third column width 
            tableCS.Rows[0].LastCell.PreferredWidthType = WidthType.Fixed;
            tableCS.Rows[0].LastCell.PreferredWidth = Units.InchesToDocumentsF(5.2f);

            pp = document.BeginUpdateParagraphs(tableCS.FirstRow.Range);
            pp.Alignment = ParagraphAlignment.Center;
            pp.SpacingBefore = 0;
            pp.SpacingAfter = 0;
            document.EndUpdateParagraphs(pp);

            cp = document.BeginUpdateCharacters(tableCS.FirstRow.Range);
            cp.FontName = "Times New Roman";
            cp.FontSize = 12;
            cp.Bold = true;
            cp.ForeColor = Color.Black;
            document.EndUpdateCharacters(cp);

            // Fill table header with column names
            for (int i = 0; i < columnsToDisplay.Count; i++)
            {
                document.InsertText(tableCS[0, i].Range.Start, columnsCaption[i]);
            }
            for (int i = 0; i < tableCS.FirstRow.Cells.Count; i++)
            {
                tableCS.FirstRow.Cells[i].BackgroundColor = Color.LightGray;
                tableCS.FirstRow.Cells[i].VerticalAlignment = TableCellVerticalAlignment.Center;
            }

            // Fill table body with data
            if (dataTableCS.Rows.Count > 0)
            {
                tableCS.ForEachCell(delegate (TableCell cell, int rowIndex, int cellIndex)
                {
                    if (rowIndex > 0 && rowIndex <= dataTableCS.Rows.Count && cellIndex < columnsToDisplay.Count)
                    {
                        document.InsertText(cell.Range.Start, dataTableCS.Rows[rowIndex - 1][columnsToDisplay[cellIndex]].ToString());
                        CharacterProperties cp = document.BeginUpdateCharacters(cell.Range);
                        cp.FontName = "Times New Roman";
                        cp.FontSize = 12;
                        cp.Bold = false;
                        cp.ForeColor = Color.Black;
                        document.EndUpdateCharacters(cp);
                    }
                });
            }
            //document.EndUpdate();

            //Qua trinh duyet
            //document.BeginUpdate();
            range = document.AppendText("Quá trình duyệt");
            document.EndUpdate();
            titleFormatting = document.BeginUpdateCharacters(range);
            //Set the character size, font name and color
            titleFormatting.FontSize = 12;
            titleFormatting.Bold = true;
            titleFormatting.FontName = "Times New Roman";
            titleFormatting.ForeColor = Color.DarkBlue;
            document.EndUpdateCharacters(titleFormatting);

            titleParagraphFormatting = document.BeginUpdateParagraphs(range);
            titleParagraphFormatting.Alignment = ParagraphAlignment.Left;
            //titleParagraphFormatting.LeftIndent = Units.InchesToDocumentsF(0.5f);
            titleParagraphFormatting.SpacingBefore = Units.InchesToDocumentsF(0.2f);
            titleParagraphFormatting.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            document.EndUpdateParagraphs(titleParagraphFormatting);

            DataTable dataTableD = new("duyet");
            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Buoc",
                Caption = "Bước",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Hoten",
                Caption = "Người duyệt",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Han",
                Caption = "Hạn duyệt",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ngay",
                Caption = "Ngày duyệt",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ketluan",
                Caption = "Kết luận",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            dtColumn = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Ghichu",
                Caption = "Ghi chú",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = false
            };
            dataTableD.Columns.Add(dtColumn);

            XPCollection<DuyetDexuat> dsd = new(session)
            {
                Criteria = CriteriaOperator.Parse("Phieu.Oid=?", phieu.Oid),
                Sorting = new SortingCollection(new SortProperty("SoTT", SortingDirection.Ascending))
            };
            if (dsd.Count > 0)
            {
                foreach (DuyetDexuat p in dsd)
                {
                    myDataRow = dataTableD.NewRow();
                    myDataRow["Buoc"] = p.SoTT;
                    if (p.Nguoiduyet != null)
                    {
                        if (p.NguoiUQ != null)
                            myDataRow["Hoten"] = p.Nguoiduyet.HoTen + "\n" + p.NguoiUQ.HoTen;
                        else
                            myDataRow["Hoten"] = p.Nguoiduyet.HoTen;
                    }

                    myDataRow["Han"] = p.Handuyet.ToString("dd/MM/yyyy");
                    if (p.Ngayduyet != DateTime.MinValue)
                    {
                        myDataRow["Ngay"] = p.Ngayduyet.ToString("dd/MM/yyyy HH:mm");
                    }
                    if (p.Trangthai == EStatusDuyet.huy)
                    {
                        if (p.DuyetUQ)
                            myDataRow["Ketluan"] = "Hủy (UQ)";
                        else
                            myDataRow["Ketluan"] = "Hủy";
                    }
                    else if (p.Trangthai == EStatusDuyet.daduyet)
                    {
                        if (p.DuyetUQ)
                            myDataRow["Ketluan"] = "Đồng ý (UQ)";
                        else
                            myDataRow["Ketluan"] = "Đồng ý";
                    }
                    else
                    {
                        if (p.Boqua)
                            myDataRow["Ketluan"] = "Bỏ qua bước này";
                        else
                            myDataRow["Ketluan"] = "Chờ duyệt";
                    }

                    myDataRow["Ghichu"] = p.Ghichu;
                    dataTableD.Rows.Add(myDataRow);
                }
            }
            dataTableRows = dataTableD.Rows.Count;
            dataTableColumns = dataTableD.Columns.Count;
            columnsToDisplay = new();
            columnsCaption = new();

            for (int i = 0; i < dataTableColumns; i++)
            {
                columnsToDisplay.Add(dataTableD.Columns[i].ColumnName);
                columnsCaption.Add(dataTableD.Columns[i].Caption);
            }

            //document.BeginUpdate();

            Table tabledy = document.Tables.Create(document.Range.End, dataTableRows + 1, columnsToDisplay.Count, AutoFitBehaviorType.AutoFitToContents);
            tabledy.FirstRow.Height = Units.InchesToDocumentsF(0.2f);
            tabledy.FirstRow.HeightType = HeightType.Auto;

            //tabledy.Rows[0].FirstCell.PreferredWidthType = WidthType.Fixed;
            //tabledy.Rows[0].FirstCell.PreferredWidth = Units.InchesToDocumentsF(0.2f);


            //Set the second column width and cell height
            //tabledy[0, 1].PreferredWidthType = WidthType.Fixed;
            //tabledy[0, 1].PreferredWidth = Units.InchesToDocumentsF(0.5f);
            //tabledy[0, 1].HeightType = HeightType.Exact;
            //tabledy[0, 1].Height = Units.InchesToDocumentsF(0.5f);

            ////Set the third column width 
            //tabledy.Rows[0].LastCell.PreferredWidthType = WidthType.Fixed;
            //tabledy.Rows[0].LastCell.PreferredWidth = Units.InchesToDocumentsF(1.2f);

            pp = document.BeginUpdateParagraphs(tabledy.FirstRow.Range);
            pp.Alignment = ParagraphAlignment.Center;
            pp.SpacingBefore = 0;
            pp.SpacingAfter = 0;
            document.EndUpdateParagraphs(pp);

            cp = document.BeginUpdateCharacters(tabledy.FirstRow.Range);
            cp.FontName = "Times New Roman";
            cp.FontSize = 12;
            cp.Bold = true;
            cp.ForeColor = Color.Black;
            document.EndUpdateCharacters(cp);


            // Fill table header with column names
            for (int i = 0; i < columnsToDisplay.Count; i++)
            {
                document.InsertText(tabledy[0, i].Range.Start, columnsCaption[i]);
            }
            for (int i = 0; i < tabledy.FirstRow.Cells.Count; i++)
            {
                tabledy.FirstRow.Cells[i].BackgroundColor = Color.LightGray;
                tabledy.FirstRow.Cells[i].VerticalAlignment = TableCellVerticalAlignment.Center;
            }

            // Fill table body with data
            if (dataTableD.Rows.Count > 0)
            {
                tabledy.ForEachCell(delegate (TableCell cell, int rowIndex, int cellIndex)
                {
                    if (rowIndex > 0 && rowIndex <= dataTableD.Rows.Count && cellIndex < columnsToDisplay.Count)
                    {
                        document.InsertText(cell.Range.Start, dataTableD.Rows[rowIndex - 1][columnsToDisplay[cellIndex]].ToString());
                        CharacterProperties cp = document.BeginUpdateCharacters(cell.Range);
                        cp.FontName = "Times New Roman";
                        cp.FontSize = 12;
                        cp.Bold = false;
                        cp.ForeColor = Color.Black;
                        document.EndUpdateCharacters(cp);
                    }
                });
            }
            document.EndUpdate();

            // Specify export options.
            PdfExportOptions options = new();
            options.DocumentOptions.Author = phieudexuat.Nhanvien.HoTen;
            options.Compressed = false;
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.ShowPrintDialogOnOpen = true;
            // Export the document to a stream.
            string fName = phieudexuat.Oid.ToString() + "_Nhatky.pdf";
            string tepurl = @"Files\" + phieudexuat.Nhanvien.Sothe + @"\" + fName;
            string path = Path.Combine(BCom.webRootPath, @"Files\" + phieudexuat.Nhanvien.Sothe);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string combined = Path.Combine(path, fName);
            if (System.IO.File.Exists(combined))
                System.IO.File.Delete(combined);

            using (FileStream pdfFileStream = new(combined, FileMode.Create))
            {
                KyProcessor.ExportToPdf(pdfFileStream, options);
            }

            PhieudexuatFile kem = objectSpace.FindObject<PhieudexuatFile>(CriteriaOperator.Parse("Phieu=? && Loai=?", phieudexuat, 2));
            if (kem == null)
            {
                kem = objectSpace.CreateObject<PhieudexuatFile>();
                kem.Phieu = phieudexuat;
                kem.Tailieu = "Nhật ký duyệt đề xuất số " + phieu.Oid.ToString();
                kem.TepTen = fName;
                kem.TepUrl = tepurl;
                kem.Loai = 2;
                kem.Save();
                phieudexuat.PhieudexuatFiles.Add(kem);
                objectSpace.CommitChanges();
            }
            else
            {
                kem.TepTen = fName;
                kem.TepUrl = tepurl;
                kem.Save();
                objectSpace.CommitChanges();
            }
        }
    }
}
