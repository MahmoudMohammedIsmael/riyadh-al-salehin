using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Riyadh_Al_Salehin
{
    public partial class ConsultationInvoicePrint : BaseInvoicePrintPage
    {
        private readonly WorkTable wt = new WorkTable();
        protected override void OnInit(EventArgs e)
        {
            TemplateName = "Consultation";
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyTemplateSettings();

                if (Request.QueryString["InvoiceId"] != null && int.TryParse(Request.QueryString["InvoiceId"], out int invoiceId))
                {
                    LoadInvoiceData(invoiceId);
                    return;
                }

                LoadFromSession();
            }
        }

        private void LoadInvoiceData(int invoiceId)
        {
            try
            {
                string query = @"
                    SELECT 
                        i.Id AS InvoiceId,
                        i.DoctorId,
                        i.AppointmentId,
                        i.TotalAmount,
                        i.PaidAmount,
                        i.QueueNumber,
                        i.InvoiceDate,
                        i.PaymentStatus,
                        i.PaymentMethod,
                        p.PatientName,
                        d.DoctorName,
                        p.Id AS PatientId
                    FROM Invoices i
                    INNER JOIN Patients p ON i.PatientId = p.Id
                    INNER JOIN Doctors d ON i.DoctorId = d.Id
                    WHERE i.Id = @InvoiceId";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@InvoiceId", invoiceId)
                };

                DataTable dt = wt.RunSelect(query, prm);

                if (dt == null || dt.Rows.Count == 0)
                {
                    lblPatient.Text = $"لا توجد بيانات للفاتورة رقم {invoiceId}";
                    lblQueue.Text = "";
                    return;
                }

                DataRow row = dt.Rows[0];

                lblPatient.Text = row["PatientName"].ToString();
                lblDoctor.Text = row["DoctorName"].ToString();

                lblInvoiceId.Text = row["InvoiceId"]?.ToString();
                lblAppointmentId.Text = row["AppointmentId"] is DBNull ? string.Empty : row["AppointmentId"].ToString();
                lblPatientId.Text = row["PatientId"]?.ToString();

                var total = Convert.ToDecimal(row["TotalAmount"]);
                var paid = row["PaidAmount"] is DBNull ? 0m : Convert.ToDecimal(row["PaidAmount"]);
                decimal remaining = total - paid;

                lblAmount.Text = $"{total:N2} ج.م";

                string queue = row["QueueNumber"]?.ToString();
                if (string.IsNullOrEmpty(queue))
                {
                    var docId = row["DoctorId"] is DBNull ? 0 : Convert.ToInt32(row["DoctorId"]);
                    var apptId = row["AppointmentId"] is DBNull ? 0 : Convert.ToInt32(row["AppointmentId"]);
                    var inv = new CInvoices();
                    queue = inv.GenerateQueueNumber(docId, apptId > 0 ? (int?)apptId : null);
                    if (!string.IsNullOrEmpty(queue))
                    {
                        wt.RunInsDelUpd($"UPDATE Invoices SET QueueNumber = '{queue}' WHERE Id = {invoiceId}");
                    }
                    else
                    {
                        queue = "غير محدد";
                    }
                }
                lblQueue.Text = queue;

                DateTime invDate = Convert.ToDateTime(row["InvoiceDate"]);
                lblDate.Text = invDate.ToString("yyyy/MM/dd hh:mm tt");

                string status = row["PaymentStatus"]?.ToString() ?? "Unpaid";
                lblPaymentStatus.Text = (status.Equals("Paid", StringComparison.OrdinalIgnoreCase)) ? "مدفوع" : "غير مدفوع";

                var patientId = row["PatientId"]?.ToString();
                if (!string.IsNullOrEmpty(patientId))
                    SetBarcodeImage(imgBarcode, patientId);
                else if (imgBarcode != null)
                    imgBarcode.Visible = false;

                if (!string.IsNullOrEmpty(patientId))
                {
                    SetBarcodeImage(Image1, patientId);
                }
                else if (Image1 != null)
                {
                    Image1.Visible = false;
                }

                var apptIdStr = row["AppointmentId"] is DBNull ? string.Empty : row["AppointmentId"].ToString();
                if (!string.IsNullOrEmpty(apptIdStr))
                {
                    SetBarcodeImage(imgAppointmentBarcode, apptIdStr);
                }
                else
                {
                    imgAppointmentBarcode.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblPatient.Text = $"خطأ: {ex.Message}";
                lblQueue.Text = "";
            }
        }

        private void LoadFromSession()
        {
            lblPatient.Text = Session["Inv_Patient"] as string;
            lblDoctor.Text = Session["Inv_Doctor"] as string;
            lblAmount.Text = Session["Inv_Amount"] as string;
            lblQueue.Text = Session["Inv_Queue"] as string;
            lblDate.Text = Session["Inv_Date"] as string ?? DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");

            string paymentStatus = Session["Inv_PaymentStatus"] as string;
            lblPaymentStatus.Text = (paymentStatus == "Paid") ? "مدفوع" : "غير مدفوع";

            string barcodeBase64 = Session["Inv_Barcode"] as string;
            if (!string.IsNullOrEmpty(barcodeBase64))
            {
                imgBarcode.ImageUrl = "data:image/png;base64," + barcodeBase64;
                imgBarcode.Visible = true;
            }
            else
            {
                imgBarcode.Visible = false;
            }

            if (string.IsNullOrEmpty(lblPatient.Text) && string.IsNullOrEmpty(lblQueue.Text))
                lblPatient.Text = "لا توجد بيانات فاتورة لعرضها";

            if (string.IsNullOrEmpty(lblInvoiceId.Text))
                lblInvoiceId.Text = Session["Inv_Id"]?.ToString();
            if (string.IsNullOrEmpty(lblAppointmentId.Text))
                lblAppointmentId.Text = Session["Inv_AppointmentId"]?.ToString();
            if (string.IsNullOrEmpty(lblPatientId.Text))
                lblPatientId.Text = Session["Inv_PatientId"]?.ToString();
        }

    }
}