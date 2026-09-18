using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

namespace Riyadh_Al_Salehin
{
    public partial class SurgeryInvoicePrint : BaseInvoicePrintPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TemplateName = "Surgery";
            base.OnInit(e);

            if (!IsPostBack)
            {
                int invoiceId = 0;
                if (int.TryParse(Request.QueryString["id"], out invoiceId))
                {
                    WorkTable wt = new WorkTable();
                    DataTable dt = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE Id=" + invoiceId);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        int surgeryId = row["SurgeryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["SurgeryId"]);

                        DataTable dtS = wt.RunSelect("SELECT s.*, p.PatientName, d.DoctorName, r.RoomName FROM Surgeries s INNER JOIN Patients p ON s.PatientId=p.Id INNER JOIN Doctors d ON s.DoctorId=d.Id LEFT JOIN OperationRooms r ON s.RoomId=r.Id WHERE s.Id=" + surgeryId);
                        if (dtS.Rows.Count > 0)
                        {
                            DataRow sr = dtS.Rows[0];
                            lblSurgery.Text = surgeryId.ToString();
                            lblSurgeryName.Text = sr["SurgeryName"].ToString();
                            lblPatient.Text = sr["PatientName"].ToString();
                            lblDoctor.Text = sr["DoctorName"].ToString();
                            lblRoom.Text = sr.Table.Columns.Contains("RoomName") ? sr["RoomName"].ToString() : "";
                            lblDate.Text = Convert.ToDateTime(sr["SurgeryDate"]).ToString("yyyy/MM/dd HH:mm");
                            lblPatientId.Text = sr["PatientId"].ToString();
                        }

                        lblDoctorCost.Text = Convert.ToDecimal(row["DoctorCost"]).ToString("0.##");
                        lblRoomCost.Text = Convert.ToDecimal(row["RoomCost"]).ToString("0.##");
                        lblSuppliesCost.Text = Convert.ToDecimal(row["SuppliesCost"]).ToString("0.##");
                        lblOtherCost.Text = Convert.ToDecimal(row["OtherCost"]).ToString("0.##");
                        lblDiscount.Text = Convert.ToDecimal(row["Discount"]).ToString("0.##");
                        lblTotal.Text = Convert.ToDecimal(row["TotalAmount"]).ToString("0.##");
                        lblPaid.Text = Convert.ToDecimal(row["PaidAmount"]).ToString("0.##");
                        lblRemain.Text = Convert.ToDecimal(row["RemainingAmount"]).ToString("0.##");
                        lblPrintDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                        string status = row["PaymentStatus"].ToString();
                        lblPaymentStatus.Text = status == "Unpaid" ? "غير مدفوع" : status == "Partial" ? "مدفوع جزئياً" : "مدفوع";

                        string surgeryBarcode = GenerateBarcode("SUR-" + surgeryId.ToString("000000"));
                        imgSurgeryBarcode.ImageUrl = "data:image/png;base64," + surgeryBarcode;
                        lblSurgeryBarcode.Text = "SUR-" + surgeryId.ToString("000000");

                        string invoiceBarcode = GenerateBarcode("INV-" + invoiceId.ToString("000000"));
                        imgInvoiceBarcode.ImageUrl = "data:image/png;base64," + invoiceBarcode;
                        lblInvoiceBarcode.Text = "INV-" + invoiceId.ToString("000000");

                        CSurgerySupplies sss = new CSurgerySupplies();
                        DataTable dtSup = sss.GetBySurgery(surgeryId);

                        if (dtSup != null && dtSup.Rows.Count > 0)
                        {
                            rptSupplies.DataSource = dtSup;
                            rptSupplies.DataBind();
                        }
                    }
                }
                else
                {
                    lblSurgery.Text = Session["SurgInv_SurgeryId"] as string;
                    lblSurgeryName.Text = Session["SurgInv_SurgeryName"] as string;
                    lblPatient.Text = Session["SurgInv_Patient"] as string;
                    lblDoctor.Text = Session["SurgInv_Doctor"] as string;
                    lblRoom.Text = Session["SurgInv_Room"] as string;
                    lblDate.Text = Session["SurgInv_Date"] as string;
                    lblPatientId.Text = Session["SurgInv_PatientId"] as string;
                    lblDoctorCost.Text = Session["SurgInv_DoctorCost"] as string;
                    lblRoomCost.Text = Session["SurgInv_RoomCost"] as string;
                    lblSuppliesCost.Text = Session["SurgInv_SuppliesCost"] as string;
                    lblOtherCost.Text = Session["SurgInv_OtherCost"] as string;
                    lblDiscount.Text = Session["SurgInv_Discount"] as string;
                    lblTotal.Text = Session["SurgInv_Total"] as string;
                    lblPaid.Text = Session["SurgInv_Paid"] as string;
                    lblRemain.Text = Session["SurgInv_Remaining"] as string;
                    lblPrintDate.Text = Session["SurgInv_PrintDate"] as string;
                    lblPaymentStatus.Text = Session["SurgInv_PaymentStatus"] as string;

                    string surgeryBarcode = Session["SurgInv_SurgeryBarcode"] as string;
                    if (!string.IsNullOrEmpty(surgeryBarcode))
                    {
                        imgSurgeryBarcode.ImageUrl = "data:image/png;base64," + surgeryBarcode;
                        lblSurgeryBarcode.Text = Session["SurgInv_SurgeryBarcodeText"] as string;
                    }
                    string invoiceBarcode = Session["SurgInv_InvoiceBarcode"] as string;
                    if (!string.IsNullOrEmpty(invoiceBarcode))
                    {
                        imgInvoiceBarcode.ImageUrl = "data:image/png;base64," + invoiceBarcode;
                        lblInvoiceBarcode.Text = Session["SurgInv_InvoiceBarcodeText"] as string;
                    }
                    if (Session["SurgInv_Supplies"] is DataTable dtSup2)
                    {
                        rptSupplies.DataSource = dtSup2;
                        rptSupplies.DataBind();
                    }
                }
            }
        }

        private string GenerateBarcode(string text)
        {
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.CODE_128;
            writer.Options = new EncodingOptions { Width = 350, Height = 80, Margin = 2 };

            using (Bitmap bmp = writer.Write(text))
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}