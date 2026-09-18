using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Riyadh_Al_Salehin.Services
{
    public class WhatsAppService
    {
        private readonly string accessToken;
        private readonly string phoneNumberId;
        private readonly string graphVersion;

        public WhatsAppService()
        {
            accessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
            phoneNumberId = ConfigurationManager.AppSettings["WhatsAppPhoneNumberId"];
            graphVersion = ConfigurationManager.AppSettings["WhatsAppGraphVersion"];

            if (string.IsNullOrEmpty(graphVersion))
                graphVersion = "v25.0";
        }


        public async Task<string> SendTestTemplate(string phoneNumber)
        {
            string url =
                "https://graph.facebook.com/" +
                graphVersion +
                "/" +
                phoneNumberId +
                "/messages";

            var data = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber,
                type = "template",
                template = new
                {
                    name = "jaspers_market_order_confirmation_v1",
                    language = new
                    {
                        code = "en_US"
                    },
                    components = new object[]
                    {
                new
                {
                    type = "body",
                    parameters = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = "John Doe"
                        },
                        new
                        {
                            type = "text",
                            text = "123456"
                        },
                        new
                        {
                            type = "text",
                            text = "Aug 10, 2026"
                        }
                    }
                }
                    }
                }
            };

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            string json = serializer.Serialize(data);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        accessToken
                    );

                StringContent content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );

                HttpResponseMessage response =
                    await client.PostAsync(url, content);

                string result =
                    await response.Content.ReadAsStringAsync();

                return response.StatusCode + "\r\n" + result;
            }
        }






























    }
}