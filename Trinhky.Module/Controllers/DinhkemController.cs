using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class DinhkemController : ViewController
    {
        public DinhkemController()
        {
            TargetObjectType = typeof(PhieudexuatFile);//Phieudexuat_PhieudexuatFiles_ListView
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                controller.ObjectCreating += Controller_ObjectCreating;
            }
        }

        private void Controller_ObjectCreating(object sender, ObjectCreatingEventArgs e)
        {
            if (((DevExpress.ExpressApp.View)((DevExpress.ExpressApp.BaseObjectSpace)this.ObjectSpace).Owner).CurrentObject is Phieudexuat CurPhieu && CurPhieu.Oid == -1)
            {
                TCom.CustomError("Cần lập phiếu trước");
                e.Cancel = true;
            }
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                controller.ObjectCreating -= Controller_ObjectCreating;
            }
            base.OnDeactivated();
        }
    }

    //public class ChinhsuaController : ViewController
    //{
    //    public ChinhsuaController()
    //    {
    //        TargetObjectType = typeof(Quatrinh);//Phieudexuat_PhieudexuatFiles_ListView
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
    //        if (controller != null)
    //        {
    //            controller.ObjectCreating += Controller_ObjectCreating;
    //        }
    //    }

    //    private void Controller_ObjectCreating(object sender, ObjectCreatingEventArgs e)
    //    {
    //        if (((DevExpress.ExpressApp.View)((DevExpress.ExpressApp.BaseObjectSpace)this.ObjectSpace).Owner).CurrentObject is Phieudexuat CurPhieu && CurPhieu.Oid == -1)
    //        {
    //            TCom.CustomError("Cần lập phiếu trước");
    //            e.Cancel = true;
    //        }
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
    //        if (controller != null)
    //        {
    //            controller.ObjectCreating -= Controller_ObjectCreating;
    //        }
    //        base.OnDeactivated();
    //    }
    //}
}
