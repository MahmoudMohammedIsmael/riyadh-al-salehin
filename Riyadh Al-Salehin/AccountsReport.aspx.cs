using System;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class AccountsReport : System.Web.UI.Page
    {
        private CAccountsReport report = new CAccountsReport();
        private CDoctors doctors = new CDoctors();  

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadDoctors();
                LoadReport();
            }
        }

        private void LoadDoctors()
        {
            DataTable dt = doctors.GetAll();  // استخدام CDoctors مباشرة
            ddlDoctor.Items.Clear();
            ddlDoctor.Items.Add(new ListItem("-- جميع الأطباء --", ""));
            foreach (DataRow row in dt.Rows)
            {
                ddlDoctor.Items.Add(new ListItem(
                    row["DoctorName"].ToString(),
                    row["Id"].ToString()
                ));
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                if (!DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) ||
                    !DateTime.TryParse(txtToDate.Text, out DateTime toDate))
                {
                    ShowMessage("يرجى إدخال تواريخ صحيحة.");
                    return;
                }

                if (fromDate > toDate)
                {
                    ShowMessage("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.");
                    return;
                }

                int? doctorId = null;
                if (!string.IsNullOrEmpty(ddlDoctor.SelectedValue))
                    doctorId = Convert.ToInt32(ddlDoctor.SelectedValue);

                DataTable dt = report.GetInvoiceData(fromDate, toDate, doctorId);
                int rowCount = dt.Rows.Count;

                if (rowCount == 0)
                {
                    ShowMessage("لا توجد فواتير مطابقة للفترة والطبيب المحددين.", false);
                    ClearStats();
                    return;
                }

                var result = report.CalculateReport(dt);

                lblTotalVisits.Text = result.TotalVisits.ToString("N0");
                lblNormalVisits.Text = result.NormalVisits.ToString("N0");
                lblConsultVisits.Text = result.ConsultVisits.ToString("N0");
                lblExtraVisits.Text = result.ExtraVisits.ToString("N0");
                lblTotalRevenue.Text = result.TotalRevenue.ToString("N2");
                lblDoctorShare.Text = result.TotalDoctor.ToString("N2");
                lblCenterShare.Text = result.TotalCenter.ToString("N2");
                lblDoctorPercent.Text = result.DoctorPercent.ToString("N2") + "%";
                lblCenterPercent.Text = result.CenterPercent.ToString("N2") + "%";

                if (doctorId.HasValue)
                {
                    decimal? commissionRate = report.GetDoctorCommissionRate(doctorId.Value);
                    if (commissionRate.HasValue)
                    {
                        lblDoctorCommissionRate.Visible = true;
                        lblDoctorCommissionRate.Text = $"نسبة العمولة المسجلة: {commissionRate.Value.ToString("N2")}%";
                    }
                    else
                    {
                        lblDoctorCommissionRate.Visible = false;
                    }

                    string doctorName = ddlDoctor.SelectedItem.Text;
                    lblSelectedDoctor.Text = $"تقرير الطبيب: {doctorName}";
                    lblSelectedDoctor.Visible = true;
                }
                else
                {
                    lblSelectedDoctor.Visible = false;
                    lblDoctorCommissionRate.Visible = false;
                }

                gvDetails.DataSource = result.DetailTable;
                gvDetails.DataBind();

                var serializer = new JavaScriptSerializer();
                string labelsJson = serializer.Serialize(result.Categories);
                string revenueJson = serializer.Serialize(result.RevenueByCategory);
                string visitsJson = serializer.Serialize(result.VisitsByCategory);

                string script = $@"
                    window.ChartLabels = {labelsJson};
                    window.ChartRevenue = {revenueJson};
                    window.ChartVisits = {visitsJson};
                    if (typeof initCharts === 'function') initCharts();
                ";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "ChartData", script, true);

                ShowMessage($"تم جلب {rowCount} فاتورة.", false);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
                string script = @"
                    window.ChartLabels = [];
                    window.ChartRevenue = [];
                    window.ChartVisits = [];
                    if (typeof initCharts === 'function') initCharts();
                ";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "ChartData", script, true);
            }
        }

        private void ClearStats()
        {
            lblTotalVisits.Text = "0";
            lblNormalVisits.Text = "0";
            lblConsultVisits.Text = "0";
            lblExtraVisits.Text = "0";
            lblTotalRevenue.Text = "0.00";
            lblDoctorShare.Text = "0.00";
            lblCenterShare.Text = "0.00";
            lblDoctorPercent.Text = "0%";
            lblCenterPercent.Text = "0%";
            lblSelectedDoctor.Visible = false;
            lblDoctorCommissionRate.Visible = false;
            gvDetails.DataSource = null;
            gvDetails.DataBind();
        }

        private void ShowMessage(string msg, bool isError = true)
        {
            lblMessage.Text = msg;
            lblMessage.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
        }
    }
}