using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Office.Blazor.Components.Models;
using DevExpress.ExpressApp.Office.Blazor.Editors;
using DevExpress.ExpressApp.Office.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class HuongdanController : ObjectViewController<DetailView, Huongdan>
    {
        private bool bAdmin = false;
        protected override void OnActivated()
        {
            base.OnActivated();
            ApplicationUser CurentUser = ObjectSpace.GetObjectByKey<ApplicationUser>(SecuritySystem.CurrentUserId);
            if (CurentUser != null)
            {
                if (!string.IsNullOrEmpty(CurentUser.UserName))
                {
                    if (CurentUser.UserName.ToLower() == "admin")
                        bAdmin = true;
                }
            }
            View.AllowDelete.SetItemValue("admin", bAdmin);
            View.AllowEdit.SetItemValue("admin", bAdmin);
            View.AllowNew.SetItemValue("admin", bAdmin);
                RefreshController refreshController=Frame.GetController<RefreshController>();
                refreshController?.Active.SetItemValue("admin", bAdmin);
                ModificationsController modificationsController=Frame.GetController<ModificationsController>();
                if (modificationsController != null)
                {
                    modificationsController.SaveAction.Active.SetItemValue("admin", bAdmin);
                    modificationsController.SaveAndCloseAction.Active.SetItemValue("admin", bAdmin);
                    modificationsController.SaveAndNewAction.Active.SetItemValue("admin", bAdmin);
                }

            foreach (RichTextPropertyEditor editor in View.GetItems<RichTextPropertyEditor>())
            {
                if (editor.Control != null)
                {
                    CustomizeRichEditComponentModel(((RichTextEditorComponentAdapter)editor.Control).ComponentModel);
                }
                else
                {
                    editor.ControlCreated += Editor_ControlCreated;
                }
            }
        }
        private void Editor_ControlCreated(object sender, EventArgs e)
        {
            CustomizeRichEditComponentModel(((RichTextEditorComponentAdapter)((RichTextPropertyEditor)sender).Control).ComponentModel);
        }
        private void CustomizeRichEditComponentModel(DxRichEditModel richEditModel)
        {
            if (!bAdmin)
            {
                richEditModel.ViewType = DevExpress.Blazor.RichEdit.ViewType.Simple;
                richEditModel.BarMode = DevExpress.Blazor.Office.BarMode.None;
                richEditModel.ReadOnly = true;
            }
            else
            {
                richEditModel.BarMode = DevExpress.Blazor.Office.BarMode.Ribbon;
                richEditModel.ReadOnly = false;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            foreach (RichTextPropertyEditor editor in View.GetItems<RichTextPropertyEditor>())
            {
                editor.ControlCreated -= Editor_ControlCreated;
            }
        }
    }
}
