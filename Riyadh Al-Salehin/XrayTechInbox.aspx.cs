using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class XrayTechInbox : System.Web.UI.Page
    {
        private CXrayRequests req = new CXrayRequests();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadData("Pending");
        }

        private void LoadData(string status)
        {
            DataTable dt;
            if (status == "All")
                dt = req.GetRequestsForTech(null);   
            else
                dt = req.GetRequestsForTech(status);

            gvTechRequests.DataSource = dt;
            gvTechRequests.DataBind();
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(ddlFilter.SelectedValue);
        }

        protected void gvTechRequests_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "StartProcessing")
            {
                string result = req.UpdateStatus(id, "InProgress");
                if (result == "OK")
                {
                    lblMessage.Text = "✅ تم بدء تنفيذ الطلب رقم " + id;
                    lblMessage.Visible = true;
                    LoadData(ddlFilter.SelectedValue);
                }
                else
                {
                    lblMessage.Text = "❌ خطأ: " + result;
                    lblMessage.Visible = true;
                }
            }
            else if (e.CommandName == "EnterResult")
            {
                Response.Redirect($"XrayResultEntry.aspx?RequestId={id}");
            }
        }
    }
}