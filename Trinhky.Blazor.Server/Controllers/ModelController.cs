using DevExpress.ExpressApp;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ModelController:ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ModelSaving += View_ModelSaving;
        }

        protected override void OnDeactivated()
        {
            View.ModelSaving -= View_ModelSaving;
            base.OnDeactivated();
        }
        private void View_ModelSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel=true;
        }
    }
}
