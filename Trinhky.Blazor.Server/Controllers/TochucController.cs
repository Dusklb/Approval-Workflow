using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.SystemModule;
using Microsoft.AspNetCore.Components;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class TochucController:ViewController
    {
        public TochucController()
        {
            TargetViewId = "CayTCDashboard";
            SimpleAction newbp = new(this, "newbp", "View")
            {
                Caption = "Bộ phận mới",
                ImageName="thembophan",
                TargetViewId= "CayTCDashboard"
            };
            newbp.Execute += Newbp_Execute;

            SimpleAction editbp = new(this, "editbp", "View")
            {
                Caption = "Sửa bộ phận",
                ImageName = "editbophan",
                TargetViewId = "CayTCDashboard"
            };
            editbp.Executed += Editbp_Executed;
        }

        private void Editbp_Executed(object sender, ActionBaseEventArgs e)
        {
            if (TCom.CurrentBophanID > 0)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Bophan));
                Bophan bp = objectSpace.FindObject<Bophan>(CriteriaOperator.Parse("Oid=?", TCom.CurrentBophanID));
                if (bp != null) 
                {
                    DetailView dView = Application.CreateDetailView(objectSpace, "Bophan_DetailView", true, bp);
                    e.ShowViewParameters.CreatedView = dView;
                }
            }
        }

        private void Newbp_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Bophan));
            Bophan newnode = objectSpace.CreateObject<Bophan>();
            if (TCom.CurrentBophanID>0)
            {
                Bophan capcha=objectSpace.FindObject<Bophan>(CriteriaOperator.Parse("Oid=?",TCom.CurrentBophanID));
                if(capcha!=null) { newnode.Parent=capcha; }
            }
            DetailView dView = Application.CreateDetailView(objectSpace, "Bophan_DetailView", true, newnode);
            e.ShowViewParameters.CreatedView=dView;
            //DialogController dc = Application.CreateController<DialogController>();
            //dc.SaveOnAccept = false;
            //dc.Accepting += Dc_Accepting; ;
            //e.ShowViewParameters.Controllers.Add(dc);
        }

    }
}
