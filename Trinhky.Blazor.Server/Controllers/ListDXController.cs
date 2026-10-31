using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using System.Diagnostics.CodeAnalysis;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ViewPhieuduyetController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public ViewPhieuduyetController()
        {
            TargetViewId = "Phieuduyet_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is Phieuduyet p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
    public class ViewPhieuDaduyetController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public ViewPhieuDaduyetController()
        {
            TargetViewId = "PhieuDaduyet_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is PhieuDaduyet p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
    public class ViewPhieuPhieuDahuyController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public ViewPhieuPhieuDahuyController()
        {
            TargetViewId = "PhieuDahuy_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is PhieuDahuy p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
    public class ViewPhieuXulyController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public ViewPhieuXulyController()
        {
            TargetViewId = "PhieuXuly_ListView";
            SimpleAction ThuchienDX = new(this, "ThuchienDX", "View")
            {
                Caption = "Thực hiện",
                ImageName = "work",
                ToolTip = "Lập đề xuất thực hiện",
                TargetViewId = "PhieuXuly_ListView",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
            ThuchienDX.Execute += ThuchienDX_Execute;

            //SimpleAction XulyDX = new(this, "XulyDX", "View")
            //{
            //    Caption = "Trạng thái",
            //    ImageName = "daduyet",
            //    ToolTip = "Xác lập trạng thái",
            //    TargetViewId = "PhieuXuly_ListView",
            //    SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            //};
            //XulyDX.Execute += XulyDX_Execute;
        }

        private void XulyDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is PhieuXuly p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu=objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    DetailView dView = Application.CreateDetailView(objectSpace, "XulyPhieudexuat", true, p);
                    Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Xuly(phieu, objectSpace));
                }

            }
        }
        private void Xuly(Phieudexuat phieudexuat, IObjectSpace objectSpace)
        {

        }
        private void ThuchienDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is PhieuXuly p)
            {
                if (p.Daxong)
                {
                    TCom.CustomError("Phiếu đề xuất đã hoàn thành");
                }
                else
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                    Phieudexuat pmoi = objectSpace.CreateObject<Phieudexuat>();
                    pmoi.LinkID = p.Oid;
                    pmoi.Thuchien = true;
                    DetailView dView = Application.CreateDetailView(objectSpace, "Phieumoi", true, pmoi);
                    Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Lapphieu(pmoi, objectSpace));

                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is PhieuXuly p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        void Lapphieu(Phieudexuat phieu, IObjectSpace objectSpace)
        {
            if (TCom.LapphieuDX(phieu, objectSpace))
            {
                IObjectSpace objectSpace1 = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat p = objectSpace1.GetObject(phieu);
                TCom.AddUserValue("viewdx", p.Oid.ToString());
                DashboardView dashboardView = Application.CreateDashboardView(objectSpace1, "viewDX", true);
                ShowViewParameters svp = new()
                {
                    CreatedView = dashboardView,
                    TargetWindow = TargetWindow.Default,
                    Context = TemplateContext.View
                };
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
            }
        }

        //void Lapphieu(Phieudexuat phieu, IObjectSpace objectSpace)
        //{
        //    if (phieu != null)
        //    {
        //        if (phieu.Mau == null)
        //        {
        //            TCom.CustomError("Cần chọn mẫu");
        //            return;
        //        }
        //        if (phieu.IsCoso)
        //        {
        //            if (phieu.CS == null)
        //            {
        //                TCom.CustomError("Cần chọn cơ sở trực tiếp");
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            if (phieu.Donvi == null)
        //            {
        //                TCom.CustomError("Cần chọn bộ phận nhận trực tiếp");
        //                return;
        //            }
        //        }
        //        if (phieu.Noidung != null)
        //        {
        //            TCom.CustomError("Đề xuất đã có nội dung, không tạo lại được");
        //            return;
        //        }
        //        if (phieu.Mau.Chucvuxuly != null)
        //        {
        //            Nhansu nxl = objectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", phieu.Mau.Chucvuxuly)));
        //            if (nxl != null)
        //                phieu.NguoiXuly = nxl;
        //        }
        //        var richServer = new RichEditDocumentServer();
        //        richServer.Document.RtfText = phieu.Mau.Noidung;
        //        //Thong tin nguoi lap
        //        if (phieu.Nhanvien != null)
        //        {
        //            string hoten = phieu.Nhanvien.HoTen;
        //            string donvi = "";
        //            if (phieu.Nhanvien.Donvi != null)
        //                donvi = phieu.Nhanvien.Donvi.Name;
        //            string tencs = "";
        //            if (phieu.Nhanvien.CS != null)
        //            {
        //                tencs = phieu.Nhanvien.CS.TenCoso;
        //            }
        //            richServer.Document.ReplaceAll("{Hoten}", hoten, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
        //            richServer.Document.ReplaceAll("{Donvi}", donvi, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
        //            richServer.Document.ReplaceAll("{CS}", tencs, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
        //        }
        //        //them cac chu ky 
        //        //Session session = ((XPObjectSpace)ObjectSpace).Session;
        //        Coso coso = phieu.CS;

        //        //phieu.Noidung = richServer.Document.RtfText;
        //        phieu.Kyduyet = phieu.Mau.Kyduyet;
        //        // qua trinh duyet
        //        foreach (QuitrinhChuan item in phieu.Mau.QuitrinhChuans)
        //        {
        //            if (item.Chucvu != null)
        //            {
        //                DuyetDexuat duyet = objectSpace.CreateObject<DuyetDexuat>();
        //                duyet.Phieu = phieu;
        //                duyet.SoTT = item.SoTT;
        //                if (item.Chucvu.TheoCoso && coso != null)
        //                {
        //                    CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
        //                        new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
        //                        CriteriaOperator.Parse("CS=?", coso));

        //                    Nhansu nscs = objectSpace.FindObject<Nhansu>(criteria);
        //                    if (nscs != null)
        //                    {
        //                        duyet.Nguoiduyet = nscs;
        //                        duyet.Maduyet = item.Chucvu.Code;
        //                        duyet.MaChuky = item.Chucvu.CodeCK;
        //                    }
        //                    if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
        //                    {
        //                        duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
        //                    }
        //                    duyet.Save();
        //                }
        //                else
        //                {
        //                    CriteriaOperator criteria = GroupOperator.Combine(GroupOperatorType.And,
        //                        new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)),
        //                        CriteriaOperator.Parse("Donvi=?", phieu.Donvi));
        //                    Nhansu ns = objectSpace.FindObject<Nhansu>(criteria);
        //                    ns ??= objectSpace.FindObject<Nhansu>(new ContainsOperator("ChucvuNVs", new BinaryOperator("Chucvu", item.Chucvu)));
        //                    if (ns != null)
        //                    {
        //                        duyet.Nguoiduyet = ns;
        //                        duyet.Maduyet = item.Chucvu.Code;
        //                        duyet.MaChuky = item.Chucvu.CodeCK;
        //                    }
        //                    if (item.Songay > 0 && phieu.Ngay != DateTime.MinValue)
        //                    {
        //                        duyet.Handuyet = phieu.Ngay.AddDays(item.Songay);
        //                    }
        //                    duyet.Save();
        //                }

        //            }
        //        }
        //        phieu.Save();
        //        objectSpace.CommitChanges();
        //        richServer.Document.ReplaceAll("{so}", phieu.Oid.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
        //        phieu.Noidung = richServer.Document.RtfText;
        //        //link ID
        //        if (phieu.LinkID > 0)
        //        {
        //            Phieudexuat pcu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieu.LinkID));
        //            if (pcu != null)
        //            {
        //                foreach (PhieudexuatFile item in pcu.PhieudexuatFiles)
        //                {
        //                    PhieudexuatFile phieudexuatFile = objectSpace.CreateObject<PhieudexuatFile>();
        //                    phieudexuatFile.Nhanvien = objectSpace.GetObject(item.Nhanvien);
        //                    phieudexuatFile.Phieu = phieu;
        //                    phieudexuatFile.Tailieu = item.Tailieu;

        //                    using (var stream = new MemoryStream())
        //                    {
        //                        item.Tep.SaveToStream(stream);
        //                        stream.Position = 0;
        //                        FileData fd = objectSpace.CreateObject<FileData>();
        //                        fd.LoadFromStream(item.Tep.FileName, stream);
        //                        phieudexuatFile.Tep = fd;
        //                    }

        //                    phieudexuatFile.Loai = 0;
        //                    phieudexuatFile.Save();
        //                }
        //            }
        //        }
        //        objectSpace.CommitChanges();

        //        IObjectSpace objectSpace1 = Application.CreateObjectSpace(typeof(Phieudexuat));
        //        Phieudexuat p = objectSpace1.GetObject(phieu);
        //        TCom.AddUserValue("viewdx", p.Oid.ToString());
        //        DashboardView dashboardView = Application.CreateDashboardView(objectSpace1, "viewDX", true);
        //        //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
        //        //e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
        //        //e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
        //        //e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;

        //        //DetailView detailView = Application.CreateDetailView(objectSpace1, "Noidungphieu", true, objectSpace1.GetObject(phieu));
        //        //detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
        //        ShowViewParameters svp = new()
        //        {
        //            CreatedView = dashboardView,
        //            TargetWindow = TargetWindow.Default,
        //            Context = TemplateContext.View
        //        };
        //        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        //    }
        //}
    }

    public class ViewPhieuPhieuChiaseController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public ViewPhieuPhieuChiaseController()
        {
            TargetViewId = "PhieuChiase_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            NewObjectViewController newObjectViewController=Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.SetItemValue("an", false);
            DeleteObjectsViewController deleteObjectsViewController=Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController?.Active.SetItemValue("an", false);

            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is PhieuChiase p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.RemoveItem("an");
            DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController?.Active.RemoveItem("an");
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
}
