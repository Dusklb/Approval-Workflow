using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Templates.Toolbar.ActionControls;
using DevExpress.ExpressApp.SystemModule;

namespace Trinhky.Blazor.Server.Controllers
{
    public class CustomizeButtons:ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            DeleteObjectsViewController deleteObjectsViewController=Frame.GetController<DeleteObjectsViewController>();
            if (deleteObjectsViewController != null)
            {
                deleteObjectsViewController.DeleteAction.CustomizeControl += DeleteAction_CustomizeControl;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteObjectsViewController != null)
            {
                deleteObjectsViewController.DeleteAction.CustomizeControl -= DeleteAction_CustomizeControl;
            }
        }
        private void DeleteAction_CustomizeControl(object sender, DevExpress.ExpressApp.Actions.CustomizeControlEventArgs e)
        {
            if (e.Control is DxToolbarItemActionControlBase toolbarItemControl)
            {
                toolbarItemControl.ToolbarItemModel.CssClass = "mybtn";
            }
            //if (e.Control is DxToolbarItemSimpleActionControl  dxToolbarItemSimpleActionControl)
            //{
            //    dxToolbarItemSimpleActionControl.ToolbarItemModel.CssClass = "mybtn";
            //}
        }
    }
}
