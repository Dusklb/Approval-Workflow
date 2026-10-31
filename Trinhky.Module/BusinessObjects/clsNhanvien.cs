using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinhky.Module.BusinessObjects
{
    [DomainComponent]
    [System.ComponentModel.DisplayName("Cập nhật nhân viên")]
    //[ImageName("blazor-clipboard-import")]
    [NavigationItem(false)]
    public class ClsNhanvien
    {
        [DevExpress.ExpressApp.Data.Key]
        public Guid Oid { get; set; }

        [XafDisplayName("Số thẻ")]
        public string Sothe { get; set; }

        [XafDisplayName("Họ và Tên")]
        public string Hoten { get; set; }

        [XafDisplayName("Giới tính")]
        public string Gioitinh { get; set; }

        [XafDisplayName("Email")]
        public string Email { get; set; }

        [XafDisplayName("Điện thoại")]
        public string Dienthoai { get; set; }
    }
}
