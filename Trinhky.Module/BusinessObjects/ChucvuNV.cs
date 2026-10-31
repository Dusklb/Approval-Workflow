using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("chucvu")]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Chức danh")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ChucvuNV : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ChucvuNV(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Nhansu _Nhanvien;
        [Association]
        [XafDisplayName("Nhân viên")]
        public Nhansu Nhanvien
        {
            get { return _Nhanvien; }
            set { SetPropertyValue<Nhansu>(nameof(Nhanvien), ref _Nhanvien, value); }
        }

        private Chucdanh _Chucvu;
        [XafDisplayName("Chức vụ")]
        [Association]
        public Chucdanh Chucvu
        {
            get { return _Chucvu; }
            set { SetPropertyValue<Chucdanh>(nameof(Chucvu), ref _Chucvu, value); }
        }



    }
}