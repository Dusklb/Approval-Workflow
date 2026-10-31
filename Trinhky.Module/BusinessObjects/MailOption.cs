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
    [ImageName("mailbox")]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Tùy chọn Mail")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class MailOption : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public MailOption(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        int _Oid;
        [Key(true), Browsable(false)]
        public int Oid
        {
            get { return _Oid; }
            set { SetPropertyValue<int>(nameof(Oid), ref _Oid, value); }
        }

        private string _TenMail;
        [XafDisplayName("Tên Email giao dịch")]
        public string TenMail
        {
            get { return _TenMail; }
            set { SetPropertyValue<string>(nameof(TenMail), ref _TenMail, value); }
        }

        private string _Email;
        [XafDisplayName("Email giao dịch")]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue<string>(nameof(Email), ref _Email, value); }
        }

        private string _MailPass;
        [XafDisplayName("Mật khẩu Email")]
        public string MailPass
        {
            get { return _MailPass; }
            set { SetPropertyValue<string>(nameof(MailPass), ref _MailPass, value); }
        }


        private string _Chuky;
        [XafDisplayName("Ký cuối Email"), Size(512)]
        public string Chuky
        {
            get { return _Chuky; }
            set { SetPropertyValue<string>(nameof(Chuky), ref _Chuky, value); }
        }
    }
}