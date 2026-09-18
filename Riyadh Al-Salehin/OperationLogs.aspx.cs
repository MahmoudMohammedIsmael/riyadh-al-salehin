using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class OperationLogs : System.Web.UI.Page
    {
        private COperationLogs log = new COperationLogs();

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
                LoadData();
            }
        }

        void LoadData()
        {
            gvLogs.DataSource = log.GetAll();
            gvLogs.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                log.UserId = Convert.ToInt32(txtUserId.Text);
                log.Action = txtAction.Text;
                log.AffectedModule = txtModule.Text;

                int rid = 0;
                int.TryParse(txtRecordId.Text, out rid);

                log.RecordId = rid;
                log.OldValue = txtOldValue.Text;
                log.NewValue = txtNewValue.Text;
                log.IPAddress = txtIP.Text;

                log.Insert();

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

        void Clear()
        {
            txtUserId.Text = "";
            txtAction.Text = "";
            txtModule.Text = "";
            txtRecordId.Text = "";
            txtOldValue.Text = "";
            txtNewValue.Text = "";
            txtIP.Text = "";
        }

        protected void gvLogs_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewRow")
            {
                DataTable dt = log.GetAll();

                DataRow[] row = dt.Select("Id=" + id);

                if (row.Length > 0)
                {
                    txtUserId.Text = row[0]["UserId"].ToString();
                    txtAction.Text = row[0]["Action"].ToString();
                    txtModule.Text = row[0]["AffectedModule"].ToString();
                    txtRecordId.Text = row[0]["RecordId"].ToString();
                    txtOldValue.Text = row[0]["OldValue"].ToString();
                    txtNewValue.Text = row[0]["NewValue"].ToString();
                    txtIP.Text = row[0]["IPAddress"].ToString();
                }
            }
        }
    }
}