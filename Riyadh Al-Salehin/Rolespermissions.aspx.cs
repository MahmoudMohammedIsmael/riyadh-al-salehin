using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Rolespermissions : Page
    {
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
                BindRolesGrid();
                ClearForm();
            }
        }

        private void BindRolesGrid()
        {
            CRoles cRoles = new CRoles();
            DataTable dt;

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dt = cRoles.GetAll();
                dt.DefaultView.RowFilter =
                    "Name LIKE '%" + txtSearch.Text.Trim().Replace("'", "''") + "%'";
                dt = dt.DefaultView.ToTable();
            }
            else
            {
                dt = cRoles.GetAll();
            }

            gvRoles.DataSource = dt;
            gvRoles.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRolesGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            CRoles cRoles = new CRoles
            {
                Name = txtName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            string result = cRoles.Insert();

            if (result == "1" || string.IsNullOrEmpty(result))
            {
                ShowMessage("تم إضافة الدور بنجاح", "success");
                ClearForm();
                BindRolesGrid();
            }
            else
            {
                ShowMessage("حدث خطأ أثناء الإضافة: " + result, "danger");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int roleId = Convert.ToInt32(hfRoleId.Value);
            if (roleId <= 0)
            {
                ShowMessage("الرجاء اختيار دور من القائمة أولاً", "warning");
                return;
            }

            CRoles cRoles = new CRoles
            {
                Id = roleId,
                Name = txtName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            string result = cRoles.Update();

            if (result == "1" || string.IsNullOrEmpty(result))
            {
                ShowMessage("تم تعديل الدور بنجاح", "success");
                BindRolesGrid();
            }
            else
            {
                ShowMessage("حدث خطأ أثناء التعديل: " + result, "danger");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int roleId = Convert.ToInt32(hfRoleId.Value);
            if (roleId <= 0)
            {
                ShowMessage("الرجاء اختيار دور من القائمة أولاً", "warning");
                return;
            }

            CRoles cRoles = new CRoles();
            string result = cRoles.Delete(roleId);

            if (result == "1" || string.IsNullOrEmpty(result))
            {
                ShowMessage("تم حذف الدور بنجاح", "success");
                ClearForm();
                BindRolesGrid();
            }
            else
            {
                ShowMessage("تعذر حذف الدور، قد يكون مرتبطاً بمستخدمين أو صلاحيات: " + result, "danger");
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfRoleId.Value = "0";
            txtName.Text = "";
            txtDescription.Text = "";
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            pnlPermissions.Visible = false;
            pnlNoSelection.Visible = true;
            lblMsg.Text = "";
        }

        private void ShowMessage(string msg, string type)
        {
            lblMsg.Text = msg;
            lblMsg.CssClass = "d-block mt-3 alert alert-" + type + " py-2 mb-0";
        }

        protected void gvRoles_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            int roleId = Convert.ToInt32(gvRoles.DataKeys[e.NewSelectedIndex].Value);

            CRoles cRoles = new CRoles();
            DataTable dt = cRoles.GetById(roleId);

            if (dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];

            hfRoleId.Value = row["Id"].ToString();
            txtName.Text = row["Name"].ToString();
            txtDescription.Text = row["Description"] == DBNull.Value ? "" : row["Description"].ToString();

            btnSave.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;

            lblSelectedRole.Text = txtName.Text;
            BindPermissionsPanel(roleId);

            pnlPermissions.Visible = true;
            pnlNoSelection.Visible = false;
        }

        private void BindPermissionsPanel(int roleId)
        {
            CPermissions cPermissions = new CPermissions();
            DataTable allPerms = cPermissions.GetAll();

            if (allPerms.Rows.Count == 0)
            {
                rptModules.DataSource = null;
                rptModules.DataBind();
                lblNoModules.Visible = true;
                return;
            }

            lblNoModules.Visible = false;

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

            int roleId = Convert.ToInt32(hfRoleId.Value);

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
                DataRow permRow = modulePerms.Rows.Find(Convert.ToInt32(item.Value)) ??
                                   modulePerms.AsEnumerable()
                                       .FirstOrDefault(r => r["Id"].ToString() == item.Value);

                string action = permRow != null ? permRow["Action"].ToString() : item.Text;
                string key = module + "|" + action;

                item.Selected = grantedKeys.Contains(key);
            }
        }

        protected void btnSavePermissions_Click(object sender, EventArgs e)
        {
            int roleId = Convert.ToInt32(hfRoleId.Value);
            if (roleId <= 0)
            {
                ShowMessage("الرجاء اختيار دور أولاً", "warning");
                return;
            }

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

            ShowMessage($"تم حفظ صلاحيات الدور بنجاح ({insertedCount} صلاحية)", "success");

            BindPermissionsPanel(roleId);
        }
    }
}