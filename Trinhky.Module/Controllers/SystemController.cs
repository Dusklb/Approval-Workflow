using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace Trinhky.Module.Controllers
{
    public class SystemController : ViewController
    {
        private const string ActiveKey = "Inactive in ListView";
        private Type objType = null;
        protected override void OnActivated()
        {
            base.OnActivated();
            RecordsNavigationController reccontroller = Frame.GetController<RecordsNavigationController>();
            if (reccontroller != null)
            {
                bool active = false;// !(View is ListView);
                reccontroller.PreviousObjectAction.Active[ActiveKey] = active;
                reccontroller.NextObjectAction.Active[ActiveKey] = active;
            }
            //DevExpress.ExpressApp.SystemModule.FilterController filterController = Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>();
            //filterController?.Active.SetItemValue(ActiveKey, false);
            //new controller
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                if (View.ObjectTypeInfo != null)
                {
                    objType = View.ObjectTypeInfo.Type;
                    controller.CollectCreatableItemTypes += NewObjectViewController_CollectCreatableItemTypes;
                    controller.CollectDescendantTypes += NewObjectViewController_CollectDescendantTypes;
                    if (controller.Active)
                    {
                        controller.UpdateNewObjectAction();
                    }
                }
            }
            ResetViewSettingsController resetViewSettingsController = Frame.GetController<ResetViewSettingsController>();
            if (resetViewSettingsController != null)
                resetViewSettingsController.Active[ActiveKey] = false;
        }
        private void NewObjectViewController_CollectDescendantTypes(object sender, CollectTypesEventArgs e)
        {
            CustomizeList(e.Types);
        }
        private void NewObjectViewController_CollectCreatableItemTypes(object sender, CollectTypesEventArgs e)
        {
            CustomizeList(e.Types);
        }
        private void CustomizeList(ICollection<Type> types)
        {
            if (objType != null)
            {
                List<Type> unusableTypes = new();
                foreach (Type item in types)
                {
                    if (item != objType)
                    {
                        unusableTypes.Add(item);
                    }
                }
                foreach (Type item in unusableTypes)
                {
                    types.Remove(item);
                }
            }
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                controller.SaveAction.ExecuteCompleted += SaveAction_ExecuteCompleted;
            }
        }
        void SaveAction_ExecuteCompleted(object sender, DevExpress.ExpressApp.Actions.ActionBaseEventArgs e)
        {
            Application.ShowViewStrategy.ShowMessage("Tác vụ dữ liệu đã hoàn tất!", InformationType.Success, 3000, InformationPosition.Top);
            View.ObjectSpace.Refresh();
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                /*
                if (tmSalesComm.Demo)
                {
                    if (objType == typeof(sPNhap) || objType == typeof(sPXuat))
                    {
                        controller.Active.RemoveItem("dungthu");
                    }

                }
                */
                controller.CollectCreatableItemTypes -= NewObjectViewController_CollectCreatableItemTypes;
                controller.CollectDescendantTypes -= NewObjectViewController_CollectDescendantTypes;
            }
            base.OnDeactivated();
        }
    }

}
