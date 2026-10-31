using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    public class BophanItem
    {
        public string Name { get; set; }
        public object Id { get; set; }
        public object CategoryId { get; set; }
    }

    public class NhanvienItem
    {
        public string Sothe { get; set; }
        public string Hoten { get; set; }
        public string Chucvu { get; set; }
    }
    public class UserEnvironment
    {
        public string UserId { get; set; }
        public string Userkey { get; set; }
        public string Uservalue { get; set; }
    }
    public class Ykien
    {
        public int Id { get; set; }
        public string Ngay { get; set; }
        public string Nguoi { get; set; }
        public string Noidung { get; set; }
        public int NhanvienId { get; set; }
    }

    public class Chiases
    {
        public int Id { get; set; }
        public string Ngay { get; set; }
        public string Nguoi { get; set; }
        public string Nguoi2 { get; set; }
        public string Bophan { get; set; }
        public string Noidung { get; set; }
        public int NhanvienId { get; set; }
    }

    public class Tailieu
    {
        public int Id { get; set; }
        public string Nguoi { get; set; }
        public string Ngay { get; set; }
        public string Tentailieu { get; set; }
        public int NhanvienId { get; set; }
        public int Loai { get; set; }
        public string Tep { get; set; }
        public string Url { get; set; }
    }

    public class QTDuyet
    {
        public string Id { get; set; }
        public int SoTT { get; set; }
        public string Nguoiduyet { get; set; }
        public string Chucvu { get; set; }
        public string Han { get; set; }
        public string Ngay { get; set; }
        public string Ketluan { get; set; }
        public string Ghichu { get; set; }
        public string Boqua { get; set; }
    }

    [DomainComponent]
    public class ConfirmationWindowParameters
    {
        public ConfirmationWindowParameters() { }
        [ModelDefault("AllowEdit", "False")]
        public string ConfirmationMessage { get; set; }
    }

    public class TepDinhkem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string TepUrl { get; set; }
        public string TepName { get; set; }
        //        public string Tepgoc { get; set; }

    }

    public class MucHD
    {
        public int Id { get; set; }
        public string Tenmuc { get; set; }
    }
    public class TailieuHD
    {
        public int Id { get; set; }
        public int Idmuc { get; set; }
        public string Tentailieu { get; set; }
        public string Tep { get; set; }
    }

    [DomainComponent]
    [System.ComponentModel.DisplayName("Cập nhật sản phẩm")]
    [ImageName("import")]
    [NavigationItem(false)]
    public class ImportHangData
    {
        [DevExpress.ExpressApp.Data.Key]
        public Guid Oid { get; set; }

        [XafDisplayName("Mã")]
        public string MaSP { get; set; }

        [XafDisplayName("Tên vật tư, hàng hóa")]
        public string TenSP { get; set; }

        [XafDisplayName("Đơn vị tính")]
        public string DVT { get; set; }

        [XafDisplayName("Đơn giá")]
        public decimal Dongia { get; set; }

    }

}
