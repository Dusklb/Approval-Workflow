using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class DuyetdexuatCountController : WindowController
    {
        private ShowNavigationItemController navigationController;
        protected override void OnFrameAssigned()
        {
            UnsubscribeFromEvents();
            base.OnFrameAssigned();
            navigationController = Frame.GetController<ShowNavigationItemController>();
            if (navigationController != null)
            {
                navigationController.NavigationItemCreated += NavigationItemCreated;
            }
        }
        void NavigationItemCreated(object sender, NavigationItemCreatedEventArgs e)
        {
            if (e.ModelNavigationItem.Id == "Phieuduyet_ListView")
            {
                string socho = TCom.Sochoduyet();
                if (string.IsNullOrEmpty(socho))
                    e.NavigationItem.Active.SetItemValue("nvthuong", false);
                else
                    e.NavigationItem.Caption = "Đang chờ duyệt (" + socho + ")";
            }
            else if (e.ModelNavigationItem.Id == "PhieuDaduyet_ListView")
            {
                int socho = TCom.SodaDuyet();
                if (socho < 0)
                    e.NavigationItem.Active.SetItemValue("nvthuong", false);
                else if (socho > 0)
                    e.NavigationItem.Caption = "Đã duyệt (" + socho + ")";
                else
                    e.NavigationItem.Caption = "Đã duyệt";
            }
            else if (e.ModelNavigationItem.Id == "PhieuDahuy_ListView")
            {
                int socho = TCom.SodaHuy();
                if (socho < 0)
                    e.NavigationItem.Active.SetItemValue("nvthuong", false);
                else if (socho > 0)
                    e.NavigationItem.Caption = "Đã hủy (" + socho + ")";
                else
                    e.NavigationItem.Caption = "Đã hủy";
            }
            else if (e.ModelNavigationItem.Id == "PhieuXuly_ListView")
            {
                string inf = TCom.Soxuly();
                if (inf == "-1")
                    e.NavigationItem.Active.SetItemValue("nvthuong", false);
                else if (inf.Length > 0)
                    e.NavigationItem.Caption = "Xử lý sau duyệt (" + inf + ")";
                else
                    e.NavigationItem.Caption = "Xử lý sau duyệt";
            }
            else if (e.ModelNavigationItem.Id == "PhieuChiase_ListView")
            {
                int so = TCom.SoChiase();
                if (so > 0)
                    e.NavigationItem.Caption = "Được chia sẻ (" + so + ")";
                else
                    e.NavigationItem.Caption = "Được chia sẻ";
            }
            else if (e.ModelNavigationItem.Id == "DuyetDexuatCanhan_ListView")
            {
                bool thayduoc = true;
                Nhansu nv = TCom.GetCurrentNhanvien();
                if (nv != null)
                {
                    if (nv.ChucvuNVs.Count == 0)
                        thayduoc = false;
                }
                e.NavigationItem.Active.SetItemValue("nvthuong", thayduoc);
            }
        }
        private void UnsubscribeFromEvents()
        {
            if (navigationController != null)
            {
                navigationController.NavigationItemCreated -= NavigationItemCreated;
                navigationController = null;
            }
        }
        protected override void Dispose(bool disposing)
        {
            UnsubscribeFromEvents();
            base.Dispose(disposing);
        }
    }
}
