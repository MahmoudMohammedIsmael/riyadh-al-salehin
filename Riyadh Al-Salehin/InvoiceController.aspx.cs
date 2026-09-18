using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Riyadh_Al_Salehin
{
    public partial class InvoiceController : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        /// <summary>
        /// WebMethod لجلب بيانات الفاتورة للمعاينة
        /// </summary>
        [WebMethod]
        public static object GetInvoiceData(string invoiceType, int invoiceId)
        {
            try
            {
                var wt = new WorkTable();
                string query = "";
                
                switch (invoiceType)
                {
                    case "Consultation":
                        query = @"
                            SELECT 
                                i.Id AS InvoiceId,
                                i.TotalAmount,
                                i.PaidAmount,
                                i.DiscountAmount,
                                i.QueueNumber,
                                i.InvoiceDate,
                                i.PaymentStatus,
                                i.PaymentMethod,
                                i.BookingType,
                                i.ServiceId,
                                p.PatientName,
                                p.Id AS PatientId,
                                d.DoctorName,
                                ms.ServiceName,
                                ms.DoctorAmount,
                                ms.CenterAmount
                            FROM Invoices i
                            INNER JOIN Patients p ON i.PatientId = p.Id
                            INNER JOIN Doctors d ON i.DoctorId = d.Id
                            LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                            WHERE i.Id = @InvoiceId";
                        break;
                    case "Surgery":
                        query = @"
                            SELECT 
                                i.Id AS InvoiceId,
                                i.TotalAmount,
                                i.PaidAmount,
                                i.DiscountAmount,
                                i.QueueNumber,
                                i.InvoiceDate,
                                i.PaymentStatus,
                                i.PaymentMethod,
                                p.PatientName,
                                p.Id AS PatientId,
                                d.DoctorName,
                                s.SurgeryName AS ServiceName
                            FROM Invoices i
                            INNER JOIN Patients p ON i.PatientId = p.Id
                            INNER JOIN Doctors d ON i.DoctorId = d.Id
                            LEFT JOIN Surgeries s ON i.ServiceId = s.Id
                            WHERE i.Id = @InvoiceId";
                        break;
                    case "Xray":
                        query = @"
                            SELECT 
                                i.Id AS InvoiceId,
                                i.TotalAmount,
                                i.PaidAmount,
                                i.DiscountAmount,
                                i.QueueNumber,
                                i.InvoiceDate,
                                i.PaymentStatus,
                                i.PaymentMethod,
                                p.PatientName,
                                p.Id AS PatientId,
                                d.DoctorName,
                                xs.XrayName AS ServiceName
                            FROM Invoices i
                            INNER JOIN Patients p ON i.PatientId = p.Id
                            INNER JOIN Doctors d ON i.DoctorId = d.Id
                            LEFT JOIN XrayServices xs ON i.ServiceId = xs.Id
                            WHERE i.Id = @InvoiceId";
                        break;
                    case "Additional":
                        query = @"
                            SELECT 
                                i.Id AS InvoiceId,
                                i.TotalAmount,
                                i.PaidAmount,
                                i.DiscountAmount,
                                i.QueueNumber,
                                i.InvoiceDate,
                                i.PaymentStatus,
                                i.PaymentMethod,
                                p.PatientName,
                                p.Id AS PatientId,
                                d.DoctorName
                            FROM Invoices i
                            INNER JOIN Patients p ON i.PatientId = p.Id
                            INNER JOIN Doctors d ON i.DoctorId = d.Id
                            WHERE i.Id = @InvoiceId";
                        break;
                }

                if (string.IsNullOrEmpty(query))
                    return new { IsSuccess = false, Message = "نوع الفاتورة غير مدعوم" };

                var prm = new List<SqlParameter> { new SqlParameter("@InvoiceId", invoiceId) };
                DataTable dt = wt.RunSelect(query, prm);

                if (dt == null || dt.Rows.Count == 0)
                    return new { IsSuccess = false, Message = "لم يتم العثور على الفاتورة" };

                DataRow row = dt.Rows[0];
                
                var result = new
                {
                    IsSuccess = true,
                    JsonData = JsonConvert.SerializeObject(new
                    {
                        InvoiceId = row["InvoiceId"].ToString(),
                        InvoiceTypeName = GetInvoiceTypeName(invoiceType),
                        PatientName = row["PatientName"]?.ToString() ?? "",
                        DoctorName = row["DoctorName"]?.ToString() ?? "",
                        ServiceName = row["ServiceName"]?.ToString() ?? "",
                        QueueNumber = row["QueueNumber"]?.ToString() ?? "",
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]).ToString("yyyy/MM/dd hh:mm tt"),
                        PaymentStatus = row["PaymentStatus"]?.ToString() ?? "Unpaid",
                        PaymentMethod = row["PaymentMethod"]?.ToString() ?? "",
                        BookingType = row["BookingType"]?.ToString() ?? "",
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]).ToString("N2"),
                        PaidAmount = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]).ToString("N2") : "0.00",
                        DiscountAmount = row["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiscountAmount"]).ToString("N2") : "0.00",
                        BarcodeValue = row["PatientId"]?.ToString() ?? "",
                        ClinicName = "عيادات الرياض الصالحين",
                        ClinicPhone = "هاتف: 01000000000"
                    })
                };

                return result;
            }
            catch (Exception ex)
            {
                return new { IsSuccess = false, Message = "خطأ: " + ex.Message };
            }
        }

        private static string GetInvoiceTypeName(string type)
        {
            switch (type)
            {
                case "Consultation": return "كشف طبي";
                case "Surgery": return "عملية جراحية";
                case "Xray": return "أشعة";
                case "Additional": return "فاتورة إضافية";
                default: return "طبية";
            }
        }

        /// <summary>
        /// WebMethod لحفظ إعدادات القالب
        /// </summary>
        [WebMethod]
        public static object SaveTemplateSettings(string templateName, string settingsJson)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<InvoiceTemplateManager.TemplateSettings>(settingsJson);
                settings.Name = templateName;
                InvoiceTemplateManager.SaveTemplate(settings);
                return new { IsSuccess = true, Message = "تم حفظ الإعدادات بنجاح" };
            }
            catch (Exception ex)
            {
                return new { IsSuccess = false, Message = "خطأ: " + ex.Message };
            }
        }

        /// <summary>
        /// WebMethod لجلب إعدادات القالب الحالي
        /// </summary>
        [WebMethod]
        public static object GetTemplateSettings(string templateName)
        {
            try
            {
                var settings = InvoiceTemplateManager.GetTemplate(templateName);
                return new { IsSuccess = true, Settings = settings };
            }
            catch (Exception ex)
            {
                return new { IsSuccess = false, Message = "خطأ: " + ex.Message };
            }
        }
    }
}