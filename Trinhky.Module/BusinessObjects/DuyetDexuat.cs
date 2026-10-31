using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("steps")]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Duyệt đề xuất")]
    [DefaultProperty("Phieu.TenDexuat")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    [Appearance("ColorDetailDuyet1", AppearanceItemType = "LayoutItem", TargetItems = "Trangthai", Criteria = "1=1", Context = "DetailView",
        FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, Priority = 1)]
    [Appearance("ColorDetailDuyet2", AppearanceItemType = "LayoutItem", TargetItems = "Ghichu", Criteria = "1=1", Context = "DetailView",
        FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, Priority = 1)]
    public class DuyetDexuat : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public DuyetDexuat(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        private Phieudexuat _Phieu;
        [Association]
        [XafDisplayName("Phiếu đề xuất")]
        public Phieudexuat Phieu
        {
            get { return _Phieu; }
            set { SetPropertyValue<Phieudexuat>(nameof(Phieu), ref _Phieu, value); }
        }

        private int _SoTT;
        [XafDisplayName("Bước"), ModelDefault("AllowEdit", "false")]
        public int SoTT
        {
            get { return _SoTT; }
            set { SetPropertyValue<int>(nameof(SoTT), ref _SoTT, value); }
        }

        private string _Maduyet;
        [Browsable(false)]
        public string Maduyet
        {
            get { return _Maduyet; }
            set { SetPropertyValue<string>(nameof(Maduyet), ref _Maduyet, value); }
        }

        private string _MaChuky;
        [Browsable(false)]
        public string MaChuky
        {
            get { return _MaChuky; }
            set { SetPropertyValue<string>(nameof(MaChuky), ref _MaChuky, value); }
        }

        private string _MaKythay;
        [Browsable(false)]
        public string MaKythay
        {
            get { return _MaKythay; }
            set { SetPropertyValue<string>(nameof(MaKythay), ref _MaKythay, value); }
        }

        private Nhansu _Nguoiduyet;
        [Association("ns-duyet")]
        [XafDisplayName("Người duyệt"), ModelDefault("AllowEdit", "false")]
        public Nhansu Nguoiduyet
        {
            get { return _Nguoiduyet; }
            set { SetPropertyValue<Nhansu>(nameof(Nguoiduyet), ref _Nguoiduyet, value); }
        }

        private Nhansu _NguoiUQ;
        [Association("ns-duyetuq")]
        [XafDisplayName("Người được UQ"), ModelDefault("AllowEdit", "false")]
        public Nhansu NguoiUQ
        {
            get { return _NguoiUQ; }
            set { SetPropertyValue<Nhansu>(nameof(NguoiUQ), ref _NguoiUQ, value); }
        }

        private DateTime _Handuyet;
        [XafDisplayName("Hạn duyệt"), ModelDefault("AllowEdit", "false")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        public DateTime Handuyet
        {
            get { return _Handuyet; }
            set { SetPropertyValue<DateTime>(nameof(Handuyet), ref _Handuyet, value); }
        }

        private DateTime _Ngayduyet;
        [XafDisplayName("Ngày duyệt"), ModelDefault("AllowEdit", "false")]
        [ModelDefault("EditMask", "dd/MM/yyyy HH:mm")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngayduyet
        {
            get { return _Ngayduyet; }
            set { SetPropertyValue<DateTime>(nameof(Ngayduyet), ref _Ngayduyet, value); }
        }

        private bool _DuyetUQ;
        [XafDisplayName("Duyệt UQ"), ModelDefault("AllowEdit", "false")]
        public bool DuyetUQ
        {
            get { return _DuyetUQ; }
            set { SetPropertyValue<bool>(nameof(DuyetUQ), ref _DuyetUQ, value); }
        }

        private bool _Ketthuc;
        [XafDisplayName("Kết thúc")]
        public bool Ketthuc
        {
            get { return _Ketthuc; }
            set { SetPropertyValue<bool>(nameof(Ketthuc), ref _Ketthuc, value); }
        }

        private int _UQId;
        [Browsable(false)]
        public int UQId
        {
            get { return _UQId; }
            set { SetPropertyValue<int>(nameof(UQId), ref _UQId, value); }
        }

        private TCom.EStatusDuyet _Trangthai;
        [XafDisplayName("Kết luận")]
        public TCom.EStatusDuyet Trangthai
        {
            get { return _Trangthai; }
            set { SetPropertyValue<TCom.EStatusDuyet>(nameof(Trangthai), ref _Trangthai, value); }
        }

        private string _Ghichu;
        [XafDisplayName("Ý kiến"), Size(2048)]
        public string Ghichu
        {
            get { return _Ghichu; }
            set { SetPropertyValue<string>(nameof(Ghichu), ref _Ghichu, value); }
        }

        private bool _Boqua = false;
        [XafDisplayName("Bỏ qua")]
        public bool Boqua
        {
            get { return _Boqua; }
            set { SetPropertyValue<bool>(nameof(Boqua), ref _Boqua, value); }
        }

        [XafDisplayName("Bước duyệt"), VisibleInListView(false)]
        public string ThongtinDuyet
        {
            get
            {
                string tt = "Bước: " + SoTT.ToString();
                if (Handuyet != DateTime.MinValue)
                    tt += ", Hạn duyệt: " + Handuyet.ToString("dd/MM/yyyy");
                if (Nguoiduyet != null)
                    tt += ", Người duyệt: " + Nguoiduyet.HoTen;
                if (DuyetUQ)
                    tt += " (Duyệt ủy quyền)";
                return tt;
            }
        }
    }
}