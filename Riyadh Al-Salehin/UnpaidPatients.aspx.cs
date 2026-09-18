using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Riyadh_Al_Salehin
{
    public partial class UnpaidPatients : System.Web.UI.Page
    {
        WorkTable wt = new WorkTable();

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

                BindDoctors();
                BindServices();

                LoadUnpaidPatients("", "", "", "", "", "");
            }
        }

        private void BindDoctors()
        {
            DataTable dt = wt.RunSelect("SELECT Id, DoctorName FROM Doctors ORDER BY DoctorName");
            ddlDoctor.DataSource = dt;
            ddlDoctor.DataTextField = "DoctorName";
            ddlDoctor.DataValueField = "Id";
            ddlDoctor.DataBind();
            ddlDoctor.Items.Insert(0, new ListItem("-- الكل --", ""));
        }

        private void BindServices()
        {
            DataTable dt = wt.RunSelect("SELECT Id, ServiceName FROM MedicalServices ORDER BY ServiceName");
            ddlService.DataSource = dt;
            ddlService.DataTextField = "ServiceName";
            ddlService.DataValueField = "Id";
            ddlService.DataBind();
            ddlService.Items.Insert(0, new ListItem("-- الكل --", ""));
        }

        private void LoadUnpaidPatients(
    string search,
    string invoiceNumber,   // جديد
    string doctorId,
    string serviceId,
    string dateFrom,
    string dateTo)
        {
            try
            {
                string sql = @"
    SELECT 
        I.Id AS InvoiceNumber,
        A.Id AS AppointmentId,
        ISNULL(NULLIF(I.QueueNumber, ''), ISNULL(D.DoctorCode, 'A') + CAST(A.QueueNumber AS NVARCHAR(20))) AS QueueNumber,
        ISNULL(A.AppointmentDate, I.InvoiceDate) AS AppointmentDate,
        P.PatientName,
        P.Phone,
        D.DoctorName,
        MS.ServiceName,
        I.TotalAmount,
        ISNULL(I.PaidAmount, 0) AS PaidAmount,
        (I.TotalAmount - ISNULL(I.PaidAmount, 0)) AS Remaining,
        I.PaymentStatus,
        I.ServiceId,
        I.BookingType
    FROM Invoices I
    LEFT JOIN Appointments A ON I.AppointmentId = A.Id
    LEFT JOIN Patients P ON I.PatientId = P.Id
    LEFT JOIN Doctors D ON I.DoctorId = D.Id
    LEFT JOIN MedicalServices MS ON I.ServiceId = MS.Id
    WHERE I.PaymentStatus IN ('Unpaid', 'Partial', 'Pending')
";

                List<SqlParameter> prm = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(search))
                {
                    sql += " AND P.PatientName LIKE @Search";
                    prm.Add(new SqlParameter("@Search", "%" + search.Replace("'", "''") + "%"));
                }

                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    int invNum;
                    if (int.TryParse(invoiceNumber.Trim(), out invNum))
                    {
                        sql += " AND I.Id = @InvoiceNumber";
                        prm.Add(new SqlParameter("@InvoiceNumber", invNum));
                    }
                    else
                    {
                        sql += " AND I.Id = -1";
                    }
                }

                if (!string.IsNullOrEmpty(doctorId))
                {
                    sql += " AND D.Id = @DoctorId";
                    prm.Add(new SqlParameter("@DoctorId", doctorId));
                }

                if (!string.IsNullOrEmpty(serviceId))
                {
                    sql += " AND MS.Id = @ServiceId";
                    prm.Add(new SqlParameter("@ServiceId", serviceId));
                }

                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sql += " AND CAST(ISNULL(A.AppointmentDate, I.InvoiceDate) AS DATE) >= @DateFrom";
                    prm.Add(new SqlParameter("@DateFrom", dateFrom));
                }

                if (!string.IsNullOrEmpty(dateTo))
                {
                    sql += " AND CAST(ISNULL(A.AppointmentDate, I.InvoiceDate) AS DATE) <= @DateTo";
                    prm.Add(new SqlParameter("@DateTo", dateTo));
                }

                sql += " ORDER BY ISNULL(A.AppointmentDate, I.InvoiceDate) DESC";

                DataTable dt = wt.RunSelect(sql, prm);

                gvUnpaid.DataSource = dt;
                gvUnpaid.DataBind();

                int count = dt.Rows.Count;
                lblCount.Text = count.ToString();

                if (count == 0)
                {
                    lblMessage.Text = "لا توجد فواتير غير مدفوعة تطابق معايير البحث.";
                    lblMessage.ForeColor = System.Drawing.Color.Orange;
                }
                else
                {
                    lblMessage.Text = $"تم العثور على {count} فاتورة غير مدفوعة.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUnpaidPatients(
                txtSearch.Text.Trim(),
                txtInvoiceNumber.Text.Trim(),   // ✅ رقم الفاتورة في مكانه
                ddlDoctor.SelectedValue,        // ✅ الطبيب في مكانه
                ddlService.SelectedValue,
                txtDateFrom.Text.Trim(),
                txtDateTo.Text.Trim()
            );
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlDoctor.SelectedIndex = 0;
            txtInvoiceNumber.Text = "";
            ddlService.SelectedIndex = 0;
            txtDateFrom.Text = "";
            txtDateTo.Text = "";
            LoadUnpaidPatients("", "", "", "", "", "");
        }


        protected void gvUnpaid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "PayInvoice")
            {
                try
                {
                    string[] args = e.CommandArgument.ToString().Split(',');
                    int appointmentId = Convert.ToInt32(args[0]);
                    int serviceId = Convert.ToInt32(args[1]);
                    int userId = Convert.ToInt32(Session["UserId"]);

                    Response.Redirect($"~/NewConsultationInvoice.aspx?AppointmentId={appointmentId}&ServiceId={serviceId}&uid={userId}");
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "خطأ: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected string GetBookingTypeBadge(object dataItem)
        {
            DataRowView row = dataItem as DataRowView;
            if (row == null || !row.DataView.Table.Columns.Contains("BookingType"))
                return "<span class='badge bg-secondary'>🏥 حضور شخصي</span>";

            string bookingType = row["BookingType"] == DBNull.Value ? "" : row["BookingType"].ToString();

            return bookingType == "WhatsApp"
                ? "<span class='badge bg-success'>📱 واتساب</span>"
                : "<span class='badge bg-secondary'>🏥 حضور شخصي</span>";
        }



    }
}