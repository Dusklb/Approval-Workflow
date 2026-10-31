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
    [ImageName("products")]
    [System.ComponentModel.DisplayName("Vật tư")]
    [DefaultProperty("Tenvt")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Vattu : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Vattu(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private NhomVT _Nhom;
        [Association]
        [XafDisplayName("Nhóm")]
        public NhomVT Nhom
        {
            get { return _Nhom; }
            set { SetPropertyValue<NhomVT>(nameof(Nhom), ref _Nhom, value); }
        }

        private string _MaVT;
        [XafDisplayName("Mã"), Size(30)]
        public string MaVT
        {
            get { return _MaVT; }
            set { SetPropertyValue<string>(nameof(MaVT), ref _MaVT, value); }
        }

        private string _Tenvt;
        [XafDisplayName("Tên vật tư, hàng hóa"), Size(255)]
        public string Tenvt
        {
            get { return _Tenvt; }
            set { SetPropertyValue<string>(nameof(Tenvt), ref _Tenvt, value); }
        }

        private string _Dvt;
        [XafDisplayName("Đơn vị tính"), Size(20)]
        public string Dvt
        {
            get { return _Dvt; }
            set { SetPropertyValue<string>(nameof(Dvt), ref _Dvt, value); }
        }

        private decimal _Dongia;
        [XafDisplayName("Đơn giá")]
        [ModelDefault("EditMask", "###.###.###")]
        [ModelDefault("DisplayFormat", "{0:###.###.###}")]
        public decimal Dongia
        {
            get { return _Dongia; }
            set { SetPropertyValue<decimal>(nameof(Dongia), ref _Dongia, value); }
        }
    }
}