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
    public partial class MedicalCenters : System.Web.UI.Page
    {
        private CMedicalCenters objCenter = new CMedicalCenters();

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
                LoadCenters();
            }
        }

        private void LoadCenters()
        {
            gvCenters.DataSource = objCenter.GetAll();
            gvCenters.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtCenterName.Text.Trim() == "")
            {
                ShowMessage("أدخل اسم المركز", "error");
                return;
            }

            objCenter.CenterName = txtCenterName.Text.Trim();
            objCenter.Phone = txtPhone.Text.Trim();
            objCenter.Address = txtAddress.Text.Trim();

            objCenter.CommissionRate =
                string.IsNullOrWhiteSpace(txtCommissionRate.Text)
                ? 0
                : Convert.ToDecimal(txtCommissionRate.Text);

            string result = objCenter.Insert();

            if (result == "OK")
            {
                ShowMessage("تم الحفظ بنجاح ✔", "success");
                ClearForm();
                LoadCenters();
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
                ShowMessage("اختر مركز أولاً", "warning");
                return;
            }

            objCenter.Id = Convert.ToInt32(hfId.Value);
            objCenter.CenterName = txtCenterName.Text.Trim();
            objCenter.Phone = txtPhone.Text.Trim();
            objCenter.Address = txtAddress.Text.Trim();

            objCenter.CommissionRate =
                string.IsNullOrWhiteSpace(txtCommissionRate.Text)
                ? 0
                : Convert.ToDecimal(txtCommissionRate.Text);

            string result = objCenter.Update();

            if (result == "OK")
            {
                ShowMessage("تم التعديل بنجاح ✔", "success");
                ClearForm();
                LoadCenters();
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
                ShowMessage("اختر مركز أولاً", "warning");
                return;
            }

            string result = objCenter.Delete(Convert.ToInt32(hfId.Value));

            if (result == "OK")
            {
                ShowMessage("تم الحذف ✔", "success");
                ClearForm();
                LoadCenters();
            }
            else
            {
                ShowMessage("لا يمكن الحذف", "error");
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
                LoadCenters();
                return;
            }

            gvCenters.DataSource =
                objCenter.Search(txtSearch.Text.Trim());

            gvCenters.DataBind();
        }

        protected void gvCenters_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvCenters.SelectedRow;

            hfId.Value =
                gvCenters.DataKeys[row.RowIndex].Value.ToString();

            txtCenterName.Text = row.Cells[1].Text;
            txtPhone.Text = row.Cells[2].Text;
            txtCommissionRate.Text = row.Cells[3].Text;

            DataTable dt = objCenter.GetById(Convert.ToInt32(hfId.Value));

            if (dt.Rows.Count > 0)
            {
                txtAddress.Text = dt.Rows[0]["Address"].ToString();
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
            txtCenterName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtCommissionRate.Text = "";
            txtSearch.Text = "";
            lblMessage.Text = "";
        }
    }
}