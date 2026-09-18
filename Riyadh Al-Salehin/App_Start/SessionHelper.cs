using System;
using System.Web;

namespace Riyadh_Al_Salehin
{
    public static class SessionHelper
    {
        public static int GetUserId()
        {
            HttpContext context = HttpContext.Current;

            if (context.Session["UserId"] != null)
                return Convert.ToInt32(context.Session["UserId"]);

            HttpCookie cookie = context.Request.Cookies["UserId"];
            if (cookie != null && int.TryParse(cookie.Value, out int userId))
            {
                context.Session["UserId"] = userId;
                return userId;
            }

            context.Session["ReturnUrl"] = context.Request.RawUrl;
            context.Response.Redirect("~/Login.aspx", false);
            context.ApplicationInstance.CompleteRequest();
            return 0;
        }

        public static void SetUserId(int userId)
        {
            HttpContext context = HttpContext.Current;
            context.Session["UserId"] = userId;

            HttpCookie cookie = new HttpCookie("UserId", userId.ToString());
            cookie.Expires = DateTime.Now.AddHours(2);
            cookie.HttpOnly = true;   // اختياري، لكن يفضل
            cookie.Secure = false;    // إذا كنت تستخدم HTTP
            cookie.SameSite = SameSiteMode.Lax; // للتوافق
            context.Response.Cookies.Add(cookie);
        }

        public static bool IsUserLoggedIn()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["UserId"] != null)
                return true;

            HttpCookie cookie = context.Request.Cookies["UserId"];
            if (cookie != null && int.TryParse(cookie.Value, out int userId))
            {
                context.Session["UserId"] = userId;
                return true;
            }
            return false;
        }









    }
}