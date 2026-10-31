using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("Nhân sự")]
    [ImageName("nhanvien")]
    [DefaultProperty("HoTen")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Nhansu : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Nhansu(Session session)
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

        private Coso _CS;
        [Association]
        [XafDisplayName("Cơ sở")]
        public Coso CS
        {
            get { return _CS; }
            set { SetPropertyValue<Coso>(nameof(CS), ref _CS, value); }
        }

        private string _Sothe;
        [XafDisplayName("Số thẻ")]
        public string Sothe
        {
            get { return _Sothe; }
            set { SetPropertyValue<string>(nameof(Sothe), ref _Sothe, value); }
        }

        private string _HoTen;
        [XafDisplayName("Họ và Tên")]
        public string HoTen
        {
            get { return _HoTen; }
            set { SetPropertyValue<string>(nameof(HoTen), ref _HoTen, value); }
        }

        private string _Gioitinh;
        [XafDisplayName("Giới tính"), Size(10)]
        public string Gioitinh
        {
            get { return _Gioitinh; }
            set { SetPropertyValue<string>(nameof(Gioitinh), ref _Gioitinh, value); }
        }

        private string _Email;
        [XafDisplayName("Email"), Size(255)]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue<string>(nameof(Email), ref _Email, value); }
        }

        private string _Phone;
        [XafDisplayName("Điện thoại")]
        public string Phone
        {
            get { return _Phone; }
            set { SetPropertyValue<string>(nameof(Phone), ref _Phone, value); }
        }

        private bool _XemAll;//update du lieu cu =false
        [XafDisplayName("Xem tất cả")]
        public bool XemAll
        {
            get { return _XemAll; }
            set { SetPropertyValue<bool>(nameof(XemAll), ref _XemAll, value); }
        }

        private bool _Nghiviec;
        [XafDisplayName("Nghỉ việc")]
        public bool Nghiviec
        {
            get { return _Nghiviec; }
            set { SetPropertyValue<bool>(nameof(Nghiviec), ref _Nghiviec, value); }
        }


        [Delayed(true), VisibleInListViewAttribute(true)]
        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit,
            DetailViewImageEditorMode = ImageEditorMode.PictureEdit)]
        [XafDisplayName("Ảnh chữ ký")]
        public byte[] AnhCK
        {
            get { return GetDelayedPropertyValue<byte[]>(nameof(AnhCK)); }
            set { SetDelayedPropertyValue<byte[]>(nameof(AnhCK), value); }
        }

        [XafDisplayName("Chức vụ")]
        public string ListCV
        {
            get
            {
                string t = "";
                foreach (ChucvuNV item in ChucvuNVs)
                {
                    if (item.Chucvu != null)
                        t += item.Chucvu.TenChucvu + ", ";
                }
                if (t.Length > 2) t = tmLib.CommonLib.Left(t, t.Length - 2);
                return t;
            }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Chức vụ")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<ChucvuNV> ChucvuNVs
        {
            get { return GetCollection<ChucvuNV>(nameof(ChucvuNVs)); }
        }

        [DevExpress.Xpo.Aggregated, Association("dexuat")]
        [XafDisplayName("Đề xuất")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Phieudexuat> Dexuats
        {
            get { return GetCollection<Phieudexuat>(nameof(Dexuats)); }
        }

        [DevExpress.Xpo.Aggregated, Association("chuthe")]
        [XafDisplayName("Đề xuất áp dụng")]
        public XPCollection<Phieudexuat> Apdungs
        {
            get { return GetCollection<Phieudexuat>(nameof(Apdungs)); }
        }

        [Association("xuly")]
        [XafDisplayName("Xử lý")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Phieudexuat> Phieudexuats
        {
            get { return GetCollection<Phieudexuat>(nameof(Phieudexuats)); }
        }

        [Association("ns-duyet")]
        [XafDisplayName("Duyệt")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<DuyetDexuat> DuyetDexuats
        {
            get { return GetCollection<DuyetDexuat>(nameof(DuyetDexuats)); }
        }

        [Association("ns-duyetuq")]
        [XafDisplayName("Duyệt UQ")]
        public XPCollection<DuyetDexuat> DuyetUQs
        {
            get { return GetCollection<DuyetDexuat>(nameof(DuyetUQs)); }
        }


        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Ý kiến")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Quatrinh> Quatrinhs
        {
            get { return GetCollection<Quatrinh>(nameof(Quatrinhs)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Tài liệu")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<PhieudexuatFile> PhieudexuatFiles
        {
            get { return GetCollection<PhieudexuatFile>(nameof(PhieudexuatFiles)); }
        }

        [Association("nv-chia")]
        [XafDisplayName("Tài liệu chia sẻ")]
        public XPCollection<Chiase> Chiases
        {
            get { return GetCollection<Chiase>(nameof(Chiases)); }
        }

        [Association("nv-duocchia")]
        [XafDisplayName("Tài liệu được chia sẻ")]
        public XPCollection<Chiase> DuocCS
        {
            get { return GetCollection<Chiase>(nameof(DuocCS)); }
        }

        [Association("nv-thuchien")]
        [XafDisplayName("Thực hiện")]
        public XPCollection<Phieudexuat> Thuchiens
        {
            get { return GetCollection<Phieudexuat>(nameof(Thuchiens)); }
        }

    }
}