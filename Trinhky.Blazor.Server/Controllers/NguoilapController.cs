using DevExpress.Blazor;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class NguoilapController : ViewController
    {
        public NguoilapController() 
        {
            TargetViewId = "Phieumoi";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if(View is DetailView v)
            {
                v.CustomizeViewItemControl<LookupPropertyEditor>(this, CustomizeLookupPropertyEditor);

            }
        }
        private void CustomizeLookupPropertyEditor(LookupPropertyEditor lookupPropertyEditor)
        {
            if (lookupPropertyEditor.Control is DxComboBoxAdapter comboBoxAdapter && lookupPropertyEditor.PropertyName== "Nhanvien")
            {
                comboBoxAdapter.ComponentModel.ListRenderMode = ListRenderMode.Entire;
            }
        }
    }
}
