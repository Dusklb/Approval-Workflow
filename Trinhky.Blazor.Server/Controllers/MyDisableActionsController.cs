using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;

namespace Trinhky.Blazor.Server.Controllers
{
    public class MyDisableActionsController : DisableActionsController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is ListView)
            {
                Frame.GetController<ResetViewSettingsController>()?.Active.RemoveItem("TemporaryNotSupported");
            }
        }
    }
}
