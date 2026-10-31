using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("steps")]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Qui trình")]
    [DefaultProperty("SoTT")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class QuitrinhChuan : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public QuitrinhChuan(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private MauVanban _Mau;
        [Association]
        [XafDisplayName("Mẫu")]
        public MauVanban Mau
        {
            get { return _Mau; }
            set { SetPropertyValue<MauVanban>(nameof(Mau), ref _Mau, value); }
        }

        private int _SoTT;
        [XafDisplayName("Bước")]
        public int SoTT
        {
            get { return _SoTT; }
            set { SetPropertyValue<int>(nameof(SoTT), ref _SoTT, value); }
        }

        private Chucdanh _Chucvu;
        [Association]
        [XafDisplayName("Người duyệt")]
        public Chucdanh Chucvu
        {
            get { return _Chucvu; }
            set { SetPropertyValue<Chucdanh>(nameof(Chucvu), ref _Chucvu, value); }
        }

        private int _Songay;
        [XafDisplayName("Số ngày duyệt")]
        public int Songay
        {
            get { return _Songay; }
            set { SetPropertyValue<int>(nameof(Songay), ref _Songay, value); }
        }
    }
}