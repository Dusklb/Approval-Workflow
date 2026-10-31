using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;

namespace Trinhky.Module.Controllers
{
    public class SavesController : ViewController<DetailView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAndCloseAction.Active.SetItemValue("dis", false);
                modificationsController.SaveAndNewAction.Active.SetItemValue("dis", false);
            }
        }
    }
}
