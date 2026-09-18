using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Threading;
using System.Web.UI;

namespace Riyadh_Al_Salehin
{
    public partial class DoctorSummary : System.Web.UI.Page
    {
        WorkTable wt = new WorkTable();

        private int DoctorId
        {
            get
            {
                object obj = Session["DoctorId"];
                if (obj == null) return 0;
                return Convert.ToInt32(obj);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                txtFrom.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                LoadDoctorInfo();
                LoadSummary();
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
                    lblDoctor.Text = Session["FullName"] != null ? Session["FullName"].ToString() : "غير محدد";
                    return;
                }

                Session["DoctorId"] = dt.Rows[0]["Id"];
                lblDoctor.Text = dt.Rows[0]["DoctorName"].ToString();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ في جلب بيانات الطبيب: " + ex.Message;
            }
        }

        private void LoadSummary()
        {
            try
            {
                if (DoctorId == 0)
                {
                    lblMessage.Text = "لم يتم تحديد الطبيب.";
                    return;
                }

                DateTime fromDate = DateTime.Today;
                DateTime toDate = DateTime.Today;

                if (!string.IsNullOrEmpty(txtFrom.Text))
                    fromDate = ParseInputDate(txtFrom.Text, fromDate);
                if (!string.IsNullOrEmpty(txtTo.Text))
                    toDate = ParseInputDate(txtTo.Text, toDate);

                DateTime sqlMin = SqlDateTime.MinValue.Value; // 1753-01-01
                DateTime sqlMax = SqlDateTime.MaxValue.Value; // 9999-12-31
                if (fromDate < sqlMin) fromDate = sqlMin;
                if (toDate < sqlMin) toDate = sqlMin;
                if (fromDate > sqlMax) fromDate = sqlMax;
                if (toDate > sqlMax) toDate = sqlMax;

                txtFrom.Text = fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                txtTo.Text = toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                string search = txtSearch.Text.Trim();

                DateTime ParseInputDate(string input, DateTime fallback)
                {
                    if (string.IsNullOrWhiteSpace(input)) return fallback;

                    string[] gregFormats = new[] { "yyyy-MM-dd", "d/M/yyyy", "dd/MM/yyyy", "M/d/yyyy", "MM/dd/yyyy" };
                    if (DateTime.TryParseExact(input, gregFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime res))
                        return res.Date;

                    try
                    {
                        CultureInfo arCulture = new CultureInfo("ar-SA");
                        arCulture.DateTimeFormat.Calendar = new UmAlQuraCalendar();

                        string[] hijriFormats = new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy" };

                        if (DateTime.TryParseExact(input, hijriFormats, arCulture, DateTimeStyles.None, out res))
                            return res.Date;
                    }
                    catch
                    {
                    }

                    if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                        return res.Date;

                    return fallback;
                }

                fromDate = ParseInputDate(txtFrom.Text, fromDate);
                toDate = ParseInputDate(txtTo.Text, toDate);

                string sql = @"
SELECT
    ROW_NUMBER() OVER (ORDER BY A.AppointmentDate, A.QueueNumber) AS RowNum,
    P.PatientName,
    CAST(A.AppointmentDate AS DATE)     AS AppointmentDate,
    CAST(A.AppointmentDate AS DATETIME) AS AppointmentTime,
    CASE
        WHEN MS.ServiceName IS NOT NULL AND MS.ServiceName LIKE N'%استشارة%'
        THEN N'استشارة'
        ELSE N'كشف'
    END AS VisitType,
    ISNULL(MS.ServiceName, N'—')        AS ServiceName,
    ISNULL((
        SELECT STRING_AGG(MS2.ServiceName, N', ')
        FROM InvoiceItems II
        INNER JOIN MedicalServices MS2 ON II.ServiceId = MS2.Id
        WHERE II.InvoiceId = I.Id 
          AND II.ServiceId != ISNULL(I.ServiceId, 0)
    ), N'—') AS ExtraServices,
    ISNULL(I.TotalAmount, 0)            AS TotalAmount,
    ISNULL(I.PaymentStatus, 'UnPaid')   AS PaymentStatus,
    ISNULL(P.InsuranceCompany, N'—')    AS InsuranceCompany
FROM Appointments A
INNER JOIN Patients P       ON A.PatientId   = P.Id
LEFT  JOIN Invoices I       ON I.AppointmentId = A.Id
LEFT  JOIN MedicalServices MS ON I.ServiceId   = MS.Id
WHERE
    A.DoctorId   = @DoctorId
    AND CAST(A.AppointmentDate AS DATE) BETWEEN @FromDate AND @ToDate
    AND LOWER(A.Status) IN ('done','finished')";

                if (!string.IsNullOrEmpty(search))
                    sql += " AND P.PatientName LIKE @Search";

                sql += " ORDER BY A.AppointmentDate DESC, A.QueueNumber";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@DoctorId", DoctorId)
                };

                var pFrom = new SqlParameter("@FromDate", System.Data.SqlDbType.Date) { Value = fromDate.Date };
                var pTo = new SqlParameter("@ToDate", System.Data.SqlDbType.Date) { Value = toDate.Date };
                prm.Add(pFrom);
                prm.Add(pTo);

                if (!string.IsNullOrEmpty(search))
                    prm.Add(new SqlParameter("@Search", "%" + search + "%"));

                DataTable dt = wt.RunSelect(sql, prm);

                gvSummary.DataSource = dt;
                gvSummary.DataBind();

                int total = dt.Rows.Count;
                int kshf = 0;
                int consult = 0;
                decimal sum = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string vt = row["VisitType"].ToString();
                    if (vt == "استشارة") consult++;
                    else kshf++;

                    sum += Convert.ToDecimal(row["TotalAmount"]);
                }

                lblTotalCount.Text = total.ToString(CultureInfo.InvariantCulture);
                lblKshfCount.Text = kshf.ToString(CultureInfo.InvariantCulture);
                lblConsultCount.Text = consult.ToString(CultureInfo.InvariantCulture);
                lblTotal.Text = sum.ToString("N2", CultureInfo.InvariantCulture);

                lblMessage.Text = total == 0
                    ? "لا توجد حالات مكتملة في هذه الفترة."
                    : $"تم عرض {total} حالة مكتملة.";

                lblMessage.ForeColor = total == 0
                    ? System.Drawing.Color.Orange
                    : System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSummary();
        }
    }
}