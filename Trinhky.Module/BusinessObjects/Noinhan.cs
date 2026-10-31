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
    [System.ComponentModel.DisplayName("Bộ phận duyệt")]
    [ImageName("bophan")]
    [DefaultProperty("Tenbophan")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Noinhan : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Noinhan(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _Tenbophan;
        [XafDisplayName("Tên bộ phận")]
        public string Tenbophan
        {
            get { return _Tenbophan; }
            set { SetPropertyValue<string>(nameof(Tenbophan), ref _Tenbophan, value); }
        }

        [XafDisplayName("Số Đề xuất")]
        public int SoDX
        {
            get { return Phieudexuats.Count; }
        }

        [Association]
        [XafDisplayName("Đề xuất"), Browsable(false)]
        [RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Phieudexuat> Phieudexuats
        {
            get { return GetCollection<Phieudexuat>(nameof(Phieudexuats)); }
        }


    }
}