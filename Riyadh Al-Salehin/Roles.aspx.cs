using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Roles : System.Web.UI.Page
    {
        CRoles role = new CRoles();

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
                LoadRoles();
                Clear();
            }
        }

        private void LoadRoles()
        {
            gvRoles.DataSource = role.GetAll();
            gvRoles.DataBind();
        }

        private void Clear()
        {
            hfId.Value = "";

            txtName.Text = "";

            txtDescription.Text = "";

            txtSearch.Text = "";

            lblMessage.Text = "";

            btnSave.Enabled = true;

            btnUpdate.Enabled = false;

            btnDelete.Enabled = false;

            pnlPermissions.Visible = false;

            txtName.Focus();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();

            LoadRoles();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يجب إدخال اسم الدور";
                return;
            }

            role.Name = txtName.Text.Trim();

            role.Description = txtDescription.Text.Trim();

            string result = role.Insert();

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;

                lblMessage.Text = "تم حفظ الدور بنجاح";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = result;
            }

            LoadRoles();

            Clear();
        }

        protected void gvRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvRoles.SelectedRow;

            hfId.Value = gvRoles.DataKeys[row.RowIndex].Value.ToString();

            txtName.Text = row.Cells[1].Text;

            if (row.Cells[2].Text == "&nbsp;")
                txtDescription.Text = "";
            else
                txtDescription.Text = Server.HtmlDecode(row.Cells[2].Text);

            btnSave.Enabled = false;

            btnUpdate.Enabled = true;

            btnDelete.Enabled = true;

            int roleId = Convert.ToInt32(hfId.Value);
            lblSelectedRoleName.Text = txtName.Text;
            BindPermissionsPanel(roleId);
            pnlPermissions.Visible = true;
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى اختيار الدور أولاً.";
                return;
            }

            if (txtName.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى إدخال اسم الدور.";
                return;
            }

            role.Id = Convert.ToInt32(hfId.Value);
            role.Name = txtName.Text.Trim();
            role.Description = txtDescription.Text.Trim();

            string result = role.Update();

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم تعديل الدور بنجاح.";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = result;
            }

            LoadRoles();
            Clear();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى اختيار الدور أولاً.";
                return;
            }

            int id = Convert.ToInt32(hfId.Value);

            string result = role.Delete(id);

            if (result.ToLower().Contains("success") || result == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم حذف الدور بنجاح.";
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "تعذر حذف الدور، قد يكون مرتبطاً بمستخدمين أو صلاحيات: " + result;
            }

            LoadRoles();
            Clear();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();

            DataTable dt = role.GetAll();

            if (search != "")
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Name LIKE '%" + search.Replace("'", "''") + "%'";
                gvRoles.DataSource = dv;
            }
            else
            {
                gvRoles.DataSource = dt;
            }

            gvRoles.DataBind();
        }

        protected void gvRoles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["style"] = "cursor:pointer;";
            }
        }

        private void BindPermissionsPanel(int roleId)
        {
            CPermissions cPermissions = new CPermissions();
            DataTable allPerms = cPermissions.GetAll();

            if (allPerms == null || allPerms.Rows.Count == 0)
            {
                rptModules.DataSource = null;
                rptModules.DataBind();
                lblNoPermissions.Visible = true;
                return;
            }

            lblNoPermissions.Visible = false;

            DataTable modulesTable = allPerms.DefaultView.ToTable(true, "Module");

            rptModules.DataSource = modulesTable;
            rptModules.DataBind();
        }

        protected void rptModules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView drv = (DataRowView)e.Item.DataItem;
            string module = drv["Module"].ToString();

            int roleId = Convert.ToInt32(hfId.Value);

            CPermissions cPermissions = new CPermissions();
            DataTable modulePerms = cPermissions.GetByModule(module);

            CRolePermissions cRolePermissions = new CRolePermissions();
            DataTable rolePerms = cRolePermissions.GetByRole(roleId);

            var grantedKeys = rolePerms.AsEnumerable()
                .Select(r => r["Module"].ToString() + "|" + r["Action"].ToString())
                .ToHashSet();

            CheckBoxList cblActions = (CheckBoxList)e.Item.FindControl("cblActions");
            cblActions.DataTextField = "Action";
            cblActions.DataValueField = "Id";
            cblActions.DataSource = modulePerms;
            cblActions.DataBind();

            foreach (ListItem item in cblActions.Items)
            {
                DataRow permRow = modulePerms.AsEnumerable()
                    .FirstOrDefault(r => r["Id"].ToString() == item.Value);

                string action = permRow != null ? permRow["Action"].ToString() : item.Text;
                string key = module + "|" + action;

                item.Selected = grantedKeys.Contains(key);
            }
        }

        protected void btnSavePermissions_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "يرجى اختيار الدور أولاً.";
                return;
            }

            int roleId = Convert.ToInt32(hfId.Value);

            CRolePermissions cRolePermissions = new CRolePermissions();

            cRolePermissions.DeleteByRole(roleId);

            int insertedCount = 0;

            foreach (RepeaterItem item in rptModules.Items)
            {
                CheckBoxList cblActions = (CheckBoxList)item.FindControl("cblActions");
                if (cblActions == null) continue;

                foreach (ListItem li in cblActions.Items)
                {
                    if (!li.Selected) continue;

                    CRolePermissions newRp = new CRolePermissions
                    {
                        RoleId = roleId,
                        PermissionId = Convert.ToInt32(li.Value)
                    };
                    newRp.Insert();
                    insertedCount++;
                }
            }

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = $"تم حفظ صلاحيات الدور بنجاح ({insertedCount} صلاحية).";

            BindPermissionsPanel(roleId);
        }
    }
}