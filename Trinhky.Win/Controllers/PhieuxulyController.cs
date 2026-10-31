using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class PhieuxulyController : ViewController
    {
        private ListViewProcessCurrentObjectController openPhieu;
        public PhieuxulyController()
        {
            TargetViewId = "PhieuXuly_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is PhieuXuly p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dView;
                    e.InnerArgs.ShowViewParameters.Context = TemplateContext.View;
                    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem -= OpenChungtu_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
}
