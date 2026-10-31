using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DomainComponent]
    [System.ComponentModel.DisplayName("Danh sách nhân viên")]
    [ImageName("nhanvien")]
    [NavigationItem(false)]
    public class DanhsachNV
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("Họ và Tên")]
        public string Hoten { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Cơ sở")]
        public string Coso { get; set; }

        [XafDisplayName("Chức vụ")]
        public string Chucvu { get; set; }
    }

    [DomainComponent]
    [NavigationItem(false)]
    [System.ComponentModel.DisplayName("Chia sẻ")]
    [ImageName("share")]
    public class ChiaseList//: XPLiteObject
    {
        //public ChiaseList(Session session)
        //    : base(session)
        //{
        //}
        //Guid fOid;
        //[Key(true), Browsable(false)]
        //public Guid Oid
        //{
        //    get { return fOid; }
        //    set { SetPropertyValue<Guid>(nameof(Oid), ref fOid, value); }
        //}
        //private string _Noidung;
        [Browsable(false)]
        public int PhieuOid { get; set; }

        [Browsable(false)]
        public int NhanvienOid { get; set; }


        [XafDisplayName("Ý kiến")]
        //[Size(SizeAttribute.Unlimited)]
        public string Noidung { get; set; }

        private BindingList<DanhsachNV> _DanhsachNVs;
        [XafDisplayName("Nhân sự")]
        public BindingList<DanhsachNV> DanhsachNVs
        {
            get
            {
                _DanhsachNVs ??= new BindingList<DanhsachNV>();
                return _DanhsachNVs;
            }
        }
        private BindingList<DanhsachNV> _DanhsachChons;
        [XafDisplayName("Chọn")]
        public BindingList<DanhsachNV> DanhsachChons
        {
            get
            {
                _DanhsachChons ??= new BindingList<DanhsachNV>();
                return _DanhsachChons;
            }
        }

    }
}
