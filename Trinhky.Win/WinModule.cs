using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using System.ComponentModel;
using Trinhky.Win.Controllers;

namespace Trinhky.Win;

[ToolboxItemFilter("Xaf.Platform.Win")]
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class TrinhkyWinModule : ModuleBase
{
    //private void Application_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
    //    e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), true, "Win");
    //    e.Handled = true;
    //}
    private void Application_CreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e)
    {
        e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), false, "Win");
        e.Handled = true;
    }
    public TrinhkyWinModule()
    {
        DevExpress.ExpressApp.Editors.FormattingProvider.UseMaskSettings = true;
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        return ModuleUpdater.EmptyModuleUpdaters;
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
        //application.CreateCustomModelDifferenceStore += Application_CreateCustomModelDifferenceStore;
        application.CreateCustomUserModelDifferenceStore += Application_CreateCustomUserModelDifferenceStore;
        application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        application.LoggedOn += Application_LoggedOn;
    }
    private void Application_LoggedOn(object sender, LogonEventArgs e)
    {
        XafApplication app = (XafApplication)sender;
        IObjectSpaceProvider objectSpaceProvider = app.ObjectSpaceProviders[0];
        ((SecuredObjectSpaceProvider)objectSpaceProvider).AllowICommandChannelDoWithSecurityContext = true;
    }

    private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e)
    {
        e.Controllers.Add(((XafApplication)sender).CreateController<DBServer>());
    }

}
