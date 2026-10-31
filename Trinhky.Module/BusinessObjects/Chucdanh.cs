using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("Chức danh duyệt")]
    [ImageName("chucvu")]
    [DefaultProperty("TenChucvu")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Chucdanh : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Chucdanh(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _TenChucvu;
        [XafDisplayName("Tên chức danh")]
        public string TenChucvu
        {
            get { return _TenChucvu; }
            set { SetPropertyValue<string>(nameof(TenChucvu), ref _TenChucvu, value); }
        }

        private bool _TheoCoso = false;
        [XafDisplayName("Cơ sở ?"), ToolTip("Xác định chức danh theo cơ sở làm việc")]
        public bool TheoCoso
        {
            get { return _TheoCoso; }
            set { SetPropertyValue<bool>(nameof(TheoCoso), ref _TheoCoso, value); }
        }

        private string _Code;
        [XafDisplayName("Mã họ tên")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue<string>(nameof(Code), ref _Code, value); }
        }

        private string _CodeCK;
        [XafDisplayName("Mã chữ ký")]
        public string CodeCK
        {
            get { return _CodeCK; }
            set { SetPropertyValue<string>(nameof(CodeCK), ref _CodeCK, value); }
        }

        private string _MaKythay;
        [XafDisplayName("Mã ký UQ")]
        public string MaKythay
        {
            get { return _MaKythay; }
            set { SetPropertyValue<string>(nameof(MaKythay), ref _MaKythay, value); }
        }

        private bool _BanTGD;
        [XafDisplayName("Ban TGĐ")]
        public bool BanTGD
        {
            get { return _BanTGD; }
            set { SetPropertyValue<bool>(nameof(BanTGD), ref _BanTGD, value); }
        }

        [Association, Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<QuitrinhChuan> Quitrinhs
        {
            get { return GetCollection<QuitrinhChuan>(nameof(Quitrinhs)); }
        }

        //[Association, Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        //public XPCollection<DuyetDexuat> DuyetDexuats
        //{
        //    get { return GetCollection<DuyetDexuat>(nameof(DuyetDexuats)); }
        //}

        [Association, Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<MauVanban> Mauvb
        {
            get { return GetCollection<MauVanban>(nameof(Mauvb)); }
        }

        [Association, Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<ChucvuNV> ChucvuNVs
        {
            get { return GetCollection<ChucvuNV>(nameof(ChucvuNVs)); }
        }

        [Association("ns-uq")]
        [XafDisplayName("Ủy quyền")]
        public XPCollection<Uyquyen> Uyquyens
        {
            get { return GetCollection<Uyquyen>(nameof(Uyquyens)); }
        }

        [Association("ns-duocuq")]
        [XafDisplayName("Được ủy quyền")]
        public XPCollection<Uyquyen> DuocUQs
        {
            get { return GetCollection<Uyquyen>(nameof(DuocUQs)); }
        }
    }
}