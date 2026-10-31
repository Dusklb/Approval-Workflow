using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace Trinhky.Blazor.Server.Editors
{
    // ==========================================
    // 1. KHAI BÁO EDITOR
    // ==========================================
    [PropertyEditor(typeof(string), "RawHtmlViewer", false)]
    public class RawHtmlPropertyEditor : BlazorPropertyEditorBase
    {
        public RawHtmlPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override IComponentAdapter CreateComponentAdapter()
        {
            var model = new RawHtmlComponentModel();
            return new RawHtmlAdapter(model);
        }
    }

    // ==========================================
    // 2. MODEL
    // ==========================================
    public class RawHtmlComponentModel : ComponentModelBase
    {
        public string HtmlContent { get; set; }
        public bool ReadOnly { get; set; }
    }

    // ==========================================
    // 3. ADAPTER (ĐÃ SỬA LỖI RENDERFRAGMENT)
    // ==========================================
    public class RawHtmlAdapter : ComponentAdapterBase
    {
        private RawHtmlComponentModel _componentModel;

        public RawHtmlAdapter(RawHtmlComponentModel componentModel)
        {
            _componentModel = componentModel;
        }

        public override ComponentModelBase ComponentModel => _componentModel;

        public override void SetValue(object value)
        {
            _componentModel.HtmlContent = value as string;
        }

        public override object GetValue()
        {
            return _componentModel.HtmlContent;
        }

        // 👇 ĐÃ SỬA: Trả về RenderFragment thay vì IComponent
        protected override RenderFragment CreateComponent()
        {
            // Tạo một RenderFragment để vẽ Component "RawHtmlRenderer"
            return builder =>
            {
                builder.OpenComponent<RawHtmlRenderer>(0);
                // Truyền Model vào Component qua Parameter
                builder.AddAttribute(1, nameof(RawHtmlRenderer.ComponentModel), _componentModel);
                builder.CloseComponent();
            };
        }

        // --- CÁC HÀM IMPLEMENT RỖNG (Để thỏa mãn class cha) ---
        public override void SetAllowEdit(bool allowEdit) { _componentModel.ReadOnly = !allowEdit; }
        public override void SetDisplayFormat(string displayFormat) { }
        public override void SetEditMask(string editMask) { }
        public override void SetEditMaskType(EditMaskType editMaskType) { }
        public override void SetErrorIcon(ImageInfo errorIcon) { }
        public override void SetErrorMessage(string errorMessage) { }
        public override void SetIsPassword(bool isPassword) { }
        public override void SetMaxLength(int maxLength) { }
        public override void SetNullText(string nullText) { }
        public override void SetAllowNull(bool allowNull) { }
    }

    // ==========================================
    // 4. RENDERER (ĐÃ SỬA ĐỂ NHẬN PARAMETER)
    // ==========================================
    public class RawHtmlRenderer : ComponentBase
    {
        // 👇 ĐÃ SỬA: Dùng [Parameter] thay vì Constructor để tương thích với RenderFragment
        [Parameter]
        public RawHtmlComponentModel ComponentModel { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ComponentModel != null && !string.IsNullOrEmpty(ComponentModel.HtmlContent))
            {
                builder.OpenElement(0, "div");
                // Thêm CSS để bảng không bị vỡ và có thanh cuộn nếu quá rộng
                builder.AddAttribute(1, "style", "padding: 10px; overflow-x: auto; width: 100%;");

                // Render HTML thật
                builder.AddMarkupContent(2, ComponentModel.HtmlContent);

                builder.CloseElement();
            }
        }
    }
}
