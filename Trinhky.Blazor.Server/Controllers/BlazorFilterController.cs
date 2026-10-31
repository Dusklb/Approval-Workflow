using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Model;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ColumnFilterController : ViewController<ListView>
    {
        public ColumnFilterController()
        {
            //TargetViewId = "Phieudexuat_DuyetDexuats_ListView;Phieudexuat_PhieudexuatFiles_ListView;Phieudexuat_Chinhsuas_ListView;";
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            //IModelListView modelListView=View.
            if (View.Editor is DxGridListEditor gridListEditor)
            {
                IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                dataGridAdapter.GridModel.FilterMenuButtonDisplayMode = DevExpress.Blazor.GridFilterMenuButtonDisplayMode.Always;
                //foreach (DxGridDataColumnModel columnModel in dataGridAdapter.GridDataColumnModels)
                //{
                //    columnModel.FilterMenuButtonDisplayMode = DevExpress.Blazor.GridFilterMenuButtonDisplayMode.Never;
                //}
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
    }
    public class ImportVTFilterController : ViewController<ListView>
    {
        public ImportVTFilterController()
        {
            TargetViewId = "ClsImportVT_ImportHangs_ListView;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            FilterEditorController filterEditorController = Frame.GetController<FilterEditorController>();
            filterEditorController?.Active.SetItemValue("dis", false);
        }
        protected override void OnDeactivated()
        {
            FilterEditorController filterEditorController = Frame.GetController<FilterEditorController>();
            filterEditorController?.Active.SetItemValue("dis", true);
            base.OnDeactivated();
        }
    }

}
