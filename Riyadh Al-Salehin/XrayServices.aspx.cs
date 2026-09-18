using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class XrayServices : System.Web.UI.Page
    {
        private CXrayServices svc = new CXrayServices();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            gvServices.DataSource = svc.GetAll();
            gvServices.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                svc.XrayName = txtXrayName.Text;
                svc.Price = Convert.ToDecimal(txtPrice.Text);
                svc.Description = txtDescription.Text;
                svc.IsActive = chkIsActive.Checked;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    svc.Insert();
                }
                else
                {
                    svc.Id = Convert.ToInt32(hfId.Value);
                    svc.Update();
                }

                Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ: " + ex.Message + "')</script>");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            hfId.Value = "";
            txtXrayName.Text = "";
            txtPrice.Text = "";
            txtDescription.Text = "";
            chkIsActive.Checked = true;
        }

        protected void gvServices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = svc.GetById(id);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfId.Value = row["Id"].ToString();
                    txtXrayName.Text = row["XrayName"].ToString();
                    txtPrice.Text = row["Price"].ToString();
                    txtDescription.Text = row["Description"].ToString();
                    chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]);
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                svc.Delete(id);
                LoadData();
            }
        }
    }
}