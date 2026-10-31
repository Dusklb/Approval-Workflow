using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using System;

namespace Trinhky.Win.Controllers
{
    public class FontController : WindowController
    {
        static FontController()
        {
            AppearanceObject.DefaultFont = new Font("Segoe UI", 9);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Window.TemplateChanged += Window_TemplateChanged;
        }
        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            if (Frame.Template is IBarManagerHolder holder)
            {
                BarManager manager = holder.BarManager;
                manager.Controller ??= new BarAndDockingController();
                var controller = manager.Controller;
                controller.AppearancesBar.MainMenu.Font = AppearanceObject.DefaultFont;
                controller.AppearancesBar.ItemsFont = AppearanceObject.DefaultFont;
            }
        }
        protected override void OnDeactivated()
        {
            Window.TemplateChanged -= Window_TemplateChanged;
            base.OnDeactivated();
        }
    }
}
