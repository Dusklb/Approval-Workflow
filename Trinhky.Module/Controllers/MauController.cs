using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Module.Controllers
{
    public class MauController:ViewController
    {
        public MauController()
        {
            TargetViewId = "MauVanban_ListView";//
            SimpleAction dupmau = new(this, "dupmau", "View")
            {
                Caption = "Bản sao",
                ImageName = "documents",
                ToolTip = "Tạo bản sao cho mẫu hiện tại",
                TargetViewId = "MauVanban_ListView",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ConfirmationMessage = "Chắc chắn tạo bản sao cho mẫu đang chọn?"
            };
            dupmau.Execute += Dupmau_Execute;
        }

        private void Dupmau_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if(View.CurrentObject is MauVanban maugoc)
            {
                MauVanban maumoi = ObjectSpace.CreateObject<MauVanban>();
                maumoi.Noidung = maugoc.Noidung;
                maumoi.Chucvuxuly = maugoc?.Chucvuxuly;
                maumoi.Tenmau = maugoc.Tenmau + " (bản sao)";
                maumoi.Loai = maugoc?.Loai;
                //maumoi.Donvi = maugoc?.Donvi;
                maumoi.Save();

                foreach(QuitrinhChuan item in maugoc.QuitrinhChuans)
                {
                    QuitrinhChuan qt = ObjectSpace.CreateObject<QuitrinhChuan>();
                    qt.Mau = maumoi;
                    qt.SoTT = item.SoTT;
                    qt.Chucvu=item.Chucvu;
                    qt.Songay= item.Songay;
                    qt.Save();
                }
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                View.Refresh();
            }
        }
    }
}
