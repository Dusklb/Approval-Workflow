using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using System.ComponentModel;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class TrinhkyModule : ModuleBase
{
    public TrinhkyModule()
    {
        // 
        // TrinhkyModule
        // 
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.ModelDifference));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.ModelDifferenceAspect));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.BaseObject));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileData));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileAttachmentBase));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Event));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Resource));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.HCategory));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Chart.ChartModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Dashboards.DashboardsModule));
        //RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Notifications.NotificationsModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Office.OfficeModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
        //RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));

        CriteriaOperator.RegisterCustomFunction(new CurrentNhanvienID());

    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
        // Manage various aspects of the application UI and behavior at the module level.
        ApplicationHelper.Instance.Initialize(application);
        application.SetupComplete += Application_SetupComplete;
        application.LoggedOn += Application_LoggedOn;
    }

    private void Application_LoggedOn(object sender, LogonEventArgs e)
    {
        TCom.CheckMailOptions();
        //DateTime ngay = new(DateTime.Now.Year, DateTime.Now.Month, 1);//DateTime.Now
        DateTime ngay= DateTime.Now.AddDays(-6);
        TCom.AddUserValue("tungay", ngay.ToString());
        TCom.AddUserValue("denngay", DateTime.Now.ToString());
    }

    private void Application_SetupComplete(object sender, EventArgs e)
    {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
    }

    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
    {
        if (e.ObjectSpace is NonPersistentObjectSpace space)
        {
            IObjectSpace additionalObjectSpace = Application.CreateObjectSpace(typeof(ApplicationUser));
            space.AdditionalObjectSpaces.Add(additionalObjectSpace);

            space.ObjectGetting += ObjectSpace_ObjectGetting;
            e.ObjectSpace.Disposed += (s, args) =>
            {
                ((NonPersistentObjectSpace)s).ObjectGetting -= ObjectSpace_ObjectGetting;
                additionalObjectSpace.Dispose();
            };
        }
        if (e.ObjectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)
        {
            nonPersistentObjectSpace.ObjectsGetting += NonPersistentObjectSpace_ObjectsGetting;
        }
    }

    private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
    {
        if (e.SourceObject is IObjectSpaceLink)
        {
            e.TargetObject = e.SourceObject;
            ((IObjectSpaceLink)e.TargetObject).ObjectSpace = (IObjectSpace)sender;
        }
    }

    private void NonPersistentObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
    {
        NonPersistentObjectSpace obs = (NonPersistentObjectSpace)sender;
        XPObjectSpace objectSpace = (XPObjectSpace)obs.AdditionalObjectSpaces[0];
        if (e.ObjectType == typeof(Phieuduyet))
        {
            e.Objects = TCom.GetDanhsachDuyet(objectSpace);
        }
        else if (e.ObjectType == typeof(PhieuDaduyet))
        {
            e.Objects = TCom.GetDanhsachDaDuyet(objectSpace);
        }
        else if (e.ObjectType == typeof(PhieuDahuy))
        {
            e.Objects = TCom.GetDanhsachDaHuy(objectSpace);
        }
        else if (e.ObjectType == typeof(PhieuXuly))
        {
            e.Objects = TCom.GetDanhsachXuly(objectSpace);
        }
        else if (e.ObjectType == typeof(ThongkeDexuat))
        {
            e.Objects = TCom.GetThongkeDexuat(objectSpace);
        }
        else if (e.ObjectType == typeof(ThongkeDuyet))
        {
            e.Objects = TCom.GetThongkeDuyet(objectSpace);
        }
        else if (e.ObjectType == typeof(DexuatCanhan))
        {
            e.Objects = TCom.GetThongkeDexuatCanhan(objectSpace);
        }
        else if (e.ObjectType == typeof(DuyetDexuatCanhan))
        {
            e.Objects = TCom.GetThongkeDuyetCanhan(objectSpace);
        }
        else if (e.ObjectType == typeof(ThongkeDexuatMau))
        {
            e.Objects = TCom.GetThongkeTheoMau(objectSpace);
        }
        else if (e.ObjectType == typeof(PhieuChiase))
        {
            e.Objects = TCom.GetDanhsachChiase(objectSpace);
        }
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        typesInfo.FindTypeInfo(typeof(XPObject)).KeyMember.AddAttribute(new BrowsableAttribute(false));
    }
}
