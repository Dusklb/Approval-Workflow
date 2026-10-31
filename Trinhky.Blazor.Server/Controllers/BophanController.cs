using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace Trinhky.Blazor.Server.Controllers
{
    public class BophanController : ViewController
    {
        public BophanController()
        {
            TargetViewId = "Bophan_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            SetFilter();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            this.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            SetFilter();
        }
        private void SetFilter()
        {
            ((ListView)View).CollectionSource.Criteria["filter"] = CriteriaOperator.Parse("[Parent] Is Null");
        }

    }
}
