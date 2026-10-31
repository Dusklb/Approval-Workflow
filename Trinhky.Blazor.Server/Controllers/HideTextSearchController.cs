using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Trinhky.Blazor.Server.Controllers
{
    public class HideTextSearchController : ViewController
    {
        public HideTextSearchController()
        {
            TargetViewId = "Phieudexuat_DuyetDexuats_ListView;DuyetDexuat_Quatrinhduyets_ListView;Phieudexuat_PhieudexuatFiles_ListView;Phieudexuat_Chinhsuas_ListView;" +
                "ChiaseList_DanhsachChons_ListView;ChiaseList_DanhsachNVs_ListView;ClsImportVT_ImportHangs_ListView;Vattu_LookupListView;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            FilterController filterController = Frame.GetController<FilterController>();
            filterController.FullTextFilterAction?.Active.SetItemValue("hide", false);
        }
        protected override void OnDeactivated()
        {
            FilterController filterController = Frame.GetController<FilterController>();
            filterController.FullTextFilterAction?.Active.SetItemValue("hide", true);
            base.OnDeactivated();
        }
    }
}
