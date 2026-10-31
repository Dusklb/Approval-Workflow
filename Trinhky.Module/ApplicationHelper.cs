using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.Linq;

namespace Trinhky.Module
{
    public class ApplicationHelper
    {
        private const string ValueManagerKey = "ApplicationHelper";
        private static volatile IValueManager<ApplicationHelper> instanceValueManager;
        private static readonly object syncRoot = new();
        private XafApplication _Application;
        private ApplicationHelper()
        {
        }
        public static ApplicationHelper Instance
        {
            get
            {
                try
                {
                    if (instanceValueManager == null)
                    {
                        lock (syncRoot)
                        {
                            instanceValueManager ??= ValueManager.GetValueManager<ApplicationHelper>(ValueManagerKey);
                        }
                    }
                    if (instanceValueManager.Value == null)
                    {
                        lock (syncRoot)
                        {
                            instanceValueManager.Value ??= new ApplicationHelper();
                        }
                    }
                    return instanceValueManager.Value;
                }
                catch (Exception)
                { 
                    return null;
                }
            }
        }
        public XafApplication Application { get { return _Application; } }
        internal void Initialize(XafApplication app)
        {
            _Application = app;
        }
    }
}
