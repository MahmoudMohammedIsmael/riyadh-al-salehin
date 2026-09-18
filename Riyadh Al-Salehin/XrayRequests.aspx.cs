using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class XrayRequests : System.Web.UI.Page
    {
        private CXrayRequests req = new CXrayRequests();
        private CXrayServices svc = new CXrayServices();
        private WorkTable wt = new WorkTable();  // <-- تم إضافة هذا السطر

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPatients();
                LoadXrayServices();
                LoadData();

                if (Request.QueryString["PatientId"] != null &&
    Request.QueryString["ReferralId"] != null)
                {
                    int patientId = Convert.ToInt32(Request.QueryString["PatientId"]);
                    int referralId = Convert.ToInt32(Request.QueryString["ReferralId"]);

                    int doctorId = 0;
                    int appointmentId = 0;

                    if (Request.QueryString["DoctorId"] != null)
                        doctorId = Convert.ToInt32(Request.QueryString["DoctorId"]);

                    if (Request.QueryString["AppointmentId"] != null)
                        appointmentId = Convert.ToInt32(Request.QueryString["AppointmentId"]);

                    string diagnosis = Request.QueryString["Diagnosis"];

                    ViewState["DoctorId"] = doctorId;
                    ViewState["AppointmentId"] = appointmentId;

                    ddlPatient.Enabled = false;
                    ddlPatient.SelectedValue = patientId.ToString();

                    txtReferralId.Text = referralId.ToString();
                    txtReferralId.ReadOnly = true;

                    ddlStatus.SelectedValue = "Pending";

                    lblInfo.Text =
                        "تم إنشاء الطلب من الكشف الطبي" +
                        "<br/>التشخيص : " + diagnosis;

                    lblInfo.Visible = true;

                    btnNew.Enabled = false;

                    DataTable existing = req.GetByReferralId(referralId);

                    if (existing.Rows.Count > 0)
                    {
                        DataRow row = existing.Rows[0];

                        hfId.Value = row["Id"].ToString();

                        ddlXrayService.SelectedValue =
                            row["XrayServiceId"].ToString();

                        ddlStatus.SelectedValue =
                            row["Status"].ToString();

                        lblInfo.Text +=
                            "<br/>هذا الطلب موجود مسبقاً وسيتم تعديله.";
                    }
                    else
                    {
                        hfId.Value = "";
                    }
                }
                else
                {
                    lblInfo.Visible = false;
                    txtReferralId.ReadOnly = false;
                    ddlPatient.Enabled = true;
                    btnNew.Enabled = true;
                }
            }
        }

        private void LoadPatients()
        {
            CPatients pat = new CPatients();
            DataTable dt = pat.GetAll();
            ddlPatient.DataSource = dt;
            ddlPatient.DataBind();
            ddlPatient.Items.Insert(0, new ListItem("-- اختر المريض --", "0"));
        }

        private void LoadXrayServices()
        {
            DataTable dt = svc.GetAll();
            ddlXrayService.DataSource = dt;
            ddlXrayService.DataBind();
            ddlXrayService.Items.Insert(0, new ListItem("-- اختر الخدمة --", "0"));
        }

        private void LoadData()
        {
            gvRequests.DataSource = req.GetAll();
            gvRequests.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                req.PatientId = Convert.ToInt32(ddlPatient.SelectedValue);
                req.XrayServiceId = Convert.ToInt32(ddlXrayService.SelectedValue);
                req.Status = ddlStatus.SelectedValue;

                if (!string.IsNullOrEmpty(txtReferralId.Text))
                    req.ReferralId = Convert.ToInt32(txtReferralId.Text);
                else
                    req.ReferralId = null;

                string result = "";

                if (!string.IsNullOrEmpty(hfId.Value))
                {
                    req.Id = Convert.ToInt32(hfId.Value);

                    DataTable oldDt = req.GetById(req.Id);

                    if (oldDt.Rows.Count > 0)
                    {
                        req.AppointmentId =
                            oldDt.Rows[0]["AppointmentId"] != DBNull.Value
                            ? Convert.ToInt32(oldDt.Rows[0]["AppointmentId"])
                            : (int?)null;

                        req.DoctorId =
                            oldDt.Rows[0]["DoctorId"] != DBNull.Value
                            ? Convert.ToInt32(oldDt.Rows[0]["DoctorId"])
                            : (int?)null;

                        req.IsBilled =
                            Convert.ToBoolean(oldDt.Rows[0]["IsBilled"]);
                    }

                    result = req.Update();
                }
                else
                {

                    if (ViewState["DoctorId"] != null)
                        req.DoctorId = Convert.ToInt32(ViewState["DoctorId"]);

                    if (ViewState["AppointmentId"] != null)
                        req.AppointmentId = Convert.ToInt32(ViewState["AppointmentId"]);

                    req.IsBilled = false;

                    result = req.Insert();
                }

                if (result == "OK")
                {
                    if (ViewState["AppointmentId"] != null)
                    {
                        Response.Redirect(
                            "~/MedicalExamination.aspx?AppointmentId=" +
                            ViewState["AppointmentId"].ToString());

                        return;
                    }

                    Clear();
                    LoadData();
                }
                else
                {
                    Response.Write("<script>alert('خطأ : " + result + "');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ : " + ex.Message + "');</script>");
            }
        }








        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            hfId.Value = "";
            ddlPatient.SelectedIndex = 0;
            ddlXrayService.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtReferralId.Text = "";
        }

        protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = req.GetById(id);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfId.Value = row["Id"].ToString();
                    ddlPatient.SelectedValue = row["PatientId"].ToString();
                    ddlXrayService.SelectedValue = row["XrayServiceId"].ToString();
                    ddlStatus.SelectedValue = row["Status"].ToString();
                    txtReferralId.Text = row["ReferralId"] != DBNull.Value ? row["ReferralId"].ToString() : "";
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                req.Delete(id);
                LoadData();
            }
        }
    }
}