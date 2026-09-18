using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace Riyadh_Al_Salehin
{
    public partial class NewConsultationInvoice : System.Web.UI.Page
    {
        private WorkTable wt = new WorkTable();
        private CInvoices inv = new CInvoices();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null && Request.QueryString["uid"] != null)
            {
                if (int.TryParse(Request.QueryString["uid"], out int uid))
                {
                    Session["UserId"] = uid;
                    HttpCookie cookie = new HttpCookie("UserId", uid.ToString());
                    cookie.Expires = DateTime.Now.AddHours(2);
                    Response.Cookies.Add(cookie);
                }
            }

            if (Session["UserId"] == null)
            {
                HttpCookie cookie = Request.Cookies["UserId"];
                if (cookie != null && int.TryParse(cookie.Value, out int uid))
                {
                    Session["UserId"] = uid;
                }
            }

            if (Session["UserId"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadInvoiceData();
            }
        }

        private void LoadInvoiceData()
        {
            int appointmentId = 0, serviceId = 0;
            if (Request.QueryString["AppointmentId"] != null)
                int.TryParse(Request.QueryString["AppointmentId"], out appointmentId);
            if (Request.QueryString["ServiceId"] != null)
                int.TryParse(Request.QueryString["ServiceId"], out serviceId);

            hfAppointmentId.Value = appointmentId.ToString();
            hfServiceId.Value = serviceId.ToString();

            if (appointmentId == 0 || serviceId == 0)
            {
                ShowMessage("بيانات غير مكتملة", "error");
                printArea.Visible = false;
                return;
            }

            string query = $@"
                SELECT 
                    p.Id AS PatientId, p.PatientName, 
                    d.Id AS DoctorId, d.DoctorName, 
                    a.CenterId,
                    i.PaymentMethod, i.PaymentStatus, i.BookingType, 
                    i.PaidAmount, i.TotalAmount, i.QueueNumber,
                    ms.ServiceName,
                    ms.DoctorAmount, ms.CenterAmount
                FROM Invoices i
                INNER JOIN Appointments a ON i.AppointmentId = a.Id
                INNER JOIN Patients p ON i.PatientId = p.Id
                INNER JOIN Doctors d ON i.DoctorId = d.Id
                INNER JOIN MedicalServices ms ON i.ServiceId = ms.Id
                WHERE a.Id = {appointmentId} AND ms.Id = {serviceId}";

            DataTable dt = wt.RunSelect(query);

            if (dt.Rows.Count == 0)
            {
                DataTable apptInfo = wt.RunSelect($"SELECT DoctorId FROM Appointments WHERE Id = {appointmentId}");
                int docId = (apptInfo != null && apptInfo.Rows.Count > 0) ? Convert.ToInt32(apptInfo.Rows[0]["DoctorId"]) : 0;
                string initQueue = inv.GenerateQueueNumber(docId, appointmentId);

                string insertQuery = $@"
                    INSERT INTO Invoices (AppointmentId, PatientId, DoctorId, CenterId, CreatedBy, ServiceId, 
                                          TotalAmount, PaymentMethod, PaymentStatus, BookingType, PaidAmount, QueueNumber)
                    SELECT 
                        a.Id, a.PatientId, a.DoctorId, a.CenterId, a.CreatedBy, ms.Id,
                        ms.TotalAmount, 'Cash' AS PaymentMethod, 'Unpaid' AS PaymentStatus, 
                        'WalkIn' AS BookingType, 0 AS PaidAmount, '{initQueue}' AS QueueNumber
                    FROM Appointments a
                    CROSS JOIN MedicalServices ms
                    WHERE a.Id = {appointmentId} AND ms.Id = {serviceId}";

                string result = wt.RunInsDelUpd(insertQuery);
                if (result == "OK")
                {
                    dt = wt.RunSelect(query);
                }
                else
                {
                    ShowMessage("حدث خطأ أثناء إنشاء الفاتورة: " + result, "error");
                    printArea.Visible = false;
                    return;
                }
            }

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                hfPatientId.Value = row["PatientId"].ToString();
                hfDoctorId.Value = row["DoctorId"].ToString();
                hfCenterId.Value = row["CenterId"].ToString();

                lblDoctor.InnerText = row["DoctorName"].ToString();
                lblPatient.InnerText = row["PatientName"].ToString();
                lblServiceName.InnerText = row["ServiceName"].ToString();

                if (ddlPaymentMethod.Items.FindByValue(row["PaymentMethod"].ToString()) != null)
                    ddlPaymentMethod.SelectedValue = row["PaymentMethod"].ToString();
                if (ddlPaymentStatus.Items.FindByValue(row["PaymentStatus"].ToString()) != null)
                    ddlPaymentStatus.SelectedValue = row["PaymentStatus"].ToString();
                if (ddlBookingType.Items.FindByValue(row["BookingType"].ToString()) != null)
                    ddlBookingType.SelectedValue = row["BookingType"].ToString();

                decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);
                lblAmount.InnerText = totalAmount.ToString("N2") + " ج.م";

                decimal paidAmount = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0;
                txtPaidAmount.Text = paidAmount.ToString("N2");
                txtRemaining.Text = (totalAmount - paidAmount).ToString("N2");

                LoadPrintData(appointmentId);

                printArea.Visible = true;
                btnPrint.Visible = true;

                ShowMessage("تم تحميل بيانات الفاتورة بنجاح", "success");
            }
            else
            {
                ShowMessage("تعذر تحميل البيانات", "error");
                printArea.Visible = false;
            }
        }

        protected void btnSaveInvoice_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfAppointmentId.Value) || hfAppointmentId.Value == "0")
            {
                ShowMessage("بيانات الموعد غير صالحة", "error");
                return;
            }

            if (!decimal.TryParse(txtPaidAmount.Text, out decimal paidAmount))
            {
                ShowMessage("يرجى إدخال مبلغ مدفوع صحيح", "warning");
                return;
            }

            int appId = Convert.ToInt32(hfAppointmentId.Value);

            string getTotalQuery = $"SELECT TotalAmount FROM Invoices WHERE AppointmentId = {appId}";
            DataTable dtTotal = wt.RunSelect(getTotalQuery);
            decimal totalAmount = 0;
            if (dtTotal.Rows.Count > 0)
                totalAmount = Convert.ToDecimal(dtTotal.Rows[0]["TotalAmount"]);

            string updateQuery = $@"
        UPDATE Invoices 
        SET 
            PaymentMethod = '{ddlPaymentMethod.SelectedValue}',
            PaymentStatus = '{ddlPaymentStatus.SelectedValue}',
            BookingType = '{ddlBookingType.SelectedValue}',
            PaidAmount = {paidAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}
        WHERE AppointmentId = {appId}";

            string result = wt.RunInsDelUpd(updateQuery);

            if (result != "OK")
            {
                ShowMessage("خطأ أثناء الحفظ: " + result, "error");
                return;
            }

            string apptDebug = "لا يوجد تحديث للموعد (الحالة ليست Paid)";
            if (ddlPaymentStatus.SelectedValue == "Paid")
            {
                string updateApptQuery = $@"
            UPDATE Appointments 
            SET Status = 'Pending' 
            WHERE Id = {appId} 
              AND Status NOT IN ('InExamination', 'Done', 'Cancelled')";

                apptDebug = wt.RunInsDelUpd(updateApptQuery);
            }

            txtRemaining.Text = (totalAmount - paidAmount).ToString("N2");

            ShowMessage($"تم حفظ الفاتورة | تحديث الموعد: {apptDebug}", "success");

            LoadPrintData(appId);
            printArea.Visible = true;
            btnPrint.Visible = true;
        }

        private void LoadPrintData(int appointmentId)
        {
            string query = $@"
                SELECT 
                    p.PatientName, d.Id AS DoctorId, d.DoctorName, i.TotalAmount, i.PaidAmount, 
                    i.PaymentMethod, i.PaymentStatus, i.BookingType,
                    i.QueueNumber
                FROM Invoices i
                INNER JOIN Patients p ON i.PatientId = p.Id
                INNER JOIN Doctors d ON i.DoctorId = d.Id
                WHERE i.AppointmentId = {appointmentId}";

            DataTable dt = wt.RunSelect(query);
            if (dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];
            int doctorId = Convert.ToInt32(row["DoctorId"]);
            string patientName = row["PatientName"].ToString();
            string doctorName = row["DoctorName"].ToString();
            decimal total = Convert.ToDecimal(row["TotalAmount"]);
            decimal paid = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0;
            decimal remaining = total - paid;

            lblPatientPrint.Text = patientName;
            lblDoctorPrint.Text = doctorName;
            lblDatePrint.Text = DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");
            lblAmountPrint.Text = total.ToString("N2") + " ج.م";
            lblPaidPrint.Text = paid.ToString("N2") + " ج.م";
            lblRemainingPrint.Text = remaining.ToString("N2") + " ج.م";
            lblPaymentMethodPrint.Text = ddlPaymentMethod.SelectedItem.Text;
            lblPaymentStatusPrint.Text = ddlPaymentStatus.SelectedItem.Text;

            string queueNumber = row["QueueNumber"]?.ToString();
            if (string.IsNullOrEmpty(queueNumber))
            {
                queueNumber = inv.GenerateQueueNumber(doctorId, appointmentId);
                string updateQueue = $"UPDATE Invoices SET QueueNumber = '{queueNumber}' WHERE AppointmentId = {appointmentId}";
                wt.RunInsDelUpd(updateQueue);
            }
            lblQueuePrint.Text = queueNumber;

            string patientId = hfPatientId.Value;
            if (!string.IsNullOrEmpty(patientId))
                GenerateBarcode(patientId);

            Session["Inv_Patient"] = patientName;
            Session["Inv_Doctor"] = doctorName;
            Session["Inv_Amount"] = total.ToString("N2") + " ج.م";
            Session["Inv_Queue"] = queueNumber;
            Session["Inv_Date"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");
            Session["Inv_PatientId"] = patientId;
            Session["Inv_PaymentStatus"] = ddlPaymentStatus.SelectedItem.Text;
            Session["Inv_TotalAmount"] = total.ToString("N2");
            Session["Inv_PaidAmount"] = paid.ToString("N2");
            Session["Inv_Remaining"] = remaining.ToString("N2");
            Session["Inv_Barcode"] = GenerateBarcodeBase64(patientId);
        }

        private void GenerateBarcode(string patientId)
        {
            string base64 = GenerateBarcodeBase64(patientId);
            imgBarcode.ImageUrl = "data:image/png;base64," + base64;
            imgBarcode.Width = Unit.Pixel(240);
            imgBarcode.Height = Unit.Pixel(60);
        }

        private string GenerateBarcodeBase64(string patientId)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.CODE_128;
            writer.Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 80,
                Margin = 2
            };
            Bitmap bmp = writer.Write(patientId);
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }


        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfAppointmentId.Value, out int appointmentId) || appointmentId == 0)
            {
                ShowMessage("بيانات الموعد غير صالحة للطباعة", "error");
                return;
            }

            string query = $"SELECT Id FROM Invoices WHERE AppointmentId = {appointmentId}";
            DataTable dt = wt.RunSelect(query);
            if (dt.Rows.Count == 0)
            {
                ShowMessage("لا توجد فاتورة لهذا الموعد", "error");
                return;
            }

            int invoiceId = Convert.ToInt32(dt.Rows[0]["Id"]);

            Response.Redirect($"ConsultationInvoicePrint.aspx?InvoiceId={invoiceId}", false);
            Context.ApplicationInstance.CompleteRequest();
        }

       


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Patients.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string msg, string type)
        {
            lblMessage.Text = msg;
            switch (type)
            {
                case "success": lblMessage.ForeColor = Color.Green; break;
                case "error": lblMessage.ForeColor = Color.Red; break;
                case "warning": lblMessage.ForeColor = Color.Orange; break;
            }
        }
    }
}