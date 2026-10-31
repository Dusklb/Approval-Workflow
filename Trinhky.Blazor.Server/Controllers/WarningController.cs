using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using Microsoft.JSInterop;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class WarningController : WindowController
    {
        public SimpleAction ListChoduyet;
        public WarningController()
        {
            ListChoduyet = new(this, "ListChoduyet", "QuickAccess")
            {
                Caption = "Chờ duyệt",
                ImageName = "choduyet",
                ToolTip = "Số văn bản chờ duyệt"
            };
            ListChoduyet.Execute += ListChoduyet_Execute;

            //SimpleAction Quaylai = new(this, "QuaylaiChung", "QuickAccess")
            //{
            //    Caption = "Quay lại",
            //    ImageName = "back",
            //    ToolTip = "Quay lại trang trước"
            //};
            //Quaylai.Execute += Quaylai_Execute;

        }
        //private void Quaylai_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
        //    System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("navigateBack"));
        //}

        private void ListChoduyet_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace(typeof(Phieuduyet));
            string lookupViewID = "Phieuduyet_ListView";
            CollectionSourceBase col = Application.CreateCollectionSource(os, typeof(Phieuduyet), lookupViewID);
            e.ShowViewParameters.CreatedView = Application.CreateListView(lookupViewID, col, true);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateCount();
        }
        public void UpdateCount()
        {
            string info = TCom.Sochoduyet();
            if (string.IsNullOrEmpty(info))
                ListChoduyet.Active.SetItemValue("thuong", false);
            else
                ListChoduyet.Caption = "Chờ duyệt (" + info + ")";
        }
    }
    //public class UpdatewarningController : ViewController
    //{
    //    public UpdatewarningController()
    //    {
    //        TargetViewNesting = Nesting.Root;
    //        TargetObjectType = typeof(DuyetDexuat);
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        View.ObjectSpace.Committed += ObjectSpace_Committed;
    //    }
    //    private void ObjectSpace_Committed(object sender, System.EventArgs e)
    //    {
    //        Controller controller = Frame.GetController<WarningController>();
    //        if (controller != null)
    //        {
    //            var act = controller.Actions["ListChoduyet"];
    //            if (act != null)
    //            {
    //                string info = TCom.Sochoduyet();
    //                if (string.IsNullOrEmpty(info))
    //                    act.Active.SetItemValue("thuong", false);
    //                else
    //                    act.Caption = "Chờ duyệt (" + info + ")";
    //            }
    //        }
    //        if (View.CurrentObject is DuyetDexuat duyet)
    //        {
    //            BCom.UpdateTrangthaiPhieu(duyet.Phieu.Oid);
    //            TCom.SendEmailDuyet(duyet);
    //        }
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        View.ObjectSpace.Committed -= ObjectSpace_Committed;
    //        base.OnDeactivated();
    //    }
    //}

}
