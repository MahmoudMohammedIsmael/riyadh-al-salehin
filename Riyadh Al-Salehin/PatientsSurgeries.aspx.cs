using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class PatientsSurgeries : System.Web.UI.Page
    {

        private CPatients objPatient = new CPatients();

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
                LoadPatients();
            }
        }

        private void LoadPatients()
        {
            gvPatients.DataSource = objPatient.GetAll();
            gvPatients.DataBind();
        }



        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtPatientName.Text.Trim() == "")
            {
                ShowMessage("يرجى إدخال اسم المريض", "error");
                return;
            }

            objPatient.PatientName = txtPatientName.Text.Trim();
            objPatient.Phone = txtPhone.Text.Trim();
            objPatient.Address = txtAddress.Text.Trim();

            string dobText = txtDateOfBirth.Text?.Trim();
            objPatient.DateOfBirth =
                string.IsNullOrWhiteSpace(dobText)
                ? (DateTime?)null
                : Convert.ToDateTime(dobText);

            objPatient.InsuranceCompany = txtInsuranceCompany.Text.Trim();
            objPatient.InsuranceNumber = txtInsuranceNumber.Text.Trim();

            int newPatientId = objPatient.InsertAndReturnId();

            if (newPatientId > 0)
            {
                Response.Redirect(
                    "~/PatientDoctorSurgeries.aspx?pid=" + newPatientId,
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ShowMessage("تعذر الحصول على رقم المريض", "error");
            }
        }



        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowMessage("اختر مريض أولاً", "warning");
                return;
            }

            objPatient.Id = Convert.ToInt32(hfId.Value);
            objPatient.PatientName = txtPatientName.Text.Trim();
            objPatient.Phone = txtPhone.Text.Trim();
            objPatient.Address = txtAddress.Text.Trim();

            if (!string.IsNullOrEmpty(txtDateOfBirth.Text))
                objPatient.DateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text);
            else
                objPatient.DateOfBirth = null;

            objPatient.InsuranceCompany = txtInsuranceCompany.Text.Trim();
            objPatient.InsuranceNumber = txtInsuranceNumber.Text.Trim();

            string result = objPatient.Update();

            if (result == "OK")
            {
                ShowMessage("تم التعديل بنجاح", "success");
                LoadPatients();
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
                ShowMessage("اختر مريض أولاً", "warning");
                return;
            }

            string result =
                objPatient.Delete(Convert.ToInt32(hfId.Value));

            if (result == "OK")
            {
                ShowMessage("تم الحذف بنجاح", "success");
                LoadPatients();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() == "")
            {
                LoadPatients();
                return;
            }

            gvPatients.DataSource =
                objPatient.Search(txtSearch.Text.Trim());

            gvPatients.DataBind();
        }

        protected void txtSearchPatient_TextChanged(object sender, EventArgs e)
        {
            string q = txtSearchPatient.Text?.Trim() ?? "";

            if (q == "")
            {
                LoadPatients();
                return;
            }

            if (int.TryParse(q, out int pid))
            {
                DataTable dt = objPatient.GetById(pid);
                if (dt.Rows.Count > 0)
                {
                    gvPatients.DataSource = dt;
                    gvPatients.DataBind();
                }
                else
                {
                    ShowMessage("لا يوجد مريض بهذا الرقم", "warning");
                    gvPatients.DataSource = null;
                    gvPatients.DataBind();
                }
                return;
            }

            DataTable results = objPatient.Search(q);

            if (results.Rows.Count > 0)
            {
                gvPatients.DataSource = results;
                gvPatients.DataBind();
            }
            else
            {
                ShowMessage("لا توجد نتائج", "warning");
                gvPatients.DataSource = null;
                gvPatients.DataBind();
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            string q = txtSearchPatient.Text?.Trim() ?? "";

            if (q == "")
            {
                ShowMessage("يرجى إدخال رقم المريض أو الاسم", "warning");
                return;
            }

            if (int.TryParse(q, out int pid))
            {
                DataTable dt = objPatient.GetById(pid);
                if (dt.Rows.Count > 0)
                {
                    Response.Redirect("~/PatientDoctorSurgeries.aspx?pid=" + pid, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    ShowMessage("لم يتم العثور على مريض بهذا الرقم", "warning");
                    return;
                }
            }

            DataTable dtSearch = objPatient.Search(q);

            if (dtSearch.Rows.Count == 1)
            {
                int foundId = Convert.ToInt32(dtSearch.Rows[0]["Id"]);
                Response.Redirect("~/PatientDoctorSurgeries.aspx?pid=" + foundId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (gvPatients.Rows.Count == 1)
            {
                int foundId = Convert.ToInt32(gvPatients.DataKeys[0].Value);
                Response.Redirect("~/PatientDoctorSurgeries.aspx?pid=" + foundId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            ShowMessage("يرجى اختيار مريض من القائمة أو تضييق البحث", "warning");
        }

        protected void gvPatients_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = gvPatients.SelectedIndex;

            if (selectedIndex < 0)
                return;

            object key = gvPatients.DataKeys[selectedIndex]?.Value;

            if (key == null)
                return;

            int id = Convert.ToInt32(key);
            hfId.Value = id.ToString();

            DataTable dt = objPatient.GetById(id);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                txtPatientName.Text = row["PatientName"] == DBNull.Value ? "" : row["PatientName"].ToString();
                txtPhone.Text = row["Phone"] == DBNull.Value ? "" : row["Phone"].ToString();
                txtAddress.Text = row["Address"] == DBNull.Value ? "" : row["Address"].ToString();
                txtInsuranceCompany.Text = row["InsuranceCompany"] == DBNull.Value ? "" : row["InsuranceCompany"].ToString();
                txtInsuranceNumber.Text = row["InsuranceNumber"] == DBNull.Value ? "" : row["InsuranceNumber"].ToString();

                if (row["DateOfBirth"] != DBNull.Value)
                {
                    txtDateOfBirth.Text = Convert.ToDateTime(row["DateOfBirth"]).ToString("yyyy-MM-dd");
                }
                else
                {
                    txtDateOfBirth.Text = "";
                }
            }
        }

        private void ShowMessage(string msg, string type)
        {
            lblMessage.Text = msg;

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
            }
        }

        private void ClearForm()
        {
            hfId.Value = "";

            txtPatientName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtDateOfBirth.Text = "";
            txtInsuranceCompany.Text = "";
            txtInsuranceNumber.Text = "";

            lblMessage.Text = "";
        }

    }
}