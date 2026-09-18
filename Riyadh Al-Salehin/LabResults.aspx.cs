using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class LabResults : System.Web.UI.Page
    {
        private CLabResults res = new CLabResults();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        void LoadData()
        {
            gvResults.DataSource = res.GetAll();
            gvResults.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                res.RequestId = Convert.ToInt32(txtRequestId.Text);
                res.Result = txtResult.Text;
                res.NormalRange = txtNormalRange.Text;

                DateTime dt;
                DateTime.TryParse(txtResultDate.Text, out dt);
                res.ResultDate = dt;

                res.IsSMSSent = chkSMSSent.Checked;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    res.Insert();
                }
                else
                {
                    res.Id = Convert.ToInt32(hfId.Value);
                    res.Update();
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

        void Clear()
        {
            hfId.Value = "";
            txtRequestId.Text = "";
            txtResult.Text = "";
            txtNormalRange.Text = "";
            txtResultDate.Text = "";
            chkSMSSent.Checked = false;
        }

        protected void gvResults_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = res.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtRequestId.Text = dt.Rows[0]["RequestId"].ToString();
                    txtResult.Text = dt.Rows[0]["Result"].ToString();
                    txtNormalRange.Text = dt.Rows[0]["NormalRange"].ToString();
                    txtResultDate.Text = Convert.ToDateTime(dt.Rows[0]["ResultDate"]).ToString("yyyy-MM-dd");
                    chkSMSSent.Checked = Convert.ToBoolean(dt.Rows[0]["IsSMSSent"]);
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                res.Delete(id);
                LoadData();
            }
        }
    }
}