using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class PermissionsI : System.Web.UI.Page
    {
        CPermissions permission = new CPermissions();

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
                LoadPermissions();
                Clear();
            }
        }

        private void LoadPermissions()
        {
            gvPermissions.DataSource = permission.GetAll();
            gvPermissions.DataBind();
        }

        private void Clear()
        {
            hfId.Value = "";

            txtModule.Text = "";

            txtDescription.Text = "";

            txtSearch.Text = "";

            ddlAction.SelectedIndex = 0;

            lblMessage.Text = "";

            btnSave.Enabled = true;

            btnUpdate.Enabled = false;

            btnDelete.Enabled = false;

            txtModule.Focus();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();

            LoadPermissions();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtModule.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى إدخال اسم الشاشة";
                return;
            }

            permission.Module = txtModule.Text.Trim();

            permission.Action = ddlAction.SelectedValue;

            permission.Description = txtDescription.Text.Trim();

            string result = permission.Insert();

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم حفظ الصلاحية بنجاح";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = result;
            }

            LoadPermissions();

            Clear();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى اختيار الصلاحية أولاً";
                return;
            }

            if (txtModule.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى إدخال اسم الشاشة";
                return;
            }

            permission.Id = Convert.ToInt32(hfId.Value);
            permission.Module = txtModule.Text.Trim();
            permission.Action = ddlAction.SelectedValue;
            permission.Description = txtDescription.Text.Trim();

            string result = permission.Update();

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم تعديل الصلاحية بنجاح";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = result;
            }

            LoadPermissions();
            Clear();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى اختيار الصلاحية أولاً";
                return;
            }

            string result =
                permission.Delete(Convert.ToInt32(hfId.Value));

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم حذف الصلاحية بنجاح";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = result;
            }

            LoadPermissions();
            Clear();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();

            DataTable dt = permission.GetAll();

            if (search != "")
            {
                DataView dv = dt.DefaultView;

                dv.RowFilter =
                    "Module LIKE '%" +
                    search.Replace("'", "''") +
                    "%'";

                gvPermissions.DataSource = dv;
            }
            else
            {
                gvPermissions.DataSource = dt;
            }

            gvPermissions.DataBind();
        }

        protected void gvPermissions_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvPermissions.SelectedRow;

            hfId.Value =
                gvPermissions.DataKeys[row.RowIndex].Value.ToString();

            txtModule.Text =
                Server.HtmlDecode(row.Cells[1].Text);

            string action =
                Server.HtmlDecode(row.Cells[2].Text);

            if (ddlAction.Items.FindByValue(action) != null)
                ddlAction.SelectedValue = action;

            if (row.Cells[3].Text == "&nbsp;")
                txtDescription.Text = "";
            else
                txtDescription.Text =
                    Server.HtmlDecode(row.Cells[3].Text);

            btnSave.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        protected void gvPermissions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["style"] = "cursor:pointer;";
            }
        }
    }
}