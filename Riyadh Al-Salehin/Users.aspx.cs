using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Users : System.Web.UI.Page
    {
        CUsers user = new CUsers();

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
                if (!CPermissions.HasPermission("Users", "View"))
                {
                    Response.Redirect("AccessDenied.aspx");
                    return;
                }

                btnSave.Visible = CPermissions.HasPermission("Users", "Create");
                btnUpdate.Visible = CPermissions.HasPermission("Users", "Edit");
                btnDelete.Visible = CPermissions.HasPermission("Users", "Delete");


                LoadDropDowns();
                LoadUsers();
                Clear();
            }

        }
       
        private void LoadDropDowns()
        {
            CRoles cRoles = new CRoles();
            ddlRole.DataSource = cRoles.GetAll();
            ddlRole.DataBind();

            CDoctors cDoctors = new CDoctors();
            ddlDoctor.DataSource = cDoctors.GetAll();
            ddlDoctor.DataBind();

            CMedicalCenters cCenters = new CMedicalCenters();
            ddlCenter.DataSource = cCenters.GetAll();
            ddlCenter.DataBind();
        }

        private void LoadUsers()
        {
            gvUsers.DataSource = user.GetAll();
            gvUsers.DataBind();
        }

        private void Clear()
        {
            hfId.Value = "";
            txtFullName.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            ddlRole.SelectedIndex = 0;
            ddlDoctor.SelectedIndex = 0;
            ddlCenter.SelectedIndex = 0;
            chkIsActive.Checked = true;
            lblMessage.Text = "";

            txtUsername.Enabled = true; // اسم المستخدم قابل للتعديل فقط عند الإضافة
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            txtFullName.Focus();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
            LoadUsers();
        }

        private bool ValidateForm(bool isInsert)
        {
            if (txtFullName.Text.Trim() == "")
            {
                ShowError("يجب إدخال الاسم الكامل");
                return false;
            }
            if (txtUsername.Text.Trim() == "")
            {
                ShowError("يجب إدخال اسم المستخدم");
                return false;
            }
            if (isInsert && txtPassword.Text.Trim() == "")
            {
                ShowError("يجب إدخال كلمة المرور عند إضافة مستخدم جديد");
                return false;
            }
            if (ddlRole.SelectedValue == "")
            {
                ShowError("يجب اختيار الدور");
                return false;
            }
            return true;
        }

        private void ShowError(string msg)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = msg;
        }

        private void ShowSuccess(string msg)
        {
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = msg;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(isInsert: true)) return;

            user.FullName = txtFullName.Text.Trim();
            user.Username = txtUsername.Text.Trim();

            user.PasswordHash = txtPassword.Text.Trim();

            user.Email = txtEmail.Text.Trim();
            user.Phone = txtPhone.Text.Trim();
            user.RoleId = Convert.ToInt32(ddlRole.SelectedValue);
            user.DoctorId = ddlDoctor.SelectedValue == "" ? (int?)null : Convert.ToInt32(ddlDoctor.SelectedValue);
            user.CenterId = ddlCenter.SelectedValue == "" ? (int?)null : Convert.ToInt32(ddlCenter.SelectedValue);
            user.IsActive = chkIsActive.Checked;
            user.CreatedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;

            string result = user.Insert();

            if (result.ToLower().Contains("success") || result == "")
                ShowSuccess("تم إضافة المستخدم بنجاح");
            else
                ShowError(result);

            LoadUsers();
            Clear();
        }

        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(gvUsers.SelectedDataKey.Value);

            DataTable dt = user.GetById(id);
            if (dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];

            hfId.Value = row["Id"].ToString();
            txtFullName.Text = row["FullName"].ToString();
            txtUsername.Text = row["Username"].ToString();
            txtPassword.Text = ""; // لا نعرض كلمة المرور أبداً
            txtEmail.Text = row["Email"] == DBNull.Value ? "" : row["Email"].ToString();
            txtPhone.Text = row["Phone"] == DBNull.Value ? "" : row["Phone"].ToString();

            ddlRole.SelectedValue = row["RoleId"] == DBNull.Value ? "" : row["RoleId"].ToString();
            ddlDoctor.SelectedValue = row["DoctorId"] == DBNull.Value ? "" : row["DoctorId"].ToString();
            ddlCenter.SelectedValue = row["CenterId"] == DBNull.Value ? "" : row["CenterId"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]);

            txtUsername.Enabled = false; // منع تعديل اسم المستخدم بعد الإنشاء
            btnSave.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;

            lblMessage.Text = "";
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowError("يرجى اختيار مستخدم أولاً.");
                return;
            }

            if (!ValidateForm(isInsert: false)) return;

            user.Id = Convert.ToInt32(hfId.Value);
            user.FullName = txtFullName.Text.Trim();
            user.Email = txtEmail.Text.Trim();
            user.Phone = txtPhone.Text.Trim();
            user.RoleId = Convert.ToInt32(ddlRole.SelectedValue);
            user.DoctorId = ddlDoctor.SelectedValue == "" ? (int?)null : Convert.ToInt32(ddlDoctor.SelectedValue);
            user.CenterId = ddlCenter.SelectedValue == "" ? (int?)null : Convert.ToInt32(ddlCenter.SelectedValue);
            user.IsActive = chkIsActive.Checked;

            string result = user.Update();

            if (result.ToLower().Contains("success") || result == "")
                ShowSuccess("تم تعديل بيانات المستخدم بنجاح");
            else
                ShowError(result);

            if (txtPassword.Text.Trim() != "")
            {
                user.UpdatePassword(user.Id, txtPassword.Text.Trim());
            }

            LoadUsers();
            Clear();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowError("يرجى اختيار مستخدم أولاً.");
                return;
            }

            int id = Convert.ToInt32(hfId.Value);

            if (Session["UserId"] != null && Convert.ToInt32(Session["UserId"]) == id)
            {
                ShowError("لا يمكنك حذف حسابك الحالي أثناء تسجيل الدخول به.");
                return;
            }

            string result = user.Delete(id);

            if (result.ToLower().Contains("success") || result == "")
                ShowSuccess("تم حذف المستخدم بنجاح");
            else
                ShowError("تعذر حذف المستخدم، قد يكون مرتبطاً بسجلات أخرى: " + result);

            LoadUsers();
            Clear();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();

            if (search != "")
                gvUsers.DataSource = user.GetByUsername(search);
            else
                gvUsers.DataSource = user.GetAll();

            gvUsers.DataBind();
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["style"] = "cursor:pointer;";
            }
        }
    }
}