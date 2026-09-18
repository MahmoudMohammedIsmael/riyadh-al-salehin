using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Appointments : System.Web.UI.Page
    {
        private CAppointments obj = new CAppointments();
        private CPatients patients = new CPatients();
        private CDoctors doctors = new CDoctors();
        private CMedicalCenters centers = new CMedicalCenters();
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
                LoadAppointments();
                LoadDropdowns();
            }
        }

        private void LoadAppointments()
        {
            gvAppointments.DataSource = obj.GetAll();
            gvAppointments.DataBind();
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
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
                ShowMessage("تم الحفظ ✔", "success");
                LoadAppointments();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowMessage("اختر موعد", "warning");
                return;
            }

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
                ShowMessage("تم التعديل ✔", "success");
                LoadAppointments();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowMessage("اختر موعد", "warning");
                return;
            }

            string result = obj.Delete(Convert.ToInt32(hfId.Value));

            if (result == "OK")
            {
                ShowMessage("تم الحذف ✔", "success");
                LoadAppointments();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void gvAppointments_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvAppointments.SelectedRow;

            hfId.Value = gvAppointments.DataKeys[row.RowIndex].Value.ToString();

            ddlPatient.SelectedIndex = ddlPatient.Items.IndexOf(
                ddlPatient.Items.FindByText(row.Cells[1].Text));

            ddlDoctor.SelectedIndex = ddlDoctor.Items.IndexOf(
                ddlDoctor.Items.FindByText(row.Cells[2].Text));

            ddlCenter.SelectedIndex = ddlCenter.Items.IndexOf(
                ddlCenter.Items.FindByText(row.Cells[3].Text));

            txtDate.Text = Convert.ToDateTime(row.Cells[4].Text).ToString("yyyy-MM-ddTHH:mm");
            ddlStatus.SelectedValue = row.Cells[5].Text;

            ShowMessage("", "");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadAppointments();
                return;
            }

            gvAppointments.DataSource = obj.GetByStatus(txtSearch.Text.Trim());
            gvAppointments.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfId.Value = "";
            ddlPatient.SelectedIndex = 0;
            ddlDoctor.SelectedIndex = 0;
            ddlCenter.SelectedIndex = 0;
            txtDate.Text = "";
            ddlStatus.SelectedIndex = 0;
            txtNotes.Text = "";
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;

            switch (type)
            {
                case "success":
                    lblMessage.ForeColor = Color.Green;
                    break;
                case "error":
                    lblMessage.ForeColor = Color.Red;
                    break;
                case "warning":
                    lblMessage.ForeColor = Color.Orange;
                    break;
                default:
                    lblMessage.Text = "";
                    break;
            }
        }
    }
}