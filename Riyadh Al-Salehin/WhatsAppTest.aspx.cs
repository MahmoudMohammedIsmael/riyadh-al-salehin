using System;
using System.Threading.Tasks;
using Riyadh_Al_Salehin.Services;


namespace Riyadh_Al_Salehin
{
    public partial class WhatsAppTest : System.Web.UI.Page
    {
        protected async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                WhatsAppService whatsapp =
                    new WhatsAppService();

                string result =
                    await whatsapp.SendTestTemplate("201500190533");

                lblResult.Text =
                    "<pre>" +
                    Server.HtmlEncode(result) +
                    "</pre>";
            }
            catch (Exception ex)
            {
                lblResult.Text =
                    "<pre>" +
                    Server.HtmlEncode(ex.ToString()) +
                    "</pre>";
            }
        }
    }
}