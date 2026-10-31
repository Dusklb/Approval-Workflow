using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraRichEdit;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Trinhky.Module.BusinessObjects
{
    public static class TCom
    {
#pragma warning disable CA2211
        //license
        //public static string eKey = "TrungMinh@2021";
        //public static int DonviID = 0;
        public static string TenDV = "tmSales";
        public static string Diachi = "Nguyễn Tri Phương, Đà Nẵng";
        public static string Dienthoai = "0916.544.455";
        public static string Email = "";
        public static int CurrentBophanID;
        public static bool bAdmin;

        public static string defEmail;
        public static string defEmailName;
        public static string defPassword;
        public static string defChuky;
        public static string testMail = "";// "vietintel.vn@gmail.com";
        public enum EStatusDuyet
        {
            [XafDisplayName("Chờ xử lý")]
            choduyet = 0,
            [XafDisplayName("Không đồng ý")]
            huy = 1,//chuyen vb sang readonly
            [XafDisplayName("Đồng ý")]
            daduyet = 2,
        }
        public enum EStatusVB
        {
            [XafDisplayName("Chờ duyệt")]
            choduyet = 0,
            [XafDisplayName("Hủy")]
            dahuy = 1,
            [XafDisplayName("Đang duyệt")]
            dangduyet = 2,
            [XafDisplayName("Đã duyệt")]
            daduyet = 3,
        }
        //public const string ProgressPropertyAlias = "ProgressProperty";
        public enum EStatusXL//xu ly sau duyet
        {
            [XafDisplayName("Chưa xử lý")]
            xlchua = 0,
            [XafDisplayName("Đang xử lý")]
            xldang = 1,
            [XafDisplayName("Đã xử lý")]
            xlxong = 2
        }

        public static List<UserEnvironment> UserEnvironments = new();

        public static void CustomInfo(string msg, bool bDone = false)
        {
            XafApplication app = ApplicationHelper.Instance.Application;
            if (bDone)
                app.ShowViewStrategy.ShowMessage("Đã hoàn thành tác vụ", InformationType.Success);
            else
                app.ShowViewStrategy.ShowMessage(msg, InformationType.Success);
        }
        public static void CustomError(string msg)
        {
            XafApplication app = ApplicationHelper.Instance.Application;
            app.ShowViewStrategy.ShowMessage(msg, InformationType.Error);
        }

        public static Nhansu GetCurrentNhanvien()
        {
            Nhansu nv = null;
            XafApplication app = ApplicationHelper.Instance.Application;
            IObjectSpace os = app.CreateObjectSpace(typeof(ApplicationUser));
            ApplicationUser CurentUser = os.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                if (!string.IsNullOrEmpty(CurentUser.UserName))
                {
                    nv = os.FindObject<Nhansu>(CriteriaOperator.Parse("Email=?", CurentUser.UserName));
                }
            }
            //os.Dispose();
            return nv;
        }
        public static string GetNhanvienMail()
        {
            string mail = "";
            XafApplication app = ApplicationHelper.Instance.Application;
            using IObjectSpace os = app.CreateObjectSpace(typeof(ApplicationUser));
            ApplicationUser CurentUser = os.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                mail = CurentUser.UserName;
                if (CurentUser.UserName.ToLower() == "admin") bAdmin = true;
            }
            return mail;
        }

        public static DateTime GetServerDateTime()
        {
            XafApplication app = ApplicationHelper.Instance.Application;
            using IObjectSpace objectSpace = app.CreateObjectSpace(typeof(ApplicationUser));
            using Session session = ((XPObjectSpace)objectSpace).Session;
            string sql = "SELECT GETDATE();";
            DateTime timeCurrent = (DateTime)session.ExecuteScalar(sql);
            return timeCurrent;
        }

        public static bool IsAdmin()
        {
            bool bAdmin = false;
            XafApplication app = ApplicationHelper.Instance.Application;
            IObjectSpace os = app.CreateObjectSpace(typeof(ApplicationUser));
            ApplicationUser CurentUser = os.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                bAdmin = CurentUser.Roles.Any(r => r.IsAdministrative);
            }
            os.Dispose();
            return bAdmin;
        }

        public static void SetAllowEdit(DetailView view, string itemID, bool bAllowEdit)
        {
            PropertyEditor item = (PropertyEditor)view.FindItem(itemID);
            if (item != null)
                item.AllowEdit["AllowEdit"] = bAllowEdit;
        }
        public static void SetAllowView(DetailView view, string itemID, bool bVisible)
        {
            ViewItem item = view.FindItem(itemID);
            if (item is IAppearanceVisibility visibility)
            {
                if (bVisible)
                    visibility.Visibility = ViewItemVisibility.Show;
                else
                    visibility.Visibility = ViewItemVisibility.Hide;
            }
        }

        public static IBindingList GetDanhsachDaDuyet(IObjectSpace objectSpace)
        {
            BindingList<PhieuDaduyet> objects = new();

            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                if (nv.ChucvuNVs.Count > 0)
                {
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //string sql = @"select ds.OID, ds.Phieu, ds.SoTT, ds.Ngayduyet
                    //from 
                    //(SELECT Oid, Phieu, SoTT, Trangthai, Nguoiduyet, Ngayduyet, ROW_NUMBER() OVER(PARTITION BY Phieu ORDER BY Ngayduyet DESC) AS row_number
                    //  FROM DuyetDexuat WHERE Nguoiduyet=" + nv.Oid + " or NguoiUQ=" + nv.Oid + ") as ds " +
                    //  " WHERE row_number = 1 and Trangthai=2 order by Ngayduyet";
                    //SelectedData results = session.ExecuteQuery(sql);

                    SelectedData results = session.ExecuteSprocParametrized("procDaduyet", new SprocParameter("@NhanvienID", nv.Oid));
                    int tt = 0;
                    foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                    {
                        int duyetid = tmLib.CommonLib.CInt(row.Values[0]);
                        //int sophieu = tmLib.CommonLib.CInt(row.Values[1]);
                        //int sott = tmLib.CommonLib.CInt(row.Values[2]);
                        DuyetDexuat d = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Oid=?", duyetid));
                        if (d != null)
                        {
                            tt++;
                            PhieuDaduyet obj = new()
                            {
                                TT = tt,
                                Oid = d.Phieu.Oid,
                                TenDexuat = d.Phieu.Mau.Kyhieu + "-" + d.Phieu.Oid.ToString() + "-" + d.Phieu.TenDexuat,
                                Ngay = d.Phieu.Ngay,
                                Trangthai = d.Phieu.Trangthai,
                            };
                            if (row.Values[3] != null)
                                obj.NgayDuyet = (DateTime)row.Values[3];
                            if (d.Phieu.Nhanvien != null)
                            {
                                obj.Nguoi = d.Phieu.Nhanvien.HoTen;
                                if (d.Phieu.Nhanvien.Donvi != null)
                                    obj.Bophan = d.Phieu.Nhanvien.Donvi.Name;
                            }
                            if (d.Phieu.Chuthe != null)
                                obj.Doituong = d.Phieu.Chuthe.HoTen;
                            objects.Add(obj);
                        }
                    }
                }
            }
            return objects;
        }

        public static IBindingList GetDanhsachDaHuy(IObjectSpace objectSpace)
        {
            BindingList<PhieuDahuy> objects = new();

            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                if (nv.ChucvuNVs.Count > 0)
                {
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //string sql = @"select ds.OID, ds.Phieu, ds.SoTT, ds.Ngayduyet
                    //from 
                    //(SELECT Oid, Phieu, SoTT, Trangthai, Nguoiduyet, Ngayduyet, ROW_NUMBER() OVER(PARTITION BY Phieu ORDER BY Ngayduyet DESC) AS row_number
                    //  FROM DuyetDexuat WHERE Nguoiduyet=" + nv.Oid + " or NguoiUQ=" + nv.Oid + ") as ds " +
                    //  " WHERE row_number = 1 and Trangthai=1 order by Ngayduyet";
                    //SelectedData results = session.ExecuteQuery(sql);
                    int tt = 0;
                    SelectedData results = session.ExecuteSprocParametrized("procDahuy", new SprocParameter("@NhanvienID", nv.Oid));
                    foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                    {
                        int duyetid = tmLib.CommonLib.CInt(row.Values[0]);
                        //int sophieu = tmLib.CommonLib.CInt(row.Values[1]);
                        //int sott = tmLib.CommonLib.CInt(row.Values[2]);
                        DuyetDexuat d = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Oid=?", duyetid));
                        if (d != null)
                        {
                            tt++;
                            PhieuDahuy obj = new()
                            {
                                TT = tt,
                                Oid = d.Phieu.Oid,
                                TenDexuat = d.Phieu.Mau.Kyhieu + "-" + d.Phieu.Oid.ToString() + "-" + d.Phieu.TenDexuat,
                                Ngay = d.Phieu.Ngay,
                            };
                            if (row.Values[3] != null)
                                obj.NgayDuyet = (DateTime)row.Values[3];
                            if (d.Phieu.Nhanvien != null)
                            {
                                obj.Nguoi = d.Phieu.Nhanvien.HoTen;
                                if (d.Phieu.Nhanvien.Donvi != null)
                                    obj.Bophan = d.Phieu.Nhanvien.Donvi.Name;
                            }
                            if (d.Phieu.Chuthe != null)
                                obj.Doituong = d.Phieu.Chuthe.HoTen;

                            objects.Add(obj);
                        }
                    }
                }
            }
            return objects;
        }

        public static int SodaDuyet()
        {
            int so = -1;
            Nhansu nv = GetCurrentNhanvien();
            if (nv != null && nv.ChucvuNVs.Count > 0)
            {
                int nvID = nv.Oid;
                string sql = @"select Count(ds.Phieu) as So 
                from 
                (SELECT Phieu, SoTT, Trangthai, Nguoiduyet, Ngayduyet, ROW_NUMBER() OVER(PARTITION BY Phieu ORDER BY Ngayduyet DESC) AS row_number
                FROM DuyetDexuat where Nguoiduyet=" + nvID + " or NguoiUQ=" + nvID + ") as ds  WHERE row_number = 1 and (Trangthai=2)";
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                Session session = ((XPObjectSpace)objectSpace).Session;
                var ret = session.ExecuteScalar(sql);
                so = tmLib.CommonLib.CInt(ret);
            }
            return so;
        }

        public static int SodaHuy()
        {
            int so = -1;
            Nhansu nv = GetCurrentNhanvien();
            if (nv != null && nv.ChucvuNVs.Count > 0)
            {
                int nvID = nv.Oid;
                string sql = @"select Count(ds.Phieu) as So 
                from 
                (SELECT Phieu, SoTT, Trangthai, Nguoiduyet, Ngayduyet, ROW_NUMBER() OVER(PARTITION BY Phieu ORDER BY Ngayduyet DESC) AS row_number
                FROM DuyetDexuat where Nguoiduyet=" + nvID + " or NguoiUQ=" + nvID + ") as ds WHERE row_number = 1 and (Trangthai=1)";
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                Session session = ((XPObjectSpace)objectSpace).Session;
                var ret = session.ExecuteScalar(sql);
                so = tmLib.CommonLib.CInt(ret);
            }
            return so;
        }
        public static string Soxuly()
        {
            string temp = "-1";
            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)// && nv.ChucvuNVs.Count > 0)
            {
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                Session session = ((XPObjectSpace)objectSpace).Session;
                int nvID = nv.Oid;
                string sql = @"select COUNT(Oid) from Phieudexuat where (Trangthai=3) and (Nguoixuly=" + nvID + " or Nguoithuchien=" + nvID + ")";
                var ret = session.ExecuteScalar(sql);
                int tong = tmLib.CommonLib.CInt(ret);

                sql = @"select COUNT(Oid) from Phieudexuat where (Trangthai=3) and (daxong=0) and (Nguoixuly=" + nvID + " or Nguoithuchien=" + nvID + ")";
                var ret1 = session.ExecuteScalar(sql);
                int chuaxl = tmLib.CommonLib.CInt(ret1);
                if (tong > 0)
                    temp = chuaxl.ToString() + "/" + tong.ToString();
                //else
                //    temp = "";
            }

            return temp;
        }
        public static IBindingList GetDanhsachXuly(IObjectSpace objectSpace)
        {
            BindingList<PhieuXuly> objects = new();

            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                //                if (nv.ChucvuNVs.Count > 0)
                //                {
                Nhansu nxl = objectSpace.GetObject(nv);
                Session session = ((XPObjectSpace)objectSpace).Session;
                XPCollection<Phieudexuat> ds = new(session)
                {
                    Criteria = CriteriaOperator.Parse("(NguoiXuly=? or Nguoithuchien=?) and Trangthai=?", nxl, nxl, EStatusVB.daduyet),
                    Sorting = new SortingCollection(new SortProperty("Ngay", SortingDirection.Descending))
                };

                int tt = 0;
                foreach (Phieudexuat p in ds)
                {
                    tt++;
                    PhieuXuly obj = new()
                    {
                        TT = tt,
                        Oid = p.Oid,
                        TenDexuat = p.Mau.Kyhieu + "-" + p.Oid.ToString() + "-" + p.TenDexuat,
                        Ngay = p.Ngay,
                        Daxong = p.Daxong,
                        NgayDuyet = p.Ngayduyet
                    };
                    if (p.Ngayxong != DateTime.MinValue)
                        obj.Ngayxong = p.Ngayxong.ToString("dd/MM/yyyy HH:mm");
                    if (p.Nhanvien != null)
                    {
                        obj.Nguoi = p.Nhanvien.HoTen;
                        if (p.Nhanvien.Donvi != null)
                            obj.Bophan = p.Nhanvien.Donvi.Name;
                    }
                    if (p.Nguoithuchien != null)
                        obj.Giaocho = p.Nguoithuchien.HoTen;
                    obj.Yeucau = p.Yeucau;
                    if (p.Chuthe != null)
                        obj.Doituong = p.Chuthe.HoTen;
                    if (p.TrangthaiXL == EStatusXL.xlxong)
                        obj.XLStatus = "Đã xử lý";
                    else if (p.TrangthaiXL == EStatusXL.xldang)
                        obj.XLStatus = "Đang xử lý";
                    else
                        obj.XLStatus = "Chưa xử lý";
                    objects.Add(obj);
                    //                  }
                }
            }
            return objects;
        }

        public static string Sochoduyet()
        {
            string ret = "";
            Nhansu nv = TCom.GetCurrentNhanvien();
            if (nv != null)
            {
                if (nv.ChucvuNVs.Count > 0)
                {
                    int Sodunghan = 0;
                    int Soquahan = 0;
                    int SodunghanUQ = 0;
                    int SoquahanUQ = 0;
                    DateTime homnay = GetServerDateTime();
                    //string sql = @"SELECT TOP (100) PERCENT dbo.DuyetDexuat.OID, dbo.DuyetDexuat.Phieu, dbo.DuyetDexuat.SoTT, dbo.DuyetDexuat.Handuyet, 
                    //dbo.DuyetDexuat.Ngayduyet, dbo.DuyetDexuat.NguoiUQ
                    //FROM     dbo.DuyetDexuat INNER JOIN
                    //dbo.Phieudexuat ON dbo.DuyetDexuat.Phieu = dbo.Phieudexuat.OID
                    //WHERE  ((dbo.Phieudexuat.Dagui = 1) AND (dbo.DuyetDexuat.Nguoiduyet = " + nv.Oid + ") AND (dbo.DuyetDexuat.Trangthai = 0) AND (dbo.DuyetDexuat.Boqua = 0)) OR " +
                    //" ((dbo.Phieudexuat.Dagui = 1) AND (dbo.DuyetDexuat.Trangthai = 0) AND (dbo.DuyetDexuat.Boqua = 0) AND (dbo.DuyetDexuat.NguoiUQ = " + nv.Oid + "))" +
                    //" ORDER BY dbo.DuyetDexuat.Phieu, dbo.DuyetDexuat.SoTT";
                    XafApplication app = ApplicationHelper.Instance.Application;
                    IObjectSpace objectSpace = app.CreateObjectSpace(typeof(Phieudexuat));
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //SelectedData results = session.ExecuteQuery(sql);
                    SelectedData results = session.ExecuteSprocParametrized("procSochoduyet", new SprocParameter("@NhanvienID", nv.Oid));
                    foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                    {
                        int sophieu = tmLib.CommonLib.CInt(row.Values[1]);
                        int sott = tmLib.CommonLib.CInt(row.Values[2]);
                        int uqid = tmLib.CommonLib.CInt(row.Values[5]);
                        DateTime handuyet = DateTime.MinValue;
                        if (row.Values[3] != null)
                            handuyet = tmLib.CommonLib.CDate(row.Values[3]);
                        //DateTime ngayduyet = TCom.GetServerDateTime();
                        //if (row.Values[4] != null)
                        //    ngayduyet = tmLib.CommonLib.CDate(row.Values[4]);
                        if (sott == 1)
                        {
                            if (uqid == 0)
                            {
                                if (handuyet == DateTime.MinValue)
                                    Sodunghan++;
                                else if (handuyet >= homnay)
                                    Sodunghan++;
                                else
                                    Soquahan++;
                            }
                            else
                            {
                                if (handuyet == DateTime.MinValue)
                                    SodunghanUQ++;
                                else if (handuyet >= homnay)
                                    SodunghanUQ++;
                                else
                                    SoquahanUQ++;
                            }
                        }
                        else
                        {
                            //DuyetDexuat d = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("SoTT=? && Phieu=?", sott - 1, sophieu));
                            DuyetDexuat d = BuocTruoc(sott, sophieu, objectSpace);
                            if (d != null)
                            {
                                if (d.Trangthai == EStatusDuyet.daduyet)
                                {
                                    if (uqid == 0)
                                    {
                                        if (handuyet == DateTime.MinValue)
                                            Sodunghan++;
                                        else if (handuyet >= homnay)
                                            Sodunghan++;
                                        else
                                            Soquahan++;
                                    }
                                    else
                                    {
                                        if (handuyet == DateTime.MinValue)
                                            SodunghanUQ++;
                                        else if (handuyet >= homnay)
                                            SodunghanUQ++;
                                        else
                                            SoquahanUQ++;

                                    }
                                }
                            }
                        }
                    }
                    ret = Sodunghan + "/" + Soquahan;
                    if (SodunghanUQ > 0 || SoquahanUQ > 0)
                        ret += "-" + SodunghanUQ + "/" + SoquahanUQ;
                }

            }
            return ret;
        }

        public static IBindingList GetDanhsachDuyet(IObjectSpace objectSpace)
        {
            BindingList<Phieuduyet> objects = new();

            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                if (nv.ChucvuNVs.Count > 0)
                {
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //                    string sql1 = @"SELECT DuyetDexuat.Oid, DuyetDexuat.Phieu, DuyetDexuat.SoTT, DuyetDexuat.Handuyet, DuyetDexuat.Ngayduyet FROM DuyetDexuat 
                    //                    Inner Join Phieudexuat on DuyetDexuat.Phieu=Phieudexuat.Oid WHERE (Phieudexuat.Dagui=1) and (DuyetDexuat.Nguoiduyet=" + nv.Oid +
                    //") and (DuyetDexuat.Trangthai=0) order by DuyetDexuat.Phieu, DuyetDexuat.SoTT";

                    //string sql = @"SELECT DuyetDexuat.Oid, DuyetDexuat.Phieu, DuyetDexuat.SoTT, DuyetDexuat.Trangthai, DuyetDexuat.Nguoiduyet, DuyetDexuat.Ngayduyet 
                    //FROM DuyetDexuat Inner Join Phieudexuat on DuyetDexuat.Phieu=Phieudexuat.Oid  
                    //WHERE ((Phieudexuat.Dagui=1) and (Nguoiduyet=" + nv.Oid + ") and (DuyetDexuat.Trangthai=0) AND (dbo.DuyetDexuat.Boqua = 0)) or " +
                    //" ((Phieudexuat.Dagui=1) and (NguoiUQ=" + nv.Oid + ") and (DuyetDexuat.Trangthai=0)  AND (dbo.DuyetDexuat.Boqua = 0)) order by DuyetDexuat.Phieu, DuyetDexuat.SoTT";
                    DateTime homnay = GetServerDateTime();
                    //SelectedData results = session.ExecuteQuery(sql);
                    int tt = 0;
                    SelectedData results = session.ExecuteSprocParametrized("procDanhsachduyet", new SprocParameter("@NhanvienID", nv.Oid));
                    foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                    {
                        int duyetid = tmLib.CommonLib.CInt(row.Values[0]);
                        int sophieu = tmLib.CommonLib.CInt(row.Values[1]);
                        int sott = tmLib.CommonLib.CInt(row.Values[2]);
                        if (sott == 1)
                        {
                            DuyetDexuat d = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Oid=?", duyetid));
                            if (d != null)
                            {
                                tt++;
                                Phieuduyet obj = new()
                                {
                                    TT = tt,
                                    Oid = d.Phieu.Oid,
                                    TenDexuat = d.Phieu.Mau.Kyhieu + "-" + d.Phieu.Oid.ToString() + "-" + d.Phieu.TenDexuat,
                                    Ngay = d.Phieu.Ngay,
                                    Han = d.Handuyet,
                                    //Trangthai = d.Phieu.Trangthai,
                                    //Tailieu = d.Phieu.PhieudexuatFiles.Count,
                                    Homnay = homnay,
                                    TyleDuyet = d.Phieu.TyleDuyet
                                };
                                if (d.Phieu.Nhanvien != null)
                                {
                                    obj.Nguoi = d.Phieu.Nhanvien.HoTen;
                                    if (d.Phieu.Nhanvien.Donvi != null)
                                        obj.Bophan = d.Phieu.Nhanvien.Donvi.Name;
                                }
                                if (d.Phieu.Chuthe != null)
                                    obj.Doituong = d.Phieu.Chuthe.HoTen;
                                objects.Add(obj);
                            }
                        }
                        else
                        {
                            //DuyetDexuat d = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("SoTT=? && Phieu=?", sott - 1, sophieu));
                            DuyetDexuat d = BuocTruoc(sott, sophieu, objectSpace);
                            if (d != null)
                            {
                                if (d.Trangthai == EStatusDuyet.daduyet)
                                {
                                    tt++;
                                    Phieuduyet obj = new()
                                    {
                                        TT = tt,
                                        Oid = d.Phieu.Oid,
                                        TenDexuat = d.Phieu.Mau.Kyhieu + "-" + d.Phieu.Oid.ToString() + "-" + d.Phieu.TenDexuat,
                                        Ngay = d.Phieu.Ngay,
                                        Han = d.Handuyet,
                                        //Trangthai = d.Phieu.Trangthai,
                                        //Tailieu = d.Phieu.PhieudexuatFiles.Count,
                                        Homnay = homnay,
                                        TyleDuyet = d.Phieu.TyleDuyet
                                    };
                                    if (d.Phieu.Nhanvien != null)
                                    {
                                        obj.Nguoi = d.Phieu.Nhanvien.HoTen;
                                        if (d.Phieu.Nhanvien.Donvi != null)
                                            obj.Bophan = d.Phieu.Nhanvien.Donvi.Name;
                                    }
                                    if (d.Phieu.Chuthe != null)
                                        obj.Doituong = d.Phieu.Chuthe.HoTen;
                                    objects.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            return objects;
        }

        public static IBindingList GetThongkeDexuat(IObjectSpace objectSpace)
        {
            BindingList<ThongkeDexuat> objects = new();
            string ngay = TCom.GetUserStringValue("tungay");
            _ = DateTime.TryParse(ngay, out DateTime fTungay);
            _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

            ngay = TCom.GetUserStringValue("denngay");
            _ = DateTime.TryParse(ngay, out DateTime fDenngay);
            _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);
            string rptngaydau = tmLib.CommonLib.ConvertDate(fTungay);
            string rptngaycuoi = tmLib.CommonLib.ConvertDate(fDenngay);

            Session session = ((XPObjectSpace)objectSpace).Session;
            string sql = @"SELECT DISTINCT TOP (100) PERCENT dbo.Phieudexuat.Nhanvien, dbo.Nhansu.Email
            FROM     dbo.Phieudexuat INNER JOIN
            dbo.Nhansu ON dbo.Phieudexuat.Nhanvien = dbo.Nhansu.OID
            WHERE  (Ngay >= CONVERT(DATETIME, '" + rptngaydau + "', 102)) " +
                        " AND (Ngay <= CONVERT(DATETIME, '" + rptngaycuoi + "', 102)) ORDER BY dbo.Nhansu.Email";

            SelectedData results = session.ExecuteQuery(sql);
            int tt = 0;
            foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
            {
                int nhanvienid = tmLib.CommonLib.CInt(row.Values[0]);
                string mail = tmLib.CommonLib.CString(row.Values[1]);
                Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nhanvienid));
                if (nv != null)
                {
                    tt++;
                    ThongkeDexuat obj = new()
                    {
                        TT = tt,
                        Oid = nhanvienid,
                        Hoten = nv.HoTen,
                    };
                    if (nv.Donvi != null)
                    {
                        obj.Donvi = nv.Donvi.Name;
                    }
                    string sql1 = @"SELECT COUNT(OID) AS SL, Trangthai FROM     dbo.Phieudexuat
                    WHERE  (dbo.Phieudexuat.Dagui=1) and (Nhanvien = " + nhanvienid + ") GROUP BY Trangthai";
                    SelectedData dxs = session.ExecuteQuery(sql1);
                    int tong = 0;
                    foreach (SelectStatementResultRow dx in dxs.ResultSet[0].Rows)
                    {
                        int sl = tmLib.CommonLib.CInt(dx.Values[0]);
                        int status = tmLib.CommonLib.CInt(dx.Values[1]);
                        if (status == 3)
                            obj.Daduyet = sl;
                        else if (status < 1)
                            obj.Chuaduyet = sl;
                        else if (status == 2)
                            obj.Dangduyet = sl;
                        else if (status == 1)
                            obj.Huy = sl;

                        tong += sl;
                    }
                    obj.Sodx = tong;
                    string sql2 = @"SELECT COUNT(OID) AS SL FROM     dbo.Phieudexuat
                    WHERE  (Nhanvien = " + nhanvienid + ") AND (Daxong = 1)";
                    var so = session.ExecuteScalar(sql2);
                    obj.Daxong = tmLib.CommonLib.CInt(so);

                    objects.Add(obj);
                }
            }
            return objects;
        }

        public static IBindingList GetThongkeDuyet(IObjectSpace objectSpace)//urgen phieu da huy
        {
            BindingList<ThongkeDuyet> objects = new();
            string ngay1 = TCom.GetUserStringValue("tungay");
            _ = DateTime.TryParse(ngay1, out DateTime fTungay);
            _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

            ngay1 = TCom.GetUserStringValue("denngay");
            _ = DateTime.TryParse(ngay1, out DateTime fDenngay);
            _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);
            //string rptngaydau = tmLib.CommonLib.ConvertDate(fTungay);
            //string rptngaycuoi = tmLib.CommonLib.ConvertDate(fDenngay);
            DateTime homnay = GetServerDateTime();

            Session session = ((XPObjectSpace)objectSpace).Session;
            string sql = @"SELECT DISTINCT TOP (100) PERCENT dbo.Nhansu.OID, dbo.Nhansu.Email
            FROM     dbo.DuyetDexuat INNER JOIN
            dbo.Nhansu ON dbo.DuyetDexuat.Nguoiduyet = dbo.Nhansu.OID
            ORDER BY dbo.Nhansu.Email";

            SelectedData results = session.ExecuteQuery(sql);
            int tt = 0;
            foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
            {
                int nhanvienid = tmLib.CommonLib.CInt(row.Values[0]);
                string mail = tmLib.CommonLib.CString(row.Values[1]);
                Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nhanvienid));
                if (nv != null)
                {
                    tt++;
                    ThongkeDuyet obj = new()
                    {
                        TT = tt,
                        Oid = nhanvienid,
                        Hoten = nv.HoTen,
                        Chucdanhky = nv.ListCV
                    };
                    XPCollection<DuyetDexuat> ds = new(session)
                    {
                        Criteria = CriteriaOperator.Parse("Nguoiduyet=? && Phieu.Ngay>=? && Phieu.Ngay<=? && Boqua=? && Phieu.Dagui=?", nhanvienid, fTungay, fDenngay, false, true)
                    };
                    int total = 0;
                    foreach (DuyetDexuat item in ds)
                    {
                        //DateTime ngayd = fDenngay;
                        //if (item.Ngayduyet != DateTime.MinValue)
                        //    ngayd = item.Ngayduyet;
                        if (item.Trangthai == EStatusDuyet.huy)
                        {
                            obj.Huy++;
                            total++;
                        }
                        else if (item.Trangthai == EStatusDuyet.daduyet)
                        {
                            if (item.Ngayduyet <= item.Handuyet)
                                obj.Dunghan++;
                            else
                                obj.Quahan++;
                            total++;
                        }
                        else
                        {
                            if (item.Phieu.Dagui)
                            {
                                if (item.SoTT == 1)
                                {
                                    if (homnay <= item.Handuyet)
                                        obj.Chuaduyet++;
                                    else
                                        obj.Chuaduyet2++;
                                    total++;
                                }
                                else
                                {
                                    //Phieudexuat p = objectSpace.GetObject(item.Phieu);
                                    //DuyetDexuat duyettruoc = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu=? && SoTT=?", item.Phieu, item.SoTT - 1));
                                    DuyetDexuat duyettruoc = BuocTruoc(item.SoTT, item.Phieu.Oid, objectSpace);
                                    if (duyettruoc != null)
                                    {
                                        if (duyettruoc.Trangthai == EStatusDuyet.daduyet)
                                        {
                                            if (homnay <= item.Handuyet)
                                                obj.Chuaduyet++;
                                            else
                                                obj.Chuaduyet2++;
                                            total++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    obj.Sodx = total;

                    //string sql1 = @"SELECT dbo.Phieudexuat.OID, dbo.DuyetDexuat.Trangthai, dbo.DuyetDexuat.Handuyet, dbo.DuyetDexuat.Ngayduyet
                    //FROM     dbo.Phieudexuat INNER JOIN
                    //dbo.DuyetDexuat ON dbo.Phieudexuat.OID = dbo.DuyetDexuat.Phieu
                    //WHERE  (dbo.DuyetDexuat.Nguoiduyet = " + nhanvienid + ")" +
                    //"  AND (dbo.DuyetDexuat.Ngayduyet >= CONVERT(DATETIME, '" + rptngaydau + "', 102)) AND(dbo.DuyetDexuat.Ngayduyet <= CONVERT(DATETIME, '" + rptngaycuoi + "', 102))";
                    //SelectedData dxs = session.ExecuteQuery(sql1);
                    //int tong = 0;
                    //foreach (SelectStatementResultRow dx in dxs.ResultSet[0].Rows)
                    //{
                    //    int status = tmLib.CommonLib.CInt(dx.Values[1]);
                    //    DateTime han = tmLib.CommonLib.CDate(dx.Values[2]);
                    //    DateTime ngay = tmLib.CommonLib.CDate(dx.Values[3]);
                    //    if (status == 1)
                    //        obj.Huy++;
                    //    else if (status == 2)
                    //    {
                    //        if (ngay <= han)
                    //            obj.Dunghan++;
                    //        else
                    //            obj.Quahan++;
                    //    }
                    //    else
                    //    {
                    //        if (ngay <= han)
                    //            obj.Chuaduyet++;
                    //        else
                    //            obj.Chuaduyet2++;
                    //    }

                    //    tong += 1;
                    //}
                    //obj.Sodx = tong;

                    objects.Add(obj);
                }
            }
            return objects;
        }

        public static IBindingList GetThongkeDexuatCanhan(IObjectSpace objectSpace)
        {
            BindingList<DexuatCanhan> objects = new();

            ApplicationUser CurentUser = objectSpace.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                if (!string.IsNullOrEmpty(CurentUser.UserName))
                {
                    Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Email=?", CurentUser.UserName));
                    if (nv != null)
                    {
                        string ngay = TCom.GetUserStringValue("tungay");
                        _ = DateTime.TryParse(ngay, out DateTime fTungay);
                        _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

                        ngay = TCom.GetUserStringValue("denngay");
                        _ = DateTime.TryParse(ngay, out DateTime fDenngay);
                        _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);
                        string rptngaydau = tmLib.CommonLib.ConvertDate(fTungay);
                        string rptngaycuoi = tmLib.CommonLib.ConvertDate(fDenngay);

                        Session session = ((XPObjectSpace)objectSpace).Session;
                        string sql1 = @"SELECT TOP (100) PERCENT COUNT(OID) AS SL, Trangthai FROM     dbo.Phieudexuat
                        WHERE  (Dagui=1) and (Nhanvien = " + nv.Oid + ") AND (Ngay >= CONVERT(DATETIME, '" + rptngaydau + "', 102)) " +
                        " AND (Ngay <= CONVERT(DATETIME, '" + rptngaycuoi + "', 102)) GROUP BY Trangthai ORDER BY Trangthai";
                        SelectedData dxs = session.ExecuteQuery(sql1);
                        int tong = 0;

                        DexuatCanhan obj = GetDx("Chờ duyệt", 0, dxs);
                        tong += obj.Sodx;
                        objects.Add(obj);

                        obj = GetDx("Hủy", 1, dxs);
                        tong += obj.Sodx;
                        objects.Add(obj);

                        obj = GetDx("Đang duyệt", 2, dxs);
                        tong += obj.Sodx;
                        objects.Add(obj);

                        obj = GetDx("Đã duyệt", 3, dxs);
                        tong += obj.Sodx;
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Tổng",
                            Sodx = tong
                        };
                        objects.Add(obj);
                    }
                }
            }


            return objects;
        }
        private static DexuatCanhan GetDx(string caption, int loai, SelectedData selectedData)
        {
            DexuatCanhan obj = new()
            {
                Thongtin = caption
            };

            foreach (SelectStatementResultRow dx in selectedData.ResultSet[0].Rows)
            {
                int sl = tmLib.CommonLib.CInt(dx.Values[0]);
                int status = tmLib.CommonLib.CInt(dx.Values[1]);
                if (status == loai)
                {
                    obj.Sodx = sl;
                    break;
                }
            }
            return obj;
        }

        public static IBindingList GetThongkeDuyetCanhan(IObjectSpace objectSpace)
        {
            BindingList<DuyetDexuatCanhan> objects = new();
            int socho = 0;
            int sochoquahan = 0;
            int sohuy = 0;
            int soduyet = 0;
            int soduyetquahan = 0;
            Session session = ((XPObjectSpace)objectSpace).Session;
            ApplicationUser CurentUser = objectSpace.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                if (!string.IsNullOrEmpty(CurentUser.UserName))
                {
                    Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Email=?", CurentUser.UserName));
                    if (nv != null)
                    {
                        string ngay1 = TCom.GetUserStringValue("tungay");
                        _ = DateTime.TryParse(ngay1, out DateTime fTungay);
                        _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

                        ngay1 = TCom.GetUserStringValue("denngay");
                        _ = DateTime.TryParse(ngay1, out DateTime fDenngay);
                        _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);
                        DateTime homnay = GetServerDateTime();

                        XPCollection<DuyetDexuat> ds = new(session)
                        {
                            Criteria = CriteriaOperator.Parse("Nguoiduyet=? && Phieu.Ngay>=? && Phieu.Ngay<=? && Boqua=? && Phieu.Dagui=?", nv.Oid, fTungay, fDenngay, false, true)
                        };
                        int total = 0;
                        foreach (DuyetDexuat item in ds)
                        {
                            //DateTime ngayd = homnay;
                            //if (item.Ngayduyet != DateTime.MinValue)
                            //    ngayd = item.Ngayduyet;
                            if (item.Trangthai == EStatusDuyet.huy)
                            {
                                sohuy++;
                                total++;
                            }
                            else if (item.Trangthai == EStatusDuyet.daduyet)
                            {
                                if (item.Ngayduyet <= item.Handuyet)
                                    soduyet++;
                                else
                                    soduyetquahan++;
                                total++;
                            }
                            else
                            {
                                if (item.Phieu.Dagui)
                                {
                                    if (item.SoTT == 1)
                                    {
                                        if (homnay <= item.Handuyet)
                                            socho++;
                                        else
                                            sochoquahan++;
                                        total++;
                                    }
                                    else
                                    {
                                        //Phieudexuat p = objectSpace.GetObject(item.Phieu);
                                        //DuyetDexuat duyettruoc = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu=? && SoTT=?", item.Phieu, item.SoTT - 1));
                                        DuyetDexuat duyettruoc = BuocTruoc(item.SoTT, item.Phieu.Oid, objectSpace);
                                        if (duyettruoc != null)
                                        {
                                            if (duyettruoc.Trangthai == EStatusDuyet.daduyet)
                                            {
                                                if (homnay <= item.Handuyet)
                                                    socho++;
                                                else
                                                    sochoquahan++;
                                                total++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        DuyetDexuatCanhan obj = new()
                        {
                            Thongtin = "Chờ duyệt",
                            Sodx = socho
                        };
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Chờ duyệt quá hạn",
                            Sodx = sochoquahan
                        };
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Đã hủy",
                            Sodx = sohuy
                        };
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Đã duyệt",
                            Sodx = soduyet
                        };
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Đã duyệt quá hạn",
                            Sodx = soduyetquahan
                        };
                        objects.Add(obj);

                        obj = new()
                        {
                            Thongtin = "Tổng",
                            Sodx = total
                        };
                        objects.Add(obj);
                    }
                }
            }
            return objects;
        }

        public static void AddUserValue(string userKey, string value)
        {
            bool Co = false;
            foreach (UserEnvironment item in UserEnvironments)
            {
                if (item.UserId == SecuritySystem.CurrentUserId.ToString() && item.Userkey == userKey)
                {
                    Co = true;
                    item.Uservalue = value;
                    return;
                }
            }
            if (!Co)
            {
                UserEnvironment item = new()
                {
                    UserId = SecuritySystem.CurrentUserId.ToString(),
                    Userkey = userKey,
                    Uservalue = value
                };
                UserEnvironments.Add(item);
            }
        }

        public static string GetUserStringValue(string userKey)
        {
            string value = "";
            foreach (UserEnvironment item in UserEnvironments)
            {
                if (item.UserId == SecuritySystem.CurrentUserId.ToString() && item.Userkey == userKey)
                {
                    value = item.Uservalue;
                    break;
                }
            }
            return value;
        }
        public static int GetUserIntValue(string userKey)
        {
            int value = 0;
            foreach (UserEnvironment item in UserEnvironments)
            {
                if (item.UserId == SecuritySystem.CurrentUserId.ToString() && item.Userkey == userKey)
                {
                    value = tmLib.CommonLib.CInt(item.Uservalue);
                    break;
                }
            }
            return value;
        }


        public static void CheckMailOptions()
        {
            XafApplication app = ApplicationHelper.Instance.Application;
            IObjectSpace objectSpace = app.CreateObjectSpace(typeof(MailOption));
            MailOption opt = objectSpace.FindObject<MailOption>(CriteriaOperator.Parse("Oid=?", 1));
            if (opt == null)
            {
                ApplicationUser CurentUser = objectSpace.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
                if (CurentUser != null)
                {
                    string user = CurentUser.UserName.ToLower();
                    if (!string.IsNullOrEmpty(user))
                    {
                        if (user == "admin")
                        {
                            opt = objectSpace.CreateObject<MailOption>();
                            opt.Oid = 1;
                            opt.TenMail = "Vietnam Australia Training System";
                            opt.Email = "minhthai.le@itf.edu.vn";
                            opt.MailPass = "";
                            opt.Save();
                            objectSpace.CommitChanges();
                        }
                    }
                }
            }
            if (opt != null)
            {
                defEmail = opt.Email;
                defPassword = opt.MailPass;
                defEmailName = opt.TenMail;
                defChuky = opt.Chuky;
            }
            objectSpace.Dispose();
        }

        public static void SendEmail(string tieude, string noidung, string mailladdr, string tenmail)
        {
            //return;

            SmtpClient client = new()
            {
                Host = "smtp.office365.com",
                Port = 587,
                UseDefaultCredentials = false, // This require to be before setting Credentials property
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(defEmail, defPassword), // you must give a full email address for authentication 
                TargetName = "STARTTLS/smtp.office365.com", // Set to avoid MustIssueStartTlsFirst exception
                EnableSsl = true // Set to avoid secure connection exception
            };

            MailMessage message = new()
            {
                From = new MailAddress(defEmail, defEmailName), // sender must be a full email address
                Subject = tieude,
                IsBodyHtml = true,
                Body = noidung,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8,

            };
            //message.To.Add(new MailAddress(duyet.Phieu.Nhanvien.Email, duyet.Phieu.Nhanvien.HoTen));
            message.To.Add(new MailAddress(mailladdr, tenmail));
            try
            {
                client.Send(message);
                //TCom.CustomInfo("Message Sent Succesfully");
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.ToString());
            }
        }
        public static void SendEmailDuyet(DuyetDexuat duyet)
        {
            if (duyet != null)
            {
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                string tieude = "Thông báo hệ thống trình ký";
                string noidung;
                string ketluan = "Chờ duyệt";
                if (duyet.Trangthai == EStatusDuyet.daduyet)
                    ketluan = "Đã duyệt";
                else if (duyet.Trangthai == EStatusDuyet.huy)
                    ketluan = "Đã hủy";
                string doituong = "";
                if (duyet.Phieu.Chuthe != null)
                {
                    doituong = "Đối với: <b>" + duyet.Phieu.Chuthe.HoTen + "</b>" +
                                "<br>Bộ phận công tác: <b>" + duyet.Phieu.Chuthe.Donvi.Name + "</b><br><br>";
                }

                if (duyet.DuyetUQ)
                {
                    noidung = "Đề xuất số <b>" + duyet.Phieu.Oid + "</b> - " + duyet.Phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + duyet.Phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + duyet.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + doituong
                    + " <b>ĐÃ ĐƯỢC XEM XÉT</b><br><br>" +
                    "Bước: <b>" + +duyet.SoTT + "</b><br>" +
                    "Người duyệt: <b>" + duyet.NguoiUQ.HoTen + "</b><br>" +
                    "Kết luận: <b>" + ketluan + "</b>";
                    Uyquyen uq = objectSpace.FindObject<Uyquyen>(CriteriaOperator.Parse("Oid=?", duyet.UQId));
                    if (uq != null)
                    {
                        noidung += "<br><br> Theo ủy quyền số " + uq.SoUQ + ", ngày " + uq.Ngayky.ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    noidung = "Đề xuất số <b>" + duyet.Phieu.Oid + "</b> - " + duyet.Phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + duyet.Phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + duyet.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + doituong
                    + " <b>ĐÃ ĐƯỢC XEM XÉT</b><br><br>" +
                    "Bước: <b>" + +duyet.SoTT + "</b><br>" +
                    "Người duyệt: <b>" + duyet.Nguoiduyet.HoTen + "</b><br>" +
                    "Kết luận: <b>" + ketluan + "</b>";
                }

                //mail cho nguwowi lap
                //string addr = duyet.Phieu.Nhanvien.Email;
                string addr = (string.IsNullOrEmpty(testMail) ? duyet.Phieu.Nhanvien.Email : testMail);
                string tenmail = duyet.Phieu.Nhanvien.HoTen;
                SendEmail(tieude, noidung, addr, tenmail);

                //gui cho nhung nguoi duyet truoc
                foreach (DuyetDexuat item in duyet.Phieu.DuyetDexuats)
                {
                    if (item.SoTT < duyet.SoTT && item.Boqua == false)
                    {
                        string addr2 = (string.IsNullOrEmpty(testMail) ? item.Nguoiduyet.Email : testMail);
                        string tenmail2 = duyet.Phieu.Nhanvien.HoTen;
                        SendEmail(tieude, noidung, addr2, tenmail2);
                        //uy quyen
                        if (item.NguoiUQ != null)
                        {
                            string addr3 = (string.IsNullOrEmpty(testMail) ? item.NguoiUQ.Email : testMail);
                            SendEmail(tieude, noidung, addr3, tenmail2);
                        }
                    }
                }
                //gui cho nguoi duyet sau
                Phieudexuat phieu = objectSpace.GetObject(duyet.Phieu);
                //int sott = duyet.SoTT + 1;
                //DuyetDexuat duyetmoi = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu=? && SoTT=?", phieu, sott));
                DuyetDexuat duyetmoi = BuocSau(duyet.SoTT, phieu.Oid, objectSpace);
                if (duyetmoi != null)
                {
                    string addr3 = (string.IsNullOrEmpty(testMail) ? duyetmoi.Nguoiduyet.Email : testMail);
                    string tenmail3 = duyetmoi.Phieu.Nhanvien.HoTen;
                    string noidung3 = "Đề xuất số <b>" + duyetmoi.Phieu.Oid + "</b> - " + duyetmoi.Phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + duyetmoi.Phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + duyetmoi.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + doituong
                    + " <b>ĐANG CHỜ XEM XÉT</b><br><br>" +
                    "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                    "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                    "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                    "<br>Xin cảm ơn.";
                    SendEmail(tieude, noidung3, addr3, tenmail3);

                    //uy quyen
                    if (duyetmoi.NguoiUQ != null)
                    {
                        string addr4 = (string.IsNullOrEmpty(testMail) ? duyetmoi.NguoiUQ.Email : testMail);
                        string tenmail4 = duyetmoi.Phieu.Nhanvien.HoTen;
                        string noidung4 = "Đề xuất số <b>" + duyetmoi.Phieu.Oid + "</b> - " + duyetmoi.Phieu.TenDexuat +
                        "<br>Người đề xuất: <b>" + duyetmoi.Phieu.Nhanvien.HoTen + "</b>" +
                        "<br>Bộ phận công tác: <b>" + duyetmoi.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                        + doituong
                        + " <b>ĐANG CHỜ XEM XÉT THEO ỦY QUYỀN</b><br><br>" +
                        "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                        "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                        "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                        "<br>Xin cảm ơn.";
                        SendEmail(tieude, noidung4, addr4, tenmail4);
                    }
                }
            }
        }

        public static void SendEmailYkien(Quatrinh tep)
        {
            if (tep != null)
            {
                if (tep.Phieu.Dagui)
                {
                    string tieude = "Thông báo hệ thống trình ký";
                    string noidung = "Đề xuất số <b>" + tep.Phieu.Oid + "</b> - " + tep.Phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + tep.Phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + tep.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + " <b>ĐÃ ĐƯỢC BỔ SUNG Ý KIẾN</b><br><br>" +
                    "Nội dung: <b>" + tep.Ykien + "</b><br>";

                    //mail cho nguwowi lap
                    //string addr = tep.Phieu.Nhanvien.Email;
                    string addr = (string.IsNullOrEmpty(testMail) ? tep.Phieu.Nhanvien.Email : testMail);
                    string tenmail = tep.Phieu.Nhanvien.HoTen;
                    SendEmail(tieude, noidung, addr, tenmail);

                    //gui cho nhung nguoi duyet truoc va den luot
                    //string sql = @"SELECT TOP (100) PERCENT Trangthai, Nguoiduyet, NguoiUQ
                    //FROM     dbo.DuyetDexuat WHERE (Boqua=0) and (Phieu = " + tep.Phieu.Oid + ") ORDER BY SoTT";

                    XafApplication app = ApplicationHelper.Instance.Application;
                    IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //SelectedData results2 = session.ExecuteQuery(sql);
                    SelectedData results2 = session.ExecuteSprocParametrized("procMailYkien", new SprocParameter("@PhieuID", tep.Phieu.Oid));
                    foreach (SelectStatementResultRow row in results2.ResultSet[0].Rows)
                    {
                        int trangthai = tmLib.CommonLib.CInt(row.Values[0]);
                        int nvId = tmLib.CommonLib.CInt(row.Values[1]);
                        int uqId = tmLib.CommonLib.CInt(row.Values[2]);
                        if (trangthai > 0)
                        {
                            //gui mail cho nguoi da duyet
                            Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nvId));
                            if (nv != null)
                            {
                                //string addr2 = nv.Email;
                                string addr2 = (string.IsNullOrEmpty(testMail) ? nv.Email : testMail);
                                string tenmail2 = nv.HoTen;
                                SendEmail(tieude, noidung, addr2, tenmail2);
                            }
                            if (uqId > 0)
                            {
                                //gui mail cho nguoi dc uy quyen
                                Nhansu nvuq = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", uqId));
                                if (nvuq != null)
                                {
                                    string addr3 = (string.IsNullOrEmpty(testMail) ? nvuq.Email : testMail);
                                    string tenmail3 = nvuq.HoTen;
                                    SendEmail(tieude, noidung, addr3, tenmail3);
                                }
                            }
                        }
                        else if (trangthai == 0)
                        {
                            //gui mail cho nguoi dang duyet va stop
                            Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nvId));
                            if (nv != null)
                            {
                                //string addr2 = nv.Email;
                                string addr2 = (string.IsNullOrEmpty(testMail) ? nv.Email : testMail);
                                string tenmail2 = nv.HoTen;
                                SendEmail(tieude, noidung, addr2, tenmail2);
                            }
                            if (uqId > 0)
                            {
                                //gui mail cho nguoi dc uy quyen
                                Nhansu nvuq = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", uqId));
                                if (nvuq != null)
                                {
                                    string addr3 = (string.IsNullOrEmpty(testMail) ? nvuq.Email : testMail);
                                    string tenmail3 = nvuq.HoTen;
                                    SendEmail(tieude, noidung, addr3, tenmail3);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void SendEmailDexuat(Phieudexuat phieu)
        {
            if (phieu != null)
            {
                string tieude = "Thông báo hệ thống trình ký";
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                Phieudexuat phieumoi = objectSpace.GetObject(phieu);
                int sott = 1;
                DuyetDexuat duyetmoi = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu=? && SoTT=?", phieumoi, sott));
                if (duyetmoi != null)
                {
                    //string addr3 = duyetmoi.Nguoiduyet.Email;
                    string addr3 = (string.IsNullOrEmpty(testMail) ? duyetmoi.Nguoiduyet.Email : testMail);
                    string tenmail3 = duyetmoi.Phieu.Nhanvien.HoTen;
                    string doituong = "";
                    if (phieumoi.Chuthe != null)
                    {
                        doituong = "Đối với: <b>" + phieumoi.Chuthe.HoTen + "</b>" +
                                    "<br>Bộ phận công tác: <b>" + phieumoi.Chuthe.Donvi.Name + "</b><br><br>";
                    }
                    if (phieumoi.Dagui)
                    {
                        string noidung3 = "Đề xuất số <b>" + duyetmoi.Phieu.Oid + "</b> - " + duyetmoi.Phieu.TenDexuat +
                            "<br>Người đề xuất: <b>" + duyetmoi.Phieu.Nhanvien.HoTen + "</b>" +
                            "<br>Bộ phận công tác: <b>" + duyetmoi.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                            + doituong
                            + " <b>ĐANG CHỜ XEM XÉT</b><br><br>" +
                            "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                            "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                            "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                            "<br>Xin cảm ơn.";
                        SendEmail(tieude, noidung3, addr3, tenmail3);
                        if (duyetmoi.NguoiUQ != null)
                        {
                            string addr = (string.IsNullOrEmpty(testMail) ? duyetmoi.NguoiUQ.Email : testMail);
                            string noidung = "Đề xuất số <b>" + duyetmoi.Phieu.Oid + "</b> - " + duyetmoi.Phieu.TenDexuat +
                                "<br>Người đề xuất: <b>" + duyetmoi.Phieu.Nhanvien.HoTen + "</b>" +
                                "<br>Bộ phận công tác: <b>" + duyetmoi.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                                + doituong
                                + " <b>ĐANG CHỜ XEM XÉT THEO ỦY QUYỀN</b><br><br>" +
                                "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                                "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                                "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                                "<br>Xin cảm ơn.";
                            SendEmail(tieude, noidung, addr, tenmail3);
                        }
                    }
                    else
                    {
                        string noidung3 = "Đề xuất số <b>" + duyetmoi.Phieu.Oid + "</b> - " + duyetmoi.Phieu.TenDexuat +
                            "<br>Người đề xuất: <b>" + duyetmoi.Phieu.Nhanvien.HoTen + "</b>" +
                            "<br>Bộ phận công tác: <b>" + duyetmoi.Phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                            + doituong
                            + " <b>ĐÃ ĐƯỢC TÁC GIẢ THU HỒI</b><br><br>";
                        SendEmail(tieude, noidung3, addr3, tenmail3);
                        if (duyetmoi.NguoiUQ != null)
                        {
                            string addr = (string.IsNullOrEmpty(testMail) ? duyetmoi.NguoiUQ.Email : testMail);
                            SendEmail(tieude, noidung3, addr, tenmail3);
                        }
                    }
                }
            }
        }

        public static void SendEmailGiaoviec(Phieudexuat phieu)
        {
            if (phieu != null && phieu.Nguoithuchien != null && phieu.Trangthai == EStatusVB.daduyet)
            {
                string tieude = "Thông báo hệ thống trình ký";
                string noidung3 = "Đề xuất số <b>" + phieu.Oid + "</b> - " + phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + " <b>ĐƯỢC GIAO CHO BẠN XỬ LÝ</b><br><br>" +
                    "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                    "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                    "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                    "<br>Xin cảm ơn.";
                string addr3 = (string.IsNullOrEmpty(testMail) ? phieu.Nguoithuchien.Email : testMail);
                SendEmail(tieude, noidung3, addr3, phieu.Nguoithuchien.HoTen);
            }
        }

        public static void SendEmailXuly(Phieudexuat phieu)
        {
            if (phieu != null && phieu.NguoiXuly != null && phieu.Trangthai == EStatusVB.daduyet)
            {
                string tieude = "Thông báo hệ thống trình ký";
                string noidung3 = "Đề xuất số <b>" + phieu.Oid + "</b> - " + phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + " <b>ĐÃ ĐƯỢC DUYỆT</b><br><br>" +
                    "Bạn vui lòng CLICK vào link sau để xem và giải quyết:<br>" +
                    "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                    "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                    "<br>Xin cảm ơn.";
                string addr3 = (string.IsNullOrEmpty(testMail) ? phieu.NguoiXuly.Email : testMail);
                SendEmail(tieude, noidung3, addr3, phieu.NguoiXuly.HoTen);
            }
        }
        public static void SendEmailChiase(Phieudexuat phieu, string userMail)
        {
            if (phieu != null)
            {
                string tieude = "Thông báo hệ thống trình ký";
                string addr3 = (string.IsNullOrEmpty(testMail) ? userMail : testMail);
                string tenmail3 = phieu.Nhanvien.HoTen;
                string noidung3 = "Đề xuất số <b>" + phieu.Oid + "</b> - " + phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + " <b>ĐÃ ĐƯỢC CHIA SẺ CHO BẠN</b><br><br>" +
                    "Bạn vui lòng CLICK vào link sau để xem:<br>" +
                    "<a href='https://confirm.skylineschool.edu.vn:7979/'><b> Confirm </b></a>" +
                    "<br><br>Nếu gặp khó khăn hay vướng mắc gì, vui lòng liên hệ <b>BAN CÔNG NGHỆ</b> để được hỗ trợ." +
                    "<br>Xin cảm ơn.";
                SendEmail(tieude, noidung3, addr3, tenmail3);
            }
        }

        public static IBindingList GetThongkeTheoMau(IObjectSpace objectSpace)//urgen phieu da huy
        {
            BindingList<ThongkeDexuatMau> objects = new();
            string ngay1 = TCom.GetUserStringValue("tungay");
            _ = DateTime.TryParse(ngay1, out DateTime fTungay);
            _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

            ngay1 = TCom.GetUserStringValue("denngay");
            _ = DateTime.TryParse(ngay1, out DateTime fDenngay);
            _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);

            Session session = ((XPObjectSpace)objectSpace).Session;
            XPCollection<MauVanban> dsmau = new(session)
            {
                Sorting = new SortingCollection(new SortProperty("Tenmau", SortingDirection.Ascending))
            };
            int tt = 0;
            foreach (MauVanban mau in dsmau)
            {
                tt++;
                ThongkeDexuatMau obj = new()
                {
                    TT = tt,
                    Loaimau = mau.Tenmau,
                };
                XPCollection<Phieudexuat> ds = new(session)
                {
                    Criteria = CriteriaOperator.Parse("Mau=? && Ngay>=? && Ngay<=? && Dagui=?", mau, fTungay, fDenngay, true)
                };
                int total = 0;
                foreach (Phieudexuat item in ds)
                {
                    if (item.Trangthai == EStatusVB.dahuy)
                    {
                        obj.Huy++;
                        total++;
                    }
                    else if (item.Trangthai == EStatusVB.dangduyet)
                    {
                        obj.Dangduyet++;
                        total++;
                    }
                    else if (item.Trangthai == EStatusVB.daduyet)
                    {
                        obj.Daduyet++;
                        if (item.Daxong)
                            obj.Daxong++;
                        total++;
                    }
                    else
                    {
                        if (item.Dagui)
                            obj.Chuaduyet++;
                        total++;
                    }
                }
                obj.Sodx = total;
                objects.Add(obj);
            }
            return objects;
        }

        public static bool LapphieuDX(Phieudexuat phieu, IObjectSpace objectSpace)
        {
            bool lapOK = true;
            try
            {
                if (phieu != null)
                {
                    if (phieu.Mau == null)
                    {
                        TCom.CustomError("Cần chọn mẫu");
                        return false;
                    }
                    if (phieu.IsCoso)
                    {
                        if (phieu.CS == null)
                        {
                            TCom.CustomError("Cần chọn cơ sở trực tiếp");
                            return false;
                        }
                    }
                    else
                    {
                        if (phieu.Donvi == null)
                        {
                            TCom.CustomError("Cần chọn bộ phận nhận trực tiếp");
                            return false;
                        }
                    }
                    if (phieu.Noidung != null)
                    {
                        TCom.CustomError("Đề xuất đã có nội dung, không tạo lại được");
                        return false;
                    }
                    //check tieu de {tieude}
                    string tieude = phieu.TenDexuat;
                    if (string.IsNullOrEmpty(tieude))
                    {
                        TCom.CustomError("Đề xuất không có tiêu đề");
                        return false;
                    }
                    int idx=tieude.IndexOf("-");
                    if (idx > 0)
                    {
                        tieude = tmLib.CommonLib.Right(tieude, tieude.Length-idx-1);
                    }

                    Coso coso = phieu.CS;
                    Bophan bp = phieu.Donvi;
                    if (phieu.Mau.Chucvuxuly != null)
                    {
                        if (phieu.Mau.Chucvuxuly.TheoCoso)
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)),
                                CriteriaOperator.Parse("CS=?", coso));

                            Nhansu nxl = objectSpace.FindObject<Nhansu>(criteria);
                            if (nxl != null)
                                phieu.NguoiXuly = nxl;

                        }
                        else
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)),
                                CriteriaOperator.Parse("Donvi=?", bp));
                            Nhansu nxl = objectSpace.FindObject<Nhansu>(criteria);
                            if (nxl != null)
                                phieu.NguoiXuly = nxl;
                            else
                            {
                                criteria = GroupOperator.Combine(GroupOperatorType.And,
                                    new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)));
                                nxl = objectSpace.FindObject<Nhansu>(criteria);
                                if (nxl != null)
                                    phieu.NguoiXuly = nxl;

                            }
                        }
                    }
                    var richServer = new RichEditDocumentServer();
                    richServer.Document.RtfText = phieu.Mau.Noidung;
                    richServer.Document.ReplaceAll("{tieude}", tieude, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    //Thong tin nguoi lap
                    if (phieu.Nhanvien != null)
                    {
                        string hoten = phieu.Nhanvien.HoTen;
                        string phone = phieu.Nhanvien.Phone;
                        string donvi = "";
                        if (phieu.Nhanvien.Donvi != null)
                            donvi = phieu.Nhanvien.Donvi.Name;
                        string tencs = "";
                        if (phieu.Nhanvien.CS != null)
                        {
                            tencs = phieu.Nhanvien.CS.TenCoso;
                        }
                        string sothe = phieu.Nhanvien.Sothe;
                        string nvmail = phieu.Nhanvien.Email;
                        string cv = phieu.Nhanvien.ListCV;
                        richServer.Document.ReplaceAll("{Hoten}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        richServer.Document.ReplaceAll("{Donvi}", donvi, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        richServer.Document.ReplaceAll("{CS}", tencs, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        if(!string.IsNullOrEmpty(phone))
                            richServer.Document.ReplaceAll("{phone}", phone, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        if (!string.IsNullOrEmpty(sothe))
                            richServer.Document.ReplaceAll("{sothe}", sothe, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        if (!string.IsNullOrEmpty(nvmail))
                            richServer.Document.ReplaceAll("{email}", nvmail, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                        if (!string.IsNullOrEmpty(cv))
                            richServer.Document.ReplaceAll("{chucvu}", cv, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    }
                    //them cac chu ky 
                    //Session session = ((XPObjectSpace)ObjectSpace).Session;

                    //phieu.Noidung = richServer.Document.RtfText;
                    phieu.Kyduyet = phieu.Mau.Kyduyet;
                    // qua trinh duyet
                    foreach (QuitrinhChuan item in phieu.Mau.QuitrinhChuans)
                    {
                        if (item.Chucvu != null)
                        {
                            DuyetDexuat duyet = objectSpace.CreateObject<DuyetDexuat>();
                            duyet.Phieu = phieu;
                            duyet.SoTT = item.SoTT;
                            if (item.Chucvu.TheoCoso && coso != null)
                            {
                                if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
                                {
                                    duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
                                }
                                CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                    new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                    CriteriaOperator.Parse("CS=?", coso));

                                Nhansu nscs = objectSpace.FindObject<Nhansu>(criteria);
                                if (nscs != null)
                                {
                                    duyet.Nguoiduyet = nscs;
                                    duyet.Maduyet = item.Chucvu.Code;
                                    duyet.MaChuky = item.Chucvu.CodeCK;
                                    duyet.MaKythay = item.Chucvu.MaKythay;
                                    //check UQ
                                    Uyquyen uq = objectSpace.FindObject<Uyquyen>(CriteriaOperator.Parse("ChucdanhUQ.Oid=? && Mau.Oid=?", item.Chucvu.Oid, phieu.Mau.Oid));
                                    if (uq != null)
                                    {
                                        //check han UQ
                                        if (!uq.Huy && uq.ChucdanhDuocUQ != null)
                                        {
                                            CriteriaOperator criteriaUQ = new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", uq.ChucdanhDuocUQ));
                                            Nhansu nuq = objectSpace.FindObject<Nhansu>(criteriaUQ);
                                            if (nuq != null)
                                            {
                                                duyet.NguoiUQ = nuq;
                                                //duyet.MaduyetUQ = uq.ChucdanhDuocUQ.Code;
                                                //duyet.MaChukyUQ = uq.ChucdanhDuocUQ.CodeCK;
                                                duyet.UQId = uq.Oid;
                                            }
                                        }
                                    }
                                }
                                duyet.Save();
                            }
                            else
                            {
                                CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                    new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                    CriteriaOperator.Parse("Donvi=?", phieu.Donvi));
                                Nhansu ns = objectSpace.FindObject<Nhansu>(criteria);
                                ns ??= objectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)));
                                if (ns != null)
                                {
                                    duyet.Nguoiduyet = ns;
                                    duyet.Maduyet = item.Chucvu.Code;
                                    duyet.MaChuky = item.Chucvu.CodeCK;
                                    duyet.MaKythay = item.Chucvu.MaKythay;
                                    //check UQ
                                    Uyquyen uq = objectSpace.FindObject<Uyquyen>(CriteriaOperator.Parse("ChucdanhUQ.Oid=? && Mau.Oid=?", item.Chucvu.Oid, phieu.Mau.Oid));
                                    if (uq != null)
                                    {
                                        //check han UQ
                                        if (!uq.Huy && uq.ChucdanhDuocUQ != null)
                                        {
                                            CriteriaOperator criteriaUQ = new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", uq.ChucdanhDuocUQ));
                                            Nhansu nuq = objectSpace.FindObject<Nhansu>(criteriaUQ);
                                            if (nuq != null)
                                            {
                                                duyet.NguoiUQ = nuq;
                                                //duyet.MaduyetUQ = uq.ChucdanhDuocUQ.Code;
                                                //duyet.MaChukyUQ = uq.ChucdanhDuocUQ.CodeCK;
                                                duyet.UQId = uq.Oid;
                                            }
                                        }
                                    }

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
                    objectSpace.CommitChanges();
                    richServer.Document.ReplaceAll("{so}", phieu.Oid.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (phieu.Mau.ChonVT)
                        phieu.NoidungGoc = richServer.Document.RtfText;
                    else
                        phieu.Noidung = richServer.Document.RtfText;
                    //link ID
                    if (phieu.LinkID > 0)
                    {
                        Phieudexuat pcu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieu.LinkID));
                        if (pcu != null)
                        {
                            foreach (PhieudexuatFile item in pcu.PhieudexuatFiles)
                            {
                                PhieudexuatFile phieudexuatFile = objectSpace.CreateObject<PhieudexuatFile>();
                                phieudexuatFile.Nhanvien = objectSpace.GetObject(item.Nhanvien);
                                phieudexuatFile.Phieu = phieu;
                                phieudexuatFile.Tailieu = item.Tailieu;
                                phieudexuatFile.TepTen = item.TepTen;
                                phieudexuatFile.TepUrl = item.TepUrl;
                                phieudexuatFile.Loai = 0;
                                phieudexuatFile.Save();
                            }
                        }
                    }
                    objectSpace.CommitChanges();

                }

            }
            catch
            {
                lapOK = false;
            }
            return lapOK;
        }

        public static int SoChiase()
        {
            int so = 0;
            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                int nvID = nv.Oid;
                string sql = @"SELECT COUNT(dbo.Phieudexuat.OID) AS so
                FROM     dbo.Phieudexuat INNER JOIN
                dbo.Chiase ON dbo.Phieudexuat.OID = dbo.Chiase.Phieu
                WHERE  (dbo.Chiase.NhanvienShared = " + nvID + ")";
                XafApplication app = ApplicationHelper.Instance.Application;
                IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                Session session = ((XPObjectSpace)objectSpace).Session;
                var ret = session.ExecuteScalar(sql);
                so = tmLib.CommonLib.CInt(ret);
            }
            return so;
        }
        public static IBindingList GetDanhsachChiase(IObjectSpace objectSpace)
        {
            BindingList<PhieuChiase> objects = new();
            Nhansu nv = GetCurrentNhanvien();
            if (nv != null)
            {
                Session session = ((XPObjectSpace)objectSpace).Session;
                //string sql = @"SELECT TOP (100) PERCENT dbo.Phieudexuat.OID, dbo.Chiase.Ykien, dbo.Chiase.NhanvienShare 
                //FROM     dbo.Phieudexuat INNER JOIN
                //dbo.Chiase ON dbo.Phieudexuat.OID = dbo.Chiase.Phieu
                //WHERE  (dbo.Chiase.NhanvienShared = " + nv.Oid + ") ORDER BY dbo.Phieudexuat.OID";
                //SelectedData results = session.ExecuteQuery(sql);
                SelectedData results = session.ExecuteSprocParametrized("procChiase", new SprocParameter("@NhanvienID", nv.Oid));
                int tt = 0;
                foreach (SelectStatementResultRow row in results.ResultSet[0].Rows)
                {
                    int phieuOid = tmLib.CommonLib.CInt(row.Values[0]);
                    string yk = tmLib.CommonLib.CString(row.Values[1]);
                    int nguoiCSID = tmLib.CommonLib.CInt(row.Values[2]);
                    Phieudexuat d = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieuOid));
                    if (d != null)
                    {
                        tt++;
                        PhieuChiase obj = new()
                        {
                            TT = tt,
                            Oid = d.Oid,
                            TenDexuat = d.Mau.Kyhieu + "-" + d.Oid.ToString() + "-" + d.TenDexuat,
                            Ngay = d.Ngay,
                            Trangthai = d.Trangthai,
                            Ykien = yk
                        };
                        if (d.Nhanvien != null)
                        {
                            obj.Nguoi = d.Nhanvien.HoTen;
                            if (d.Nhanvien.Donvi != null)
                                obj.Bophan = d.Nhanvien.Donvi.Name;
                        }
                        Nhansu nvcs = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nguoiCSID));
                        if (nvcs != null)
                            obj.NguoiCS = nvcs.HoTen;
                        objects.Add(obj);
                    }
                }
            }
            return objects;
        }


        public static bool LapphieuTheoChuthe(Phieudexuat phieu, IObjectSpace objectSpace)
        {
            bool lapOK = true;
            try
            {
                if (phieu.Chuthe == null)
                {
                    TCom.CustomError("Không xác định được chủ thể");
                    return false;
                }
                if (phieu.Noidung != null)
                {
                    TCom.CustomError("Đề xuất đã có nội dung, không tạo lại được");
                    return false;
                }
                //check tieu de {tieude}
                string tieude = phieu.TenDexuat;
                if (string.IsNullOrEmpty(tieude))
                {
                    TCom.CustomError("Đề xuất không có tiêu đề");
                    return false;
                }
                int idx = tieude.IndexOf("-");
                if (idx > 0)
                {
                    tieude = tmLib.CommonLib.Right(tieude, tieude.Length - idx - 1);
                }

                Coso coso = phieu.Chuthe.CS;
                Bophan bp = phieu.Chuthe.Donvi;


                if (phieu.Mau.Chucvuxuly != null)
                {
                    if (phieu.Mau.Chucvuxuly.TheoCoso)
                    {
                        CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                            new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)),
                            CriteriaOperator.Parse("CS=?", coso));

                        Nhansu nxl = objectSpace.FindObject<Nhansu>(criteria);
                        if (nxl != null)
                            phieu.NguoiXuly = nxl;

                    }
                    else
                    {
                        Nhansu nxl = objectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)));
                        if (nxl != null)
                            phieu.NguoiXuly = nxl;
                    }
                }
                var richServer = new RichEditDocumentServer();
                richServer.Document.RtfText = phieu.Mau.Noidung;
                richServer.Document.ReplaceAll("{tieude}", tieude, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                //Thong tin nguoi lap
                if (phieu.Nhanvien != null)
                {
                    string hoten = phieu.Nhanvien.HoTen;
                    string phone = phieu.Nhanvien.Phone;
                    string donvi = "";
                    if (phieu.Nhanvien.Donvi != null)
                        donvi = phieu.Nhanvien.Donvi.Name;
                    string tencs = "";
                    if (phieu.Nhanvien.CS != null)
                    {
                        tencs = phieu.Nhanvien.CS.TenCoso;
                    }
                    string sothe = phieu.Nhanvien.Sothe;
                    string nvmail = phieu.Nhanvien.Email;
                    string cv = phieu.Nhanvien.ListCV;
                    richServer.Document.ReplaceAll("{Hoten}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{Donvi}", donvi, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{CS}", tencs, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (!string.IsNullOrEmpty(phone))
                        richServer.Document.ReplaceAll("{phone}", phone, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (!string.IsNullOrEmpty(sothe))
                        richServer.Document.ReplaceAll("{sothe}", sothe, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (!string.IsNullOrEmpty(nvmail))
                        richServer.Document.ReplaceAll("{email}", nvmail, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (!string.IsNullOrEmpty(cv))
                        richServer.Document.ReplaceAll("{chucvu}", cv, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                //Chu the
                if (phieu.Chuthe != null)
                {
                    string hoten = phieu.Chuthe.HoTen;
                    string email = phieu.Chuthe.Email;
                    string cv = phieu.Chuthe.ListCV;
                    string donvi = "";
                    if (phieu.Chuthe.Donvi != null)
                        donvi = phieu.Chuthe.Donvi.Name;
                    string tencs = "";
                    if (phieu.Chuthe.CS != null)
                    {
                        tencs = phieu.Chuthe.CS.TenCoso;
                    }
                    richServer.Document.ReplaceAll("{HotenCT}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{DonviCT}", donvi, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{CosoCT}", tencs, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{EmailCT}", email, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{ChucvuCT}", cv, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                //Ngay thang nam
                if (phieu.Ngay != DateTime.MinValue)
                {
                    string ngay = phieu.Ngay.Day.ToString();
                    string thang = phieu.Ngay.Month.ToString();
                    string nam = phieu.Ngay.Year.ToString("####");
                    richServer.Document.ReplaceAll("{dd}", ngay, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{mm}", thang, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("{yy}", nam, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }

                //phieu.Noidung = richServer.Document.RtfText;
                phieu.Kyduyet = phieu.Mau.Kyduyet;
                // qua trinh duyet
                foreach (QuitrinhChuan item in phieu.Mau.QuitrinhChuans)
                {
                    if (item.Chucvu != null)
                    {
                        DuyetDexuat duyet = objectSpace.CreateObject<DuyetDexuat>();
                        duyet.Phieu = phieu;
                        duyet.SoTT = item.SoTT;
                        if (item.Chucvu.TheoCoso && coso != null)
                        {
                            if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
                            {
                                duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
                            }
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                CriteriaOperator.Parse("CS=?", coso));

                            Nhansu nscs = objectSpace.FindObject<Nhansu>(criteria);
                            if (nscs != null)
                            {
                                duyet.Nguoiduyet = nscs;
                                duyet.Maduyet = item.Chucvu.Code;
                                duyet.MaChuky = item.Chucvu.CodeCK;
                                duyet.MaKythay = item.Chucvu.MaKythay;
                                //check UQ
                                Uyquyen uq = objectSpace.FindObject<Uyquyen>(CriteriaOperator.Parse("ChucdanhUQ.Oid=? && Mau.Oid=?", item.Chucvu.Oid, phieu.Mau.Oid));
                                if (uq != null)
                                {
                                    //check han UQ
                                    if (!uq.Huy && uq.ChucdanhDuocUQ != null)
                                    {
                                        CriteriaOperator criteriaUQ = new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", uq.ChucdanhDuocUQ));
                                        Nhansu nuq = objectSpace.FindObject<Nhansu>(criteriaUQ);
                                        if (nuq != null)
                                        {
                                            duyet.NguoiUQ = nuq;
                                            duyet.UQId = uq.Oid;
                                        }
                                    }
                                }
                            }
                            duyet.Save();
                        }
                        else
                        {
                            CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
                                new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
                                CriteriaOperator.Parse("Donvi=?", bp));
                            Nhansu ns = objectSpace.FindObject<Nhansu>(criteria);
                            ns ??= objectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)));
                            if (ns != null)
                            {
                                duyet.Nguoiduyet = ns;
                                duyet.Maduyet = item.Chucvu.Code;
                                duyet.MaChuky = item.Chucvu.CodeCK;
                                duyet.MaKythay = item.Chucvu.MaKythay;
                                //check UQ
                                Uyquyen uq = objectSpace.FindObject<Uyquyen>(CriteriaOperator.Parse("ChucdanhUQ.Oid=? && Mau.Oid=?", item.Chucvu.Oid, phieu.Mau.Oid));
                                if (uq != null)
                                {
                                    //check han UQ
                                    if (!uq.Huy && uq.ChucdanhDuocUQ != null)
                                    {
                                        CriteriaOperator criteriaUQ = new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", uq.ChucdanhDuocUQ));
                                        Nhansu nuq = objectSpace.FindObject<Nhansu>(criteriaUQ);
                                        if (nuq != null)
                                        {
                                            duyet.NguoiUQ = nuq;
                                            duyet.UQId = uq.Oid;
                                        }
                                    }
                                }

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
                objectSpace.CommitChanges();
                richServer.Document.ReplaceAll("{so}", phieu.Oid.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                phieu.Noidung = richServer.Document.RtfText;
            }
            catch
            {
                lapOK = false;
            }
            return lapOK;
        }
        private static DuyetDexuat BuocSau(int SoCurrent, int PhieuID, IObjectSpace objectSpace)
        {
            DuyetDexuat d = null;
            for (int i = SoCurrent + 1; i < 10; i++)
            {
                DuyetDexuat duyetsau = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu.Oid=? && SoTT=?", PhieuID, i));
                if (duyetsau != null)
                {
                    if (duyetsau.Boqua == false)
                    {
                        d = duyetsau;
                        break;
                    }
                }
            }
            return d;
        }

        private static DuyetDexuat BuocTruoc(int SoCurrent, int PhieuID, IObjectSpace objectSpace)
        {
            DuyetDexuat d = null;
            for (int i = SoCurrent - 1; i > 0; i--)
            {
                DuyetDexuat duyettruoc = objectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Phieu.Oid=? && SoTT=?", PhieuID, i));
                if (duyettruoc != null)
                {
                    if (duyettruoc.Boqua == false)
                    {
                        d = duyettruoc;
                        break;
                    }
                }
            }
            return d;
        }

        public static void SendEmailTailieu(Phieudexuat phieu, string tentailieu)
        {
            if (phieu != null)
            {
                if (phieu.Dagui)
                {
                    string tieude = "Thông báo hệ thống trình ký";
                    string noidung = "Đề xuất số <b>" + phieu.Oid + "</b> - " + phieu.TenDexuat +
                    "<br>Người đề xuất: <b>" + phieu.Nhanvien.HoTen + "</b>" +
                    "<br>Bộ phận công tác: <b>" + phieu.Nhanvien.Donvi.Name + "</b><br><br>"
                    + " <b>ĐÃ ĐƯỢC BỔ SUNG TÀI LIỆU</b><br><br>" +
                    "Tệp: <b>" + tentailieu + "</b><br>";

                    //mail cho nguwowi lap
                    //string addr = tep.Phieu.Nhanvien.Email;
                    string addr = (string.IsNullOrEmpty(testMail) ? phieu.Nhanvien.Email : testMail);
                    string tenmail = phieu.Nhanvien.HoTen;
                    SendEmail(tieude, noidung, addr, tenmail);

                    //gui cho nhung nguoi duyet truoc va den luot
                    //string sql = @"SELECT TOP (100) PERCENT Trangthai, Nguoiduyet, NguoiUQ 
                    //FROM     dbo.DuyetDexuat WHERE (Boqua=0) and (Phieu = " + phieu.Oid + ") ORDER BY SoTT";

                    XafApplication app = ApplicationHelper.Instance.Application;
                    IObjectSpace objectSpace = app.CreateObjectSpace(typeof(DuyetDexuat));
                    Session session = ((XPObjectSpace)objectSpace).Session;
                    //SelectedData results2 = session.ExecuteQuery(sql);
                    SelectedData results2 = session.ExecuteSprocParametrized("procMailTailieu", new SprocParameter("@PhieuID", phieu.Oid));
                    foreach (SelectStatementResultRow row in results2.ResultSet[0].Rows)
                    {
                        int trangthai = tmLib.CommonLib.CInt(row.Values[0]);
                        int nvId = tmLib.CommonLib.CInt(row.Values[1]);
                        int uqId = tmLib.CommonLib.CInt(row.Values[2]);
                        if (trangthai > 0)
                        {
                            //gui mail cho nguoi da duyet
                            Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nvId));
                            if (nv != null)
                            {
                                string addr2 = (string.IsNullOrEmpty(testMail) ? nv.Email : testMail);
                                string tenmail2 = nv.HoTen;
                                SendEmail(tieude, noidung, addr2, tenmail2);
                            }
                            if (uqId > 0)
                            {
                                //gui mail cho nguoi dc uy quyen
                                Nhansu nvuq = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", uqId));
                                if (nvuq != null)
                                {
                                    string addr3 = (string.IsNullOrEmpty(testMail) ? nvuq.Email : testMail);
                                    string tenmail3 = nvuq.HoTen;
                                    SendEmail(tieude, noidung, addr3, tenmail3);
                                }
                            }
                        }
                        else if (trangthai == 0)
                        {
                            //gui mail cho nguoi dang duyet va stop
                            Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", nvId));
                            if (nv != null)
                            {
                                //string addr2 = nv.Email;
                                string addr2 = (string.IsNullOrEmpty(testMail) ? nv.Email : testMail);
                                string tenmail2 = nv.HoTen;
                                SendEmail(tieude, noidung, addr2, tenmail2);
                            }
                            if (uqId > 0)
                            {
                                //gui mail cho nguoi dc uy quyen
                                Nhansu nvuq = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", uqId));
                                if (nvuq != null)
                                {
                                    string addr3 = (string.IsNullOrEmpty(testMail) ? nvuq.Email : testMail);
                                    string tenmail3 = nvuq.HoTen;
                                    SendEmail(tieude, noidung, addr3, tenmail3);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}

