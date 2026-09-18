using System;
using System.Threading.Tasks;
using Riyadh_Al_Salehin.Services;

namespace Riyadh_Al_Salehin
{
    public partial class TestAI : System.Web.UI.Page
    {
        protected async void btnTest_Click(
    object sender,
    EventArgs e)
        {
            try
            {
                string message =
                    txtMessage.Text.Trim();

                if (string.IsNullOrWhiteSpace(message))
                {
                    lblResult.Text =
                        "من فضلك اكتب رسالة.";
                    return;
                }

                WhatsAppAIService ai =
                    new WhatsAppAIService();

                string result =
                    await ai.AskAsync(message);

                lblResult.Text =
                    "<pre>" +
                    Server.HtmlEncode(result) +
                    "</pre>";
            }
            catch (Exception ex)
            {
                lblResult.Text =
                    "<pre>" +
                    Server.HtmlEncode(
                        ex.ToString()) +
                    "</pre>";
            }
        }
    }
}