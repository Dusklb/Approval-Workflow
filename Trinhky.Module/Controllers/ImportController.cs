using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class ImportController : ViewController
    {
        public ImportController()
        {
            SimpleAction ImpNV = new(this, "ImpNV", "View")
            {
                Caption = "Cập nhật nhân viên",
                ImageName = "import",
                ToolTip = "Cập nhật dữ liệu nhân viên qua Windows Clipboard",
                TargetViewId = "Bophan_ListView"
            };
            ImpNV.Execute += ImpNV_Execute;
        }

        private void ImpNV_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ClsImport));
                ClsImport NewImport = objectSpace.CreateObject<ClsImport>();
                DetailView NewView = Application.CreateDetailView(objectSpace, NewImport);
                NewView.Caption = "Cập nhật nhân viên";
                e.ShowViewParameters.CreatedView = NewView;
                e.ShowViewParameters.Context = TemplateContext.View;
                e.ShowViewParameters.TargetWindow = TargetWindow.Default;
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.Message);
            }
        }
    }
}
