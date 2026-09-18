using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class AppointmentsList : System.Web.UI.Page
    {
        private CAppointments obj = new CAppointments();

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
                LoadStatistics();
                LoadAppointments();
                LoadDropdowns();
                ClearModal();
            }
        }

        private void LoadStatistics()
        {
            DataTable dt = obj.GetAll();
            int total = dt.Rows.Count;
            int todayCount = 0, pendingCount = 0, doneCount = 0;
            DateTime today = DateTime.Today;

            foreach (DataRow row in dt.Rows)
            {
                if (row["AppointmentDate"] != DBNull.Value)
                {
                    DateTime appDate = Convert.ToDateTime(row["AppointmentDate"]);
                    if (appDate.Date == today) todayCount++;
                }

                if (row["Status"] != DBNull.Value)
                {
                    string status = row["Status"].ToString();
                    if (status.Equals("pending", StringComparison.OrdinalIgnoreCase)) pendingCount++;
                    if (status.Equals("done", StringComparison.OrdinalIgnoreCase)) doneCount++;
                }
            }

            lblTotal.Text = total.ToString();
            lblToday.Text = todayCount.ToString();
            lblPending.Text = pendingCount.ToString();
            lblDone.Text = doneCount.ToString();
        }

        private void ReloadCurrentAppointments()
        {
            string search = txtSearch.Text.Trim();
            string status = ddlStatusFilter.SelectedValue;
            string from = txtDateFrom.Text.Trim();
            string to = txtDateTo.Text.Trim();
            LoadAppointments(search, status, from, to);
        }

        private void LoadAppointments(string search = null, string status = null, string from = null, string to = null)
        {
            DateTime? fromDate = null, toDate = null;

            if (!string.IsNullOrWhiteSpace(from))
            {
                DateTime parsedFrom;
                if (DateTime.TryParse(from, out parsedFrom))
                {
                    fromDate = parsedFrom;
                }
                else
                {
                    ShowMessage("تاريخ \"من\" غير صحيح.", "warning");
                }
            }

            if (!string.IsNullOrWhiteSpace(to))
            {
                DateTime parsedTo;
                if (DateTime.TryParse(to, out parsedTo))
                {
                    toDate = parsedTo;
                }
                else
                {
                    ShowMessage("تاريخ \"إلى\" غير صحيح.", "warning");
                }
            }

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                ShowMessage("تاريخ \"من\" يجب أن يكون قبل تاريخ \"إلى\".", "warning");
                fromDate = null;
                toDate = null;
            }

            DataTable dt = obj.GetFiltered(search, status, fromDate, toDate);

            if (dt == null || dt.Rows.Count == 0)
            {
                gvAppointments.DataSource = null;
                gvAppointments.DataBind();
                gvAppointments.Visible = false;
                pnlNoData.Visible = true;
                return;
            }

            gvAppointments.Visible = true;
            pnlNoData.Visible = false;

            gvAppointments.DataSource = dt;
            gvAppointments.DataBind();

            gvAppointments.UseAccessibleHeader = true;
            if (gvAppointments.HeaderRow != null)
            {
                gvAppointments.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        private void LoadDropdowns()
        {
            CPatients p = new CPatients();
            ddlPatient.DataSource = p.GetAll();
            ddlPatient.DataTextField = "PatientName";
            ddlPatient.DataValueField = "Id";
            ddlPatient.DataBind();
            ddlPatient.Items.Insert(0, new ListItem("-- اختر مريض --", ""));

            CDoctors d = new CDoctors();
            ddlDoctor.DataSource = d.GetAll();
            ddlDoctor.DataTextField = "DoctorName";
            ddlDoctor.DataValueField = "Id";
            ddlDoctor.DataBind();
            ddlDoctor.Items.Insert(0, new ListItem("-- اختر طبيب --", ""));

            CMedicalCenters mc = new CMedicalCenters();
            ddlCenter.DataSource = mc.GetAll();
            ddlCenter.DataTextField = "CenterName";
            ddlCenter.DataValueField = "Id";
            ddlCenter.DataBind();
            ddlCenter.Items.Insert(0, new ListItem("-- اختر مركز --", ""));
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAppointments(txtSearch.Text.Trim(), ddlStatusFilter.SelectedValue, txtDateFrom.Text.Trim(), txtDateTo.Text.Trim());
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatusFilter.SelectedIndex = 0;
            txtDateFrom.Text = "";
            txtDateTo.Text = "";
            LoadAppointments();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearModal();
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            lblMessage.Text = "";
            RegisterModalScript("show");
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = obj.GetById(id);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfId.Value = id.ToString();
                    ddlPatient.SelectedValue = row["PatientId"].ToString();
                    ddlDoctor.SelectedValue = row["DoctorId"].ToString();
                    ddlCenter.SelectedValue = row["CenterId"].ToString();
                    txtDate.Text = Convert.ToDateTime(row["AppointmentDate"]).ToString("yyyy-MM-ddTHH:mm");
                    ddlStatus.SelectedValue = row["Status"].ToString();
                    txtNotes.Text = row["Notes"]?.ToString();

                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    RegisterModalScript("show");
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                string result = obj.Delete(id);
                if (result == "OK")
                {
                    ShowMessage("تم الحذف بنجاح ✔", "success");
                    ReloadCurrentAppointments();
                    LoadStatistics();
                }
                else
                {
                    ShowMessage("خطأ في الحذف: " + result, "error");
                }
            }
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            string raw = hfSelectedAppointmentIds.Value;

            if (string.IsNullOrWhiteSpace(raw))
            {
                ShowMessage("لم يتم تحديد أي موعد.", "warning");
                return;
            }

            List<int> ids = new List<int>();
            foreach (string part in raw.Split(','))
            {
                int id;
                if (int.TryParse(part.Trim(), out id) && id > 0 && !ids.Contains(id))
                {
                    ids.Add(id);
                }
            }

            if (ids.Count == 0)
            {
                ShowMessage("لم يتم تحديد أي موعد صالح.", "warning");
                return;
            }

            DataTable resultDt = obj.DeleteMultiple(ids);

            int successCount = 0;
            int failCount = 0;
            foreach (DataRow row in resultDt.Rows)
            {
                bool success = Convert.ToBoolean(row["Success"]);
                if (success) successCount++;
                else failCount++;
            }

            hfSelectedAppointmentIds.Value = "";

            if (successCount > 0 && failCount == 0)
            {
                ShowMessage(successCount == 1
                    ? "تم حذف الموعد بنجاح."
                    : string.Format("تم حذف {0} مواعيد بنجاح.", successCount), "success");
            }
            else if (successCount > 0 && failCount > 0)
            {
                ShowMessage(string.Format("تم حذف {0} مواعيد، وتعذر حذف {1}.", successCount, failCount), "warning");
            }
            else
            {
                ShowMessage("لم يتم حذف أي موعد.", "error");
            }

            ReloadCurrentAppointments();
            LoadStatistics();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            obj.PatientId = Convert.ToInt32(ddlPatient.SelectedValue);
            obj.DoctorId = Convert.ToInt32(ddlDoctor.SelectedValue);
            obj.CenterId = Convert.ToInt32(ddlCenter.SelectedValue);
            obj.CreatedBy = Convert.ToInt32(Session["UserId"]);
            obj.AppointmentDate = Convert.ToDateTime(txtDate.Text);
            obj.Status = ddlStatus.SelectedValue;
            obj.Notes = txtNotes.Text;

            string result = obj.Insert();
            if (result == "OK")
            {
                ShowMessage("تم الحفظ بنجاح ✔", "success");
                ReloadCurrentAppointments();
                LoadStatistics();
                RegisterModalScript("hide");
                ClearModal();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfId.Value))
            {
                ShowMessage("اختر موعداً للتعديل", "warning");
                return;
            }
            if (!ValidateForm()) return;

            obj.Id = Convert.ToInt32(hfId.Value);
            obj.PatientId = Convert.ToInt32(ddlPatient.SelectedValue);
            obj.DoctorId = Convert.ToInt32(ddlDoctor.SelectedValue);
            obj.CenterId = Convert.ToInt32(ddlCenter.SelectedValue);
            obj.AppointmentDate = Convert.ToDateTime(txtDate.Text);
            obj.Status = ddlStatus.SelectedValue;
            obj.Notes = txtNotes.Text;

            string result = obj.Update();
            if (result == "OK")
            {
                ShowMessage("تم التعديل بنجاح ✔", "success");
                ReloadCurrentAppointments();
                LoadStatistics();
                RegisterModalScript("hide");
                ClearModal();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlPatient.SelectedValue))
            {
                ShowMessage("يرجى اختيار المريض", "warning");
                return false;
            }
            if (string.IsNullOrEmpty(ddlDoctor.SelectedValue))
            {
                ShowMessage("يرجى اختيار الطبيب", "warning");
                return false;
            }
            if (string.IsNullOrEmpty(ddlCenter.SelectedValue))
            {
                ShowMessage("يرجى اختيار المركز", "warning");
                return false;
            }
            if (string.IsNullOrEmpty(txtDate.Text))
            {
                ShowMessage("يرجى تحديد تاريخ الموعد", "warning");
                return false;
            }
            return true;
        }

        private void ClearModal()
        {
            hfId.Value = "";
            ddlPatient.SelectedIndex = 0;
            ddlDoctor.SelectedIndex = 0;
            ddlCenter.SelectedIndex = 0;
            txtDate.Text = "";
            ddlStatus.SelectedIndex = 0;
            txtNotes.Text = "";
            btnSave.Visible = true;
            btnUpdate.Visible = false;
        }

        private void RegisterModalScript(string action)
        {
            string script = @"
(function () {
    var el = document.getElementById('appointmentModal');
    if (!el || typeof bootstrap === 'undefined') { return; }
    var modal = bootstrap.Modal.getOrCreateInstance(el);
    modal." + action + @"();
})();";
            ScriptManager.RegisterStartupScript(this, GetType(), "ModalAction_" + action + "_" + DateTime.Now.Ticks, script, true);
        }

        public string GetStatusClass(string status)
        {
            switch (status.ToLower())
            {
                case "pending": return "bg-warning text-dark";
                case "confirmed": return "bg-info text-white";
                case "done": return "bg-success text-white";
                case "cancelled": return "bg-danger text-white";
                default: return "bg-secondary text-white";
            }
        }

        public string GetStatusText(string status)
        {
            switch (status.ToLower())
            {
                case "pending": return "قيد الانتظار";
                case "confirmed": return "مؤكد";
                case "done": return "منتهي";
                case "cancelled": return "ملغي";
                default: return status;
            }
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            switch (type)
            {
                case "success": lblMessage.ForeColor = Color.Green; break;
                case "error": lblMessage.ForeColor = Color.Red; break;
                case "warning": lblMessage.ForeColor = Color.Orange; break;
                default: lblMessage.Text = ""; break;
            }
        }
    }
}