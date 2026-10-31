using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class ImportActions : ViewController
    {
        private ClsImport imp;
        public ImportActions()
        {
            TargetViewId = "ClsImport_DetailView";
            SimpleAction CheckData = new(this, "CheckData", "View")
            {
                Caption = "Kiểm tra dữ liệu",
                ImageName = "checkdb",
                TargetViewId = "ClsImport_DetailView"
            };
            CheckData.Execute += CheckData_Execute;

            SimpleAction CapnhatDulieu = new(this, "CapnhatDulieu", "View")
            {
                Caption = "Lưu dữ liệu",
                TargetViewId = "ClsImport_DetailView",
                ImageName = "database-save",
                ToolTip = "Lưu dữ liệu nhân viên",
                ConfirmationMessage = "Chắc chắn lưu dữ liệu?"
            };
            CapnhatDulieu.Execute += CapnhatDulieu_Execute;
        }

        private void CheckData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            imp = View.CurrentObject as ClsImport;
            string dulieu = imp.Noidung;
            if (string.IsNullOrEmpty(dulieu))
                return;

            imp.ImportNhanviens.Clear();
            string[] lines = dulieu.Split(new string[] { "\n" }, StringSplitOptions.None);
            int linecount = lines.Length;
            if (linecount > 0)
            {
                for (int i = 0; i < linecount; i++)
                {
                    string line = lines[i];
                    line = line.Replace("\"", "");
                    line = line.Replace("\r", "");
                    //line = line.Replace(".", "");
                    //line = line.Replace(",", ".");
                    //read data
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] datas = line.Split("\t".ToCharArray());
                        ClsNhanvien khach = new()
                        {
                            Oid = Guid.NewGuid(),
                            Sothe = GetStringInArray(datas, 0),
                            Hoten = GetStringInArray(datas, 1),
                            Gioitinh = GetStringInArray(datas, 2),
                            Email = GetStringInArray(datas, 3),
                            Dienthoai = GetStringInArray(datas, 4),
                        };
                        imp.ImportNhanviens.Add(khach);
                    }
                }
            }

        }
        private static string GetStringInArray(string[] array, int index)
        {
            if (index < array.Length)
                return array[index].Trim();
            else
                return string.Empty;
        }

        private void CapnhatDulieu_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                if (imp != null)
                {
                    if (imp.Donvi == null)
                    {
                        TCom.CustomError("Cần chọn bộ phận");
                        return;
                    }
                    if (imp.CS == null)
                    {
                        TCom.CustomError("Cần chọn cơ sở");
                        return;
                    }
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Nhansu));
                    foreach (ClsNhanvien row in imp.ImportNhanviens)
                    {
                        Nhansu khach = null;
                        string email = row.Email.Trim();
                        string sothe = row.Sothe.Trim();
                        string ten = row.Hoten.Trim();
                        string gt = row.Gioitinh.Trim();
                        string phone = row.Dienthoai.Trim();
                        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(sothe))
                        {
                            khach = objectSpace.FindObject<Nhansu>(CriteriaOperator.Parse("Email=?", email));
                            if (khach == null)
                            {
                                khach = objectSpace.CreateObject<Nhansu>();
                                khach.Donvi = objectSpace.GetObject<Bophan>(imp.Donvi);
                                khach.CS = objectSpace.GetObject<Coso>(imp.CS);
                                khach.Sothe = sothe;
                                khach.HoTen = ten;
                                khach.Email = email;
                                khach.Gioitinh = gt;
                                khach.Phone = phone;
                                khach.Save();
                            }
                            else
                            {
                                khach.Donvi = objectSpace.GetObject<Bophan>(imp.Donvi);
                                khach.CS = objectSpace.GetObject<Coso>(imp.CS);
                                khach.Sothe = sothe;
                                khach.HoTen = ten;
                                khach.Gioitinh = gt;
                                khach.Phone = phone;
                                khach.Save();
                            }
                            //tao tai khoan dang nhap
                            ApplicationUser sampleUser = objectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == khach.Email);
                            if (sampleUser == null)
                            {
                                sampleUser = objectSpace.CreateObject<ApplicationUser>();
                                sampleUser.UserName = khach.Email.ToLower();
                                // Set a password if the standard authentication type is used
                                sampleUser.SetPassword("1234567");

                                PermissionPolicyRole defaultRole = objectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Nhanvien");
                                if (defaultRole != null)
                                {
                                    ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, objectSpace.GetKeyValueAsString(sampleUser));
                                    sampleUser.Roles.Add(defaultRole);
                                    sampleUser.IsActive = true;
                                }
                            }
                            objectSpace.CommitChanges();
                        }
                    }
                    imp.ImportNhanviens.Clear();
                    //View.ObjectSpace.Refresh();
                    TCom.CustomInfo("", true);
                }
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.Message);
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ControlActions(false);
        }
        protected override void OnDeactivated()
        {
            ControlActions(true);
            base.OnDeactivated();
        }
        private void ControlActions(bool bActive)
        {
            if (View.Id == "ClsImport_DetailView")
            {
                string ActiveKey = "XlsImport";
                RefreshController controller2 = Frame.GetController<RefreshController>();
                if (controller2 != null)
                {
                    controller2.RefreshAction.Active[ActiveKey] = bActive;
                }
                ShowAllContextsController controller3 = Frame.GetController<ShowAllContextsController>();
                if (controller3 != null)
                {
                    controller3.ValidateAction.Active[ActiveKey] = bActive;
                }
                ModificationsController mod = Frame.GetController<ModificationsController>();
                if (mod != null)
                {
                    mod.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
                    mod.SaveAction.Active[ActiveKey] = bActive;
                    mod.SaveAndCloseAction.Active[ActiveKey] = bActive;
                    mod.SaveAndNewAction.Active[ActiveKey] = bActive;
                    mod.CancelAction.Active[ActiveKey] = bActive;
                }
                DeleteObjectsViewController controller4 = Frame.GetController<DeleteObjectsViewController>();
                if (controller4 != null)
                {
                    controller4.Active[ActiveKey] = bActive;
                }
                NewObjectViewController controller5 = Frame.GetController<NewObjectViewController>();
                if (controller5 != null)
                {
                    controller5.Active[ActiveKey] = bActive;
                }
                //ResetViewSettingsController resetViewSettingsController = Frame.GetController<ResetViewSettingsController>();
                //if (resetViewSettingsController != null)
                //    resetViewSettingsController.Active[ActiveKey] = bActive;
            }
        }

    }
}
