using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors.Grid;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using Trinhky.Blazor.Server.Pages;

namespace Trinhky.Blazor.Server.Controllers
{
    public class CommandController:ViewController<ListView>
    {
        readonly string TargetViewIds = "Phieudexuat_DuyetDexuats_ListView;";
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (TargetViewIds.Contains(View.Id + ";"))
            {
                if (View.Editor is DxGridListEditor gridListEditor)
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    if (dataGridAdapter != null)
                    {
                        dataGridAdapter.GridCommandColumnModel.Visible = false;
                        dataGridAdapter.GridCommandColumnModel.NewButtonVisible = false;
                        dataGridAdapter.GridCommandColumnModel.EditButtonVisible = false;
                    }
                }

            }

        }
    }
}
