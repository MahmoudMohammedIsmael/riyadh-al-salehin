using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Riyadh_Al_Salehin
{
    public partial class XrayReportsView : System.Web.UI.Page
    {
        WorkTable wt = new WorkTable();
        private int _appointmentId = 0; // القيمة الافتراضية

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
                if (Request.QueryString["PatientId"] == null)
                {
                    lblMessage.Text = "لم يتم تحديد المريض.";
                    lblMessage.Visible = true;
                    return;
                }

                int patientId = 0;
                if (!int.TryParse(Request.QueryString["PatientId"], out patientId))
                {
                    lblMessage.Text = "رقم المريض غير صحيح.";
                    lblMessage.Visible = true;
                    return;
                }

                string appointmentIdStr = Request.QueryString["AppointmentId"];
                if (!string.IsNullOrEmpty(appointmentIdStr))
                {
                    int.TryParse(appointmentIdStr, out _appointmentId);
                }

                LoadReports(patientId);
            }
        }

        private void LoadReports(int patientId)
        {
            try
            {
                string sql = @"
SELECT 
    r.Id AS RequestId,
    s.XrayName,
    r.CreatedAt AS RequestDate,
    res.ResultText,
    res.FilePath,
    d.DoctorName
FROM XrayRequests r
INNER JOIN XrayServices s ON r.XrayServiceId = s.Id
INNER JOIN XrayResults res ON res.RequestId = r.Id
LEFT JOIN Doctors d ON r.DoctorId = d.Id
WHERE r.PatientId = @PatientId
ORDER BY r.CreatedAt DESC";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@PatientId", patientId)
                };

                DataTable dt = wt.RunSelect(sql, prm);

                gvReports.DataSource = dt;
                gvReports.DataBind();

                if (dt.Rows.Count == 0)
                {
                    lblMessage.Text = "لا توجد تقارير أشعة لهذا المريض.";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (_appointmentId > 0)
            {
                Response.Redirect($"~/MedicalExamination.aspx?AppointmentId={_appointmentId}");
            }
            else
            {
                lblMessage.Text = "لا يوجد موعد محدد للعودة، سيتم التوجيه إلى قائمة الانتظار.";
                lblMessage.Visible = true;
                Response.Redirect("~/DoctorWaitingPatients.aspx");
            }
        }
    }
}