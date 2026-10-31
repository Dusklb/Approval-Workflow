using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("help")]
    [DefaultProperty("Muc")]
    [System.ComponentModel.DisplayName("Cập nhật hướng dẫn")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Huongdan : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Huongdan(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private int _SoTT;
        [XafDisplayName("TT")]
        public int SoTT
        {
            get { return _SoTT; }
            set { SetPropertyValue<int>(nameof(SoTT), ref _SoTT, value); }
        }

        private string _Muc;
        [XafDisplayName("Mục"), Size(255)]
        public string Muc
        {
            get { return _Muc; }
            set { SetPropertyValue<string>(nameof(Muc), ref _Muc, value); }
        }

        //private string _Noidung;
        //[XafDisplayName("Nội dung")]
        //[Size(SizeAttribute.Unlimited)]
        //[EditorAlias(EditorAliases.RichTextPropertyEditor)]
        //public string Noidung
        //{
        //    get { return _Noidung; }
        //    set { SetPropertyValue<string>(nameof(Noidung), ref _Noidung, value); }
        //}

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Tệp hướng dẫn")]
        public XPCollection<HuongdanTep> HuongdanTeps
        {
            get { return GetCollection<HuongdanTep>(nameof(HuongdanTeps)); }
        }

        [XafDisplayName("Số tài liệu")]
        public int Sotep
        {
            get { return HuongdanTeps.Count; }
        }
    }
}