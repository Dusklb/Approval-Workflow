using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;

namespace Trinhky.Blazor.Server.Controllers
{
    public class HideExportController:ViewController
    {
        public HideExportController()
        {
            TargetViewId = "Phieudexuat_DuyetDexuats_ListView;DuyetDexuat_Quatrinhduyets_ListView;Phieudexuat_PhieudexuatFiles_ListView;Phieudexuat_Chinhsuas_ListView;" +
                "ChiaseList_DanhsachChons_ListView;ChiaseList_DanhsachNVs_ListView;ClsImportVT_ImportHangs_ListView;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            BlazorExportController exportController = Frame.GetController<BlazorExportController>();
            exportController?.Active.SetItemValue("hide", false);
        }
        protected override void OnDeactivated()
        {
            BlazorExportController exportController = Frame.GetController<BlazorExportController>();
            exportController?.Active.SetItemValue("hide", true);
            base.OnDeactivated();
        }
    }
}
