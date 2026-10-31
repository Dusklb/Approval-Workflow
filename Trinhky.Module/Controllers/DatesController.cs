using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class DatesController : ViewController
    {
        public ParametrizedAction actionTungay;
        public ParametrizedAction actionDenngay;
        private readonly string TargetId = "DexuatCanhan_ListView;DuyetDexuatCanhan_ListView;ThongkeDexuat_ListView;ThongkeDuyet_ListView;ThongkeDexuatMau_ListView;Phieudexuat_ListView;";
        public DatesController()
        {
            TargetViewId = TargetId;
            actionTungay = new ParametrizedAction(this, "actionTungay", "View", typeof(DateTime))
            {
                Caption = "Từ ngày",
                TargetViewId = TargetId
            };
            actionTungay.Execute += ActionTungay_Execute;

            actionDenngay = new ParametrizedAction(this, "actionDenngay", "View", typeof(DateTime))
            {
                Caption = "Đến ngày",
                TargetViewId = TargetId
            };
            actionDenngay.Execute += ActionDenngay_Execute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            string ngay = TCom.GetUserStringValue("tungay");
            _ = DateTime.TryParse(ngay, out DateTime fTungay);
            _ = DateTime.TryParse(fTungay.ToShortDateString() + " 00:00:01", out fTungay);

            ngay = TCom.GetUserStringValue("denngay");
            _ = DateTime.TryParse(ngay, out DateTime fDenngay);
            _ = DateTime.TryParse(fDenngay.ToShortDateString() + " 23:59:59", out fDenngay);

            actionTungay.Value = fTungay;
            actionDenngay.Value = fDenngay;
        }
        private void ActionDenngay_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (e.ParameterCurrentValue != null)
                TCom.AddUserValue("denngay", e.ParameterCurrentValue.ToString());
        }

        private void ActionTungay_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (e.ParameterCurrentValue != null)
                TCom.AddUserValue("tungay", e.ParameterCurrentValue.ToString());
        }

    }
}
