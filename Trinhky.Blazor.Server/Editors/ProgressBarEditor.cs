using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using System;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Editors
{
    [PropertyEditor(typeof(int), "ProgressProperty", false)]
    public class ProgressBarEditor : BlazorPropertyEditorBase
    {
        public ProgressBarEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override IComponentAdapter CreateComponentAdapter() => new ProgressBarAdapter(new ProgressBarModel());

        protected override RenderFragment CreateViewComponentCore(object dataContext)
        {
            ProgressBarModel componentModel = new();
            var cs = dataContext.GetType();
            if (cs.Name == "Phieudexuat")
            {
                var testProgressBusinessObject = (Phieudexuat)dataContext;
                componentModel.Value = testProgressBusinessObject.TyleDuyet;
            }
            else if (cs.Name == "Phieuduyet")
            {
                var testProgressBusinessObject = (Phieuduyet)dataContext;
                componentModel.Value = testProgressBusinessObject.TyleDuyet;
            }
            return ProgressBarRenderer.Create(componentModel);
        }
    }
}
