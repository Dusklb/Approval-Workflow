using DevExpress.ExpressApp.Blazor.Templates;
using DevExpress.ExpressApp;

namespace Trinhky.Blazor.Server.Controllers
{
    public class CustomizePopupWindows : ViewController
    {
        public CustomizePopupWindows()
        {
            TargetViewId = "Donvi_DetailView;Bophan_DetailView;Quatrinhduyet_DetailView;DuyetDexuat_DetailView;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            CustomizeTemplate();
            Frame.TemplateChanged += Frame_TemplateChanged;
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Frame.TemplateChanged -= Frame_TemplateChanged;
        }
        private void Frame_TemplateChanged(object sender, EventArgs e)
        {
            CustomizeTemplate();
        }
        private void CustomizeTemplate()
        {
            if (Frame.Template is PopupWindowTemplate popupWindowTemplate)
            {
                popupWindowTemplate.MaxWidth = "90vw !important;";
                popupWindowTemplate.MaxHeight = "90vh !important;";
            }
        }
    }

}
