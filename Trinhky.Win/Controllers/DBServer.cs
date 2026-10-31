using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;
using Trinhky.Win.Editors;

namespace Trinhky.Win.Controllers
{
    public class DBServer : ViewController
    {
        public DBServer()
        {
            SimpleAction actionInPopup = new(this,
                "dbserver",
                DevExpress.Persistent.Base.PredefinedCategory.PopupActions
            )
            {
                Caption = "Máy chủ",
                TargetViewId = "AuthenticationStandardLogonParameters_DetailView"
            };
            //Dennis: Refer to the http://documentation.devexpress.com/#Xaf/CustomDocument2815 help article to see how to reorder Actions within the PopupActions container.
            actionInPopup.Execute += ActionInPopup_Execute;
        }
        void ActionInPopup_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            XtraForm form = new ServerForm();
            form.ShowDialog();
        }
    }
}
