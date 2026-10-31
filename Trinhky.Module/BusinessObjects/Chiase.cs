using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("Được chia sẻ")]
    [ImageName("share")]
    [DefaultProperty("NhanvienShared")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [NavigationItem(false)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Chiase : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Chiase(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Session.IsNewObject(this))
                Ngay = TCom.GetServerDateTime();
        }
        private Phieudexuat _Phieu;
        [Association]
        [XafDisplayName("Phiếu đề xuất")]
        public Phieudexuat Phieu
        {
            get { return _Phieu; }
            set { SetPropertyValue<Phieudexuat>(nameof(Phieu), ref _Phieu, value); }
        }

        private Nhansu _NhanvienShare;
        [Association("nv-chia")]
        [XafDisplayName("Người chia sẻ")]
        public Nhansu NhanvienShare
        {
            get { return _NhanvienShare; }
            set { SetPropertyValue<Nhansu>(nameof(NhanvienShare), ref _NhanvienShare, value); }
        }


        private Nhansu _NhanvienShared;
        [Association("nv-duocchia")]
        [XafDisplayName("Người được chia sẻ")]
        public Nhansu NhanvienShared
        {
            get { return _NhanvienShared; }
            set { SetPropertyValue<Nhansu>(nameof(NhanvienShared), ref _NhanvienShared, value); }
        }


        private string _Ykien;
        [XafDisplayName("Ghi chú"), Size(2048)]
        public string Ykien
        {
            get { return _Ykien; }
            set { SetPropertyValue<string>(nameof(Ykien), ref _Ykien, value); }
        }

        private DateTime _Ngay;
        [XafDisplayName("Ngày"), ModelDefault("AllowEdit", "false")]
        [ModelDefault("EditMask", "dd/MM/yyyy HH:mm")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay
        {
            get { return _Ngay; }
            set { SetPropertyValue<DateTime>(nameof(Ngay), ref _Ngay, value); }
        }

    }
}