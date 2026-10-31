using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Layout;
using DevExpress.ExpressApp.SystemModule;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class BlazorDexuatController : ViewController<DetailView>
    {
        BlazorLayoutManager layoutManager;
        private SimpleAction XulyThucong;
        public BlazorDexuatController()
        {
            TargetObjectType = typeof(Phieudexuat);
            XulyThucong = new(this, "XulyThucong", "View")
            {
                Caption = "Cập nhật thủ công",
                TargetViewId = "Phieudexuat_DetailView"
            };
            XulyThucong.Execute += XulyThucong_Execute;
        }

        private void XulyThucong_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if(View.CurrentObject is Phieudexuat p)
            {
                BCom.UpdateTrangthaiPhieu(p.Oid);
                ObjectSpace.Refresh();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            layoutManager = (BlazorLayoutManager)View.LayoutManager;
            layoutManager.ItemCreated += OnItemCreated;

            bool bAdmin = TCom.IsAdmin();
            if (!bAdmin) { XulyThucong.Active.SetItemValue("an", false); }

        }
        private void OnItemCreated(object sender, BlazorLayoutManager.ItemCreatedEventArgs e)
        {
            if (View.CurrentObject is Phieudexuat p)
            {
                if (p.Oid == -1)
                {
                    if (e.ModelLayoutElement.Id == "Item2" && e.LayoutControlItem is DxFormLayoutTabPagesModel tabbedGroup)
                    {
                        tabbedGroup.ActiveTabIndex = 1;
                        tabbedGroup.ActiveTabIndexChanged = EventCallback.Factory.Create<int>(this, index => tabbedGroup.ActiveTabIndex = index);
                    }
                }
                else
                {
                    if (e.ModelLayoutElement.Id == "Item2" && e.LayoutControlItem is DxFormLayoutTabPagesModel tabbedGroup)
                    {
                        tabbedGroup.ActiveTabIndex = 0;
                        tabbedGroup.ActiveTabIndexChanged = EventCallback.Factory.Create<int>(this, index => tabbedGroup.ActiveTabIndex = index);
                    }
                }
            }
        }
        protected override void OnDeactivated()
        {
            if (layoutManager is not null)
            {
                layoutManager.ItemCreated -= OnItemCreated;
                layoutManager = null;
            }
            base.OnDeactivated();
        }
    }
    public class BlazorListDexuatController : ObjectViewController<ListView, Phieudexuat>
    {
        private ListViewProcessCurrentObjectController openPhieu;
        private readonly SimpleAction Thucong;
        public BlazorListDexuatController()
        {
            SimpleAction newPhieu = new(this, "newPhieu", "View")
            {
                Caption = "Lập phiếu",
                ImageName = "dxmoi",
                TargetViewId = "Phieudexuat_ListView"
            };
            newPhieu.Execute += NewPhieu_Execute;

            Thucong = new(this, "Thucong", "View")
            {
                Caption = "Thủ công",
                TargetViewId = "Phieudexuat_ListView",
                SelectionDependencyType=SelectionDependencyType.RequireSingleObject
            };
            Thucong.Execute += Thucong_Execute;
        }

        private void Thucong_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is Phieudexuat p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    //TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.ShowViewParameters.CreatedView = dView;
                    e.ShowViewParameters.Context = TemplateContext.View;
                    e.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
        }

        private void NewPhieu_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
            Phieudexuat pmoi = objectSpace.CreateObject<Phieudexuat>();
            DetailView dView = Application.CreateDetailView(objectSpace, "Phieumoi", true, pmoi);
            Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Lapphieu(pmoi, objectSpace));
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            openPhieu = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (openPhieu != null)
            {
                openPhieu.CustomProcessSelectedItem += OpenChungtu_CustomProcessSelectedItem;
            }
            bool bAdmin = TCom.IsAdmin();
            if (!bAdmin) { Thucong.Active.SetItemValue("an", false); }
        }
        void OpenChungtu_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            if (View.CurrentObject is Phieudexuat p)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", p.Oid));
                if (phieu != null)
                {
                    TCom.AddUserValue("viewdx", phieu.Oid.ToString());
                    DashboardView dashboardView = Application.CreateDashboardView(objectSpace, "viewDX", true);
                    //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, phieu);
                    e.InnerArgs.ShowViewParameters.CreatedView = dashboardView;
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

        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        void Lapphieu(Phieudexuat phieu, IObjectSpace objectSpace)
        {
            if (phieu.Mau != null)
            {
                if (phieu.Mau.ChonNV)
                {
                    if (TCom.LapphieuTheoChuthe(phieu, objectSpace))
                    {
                        IObjectSpace objectSpace1 = Application.CreateObjectSpace(typeof(Phieudexuat));
                        Phieudexuat p = objectSpace1.GetObject(phieu);
                        TCom.AddUserValue("viewdx", p.Oid.ToString());
                        DashboardView dashboardView = Application.CreateDashboardView(objectSpace1, "viewDX", true);
                        ShowViewParameters svp = new()
                        {
                            CreatedView = dashboardView,
                            TargetWindow = TargetWindow.Default,
                            Context = TemplateContext.View
                        };
                        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    }
                }
                else
                {
                    if (TCom.LapphieuDX(phieu, objectSpace))
                    {
                        IObjectSpace objectSpace1 = Application.CreateObjectSpace(typeof(Phieudexuat));
                        Phieudexuat p = objectSpace1.GetObject(phieu);
                        TCom.AddUserValue("viewdx", p.Oid.ToString());
                        DashboardView dashboardView = Application.CreateDashboardView(objectSpace1, "viewDX", true);
                        ShowViewParameters svp = new()
                        {
                            CreatedView = dashboardView,
                            TargetWindow = TargetWindow.Default,
                            Context = TemplateContext.View
                        };
                        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    }
                }
            }
        }
    }
}
