using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class InvoicesList : System.Web.UI.Page
    {
        private CInvoices inv = new CInvoices();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                LoadInvoices();
                LoadStatistics();
            }
        }

        private void LoadInvoices()
        {
            DataTable dt = inv.GetAll(); // يسترجع جميع الفواتير مع بيانات المريض والطبيب والمركز
            gvInvoices.DataSource = dt;
            gvInvoices.DataBind();
        }

        private void LoadStatistics()
        {
            DataTable dt = inv.GetAll();
            if (dt.Rows.Count > 0)
            {
                decimal total = 0, paid = 0, pending = 0;
                foreach (DataRow row in dt.Rows)
                {
                    decimal amt = Convert.ToDecimal(row["TotalAmount"]);
                    decimal paidAmt = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0;
                    total += amt;
                    paid += paidAmt;
                    pending += (amt - paidAmt);
                }
                lblTotalInvoices.Text = dt.Rows.Count.ToString();
                lblTotalAmount.Text = total.ToString("N2") + " ج.م";
                lblTotalPaid.Text = paid.ToString("N2") + " ج.م";
                lblTotalPending.Text = pending.ToString("N2") + " ج.م";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string filter = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(filter))
            {
                DataTable dt = inv.GetAll();
                DataView dv = dt.DefaultView;
                dv.RowFilter = $"PatientName LIKE '%{filter}%' OR DoctorName LIKE '%{filter}%' OR InvoiceId LIKE '%{filter}%'";
                gvInvoices.DataSource = dv;
                gvInvoices.DataBind();
            }
            else
            {
                LoadInvoices();
            }
        }

        protected void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string invoiceId = btn.CommandArgument;
            Response.Redirect($"ConsultationInvoicePrint.aspx?InvoiceId={invoiceId}");
        }

        protected void gvInvoices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInvoices.PageIndex = e.NewPageIndex;
            LoadInvoices();
        }
    }
}