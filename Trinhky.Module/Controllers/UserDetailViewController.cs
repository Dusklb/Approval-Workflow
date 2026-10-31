using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class UserDetailViewController : ObjectViewController<DetailView, ApplicationUser>
    {
        string ActiveKey = "quanly";
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CurrentObjectChanged += View_CurrentObjectChanged;
            UpdateVisibility();
            UpdateControllers(false);
        }

        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            UpdateControllers(true);
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
        }
        private void UpdateVisibility()
        {
            if (!TCom.IsAdmin())
            {
                TCom.SetAllowView(View, "Roles", false);
                TCom.SetAllowEdit(View, "ChangePasswordOnFirstLogon", false);
                TCom.SetAllowEdit(View, "IsActive", false);
                //TCom.SetAllowEdit(View, "Loai", false);
                TCom.SetAllowEdit(View, "UserName", false);
                //TCom.SetAllowEdit(View, "Photo", false);
            }
            else
            {
                TCom.SetAllowView(View, "Roles", true);
                TCom.SetAllowEdit(View, "ChangePasswordOnFirstLogon", true);
                TCom.SetAllowEdit(View, "IsActive", true);
                //TCom.SetAllowEdit(View, "Loai", true);
                TCom.SetAllowEdit(View, "UserName", true);
                //TCom.SetAllowEdit(View, "Photo", true);
            }
        }
        private void UpdateControllers(bool bActive)
        {
            if (!TCom.IsAdmin())
            {
                RefreshController controller2 = Frame.GetController<RefreshController>();
                if (controller2 != null)
                {
                    controller2.RefreshAction.Active[ActiveKey] = bActive;
                }
                ModificationsController mod = Frame.GetController<ModificationsController>();
                if (mod != null)
                {
                    mod.SaveAction.Active[ActiveKey] = bActive;
                    mod.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
                    mod.SaveAndCloseAction.Active[ActiveKey] = bActive;
                    mod.SaveAndNewAction.Active[ActiveKey] = bActive;
                    mod.CancelAction.Active[ActiveKey] = bActive;
                }
                DeleteObjectsViewController controller4 = Frame.GetController<DeleteObjectsViewController>();
                if (controller4 != null)
                {
                    controller4.Active[ActiveKey] = bActive;
                }
                NewObjectViewController controller5 = Frame.GetController<NewObjectViewController>();
                if (controller5 != null)
                {
                    controller5.Active[ActiveKey] = bActive;
                }
            }
        }
    }

    //public class QuanlyController : ViewController
    //{
    //    public QuanlyController()
    //    {
    //        TargetViewId = "ApplicationUser_DetailView";
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        ApplicationUser CurentUser = ObjectSpace.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
    //        if (CurentUser != null)
    //        {
    //            if (CurentUser.UserName == "quanly" || CurentUser.UserName == "guest")
    //            {
    //                string ActiveKey = "quanly";
    //                ChangePasswordController ResetPasswordAction = Frame.GetController<ChangePasswordController>();
    //                if (ResetPasswordAction != null)
    //                {
    //                    //ResetPasswordAction.Active.RemoveItem(BaseManagePasswordController.IsChangePasswordSupportedKey);
    //                    ResetPasswordAction.Active[ActiveKey] = false;
    //                    //controller5.Active[ActiveKey] = false;
    //                }
    //                RefreshController controller2 = Frame.GetController<RefreshController>();
    //                if (controller2 != null)
    //                {
    //                    controller2.RefreshAction.Active[ActiveKey] = false;
    //                }
    //                ModificationsController mod = Frame.GetController<ModificationsController>();
    //                if (mod != null)
    //                {
    //                    mod.SaveAction.Active[ActiveKey] = false;
    //                    mod.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
    //                    //mod.SaveAndCloseAction.Active[ActiveKey] = bActive;
    //                    //mod.SaveAndNewAction.Active[ActiveKey] = bActive;
    //                    //mod.CancelAction.Active[ActiveKey] = bActive;
    //                }
    //                DeleteObjectsViewController controller4 = Frame.GetController<DeleteObjectsViewController>();
    //                if (controller4 != null)
    //                {
    //                    controller4.Active[ActiveKey] = false;
    //                }
    //                NewObjectViewController controller5 = Frame.GetController<NewObjectViewController>();
    //                if (controller5 != null)
    //                {
    //                    controller5.Active[ActiveKey] = false;
    //                }
    //            }
    //        }

    //    }
    //}

}
