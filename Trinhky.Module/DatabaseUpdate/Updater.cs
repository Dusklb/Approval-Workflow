using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        //string name = "MyName";
        //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
        //if(theObject == null) {
        //    theObject = ObjectSpace.CreateObject<DomainObject1>();
        //    theObject.Name = name;
        //}
#if !RELEASE
        //ApplicationUser sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "User");
        //if(sampleUser == null) {
        //    sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
        //    sampleUser.UserName = "User";
        //    // Set a password if the standard authentication type is used
        //    sampleUser.SetPassword("");

        //    // The UserLoginInfo object requires a user object Id (Oid).
        //    // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
        //    ObjectSpace.CommitChanges(); //This line persists created object(s).
        //    ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        //}
        //PermissionPolicyRole defaultRole = CreateDefaultRole();
        //sampleUser.Roles.Add(defaultRole);

        ApplicationUser userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
        if(userAdmin == null) {
            userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
            userAdmin.UserName = "Admin";
            // Set a password if the standard authentication type is used
            userAdmin.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(userAdmin));
        }
		// If a role with the Administrators name doesn't exist in the database, create this role
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if(adminRole == null) {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
        }
        adminRole.IsAdministrative = true;
		userAdmin.Roles.Add(adminRole);

        CreateNhanvientRole();
        ObjectSpace.CommitChanges(); //This line persists created object(s).
#endif
    }
    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
    private PermissionPolicyRole CreateDefaultRole() {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if(defaultRole == null) {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

			defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
			defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
        return defaultRole;
    }

    private void CreateNhanvientRole()
    {
        PermissionPolicyRole role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Nhanvien"));
        if (role == null)
        {
            role = ObjectSpace.CreateObject<PermissionPolicyRole>();
            role.Name = "Nhanvien";
            role.PermissionPolicy = SecurityPermissionPolicy.ReadOnlyAllByDefault;
            role.AddTypePermission<ModelDifference>("Read;Write;Create", SecurityPermissionState.Allow);
            role.AddTypePermission<DuyetDexuat>(SecurityOperations.Read, SecurityPermissionState.Allow);
            role.AddTypePermission<DuyetDexuat>("Create;Delete", SecurityPermissionState.Deny);
            role.AddObjectPermission<DuyetDexuat>(SecurityOperations.ReadWriteAccess, "[Nguoiduyet.Email] = NHANVIENMAIL()", SecurityPermissionState.Allow);
            role.AddObjectPermission<DuyetDexuat>(SecurityOperations.Delete, "[Nguoiduyet.Email] = NHANVIENMAIL()", SecurityPermissionState.Deny);
            role.AddTypePermission<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            role.AddObjectPermission<ApplicationUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
            role.AddMemberPermission<ApplicationUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
            role.AddMemberPermission<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
            role.AddTypePermission<Phieudexuat>("Read;Create", SecurityPermissionState.Allow);
            role.AddTypePermission<Phieudexuat>(SecurityOperations.Delete, SecurityPermissionState.Deny);
            role.AddObjectPermission<Phieudexuat>(SecurityOperations.ReadWriteAccess, "[Nhanvien.Email] = NHANVIENMAIL()", SecurityPermissionState.Allow);
            role.AddObjectPermission<Phieudexuat>(SecurityOperations.Delete, "[Nhanvien.Email] = NHANVIENMAIL()", SecurityPermissionState.Deny);
            role.AddTypePermission<ModelDifferenceAspect>("Read;Write;Create", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default", SecurityPermissionState.Deny);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/mnuDanhmuc", SecurityPermissionState.Deny);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/mnuDanhmuc/Items/Phieuduyet_ListView", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/mnuDanhmuc/Items/Phieudexuat_ListView", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Reports", SecurityPermissionState.Deny);
        }
    }

}
