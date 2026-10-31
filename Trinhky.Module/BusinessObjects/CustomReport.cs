using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("choduyet")]
    [System.ComponentModel.DisplayName("Đang chờ duyệt")]
    [Appearance("ColorListcho1", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat;Han", Criteria = "Han<Homnay", Context = "ListView",
        FontColor = "red", Priority = 1)]
    public class Phieuduyet
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Đề xuất")]
        public string TenDexuat { get; set; }

        [XafDisplayName("Người lập")]
        public string Nguoi { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay { get; set; }

        [XafDisplayName("Hạn duyệt")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        public DateTime Han { get; set; }

        [XafDisplayName("Hôm nay"), Browsable(false)]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Homnay { get; set; }

        [XafDisplayName("Tiến trình")]
        [EditorAlias("ProgressProperty")]
        [ModelDefault("DisplayFormat", "")]
        public int TyleDuyet { get; set; }

        [XafDisplayName("Chủ thể")]
        public string Doituong { get; set; }
        //[XafDisplayName("Tài liệu")]
        //public int Tailieu { get; set; }

        //[XafDisplayName("Qui trình")]
        //public TCom.EStatusVB Trangthai { get; set; }
        //public string Daduyet { get; set; }
    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("daduyet")]
    [System.ComponentModel.DisplayName("Đã duyệt")]
    [Appearance("DaduyetListView1", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=2", Context = "ListView",
        FontColor = "Blue", Priority = 1)]
    [Appearance("DaduyetListView2", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=3", Context = "ListView",
        FontColor = "Blue", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, Priority = 1)]
    [Appearance("DaduyetListView3", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=1", Context = "ListView",
        FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Strikeout, Priority = 1)]

    [Appearance("DaduyetListView4", AppearanceItemType = "ViewItem", TargetItems = "Trangthai", Criteria = "Trangthai=2", Context = "ListView",
        FontColor = "Blue", Priority = 1)]
    [Appearance("DaduyetListView5", AppearanceItemType = "ViewItem", TargetItems = "Trangthai", Criteria = "Trangthai=3", Context = "ListView",
        FontColor = "Blue", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, Priority = 1)]
    [Appearance("DaduyetListView6", AppearanceItemType = "ViewItem", TargetItems = "Trangthai", Criteria = "Trangthai=1", Context = "ListView",
        FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Strikeout, Priority = 1)]

    public class PhieuDaduyet
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Đề xuất")]
        public string TenDexuat { get; set; }

        [XafDisplayName("Người lập")]
        public string Nguoi { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay { get; set; }

        [XafDisplayName("Ngày duyệt")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayDuyet { get; set; }

        [XafDisplayName("Qui trình")]
        public TCom.EStatusVB Trangthai { get; set; }
        //public string Daduyet { get; set; }
        [XafDisplayName("Chủ thể")]
        public string Doituong { get; set; }

    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("dahuy")]
    [System.ComponentModel.DisplayName("Đã hủy")]
    public class PhieuDahuy
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Đề xuất")]
        public string TenDexuat { get; set; }

        [XafDisplayName("Người lập")]
        public string Nguoi { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay { get; set; }

        [XafDisplayName("Ngày hủy")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayDuyet { get; set; }

        [XafDisplayName("Chủ thể")]
        public string Doituong { get; set; }

    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("choxuly")]
    [System.ComponentModel.DisplayName("Xử lý sau duyệt")]
    public class PhieuXuly
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Đề xuất")]
        public string TenDexuat { get; set; }

        [XafDisplayName("Người lập")]
        public string Nguoi { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay { get; set; }

        [XafDisplayName("Ngày duyệt")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayDuyet { get; set; }

        [XafDisplayName("Chủ thể")]
        public string Doituong { get; set; }

        [XafDisplayName("Giao cho")]
        public string Giaocho { get; set; }

        [XafDisplayName("Yêu cầu")]
        public string Yeucau { get; set; }

        [XafDisplayName("Hoàn thành")]
        public bool Daxong { get; set; }

        [XafDisplayName("Ngày hoàn thành")]
        //[ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public string Ngayxong { get; set; }

        [XafDisplayName("Trạng thái")]
        public string XLStatus { get; set; }
    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("tkdexuat")]
    [System.ComponentModel.DisplayName("Thống kê đề xuất theo người")]
    public class ThongkeDexuat
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Họ và Tên")]
        public string Hoten { get; set; }

        [XafDisplayName("Đơn vị")]
        public string Donvi { get; set; }

        [XafDisplayName("Số đề xuất")]
        public int Sodx { get; set; }

        [XafDisplayName("Chưa duyệt")]
        public int Chuaduyet { get; set; }

        [XafDisplayName("Đang duyệt")]
        public int Dangduyet { get; set; }

        [XafDisplayName("Đã hủy")]
        public int Huy { get; set; }

        [XafDisplayName("Đã duyệt")]
        public int Daduyet { get; set; }

        [XafDisplayName("Thực hiện")]
        public int Daxong { get; set; }
    }


    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("tkduyet")]
    [System.ComponentModel.DisplayName("Thống kê duyệt")]
    public class ThongkeDuyet
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Họ và Tên")]
        public string Hoten { get; set; }

        [XafDisplayName("Chức danh")]
        public string Chucdanhky { get; set; }

        [XafDisplayName("Số lượt duyệt")]
        public int Sodx { get; set; }

        [XafDisplayName("Chưa duyệt trong hạn")]
        public int Chuaduyet { get; set; }

        [XafDisplayName("Chưa duyệt quá hạn")]
        public int Chuaduyet2 { get; set; }

        [XafDisplayName("Đã hủy")]
        public int Huy { get; set; }

        [XafDisplayName("Duyệt đúng hạn")]
        public int Dunghan { get; set; }

        [XafDisplayName("Duyệt quá hạn")]
        public int Quahan { get; set; }

    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("tkdexuat")]
    [System.ComponentModel.DisplayName("Thống kê đề xuất cá nhân")]
    public class DexuatCanhan
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("Thông tin")]
        public string Thongtin { get; set; }

        [XafDisplayName("Số đề xuất")]
        public int Sodx { get; set; }

    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("tkduyet")]
    [System.ComponentModel.DisplayName("Thống kê duyệt cá nhân")]
    public class DuyetDexuatCanhan
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("Thông tin")]
        public string Thongtin { get; set; }

        [XafDisplayName("Số đề xuất")]
        public int Sodx { get; set; }

    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("tkdexuat")]
    [System.ComponentModel.DisplayName("Thống kê đề xuất theo mẫu")]
    public class ThongkeDexuatMau
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Loại mẫu")]
        public string Loaimau { get; set; }

        [XafDisplayName("Số đề xuất")]
        public int Sodx { get; set; }

        [XafDisplayName("Chưa duyệt")]
        public int Chuaduyet { get; set; }

        [XafDisplayName("Đang duyệt")]
        public int Dangduyet { get; set; }

        [XafDisplayName("Đã hủy")]
        public int Huy { get; set; }

        [XafDisplayName("Đã duyệt")]
        public int Daduyet { get; set; }

        [XafDisplayName("Thực hiện")]
        public int Daxong { get; set; }
    }

    [DomainComponent]
    [NavigationItem("mnuDanhmuc")]
    [ImageName("share")]
    [System.ComponentModel.DisplayName("Chia sẻ")]
    [Appearance("DaduyetListView1", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=2", Context = "ListView",
    FontColor = "Blue", Priority = 1)]
    [Appearance("DaduyetListView2", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=3", Context = "ListView",
    FontColor = "Blue", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, Priority = 1)]
    [Appearance("DaduyetListView3", AppearanceItemType = "ViewItem", TargetItems = "TenDexuat", Criteria = "Trangthai=1", Context = "ListView",
    FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Strikeout, Priority = 1)]

    public class PhieuChiase
    {
        [DevExpress.ExpressApp.Data.Key, Browsable(false)]
        public int Oid { get; set; }

        [XafDisplayName("TT")]
        public int TT { get; set; }

        [XafDisplayName("Đề xuất")]
        public string TenDexuat { get; set; }

        [XafDisplayName("Người lập")]
        public string Nguoi { get; set; }

        [XafDisplayName("Bộ phận")]
        public string Bophan { get; set; }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay { get; set; }

        [XafDisplayName("Qui trình")]
        public TCom.EStatusVB Trangthai { get; set; }
        //public string Daduyet { get; set; }

        [XafDisplayName("Người chia sẻ")]
        public string NguoiCS { get; set; }
        [XafDisplayName("Ý kiến")]
        public string Ykien { get; set; }
    }

}
