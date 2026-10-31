using DevExpress.ExpressApp;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    //public class MailDexuatController : ViewController
    //{
    //    public MailDexuatController()
    //    {
    //        TargetViewNesting = Nesting.Root;
    //        TargetObjectType = typeof(Phieudexuat);
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        View.ObjectSpace.Committed += ObjectSpace_Committed;
    //    }
    //    private void ObjectSpace_Committed(object sender, System.EventArgs e)
    //    {
    //        if (View.CurrentObject is Phieudexuat p)
    //        {
    //            if(p.Dagui)
    //                TCom.SendEmailDexuat(p);
    //        }
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        View.ObjectSpace.Committed -= ObjectSpace_Committed;
    //        base.OnDeactivated();
    //    }
    //}

    //public class MailYkienController : ViewController
    //{
    //    public MailYkienController()
    //    {
    //        TargetViewNesting = Nesting.Root;
    //        TargetObjectType = typeof(Quatrinh);
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        View.ObjectSpace.Committed += ObjectSpace_Committed;
    //    }
    //    private void ObjectSpace_Committed(object sender, System.EventArgs e)
    //    {
    //        if (View.CurrentObject is Quatrinh p)
    //        {
    //            if (p.Phieu != null && p.Phieu.Dagui)
    //                TCom.SendEmailYkien(p);
    //        }
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        View.ObjectSpace.Committed -= ObjectSpace_Committed;
    //        base.OnDeactivated();
    //    }
    //}

    //public class MailTailieuController : ViewController
    //{
    //    public MailTailieuController()
    //    {
    //        TargetViewNesting = Nesting.Root;
    //        TargetObjectType = typeof(PhieudexuatFile);
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        View.ObjectSpace.Committed += ObjectSpace_Committed;
    //    }
    //    private void ObjectSpace_Committed(object sender, System.EventArgs e)
    //    {
    //        if (View.CurrentObject is PhieudexuatFile p)
    //        {
    //            if(p.Phieu!=null && p.Phieu.Dagui)
    //                TCom.SendEmailTailieu(p);
    //        }
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        View.ObjectSpace.Committed -= ObjectSpace_Committed;
    //        base.OnDeactivated();
    //    }
    //}
}
