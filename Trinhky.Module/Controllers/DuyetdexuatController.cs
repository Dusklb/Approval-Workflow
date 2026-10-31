using DevExpress.ExpressApp;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class DuyetdexuatController : ViewController<DetailView>
    {
        public DuyetdexuatController()
        {
            TargetObjectType = typeof(DuyetDexuat);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            bool chophep = false;
            if (View.CurrentObject is DuyetDexuat d)
            {
                string email = TCom.GetNhanvienMail();
                if (d.Nguoiduyet != null)
                {
                    chophep = (email.ToLower() == d.Nguoiduyet.Email.ToLower());
                }
                if(d.NguoiUQ!=null && !chophep)
                {
                    chophep = (email.ToLower() == d.NguoiUQ.Email.ToLower());
                }
            }
            View.AllowDelete.SetItemValue("d", TCom.bAdmin);
            View.AllowEdit.SetItemValue("d", chophep);
            View.AllowNew.SetItemValue("d", chophep);
        }
    }
}
