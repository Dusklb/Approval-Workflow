using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.SystemModule;
using Microsoft.JSInterop;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class SaveCloseController: ViewController<DetailView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAndCloseAction.Active.SetItemValue("an", false);
                modificationsController.SaveAndNewAction.Active.SetItemValue("an", false);
            }
        }
    }
}
