using Riyadh_Al_Salehin.App_Start;
using Riyadh_Al_Salehin.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Script.Serialization;
using ZXing;

namespace Riyadh_Al_Salehin.Controllers
{
    [RoutePrefix("api/whatsapp")]
    public class WhatsAppController : ApiController
    {
        private const string VerifyToken = "RiyadhAPI2026";
        private static readonly WhatsAppAIService _aiService = new WhatsAppAIService();

        [HttpGet]
        [Route("webhook")]
        public HttpResponseMessage VerifyWebhook()
        {
            string hubMode = null;
            string hubVerifyToken = null;
            string hubChallenge = null;

            foreach (var item in Request.GetQueryNameValuePairs())
            {
                if (item.Key == "hub.mode")
                    hubMode = item.Value;
                if (item.Key == "hub.verify_token")
                    hubVerifyToken = item.Value;
                if (item.Key == "hub.challenge")
                    hubChallenge = item.Value;
            }

            if (hubMode == "subscribe" && hubVerifyToken == VerifyToken)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(hubChallenge ?? "", Encoding.UTF8, "text/plain")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid verify token", Encoding.UTF8, "text/plain")
            };
        }


        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> _processedMessageIds
    = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();
        [HttpPost]
        [Route("webhook")]
        public async Task<IHttpActionResult> ReceiveWebhook()
        {
            try
            {
                WriteLog("========== START POST ==========");
                string body = await Request.Content.ReadAsStringAsync();
                WriteLog("BODY RECEIVED:");
                WriteLog(body);

                if (string.IsNullOrWhiteSpace(body))
                {
                    WriteLog("BODY EMPTY");
                    return Ok();
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.DeserializeObject(body) as Dictionary<string, object>;

                if (data == null)
                {
                    WriteLog("ERROR: JSON DATA NULL");
                    return Ok();
                }

                if (!data.ContainsKey("entry"))
                {
                    WriteLog("ENTRY NOT FOUND");
                    return Ok();
                }

                object[] entries = data["entry"] as object[];
                if (entries == null)
                {
                    WriteLog("ENTRIES NULL");
                    return Ok();
                }

                foreach (object entryObject in entries)
                {
                    var entry = entryObject as Dictionary<string, object>;
                    if (entry == null) continue;
                    if (!entry.ContainsKey("changes")) continue;

                    object[] changes = entry["changes"] as object[];
                    if (changes == null) continue;

                    foreach (object changeObject in changes)
                    {
                        var change = changeObject as Dictionary<string, object>;
                        if (change == null) continue;

                        string field = change.ContainsKey("field") && change["field"] != null ? change["field"].ToString() : "";
                        WriteLog("FIELD = " + field);

                        if (field != "messages")
                        {
                            WriteLog("STATUS / OTHER EVENT - IGNORED");
                            continue;
                        }

                        if (!change.ContainsKey("value")) continue;
                        var value = change["value"] as Dictionary<string, object>;
                        if (value == null) continue;

                        if (!value.ContainsKey("messages"))
                        {
                            WriteLog("NO MESSAGES - STATUS EVENT");
                            continue;
                        }

                        object[] messages = value["messages"] as object[];
                        if (messages == null) continue;

                        foreach (object messageObject in messages)
                        {
                            var message = messageObject as Dictionary<string, object>;
                            if (message == null) continue;

                            string fromPhone = "";
                            if (message.ContainsKey("from") && message["from"] != null)
                                fromPhone = message["from"].ToString().Trim();

                            if (string.IsNullOrWhiteSpace(fromPhone) && value.ContainsKey("contacts") && value["contacts"] != null)
                            {
                                object[] contacts = value["contacts"] as object[];
                                if (contacts != null && contacts.Length > 0)
                                {
                                    Dictionary<string, object> contact = contacts[0] as Dictionary<string, object>;
                                    if (contact != null && contact.ContainsKey("wa_id") && contact["wa_id"] != null)
                                        fromPhone = contact["wa_id"].ToString().Trim();
                                }
                            }

                            if (string.IsNullOrWhiteSpace(fromPhone))
                            {
                                WriteLog("ERROR: NO WHATSAPP PHONE NUMBER FOUND");
                                continue;
                            }

                            WriteLog("FINAL CUSTOMER PHONE = " + fromPhone);

                            string messageType = message.ContainsKey("type") && message["type"] != null ? message["type"].ToString() : "";
                            WriteLog("MESSAGE TYPE = " + messageType);



                            string messageId = message.ContainsKey("id") && message["id"] != null ? message["id"].ToString() : "";

                            if (!string.IsNullOrWhiteSpace(messageId))
                            {
                                if (_processedMessageIds.ContainsKey(messageId))
                                {
                                    WriteLog("DUPLICATE MESSAGE IGNORED: " + messageId);
                                    continue; // تجاهل الرسالة المكررة
                                }
                                _processedMessageIds[messageId] = DateTime.Now;

                                if (_processedMessageIds.Count > 5000)
                                {
                                    DateTime cutoff = DateTime.Now.AddHours(-6);
                                    foreach (var oldKey in _processedMessageIds
                                                 .Where(kv => kv.Value < cutoff)
                                                 .Select(kv => kv.Key).ToList())
                                    {
                                        _processedMessageIds.TryRemove(oldKey, out _);
                                    }
                                }
                            }








                            if (messageType == "text")
                            {
                                string messageText = "";
                                if (message.ContainsKey("text") && message["text"] != null)
                                {
                                    var textObject = message["text"] as Dictionary<string, object>;
                                    if (textObject != null && textObject.ContainsKey("body") && textObject["body"] != null)
                                        messageText = textObject["body"].ToString();
                                }

                                messageText = (messageText ?? "").Trim();
                                WriteLog("MESSAGE TEXT = " + messageText);

                                if (string.IsNullOrWhiteSpace(messageText))
                                    continue;

                                try
                                {
                                    WriteLog("========== PROCESS TEXT START ==========");
                                    string reply = await ProcessWhatsAppMessage(fromPhone, messageText);
                                    WriteLog("REPLY:");
                                    WriteLog(reply);

                                    if (!string.IsNullOrWhiteSpace(reply))
                                    {
                                        WriteLog("========== SEND AI REPLY ==========");
                                        await SendWhatsAppMessageAsync(fromPhone, reply);
                                    }
                                    WriteLog("========== PROCESS TEXT END ==========");
                                }
                                catch (Exception ex)
                                {
                                    WriteLog("PROCESS TEXT ERROR:");
                                    WriteLog(ex.ToString());
                                    try
                                    {
                                        await SendWhatsAppMessageAsync(fromPhone, FormatError("حدث خطأ أثناء معالجة رسالتك.\r\nيرجى المحاولة مرة أخرى."));
                                    }
                                    catch (Exception sendEx)
                                    {
                                        WriteLog("ERROR SENDING ERROR MESSAGE:");
                                        WriteLog(sendEx.ToString());
                                    }
                                }

                                continue;
                            }

                            if (messageType == "audio")
                            {
                                var audioObject = message.ContainsKey("audio") ? message["audio"] as Dictionary<string, object> : null;
                                string mediaId = "";

                                if (audioObject != null && audioObject.ContainsKey("id") && audioObject["id"] != null)
                                    mediaId = audioObject["id"].ToString();

                                if (string.IsNullOrWhiteSpace(mediaId))
                                {
                                    await SendWhatsAppMessageAsync(fromPhone, "لم أستطع قراءة التسجيل الصوتي.");
                                    continue;
                                }

                                try
                                {
                                    WriteLog("========== VOICE PROCESS START ==========");
                                    string speechText = await DownloadAndTranscribeWithGeminiAsync(mediaId);
                                    WriteLog("VOICE TRANSCRIPT = " + speechText);

                                    if (string.IsNullOrWhiteSpace(speechText))
                                    {
                                        await SendWhatsAppMessageAsync(fromPhone, "عذرًا، لم أستطع فهم التسجيل الصوتي. حاول مرة أخرى.");
                                        continue;
                                    }

                                    string reply = await ProcessWhatsAppMessage(fromPhone, speechText);
                                    WriteLog("VOICE AI REPLY = " + reply);

                                    if (!string.IsNullOrWhiteSpace(reply))
                                    {
                                        await SendWhatsAppMessageAsync(fromPhone, reply);
                                    }
                                    WriteLog("========== VOICE PROCESS END ==========");
                                }
                                catch (Exception ex)
                                {
                                    WriteLog("VOICE PROCESS ERROR:");
                                    WriteLog(ex.ToString());
                                    try
                                    {
                                        await SendWhatsAppMessageAsync(fromPhone, FormatError("حدث خطأ أثناء معالجة التسجيل الصوتي."));
                                    }
                                    catch (Exception sendEx)
                                    {
                                        WriteLog("VOICE ERROR SEND FAILED:");
                                        WriteLog(sendEx.ToString());
                                    }
                                }

                                continue;
                            }

                            WriteLog("UNSUPPORTED MESSAGE TYPE: " + messageType);
                            await SendWhatsAppMessageAsync(fromPhone, "عذرًا، أستطيع استقبال الرسائل النصية والتسجيلات الصوتية فقط حاليًا.");
                        }
                    }
                }

                WriteLog("========== POST FINISHED - 200 OK ==========");
                return Ok();
            }
            catch (Exception ex)
            {
                WriteLog("========== WEBHOOK ERROR ==========");
                WriteLog(ex.ToString());
                return Ok();
            }
        }

        private async Task<string> DownloadAndTranscribeWithGeminiAsync(string mediaId)
        {
            try
            {
                string accessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
                string graphVersion = ConfigurationManager.AppSettings["WhatsAppGraphVersion"] ?? "v25.0";

                string downloadUrl = $"https://graph.facebook.com/{graphVersion}/{mediaId}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.GetAsync(downloadUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        WriteLog("AUDIO DOWNLOAD ERROR: " + error);
                        return "";
                    }

                    byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                    string mimeType = "audio/ogg";

                    string transcript = await _aiService.TranscribeAudioAsync(audioBytes, mimeType);
                    return transcript;
                }
            }
            catch (Exception ex)
            {
                WriteLog("AUDIO TRANSCRIPTION ERROR (Gemini):");
                WriteLog(ex.ToString());
                return "";
            }
        }

        #region Response Formatting

        private string FormatMainMenu(string greeting = "مرحبًا بك 👋")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🏥 *مستشفى الرياض الصالحين*");
            sb.AppendLine();
            sb.AppendLine(greeting);
            sb.AppendLine("كيف يمكنني مساعدتك؟");
            sb.AppendLine();
            sb.AppendLine("1️⃣ حجز موعد");
            sb.AppendLine("2️⃣ مواعيدي");
            sb.AppendLine("3️⃣ الأطباء");
            sb.AppendLine("4️⃣ الاستفسارات");
            sb.AppendLine();
            sb.AppendLine("يمكنك إرسال طلبك مباشرة، مثل:");
            sb.AppendLine("«أريد حجز موعد عند طبيب أسنان»");
            return sb.ToString().TrimEnd();
        }

        private string ToEmojiNumber(int n)
        {
            switch (n)
            {
                case 1: return "1️⃣";
                case 2: return "2️⃣";
                case 3: return "3️⃣";
                case 4: return "4️⃣";
                case 5: return "5️⃣";
                case 6: return "6️⃣";
                case 7: return "7️⃣";
                case 8: return "8️⃣";
                case 9: return "9️⃣";
                case 10: return "🔟";
                default: return n.ToString() + ".";
            }
        }

        private string FormatDoctorsList(DataTable dt, WhatsAppBookingSession session, string specialty = null)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                string specText = !string.IsNullOrWhiteSpace(specialty) ? specialty :
                    (session != null ? session.PendingAISpecialty : null);

                if (!string.IsNullOrWhiteSpace(specText))
                {
                    return "😕 لم أجد أطباء في تخصص «" + specText + "».\r\n\r\n" +
                           "يمكنك اختيار تخصص آخر أو إرسال «الأطباء» لعرض جميع الأطباء.";
                }

                return "😕 عذرًا، لا يوجد أطباء متاحون حاليًا.";
            }

            StringBuilder sb = new StringBuilder();

            string headerSpecialty = !string.IsNullOrWhiteSpace(specialty) ? specialty :
                (session != null ? session.PendingAISpecialty : null);

            if (!string.IsNullOrWhiteSpace(headerSpecialty))
                sb.AppendLine("👨‍⚕️ الأطباء في تخصص " + headerSpecialty);
            else
                sb.AppendLine("👨‍⚕️ الأطباء المتاحون");

            sb.AppendLine();

            int number = 1;
            foreach (DataRow row in dt.Rows)
            {
                string name = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString();
                string spec = row["Specialty"] == DBNull.Value ? "" : row["Specialty"].ToString();

                sb.Append(ToEmojiNumber(number) + " د. " + name);
                sb.AppendLine();
                if (!string.IsNullOrWhiteSpace(spec))
                    sb.AppendLine("🏥 التخصص: " + spec);
                if (dt.Columns.Contains("AvailableSchedules"))
                {
                    int availableCount = row["AvailableSchedules"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["AvailableSchedules"]);
                    sb.AppendLine("📅 المواعيد المتاحة: " + availableCount);
                }
                sb.AppendLine();
                number++;
            }

            sb.AppendLine("أرسل رقم الطبيب أو اكتب اسمه.");
            sb.AppendLine("أرسل «إلغاء» لإلغاء العملية.");
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// عرض قائمة الأطباء مع عدد المواعيد المتاحة
        /// </summary>
        private string FormatDoctorsListWithAvailability(DataTable dt, WhatsAppBookingSession session, string specialty = null)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                string specText = !string.IsNullOrWhiteSpace(specialty) ? specialty :
                    (session != null ? session.PendingAISpecialty : null);

                if (!string.IsNullOrWhiteSpace(specText))
                {
                    return "😔 للأسف، لم أجد أطباء متاحين في تخصص «" + specText + "».\r\n\r\n" +
                           "يمكنك اختيار تخصص آخر أو إرسال «الأطباء» لعرض جميع الأطباء المتاحين.";
                }

                return "😔 عذراً، لا يوجد أطباء متاحين حالياً. حاول لاحقاً أو تواصل معنا عبر الهاتف.";
            }

            StringBuilder sb = new StringBuilder();

            string headerSpecialty = !string.IsNullOrWhiteSpace(specialty) ? specialty :
                (session != null ? session.PendingAISpecialty : null);

            if (!string.IsNullOrWhiteSpace(headerSpecialty))
                sb.AppendLine("👨‍⚕️ الأطباء المتاحون في تخصص " + headerSpecialty);
            else
                sb.AppendLine("👨‍⚕️ الأطباء المتاحون");

            sb.AppendLine();

            int number = 1;
            foreach (DataRow row in dt.Rows)
            {
                string name = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString();
                string spec = row["Specialty"] == DBNull.Value ? "" : row["Specialty"].ToString();
                int availableCount = row["AvailableSchedules"] == DBNull.Value ? 0 : Convert.ToInt32(row["AvailableSchedules"]);

                sb.Append(ToEmojiNumber(number) + " د. " + name);
                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(spec))
                    sb.AppendLine("🏥 " + spec);

                if (availableCount > 0)
                {
                    sb.AppendLine("📅 " + availableCount + " موعد متاح" + (availableCount > 2 ? "ة" : ""));
                }

                sb.AppendLine();
                number++;
            }

            sb.AppendLine("أرسل رقم الطبيب أو اكتب اسمه.");
            sb.AppendLine("أرسل «إلغاء» لإلغاء العملية.");
            return sb.ToString().TrimEnd();
        }

        private string FormatSchedulesList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return "😕 لا توجد مواعيد متاحة حاليًا لهذا الطبيب.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🗓️ المواعيد المتاحة (أقرب 7 مواعيد)");
            sb.AppendLine();

            int option = 1;
            int maxShown = Math.Min(dt.Rows.Count, 7);
            for (int i = 0; i < maxShown; i++)
            {
                DataRow row = dt.Rows[i];
                DateTime start = Convert.ToDateTime(row["StartTime"]);
                DateTime end = Convert.ToDateTime(row["EndTime"]);

                sb.AppendLine(ToEmojiNumber(option) + " " + GetArabicDayName(start.DayOfWeek) + " " + start.ToString("dd/MM"));
                sb.AppendLine("⏰ " + start.ToString("HH:mm") + " - " + end.ToString("HH:mm"));
                sb.AppendLine();
                option++;
            }

            sb.AppendLine("أرسل رقم الموعد لاختياره.");
            sb.AppendLine("أرسل «إلغاء» لإلغاء العملية.");
            return sb.ToString().TrimEnd();
        }

        private string GetArabicDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Saturday: return "السبت";
                case DayOfWeek.Sunday: return "الأحد";
                case DayOfWeek.Monday: return "الاثنين";
                case DayOfWeek.Tuesday: return "الثلاثاء";
                case DayOfWeek.Wednesday: return "الأربعاء";
                case DayOfWeek.Thursday: return "الخميس";
                case DayOfWeek.Friday: return "الجمعة";
                default: return "";
            }
        }

        private string FormatBookingConfirmation(string patientName, string doctorName, string serviceName,
            DateTime start, DateTime end, decimal amount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📋 *تأكيد الحجز*");
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(patientName))
                sb.AppendLine("👤 المريض: " + patientName);
            sb.AppendLine("👨‍⚕️ الطبيب: " + doctorName);
            if (!string.IsNullOrWhiteSpace(serviceName))
                sb.AppendLine("🩺 الخدمة: " + serviceName);
            sb.AppendLine("🗓️ التاريخ: " + GetArabicDayName(start.DayOfWeek) + " " + start.ToString("dd/MM/yyyy"));
            sb.AppendLine("⏰ الوقت: " + start.ToString("HH:mm") + " - " + end.ToString("HH:mm"));
            sb.AppendLine("💰 السعر: " + amount.ToString("0.00") + " ج.م");
            sb.AppendLine();
            sb.AppendLine("هل تريد تأكيد الحجز؟");
            sb.AppendLine();
            sb.AppendLine("1️⃣ نعم ✅");
            sb.AppendLine("2️⃣ لا ❌");
            return sb.ToString().TrimEnd();
        }

        private string FormatBookingSuccess(string doctorName, DateTime appointmentDate, string queueNumber,
            int invoiceId, decimal totalAmount, bool invoiceImageSent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("✅ *تم تأكيد الحجز بنجاح*");
            sb.AppendLine();
            sb.AppendLine("🏥 مستشفى الرياض الصالحين");
            sb.AppendLine();
            sb.AppendLine("👨‍⚕️ الطبيب: " + doctorName);
            sb.AppendLine("🗓️ التاريخ: " + GetArabicDayName(appointmentDate.DayOfWeek) + " " + appointmentDate.ToString("dd/MM/yyyy"));
            sb.AppendLine("⏰ الوقت: " + appointmentDate.ToString("HH:mm"));
            sb.AppendLine("🎫 رقم الانتظار: " + queueNumber);
            sb.AppendLine("🧾 رقم الفاتورة: " + invoiceId);
            sb.AppendLine("💰 الإجمالي: " + totalAmount.ToString("0.00") + " ج.م");
            sb.AppendLine();

            if (invoiceImageSent)
                sb.AppendLine("📎 تم إرسال الفاتورة في صورة منفصلة.");

            sb.AppendLine();
            sb.AppendLine("نتمنى لكم الشفاء العاجل 🌷");
            return sb.ToString().TrimEnd();
        }

        private string FormatMyAppointments(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return "لا توجد لديك مواعيد مسجلة حاليًا.\r\n\r\nيمكنك حجز موعد جديد بإرسال «حجز موعد».";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📅 *مواعيدك الحالية*");
            sb.AppendLine();

            int number = 1;
            foreach (DataRow row in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(row["AppointmentDate"]);
                string status = row["Status"] == DBNull.Value ? "" : row["Status"].ToString();

                sb.AppendLine(ToEmojiNumber(number));
                sb.AppendLine("👨‍⚕️ د. " + row["DoctorName"]);
                sb.AppendLine("🗓️ " + date.ToString("dd/MM/yyyy"));
                sb.AppendLine("⏰ " + date.ToString("HH:mm"));
                sb.AppendLine("📌 الحالة: " + TranslateStatus(status));
                sb.AppendLine();
                number++;
            }

            sb.AppendLine("يمكنك إرسال:");
            sb.AppendLine("«إلغاء موعد» أو «تغيير الموعد»");
            return sb.ToString().TrimEnd();
        }

        private string TranslateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return "غير محدد";
            string s = status.Trim().ToLower();
            if (s == "confirmed") return "مؤكد";
            if (s == "pending") return "قيد الانتظار";
            if (s == "cancelled" || s == "canceled") return "ملغى";
            if (s == "completed") return "مكتمل";
            return status;
        }

        private string FormatError(string details)
        {
            if (string.IsNullOrWhiteSpace(details))
                return "⚠️ حدث خطأ غير متوقع.\r\n\r\nيرجى المحاولة مرة أخرى.";
            return "⚠️ " + details;
        }

        private string FormatUnavailableFeature(string suggestion = null)
        {
            string msg = "🚧 هذه الوظيفة غير مفعلة حاليًا.";
            if (!string.IsNullOrWhiteSpace(suggestion))
                msg += "\r\n\r\n" + suggestion;
            return msg;
        }

        #endregion

        #region Natural Language Helpers

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var normalized = text.Normalize(NormalizationForm.FormKD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private string NormalizeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            string result = RemoveDiacritics(text.Trim());
            result = result.Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا")
                            .Replace("ة", "ه").Replace("ى", "ي").Replace("ؤ", "و").Replace("ئ", "ي");
            result = Regex.Replace(result, "[ًٌٍَُِّْ]", "");
            result = Regex.Replace(result, @"\s+", " ");
            return result.Trim().ToLower();
        }

        private string NormalizeArabicDigits(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            char[] arabicDigits = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
            char[] persianDigits = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };

            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                int index = Array.IndexOf(arabicDigits, c);
                if (index >= 0)
                    sb[i] = (char)('0' + index);
                else
                {
                    index = Array.IndexOf(persianDigits, c);
                    if (index >= 0)
                        sb[i] = (char)('0' + index);
                }
            }
            return sb.ToString();
        }

        private string DetectGlobalIntent(string normalizedText)
        {
            if (string.IsNullOrWhiteSpace(normalizedText))
                return null;

            string t = normalizedText;

            string[] cancelWords = { "الغاء", "cancel", "الغ الحجز", "الغاء الحجز", "لا اريد", "لا أريد" };
            foreach (string w in cancelWords)
            {
                if (t == NormalizeArabic(w) || t.Contains(NormalizeArabic(w)))
                    return "CANCEL";
            }

            string[] queueWords = { "رقم الانتظار", "دوري", "متبقي كام", "باقي كام", "كام قدامي", "دوري كام" };
            foreach (string w in queueWords)
            {
                if (t.Contains(NormalizeArabic(w)))
                    return "QUEUE";
            }



            string[] myAppointmentsWords = { "مواعيدي", "حجوزاتي", "عندي مواعيد ايه", "عايز اشوف حجوزاتي", "ما هي مواعيدي" };
            foreach (string w in myAppointmentsWords)
            {
                if (t.Contains(NormalizeArabic(w)))
                    return "MYAPPOINTMENTS";
            }

            string[] doctorsListWords = { "الاطباء", "الدكاتره", "مين الدكاتره الموجودين" };
            foreach (string w in doctorsListWords)
            {
                if (t == NormalizeArabic(w) || t.Contains(NormalizeArabic(w)))
                    return "DOCTORS";
            }

            string[] salamWords = { "السلام عليكم", "السلام عليكم ورحمة الله", "سلام عليكم" };
            foreach (string w in salamWords)
            {
                if (t == NormalizeArabic(w) || t.Contains(NormalizeArabic(w)))
                    return "SALAM";
            }

            string[] menuWords = { "مرحبا", "اهلا", "هاي", "hi", "hello",
                "القائمه", "القائمه الرئيسيه", "رجوع للقائمه", "عايز ارجع للقائمه", "ارجع للقائمه" };
            foreach (string w in menuWords)
            {
                if (t == NormalizeArabic(w) || t.Contains(NormalizeArabic(w)))
                    return "MENU";
            }

            return null;
        }

        private bool IsAffirmative(string normalizedText)
        {
            if (string.IsNullOrWhiteSpace(normalizedText)) return false;
            string[] words = { "نعم", "تاكيد", "اكد", "ايوه", "موافق", "yes", "ok", "اوك" };
            foreach (string w in words)
            {
                if (normalizedText == NormalizeArabic(w))
                    return true;
            }
            return false;
        }

        private bool IsNegative(string normalizedText)
        {
            if (string.IsNullOrWhiteSpace(normalizedText)) return false;
            string[] words = { "لا", "لأ", "لا اريد", "الغاء", "no" };
            foreach (string w in words)
            {
                if (normalizedText == NormalizeArabic(w))
                    return true;
            }
            return false;
        }

        private static readonly Dictionary<string, int> ArabicMonths = new Dictionary<string, int>
        {
            { "يناير", 1 }, { "فبراير", 2 }, { "مارس", 3 }, { "ابريل", 4 }, { "أبريل", 4 },
            { "مايو", 5 }, { "يونيو", 6 }, { "يوليو", 7 }, { "اغسطس", 8 }, { "أغسطس", 8 },
            { "سبتمبر", 9 }, { "اكتوبر", 10 }, { "أكتوبر", 10 }, { "نوفمبر", 11 }, { "ديسمبر", 12 }
        };

        private DateTime? ParseArabicDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            string raw = text.Trim();
            string t = NormalizeArabic(raw);
            DateTime today = DateTime.Now.Date;

            if (t == "اليوم") return today;
            if (t == "بكره" || t == "غدا" || t == "غدًا") return today.AddDays(1);
            if (t == "بعد بكره" || t == "بعد غد") return today.AddDays(2);

            DateTime parsed;
            if (DateTime.TryParseExact(raw, new[] { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "dd/MM", "d/M" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                if (parsed.Year == 1)
                    parsed = new DateTime(today.Year, parsed.Month, parsed.Day);
                if (parsed < today)
                    parsed = parsed.AddYears(1);
                return parsed;
            }

            if (DateTime.TryParse(raw, out parsed))
                return parsed;

            Dictionary<string, DayOfWeek> dayNames = new Dictionary<string, DayOfWeek>
            {
                { "السبت", DayOfWeek.Saturday }, { "الاحد", DayOfWeek.Sunday },
                { "الاثنين", DayOfWeek.Monday }, { "الثلاثاء", DayOfWeek.Tuesday },
                { "الاربعاء", DayOfWeek.Wednesday }, { "الخميس", DayOfWeek.Thursday },
                { "الجمعه", DayOfWeek.Friday }
            };

            foreach (var kv in dayNames)
            {
                if (t.Contains(kv.Key))
                {
                    DateTime candidate = today;
                    for (int i = 0; i < 8; i++)
                    {
                        if (candidate.DayOfWeek == kv.Value && candidate >= today)
                        {
                            if (candidate == today) candidate = candidate.AddDays(7);
                            return candidate;
                        }
                        candidate = candidate.AddDays(1);
                    }
                }
            }

            Match m = Regex.Match(raw, @"(\d{1,2})\s+([\u0600-\u06FF]+)(\s+(\d{4}))?");
            if (m.Success)
            {
                int day = int.Parse(m.Groups[1].Value);
                string monthWord = NormalizeArabic(m.Groups[2].Value);
                int year = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : today.Year;

                foreach (var kv in ArabicMonths)
                {
                    if (NormalizeArabic(kv.Key) == monthWord)
                    {
                        try
                        {
                            DateTime candidate = new DateTime(year, kv.Value, day);
                            if (candidate < today) candidate = candidate.AddYears(1);
                            return candidate;
                        }
                        catch { return null; }
                    }
                }
            }

            return null;
        }

        private bool TryParseSelectionNumber(string text, out int number)
        {
            number = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            string cleaned = text.Trim();
            cleaned = Regex.Replace(cleaned, @"^(رقم|اختار رقم|اخر رقم)\s*", "", RegexOptions.IgnoreCase).Trim();
            cleaned = NormalizeArabicDigits(cleaned);

            return int.TryParse(cleaned, out number);
        }

        #endregion


        private string GetWhatsAppDoctors()
        {
            CDoctors doctors = new CDoctors();
            DataTable dt = doctors.GetClinicDoctorsWithAvailableSchedules();
            return FormatDoctorsList(dt, null);
        }

        private DataTable GetDoctorsTableForSession(WhatsAppBookingSession session)
        {
            DataTable dt = new CDoctors().GetClinicDoctorsWithAvailableSchedules();

            if (dt == null || string.IsNullOrWhiteSpace(session?.PendingAISpecialty))
                return dt;

            string specialty = RemoveDiacritics(session.PendingAISpecialty.Trim());

            DataTable filtered = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                string spec = row["Specialty"] == DBNull.Value ? "" : row["Specialty"].ToString().Trim();
                string specNormalized = RemoveDiacritics(spec);

                if (specNormalized.IndexOf(specialty, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    specialty.IndexOf(specNormalized, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filtered.ImportRow(row);
                }
            }

            return filtered;
        }

        private string SelectWhatsAppBookingTarget(string fromPhone, string text, WhatsAppBookingSession session)
        {
            string input = NormalizeArabicDigits(text.Trim());
            string normalized = NormalizeArabic(input);

            if (input == "1" || normalized == "لي" || normalized == "لي انا" || normalized == "انا")
            {
                session.IsBookingForOther = false;
                session.RegPhone = fromPhone;

                int existingPatientId = GetPatientIdByWhatsAppPhone(fromPhone);
                if (existingPatientId > 0)
                {
                    session.PatientId = existingPatientId;
                    session.Step = 1;
                    return FormatDoctorsList(GetDoctorsTableForSession(session), session);
                }

                session.RegistrationStep = 1;
                return "يبدو إنك تحجز معنا لأول مرة 👋\r\n\r\nمن فضلك أدخل اسمك الكامل لإتمام تسجيلك:";
            }

            if (input == "2" || normalized == "لشخص اخر" || normalized == "شخص اخر" || normalized == "اخر")
            {
                session.IsBookingForOther = true;
                session.RegistrationStep = 5;
                return "من فضلك أدخل رقم هاتف الشخص المطلوب الحجز له\r\n(مثال: 201001234567):";
            }

            return "من فضلك اختر:\r\n\r\n1️⃣ لي أنا\r\n2️⃣ لشخص آخر";
        }

        private string HandleRegistrationInput(string fromPhone, string messageText, WhatsAppBookingSession session)
        {
            string text = (messageText ?? "").Trim();

            if (session.RegistrationStep == 5)
            {
                string phoneInput = text.Trim();
                if (string.IsNullOrWhiteSpace(phoneInput) || phoneInput.Length < 8)
                {
                    return "رقم الهاتف غير صحيح ❌\r\n\r\nمن فضلك أدخل رقم هاتف صحيح:";
                }

                session.RegPhone = phoneInput;

                int existingPatientId = GetPatientIdByWhatsAppPhone(phoneInput);
                if (existingPatientId > 0)
                {
                    session.PatientId = existingPatientId;
                    session.RegistrationStep = 0;
                    session.Step = 1;
                    return "تم العثور على بيانات مسجلة لهذا الرقم ✅\r\n\r\n" + FormatDoctorsList(GetDoctorsTableForSession(session), session);
                }

                session.RegistrationStep = 1;
                return "من فضلك أدخل اسم الشخص المطلوب الحجز له بالكامل:";
            }

            if (session.RegistrationStep == 1)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return "من فضلك أدخل الاسم الكامل.";

                session.RegName = text;
                session.RegistrationStep = 2;
                return "تم استلام الاسم ✅\r\n\r\nمن فضلك أدخل العنوان:";
            }

            if (session.RegistrationStep == 2)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return "من فضلك أدخل العنوان.";

                session.RegAddress = text;
                session.RegistrationStep = 3;
                return "من فضلك أدخل تاريخ الميلاد بصيغة:\r\n\r\nYYYY-MM-DD\r\n\r\nمثال: 1990-05-20";
            }

            if (session.RegistrationStep == 3)
            {
                string rawDate = NormalizeArabicDigits(text.Trim());
                DateTime dateOfBirth;
                
                string[] formats = { 
                    "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", 
                    "yyyy/MM/dd", "dd.MM.yyyy", "d.M.yyyy", "yyyy/M/d", "yyyy-M-d",
                    "dd MMMM yyyy"
                };

                if (!DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
                {
                    if (!DateTime.TryParse(rawDate, out dateOfBirth))
                    {
                        return "تاريخ الميلاد غير صحيح ❌\r\n\r\nأدخل التاريخ بهذا الشكل:\r\n1990-05-20 أو 20/05/1990";
                    }
                }

                session.RegDateOfBirth = dateOfBirth.ToString("yyyy-MM-dd");

                try
                {
                    CPatients patient = new CPatients();
                    patient.PatientName = session.RegName;
                    patient.Phone = session.RegPhone ?? fromPhone;
                    patient.Address = session.RegAddress;
                    patient.DateOfBirth = dateOfBirth;
                    patient.InsuranceCompany = null;
                    patient.InsuranceNumber = null;

                    int patientId = patient.InsertAndReturnId();
                    if (patientId <= 0)
                    {
                        WriteLog("ERROR: Patient Insert returned ID <= 0");
                        return FormatError("حدث خطأ أثناء إنشاء ملف المريض.\r\nيرجى المحاولة مرة أخرى.");
                    }

                    session.PatientId = patientId;
                    session.RegistrationStep = 0;
                    session.Step = 1;

                    WriteLog("NEW PATIENT CREATED - ID = " + patientId.ToString());
                    WriteLog("PATIENT NAME = " + session.RegName);
                    WriteLog("PATIENT PHONE = " + patient.Phone);

                    return "تم إنشاء ملف المريض بنجاح ✅\r\n\r\n" +
                           "👤 الاسم: " + session.RegName + "\r\n" +
                           "🏠 العنوان: " + session.RegAddress + "\r\n" +
                           "🎂 تاريخ الميلاد: " + dateOfBirth.ToString("dd/MM/yyyy") + "\r\n\r\n" +
                           FormatDoctorsList(GetDoctorsTableForSession(session), session);
                }
                catch (Exception ex)
                {
                    WriteLog("PATIENT CREATE ERROR:");
                    WriteLog(ex.ToString());
                    return FormatError("حدث خطأ أثناء إنشاء ملف المريض.\r\nيرجى المحاولة مرة أخرى.");
                }
            }

            return "حدث خطأ في خطوات تسجيل المريض.\r\n\r\nأرسل «إلغاء» ثم أرسل «مرحبا» للبدء من جديد.";
        }


        private void MergeAIWithSession(AIIntentResult ai, WhatsAppBookingSession session)
        {
            if (!string.IsNullOrWhiteSpace(ai.Intent) && ai.Intent != "Unknown")
            {
                session.CurrentIntent = ai.Intent;
                WriteLog("MERGED INTENT = " + session.CurrentIntent);
            }
            else if (!string.IsNullOrWhiteSpace(session.CurrentIntent))
            {
                WriteLog("MERGED INTENT (kept from session) = " + session.CurrentIntent);
            }

            if (!string.IsNullOrWhiteSpace(ai.Specialty))
            {
                session.PendingAISpecialty = ai.Specialty;
                WriteLog("MERGED SPECIALTY = " + session.PendingAISpecialty);
            }
            else
            {
                WriteLog("MERGED SPECIALTY (kept from session) = " + (session.PendingAISpecialty ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.DoctorName))
            {
                session.PendingAIDoctorName = ai.DoctorName;
                WriteLog("MERGED DOCTOR NAME = " + session.PendingAIDoctorName);
            }
            else
            {
                WriteLog("MERGED DOCTOR NAME (kept from session) = " + (session.PendingAIDoctorName ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.Date))
            {
                DateTime? parsed = ParseArabicDate(ai.Date);
                session.PendingAIDate = parsed.HasValue ? parsed.Value.ToString("yyyy-MM-dd") : ai.Date;
                WriteLog("MERGED DATE = " + session.PendingAIDate);
            }
            else
            {
                WriteLog("MERGED DATE (kept from session) = " + (session.PendingAIDate ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.Time))
            {
                session.PendingAITime = ai.Time;
                WriteLog("MERGED TIME = " + session.PendingAITime);
            }
            else
            {
                WriteLog("MERGED TIME (kept from session) = " + (session.PendingAITime ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.ServiceName))
            {
                session.ServiceName = ai.ServiceName;
                WriteLog("MERGED SERVICE NAME = " + session.ServiceName);
            }
            else
            {
                WriteLog("MERGED SERVICE NAME (kept from session) = " + (session.ServiceName ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.VisitType))
            {
                session.VisitType = ai.VisitType;
                WriteLog("MERGED VISIT TYPE = " + session.VisitType);
            }
            else
            {
                WriteLog("MERGED VISIT TYPE (kept from session) = " + (session.VisitType ?? "(none)"));
            }

            if (!string.IsNullOrWhiteSpace(ai.PatientTarget))
                session.PatientTarget = ai.PatientTarget;
            if (!string.IsNullOrWhiteSpace(ai.PatientName))
                session.PatientName = ai.PatientName;
            if (!string.IsNullOrWhiteSpace(ai.Phone))
                session.RegPhone = ai.Phone;
        }

        private void EnsureDoctorIdFromSession(WhatsAppBookingSession session)
        {
            if (!session.DoctorId.HasValue && !string.IsNullOrWhiteSpace(session.PendingAIDoctorName))
            {
                DataTable dt = GetWhatsAppDoctorsTable();
                if (dt != null)
                {
                    string searchName = NormalizeArabic(session.PendingAIDoctorName.Replace("دكتور", "").Replace("الدكتور", "").Trim());
                    foreach (DataRow row in dt.Rows)
                    {
                        string doctorName = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString();
                        if (NormalizeArabic(doctorName).Contains(searchName) || searchName.Contains(NormalizeArabic(doctorName)))
                        {
                            session.DoctorId = Convert.ToInt32(row["Id"]);
                            WriteLog("RESOLVED DOCTOR ID FROM DB = " + session.DoctorId.Value + " FOR DOCTOR: " + doctorName);
                            break;
                        }
                    }
                }
            }
        }

        private async Task<string> ProcessWhatsAppMessage(string fromPhone, string messageText)
        {
            WhatsAppBookingSession session = WhatsAppSessionManager.Get(fromPhone);

            try
            {
                string text = (messageText ?? "").Trim();
                string textWithNormalizedDigits = NormalizeArabicDigits(text);
                string normalizedText = NormalizeArabic(text);

                WriteLog("========== PROCESS MESSAGE START ==========");
                WriteLog("PHONE = " + fromPhone);
                WriteLog("ORIGINAL MESSAGE = " + messageText);
                WriteLog("NORMALIZED DIGITS = " + textWithNormalizedDigits);
                WriteLog("NORMALIZED TEXT = " + normalizedText);
                WriteLog("SESSION STEP = " + session.Step);
                WriteLog("REGISTRATION STEP = " + session.RegistrationStep);
                WriteLog("SESSION DOCTOR ID = " + (session.DoctorId.HasValue ? session.DoctorId.Value.ToString() : "NULL"));
                WriteLog("SESSION DOCTOR NAME = " + (session.PendingAIDoctorName ?? "NULL"));
                WriteLog("SESSION SPECIALTY = " + (session.PendingAISpecialty ?? "NULL"));

                if (session.RegistrationStep > 0)
                {
                    string regReply = HandleRegistrationInput(fromPhone, messageText, session);
                    WriteLog("ACTION = REGISTRATION INPUT");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return regReply;
                }

                string globalIntent = DetectGlobalIntent(normalizedText);
                if (globalIntent != null)
                {
                    WriteLog("ACTION = GLOBAL INTENT: " + globalIntent);
                    string globalReply;

                    switch (globalIntent)
                    {
                        case "CANCEL":
                            WhatsAppSessionManager.Remove(fromPhone);
                            globalReply = "تم إلغاء العملية.\r\n\r\nيمكنك إرسال «مرحبا» للبدء من جديد.";
                            break;

                        case "MYAPPOINTMENTS":
                            session.Step = 0;
                            globalReply = GetWhatsAppMyAppointments(fromPhone);
                            break;

                        case "DOCTORS":
                            session.Step = 0;
                            globalReply = GetWhatsAppDoctors();
                            break;

                        case "QUEUE":
                            session.Step = 0;
                            session.QueueLookupStep = 1;
                            globalReply = "هل تريد الاستعلام عن دورك أنت أم عن مريض آخر؟\r\n\r\n" +
                                          "1️⃣ دوري أنا\r\n" +
                                          "2️⃣ مريض آخر";
                            break;

                        case "SALAM":
                            session.Step = 0;
                            globalReply = FormatMainMenu("وعليكم السلام ورحمة الله وبركاته، أهلاً بك 👋");
                            break;

                        case "MENU":
                        default:
                            session.Step = 0;
                            globalReply = FormatMainMenu();
                            break;


                    }

                    WriteLog("RESULT = " + globalIntent);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return globalReply;
                }

                if (session.QueueLookupStep > 0)
                {
                    string queueReply = HandleQueueLookupInput(fromPhone, text, session);
                    WriteLog("ACTION = QUEUE LOOKUP STEP " + session.QueueLookupStep);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return queueReply;
                }

                if (session.Step == 0 && IsAffirmative(normalizedText))
                {
                    session.Step = 10;
                    WriteLog("ACTION = START BOOKING FROM AFFIRMATIVE REPLY");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return "تمام ✅ هل هذا الحجز لك أنت أم لشخص آخر؟\r\n\r\n" +
                           "1️⃣ لي أنا\r\n" +
                           "2️⃣ لشخص آخر (اسم ورقم هاتف مختلف)";
                }

                if (session.Step != 0 && session.Step != 10)
                {
                    AIIntentResult aiCheck = await _aiService.AskIntentAsync(messageText);
                    WriteLog("AI CHECK FOR CONTEXT CHANGE: INTENT = " + aiCheck.Intent + ", SPECIALTY = " + aiCheck.Specialty + ", DOCTOR = " + aiCheck.DoctorName);

                    bool isDoctorChange = (aiCheck.Intent == "DoctorAvailability" || aiCheck.Intent == "Doctors" || aiCheck.Intent == "MedicalServices")
                                          && (!string.IsNullOrWhiteSpace(aiCheck.Specialty) || !string.IsNullOrWhiteSpace(aiCheck.DoctorName));

                    if (isDoctorChange)
                    {
                        WriteLog("DETECTED DOCTOR/SPECIALTY CHANGE DURING STEP " + session.Step + ", RESETTING STEP TO 0");

                        if (!string.IsNullOrWhiteSpace(aiCheck.Specialty) &&
                            (string.IsNullOrWhiteSpace(session.PendingAISpecialty) ||
                             !NormalizeArabic(session.PendingAISpecialty).Equals(NormalizeArabic(aiCheck.Specialty))))
                        {
                            WriteLog("RESETTING DOCTOR ID DUE TO SPECIALTY CHANGE");
                            session.DoctorId = null;
                            session.PendingAIDoctorName = null;
                        }

                        session.Step = 0;
                        MergeAIWithSession(aiCheck, session);
                        EnsureDoctorIdFromSession(session);
                    }
                    else
                    {
                        WriteLog("NO CONTEXT CHANGE DETECTED, CONTINUING WITH CURRENT STEP");
                    }
                }

                if (session.Step == 0 && (textWithNormalizedDigits == "1" || normalizedText == "حجز" || normalizedText == "حجز موعد" || normalizedText == "موعد"))
                {
                    session.Step = 10;
                    WriteLog("ACTION = START BOOKING");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return "هل هذا الحجز لك أنت أم لشخص آخر؟\r\n\r\n1️⃣ لي أنا\r\n2️⃣ لشخص آخر (اسم ورقم هاتف مختلف)";
                }

                if (session.Step == 10)
                {
                    WriteLog("ACTION = SELECT BOOKING TARGET");
                    string targetReply = SelectWhatsAppBookingTarget(fromPhone, textWithNormalizedDigits, session);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return targetReply;
                }

                if (session.Step == 1)
                {
                    int doctorNumber;
                    string doctorReply;
                    if (TryParseSelectionNumber(text, out doctorNumber))
                    {
                        WriteLog("ACTION = SELECT DOCTOR BY NUMBER");
                        doctorReply = SelectWhatsAppDoctor(fromPhone, doctorNumber.ToString(), session);
                    }
                    else
                    {
                        WriteLog("ACTION = SELECT DOCTOR BY AI/TEXT");
                        doctorReply = await HandleDoctorSelectionWithAI(fromPhone, messageText, session);
                    }
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return doctorReply;
                }

                if (session.Step == 4)
                {
                    WriteLog("ACTION = SELECT VISIT TYPE");
                    string visitReply = SelectWhatsAppVisitType(fromPhone, normalizedText, session);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return visitReply;
                }

                if (session.Step == 2)
                {
                    WriteLog("ACTION = SELECT SCHEDULE");
                    string scheduleReply = SelectWhatsAppSchedule(fromPhone, text, session);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return scheduleReply;
                }

                if (session.Step == 3)
                {
                    string confirmReply;
                    if (textWithNormalizedDigits == "1" || IsAffirmative(normalizedText))
                    {
                        WriteLog("ACTION = CONFIRM BOOKING");
                        confirmReply = await CreateWhatsAppBooking(fromPhone, session);
                    }
                    else if (textWithNormalizedDigits == "2" || IsNegative(normalizedText))
                    {
                        WriteLog("ACTION = REJECT BOOKING");
                        WhatsAppSessionManager.Remove(fromPhone);
                        confirmReply = "تم إلغاء الحجز.\r\n\r\nيمكنك إرسال «مرحبا» للبدء من جديد.";
                    }
                    else
                    {
                        confirmReply = "من فضلك أرسل:\r\n\r\n1️⃣ نعم ✅ لتأكيد الحجز\r\n2️⃣ لا ❌ للإلغاء";
                    }
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return confirmReply;
                }

                if (textWithNormalizedDigits == "3")
                {
                    WriteLog("ACTION = LIST DOCTORS");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return GetWhatsAppDoctors();
                }

                if (textWithNormalizedDigits == "2")
                {
                    WriteLog("ACTION = MY APPOINTMENTS");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return GetWhatsAppMyAppointments(fromPhone);
                }

                if (textWithNormalizedDigits == "4" || normalizedText == "استفسار" || normalizedText == "استفسارات")
                {
                    WriteLog("ACTION = INQUIRY");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return "من فضلك اكتب استفسارك وسأحاول مساعدتك.";
                }

                if (normalizedText.Contains("الغاء موعد") || normalizedText.Contains("الغاء الموعد"))
                {
                    WriteLog("ACTION = CANCEL APPOINTMENT (NOT IMPLEMENTED)");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return FormatUnavailableFeature("يمكنك التواصل مع المركز مباشرة لإلغاء موعدك، أو إرسال «مواعيدي» لعرض حجوزاتك الحالية.");
                }

                if (normalizedText.Contains("تغيير الموعد") || normalizedText.Contains("غيرلي الحجز") || normalizedText.Contains("اغير الموعد"))
                {
                    WriteLog("ACTION = CHANGE APPOINTMENT (NOT IMPLEMENTED)");
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return FormatUnavailableFeature("يمكنك التواصل مع المركز مباشرة لتعديل موعدك، أو إرسال «مواعيدي» لعرض حجوزاتك الحالية.");
                }

                if (session.Step == 0)
                {
                    WriteLog("ACTION = FREE TEXT AI");
                    string freeTextReply = await HandleFreeTextWithAI(fromPhone, messageText, session);
                    WriteLog("========== PROCESS MESSAGE END ==========");
                    return freeTextReply;
                }

                WriteLog("ACTION = FALLBACK CHAT AI");
                string chatReply = await _aiService.AskChatAsync(messageText, session);
                WriteLog("========== PROCESS MESSAGE END ==========");
                if (!string.IsNullOrWhiteSpace(chatReply))
                    return chatReply;

                return "لم أفهم طلبك.\r\n\r\nأرسل «مرحبا» لعرض القائمة الرئيسية.";
            }
            catch (Exception ex)
            {
                WriteLog("PROCESS MESSAGE FATAL ERROR:");
                WriteLog(ex.ToString());
                WhatsAppSessionManager.Remove(fromPhone);
                return FormatError("حدث خطأ غير متوقع.\r\n\r\nتم إعادة تعيين المحادثة، من فضلك أرسل «مرحبا» للبدء من جديد.");
            }
        }

        private async Task<string> HandleFreeTextWithAI(string fromPhone, string messageText, WhatsAppBookingSession session)
        {
            AIIntentResult ai = await _aiService.AskIntentAsync(messageText);

            WriteLog("AI INTENT = " + ai.Intent);
            WriteLog("AI SPECIALTY = " + ai.Specialty);
            WriteLog("AI DOCTOR = " + ai.DoctorName);
            WriteLog("AI DATE = " + ai.Date);
            WriteLog("AI TIME = " + ai.Time);
            WriteLog("AI SERVICE = " + ai.ServiceName);
            WriteLog("AI VISIT_TYPE = " + ai.VisitType);

            bool isDoctorIntent = (ai.Intent == "Doctors" || ai.Intent == "DoctorAvailability" || ai.Intent == "MedicalServices");
            bool hasSpecialty = !string.IsNullOrWhiteSpace(ai.Specialty);
            bool specialtyChanged = hasSpecialty &&
                                    (string.IsNullOrWhiteSpace(session.PendingAISpecialty) ||
                                     !NormalizeArabic(session.PendingAISpecialty).Equals(NormalizeArabic(ai.Specialty)));

            if (isDoctorIntent && hasSpecialty && specialtyChanged)
            {
                WriteLog("SPECIALTY CHANGED TO '" + ai.Specialty + "', RESETTING DOCTOR ID");
                session.DoctorId = null;
                session.PendingAIDoctorName = null;
            }

            MergeAIWithSession(ai, session);

            EnsureDoctorIdFromSession(session);

            string finalIntent = session.CurrentIntent ?? ai.Intent ?? "Unknown";
            WriteLog("FINAL INTENT = " + finalIntent);
            WriteLog("FINAL DOCTOR ID = " + (session.DoctorId.HasValue ? session.DoctorId.Value.ToString() : "NULL"));
            WriteLog("FINAL DOCTOR NAME = " + (session.PendingAIDoctorName ?? "NULL"));
            WriteLog("FINAL SPECIALTY = " + (session.PendingAISpecialty ?? "NULL"));
            WriteLog("FINAL DATE = " + (session.PendingAIDate ?? "NULL"));

            switch (finalIntent)
            {
                case "BookAppointment":
                    session.Step = 10;
                    string note = !string.IsNullOrWhiteSpace(session.PendingAISpecialty)
                        ? "\r\n\r\n(فهمت أنك تبحث عن تخصص: " + session.PendingAISpecialty + ")"
                        : "";
                    return "تمام ✅ هل هذا الحجز لك أنت أم لشخص آخر؟" + note +
                           "\r\n\r\n1️⃣ لي أنا\r\n2️⃣ لشخص آخر (اسم ورقم هاتف مختلف)";

                case "DoctorAvailability":
                case "Doctors":
                case "MedicalServices":
                    if (session.DoctorId.HasValue && !specialtyChanged)
                    {
                        WriteLog("ACTION = ASK VISIT TYPE FOR DOCTOR ID = " + session.DoctorId.Value);
                        session.Step = 4;   // ✅ لازم يختار كشف/استشارة الأول
                        return "👨‍⚕️ الطبيب: " + session.PendingAIDoctorName +
                               "\r\n\r\nاختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة\r\n\r\nأرسل رقم الاختيار.";
                    }
                    else if (!string.IsNullOrWhiteSpace(session.PendingAISpecialty))
                    {
                        DataTable filtered = GetDoctorsTableForSession(session);
                        if (filtered != null && filtered.Rows.Count > 0)
                        {
                            if (filtered.Rows.Count == 1)
                            {
                                DataRow row = filtered.Rows[0];
                                session.DoctorId = Convert.ToInt32(row["Id"]);
                                session.PendingAIDoctorName = row["DoctorName"].ToString();
                                WriteLog("AUTO-SELECTED SINGLE DOCTOR ID = " + session.DoctorId.Value);

                                session.Step = 4;   // ✅ نفس الإصلاح هنا
                                return "👨‍⚕️ تم اختيار الطبيب: " + session.PendingAIDoctorName +
                                       "\r\n\r\nاختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة\r\n\r\nأرسل رقم الاختيار.";
                            }
                            else
                            {
                                session.Step = 1;
                                return FormatDoctorsList(filtered, session, session.PendingAISpecialty);
                            }
                        }
                        else
                        {
                            return FormatDoctorsList(null, session, session.PendingAISpecialty);
                        }
                    }
                    else
                    {
                        return GetWhatsAppDoctors();
                    }

                case "MyAppointments":
                    return GetWhatsAppMyAppointments(fromPhone);

                case "CancelAppointment":
                    return FormatUnavailableFeature("يمكنك التواصل مع المركز مباشرة لإلغاء موعدك، أو إرسال «مواعيدي» لعرض حجوزاتك الحالية.");

                case "ChangeAppointment":
                    return FormatUnavailableFeature("يمكنك التواصل مع المركز مباشرة لتعديل موعدك، أو إرسال «مواعيدي» لعرض حجوزاتك الحالية.");

                default:
                    string chatReply = await _aiService.AskChatAsync(messageText, session);
                    if (!string.IsNullOrWhiteSpace(chatReply))
                        return chatReply;

                    return FormatMainMenu();
            }
        }

        private string GetWhatsAppMyAppointments(string fromPhone)
        {
            int patientId = GetPatientIdByWhatsAppPhone(fromPhone);
            if (patientId <= 0)
                return "لم يتم العثور على حجوزات مرتبطة برقمك بعد.\r\n\r\nيمكنك حجز موعد جديد بإرسال «حجز موعد».";

            WorkTable wt = new WorkTable();
            DataTable dt = wt.RunSelect(@"
                SELECT TOP 5 a.AppointmentDate, a.Status, d.DoctorName
                FROM Appointments a
                INNER JOIN Doctors d ON d.Id = a.DoctorId
                WHERE a.PatientId = " + patientId + @"
                ORDER BY a.AppointmentDate DESC");

            return FormatMyAppointments(dt);
        }

        private string SelectWhatsAppDoctor(string fromPhone, string text, WhatsAppBookingSession session)
        {
            try
            {
                string input = (text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(input))
                    return "من فضلك أرسل رقم الطبيب أو اسم الطبيب.";

                DataTable doctors = GetDoctorsTableForSession(session);
                if (doctors == null || doctors.Rows.Count == 0)
                    return "لا يوجد أطباء متاحون حاليًا.";

                DataRow selectedDoctor = null;
                int number;

                if (TryParseSelectionNumber(input, out number))
                {
                    if (number < 1 || number > doctors.Rows.Count)
                        return "رقم الطبيب غير صحيح.\r\nمن فضلك اختر رقمًا من القائمة.";
                    selectedDoctor = doctors.Rows[number - 1];
                }
                else
                {
                    string normalizedInput = NormalizeArabic(input);
                    foreach (DataRow row in doctors.Rows)
                    {
                        string doctorName = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(doctorName)) continue;

                        string normalizedDoctorName = NormalizeArabic(doctorName);

                        if (normalizedDoctorName == normalizedInput ||
                            normalizedDoctorName.IndexOf(normalizedInput, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            normalizedInput.IndexOf(normalizedDoctorName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            selectedDoctor = row;
                            break;
                        }
                    }
                }

                if (selectedDoctor == null)
                    return "لم أجد الطبيب \"" + input + "\" في القائمة.\r\n\r\n" + FormatDoctorsList(doctors, session);

                int doctorId = Convert.ToInt32(selectedDoctor["Id"]);
                string doctorNameSelected = selectedDoctor["DoctorName"].ToString();
                session.DoctorId = doctorId;
                session.PendingAIDoctorName = doctorNameSelected;

                DateTime aiDate;
                if (!string.IsNullOrWhiteSpace(session.PendingAIDate) && DateTime.TryParse(session.PendingAIDate, out aiDate))
                    session.PendingAIDate = aiDate.ToString("yyyy-MM-dd");

                session.Step = 4;
                return "👨‍⚕️ الطبيب: " + doctorNameSelected + "\r\n\r\nاختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة\r\n\r\nأرسل رقم الاختيار.";
            }
            catch (Exception ex)
            {
                WriteLog("SELECT DOCTOR ERROR:");
                WriteLog(ex.ToString());
                return FormatError("حدث خطأ مؤقت أثناء اختيار الطبيب.\r\nلم يتم فقد بياناتك، حاول مرة أخرى أو أرسل رقم الطبيب من القائمة.");
            }
        }

        private string SelectWhatsAppVisitType(string fromPhone, string normalizedText, WhatsAppBookingSession session)
        {
            try
            {
                if (!session.DoctorId.HasValue)
                {
                    session.Step = 1;
                    return GetWhatsAppDoctors();
                }

                int serviceNumber = 0;
                string input = (normalizedText ?? "").Trim();
                input = NormalizeArabicDigits(input);

                if (int.TryParse(input, out serviceNumber))
                {
                }
                else if (input == "كشف")
                {
                    serviceNumber = 1;
                }
                else if (input == "استشاره")
                {
                    serviceNumber = 2;
                }
                else
                {
                    return "من فضلك اختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة";
                }

                if (serviceNumber != 1 && serviceNumber != 2)
                    return "من فضلك اختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة";

                string serviceKeyword = serviceNumber == 1 ? "كشف" : "استشارة";
                session.VisitType = serviceNumber == 1 ? "Exam" : "Consultation";

                CMedicalServices services = new CMedicalServices();
                DataTable dt = services.Search(serviceKeyword);
                if (dt == null || dt.Rows.Count == 0)
                {
                    WriteLog("SERVICE NOT FOUND FOR KEYWORD = " + serviceKeyword);
                    session.Step = 1;
                    session.DoctorId = null;
                    return "عذرًا، خدمة '" + serviceKeyword + "' غير متاحة حاليًا.\r\n\r\nيرجى التواصل مع المركز، أو اختيار طبيب آخر:\r\n\r\n" + GetWhatsAppDoctors();
                }

                DataRow serviceRow = dt.Rows[0];
                int serviceId = Convert.ToInt32(serviceRow["Id"]);
                string serviceName = serviceRow["ServiceName"].ToString();

                decimal doctorAmount = serviceRow["DoctorAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(serviceRow["DoctorAmount"]);
                decimal centerAmount = serviceRow["CenterAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(serviceRow["CenterAmount"]);
                decimal totalAmount = (serviceRow.Table.Columns.Contains("TotalAmount") && serviceRow["TotalAmount"] != DBNull.Value) ?
                    Convert.ToDecimal(serviceRow["TotalAmount"]) :
                    doctorAmount + centerAmount;

                session.ServiceId = serviceId;
                session.ServiceName = serviceName;
                session.ServiceTotalAmount = totalAmount;

                CDoctorSchedules schedulesObj = new CDoctorSchedules();
                DataTable schedulesDt = schedulesObj.GetAvailableByDoctor(session.DoctorId.Value);

                if (schedulesDt == null || schedulesDt.Rows.Count == 0)
                {
                    session.Step = 1;
                    session.DoctorId = null;
                    return "✅ تم اختيار: " + serviceName + "\r\n\r\n" + FormatSchedulesList(schedulesDt) + "\r\n\r\nيمكنك اختيار طبيب آخر:\r\n\r\n" + GetWhatsAppDoctors();
                }

                session.Step = 2;
                return "✅ تم اختيار: " + serviceName + "\r\n\r\n💰 السعر الإجمالي: " + totalAmount.ToString("0.00") + " ج.م\r\n\r\nالسعر شامل المركز.\r\n\r\n" + FormatSchedulesList(schedulesDt);
            }
            catch (Exception ex)
            {
                WriteLog("SELECT VISIT TYPE ERROR:");
                WriteLog(ex.ToString());
                session.Step = 1;
                session.DoctorId = null;
                return FormatError("حدث خطأ أثناء اختيار نوع الزيارة.\r\n\r\nمن فضلك اختر طبيبًا من جديد:\r\n\r\n" + GetWhatsAppDoctors());
            }
        }

        private string SelectWhatsAppSchedule(string fromPhone, string text, WhatsAppBookingSession session)
        {
            try
            {
                int number;
                if (!TryParseSelectionNumber(text, out number))
                    return "من فضلك أرسل رقم الموعد من القائمة.";

                if (!session.DoctorId.HasValue)
                {
                    session.Step = 1;
                    return GetWhatsAppDoctors();
                }

                CDoctorSchedules schedules = new CDoctorSchedules();
                DataTable dt = schedules.GetAvailableByDoctor(session.DoctorId.Value);
                if (dt == null || dt.Rows.Count == 0)
                {
                    session.Step = 1;
                    session.DoctorId = null;
                    return "لم تعد هناك مواعيد متاحة لهذا الطبيب.\r\n\r\n" + GetWhatsAppDoctors();
                }

                if (number < 1 || number > dt.Rows.Count)
                    return "رقم الموعد غير صحيح.\r\nمن فضلك اختر رقمًا من القائمة.";

                DataRow row = dt.Rows[number - 1];
                int scheduleId = Convert.ToInt32(row["Id"]);
                DateTime start = Convert.ToDateTime(row["StartTime"]);
                DateTime end = Convert.ToDateTime(row["EndTime"]);
                session.ScheduleId = scheduleId;

                CDoctors doctors = new CDoctors();
                DataTable doctorDt = doctors.GetById(session.DoctorId.Value);
                if (doctorDt == null || doctorDt.Rows.Count == 0)
                {
                    session.Step = 1;
                    session.DoctorId = null;
                    return FormatError("تعذر قراءة بيانات الطبيب.\r\n\r\n" + GetWhatsAppDoctors());
                }

                string doctorName = doctorDt.Rows[0]["DoctorName"].ToString();
                decimal consultationFee = session.ServiceTotalAmount ?? (doctorDt.Rows[0]["ConsultationFee"] == DBNull.Value ? 0 : Convert.ToDecimal(doctorDt.Rows[0]["ConsultationFee"]));

                string patientName = null;
                int patientIdForDisplay = (session.PatientId.HasValue && session.PatientId.Value > 0) ? session.PatientId.Value : 0;
                if (patientIdForDisplay > 0)
                {
                    CPatients patientObj = new CPatients();
                    DataTable patientDt = patientObj.GetById(patientIdForDisplay);
                    if (patientDt != null && patientDt.Rows.Count > 0)
                        patientName = patientDt.Rows[0]["PatientName"].ToString();
                }

                session.Step = 3;
                return FormatBookingConfirmation(patientName, doctorName, session.ServiceName, start, end, consultationFee);
            }
            catch (Exception ex)
            {
                WriteLog("SELECT SCHEDULE ERROR:");
                WriteLog(ex.ToString());
                return FormatError("حدث خطأ مؤقت أثناء قراءة المواعيد.\r\nلم يتم إلغاء الحجز، حاول مرة أخرى.");
            }
        }

        private async Task<string> CreateWhatsAppBooking(string fromPhone, WhatsAppBookingSession session)
        {
            try
            {
                WriteLog("BOOKING START");

                if (!session.DoctorId.HasValue || !session.ScheduleId.HasValue)
                {
                    WhatsAppSessionManager.Remove(fromPhone);
                    return FormatError("حدث خطأ في بيانات الحجز.\r\nيرجى إرسال «مرحبا» والمحاولة مرة أخرى.");
                }

                CDoctorSchedules schedules = new CDoctorSchedules();
                DataTable scheduleDt = schedules.GetById(session.ScheduleId.Value);
                if (scheduleDt == null || scheduleDt.Rows.Count == 0)
                {
                    WhatsAppSessionManager.Remove(fromPhone);
                    return "الموعد غير موجود.";
                }


                DataRow scheduleRow = scheduleDt.Rows[0];

                DateTime appointmentDate = Convert.ToDateTime(scheduleRow["StartTime"]);

                int patientId = (session.PatientId.HasValue && session.PatientId.Value > 0) ?
                    session.PatientId.Value :
                    GetPatientIdByWhatsAppPhone(session.RegPhone ?? fromPhone);

                if (patientId <= 0)
                    return FormatError("حدث خطأ في تحديد بيانات المريض.\r\n\r\nمن فضلك أرسل «مرحبا» للبدء من جديد.");

                session.PatientId = patientId;
                WriteLog("PATIENT ID = " + patientId);
                WriteLog("DOCTOR ID = " + session.DoctorId.Value);

                int centerId = GetDefaultCenterId();
                if (centerId <= 0)
                    return "تعذر تحديد المركز الطبي للحجز.";

                session.CenterId = centerId;
                WriteLog("CENTER ID = " + centerId);

                if (!schedules.TryBook(session.ScheduleId.Value))
                {
                    session.Step = 2;
                    return "عذرًا، تم حجز هذا الموعد من عميل آخر.\r\n\r\nيرجى اختيار موعد آخر.";
                }

                CAppointments appointments = new CAppointments();
                appointments.PatientId = patientId;
                appointments.DoctorId = session.DoctorId.Value;
                appointments.ScheduleId = session.ScheduleId.Value;
                appointments.CenterId = centerId;
                appointments.CreatedBy = GetWhatsAppCreatedByUserId();
                appointments.AppointmentDate = appointmentDate;
                appointments.Status = "confirmed";
                appointments.Notes = session.IsBookingForOther ?
                    "WhatsApp Booking - Sender:" + fromPhone + " - For:" + (session.RegPhone ?? "") :
                    "WhatsApp Booking - " + fromPhone;

                int appointmentId = appointments.InsertAndReturnId();
                if (appointmentId <= 0)
                {
                    schedules.UnBook(session.ScheduleId.Value);
                    return "تعذر إنشاء الحجز.";
                }

                int queueNumber = appointments.QueueNumber; // اتملى تلقائيًا جوه InsertAndReturnId
                session.AppointmentId = appointmentId;
                WriteLog("APPOINTMENT ID = " + appointmentId);
                WriteLog("QUEUE NUMBER = " + queueNumber);


                CDoctors doctors = new CDoctors();
                DataTable doctorDt = doctors.GetById(session.DoctorId.Value);
                if (doctorDt == null || doctorDt.Rows.Count == 0)
                    return "تم إنشاء الحجز ولكن تعذر قراءة بيانات الطبيب.";

                DataRow doctorRow = doctorDt.Rows[0];
                string doctorName = doctorRow["DoctorName"].ToString();
                decimal consultationFee = doctorRow["ConsultationFee"] == DBNull.Value ? 0 : Convert.ToDecimal(doctorRow["ConsultationFee"]);

                decimal doctorCommission;
                decimal centerCommission;
                decimal totalAmount;

                CMedicalServices svc = new CMedicalServices();
                DataTable svcDt = (session.ServiceId.HasValue && session.ServiceId.Value > 0)
                    ? svc.GetById(session.ServiceId.Value)
                    : null;

                if (svcDt != null && svcDt.Rows.Count > 0)
                {
                    DataRow svcRow = svcDt.Rows[0];
                    doctorCommission = svcRow["DoctorAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(svcRow["DoctorAmount"]);
                    centerCommission = svcRow["CenterAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(svcRow["CenterAmount"]);
                    totalAmount = doctorCommission + centerCommission;
                }
                else
                {
                    decimal commissionRate = doctorRow["CommissionRate"] == DBNull.Value ? 0 : Convert.ToDecimal(doctorRow["CommissionRate"]);
                    totalAmount = session.ServiceTotalAmount ?? consultationFee;
                    doctorCommission = totalAmount * commissionRate / 100m;
                    centerCommission = totalAmount - doctorCommission;
                }

                CInvoices invoice = new CInvoices();
                invoice.AppointmentId = appointmentId;
                invoice.PatientId = patientId;
                invoice.DoctorId = session.DoctorId.Value;
                invoice.CenterId = centerId;
                invoice.CreatedBy = GetWhatsAppCreatedByUserId();
                invoice.ServiceId = session.ServiceId ?? 0;
                invoice.TotalAmount = totalAmount;
                invoice.DiscountAmount = 0;
                invoice.DiscountType = "";
                invoice.PaymentMethod = "WhatsApp";
                invoice.PaidAmount = 0;
                invoice.DoctorCommission = doctorCommission;
                invoice.CenterCommission = centerCommission;
                invoice.BookingType = "WhatsApp";
                invoice.PaymentStatus = "Pending";
                invoice.QueueNumber = invoice.GenerateQueueNumber(session.DoctorId.Value, appointmentId);
                invoice.Printed = false;
                invoice.PrintedDate = null;
                invoice.InvoiceDate = DateTime.Now;

                string invoiceResult = invoice.Insert();
                int invoiceId = invoice.GetLastInvoiceId();

                WriteLog("INVOICE INSERT RESULT = " + (invoiceResult ?? "NULL"));
                WriteLog("INVOICE ID = " + invoiceId.ToString());

                if (invoiceId <= 0)
                {
                    WriteLog("INVOICE INSERT FAILED - ROLLING BACK BOOKING");
                    schedules.UnBook(session.ScheduleId.Value);
                    WhatsAppSessionManager.Remove(fromPhone);
                    return FormatError("حدث خطأ فني أثناء إتمام حجزك ولم يتم تأكيده.\r\n\r\nيرجى المحاولة مرة أخرى، أو التواصل مع المركز مباشرة.");
                }

                session.InvoiceId = invoiceId;

                bool invoiceImageSent = false;
                string mediaId = null;
                try
                {
                    CPatients patientObj = new CPatients();
                    DataTable patientDt = patientObj.GetById(patientId);
                    string patientNameForInvoice = (patientDt != null && patientDt.Rows.Count > 0) ? patientDt.Rows[0]["PatientName"].ToString() : "";

                    byte[] imageBytes = GenerateInvoiceImageBytes(
                        patientNameForInvoice,
                        doctorName,
                        "غير مدفوع",
                        totalAmount,
                        appointmentDate,             // ← بدل invoice.InvoiceDate
                        invoice.InvoiceDate,
                        invoice.QueueNumber,
                        patientId.ToString());

                    mediaId = await UploadWhatsAppMediaAsync(imageBytes, "invoice.png", "image/png");
                    if (!string.IsNullOrWhiteSpace(mediaId))
                    {
                        await SendWhatsAppImageMessageAsync(fromPhone, mediaId, "🧾 فاتورة حجزك - رقم الانتظار: " + invoice.QueueNumber);
                        invoiceImageSent = true;
                    }
                    else
                    {
                        WriteLog("INVOICE IMAGE NOT SENT - MEDIA UPLOAD FAILED");
                    }
                }
                catch (Exception imgEx)
                {
                    WriteLog("INVOICE IMAGE GENERATION/SEND ERROR:");
                    WriteLog(imgEx.ToString());
                }

                WriteLog("MEDIA ID = " + (mediaId ?? "NONE"));
                WriteLog("BOOKING SUCCESS");

                WhatsAppSessionManager.Remove(fromPhone);

                return FormatBookingSuccess(doctorName, appointmentDate, invoice.QueueNumber, invoiceId, totalAmount, invoiceImageSent);
            }
            catch (Exception ex)
            {
                WriteLog("CREATE BOOKING ERROR:");
                WriteLog(ex.ToString());
                return FormatError("حدث خطأ أثناء إنشاء الحجز.\r\nلم يتم إلغاء بياناتك، يرجى المحاولة مرة أخرى.");
            }
        }

        private async Task<string> HandleDoctorSelectionWithAI(string fromPhone, string messageText, WhatsAppBookingSession session)
        {
            try
            {
                WriteLog("========== AI DOCTOR SELECTION ==========");
                WriteLog("AI INPUT = " + messageText);

                AIIntentResult ai = await _aiService.AskDoctorAsync(messageText);

                WriteLog("AI INTENT = " + ai.Intent);
                WriteLog("AI SPECIALTY = " + ai.Specialty);
                WriteLog("AI DOCTOR = " + ai.DoctorName);
                WriteLog("AI DATE = " + ai.Date);

                if (!string.IsNullOrWhiteSpace(ai.DoctorName))
                {
                    DataTable doctors = GetWhatsAppDoctorsTable();
                    if (doctors != null && doctors.Rows.Count > 0)
                    {
                        string searchName = NormalizeArabic(ai.DoctorName.Replace("دكتور", "").Replace("الدكتور", "").Trim());

                        foreach (DataRow row in doctors.Rows)
                        {
                            string doctorName = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString();
                            string normalizedDoctorName = NormalizeArabic(doctorName);

                            if (normalizedDoctorName.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                searchName.IndexOf(normalizedDoctorName, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                session.DoctorId = Convert.ToInt32(row["Id"]);
                                session.PendingAIDoctorName = doctorName;
                                session.Step = 4;
                                return "👨‍⚕️ تم اختيار الطبيب: " + doctorName +
                                       "\r\n\r\nاختر نوع الزيارة:\r\n\r\n1️⃣ كشف\r\n2️⃣ استشارة\r\n\r\nأرسل رقم الاختيار.";
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(ai.Specialty))
                {
                    session.PendingAISpecialty = ai.Specialty;
                    DataTable doctors = GetDoctorsTableForSession(session);
                    if (doctors != null && doctors.Rows.Count > 0)
                    {
                        session.Step = 1;
                        return FormatDoctorsList(doctors, session, ai.Specialty);
                    }
                }

                DataTable allDoctors = GetWhatsAppDoctorsTable();
                if (allDoctors != null && allDoctors.Rows.Count > 0)
                {
                    string normalizedMessage = NormalizeArabic(messageText);
                    DataTable filtered = allDoctors.Clone();
                    foreach (DataRow row in allDoctors.Rows)
                    {
                        string doctorName = row["DoctorName"] == DBNull.Value ? "" : row["DoctorName"].ToString();
                        string specialty = row["Specialty"] == DBNull.Value ? "" : row["Specialty"].ToString();

                        if (NormalizeArabic(doctorName).IndexOf(normalizedMessage, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            NormalizeArabic(specialty).IndexOf(normalizedMessage, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            filtered.ImportRow(row);
                        }
                    }
                    if (filtered.Rows.Count > 0)
                    {
                        session.PendingAISpecialty = messageText;
                        session.Step = 1;
                        return FormatDoctorsList(filtered, session, messageText);
                    }
                }

                return "لم أتمكن من تحديد الطبيب أو التخصص المطلوب 🤔\r\n\r\n" +
                       "من فضلك أرسل اسم الطبيب أو التخصص، مثل:\r\n\r\n" +
                       "👨‍⚕️ محمد الغباشي\r\n🩺 قلب\r\n👶 أطفال\r\nأو أرسل رقم الطبيب من القائمة.";
            }
            catch (Exception ex)
            {
                WriteLog("AI DOCTOR SELECTION ERROR:");
                WriteLog(ex.ToString());
                return FormatError("حدث خطأ أثناء فهم طلبك.\r\n\r\nمن فضلك أرسل اسم الطبيب أو التخصص، أو رقم الطبيب من القائمة.");
            }
        }


        private int GetPatientIdByWhatsAppPhone(string phone)
        {
            WorkTable wt = new WorkTable();
            DataTable dt = wt.RunSelect(@"
                SELECT TOP 1 Id FROM Patients WHERE Phone = '" + phone.Replace("'", "''") + @"' ORDER BY Id DESC");
            return (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["Id"]) : 0;
        }

        private int GetDefaultCenterId()
        {
            WorkTable wt = new WorkTable();
            DataTable dt = wt.RunSelect("SELECT TOP 1 Id FROM MedicalCenters ORDER BY Id");
            return (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["Id"]) : 0;
        }

        private int GetWhatsAppCreatedByUserId()
        {
            string value = ConfigurationManager.AppSettings["WhatsAppCreatedByUserId"];
            int id;
            return int.TryParse(value, out id) ? id : 1;
        }

        private DataTable GetWhatsAppDoctorsTable()
        {
            try
            {
                CDoctors doctors = new CDoctors();
                return doctors.GetClinicDoctorsWithAvailableSchedules();
            }
            catch (Exception ex)
            {
                WriteLog("GET WHATSAPP DOCTORS ERROR:");
                WriteLog(ex.ToString());
                return null;
            }
        }

        private string GetAvailableSchedulesMessage(int doctorId)
        {
            try
            {
                CDoctorSchedules schedules = new CDoctorSchedules();
                DataTable dt = schedules.GetAvailableByDoctor(doctorId);
                return FormatSchedulesList(dt);
            }
            catch (Exception ex)
            {
                WriteLog("GET AVAILABLE SCHEDULES ERROR:");
                WriteLog(ex.ToString());
                return FormatError("حدث خطأ أثناء قراءة المواعيد المتاحة.");
            }
        }

        private void WriteLog(string text)
        {
            try
            {
                string logPath = HostingEnvironment.MapPath("~/webhook_log.txt");
                if (string.IsNullOrWhiteSpace(logPath))
                    logPath = @"D:\Riyadh Al-Salehin\Riyadh Al-Salehin\webhook_log.txt";

                File.AppendAllText(logPath, text + "\r\n", Encoding.UTF8);
            }
            catch { /* تجاهل فشل الكتابة */ }
        }

        private byte[] GenerateInvoiceImageBytes(
            string patientName,
            string doctorName,
            string paymentStatusText,
            decimal amount,
            DateTime appointmentDate,
            DateTime invoiceDate,
            string queueNumber,
            string barcodeData)
        {
            int width = 600;
            int height = 950;

            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.White);

                StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center };
                StringFormat sfRtl = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                };

                using (Font titleFont = new Font("Tahoma", 22, FontStyle.Bold))
                using (Font subFont = new Font("Tahoma", 14, FontStyle.Regular))
                using (Font labelFont = new Font("Tahoma", 14, FontStyle.Bold))
                using (Font queueFont = new Font("Tahoma", 46, FontStyle.Bold))
                using (Brush black = new SolidBrush(Color.Black))
                using (Pen linePen = new Pen(Color.Black, 1))
                using (Pen boxPen = new Pen(Color.Black, 2))
                {
                    int y = 20;

                    g.DrawString("عيادات الرياض الصالحين", titleFont, black,
                        new RectangleF(0, y, width, 40), sfCenter);
                    y += 45;

                    g.DrawString("فاتورة كشف طبي", subFont, black,
                        new RectangleF(0, y, width, 30), sfCenter);
                    y += 35;

                    g.DrawLine(linePen, 20, y, width - 20, y);
                    y += 15;

                    void DrawRow(string label, string value)
                    {
                        string line = label + " :  " + value;
                        g.DrawString(line, labelFont, black,
                            new RectangleF(30, y, width - 60, 30), sfRtl);
                        y += 34;
                    }

                    DrawRow("المريض", patientName);
                    DrawRow("الطبيب", doctorName);
                    DrawRow("حالة الدفع", paymentStatusText);
                    DrawRow("القيمة", amount.ToString("N2") + " ج.م");
                    DrawRow("التاريخ", invoiceDate.ToString("yyyy/MM/dd hh:mm tt"));

                    y += 8;
                    g.DrawLine(linePen, 20, y, width - 20, y);
                    y += 15;

                    g.DrawString("رقم الانتظار", subFont, black,
                        new RectangleF(0, y, width, 25), sfCenter);
                    y += 30;

                    Rectangle queueRect = new Rectangle(width / 2 - 110, y, 220, 90);
                    g.DrawRectangle(boxPen, queueRect);
                    g.DrawString(queueNumber, queueFont, black, queueRect, sfCenter);
                    y += 110;

                    if (!string.IsNullOrWhiteSpace(barcodeData))
                    {
                        try
                        {
                            BarcodeWriter writer = new BarcodeWriter();
                            writer.Format = BarcodeFormat.CODE_128;
                            writer.Options = new ZXing.Common.EncodingOptions
                            {
                                Width = 420,
                                Height = 110,
                                Margin = 2
                            };

                            using (Bitmap barcodeBmp = writer.Write(barcodeData))
                            {
                                g.DrawImage(barcodeBmp, (width - 420) / 2, y, 420, 110);
                            }
                            y += 120;
                        }
                        catch { /* تجاهل فشل الباركود */ }
                    }

                    g.DrawLine(linePen, 20, y, width - 20, y);
                    y += 15;

                    g.DrawString("نتمنى لكم الشفاء العاجل", subFont, black,
                        new RectangleF(0, y, width, 25), sfCenter);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }


        private string HandleQueueLookupInput(string fromPhone, string text, WhatsAppBookingSession session)
        {
            string normalized = NormalizeArabic(text);

            if (session.QueueLookupStep == 1)
            {
                if (text == "1" || normalized == "لي" || normalized == "لي انا" || normalized == "انا")
                {
                    session.QueueLookupStep = 0;
                    return GetWhatsAppQueueStatus(fromPhone);
                }

                if (text == "2" || normalized == "شخص اخر" || normalized == "مريض اخر" || normalized == "اخر")
                {
                    session.QueueLookupStep = 2;
                    return "من فضلك أرسل الاسم الكامل للمريض، مثل:\r\n\r\nيوسف محمد أحمد";
                }

                return "من فضلك اختر:\r\n\r\n1️⃣ دوري أنا\r\n2️⃣ مريض آخر";
            }

            if (session.QueueLookupStep == 2)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return "من فضلك أرسل الاسم الكامل للمريض.";

                session.QueuePatientName = text.Trim();
                int patientId = GetPatientIdByNameWithTodayAppointment(session.QueuePatientName);
                if (patientId <= 0)
                {
                    session.QueueLookupStep = 0;
                    return "لم أجد موعد انتظار اليوم بهذا الاسم.\r\n\r\n" +
                           "تأكد من كتابة الاسم كما هو مسجل، أو أرسل «استفسار عن الدور» للمحاولة مرة أخرى.";
                }

                session.QueueLookupStep = 0;
                return GetWhatsAppQueueStatusByPatientId(patientId);
            }

            session.QueueLookupStep = 0;
            return "أرسل «استفسار عن الدور» للبدء من جديد.";
        }

        private int GetPatientIdByNameWithTodayAppointment(string patientName)
        {
            WorkTable wt = new WorkTable();
            DataTable dt = wt.RunSelect(@"
                SELECT
                    p.Id,
                    p.PatientName,
                    a.AppointmentDate,
                    a.QueueNumber
                FROM Patients p
                INNER JOIN Appointments a ON a.PatientId = p.Id
                WHERE CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
                  AND a.Status IN ('confirmed', 'pending', 'Pending', 'WaitingDoctor', 'WaitingForXrayResult', 'XrayDone')
                ORDER BY a.AppointmentDate ASC, a.QueueNumber ASC");

            if (dt == null || dt.Rows.Count == 0)
                return 0;

            string searchName = NormalizeArabic(patientName);
            DataRow partialMatch = null;

            foreach (DataRow row in dt.Rows)
            {
                string storedName = row["PatientName"] == DBNull.Value
                    ? ""
                    : NormalizeArabic(row["PatientName"].ToString());

                if (string.IsNullOrWhiteSpace(storedName))
                    continue;

                if (storedName == searchName)
                    return Convert.ToInt32(row["Id"]);

                if (!string.IsNullOrWhiteSpace(searchName) &&
                    (storedName.Contains(searchName) || searchName.Contains(storedName)))
                {
                    partialMatch = row;
                }
            }

            return partialMatch == null ? 0 : Convert.ToInt32(partialMatch["Id"]);
        }

        private string GetWhatsAppQueueStatus(string fromPhone)
        {
            int patientId = GetPatientIdByWhatsAppPhone(fromPhone);
            if (patientId <= 0)
                return "لم يتم العثور على حجز مرتبط برقمك.\r\n\r\nيمكنك حجز موعد جديد بإرسال «حجز موعد».";

            return GetWhatsAppQueueStatusByPatientId(patientId);
        }

        private string GetWhatsAppQueueStatusByPatientId(int patientId)
        {
            if (patientId <= 0)
                return "لم يتم العثور على بيانات المريض.";

            WorkTable wt = new WorkTable();
            var appointmentParams = new List<SqlParameter>
            {
                new SqlParameter("@PatientId", patientId)
            };

            DataTable dt = wt.RunSelect(@"
                SELECT TOP 1
                    a.DoctorId,
                    a.AppointmentDate,
                    a.QueueNumber,
                    d.DoctorName
                FROM Appointments a
                INNER JOIN Doctors d ON d.Id = a.DoctorId
                WHERE a.PatientId = @PatientId
                  AND CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
                  AND a.Status IN ('confirmed', 'pending', 'Pending', 'WaitingDoctor', 'WaitingForXrayResult', 'XrayDone')
                ORDER BY a.AppointmentDate ASC, a.QueueNumber ASC", appointmentParams);

            if (dt == null || dt.Rows.Count == 0)
                return "لا يوجد لديك موعد مسجل اليوم.\r\n\r\nيمكنك إرسال «مواعيدي» لعرض كل حجوزاتك.";

            DataRow row = dt.Rows[0];
            int doctorId = Convert.ToInt32(row["DoctorId"]);
            int myQueueNumber = row["QueueNumber"] == DBNull.Value ? 0 : Convert.ToInt32(row["QueueNumber"]);
            string doctorName = row["DoctorName"].ToString();
            DateTime apptDate = Convert.ToDateTime(row["AppointmentDate"]);

            var aheadParams = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@QueueNumber", myQueueNumber)
            };

            DataTable aheadDt = wt.RunSelect(@"
                SELECT COUNT(*) AS Cnt
                FROM Appointments
                WHERE DoctorId = @DoctorId
                  AND CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
                  AND QueueNumber < @QueueNumber
                  AND Status IN ('confirmed', 'pending', 'Pending', 'WaitingDoctor', 'WaitingForXrayResult', 'XrayDone')", aheadParams);

            int aheadCount = (aheadDt != null && aheadDt.Rows.Count > 0) ? Convert.ToInt32(aheadDt.Rows[0]["Cnt"]) : 0;

            string doctorCode = Riyadh_Al_Salehin.CDoctors.GetDoctorCode(doctorId);
            string formattedQueue = myQueueNumber > 0 ? $"{doctorCode}{myQueueNumber}" : "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🎫 *استعلام رقم الانتظار*");
            sb.AppendLine();
            sb.AppendLine("👨‍⚕️ الطبيب: " + doctorName);
            sb.AppendLine("⏰ موعدك: " + apptDate.ToString("HH:mm"));
            if (!string.IsNullOrEmpty(formattedQueue))
                sb.AppendLine("🎫 رقم انتظارك: " + formattedQueue);
            sb.AppendLine();

            if (aheadCount <= 0)
                sb.AppendLine("✅ لا يوجد مرضى قبلك حالياً، أنت التالي لدخول الطبيب.");
            else
                sb.AppendLine("👥 يوجد قبلك: " + aheadCount + (aheadCount == 1 ? " مريض واحد" : " مرضى") + ".");

            sb.AppendLine("يرجى متابعة هاتفك حتى يحين دورك.");

            return sb.ToString().TrimEnd();
        }



    
        private async Task<string> UploadWhatsAppMediaAsync(byte[] fileBytes, string fileName, string mimeType)
        {
            WriteLog("========== UPLOAD MEDIA START ==========");
            try
            {
                string accessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
                string phoneNumberId = ConfigurationManager.AppSettings["WhatsAppPhoneNumberId"];
                string graphVersion = ConfigurationManager.AppSettings["WhatsAppGraphVersion"];

                if (string.IsNullOrWhiteSpace(graphVersion))
                    graphVersion = "v25.0";

                if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
                {
                    WriteLog("ERROR: ACCESS TOKEN OR PHONE NUMBER ID EMPTY");
                    return null;
                }

                string url = "https://graph.facebook.com/" + graphVersion + "/" + phoneNumberId + "/media";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (MultipartFormDataContent form = new MultipartFormDataContent())
                    {
                        ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                        form.Add(fileContent, "file", fileName);
                        form.Add(new StringContent("whatsapp"), "messaging_product");
                        form.Add(new StringContent(mimeType), "type");

                        HttpResponseMessage response = await client.PostAsync(url, form);
                        string result = await response.Content.ReadAsStringAsync();

                        WriteLog("MEDIA UPLOAD HTTP STATUS = " + ((int)response.StatusCode));
                        WriteLog("MEDIA UPLOAD RESPONSE = " + result);

                        if (!response.IsSuccessStatusCode)
                            return null;

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        var data = serializer.DeserializeObject(result) as Dictionary<string, object>;

                        if (data != null && data.ContainsKey("id") && data["id"] != null)
                            return data["id"].ToString();

                        WriteLog("MEDIA UPLOAD: NO ID IN RESPONSE");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("MEDIA UPLOAD EXCEPTION:");
                WriteLog(ex.ToString());
                return null;
            }
            finally
            {
                WriteLog("========== UPLOAD MEDIA END ==========");
            }
        }

        private async Task SendWhatsAppImageMessageAsync(string toPhone, string mediaId, string caption)
        {
            WriteLog("========== SEND IMAGE START ==========");
            try
            {
                string accessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
                string phoneNumberId = ConfigurationManager.AppSettings["WhatsAppPhoneNumberId"];
                string graphVersion = ConfigurationManager.AppSettings["WhatsAppGraphVersion"];

                if (string.IsNullOrWhiteSpace(graphVersion))
                    graphVersion = "v25.0";

                string url = "https://graph.facebook.com/" + graphVersion + "/" + phoneNumberId + "/messages";

                var data = new
                {
                    messaging_product = "whatsapp",
                    to = toPhone,
                    type = "image",
                    image = new { id = mediaId, caption = caption }
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(data);

                WriteLog("SEND IMAGE REQUEST JSON = " + json);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string result = await response.Content.ReadAsStringAsync();

                        WriteLog("SEND IMAGE HTTP STATUS = " + ((int)response.StatusCode));
                        WriteLog("SEND IMAGE RESPONSE = " + result);

                        if (response.IsSuccessStatusCode)
                            WriteLog("SEND IMAGE SUCCESS");
                        else
                            WriteLog("SEND IMAGE FAILED");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("SEND IMAGE EXCEPTION:");
                WriteLog(ex.ToString());
            }
            finally
            {
                WriteLog("========== SEND IMAGE END ==========");
            }
        }

        private async Task SendWhatsAppMessageAsync(string toPhone, string messageText)
        {
            WriteLog("========== SEND WHATSAPP START ==========");
            try
            {
                string accessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
                string phoneNumberId = ConfigurationManager.AppSettings["WhatsAppPhoneNumberId"];
                string graphVersion = ConfigurationManager.AppSettings["WhatsAppGraphVersion"];

                if (string.IsNullOrWhiteSpace(graphVersion))
                    graphVersion = "v25.0";

                if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
                {
                    WriteLog("ERROR: ACCESS TOKEN OR PHONE NUMBER ID EMPTY");
                    return;
                }

                string url = "https://graph.facebook.com/" + graphVersion + "/" + phoneNumberId + "/messages";

                var data = new
                {
                    messaging_product = "whatsapp",
                    to = toPhone,
                    type = "text",
                    text = new { body = messageText }
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(data);

                WriteLog("REQUEST JSON:");
                WriteLog(json);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string result = await response.Content.ReadAsStringAsync();

                        WriteLog("META HTTP STATUS = " + ((int)response.StatusCode) + " " + response.StatusCode.ToString());
                        WriteLog("META RESPONSE:");
                        WriteLog(result);

                        if (response.IsSuccessStatusCode)
                            WriteLog("SEND SUCCESS");
                        else
                            WriteLog("SEND FAILED");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("SEND EXCEPTION:");
                WriteLog(ex.ToString());
            }
            WriteLog("========== SEND WHATSAPP END ==========");
        }
    }






}