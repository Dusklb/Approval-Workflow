using DevExpress.ExpressApp;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class ChiaseListNVController : ViewController<ListView>
    {
        private ChiaseList curObj;
        public ChiaseListNVController()
        {
            //TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(DanhsachNV);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CollectionSource is PropertyCollectionSource collectionSource)
            {
                if (collectionSource.MasterObject != null)
                {
                    curObj = (ChiaseList)collectionSource.MasterObject;
                }
            }
            View.CurrentObjectChanged += View_CurrentObjectChanged;
        }

        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            if (Frame is NestedFrame)
            {
                if (View.CurrentObject != null && curObj != null)
                {
                    bool bDaco = false;
                    DanhsachNV item = (DanhsachNV)View.CurrentObject;
                    foreach (DanhsachNV itemchon in curObj.DanhsachChons)
                    {
                        if (itemchon.Oid == item.Oid)
                        {
                            bDaco = true;
                            break;
                        }
                    }
                    if (!bDaco)
                    {
                        curObj.DanhsachChons.Add(item);
                    }
                    //frame.ViewItem.View.Caption = ((DanhsachNV)View.CurrentObject).Hoten;
                }
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
        }
    }
}
