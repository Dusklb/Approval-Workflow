using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;

namespace Trinhky.Blazor.Server.Controllers
{
    public class CustomRefreshController : ViewController
    {
        public CustomRefreshController()
        {
            TargetViewId = "HuongdanSD;viewDX;upload;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            RefreshController refreshController = Frame.GetController<RefreshController>();
            refreshController?.Active.SetItemValue("an", false);
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            RefreshController refreshController = Frame.GetController<RefreshController>();
            refreshController?.Active.RemoveItem("an");
        }
    }
}
