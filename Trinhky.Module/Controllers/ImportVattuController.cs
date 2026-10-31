using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class ImportControllerVT : ViewController
    {
        public ImportControllerVT()
        {
            SimpleAction ImpVT = new(this, "ImpVT", "View")
            {
                Caption = "Cập nhật vật tư",
                ImageName = "import",
                ToolTip = "Cập nhật dữ liệu vật tư qua Windows Clipboard",
                TargetViewId = "NhomVT_ListView",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
            ImpVT.Execute += ImpVT_Execute;
        }

        private void ImpVT_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                if (View.CurrentObject is NhomVT nhom)
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ClsImportVT));
                    ClsImportVT NewImport = objectSpace.CreateObject<ClsImportVT>();
                    NewImport.Nhomhang = nhom;
                    DetailView NewView = Application.CreateDetailView(objectSpace, NewImport);
                    NewView.Caption = "Cập nhật vật tư";
                    e.ShowViewParameters.CreatedView = NewView;
                    e.ShowViewParameters.Context = TemplateContext.View;
                    e.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.Message);
            }
        }
    }

    public class ImportVTActions : ObjectViewController<DetailView, ClsImportVT>
    {
        private ClsImportVT imp;
        public ImportVTActions()
        {
            SimpleAction CheckData = new(this, "CheckDataVT", "View")
            {
                Caption = "Kiểm tra",
                ImageName = "checkdata",
                TargetViewId = "ClsImportVT_DetailView",
                ToolTip = "Kiểm tra dữ liệu cần cập nhật"
            };
            CheckData.Execute += CheckData_Execute;

            SimpleAction CapnhatDulieu = new(this, "CapnhatDulieuVT", "View")
            {
                Caption = "Lưu dữ liệu",
                TargetViewId = "ClsImportVT_DetailView",
                ImageName = "dataimport",
                ToolTip = "Lưu dữ liệu vật tư, hàng hóa",
                ConfirmationMessage = "Chắc chắn lưu dữ liệu?"
            };
            CapnhatDulieu.Execute += CapnhatDulieu_Execute;
        }

        private void CapnhatDulieu_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            imp = View.CurrentObject as ClsImportVT;
            if (imp != null)
            {
                LuuHang();
            }
        }

        private void CheckData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            imp = View.CurrentObject as ClsImportVT;
            if (imp != null)
            {
                CheckHangClipboard();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAction.Active.SetItemValue("an", false);
                modificationsController.SaveAndCloseAction.Active.SetItemValue("an", false);
                modificationsController.SaveAndNewAction.Active.SetItemValue("an", false);
                modificationsController.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
            }
        }
        protected override void OnDeactivated()
        {
            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAction.Active.RemoveItem("an");
                modificationsController.SaveAndCloseAction.Active.RemoveItem("an");
                modificationsController.SaveAndNewAction.Active.RemoveItem("an");
                modificationsController.ModificationsHandlingMode = ModificationsHandlingMode.Confirmation;
            }
            base.OnDeactivated();
        }

        private void CheckHangClipboard()
        {
            string dulieu = imp.Noidung;
            if (string.IsNullOrEmpty(dulieu))
                return;

            imp.ImportHangs.Clear();
            string[] lines = dulieu.Split(new string[] { "\n" }, StringSplitOptions.None);
            int linecount = lines.Length;
            if (linecount > 0)
            {
                for (int i = 0; i < linecount; i++)
                {
                    string line = lines[i];
                    line = line.Replace("\"", "");
                    line = line.Replace("\r", "");
                    line = line.Replace(".", "");
                    line = line.Replace(",", ".");
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] datas = line.Split("\t".ToCharArray());
                        ImportHangData khach = new()
                        {
                            Oid = Guid.NewGuid(),
                            MaSP = GetStringInArray(datas, 0),
                            TenSP = GetStringInArray(datas, 1),
                            DVT = GetStringInArray(datas, 2),
                            Dongia = tmLib.CommonLib.CDecimal(GetStringInArray(datas, 3)),
                        };
                        imp.ImportHangs.Add(khach);
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
        private void LuuHang()
        {
            try
            {
                if (imp.ImportHangs.Count > 0)
                {
                    XafApplication app = ApplicationHelper.Instance.Application;
                    IObjectSpace objectSpace = app.CreateObjectSpace(typeof(Vattu));
                    NhomVT nhom = objectSpace.GetObject(imp.Nhomhang);
                    foreach (ImportHangData row in imp.ImportHangs)
                    {
                        Vattu sp = null;
                        string ma = row.MaSP.Trim();
                        if (ma.Length > 30) ma = tmLib.CommonLib.Left(ma, 30);
                        if (!string.IsNullOrEmpty(ma))//co ma thi tim
                        {
                            sp = objectSpace.FindObject<Vattu>(CriteriaOperator.Parse("MaVT=?", ma));
                        }
                        if (sp == null)//sp moi thi tao sp moi
                        {
                            sp = objectSpace.CreateObject<Vattu>();
                            sp.Nhom = nhom;
                            sp.MaVT = ma;
                            string ten = row.TenSP.Trim();
                            if (ten.Length > 255) ten = tmLib.CommonLib.Left(ten, 255);
                            sp.Tenvt = ten;

                            string dvt = row.DVT.Trim();
                            if (dvt.Length > 20) dvt = tmLib.CommonLib.Left(dvt, 20);
                            sp.Dvt = dvt;
                            sp.Dongia = row.Dongia;
                            sp.Save();
                        }

                        objectSpace.CommitChanges();
                    }
                    imp.ImportHangs.Clear();
                    TCom.CustomInfo("", true);
                }
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.Message);
            }

        }
    }

}
