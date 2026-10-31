using DevExpress.ExpressApp.Blazor.Components;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace Trinhky.Blazor.Server.Editors
{
    public class ProgressBarAdapter(ProgressBarModel componentModel) : ComponentAdapterBase
    {
        private readonly ProgressBarModel componentModel = componentModel ?? throw new ArgumentNullException(nameof(componentModel));

        public ProgressBarModel GetComponentModel()
        {
            return componentModel;
        }

        public override IComponentModel ComponentModel => throw new NotImplementedException();

        public override void SetAllowEdit(bool allowEdit)
        {
        }
        public override object GetValue()
        {
            return GetComponentModel().Value;
        }
        public override void SetValue(object value)
        {
            GetComponentModel().Value = value is null ? default : (int)value;
        }
        protected override RenderFragment CreateComponent()
        {
            return ComponentModelObserver.Create(GetComponentModel(), ProgressBarRenderer.Create(GetComponentModel()));
        }
        public override void SetAllowNull(bool allowNull) { /* ...*/ }
        public override void SetDisplayFormat(string displayFormat) { /* ...*/ }
        public override void SetEditMask(string editMask) { /* ...*/ }
        public override void SetEditMaskType(EditMaskType editMaskType) { /* ...*/ }
        public override void SetErrorIcon(ImageInfo errorIcon) { /* ...*/ }
        public override void SetErrorMessage(string errorMessage) { /* ...*/ }
        public override void SetIsPassword(bool isPassword) { /* ...*/ }
        public override void SetMaxLength(int maxLength) { /* ...*/ }
        public override void SetNullText(string nullText) { /* ...*/ }
    }
}
