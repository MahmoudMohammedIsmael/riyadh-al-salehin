using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class MedicalServices : System.Web.UI.Page
    {
        private CMedicalServices objService = new CMedicalServices();

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
                if (!CPermissions.Check("MedicalServices", "View"))
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                btnSave.Visible = CPermissions.Check("MedicalServices", "Add");
                btnUpdate.Visible = CPermissions.Check("MedicalServices", "Edit");
                btnDelete.Visible = CPermissions.Check("MedicalServices", "Delete");

                LoadServices();
            }
        }

        private void LoadServices()
        {
            gvServices.DataSource = objService.GetAll();
            gvServices.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("MedicalServices", "Add"))
            {
                ShowMessage("ليس لديك صلاحية الحفظ", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtServiceName.Text))
            {
                ShowMessage("يرجى إدخال اسم الخدمة", "error");
                return;
            }

            objService.ServiceName = txtServiceName.Text.Trim();
            objService.DoctorAmount = GetDecimal(txtDoctorAmount.Text);
            objService.CenterAmount = GetDecimal(txtCenterAmount.Text);
            objService.IsActive = chkIsActive.Checked;

            int newId = objService.InsertAndReturnId();

            if (newId > 0)
            {
                ShowMessage("تم الحفظ بنجاح برقم " + newId, "success");
                LoadServices();
                ClearForm();
            }
            else
            {
                ShowMessage("تعذر الحفظ، يرجى المحاولة مرة أخرى", "error");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("MedicalServices", "Edit"))
            {
                ShowMessage("ليس لديك صلاحية التعديل", "error");
                return;
            }

            if (string.IsNullOrEmpty(hfId.Value))
            {
                ShowMessage("اختر خدمة أولاً من القائمة", "warning");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtServiceName.Text))
            {
                ShowMessage("يرجى إدخال اسم الخدمة", "error");
                return;
            }

            objService.Id = Convert.ToInt32(hfId.Value);
            objService.ServiceName = txtServiceName.Text.Trim();
            objService.DoctorAmount = GetDecimal(txtDoctorAmount.Text);
            objService.CenterAmount = GetDecimal(txtCenterAmount.Text);
            objService.IsActive = chkIsActive.Checked;

            string result = objService.Update();

            if (result == "OK")
            {
                ShowMessage("تم التعديل بنجاح", "success");
                LoadServices();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("MedicalServices", "Delete"))
            {
                ShowMessage("ليس لديك صلاحية الحذف", "error");
                return;
            }

            if (string.IsNullOrEmpty(hfId.Value))
            {
                ShowMessage("اختر خدمة أولاً من القائمة", "warning");
                return;
            }

            string result = objService.Delete(Convert.ToInt32(hfId.Value));

            if (result == "OK")
            {
                ShowMessage("تم الحذف بنجاح", "success");
                LoadServices();
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
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadServices();
                return;
            }

            gvServices.DataSource = objService.Search(txtSearch.Text.Trim());
            gvServices.DataBind();
        }

        protected void gvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvServices.SelectedRow;

            hfId.Value = gvServices.DataKeys[row.RowIndex].Value.ToString();

            txtServiceName.Text = Server.HtmlDecode(row.Cells[1].Text);

            txtDoctorAmount.Text = row.Cells[2].Text.Replace(",", "");
            txtCenterAmount.Text = row.Cells[3].Text.Replace(",", "");
            txtTotalAmount.Text = row.Cells[4].Text.Replace(",", "");

            DataTable dt = objService.GetById(Convert.ToInt32(hfId.Value));
            if (dt.Rows.Count > 0)
            {
                bool? isActive = dt.Rows[0]["IsActive"] as bool?;
                chkIsActive.Checked = isActive ?? false;
            }
        }

        private decimal? GetDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (decimal.TryParse(input, out decimal result))
                return result;

            return null;
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
                default:
                    lblMessage.ForeColor = Color.Black;
                    break;
            }
        }

        private void ClearForm()
        {
            hfId.Value = "";
            txtServiceName.Text = "";
            txtDoctorAmount.Text = "";
            txtCenterAmount.Text = "";
            txtTotalAmount.Text = "";
            chkIsActive.Checked = true;
            lblMessage.Text = "";
        }
    }
}