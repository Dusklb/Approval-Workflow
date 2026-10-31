using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class AdminController : ViewController<ListView>
    {
        public AdminController()
        {
            //TargetObjectType = typeof(ApplicationUser);
            //SimpleAction updatenv = new SimpleAction(this, "updatenv", "View")
            //{
            //    Caption = "pass 123",
            //    TargetObjectType = typeof(ApplicationUser)
            //};
            //updatenv.Execute += Updatenv_Execute;

            //SimpleAction updatemail = new SimpleAction(this, "updatemail", "View")
            //{
            //    Caption = "Mail",
            //    TargetObjectType = typeof(Nhansu)
            //};
            //updatemail.Execute += Updatemail_Execute;
            SimpleAction UserNew = new(this, "UserNew", "View")
            {
                Caption = "Tạo tài khoản",
                ImageName = "usernew",
                TargetObjectType = typeof(Nhansu),
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ConfirmationMessage = "Chắc chắn tạo tài khoản đăng nhập cho nhân viên này?"
            };
            UserNew.Execute += UserNew_Execute;

            SimpleAction UserLock = new(this, "UserLock", "View")
            {
                Caption = "Khóa tài khoản",
                ImageName = "userdel",
                TargetObjectType = typeof(Nhansu),
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ConfirmationMessage = "Chắc chắn khóa tài khoản đăng nhập cho nhân viên này?"
            };
            UserLock.Execute += UserLock_Execute;
        }

        //private void Updatenv_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    XafApplication app = ApplicationHelper.Instance.Application;
        //    using IObjectSpace objectSpace = app.CreateObjectSpace(typeof(ApplicationUser));
        //    using Session session = ((XPObjectSpace)objectSpace).Session;
        //    XPCollection<ApplicationUser> list = new XPCollection<ApplicationUser>(session);
        //    foreach (ApplicationUser user in list)
        //    {
        //        user.SetPassword("1234567");
        //    }
        //    objectSpace.CommitChanges();
        //}

        private void UserLock_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is Nhansu ns)
            {
                string mail = ns.Email;
                if (!string.IsNullOrEmpty(mail))
                {
                    ApplicationUser usr = ObjectSpace.FindObject<ApplicationUser>(CriteriaOperator.Parse("UserName=?", mail));
                    if (usr != null)
                    {
                        usr.IsActive = false;
                        usr.Save();
                        ns.Nghiviec = true;
                        ObjectSpace.CommitChanges();
                        TCom.CustomInfo("", true);
                    }
                    else
                        TCom.CustomError("Không xác định được tài khoản Mail này");
                }
                else
                    TCom.CustomError("Không có địa chỉ Mail");
            }
        }

        private void UserNew_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is Nhansu ns)
            {
                string mail = ns.Email;
                if (!string.IsNullOrEmpty(mail))
                {
                    ApplicationUser usr = ObjectSpace.FindObject<ApplicationUser>(CriteriaOperator.Parse("UserName=?", mail));
                    if (usr == null)
                    {
                        usr = ObjectSpace.CreateObject<ApplicationUser>();
                        usr.UserName = mail;
                        usr.SetPassword("");

                        // The UserLoginInfo object requires a user object Id (Oid).
                        // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
                        ObjectSpace.CommitChanges(); //This line persists created object(s).
                        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Nhanvien");
                        if (defaultRole != null)
                        {
                            ((ISecurityUserWithLoginInfo)usr).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(usr));
                            usr.Roles.Add(defaultRole);
                            usr.IsActive = true;
                        }
                        ObjectSpace.CommitChanges();
                        TCom.CustomInfo("", true);
                    }
                    else
                        TCom.CustomError("Tài khoản Mail này đã có");
                }
                else
                    TCom.CustomError("Không có địa chỉ Mail");
            }
        }

    }
}
