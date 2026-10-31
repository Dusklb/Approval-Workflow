using DevExpress.ExpressApp.Security;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class ChangePassInfo:ChangePasswordController
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            this.ChangeMyPasswordAction.ExecuteCompleted += ChangeMyPasswordAction_ExecuteCompleted;
        }

        private void ChangeMyPasswordAction_ExecuteCompleted(object sender, DevExpress.ExpressApp.Actions.ActionBaseEventArgs e)
        {
            TCom.CustomInfo("Bạn đã thay đổi mật khẩu thành công");
        }
        protected override void OnDeactivated()
        {
            this.ChangeMyPasswordAction.ExecuteCompleted-= ChangeMyPasswordAction_ExecuteCompleted;
            base.OnDeactivated();
        }
    }
}
