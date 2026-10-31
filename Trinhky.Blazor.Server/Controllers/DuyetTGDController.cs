using DevExpress.ExpressApp;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class DuyetTGDController:ViewController
    {
        public DuyetTGDController()
        {
            TargetViewId = "DuyetView;";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            bool BuocCuoi = false;
            if(View.CurrentObject is DuyetDexuat duyet)
            {
                int tong = duyet.Phieu.DuyetDexuats.Count;
                int sott = duyet.SoTT;
                if (sott == tong) BuocCuoi = true;
            }
            if(View is DetailView view)
            {
                bool banTgd = false;
                Nhansu nv = TCom.GetCurrentNhanvien();
                if (nv != null)
                {
                    foreach (ChucvuNV cv in nv.ChucvuNVs)
                    {
                        if (cv.Chucvu != null && cv.Chucvu.BanTGD)
                        {
                            banTgd = true;
                            break;
                        }
                    }
                }
                if (BuocCuoi) banTgd = false;
                TCom.SetAllowView(view, "Ketthuc", banTgd);
            }

        }
    }
}
