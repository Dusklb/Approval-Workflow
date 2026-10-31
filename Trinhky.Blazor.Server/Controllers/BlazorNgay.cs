using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Templates.Toolbar.ActionControls;
using Microsoft.AspNetCore.Components;
using Trinhky.Module.Controllers;

namespace Trinhky.Blazor.Server.Controllers
{
    public class BlazorNgay: WindowController
    {
        private ParametrizedAction tungay;
        private ParametrizedAction denngay;
        protected override void OnActivated()
        {
            base.OnActivated();
            Controller controller = Frame.GetController<DatesController>();
            if(controller!= null )
            {
                tungay = Frame.GetController<DatesController>().actionTungay;
                if (tungay != null) { tungay.CustomizeControl += Tungay_CustomizeControl; }
                denngay = Frame.GetController<DatesController>().actionDenngay;
                if (denngay != null) { denngay.CustomizeControl += Denngay_CustomizeControl; }
            }
        }

        private void Denngay_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is DxToolbarItemParametrizedActionControl actionControl &&
                        actionControl.EditModel is DxDateEditModel dateEditModel)
            {
                dateEditModel.Format = "dd/MM/yyyy";
                dateEditModel.CssClass += " width90";
                actionControl.ButtonModel.CssClass += "d-none"; //make button invisble;
                DxDateEditModel<DateTime> dateEditModel1 = (DxDateEditModel<DateTime>)actionControl.EditModel;
                var defaultDateChanged = dateEditModel1.DateChanged;
                dateEditModel1.DateChanged = EventCallback.Factory.Create<DateTime>(this, async date => {
                    await defaultDateChanged.InvokeAsync(date);
                    ((ParametrizedAction)sender).DoExecute(date);
                });
            }

        }

        private void Tungay_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is DxToolbarItemParametrizedActionControl actionControl &&
                        actionControl.EditModel is DxDateEditModel dateEditModel)
            {
                dateEditModel.Format = "dd/MM/yyyy";
                dateEditModel.CssClass += " width90";
                actionControl.ButtonModel.CssClass += "d-none"; //make button invisble;
                DxDateEditModel<DateTime> dateEditModel1 = (DxDateEditModel<DateTime>)actionControl.EditModel;
                var defaultDateChanged = dateEditModel1.DateChanged;
                dateEditModel1.DateChanged = EventCallback.Factory.Create<DateTime>(this, async date => {
                    await defaultDateChanged.InvokeAsync(date);
                    ((ParametrizedAction)sender).DoExecute(date);
                });
            }
        }
        protected override void OnDeactivated()
        {
            if (tungay != null) { tungay.CustomizeControl -= Tungay_CustomizeControl; tungay = null; }
            if (denngay != null) { denngay.CustomizeControl -= Denngay_CustomizeControl; denngay = null; }
            base.OnDeactivated();
        }
    }
}
