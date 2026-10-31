using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Components;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Blazor.Components.Models;

namespace Trinhky.Blazor.Server.Pages
{
    public class ViewDXAdapter : IComponentAdapter, IComplexControl
    {
        private XafApplication application;
        private IObjectSpace objectSpace1;
        private RenderFragment component;
        public RenderFragment ComponentContent
        {
            get
            {
                if (component == null)
                {
                    component = builder => {
                        builder.OpenComponent<ViewDX>(0);
                        builder.AddAttribute(1, nameof(ViewDX.myApp), application);
                        builder.AddAttribute(2, nameof(ViewDX.myObjectSpace), objectSpace1);
                        builder.CloseComponent();
                    };
                }
                return component;
            }
        }

        public IComponentModel ComponentModel => throw new NotImplementedException();

        public void Refresh() { }
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application;
            objectSpace1 = objectSpace;
        }
        public object GetValue()
        {
            return null;
        }
        public void SetValue(object value) { }
        public event EventHandler ValueChanged;
    }
}
