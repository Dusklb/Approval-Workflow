using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("dinhkem")]
    [System.ComponentModel.DisplayName("Tài liệu")]
    [NavigationItem(false)]
    [DefaultProperty("Tailieu")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    public class HuongdanTep : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public HuongdanTep(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Huongdan _Help;
        [Association]
        public Huongdan Help
        {
            get { return _Help; }
            set { SetPropertyValue<Huongdan>(nameof(Help), ref _Help, value); }
        }

        private string _Tailieu;
        [XafDisplayName("Tên tài liệu"), Size(255)]
        public string Tailieu
        {
            get { return _Tailieu; }
            set { SetPropertyValue<string>(nameof(Tailieu), ref _Tailieu, value); }
        }

        private FileData _Tep;
        [XafDisplayName("Tệp")]
        public FileData Tep
        {
            get { return _Tep; }
            set { SetPropertyValue<FileData>(nameof(Tep), ref _Tep, value); }
        }

    }
}