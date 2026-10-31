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
    [ImageName("phanloai")]
    [System.ComponentModel.DisplayName("Phân loại")]
    [DefaultProperty("Tenloai")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Phanloai : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Phanloai(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _Tenloai;
        [XafDisplayName("Tên phân loại")]
        public string Tenloai
        {
            get { return _Tenloai; }
            set { SetPropertyValue<string>(nameof(Tenloai), ref _Tenloai, value); }
        }

        [Association, Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<MauVanban> MauVanbans
        {
            get { return GetCollection<MauVanban>(nameof(MauVanbans)); }
        }


    }
}