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
    [System.ComponentModel.DisplayName("Mẫu văn bản")]
    [ImageName("mauvb")]
    [DefaultProperty("Tenmau")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class MauVanban : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public MauVanban(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private string _Kyhieu;
        [XafDisplayName("Ký hiệu mẫu"), Size(20)]
        public string Kyhieu
        {
            get { return _Kyhieu; }
            set { SetPropertyValue<string>(nameof(Kyhieu), ref _Kyhieu, value); }
        }

        private string _Tenmau;
        [XafDisplayName("Tên mẫu"), Size(255)]
        public string Tenmau
        {
            get { return _Tenmau; }
            set { SetPropertyValue<string>(nameof(Tenmau), ref _Tenmau, value); }
        }

        private bool _ApdungCS;
        [XafDisplayName("Cho Cơ sở")]
        public bool ApdungCS
        {
            get { return _ApdungCS; }
            set { SetPropertyValue<bool>(nameof(ApdungCS), ref _ApdungCS, value); }
        }

        private bool _Thuchien;
        [XafDisplayName("Thực hiện")]
        public bool Thuchien
        {
            get { return _Thuchien; }
            set { SetPropertyValue<bool>(nameof(Thuchien), ref _Thuchien, value); }
        }

        private bool _Huy;
        [XafDisplayName("Hủy")]
        public bool Huy
        {
            get { return _Huy; }
            set { SetPropertyValue<bool>(nameof(Huy), ref _Huy, value); }
        }

        private bool _ChonNV;
        [XafDisplayName("Chọn chủ thể")]
        public bool ChonNV
        {
            get { return _ChonNV; }
            set { SetPropertyValue<bool>(nameof(ChonNV), ref _ChonNV, value); }
        }

        private bool _ChonVT;
        [XafDisplayName("Chọn vật tư")]
        public bool ChonVT
        {
            get { return _ChonVT; }
            set { SetPropertyValue<bool>(nameof(ChonVT), ref _ChonVT, value); }
        }


        private bool _Xoaduyet;
        [XafDisplayName("Xóa bước duyệt")]
        public bool Xoaduyet
        {
            get { return _Xoaduyet; }
            set { SetPropertyValue<bool>(nameof(Xoaduyet), ref _Xoaduyet, value); }
        }

        private Phanloai _Loai;
        [Association]
        [XafDisplayName("Loại VB")]
        public Phanloai Loai
        {
            get { return _Loai; }
            set { SetPropertyValue<Phanloai>(nameof(Loai), ref _Loai, value); }
        }

        private Chucdanh _Chucvuxuly;
        [Association]
        [XafDisplayName("Xử lý sau duyệt")]
        public Chucdanh Chucvuxuly
        {
            get { return _Chucvuxuly; }
            set { SetPropertyValue<Chucdanh>(nameof(Chucvuxuly), ref _Chucvuxuly, value); }
        }

        private string _Noidung;
        [XafDisplayName("Nội dung")]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.RichTextPropertyEditor)]
        public string Noidung
        {
            get { return _Noidung; }
            set { SetPropertyValue<string>(nameof(Noidung), ref _Noidung, value); }
        }

        private string _Kyduyet;
        [XafDisplayName("Ký duyệt")]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.RichTextPropertyEditor)]
        public string Kyduyet
        {
            get { return _Kyduyet; }
            set { SetPropertyValue<string>(nameof(Kyduyet), ref _Kyduyet, value); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Qui trình duyệt")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<QuitrinhChuan> QuitrinhChuans
        {
            get { return GetCollection<QuitrinhChuan>(nameof(QuitrinhChuans)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Đề xuất")]
        public XPCollection<Phieudexuat> Dexuats
        {
            get { return GetCollection<Phieudexuat>(nameof(Dexuats)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Bộ phận sử dụng")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<MauBP> MauBPs
        {
            get { return GetCollection<MauBP>(nameof(MauBPs)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Mẫu ủy quyền")]
        public XPCollection<Uyquyen> Uyquyens
        {
            get { return GetCollection<Uyquyen>(nameof(Uyquyens)); }
        }

    }
}