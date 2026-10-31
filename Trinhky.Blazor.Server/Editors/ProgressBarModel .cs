using DevExpress.ExpressApp.Blazor.Components.Models;
using System;
using System.Linq;

namespace Trinhky.Blazor.Server.Editors
{
    public class ProgressBarModel : ComponentModelBase
    {
        public int Value
        {
            get => GetPropertyValue<int>();
            set => SetPropertyValue(value);
        }
    }
}
