using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Http;
using System.Globalization;
using System.Threading;

namespace Riyadh_Al_Salehin
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var culture = new CultureInfo("ar-SA");
            culture.DateTimeFormat.Calendar = new GregorianCalendar();
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            culture.DateTimeFormat.LongDatePattern = "yyyy/MM/dd HH:mm:ss";
            
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var culture = new CultureInfo("ar-SA");
            culture.DateTimeFormat.Calendar = new GregorianCalendar();
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            culture.DateTimeFormat.LongDatePattern = "yyyy/MM/dd HH:mm:ss";
            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}