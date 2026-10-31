using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class SingleRowController: ObjectViewController<ListView,Phieudexuat>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            bool bAdmin = TCom.IsAdmin();
            if (View.Editor is DxGridListEditor gridList && !bAdmin)
            {
                IDxGridAdapter dataGridAdapter = gridList.GetGridAdapter();
                dataGridAdapter.GridModel.SelectionMode = GridSelectionMode.Single;
            }
        }
    }
    public class XulySingleRowController : ObjectViewController<ListView, PhieuXuly>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (View.Editor is DxGridListEditor gridList)
            {
                IDxGridAdapter dataGridAdapter = gridList.GetGridAdapter();
                dataGridAdapter.GridModel.SelectionMode = GridSelectionMode.Single;
            }
        }
    }
}
