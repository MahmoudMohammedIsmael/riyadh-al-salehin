using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class InvoiceItems : Page
    {
        WorkTable wt = new WorkTable();
        CInvoiceItems items = new CInvoiceItems();
        CInvoices invoices = new CInvoices();

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
                LoadServices();

                if (Request.QueryString["AppointmentId"] != null)
                {
                    int appointmentId;
                    if (int.TryParse(Request.QueryString["AppointmentId"], out appointmentId))
                    {
                        LoadInvoiceByAppointment(appointmentId);
                        return;
                    }
                }

                ShowAppointmentsList();
            }
        }

        private void ShowAppointmentsList()
        {
            pnlSearch.Visible = true;
            pnlInvoice.Visible = false;
            divSearchResults.Visible = false;

            int doctorId = 0;

            if (Session["DoctorId"] != null)
            {
                doctorId = Convert.ToInt32(Session["DoctorId"]);
            }
            else
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                string sqlDoctor = @"
            SELECT DoctorId
            FROM Users
            WHERE Id = @UserId";

                List<SqlParameter> doctorParams = new List<SqlParameter>
        {
            new SqlParameter("@UserId", userId)
        };

                DataTable dtDoctor = wt.RunSelect(sqlDoctor, doctorParams);

                if (dtDoctor.Rows.Count == 0)
                {
                    lblMessage.Text = "المستخدم غير موجود.";
                    lblMessage.CssClass = "text-danger";
                    return;
                }

                if (dtDoctor.Rows[0]["DoctorId"] == DBNull.Value)
                {
                    lblMessage.Text = "هذا المستخدم غير مرتبط بأي طبيب.";
                    lblMessage.CssClass = "text-danger";
                    return;
                }

                doctorId = Convert.ToInt32(dtDoctor.Rows[0]["DoctorId"]);

                Session["DoctorId"] = doctorId;
            }

            string sql = @"
SELECT
    A.Id AS AppointmentId,
    A.AppointmentDate,
    P.PatientName,
    D.DoctorName,
    CASE
        WHEN I.Id IS NULL THEN CAST(0 AS BIT)
        ELSE CAST(1 AS BIT)
    END AS HasInvoice
FROM Appointments A
INNER JOIN Patients P
    ON A.PatientId = P.Id
INNER JOIN Doctors D
    ON A.DoctorId = D.Id
LEFT JOIN Invoices I
    ON I.AppointmentId = A.Id
WHERE
    A.DoctorId = @DoctorId
    AND A.AppointmentDate <= GETDATE()
