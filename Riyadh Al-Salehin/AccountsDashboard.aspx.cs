using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class AccountsDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFilters();
                LoadDashboard();
            }
        }

        private void LoadFilters()
        {
            CAccountsReports.LoadDoctors(ddlDoctor);
            CAccountsReports.LoadCenters(ddlCenter);
            CAccountsReports.LoadDoctorTypes(ddlDoctorType);
            CAccountsReports.LoadPaymentMethods(ddlPaymentMethod);

            txtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void LoadDashboard()
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFromDate.Text, out fromDate))
                fromDate = DateTime.Now.Date;
            if (!DateTime.TryParse(txtToDate.Text, out toDate))
                toDate = DateTime.Now.Date;

            string doctorId = ddlDoctor.SelectedValue;
            string centerId = ddlCenter.SelectedValue;
            string doctorType = ddlDoctorType.SelectedValue;
            string paymentMethod = ddlPaymentMethod.SelectedValue;

            DataTable summary = CAccountsReports.GetDashboardSummary(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
            DataTable radiology = CAccountsReports.GetRadiologyReport(fromDate, toDate, doctorId, centerId, paymentMethod);
            
            decimal totalXrayPaid = 0;
            if (radiology != null && radiology.Rows.Count > 0)
            {
                foreach (DataRow r in radiology.Rows)
                {
                    totalXrayPaid += Convert.ToDecimal(r["TotalPaid"]);
                }
            }

            if (summary.Rows.Count > 0)
            {
                DataRow row = summary.Rows[0];
                decimal totalRev = Convert.ToDecimal(row["TotalRevenue"]) + totalXrayPaid;
                decimal totalPaid = Convert.ToDecimal(row["TotalPaid"]) + totalXrayPaid;
                decimal totalRemaining = Convert.ToDecimal(row["TotalRemaining"]);
                
                lblTotalRevenue.InnerHtml = totalRev.ToString("N2");
                lblTotalPaid.InnerHtml = totalPaid.ToString("N2");
                lblTotalDiscount.InnerHtml = Convert.ToDecimal(row["TotalDiscount"]).ToString("N2");
                lblTotalRemaining.InnerHtml = totalRemaining.ToString("N2");
                lblTotalDoctorCommission.InnerHtml = Convert.ToDecimal(row["TotalDoctorCommission"]).ToString("N2");
                lblTotalCenterCommission.InnerHtml = Convert.ToDecimal(row["TotalCenterCommission"]).ToString("N2");
                lblInvoiceCount.InnerHtml = Convert.ToInt32(row["InvoiceCount"]).ToString();
                lblPatientCount.InnerHtml = Convert.ToInt32(row["PatientCount"]).ToString();
                lblVisitCount.InnerHtml = Convert.ToInt32(row["InvoiceCount"]).ToString(); // نفس عدد الفواتير
                lblActiveDoctors.InnerHtml = Convert.ToInt32(row["ActiveDoctorCount"]).ToString();
                
                lblPaidInvoiceCount.InnerHtml = Convert.ToInt32(row["PaidInvoiceCount"]).ToString();
                lblUnpaidInvoiceCount.InnerHtml = Convert.ToInt32(row["UnpaidInvoiceCount"]).ToString();
                
                decimal paidInvoicesTotal = Convert.ToDecimal(row["PaidInvoicesTotal"]) + totalXrayPaid;
                lblPaidInvoicesTotal.InnerHtml = paidInvoicesTotal.ToString("N2");
            }

            DataTable doctors = CAccountsReports.GetDoctorsAccounts(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
            gvDoctors.DataSource = doctors;
            gvDoctors.DataBind();

            DataTable visitBreakdown = CAccountsReports.GetDoctorVisitBreakdown(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
            gvVisitBreakdown.DataSource = visitBreakdown;
            gvVisitBreakdown.DataBind();

            DataTable additional = CAccountsReports.GetAdditionalServicesReport(fromDate, toDate, doctorId, centerId, paymentMethod);
            gvAdditionalServices.DataSource = additional;
            gvAdditionalServices.DataBind();

            DataTable radiology = CAccountsReports.GetRadiologyReport(fromDate, toDate, doctorId, centerId, paymentMethod);
            if (radiology == null || radiology.Rows.Count == 0)
            {
                radiologyWarning.Visible = true;
                gvRadiology.Visible = false;
            }
            else
            {
                radiologyWarning.Visible = false;
                gvRadiology.Visible = true;
                gvRadiology.DataSource = radiology;
                gvRadiology.DataBind();
            }

            DataTable paymentMethods = CAccountsReports.GetPaymentMethodsSummary(fromDate, toDate, doctorId, centerId);
            gvPaymentMethods.DataSource = paymentMethods;
            gvPaymentMethods.DataBind();

            DataTable centerSummary = CAccountsReports.GetCenterSummary(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
            if (centerSummary.Rows.Count > 0)
            {
                DataRow cr = centerSummary.Rows[0];
                decimal centerRev = Convert.ToDecimal(cr["TotalRevenue"]) + totalXrayPaid;
                decimal centerPaid = Convert.ToDecimal(cr["TotalPaid"]) + totalXrayPaid;
                decimal centerRemaining = Convert.ToDecimal(cr["TotalRemaining"]);
                
                spnCenterRevenue.InnerHtml = centerRev.ToString("N2");
                spnCenterDiscount.InnerHtml = Convert.ToDecimal(cr["TotalDiscount"]).ToString("N2");
                spnCenterPaid.InnerHtml = centerPaid.ToString("N2");
                spnCenterRemaining.InnerHtml = centerRemaining.ToString("N2");
                spnCenterDoctorCommission.InnerHtml = Convert.ToDecimal(cr["TotalDoctorCommission"]).ToString("N2");
                spnCenterNet.InnerHtml = Convert.ToDecimal(cr["CenterNet"]).ToString("N2");
                spnCenterAdditional.InnerHtml = Convert.ToDecimal(cr["AdditionalTotal"]).ToString("N2");
                spnCenterRadiology.InnerHtml = totalXrayPaid.ToString("N2");
                spnCenterVisits.InnerHtml = Convert.ToInt32(cr["VisitCount"]).ToString();
                spnCenterPatients.InnerHtml = Convert.ToInt32(cr["PatientCount"]).ToString();
                spnCenterInvoices.InnerHtml = Convert.ToInt32(cr["InvoiceCount"]).ToString();
                spnCenterDoctors.InnerHtml = Convert.ToInt32(cr["ActiveDoctorCount"]).ToString();
            }

            RegisterCharts(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
        }

        private void RegisterCharts(DateTime fromDate, DateTime toDate, string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            DataTable dailyRevenue = CAccountsReports.GetDailyRevenueChart(fromDate, toDate, doctorId, centerId, paymentMethod);
            DataTable doctorRevenue = CAccountsReports.GetDoctorRevenueChart(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);
            DataTable paymentMethodData = CAccountsReports.GetPaymentMethodChart(fromDate, toDate, doctorId, centerId);
            DataTable doctorCenterPie = CAccountsReports.GetDoctorCenterPieChart(fromDate, toDate, doctorId, centerId, doctorType, paymentMethod);

            string dailyJson = Newtonsoft.Json.JsonConvert.SerializeObject(dailyRevenue);
            string doctorJson = Newtonsoft.Json.JsonConvert.SerializeObject(doctorRevenue);
            string paymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(paymentMethodData);
            string pieJson = Newtonsoft.Json.JsonConvert.SerializeObject(doctorCenterPie);

            string script = $@"
            <script>
                (function() {{
                    var dailyData = {dailyJson};
                    var doctorData = {doctorJson};
                    var paymentData = {paymentJson};
                    var pieData = {pieJson};

                    var dailyLabels = dailyData.map(r => r.DateLabel);
                    var dailyValues = dailyData.map(r => r.Revenue);
                    new Chart(document.getElementById('dailyRevenueChart'), {{
                        type: 'line',
                        data: {{ labels: dailyLabels, datasets: [{{ label: 'الإيرادات', data: dailyValues, borderColor: '#007bff', fill: false }}] }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});

                    var docLabels = doctorData.map(r => r.DoctorName);
                    var docValues = doctorData.map(r => r.Revenue);
                    new Chart(document.getElementById('doctorRevenueChart'), {{
                        type: 'bar',
                        data: {{ labels: docLabels, datasets: [{{ label: 'الإيرادات', data: docValues, backgroundColor: '#28a745' }}] }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});

                    var payLabels = paymentData.map(r => r.PaymentMethod);
                    var payValues = paymentData.map(r => r.TotalPaid);
                    new Chart(document.getElementById('paymentMethodChart'), {{
                        type: 'pie',
                        data: {{ labels: payLabels, datasets: [{{ data: payValues, backgroundColor: ['#ff6384','#36a2eb','#ffce56','#4bc0c0','#9966ff'] }}] }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});

                    var doctorTotal = pieData.length > 0 ? pieData[0].DoctorTotal : 0;
                    var centerTotal = pieData.length > 0 ? pieData[0].CenterTotal : 0;
                    new Chart(document.getElementById('doctorCenterPieChart'), {{
                        type: 'pie',
                        data: {{ labels: ['الأطباء', 'المركز'], datasets: [{{ data: [doctorTotal, centerTotal], backgroundColor: ['#007bff','#6c757d'] }}] }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});
                }})();
            </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "Charts", script);
        }

        protected void btnRefresh_Click(object sender, EventArgs e) => LoadDashboard();
        protected void btnFilter_Click(object sender, EventArgs e) => LoadDashboard();

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlDoctor.SelectedIndex = 0;
            ddlCenter.SelectedIndex = 0;
            ddlDoctorType.SelectedIndex = 0;
            ddlPaymentMethod.SelectedIndex = 0;
            LoadDashboard();
        }

        protected void gvDoctors_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DoctorDetails")
            {
                int doctorId = Convert.ToInt32(e.CommandArgument);
                ShowDoctorDetails(doctorId);
            }
        }

        private void ShowDoctorDetails(int doctorId)
        {
            DateTime fromDate, toDate;
            DateTime.TryParse(txtFromDate.Text, out fromDate);
            DateTime.TryParse(txtToDate.Text, out toDate);

            DataTable details = CAccountsReports.GetDoctorDetails(fromDate, toDate, doctorId.ToString());
            if (details.Rows.Count == 0) return;

            StringBuilder html = new StringBuilder();
            html.Append("<div class='row'>");
            html.Append($"<div class='col-md-3'><strong>اسم الطبيب:</strong> {details.Rows[0]["DoctorName"]}</div>");
            html.Append($"<div class='col-md-3'><strong>التخصص:</strong> {details.Rows[0]["Specialty"]}</div>");
            html.Append($"<div class='col-md-3'><strong>عدد المرضى:</strong> {details.Rows[0]["PatientCount"]}</div>");
            html.Append($"<div class='col-md-3'><strong>عدد الكشوفات:</strong> {details.Rows[0]["VisitCount"]}</div>");
            html.Append($"<div class='col-md-3'><strong>إجمالي الحساب:</strong> {Convert.ToDecimal(details.Rows[0]["TotalRevenue"]).ToString("N2")} ج.م</div>");
            html.Append($"<div class='col-md-3'><strong>نصيب الطبيب:</strong> {Convert.ToDecimal(details.Rows[0]["DoctorNet"]).ToString("N2")} ج.م</div>");
            html.Append($"<div class='col-md-3'><strong>نصيب المركز:</strong> {Convert.ToDecimal(details.Rows[0]["CenterNet"]).ToString("N2")} ج.م</div>");
            html.Append("</div><hr /><h5>تفاصيل الفواتير</h5>");
            html.Append("<div class='table-responsive'><table class='table table-bordered'><thead><tr>");
            html.Append("<th>رقم الفاتورة</th><th>اسم المريض</th><th>تاريخ الكشف</th><th>نوع الحجز</th>");
            html.Append("<th>الخدمة</th><th>سعر الخدمة</th><th>الخصم</th><th>الإجمالي</th>");
            html.Append("<th>المدفوع</th><th>المتبقي</th><th>طريقة الدفع</th><th>عمولة الطبيب</th><th>نصيب المركز</th>");
            html.Append("</tr></thead><tbody>");
            foreach (DataRow row in details.Rows)
            {
                html.Append("<tr>");
                html.Append($"<td>{row["InvoiceId"]}</td>");
                html.Append($"<td>{row["PatientName"]}</td>");
                html.Append($"<td>{Convert.ToDateTime(row["InvoiceDate"]).ToString("yyyy-MM-dd")}</td>");
                html.Append($"<td>{row["BookingType"]}</td>");
                html.Append($"<td>{row["ServiceName"]}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["ServicePrice"]).ToString("N2")}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["DiscountAmount"]).ToString("N2")}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["TotalAmount"]).ToString("N2")}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["PaidAmount"]).ToString("N2")}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["Remaining"]).ToString("N2")}</td>");
                html.Append($"<td>{row["PaymentMethod"]}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["DoctorCommission"]).ToString("N2")}</td>");
                html.Append($"<td>{Convert.ToDecimal(row["CenterCommission"]).ToString("N2")}</td>");
                html.Append("</tr>");
            }
            html.Append("</tbody></table></div>");

            string script = $"$('#doctorDetailsBody').html('{html.ToString().Replace("'", "\\'")}'); $('#doctorDetailsModal').modal('show');";
            ClientScript.RegisterStartupScript(this.GetType(), "DoctorDetails", script, true);
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = CAccountsReports.GetDoctorsAccounts(
                DateTime.Parse(txtFromDate.Text),
                DateTime.Parse(txtToDate.Text),
                ddlDoctor.SelectedValue,
                ddlCenter.SelectedValue,
                ddlDoctorType.SelectedValue,
                ddlPaymentMethod.SelectedValue
            );

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=AccountsReport.xls");
            Response.Charset = "utf-8";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                hw.RenderBeginTag(HtmlTextWriterTag.H1);
                hw.Write($"تقرير الحسابات المالية - {DateTime.Now:yyyy-MM-dd HH:mm}");
                hw.RenderEndTag();
                hw.Write("<br />");

                hw.RenderBeginTag(HtmlTextWriterTag.Table);
                hw.RenderBeginTag(HtmlTextWriterTag.Thead);
                hw.RenderBeginTag(HtmlTextWriterTag.Tr);
                foreach (DataColumn col in dt.Columns)
                {
                    hw.RenderBeginTag(HtmlTextWriterTag.Th);
                    hw.Write(col.ColumnName);
                    hw.RenderEndTag();
                }
                hw.RenderEndTag();
                hw.RenderBeginTag(HtmlTextWriterTag.Tbody);
                foreach (DataRow row in dt.Rows)
                {
                    hw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    foreach (object item in row.ItemArray)
                    {
                        hw.RenderBeginTag(HtmlTextWriterTag.Td);
                        hw.Write(item.ToString());
                        hw.RenderEndTag();
                    }
                    hw.RenderEndTag();
                }
                hw.RenderEndTag();
                hw.RenderEndTag();

                Response.Output.Write(sw.ToString());
            }
            Response.Flush();
            Response.End();
        }

        protected void gvDoctors_RowDataBound(object sender, GridViewRowEventArgs e) { }

        protected void gvVisitBreakdown_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int examinationCount = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ExaminationCount"));
                if (examinationCount > 10)
                {
                    e.Row.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }
    }
}