using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("Áp dụng mẫu")]
    [ImageName("mauvb")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class MauBP : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public MauBP(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Bophan _Donvi;
        [Association]
        [XafDisplayName("Bộ phận")]
        public Bophan Donvi
        {
            get { return _Donvi; }
            set { SetPropertyValue<Bophan>(nameof(Donvi), ref _Donvi, value); }
        }

        private MauVanban _Mau;
        [Association]
        [XafDisplayName("Mẫu sử dụng")]
        public MauVanban Mau
        {
            get { return _Mau; }
            set { SetPropertyValue<MauVanban>(nameof(Mau), ref _Mau, value); }
        }



    }
}