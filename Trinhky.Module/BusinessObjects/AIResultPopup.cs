using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;

namespace Trinhky.Module.BusinessObjects
{
    [DomainComponent]
    public class AIResultPopup
    {
        public AIResultPopup(string content)
        {
            this.HtmlContent = content;
        }

        // 1. BẮT BUỘC: Để XAF không cắt ngắn và dùng editor nhiều dòng
        [FieldSize(SizeAttribute.Unlimited)]

        // 2. EditorAlias: Để kích hoạt trình hiển thị HTML (DevExpress Blazor Rich Edit)
        [ModelDefault("PropertyEditorType", "Trinhky.Blazor.Server.Editors.RawHtmlPropertyEditor")]
        public string HtmlContent { get; set; }
    }

    [DomainComponent]
    [ModelDefault("Caption", "Yêu cầu cụ thể cho AI")]
    public class AIInputParam
    {
        [ModelDefault("Caption", "Ghi chú thêm")]
        [FieldSize(FieldSizeAttribute.Unlimited)]
        [ModelDefault("RowCount", "5")] // Hiện 5 dòng để nhập cho thoải mái
        public string YeuCauChiTiet { get; set; }
    }
}
