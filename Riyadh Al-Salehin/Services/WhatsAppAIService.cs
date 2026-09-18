using Riyadh_Al_Salehin.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Riyadh_Al_Salehin.Services
{
    public class AIIntentResult
    {
        public string Intent { get; set; }
        public string Specialty { get; set; }
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ServiceName { get; set; }
        public string PatientTarget { get; set; }   // "self" أو "other"
        public string PatientName { get; set; }
        public string Phone { get; set; }
        public string VisitType { get; set; }       // "Exam" أو "Consultation"
    }

    public class WhatsAppAIService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private const string Model = "gemini-3.5-flash-lite";

        public WhatsAppAIService()
        {
            _apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("GeminiApiKey is missing in app settings.");

            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        }

        private async Task<string> CallGeminiAsync(string systemPrompt, string userMessage, bool expectJson = false, byte[] audioBytes = null, string audioMimeType = null)
        {
            try
            {
                var contentParts = new List<object>();

                if (!string.IsNullOrWhiteSpace(userMessage))
                {
                    contentParts.Add(new { text = userMessage });
                }

                if (audioBytes != null && audioBytes.Length > 0 && !string.IsNullOrWhiteSpace(audioMimeType))
                {
                    contentParts.Add(new
                    {
                        inline_data = new
                        {
                            mime_type = audioMimeType,
                            data = Convert.ToBase64String(audioBytes)
                        }
                    });
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = systemPrompt },
                                new { text = "رسالة المريض:" }
                            }
                        },
                        new
                        {
                            parts = contentParts.ToArray()
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024,
                        responseMimeType = expectJson ? "application/json" : "text/plain"
                    }
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(requestBody);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(2);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(_apiUrl, content);
                    string result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Gemini API Error: {response.StatusCode}\n{result}");
                    }

                    var data = serializer.DeserializeObject(result) as Dictionary<string, object>;
                    if (data == null || !data.ContainsKey("candidates"))
                        return "";

                    var candidates = data["candidates"] as object[];
                    if (candidates == null || candidates.Length == 0)
                        return "";

                    var firstCandidate = candidates[0] as Dictionary<string, object>;
                    if (firstCandidate == null || !firstCandidate.ContainsKey("content"))
                        return "";

                    var contentObj = firstCandidate["content"] as Dictionary<string, object>;
                    if (contentObj == null || !contentObj.ContainsKey("parts"))
                        return "";

                    var parts = contentObj["parts"] as object[];
                    if (parts == null || parts.Length == 0)
                        return "";

                    var firstPart = parts[0] as Dictionary<string, object>;
                    if (firstPart == null || !firstPart.ContainsKey("text"))
                        return "";

                    return firstPart["text"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GEMINI API ERROR: " + ex);
                return "ERROR: " + ex.Message;
            }
        }

        public async Task<string> AskAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "";

            string today = DateTime.Now.ToString("yyyy-MM-dd");

            string systemPrompt =
                "أنت محلل طلبات المرضى في مستشفى الرياض الصالحين. " +
                "مهمتك فقط فهم رسالة المريض وتحويلها إلى JSON. " +
                "لا تقدم نصائح طبية. " +
                "لا تخترع أطباء أو مواعيد أو أسعار. " +
                "لا تنفذ أي عملية بنفسك. " +
                "النظام البرمجي هو الذي سينفذ العملية بعد تحليل طلبك. " +
                "تاريخ اليوم هو " + today + ". " +
                "حدد intent من القيم التالية فقط: " +
                "BookAppointment, MyAppointments, CancelAppointment, ChangeAppointment, " +
                "Doctors, DoctorAvailability, MedicalServices, Registration, GeneralQuestion, Unknown. " +
                "استخرج المعلومات التالية إن وجدت: specialty, doctorName, date, time, serviceName, patientTarget (self أو other), patientName, phone, visitType (Exam أو Consultation). " +
                "أعد JSON فقط بدون أي شرح أو Markdown. " +
                "الصيغة: {\"intent\":\"Unknown\",\"specialty\":\"\",\"doctorName\":\"\",\"date\":\"\",\"time\":\"\",\"serviceName\":\"\",\"patientTarget\":\"\",\"patientName\":\"\",\"phone\":\"\",\"visitType\":\"\"}";

            return await CallGeminiAsync(systemPrompt, message, expectJson: true);
        }

        public async Task<AIIntentResult> AskIntentAsync(string message)
        {
            string raw = await AskAsync(message);
            var result = new AIIntentResult();

            if (string.IsNullOrWhiteSpace(raw) || raw.StartsWith("ERROR:"))
                return result;

            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.DeserializeObject(raw) as Dictionary<string, object>;
                if (data == null) return result;

                result.Intent = GetStr(data, "intent", "Unknown");
                result.Specialty = GetStr(data, "specialty", "");
                result.DoctorName = GetStr(data, "doctorName", "");
                result.Date = GetStr(data, "date", "");
                result.Date = NormalizeArabicDate(message, result.Date);
                result.Time = GetStr(data, "time", "");
                result.ServiceName = GetStr(data, "serviceName", "");
                result.PatientTarget = GetStr(data, "patientTarget", "");
                result.PatientName = GetStr(data, "patientName", "");
                result.Phone = GetStr(data, "phone", "");
                result.VisitType = GetStr(data, "visitType", "");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI JSON PARSE ERROR: " + ex);
            }

            return result;
        }

        public async Task<AIIntentResult> AskDoctorAsync(string message)
        {
            var result = new AIIntentResult();
            if (string.IsNullOrWhiteSpace(message))
                return result;

            string today = DateTime.Now.ToString("yyyy-MM-dd");

            string systemPrompt =
                "أنت مساعد ذكي لمستشفى الرياض الصالحين. " +
                "المريض الآن يريد اختيار طبيب. " +
                "حلل رسالة المريض لمعرفة هل يقصد اسم طبيب أو تخصص طبي. " +
                "إذا كتب المريض اسم طبيب، ضعه في doctorName. " +
                "إذا كتب تخصصًا طبيًا، ضعه في specialty. " +
                "أمثلة: محمد الغباشي => doctorName = محمد الغباشي. " +
                "دكتور محمد الغباشي => doctorName = محمد الغباشي. " +
                "اطفال => specialty = أطفال. قلب => specialty = قلب. " +
                "لا تخترع أي اسم طبيب أو تخصص غير موجود في رسالة المستخدم. " +
                "إذا ذكر المريض تاريخًا، ضعه في date (بصيغة YYYY-MM-DD). " +
                "إذا ذكر وقتًا، ضعه في time (مثل 10:00). " +
                "أعد JSON فقط بدون Markdown أو شرح. " +
                "التاريخ الحالي هو " + today + ". " +
                "الصيغة: {\"intent\":\"Doctors\",\"specialty\":\"\",\"doctorName\":\"\",\"date\":\"\",\"time\":\"\",\"serviceName\":\"\",\"patientTarget\":\"\",\"patientName\":\"\",\"phone\":\"\",\"visitType\":\"\"}";

            string raw = await CallGeminiAsync(systemPrompt, message, expectJson: true);
            if (string.IsNullOrWhiteSpace(raw) || raw.StartsWith("ERROR:"))
                return result;

            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.DeserializeObject(raw) as Dictionary<string, object>;
                if (data == null) return result;

                result.Intent = GetStr(data, "intent", "Doctors");
                result.Specialty = GetStr(data, "specialty", "");
                result.DoctorName = GetStr(data, "doctorName", "");
                result.Date = GetStr(data, "date", "");
                result.Date = NormalizeArabicDate(message, result.Date);
                result.Time = GetStr(data, "time", "");
                result.ServiceName = GetStr(data, "serviceName", "");
                result.PatientTarget = GetStr(data, "patientTarget", "");
                result.PatientName = GetStr(data, "patientName", "");
                result.Phone = GetStr(data, "phone", "");
                result.VisitType = GetStr(data, "visitType", "");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI DOCTOR JSON PARSE ERROR: " + ex);
            }

            return result;
        }

        public async Task<string> AskChatAsync(string message, WhatsAppBookingSession session)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "";

            string today = DateTime.Now.ToString("yyyy-MM-dd");

            string context =
                "حالة المحادثة الحالية:\r\n" +
                "Step = " + session.Step + "\r\n" +
                "RegistrationStep = " + session.RegistrationStep + "\r\n" +
                "PatientId = " + (session.PatientId.HasValue ? session.PatientId.Value.ToString() : "") + "\r\n" +
                "DoctorId = " + (session.DoctorId.HasValue ? session.DoctorId.Value.ToString() : "") + "\r\n" +
                "PendingSpecialty = " + (session.PendingAISpecialty ?? "") + "\r\n" +
                "PendingDoctor = " + (session.PendingAIDoctorName ?? "") + "\r\n" +
                "PendingDate = " + (session.PendingAIDate ?? "") + "\r\n";

            string systemPrompt =
                "أنت المساعد الذكي الرسمي لمستشفى الرياض الصالحين 🏥.\r\n" +
                "تتحدث مع المرضى باللغة العربية.\r\n" +
                "كن طبيعيًا وودودًا ومختصرًا.\r\n" +
                "افهم اللهجات العربية والأخطاء الإملائية.\r\n" +
                "لا تخترع أسماء أطباء أو مواعيد أو أسعار.\r\n" +
                "لا تقل إن الحجز تم إلا إذا قام النظام البرمجي بتنفيذه فعليًا.\r\n" +
                "إذا كان المستخدم يريد حجز موعد، ساعده في إكمال بيانات الحجز.\r\n" +
                "إذا كان المستخدم يسأل عن طبيب أو تخصص أو موعد، افهم المقصود.\r\n" +
                "إذا كانت الرسالة قصيرة مثل: نعم، لا، لي، غدا، أطفال، قلب، محمود، افهمها حسب سياق المحادثة.\r\n" +
                "لا تقدم تشخيصًا طبيًا أو علاجًا.\r\n" +
                "تاريخ اليوم هو " + today + ".\r\n\r\n" +
                context;

            return await CallGeminiAsync(systemPrompt, message, expectJson: false);
        }

        public async Task<string> TranscribeAudioAsync(byte[] audioBytes, string mimeType)
        {
            if (audioBytes == null || audioBytes.Length == 0)
                return "";

            try
            {
                string fileUri = await UploadAudioToGeminiAsync(audioBytes, mimeType);
                if (string.IsNullOrWhiteSpace(fileUri))
                    return "غير واضح (فشل رفع الصوت)";

                string systemPrompt =
                    "أنت مساعد متخصص في تحويل التسجيلات الصوتية إلى نص مكتوب. " +
                    "الرجاء كتابة النص العربي الذي قاله المريض في التسجيل الصوتي بدقة. " +
                    "أعد النص فقط بدون أي شرح إضافي. " +
                    "إذا كان التسجيل غير واضح أو لا يحتوي على كلام، اكتب 'غير واضح'.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = systemPrompt },
                                new { text = "المحتوى الصوتي:" },
                                new { file_data = new { mime_type = mimeType, file_uri = fileUri } }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        maxOutputTokens = 1024
                    }
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(requestBody);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(2);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(_apiUrl, content);
                    string result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Gemini API Error: {response.StatusCode}\n{result}");

                    var data = serializer.DeserializeObject(result) as Dictionary<string, object>;
                    if (data == null || !data.ContainsKey("candidates")) return "غير واضح";

                    var candidates = data["candidates"] as object[];
                    if (candidates == null || candidates.Length == 0) return "غير واضح";

                    var firstCandidate = candidates[0] as Dictionary<string, object>;
                    if (firstCandidate == null || !firstCandidate.ContainsKey("content")) return "غير واضح";

                    var contentObj = firstCandidate["content"] as Dictionary<string, object>;
                    if (contentObj == null || !contentObj.ContainsKey("parts")) return "غير واضح";

                    var parts = contentObj["parts"] as object[];
                    if (parts == null || parts.Length == 0) return "غير واضح";

                    var firstPart = parts[0] as Dictionary<string, object>;
                    if (firstPart == null || !firstPart.ContainsKey("text")) return "غير واضح";

                    return firstPart["text"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GEMINI AUDIO ERROR: " + ex);
                return "غير واضح";
            }
        }

        private async Task<string> UploadAudioToGeminiAsync(byte[] audioBytes, string mimeType)
        {
            try
            {
                string uploadUrl = $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={_apiKey}";
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(2);

                    using (MultipartFormDataContent content = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(audioBytes);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                        content.Add(fileContent, "file", "audio.ogg");
                        content.Add(new StringContent("user_data"), "purpose");

                        HttpResponseMessage response = await client.PostAsync(uploadUrl, content);
                        string result = await response.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.WriteLine("UPLOAD RESPONSE STATUS: " + response.StatusCode);
                        System.Diagnostics.Debug.WriteLine("UPLOAD RESPONSE BODY: " + result);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Upload failed: {response.StatusCode}\n{result}");
                        }

                        var data = new JavaScriptSerializer().DeserializeObject(result) as Dictionary<string, object>;
                        if (data != null && data.ContainsKey("file") && data["file"] is Dictionary<string, object> fileObj &&
                            fileObj.ContainsKey("uri") && fileObj["uri"] != null)
                        {
                            return fileObj["uri"].ToString();
                        }

                        if (data != null && data.ContainsKey("uri") && data["uri"] != null)
                            return data["uri"].ToString();

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UPLOAD AUDIO ERROR: " + ex);
                return null;
            }
        }

        private string NormalizeArabicDate(string message, string aiDate)
        {
            string text = (message ?? "").Trim();

            if (text.Contains("اليوم"))
                return DateTime.Today.ToString("yyyy-MM-dd");

            if (text.Contains("غدا") || text.Contains("غدًا") || text.Contains("بكره") || text.Contains("بكرة"))
                return DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            if (text.Contains("بعد غد") || text.Contains("بعد بكرة") || text.Contains("بعدبكرة"))
                return DateTime.Today.AddDays(2).ToString("yyyy-MM-dd");

            if (DateTime.TryParse(aiDate, out DateTime date))
                return date.ToString("yyyy-MM-dd");

            return "";
        }

        private static string GetStr(Dictionary<string, object> data, string key, string def)
        {
            return (data.ContainsKey(key) && data[key] != null) ? data[key].ToString().Trim() : def;
        }
    }
}