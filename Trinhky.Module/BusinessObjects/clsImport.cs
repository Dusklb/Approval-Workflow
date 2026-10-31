using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Cập nhật dữ liệu")]
    public class ClsImport : XPLiteObject
    {
        public ClsImport(Session session)
            : base(session)
        {
        }
        Guid fImportId;
        [Key(true), Browsable(false)]
        public Guid ImportId
        {
            get { return fImportId; }
            set { SetPropertyValue<Guid>(nameof(ImportId), ref fImportId, value); }
        }

        private Bophan _Donvi;
        [XafDisplayName("Bộ phận")]
        public Bophan Donvi
        {
            get { return _Donvi; }
            set { SetPropertyValue<Bophan>(nameof(Donvi), ref _Donvi, value); }
        }

        private Coso _CS;
        [XafDisplayName("Cơ sở")]
        public Coso CS
        {
            get { return _CS; }
            set { SetPropertyValue<Coso>(nameof(CS), ref _CS, value); }
        }

        private string _Noidung;
        [XafDisplayName("Dữ liệu Clipboard")]
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        public string Noidung
        {
            get { return _Noidung; }
            set { SetPropertyValue<string>(nameof(Noidung), ref _Noidung, value); }
        }

        private BindingList<ClsNhanvien> _ImportNhanviens;
        [XafDisplayName("Danh sách")]
        public BindingList<ClsNhanvien> ImportNhanviens
        {
            get
            {
                _ImportNhanviens ??= new BindingList<ClsNhanvien>();
                return _ImportNhanviens;
            }
        }
    }
}
