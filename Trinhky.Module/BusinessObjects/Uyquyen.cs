using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("uyquyen")]
    [DefaultProperty("SoUQ")]
    [System.ComponentModel.DisplayName("Ủy quyền")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Uyquyen : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Uyquyen(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _SoUQ;
        [XafDisplayName("Số UQ")]
        public string SoUQ
        {
            get { return _SoUQ; }
            set { SetPropertyValue<string>(nameof(SoUQ), ref _SoUQ, value); }
        }

        private DateTime _Ngayky;
        [XafDisplayName("Ngày hiệu lực")]
        public DateTime Ngayky
        {
            get { return _Ngayky; }
            set { SetPropertyValue<DateTime>(nameof(Ngayky), ref _Ngayky, value); }
        }

        private Chucdanh _ChucdanhUQ;
        [Association("ns-uq")]
        [XafDisplayName("Chức danh ủy quyền")]
        public Chucdanh ChucdanhUQ
        {
            get { return _ChucdanhUQ; }
            set { SetPropertyValue<Chucdanh>(nameof(ChucdanhUQ), ref _ChucdanhUQ, value); }
        }

        private Chucdanh _ChucdanhDuocUQ;
        [Association("ns-duocuq")]
        [XafDisplayName("Được ủy quyền")]
        public Chucdanh ChucdanhDuocUQ
        {
            get { return _ChucdanhDuocUQ; }
            set { SetPropertyValue<Chucdanh>(nameof(ChucdanhDuocUQ), ref _ChucdanhDuocUQ, value); }
        }

        private MauVanban _Mau;
        [Association]
        [XafDisplayName("Mẫu ủy quyền")]
        public MauVanban Mau
        {
            get { return _Mau; }
            set { SetPropertyValue<MauVanban>(nameof(Mau), ref _Mau, value); }
        }

        private FileData _Tep;
        [XafDisplayName("Tệp UQ")]
        public FileData Tep
        {
            get { return _Tep; }
            set { SetPropertyValue<FileData>(nameof(Tep), ref _Tep, value); }
        }

        private bool _Huy;
        [XafDisplayName("Hủy")]
        public bool Huy
        {
            get { return _Huy; }
            set { SetPropertyValue<bool>(nameof(Huy), ref _Huy, value); }
        }
    }
}