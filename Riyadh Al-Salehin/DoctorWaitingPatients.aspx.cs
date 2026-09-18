using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Riyadh_Al_Salehin
{
    public partial class DoctorWaitingPatients : System.Web.UI.Page
    {
        WorkTable wt = new WorkTable();

        private int DoctorId
        {
            get
            {
                object obj = Session["DoctorId"];
                if (obj == null)
                    return 0;
                return Convert.ToInt32(obj);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Session["ReturnUrl"] = Request.RawUrl;
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadDoctorInfo();
                LoadPatients("");
            }
        }

        private void LoadDoctorInfo()
        {
            try
            {
                string sql = @"
SELECT D.Id, D.DoctorName
FROM Users U
INNER JOIN Doctors D ON U.DoctorId = D.Id
WHERE U.Id = @UserId";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", Session["UserId"])
                };

                DataTable dt = wt.RunSelect(sql, prm);

                if (dt.Rows.Count == 0)
                {
                    lblMessage.Text = "لا يوجد طبيب مرتبط بهذا المستخدم.";
                    gvPatients.DataSource = null;
                    gvPatients.DataBind();
                    return;
                }

                Session["DoctorId"] = dt.Rows[0]["Id"];
                lblDoctor.Text = dt.Rows[0]["DoctorName"].ToString();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }










        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPatients(txtSearch.Text.Trim());
        }

        protected void gvPatients_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Start")
            {
                try
                {
                    int appointmentId = Convert.ToInt32(e.CommandArgument);
                    string sql = "UPDATE Appointments SET Status = 'InExamination' WHERE Id = @AppointmentId";
                    var prm = new List<SqlParameter>
                    {
                        new SqlParameter("@AppointmentId", appointmentId)
                    };

                    string result = wt.RunInsDelUpd(sql, prm);

                    if (result == "OK")
                    {
                        Response.Redirect($"~/MedicalExamination.aspx?AppointmentId={appointmentId}", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        lblMessage.Text = result;
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
            }
        }


        private void LoadPatients(string search)
        {
            try
            {
                if (DoctorId == 0)
                {
                    lblMessage.Text = "⚠️ DoctorId = 0. تأكد من تسجيل الدخول.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string sql = @"
SELECT
    A.Id AS AppointmentId,
    ISNULL(NULLIF(I.QueueNumber, ''), ISNULL(D.DoctorCode, 'A') + CAST(A.QueueNumber AS NVARCHAR(20))) AS QueueNumber,
    A.AppointmentDate,
    A.Status,
    P.PatientName,
    P.Phone,
    P.InsuranceCompany,
    I.PaymentStatus,
    CASE 
        WHEN MS.ServiceName IS NOT NULL AND MS.ServiceName LIKE N'%استشارة%' 
        THEN N'استشارة'
        ELSE N'كشف'
    END AS VisitType
FROM Appointments A
INNER JOIN Patients P ON A.PatientId = P.Id
INNER JOIN Doctors D ON A.DoctorId = D.Id
INNER JOIN Invoices I ON I.AppointmentId = A.Id          -- ✅ تغيير إلى INNER JOIN
LEFT JOIN MedicalServices MS ON I.ServiceId = MS.Id
WHERE
    A.DoctorId = @DoctorId
    AND A.Status IN ('Pending', 'WaitingDoctor', 'WaitingForXrayResult', 'XrayDone')
    AND I.PaymentStatus = 'Paid'                         -- ✅ شرط الدفع
";

                if (!string.IsNullOrEmpty(search))
                    sql += " AND P.PatientName LIKE @Search";

                sql += " ORDER BY A.QueueNumber";

                var prm = new List<SqlParameter> { new SqlParameter("@DoctorId", DoctorId) };
                if (!string.IsNullOrEmpty(search))
                    prm.Add(new SqlParameter("@Search", "%" + search.Replace("'", "''") + "%"));

                DataTable dt = wt.RunSelect(sql, prm);

                gvPatients.DataSource = dt;
                gvPatients.DataBind();

                int count = dt.Rows.Count;
                lblCount.Text = count.ToString();

                if (count == 0)
                {
                    lblMessage.Text = "لا يوجد مرضى مدفوعون في الانتظار حالياً.";
                    lblMessage.ForeColor = System.Drawing.Color.Orange;
                }
                else
                {
                    lblMessage.Text = $"تم العثور على {count} مريض مدفوع في الانتظار.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

















































        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPatients(txtSearch.Text.Trim());
        }






        public DataTable GetWaitingPatients(int doctorId, string patientName)
        {
            string sql = @"
SELECT
    A.Id AS AppointmentId,
    ISNULL(NULLIF(I.QueueNumber, ''), ISNULL(D.DoctorCode, 'A') + CAST(A.QueueNumber AS NVARCHAR(20))) AS QueueNumber,
    A.AppointmentDate,
    A.Status,
    P.PatientName,
    P.Phone,
    P.InsuranceCompany,
    ISNULL(I.PaymentStatus,'UnPaid') PaymentStatus,
    CASE 
        WHEN MS.ServiceName IS NOT NULL AND MS.ServiceName LIKE N'%استشارة%' 
        THEN N'استشارة'
        ELSE N'كشف'
    END AS VisitType
FROM Appointments A
INNER JOIN Patients P ON A.PatientId = P.Id
INNER JOIN Doctors D ON A.DoctorId = D.Id
INNER JOIN Invoices I ON I.AppointmentId = A.Id          -- ✅ تغيير إلى INNER JOIN
LEFT JOIN MedicalServices MS ON I.ServiceId = MS.Id
WHERE
    A.DoctorId = @DoctorId
    AND A.Status IN ('Pending', 'WaitingDoctor')
    AND A.AppointmentDate <= GETDATE()
    AND I.PaymentStatus = 'Paid'                         -- ✅ شرط الدفع
";

            if (!string.IsNullOrEmpty(patientName))
            {
                sql += " AND P.PatientName LIKE @Search";
            }

            sql += " ORDER BY A.QueueNumber";

            var prm = new List<SqlParameter>
    {
        new SqlParameter("@DoctorId", doctorId)
    };

            if (!string.IsNullOrEmpty(patientName))
            {
                prm.Add(new SqlParameter("@Search", "%" + patientName.Replace("'", "''") + "%"));
            }

            return wt.RunSelect(sql, prm);
        }


























        public void UpdateStatus(int appointmentId, string status)
        {
            string sql = "UPDATE Appointments SET Status = @Status WHERE Id = @AppointmentId";
            var prm = new List<SqlParameter>
            {
                new SqlParameter("@Status", status),
                new SqlParameter("@AppointmentId", appointmentId)
            };
            wt.RunInsDelUpd(sql, prm);
        }
    }
}