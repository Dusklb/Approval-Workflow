using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class PhieudexuatController : ObjectViewController<ListView, Phieudexuat>
    {
        private bool bAdmin;
        public PhieudexuatController()
        {
            //TargetViewId = "Phieudexuat_ListView";
        }

        private void DelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if(View.CurrentObject is Phieudexuat p)
            {
                string msg = "Chắc chắn xóa đề xuất này?";
                NonPersistentObjectSpace objectSpace =
                    (NonPersistentObjectSpace)Application.CreateObjectSpace(typeof(ConfirmationWindowParameters));
                ConfirmationWindowParameters parameters = new()
                {
                    ConfirmationMessage = msg
                };
                DetailView confirmationDetailView = Application.CreateDetailView(objectSpace, parameters);
                confirmationDetailView.Caption = "Xác nhận";
                Application.ShowViewStrategy.ShowViewInPopupWindow(confirmationDetailView,()=> OkDelegate(p));
            }
        }
        void OkDelegate(Phieudexuat phieu)
        {
            phieu.Delete();
            ObjectSpace.CommitChanges();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            bAdmin = TCom.IsAdmin();
            View.ControlsCreated += View_ControlsCreated;
            SetFilter();
            NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.SetItemValue("hide", false);

            DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            if(deleteObjectsViewController != null)
                deleteObjectsViewController.Deleting += DeleteObjectsViewController_Deleting;
        }

        private void DeleteObjectsViewController_Deleting(object sender, DeletingEventArgs e)
        {
            if (!bAdmin)
            {
                if (View.CurrentObject is Phieudexuat p)
                {
                    if (p.Dagui)
                    {
                        e.Cancel = true;
                        TCom.CustomError("Không xóa phiếu đã gửi được");
                    }
                }
            }
        }

        private void View_ControlsCreated(object sender, EventArgs e)
        {
            this.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            SetFilter();
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.SetItemValue("hide", true);
            this.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            base.OnDeactivated();
        }
        private void SetFilter()
        {
            string ngay1 = TCom.GetUserStringValue("tungay");
            _ = DateTime.TryParse(ngay1, out DateTime fTungay);
            _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

            ngay1 = TCom.GetUserStringValue("denngay");
            _ = DateTime.TryParse(ngay1, out DateTime fDenngay);
            _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);

            CriteriaOperator criteriaOperator;
            if (!bAdmin)
            {
                Nhansu nv = TCom.GetCurrentNhanvien();
                if (nv != null)
                {
                    bool xemall = nv.XemAll;
                    if (!xemall)
                    {
                        criteriaOperator = CriteriaOperator.Parse("Nhanvien=? && Ngay>=? && Ngay<=?", ObjectSpace.GetObject<Nhansu>(nv), fTungay, fDenngay);
                        View.CollectionSource.Criteria["filter"] = criteriaOperator;
                    }
                    else
                    {
                        criteriaOperator = CriteriaOperator.Parse("Ngay>=? && Ngay<=?", fTungay, fDenngay);
                        View.CollectionSource.Criteria["filter"] = criteriaOperator;
                    }
                }
            }
            else
            {
                criteriaOperator = CriteriaOperator.Parse("Ngay>=? && Ngay<=?", fTungay, fDenngay);
                View.CollectionSource.Criteria["filter"] = criteriaOperator;
            }

        }
        //private void SetFilter1()
        //{
        //    if (!bAdmin)
        //    {
        //        Nhansu nv = TCom.GetCurrentNhanvien();
        //        if (nv != null)
        //        {
        //            bool xemall = nv.XemAll;
        //            if (!xemall) 
        //            {
        //                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Nhanvien=?", ObjectSpace.GetObject<Nhansu>(nv));
        //                View.CollectionSource.Criteria["filter"] = criteriaOperator;
        //            }
        //        }
        //    }

        //}
    }
    public class PhieudexuatDetailController : ObjectViewController<DetailView, Phieudexuat>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            //bool chophep = false;
            if (View.CurrentObject is Phieudexuat p)
            {
                Nhansu curNV = TCom.GetCurrentNhanvien();
                if (curNV != null)
                {
                    if (p.NguoiXuly != null)
                    {
                        if (p.NguoiXuly.Oid == curNV.Oid)
                        {
                            TCom.SetAllowEdit(View, "Daxong", true);
                        }
                        else
                            TCom.SetAllowEdit(View, "Daxong", false);
                    }
                    else
                        TCom.SetAllowEdit(View, "Daxong", false);
                }
                if (p.Oid < 0)
                {
                    TCom.SetAllowEdit(View, "Mau", true);
                    TCom.SetAllowEdit(View, "Donvi", true);
                    TCom.SetAllowEdit(View, "CS", true);
                }
                else
                {
                    TCom.SetAllowEdit(View, "Mau", false);
                    TCom.SetAllowEdit(View, "Donvi", false);
                    TCom.SetAllowEdit(View, "CS", false);
                }
            }
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAction.Executing += SaveAction_Executing;
            }
        }

        private void SaveAction_Executing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (View.CurrentObject is Phieudexuat p)
            {
                if (p.DuyetDexuats.Count == 0)
                {
                    e.Cancel = true;
                    TCom.CustomError("Phiếu đề xuất này không có bước duyệt");
                }
            }
        }

    }
}
