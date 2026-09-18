
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class InvoiceItemsPrint : BaseInvoicePrintPage
    {
        private readonly WorkTable wt = new WorkTable();
        private readonly CInvoiceItems items = new CInvoiceItems();

        protected void Page_Load(object sender, EventArgs e)
        {
            TemplateName = "Additional";
            base.OnInit(e);

            if (IsPostBack)
                return;

            int invoiceId;

            if (!int.TryParse(Request.QueryString["InvoiceId"], out invoiceId) || invoiceId <= 0)
            {
                lblPatient.Text = "لا يوجد رقم فاتورة صحيح";
                return;
            }

            DataTable dt = wt.RunSelect(@"
SELECT
    i.Id AS InvoiceId,
    i.AppointmentId,
    p.Id AS PatientId,
    i.QueueNumber,
    i.TotalAmount,
    i.PaidAmount,
    i.InvoiceDate,
    p.PatientName,
    d.DoctorName
FROM Invoices i
INNER JOIN Patients p ON i.PatientId = p.Id
INNER JOIN Doctors d ON i.DoctorId = d.Id
WHERE i.Id = " + invoiceId);

            if (dt.Rows.Count == 0)
            {
                lblPatient.Text = "الفاتورة غير موجودة";
                return;
            }

            DataRow row = dt.Rows[0];

            lblInvoiceId.Text = row["InvoiceId"]?.ToString();
            lblAppointmentId.Text = row["AppointmentId"] != DBNull.Value ? row["AppointmentId"].ToString() : string.Empty;
            lblPatientId.Text = row["PatientId"]?.ToString();
            lblPatient.Text = row["PatientName"].ToString();
            lblDoctor.Text = row["DoctorName"].ToString();
            lblQueue.Text = row["QueueNumber"].ToString();

            decimal total = Convert.ToDecimal(row["TotalAmount"]);
            decimal paid = Convert.ToDecimal(row["PaidAmount"]);

            lblGrandTotal.Text = total.ToString("N2") + " ج.م";
            lblPaymentStatus.Text = (paid >= total) ? "مدفوع" : "غير مدفوع";

            lblDate.Text = Convert.ToDateTime(row["InvoiceDate"]).ToString("yyyy/MM/dd hh:mm tt");

            string patientId = row["PatientId"]?.ToString();
            if (!string.IsNullOrEmpty(patientId))
                SetBarcodeImage(imgPatientBarcode, patientId);
            else
                imgPatientBarcode.Visible = false;

            string apptId = row["AppointmentId"] != DBNull.Value ? row["AppointmentId"].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(apptId))
                SetBarcodeImage(imgAppointmentBarcode, apptId);
            else
                imgAppointmentBarcode.Visible = false;

            DataTable itemsDt = items.GetByInvoice(invoiceId);
            rptItems.DataSource = itemsDt;
            rptItems.DataBind();
        }
    }
}