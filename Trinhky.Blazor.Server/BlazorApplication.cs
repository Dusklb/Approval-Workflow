using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using Microsoft.JSInterop;

namespace Trinhky.Blazor.Server;

public class TrinhkyCookieStorage : SettingsStorage
{
    IServiceProvider serviceProvider;
    IHttpContextAccessor httpCont;
    public TrinhkyCookieStorage(IServiceProvider _serviceProvider)
    {
        serviceProvider = _serviceProvider;
        jsRuntime = (IJSRuntime)serviceProvider.GetService(typeof(IJSRuntime));
        httpCont = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
    }
    IJSRuntime jsRuntime;
    public override string LoadOption(string optionPath, string optionName)
    {
        if (httpCont != null)
        {
            var val = httpCont.HttpContext.Request.Cookies[optionName];
            return val;
        }
        return null;
    }
    public override void SaveOption(string optionPath, string optionName, string optionValue)
    {
        Task.Run(async () => await jsRuntime.InvokeAsync<object>("blazorExtensions.WriteCookie", new object[] { optionName, optionValue, 30 }));
    }
}
public class TrinhkyBlazorApplication : BlazorApplication
{
    public TrinhkyBlazorApplication()
    {
        ApplicationName = "Trinhky";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += TrinhkyBlazorApplication_DatabaseVersionMismatch;
        this.CreateCustomLogonParameterStore += TrinhkyBlazorApplication_CreateCustomLogonParameterStore;
        this.LastLogonParametersReading += TrinhkyBlazorApplication_LastLogonParametersReading;
        this.LastLogonParametersWriting += TrinhkyBlazorApplication_LastLogonParametersWriting;
    }

    private void TrinhkyBlazorApplication_LastLogonParametersWriting(object sender, LastLogonParametersWritingEventArgs e)
    {
        var st = e.LogonObject as AuthenticationStandardLogonParameters;
        e.SettingsStorage.SaveOption("", "user", st.UserName);
    }

    private void TrinhkyBlazorApplication_LastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e)
    {
        try
        {
            e.Handled = true;
            string user = e.SettingsStorage.LoadOption("", "user");
            var st = e.LogonObject as AuthenticationStandardLogonParameters;
            st.UserName = user;
        }
        catch (Exception ex)
        {
            //throw new UserFriendlyException(ex.Message);
        }
    }

    private void TrinhkyBlazorApplication_CreateCustomLogonParameterStore(object sender, CreateCustomLogonParameterStoreEventArgs e)
    {
        e.Storage = new TrinhkyCookieStorage(this.ServiceProvider);
        e.Handled = true;
    }

    protected override void OnSetupStarted()
    {
        base.OnSetupStarted();
#if DEBUG
        if (System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
        {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
        }
#endif
    }
    private void TrinhkyBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
    {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if (System.Diagnostics.Debugger.IsAttached)
        {
            e.Updater.Update();
            e.Handled = true;
        }
        else
        {
            string message = "The application cannot connect to the specified database, " +
                "because the database doesn't exist, its version is older " +
                "than that of the application or its schema does not match " +
                "the ORM data model structure. To avoid this error, use one " +
                "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            if (e.CompatibilityError != null && e.CompatibilityError.Exception != null)
            {
                message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            }
            throw new InvalidOperationException(message);
        }
#endif
    }
}
