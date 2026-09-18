using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class XrayInvoicePrint : BaseInvoicePrintPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TemplateName = "Xray";
            base.OnInit(e);

            if (!IsPostBack)
            {
                lblRequestId.Text = Session["XrayInv_RequestId"] as string ?? "0";
                lblPatient.Text = Session["XrayInv_Patient"] as string ?? "غير معروف";
                lblService.Text = Session["XrayInv_Service"] as string ?? "غير محدد";
                string price = Session["XrayInv_Price"] as string ?? "0.00";
                lblPrice.Text = price;
                lblPriceCost.Text = price + " ر.س";
                lblTotal.Text = price + " ر.س";
                lblDate.Text = Session["XrayInv_Date"] as string ?? DateTime.Now.ToString("yyyy-MM-dd");
                lblPrintDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                string barcodeText = "XR-" + lblRequestId.Text;
                SetBarcodeImage(imgBarcode, barcodeText);
                lblBarcodeText.Text = barcodeText;

                string patientId = Session["XrayInv_PatientId"] as string;
                if (!string.IsNullOrEmpty(patientId))
                {
                    lblPatientId.Text = patientId;
                    SetBarcodeImage(imgPatientBarcode, patientId);
                }
                else
                {
                    imgPatientBarcode.Visible = false;
                }

                string apptId = Session["XrayInv_AppointmentId"] as string ?? lblRequestId.Text;
                if (!string.IsNullOrEmpty(apptId))
                {
                    lblAppointmentId.Text = apptId;
                    SetBarcodeImage(imgAppointmentBarcode, apptId);
                }
                else
                {
                    imgAppointmentBarcode.Visible = false;
                }
            }
        }
    }
}