ORDER BY
    A.AppointmentDate DESC;";

            List<SqlParameter> prm = new List<SqlParameter>
    {
        new SqlParameter("@DoctorId", doctorId)
    };

            DataTable dt = wt.RunSelect(sql, prm);

            gvAppointments.DataSource = dt;
            gvAppointments.DataBind();
        }

        protected void lnkRefreshAppointments_Click(object sender, EventArgs e)
        {
            ShowAppointmentsList();
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAppointment")
            {
                int appointmentId = Convert.ToInt32(e.CommandArgument);
                EnsureInvoiceExists(appointmentId);
                LoadInvoiceByAppointment(appointmentId);
            }
        }

        private void EnsureInvoiceExists(int appointmentId)
        {
            string sql = "SELECT Id FROM Invoices WHERE AppointmentId = " + appointmentId;
            DataTable dt = wt.RunSelect(sql);
            if (dt.Rows.Count > 0)
                return;

            sql = "SELECT PatientId, DoctorId FROM Appointments WHERE Id = " + appointmentId;
            dt = wt.RunSelect(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("الحجز غير موجود");

            int patientId = Convert.ToInt32(dt.Rows[0]["PatientId"]);
            int doctorId = Convert.ToInt32(dt.Rows[0]["DoctorId"]);

            string queueNumber = GenerateQueueNumber();

            string insertSql = @"
INSERT INTO Invoices (PatientId, DoctorId, AppointmentId, QueueNumber, InvoiceDate, TotalAmount, PaidAmount)
VALUES (@PatientId, @DoctorId, @AppointmentId, @QueueNumber, GETDATE(), 0, 0)";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@PatientId", patientId),
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@AppointmentId", appointmentId),
                new SqlParameter("@QueueNumber", queueNumber)
            };
            wt.RunInsDelUpd(insertSql, prm);
        }

        private string GenerateQueueNumber()
        {
            string sql = "SELECT MAX(Id) FROM Invoices";
            DataTable dt = wt.RunSelect(sql);
            int maxId = (dt.Rows[0][0] == DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            return "A" + (maxId + 1).ToString("D3");
        }

        private void LoadInvoiceByAppointment(int appointmentId)
        {
            DataTable dt = wt.RunSelect(@"
        SELECT Id
        FROM Invoices
        WHERE AppointmentId = " + appointmentId);

            if (dt.Rows.Count > 0)
            {
                int invoiceId = Convert.ToInt32(dt.Rows[0]["Id"]);
                LoadInvoice(invoiceId);
            }
            else
            {
                EnsureInvoiceExists(appointmentId);
                LoadInvoiceByAppointment(appointmentId);
            }
        }

        protected void btnSearchInvoice_Click(object sender, EventArgs e)
        {
            string term = txtSearchInvoice.Text.Trim();
            if (string.IsNullOrWhiteSpace(term))
                return;

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@Term", "%" + term + "%"),
                new SqlParameter("@ExactQueue", term)
            };

            string sql = @"
SELECT TOP 20
    i.Id,
    i.QueueNumber,
    i.InvoiceDate,
    i.TotalAmount,
    p.PatientName,
    d.DoctorName
FROM Invoices i
INNER JOIN Patients p ON i.PatientId = p.Id
INNER JOIN Doctors d ON i.DoctorId = d.Id
WHERE p.PatientName LIKE @Term OR i.QueueNumber = @ExactQueue
ORDER BY i.Id DESC";

            DataTable dt = wt.RunSelect(sql, prm);
            gvSearchResults.DataSource = dt;
            gvSearchResults.DataBind();
            divSearchResults.Visible = true;
        }

        protected void gvSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectInvoice")
            {
                int invoiceId = Convert.ToInt32(e.CommandArgument);
                LoadInvoice(invoiceId);
            }
        }

        private void LoadInvoice(int invoiceId)
        {
            DataTable dt = wt.RunSelect(@"
SELECT
    i.Id,
    i.QueueNumber,
    i.TotalAmount,
    i.PaidAmount,
    (i.TotalAmount - i.PaidAmount) AS Remaining,
    p.PatientName,
    d.DoctorName
FROM Invoices i
INNER JOIN Patients p ON i.PatientId = p.Id
INNER JOIN Doctors d ON i.DoctorId = d.Id
WHERE i.Id = " + invoiceId);

            if (dt.Rows.Count == 0)
            {
                lblMessage.Text = "الفاتورة غير موجودة";
                lblMessage.CssClass = "d-block mt-2 text-danger";
                return;
            }

            DataRow row = dt.Rows[0];
            hfInvoiceId.Value = invoiceId.ToString();

            lblPatientName.Text = row["PatientName"].ToString();
            lblDoctorName.Text = row["DoctorName"].ToString();
            lblQueueNumber.Text = row["QueueNumber"].ToString();
            lblCurrentTotal.Text = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
            lblCurrentRemaining.Text = Convert.ToDecimal(row["Remaining"]).ToString("N2");

            pnlSearch.Visible = false;
            pnlInvoice.Visible = true;

            ClearItemForm();
            LoadItemsGrid(invoiceId);
            btnPrintInvoice.Visible = true;
        }

        private void LoadItemsGrid(int invoiceId)
        {
            DataTable dt = items.GetByInvoice(invoiceId);
            gvItems.DataSource = dt;
            gvItems.DataBind();
        }

        protected void lnkBackToSearch_Click(object sender, EventArgs e)
        {
            pnlInvoice.Visible = false;
            pnlSearch.Visible = true;
            gvSearchResults.DataSource = null;
            gvSearchResults.DataBind();
            txtSearchInvoice.Text = "";
            divSearchResults.Visible = false;
        }

        private void LoadServices()
        {
            DataTable dt = wt.RunSelect(
                "SELECT Id, ServiceName, BasePrice FROM ServicesAndPrices ORDER BY ServiceName");
            rptServices.DataSource = dt;
            rptServices.DataBind();
        }

        protected void btnSaveItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfInvoiceId.Value))
                return;

            if (string.IsNullOrWhiteSpace(hfSelectedServiceId.Value))
            {
                lblMessage.Text = "من فضلك اختر الخدمة";
                lblMessage.CssClass = "d-block mt-2 text-danger";
                return;
            }

            decimal qty, unitPrice;

            if (!decimal.TryParse(txtQuantity.Text, out qty) || qty <= 0)
            {
                lblMessage.Text = "الكمية غير صحيحة";
                lblMessage.CssClass = "d-block mt-2 text-danger";
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out unitPrice) || unitPrice < 0)
            {
                lblMessage.Text = "سعر الوحدة غير صحيح";
                lblMessage.CssClass = "d-block mt-2 text-danger";
                return;
            }

            decimal newTotalPrice = qty * unitPrice;
            int invoiceId = Convert.ToInt32(hfInvoiceId.Value);
            int editItemId = Convert.ToInt32(hfEditItemId.Value);

            if (editItemId > 0)
            {
                DataTable oldDt = items.GetById(editItemId);

                if (oldDt.Rows.Count == 0)
                {
                    lblMessage.Text = "تعذر إيجاد البند المطلوب تعديله";
                    lblMessage.CssClass = "d-block mt-2 text-danger";
                    return;
                }

                decimal oldTotalPrice = Convert.ToDecimal(oldDt.Rows[0]["TotalPrice"]);
                decimal delta = newTotalPrice - oldTotalPrice;

                items.Id = editItemId;
                items.Quantity = (int)qty;
                items.UnitPrice = unitPrice;
                items.TotalPrice = newTotalPrice;

                string updResult = items.Update();

                if (updResult == "OK")
                {
                    invoices.AdjustTotalAmount(invoiceId, delta);
                    lblMessage.Text = "تم تحديث البند بنجاح";
                    lblMessage.CssClass = "d-block mt-2 text-success";
                }
                else
                {
                    lblMessage.Text = updResult;
                    lblMessage.CssClass = "d-block mt-2 text-danger";
                    return;
                }
            }
            else
            {
                items.InvoiceId = invoiceId;
                items.ServiceId = Convert.ToInt32(hfSelectedServiceId.Value);
                items.Quantity = (int)qty;
                items.UnitPrice = unitPrice;
                items.TotalPrice = newTotalPrice;

                string insResult = items.Insert();

                if (insResult == "OK")
                {
                    invoices.AdjustTotalAmount(invoiceId, newTotalPrice);
                    lblMessage.Text = "تمت إضافة البند بنجاح وتحديث إجمالي الفاتورة";
                    lblMessage.CssClass = "d-block mt-2 text-success";
                }
                else
                {
                    lblMessage.Text = insResult;
                    lblMessage.CssClass = "d-block mt-2 text-danger";
                    return;
                }
            }

            ClearItemForm();
            LoadInvoice(invoiceId);
            btnPrintInvoice.Visible = true;
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);
            int invoiceId = Convert.ToInt32(hfInvoiceId.Value);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = items.GetById(itemId);

                if (dt.Rows.Count == 0)
                    return;

                DataRow row = dt.Rows[0];

                hfEditItemId.Value = itemId.ToString();
                hfSelectedServiceId.Value = row["ServiceId"].ToString();
                txtQuantity.Text = row["Quantity"].ToString();
                txtUnitPrice.Text = Convert.ToDecimal(row["UnitPrice"]).ToString("0.00");
                txtTotalPrice.Text = Convert.ToDecimal(row["TotalPrice"]).ToString("0.00");

                btnSaveItem.Text = "💾 تحديث";
                btnCancelEdit.Visible = true;
            }
            else if (e.CommandName == "DeleteRow")
            {
                DataTable dt = items.GetById(itemId);

                if (dt.Rows.Count > 0)
                {
                    decimal oldTotalPrice = Convert.ToDecimal(dt.Rows[0]["TotalPrice"]);

                    string delResult = items.Delete(itemId);

                    if (delResult == "OK")
                    {
                        invoices.AdjustTotalAmount(invoiceId, -oldTotalPrice);
                    }
                }

                LoadInvoice(invoiceId);
            }
        }

        protected void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            Response.Redirect("InvoiceItemsPrint.aspx?InvoiceId=" + hfInvoiceId.Value);
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ClearItemForm();
        }

        private void ClearItemForm()
        {
            hfEditItemId.Value = "0";
            hfSelectedServiceId.Value = "";
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "";
            txtTotalPrice.Text = "";
            btnSaveItem.Text = "💾 إضافة";
            btnCancelEdit.Visible = false;
        }
    }
}