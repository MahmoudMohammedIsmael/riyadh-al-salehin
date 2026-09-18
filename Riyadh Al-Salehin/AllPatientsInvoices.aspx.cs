using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class AllPatientsInvoices : Page
    {
        WorkTable wt = new WorkTable();
        CInvoiceItems items = new CInvoiceItems();
        CInvoices invoices = new CInvoices();

        protected global::System.Web.UI.WebControls.TextBox txtFromDate;
        protected global::System.Web.UI.WebControls.TextBox txtToDate;
        protected global::System.Web.UI.WebControls.TextBox txtSearchPatient;
        protected global::System.Web.UI.WebControls.TextBox txtQuantity;
        protected global::System.Web.UI.WebControls.TextBox txtUnitPrice;
        protected global::System.Web.UI.WebControls.TextBox txtTotalPrice;
        protected global::System.Web.UI.WebControls.HiddenField hfInvoiceId;
        protected global::System.Web.UI.WebControls.HiddenField hfPatientId;
        protected global::System.Web.UI.WebControls.HiddenField hfEditItemId;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedServiceId;
        protected global::System.Web.UI.WebControls.Label lblMessage;
        protected global::System.Web.UI.WebControls.Label lblPatientName;
        protected global::System.Web.UI.WebControls.Label lblDetailPatient;
        protected global::System.Web.UI.WebControls.Label lblDetailQueue;
        protected global::System.Web.UI.WebControls.Label lblDetailTotal;
        protected global::System.Web.UI.WebControls.Label lblDetailRemaining;
        protected global::System.Web.UI.WebControls.Label lblDetailDate;
        protected global::System.Web.UI.WebControls.GridView gvPatients;
        protected global::System.Web.UI.WebControls.GridView gvInvoices;
        protected global::System.Web.UI.WebControls.GridView gvItems;
        protected global::System.Web.UI.WebControls.Repeater rptServices;
        protected global::System.Web.UI.WebControls.Panel pnlPatients;
        protected global::System.Web.UI.WebControls.Panel pnlInvoices;
        protected global::System.Web.UI.WebControls.Panel pnlInvoiceDetails;
        protected global::System.Web.UI.WebControls.Button btnSearch;
        protected global::System.Web.UI.WebControls.Button btnNewInvoice;
        protected global::System.Web.UI.WebControls.Button btnSaveItem;
        protected global::System.Web.UI.WebControls.Button btnCancelEdit;
        protected global::System.Web.UI.WebControls.Button btnConfirmInvoice;
        protected global::System.Web.UI.WebControls.Button btnPrintInvoice;
        protected global::System.Web.UI.WebControls.LinkButton lnkBackToPatients;
        protected global::System.Web.UI.WebControls.LinkButton lnkBackToInvoices;

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
                txtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadServices();

                if (Request.QueryString["PatientId"] != null)
                {
                    int patientId = 0;
                    if (int.TryParse(Request.QueryString["PatientId"], out patientId))
                    {
                        hfPatientId.Value = patientId.ToString();
                        
                        if (Request.QueryString["AppointmentId"] != null)
                        {
                            int appointmentId = 0;
                            if (int.TryParse(Request.QueryString["AppointmentId"], out appointmentId))
                            {
                                DataTable dtInv = wt.RunSelect("SELECT Id FROM Invoices WHERE AppointmentId = " + appointmentId);
                                if (dtInv.Rows.Count > 0)
                                {
                                    int invoiceId = Convert.ToInt32(dtInv.Rows[0]["Id"]);
                                    ShowPatientInvoices(patientId);
                                    LoadInvoiceDetails(invoiceId);
                                }
                                else
                                {
                                    CreateInvoiceFromAppointment(appointmentId);
                                }
                            }
                            else
                            {
                                ShowPatientInvoices(patientId);
                            }
                        }
                        else
                        {
                            ShowPatientInvoices(patientId);
                        }
                    }
                    else
                    {
                        LoadPatients();
                    }
                }
                else
                {
                    LoadPatients();
                }
            }
        }

        private void CreateInvoiceFromAppointment(int appointmentId)
        {
            DataTable dtApp = wt.RunSelect("SELECT PatientId, DoctorId, CenterId, QueueNumber FROM Appointments WHERE Id = " + appointmentId);
            if (dtApp.Rows.Count > 0)
            {
                DataRow dr = dtApp.Rows[0];
                int patientId = Convert.ToInt32(dr["PatientId"]);
                int doctorId = dr["DoctorId"] != DBNull.Value ? Convert.ToInt32(dr["DoctorId"]) : 0;
                int centerId = dr["CenterId"] != DBNull.Value ? Convert.ToInt32(dr["CenterId"]) : 1;
                string queue = dr["QueueNumber"] != DBNull.Value ? dr["QueueNumber"].ToString() : "A001";

                CInvoices inv = new CInvoices();
                inv.PatientId = patientId;
                inv.DoctorId = doctorId;
                inv.AppointmentId = appointmentId;
                inv.CenterId = centerId;
                inv.QueueNumber = queue;
                inv.InvoiceDate = DateTime.Now;
                inv.TotalAmount = 0;
                inv.PaidAmount = 0;
                inv.PaymentStatus = "Unpaid";
                inv.CreatedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 0;
                
                string res = inv.Insert();
                if (res == "OK")
                {
                    int newId = inv.GetLastInvoiceId();
                    ShowPatientInvoices(patientId);
                    LoadInvoiceDetails(newId);
                }
                else
                {
                    ShowMessage("فشل إنشاء الفاتورة: " + res, true);
                    ShowPatientInvoices(patientId);
                }
            }
            else
            {
                ShowMessage("الموعد غير موجود", true);
                LoadPatients();
            }
        }

        private void LoadPatients()
        {
            DateTime fromDate, toDate;
            string fromDateStr = txtFromDate.Text.Trim();
            string toDateStr = txtToDate.Text.Trim();

            if (string.IsNullOrEmpty(fromDateStr))
                fromDateStr = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(toDateStr))
                toDateStr = DateTime.Now.ToString("yyyy-MM-dd");

            if (!DateTime.TryParseExact(fromDateStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fromDate))
            {
                ShowMessage($"تاريخ البداية غير صحيح: '{fromDateStr}'. يُرجى إدخال تاريخ بصيغة YYYY-MM-DD.", true);
                return;
            }
            if (!DateTime.TryParseExact(toDateStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out toDate))
            {
                ShowMessage($"تاريخ النهاية غير صحيح: '{toDateStr}'. يُرجى إدخال تاريخ بصيغة YYYY-MM-DD.", true);
                return;
            }

            if (fromDate > toDate)
            {
                ShowMessage("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.", true);
                return;
            }

            string searchTerm = txtSearchPatient.Text.Trim();

            string sql = @"
                SELECT
                    p.Id,
                    p.PatientName,
                    p.Phone,
                    COUNT(i.Id) AS InvoiceCount,
                    ISNULL(SUM(i.TotalAmount), 0) AS TotalSpent
                FROM Patients p
                INNER JOIN Invoices i ON p.Id = i.PatientId
                WHERE CAST(i.InvoiceDate AS DATE) >= @FromDate
                  AND CAST(i.InvoiceDate AS DATE) <= @ToDate
            ";

            if (!string.IsNullOrWhiteSpace(searchTerm))
                sql += " AND p.PatientName LIKE @Search";

            sql += " GROUP BY p.Id, p.PatientName, p.Phone ORDER BY p.PatientName";

            List<SqlParameter> prm = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                prm.Add(new SqlParameter("@Search", "%" + searchTerm + "%"));

            DataTable dt = wt.RunSelect(sql, prm);
            gvPatients.DataSource = dt;
            gvPatients.DataBind();

            if (dt.Rows.Count == 0)
                ShowMessage($"لا توجد فواتير في الفترة من {fromDate:yyyy-MM-dd} إلى {toDate:yyyy-MM-dd}", false);
            else
                ShowMessage($"تم العثور على {dt.Rows.Count} مريض لديهم فواتير في الفترة المحددة", false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPatients();
        }

        protected void gvPatients_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectPatient")
            {
                int patientId = Convert.ToInt32(e.CommandArgument);
                ShowPatientInvoices(patientId);
            }
        }

        private void ShowPatientInvoices(int patientId)
        {
            string sql = @"
                SELECT
                    Id,
                    InvoiceDate,
                    QueueNumber,
                    TotalAmount,
                    PaidAmount,
                    (TotalAmount - PaidAmount) AS Remaining
                FROM Invoices
                WHERE PatientId = @PatientId
                ORDER BY InvoiceDate DESC
            ";

            var prm = new List<SqlParameter> { new SqlParameter("@PatientId", patientId) };
            DataTable dt = wt.RunSelect(sql, prm);

            gvInvoices.DataSource = dt;
            gvInvoices.DataBind();

            DataTable patient = wt.RunSelect("SELECT PatientName FROM Patients WHERE Id = " + patientId);
            if (patient.Rows.Count > 0)
                lblPatientName.Text = patient.Rows[0]["PatientName"].ToString();

            hfPatientId.Value = patientId.ToString();

            pnlPatients.Visible = false;
            pnlInvoices.Visible = true;
            pnlInvoiceDetails.Visible = false;
        }

        protected void gvInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectInvoice")
            {
                int invoiceId = Convert.ToInt32(e.CommandArgument);
                LoadInvoiceDetails(invoiceId);
            }
        }

        protected void lnkBackToPatients_Click(object sender, EventArgs e)
        {
            pnlInvoices.Visible = false;
            pnlPatients.Visible = true;
            pnlInvoiceDetails.Visible = false;
        }

        protected void btnNewInvoice_Click(object sender, EventArgs e)
        {
            int patientId = Convert.ToInt32(hfPatientId.Value);

            string queueNumber = GenerateQueueNumber();
            string sql = @"
                INSERT INTO Invoices (PatientId, DoctorId, QueueNumber, InvoiceDate, TotalAmount, PaidAmount)
                VALUES (@PatientId, NULL, @QueueNumber, GETDATE(), 0, 0);
                SELECT SCOPE_IDENTITY();";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@PatientId", patientId),
                new SqlParameter("@QueueNumber", queueNumber)
            };
            DataTable dt = wt.RunSelect(sql, prm);
            int newInvoiceId = Convert.ToInt32(dt.Rows[0][0]);

            ShowPatientInvoices(patientId);
            LoadInvoiceDetails(newInvoiceId);
        }

        private string GenerateQueueNumber()
        {
            string sql = "SELECT MAX(Id) FROM Invoices";
            DataTable dt = wt.RunSelect(sql);
            int maxId = (dt.Rows[0][0] == DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            return "A" + (maxId + 1).ToString("D3");
        }

        private void LoadInvoiceDetails(int invoiceId)
        {
            string sql = @"
                SELECT i.Id, i.QueueNumber, i.TotalAmount, i.PaidAmount,
                       (i.TotalAmount - i.PaidAmount) AS Remaining,
                       i.InvoiceDate,
                       p.PatientName
                FROM Invoices i
                INNER JOIN Patients p ON i.PatientId = p.Id
                WHERE i.Id = @InvoiceId
            ";

            var prm = new List<SqlParameter> { new SqlParameter("@InvoiceId", invoiceId) };
            DataTable dt = wt.RunSelect(sql, prm);

            if (dt.Rows.Count == 0)
            {
                ShowMessage("الفاتورة غير موجودة", true);
                return;
            }

            DataRow row = dt.Rows[0];
            hfInvoiceId.Value = invoiceId.ToString();

            lblDetailPatient.Text = row["PatientName"].ToString();
            lblDetailQueue.Text = row["QueueNumber"].ToString();
            lblDetailTotal.Text = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
            lblDetailRemaining.Text = Convert.ToDecimal(row["Remaining"]).ToString("N2");
            lblDetailDate.Text = Convert.ToDateTime(row["InvoiceDate"]).ToString("yyyy/MM/dd HH:mm");

            pnlInvoices.Visible = false;
            pnlInvoiceDetails.Visible = true;

            ClearItemForm();
            LoadItemsGrid(invoiceId);
        }

        protected void lnkBackToInvoices_Click(object sender, EventArgs e)
        {
            pnlInvoiceDetails.Visible = false;
            pnlInvoices.Visible = true;
            int patientId = Convert.ToInt32(hfPatientId.Value);
            ShowPatientInvoices(patientId);
        }

        private void LoadItemsGrid(int invoiceId)
        {
            DataTable dt = items.GetByInvoice(invoiceId);
            gvItems.DataSource = dt;
            gvItems.DataBind();
        }

        private void LoadServices()
        {
            DataTable dt = wt.RunSelect("SELECT Id, ServiceName, BasePrice FROM ServicesAndPrices ORDER BY ServiceName");
            rptServices.DataSource = dt;
            rptServices.DataBind();
        }

        protected void btnSaveItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfInvoiceId.Value))
                return;

            if (string.IsNullOrWhiteSpace(hfSelectedServiceId.Value))
            {
                ShowMessage("من فضلك اختر الخدمة", true);
                return;
            }

            decimal qty, unitPrice;
            if (!decimal.TryParse(txtQuantity.Text, out qty) || qty <= 0)
            {
                ShowMessage("الكمية غير صحيحة", true);
                return;
            }
            if (!decimal.TryParse(txtUnitPrice.Text, out unitPrice) || unitPrice < 0)
            {
                ShowMessage("سعر الوحدة غير صحيح", true);
                return;
            }

            decimal newTotal = qty * unitPrice;
            int invoiceId = Convert.ToInt32(hfInvoiceId.Value);
            int editItemId = Convert.ToInt32(hfEditItemId.Value);

            if (editItemId > 0)
            {
                DataTable old = items.GetById(editItemId);
                if (old.Rows.Count == 0)
                {
                    ShowMessage("البند غير موجود", true);
                    return;
                }
                decimal oldTotal = Convert.ToDecimal(old.Rows[0]["TotalPrice"]);
                decimal delta = newTotal - oldTotal;

                items.Id = editItemId;
                items.Quantity = (int)qty;
                items.UnitPrice = unitPrice;
                items.TotalPrice = newTotal;

                if (items.Update() == "OK")
                {
                    invoices.AdjustTotalAmount(invoiceId, delta);
                    ShowMessage("تم تحديث البند", false);
                }
                else
                {
                    ShowMessage("فشل التحديث", true);
                    return;
                }
            }
            else
            {
                items.InvoiceId = invoiceId;
                items.ServiceId = Convert.ToInt32(hfSelectedServiceId.Value);
                items.Quantity = (int)qty;
                items.UnitPrice = unitPrice;
                items.TotalPrice = newTotal;

                if (items.Insert() == "OK")
                {
                    invoices.AdjustTotalAmount(invoiceId, newTotal);
                    ShowMessage("تمت إضافة البند", false);
                }
                else
                {
                    ShowMessage("فشل الإضافة", true);
                    return;
                }
            }

            ClearItemForm();
            LoadInvoiceDetails(invoiceId);
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);
            int invoiceId = Convert.ToInt32(hfInvoiceId.Value);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = items.GetById(itemId);
                if (dt.Rows.Count == 0) return;
                DataRow row = dt.Rows[0];

                hfEditItemId.Value = itemId.ToString();
                hfSelectedServiceId.Value = row["ServiceId"].ToString();
                txtQuantity.Text = row["Quantity"].ToString();
                txtUnitPrice.Text = Convert.ToDecimal(row["UnitPrice"]).ToString("0.00");
                txtTotalPrice.Text = Convert.ToDecimal(row["TotalPrice"]).ToString("0.00");
                btnSaveItem.Text = "💾 تحديث";
                btnCancelEdit.Visible = true;

                ClientScript.RegisterStartupScript(this.GetType(), "SelectService",
                    $"document.querySelector('.service-card[data-id=\"{row["ServiceId"]}\"]')?.click();", true);
            }
            else if (e.CommandName == "DeleteRow")
            {
                DataTable dt = items.GetById(itemId);
                if (dt.Rows.Count > 0)
                {
                    decimal oldTotal = Convert.ToDecimal(dt.Rows[0]["TotalPrice"]);
                    if (items.Delete(itemId) == "OK")
                    {
                        invoices.AdjustTotalAmount(invoiceId, -oldTotal);
                        ShowMessage("تم حذف البند", false);
                    }
                    else
                        ShowMessage("فشل الحذف", true);
                }
                LoadInvoiceDetails(invoiceId);
            }
        }

        protected void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            Response.Redirect("InvoiceItemsPrint.aspx?InvoiceId=" + hfInvoiceId.Value);
        }

        protected void btnConfirmInvoice_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfInvoiceId.Value))
            {
                ShowMessage("لا توجد فاتورة لحفظها", true);
                return;
            }

            int invoiceId = Convert.ToInt32(hfInvoiceId.Value);
            
            string updateSql = "UPDATE Invoices SET PaidAmount = TotalAmount, PaymentStatus = N'مدفوع' WHERE Id = @Id";
            var prm = new List<SqlParameter> { new SqlParameter("@Id", invoiceId) };
            string result = wt.RunInsDelUpd(updateSql, prm);
            
            if (result == "OK")
            {
                ShowMessage("تم حفظ وتثبيت الفاتورة بنجاح في النظام", false);
                LoadInvoiceDetails(invoiceId);
            }
            else
            {
                ShowMessage("فشل حفظ الفاتورة: " + result, true);
            }
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

        private void ShowMessage(string msg, bool isError)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = "d-block mt-2 " + (isError ? "text-danger" : "text-success");
        }
    }
}