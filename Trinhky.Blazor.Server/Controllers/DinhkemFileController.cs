using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;
using System;
using System.Linq;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class DinhkemFileController : ObjectViewController<ListView, DinhkemFile>
    {
        public DinhkemFileController()
        {
            TargetViewId = "DinhkemFile_ListView";
            SimpleAction LuuDinhkem = new(this, "LuuDinhkem", "View")
            {
                Caption = "Ghi",
                ImageName = "dinhkem",
                TargetViewId = "DinhkemFile_ListView"
            };
            LuuDinhkem.Execute += LuuDinhkem_Execute;

            //SimpleAction Quaylai = new(this, "Quaylai", "View")
            //{
            //    Caption = "Quay lại",
            //    ImageName = "back",
            //    TargetViewId = "DinhkemFile_ListView",
            //    ToolTip = "Quay lại trang trước"
            //};
            //Quaylai.Execute += Quaylai_Execute;
        }

        //private void Quaylai_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
        //    System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("navigateBack"));
        //}

        private void LuuDinhkem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(PhieudexuatFile));
            int phieuid = TCom.GetUserIntValue("viewdx");
            if (phieuid > 0)
            {
                Phieudexuat CurPhieu = objectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieuid));
                if (CurPhieu != null)
                {
                    Nhansu nv = objectSpace.GetObject(TCom.GetCurrentNhanvien());
                    foreach (DinhkemFile item in View.CollectionSource.List)
                    {
                        if (item.Tep != null)
                        {
                            PhieudexuatFile phieudexuatFile = objectSpace.CreateObject<PhieudexuatFile>();
                            phieudexuatFile.Nhanvien = nv;
                            phieudexuatFile.Phieu = CurPhieu;
                            phieudexuatFile.Tailieu = item.Tentailieu;

                            using (var stream = new MemoryStream())
                            {
                                item.Tep.SaveToStream(stream);
                                stream.Position = 0;
                                FileData fd = objectSpace.CreateObject<FileData>();
                                fd.LoadFromStream(item.Tep.FileName, stream);
                                phieudexuatFile.Tep = fd;
                            }

                            phieudexuatFile.Loai = 0;
                            phieudexuatFile.Save();
                        }
                    }
                    objectSpace.CommitChanges();
                    TCom.CustomInfo("Đã hoàn thành tác vụ");
                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.SetItemValue("an", false);
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController?.Active.RemoveItem("an");
            base.OnDeactivated();
        }
    }
    public class ViewDinhkem : ObjectViewController<DetailView, PhieudexuatFile>
    {
        public ViewDinhkem()
        {
            SimpleAction Mopdf = new(this, "Mopdf", "View")
            {
                Caption = "Xem",
                ImageName = "",
                TargetObjectType = typeof(PhieudexuatFile),
            };
            Mopdf.Execute += Mopdf_Execute;
        }

        private void Mopdf_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
