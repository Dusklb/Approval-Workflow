using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.SystemModule;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ChiaseListNVController : ObjectViewController<ListView, DanhsachNV>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (View.Editor is DxGridListEditor gridList)
            {
                IDxGridAdapter dataGridAdapter = gridList.GetGridAdapter();
                dataGridAdapter.GridModel.SelectionMode = GridSelectionMode.Single;
            }
        }
    }
    public class ChiaseDetailViewController : ViewController
    {
        public ChiaseDetailViewController()
        {
            TargetViewId = "ChiaseList_DetailView";
            SimpleAction ChiaseDX = new(this, "ChiaseDX2", "View")
            {
                Caption = "Chia sẻ",
                ImageName = "share",
                ToolTip = "Chia sẻ đề xuất",
                TargetViewId = "ChiaseList_DetailView",
            };
            ChiaseDX.Execute += ChiaseDX_Execute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            DeleteObjectsViewController deleteObjectsViewController=Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController?.Active.SetItemValue("an", false);
            ModificationsController modificationsController=Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAction.Active.SetItemValue("an", false);
                modificationsController.SaveAndCloseAction.Active.SetItemValue("an", false);
                modificationsController.SaveAndNewAction.Active.SetItemValue("an", false);
            }
            RefreshController refreshController=Frame.GetController<RefreshController>();
            refreshController?.Active.SetItemValue("an", false);
        }
        protected override void OnDeactivated()
        {
            DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController?.Active.RemoveItem("an");
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAction.Active.RemoveItem("an");
                modificationsController.SaveAndCloseAction.Active.RemoveItem("an");
                modificationsController.SaveAndNewAction.Active.RemoveItem("an");
            }
            RefreshController refreshController = Frame.GetController<RefreshController>();
            refreshController?.Active.RemoveItem("an");
            base.OnDeactivated();
        }
        private void ChiaseDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if(View.CurrentObject is ChiaseList chiaselist)
            {
                if(chiaselist.DanhsachChons.Count>0)
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Chiase));
                    Phieudexuat phieudexuat = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", chiaselist.PhieuOid));
                    Nhansu nv = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Oid=?", chiaselist.NhanvienOid));
                    if(nv != null && phieudexuat!=null)
                    {
                        foreach(DanhsachNV item in chiaselist.DanhsachChons)
                        {
                            Nhansu nvShared = objectSpace.GetObjectByKey<Nhansu>(item.Oid);
                            Chiase chiase = objectSpace.FindObject<Chiase>(CriteriaOperator.Parse("Phieu=? && NhanvienShared=?", phieudexuat, nvShared));
                            if (chiase == null)
                            {
                                chiase = objectSpace.CreateObject<Chiase>();
                                chiase.Phieu = phieudexuat;
                                chiase.NhanvienShare = nv;
                                chiase.NhanvienShared = nvShared;
                                chiase.Ykien = chiaselist.Noidung;
                                chiase.Save();

                                string uMail = (string.IsNullOrEmpty(TCom.testMail) ? nvShared.Email : TCom.testMail);
                                TCom.SendEmailChiase(phieudexuat, uMail);
                            }
                        }
                        objectSpace.CommitChanges();
                        TCom.CustomInfo("", true);
                    }
                }
                else
                {
                    TCom.CustomError("Không chọn nhân sự để chia sẻ");
                }
            }
        }
    }
}
