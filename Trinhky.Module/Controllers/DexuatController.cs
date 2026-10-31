using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Drawing;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class DexuatController: ViewController
    {
        SimpleAction lapdexuat;
        public DexuatController() 
        {
            TargetViewId = "Phieudexuat_DetailView";
            lapdexuat = new(this, "lapdexuat", "View")
            {
                Caption="Lập phiếu",
                ImageName= "lapphieu",
                ToolTip="Lập phiếu đề xuất theo mẫu",
                TargetViewId = "Phieudexuat_DetailView",
                ConfirmationMessage = "Chắc chắn lập phiếu đề xuất theo mẫu?"
            };
            lapdexuat.Execute += Lapdexuat_Execute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CurrentObject is Phieudexuat phieu)
            {
                if (phieu.Oid > 0)
                    lapdexuat.Active.SetItemValue("thay", false);
                else
                    lapdexuat.Active.SetItemValue("thay", true);
            }

        }
        private void Lapdexuat_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is Phieudexuat phieu)
            {
                if (phieu.Mau == null)
                {
                    TCom.CustomError("Cần chọn mẫu");
                    return;
                }
                if (phieu.IsCoso)
                {
                    if (phieu.CS == null)
                    {
                        TCom.CustomError("Cần chọn cơ sở trực tiếp");
                        return;
                    }
                }
                else
                {
                    if (phieu.Donvi == null)
                    {
                        TCom.CustomError("Cần chọn bộ phận nhận trực tiếp");
                        return;
                    }
                }
                if (phieu.Trangthai != TCom.EStatusVB.choduyet)
                {
                    TCom.CustomError("Đề xuất đã được xử lý, không tạo lại được");
                    return;
                }
                if (phieu.Mau.Chucvuxuly != null)
                {
                    Nhansu nxl= ObjectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)));
                    if(nxl != null)
                        phieu.NguoiXuly=nxl;
                }
                var richServer = new RichEditDocumentServer();
                richServer.Document.RtfText = phieu.Mau.Noidung;
                //Thong tin nguoi lap
                if (phieu.Nhanvien != null)
                {
                    string hoten = phieu.Nhanvien.HoTen;
                    string donvi = "";
                    if (phieu.Nhanvien.Donvi!=null)
                        donvi = phieu.Nhanvien.Donvi.Name;
                    string tencs = "";
                    if(phieu.Nhanvien.CS!=null)
                    {
                        tencs = phieu.Nhanvien.CS.TenCoso;
                    }
                    richServer.Document.ReplaceAll("{Hoten}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{Donvi}", donvi, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{CS}", tencs, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (phieu.Nhanvien.AnhCK != null)
                    {
                        using var ms = new MemoryStream(phieu.Nhanvien.AnhCK);
                        Image img = Image.FromStream(ms);
                        DocumentRange[] ranges = richServer.Document.FindAll("{KyHoten}", SearchOptions.None, richServer.Document.Range);
                        if (ranges.Length > 0)
                        {
                            DocumentRange range = ranges[0];
                            DocumentPosition startPosition = range.Start;
                            richServer.Document.Delete(range);
                            richServer.Document.Images.Insert(startPosition, DocumentImageSource.FromImage(img));
                        }
                    }
                }
                //Ngay thang nam
                if(phieu.Ngay!=DateTime.MinValue)
                {
                    string ngay = phieu.Ngay.Day.ToString();
                    string thang = phieu.Ngay.Month.ToString();
                    string nam = phieu.Ngay.Year.ToString("####");
                    richServer.Document.ReplaceAll("{dd}", ngay, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{mm}", thang, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{yy}", nam, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                //them cac chu ky 
                Session session = ((XPObjectSpace)ObjectSpace).Session;
                Coso coso = phieu.CS;
                if (coso != null)
                {
                    //Co so
                    XPCollection<Chucdanh> chucdanhcosos = new(session)
                    {
                        Criteria = CriteriaOperator.Parse("TheoCoso=?", true)
                    };
                    foreach (Chucdanh item in chucdanhcosos)
                    {
                        string ma = item.Code;
                        if (!string.IsNullOrEmpty(ma))
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And, 
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item)),
                                CriteriaOperator.Parse("CS=?", coso));
                            Nhansu nscs = ObjectSpace.FindObject<Nhansu>(criteria);
                            if (nscs != null)
                            {
                                string kyten = nscs.HoTen;
                                richServer.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                            }
                        }
                    }
                }
                //theo bo phan
                XPCollection<Chucdanh> chucdanhs=new(session)
                {
                    Criteria = CriteriaOperator.Parse("TheoCoso=?", false)
                };
                foreach (Chucdanh item in chucdanhs)
                {
                    string ma = item.Code;
                    if(!string.IsNullOrEmpty(ma))
                    {
                        CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                            new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item)),
                            CriteriaOperator.Parse("Donvi=?", phieu.Donvi));
                        Nhansu ns = ObjectSpace.FindObject<Nhansu>(criteria);
                        ns ??= ObjectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item)));
                        if (ns!=null)
                        {
                            string kyten = ns.HoTen;
                            richServer.Document.ReplaceAll(ma, kyten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        }
                    }
                }

                phieu.Noidung = richServer.Document.RtfText;
                // qua trinh duyet
                foreach(QuitrinhChuan item in phieu.Mau.QuitrinhChuans)
                {
                    if(item.Chucvu!=null)
                    {
                        DuyetDexuat duyet = ObjectSpace.CreateObject<DuyetDexuat>();
                        duyet.Phieu = phieu;
                        duyet.SoTT = item.SoTT;
                        if (item.Chucvu.TheoCoso && coso!=null)
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                CriteriaOperator.Parse("CS=?", coso));

                            Nhansu nscs = ObjectSpace.FindObject<Nhansu>(criteria);
                            if (nscs != null) 
                            { 
                                duyet.Nguoiduyet = nscs;
                                duyet.Maduyet = item.Chucvu.Code;
                                duyet.MaChuky=item.Chucvu.CodeCK;
                            }
                            if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
                            {
                                duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
                            }
                            duyet.Save();
                        }
                        else
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                CriteriaOperator.Parse("Donvi=?", phieu.Donvi));
                            Nhansu ns = ObjectSpace.FindObject<Nhansu>(criteria);
                            ns ??= ObjectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)));
                            if (ns != null) 
                            { 
                                duyet.Nguoiduyet = ns;
                                duyet.Maduyet = item.Chucvu.Code;
                                duyet.MaChuky = item.Chucvu.CodeCK;
                            }
                            if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
                            {
                                duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
                            }
                            duyet.Save();
                        }

                    }
                }
                phieu.Save();
                ObjectSpace.CommitChanges();
                richServer.Document.ReplaceAll("{so}", phieu.Oid.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                phieu.Noidung = richServer.Document.RtfText;
                ObjectSpace.CommitChanges();

            }
        }
    }
}
