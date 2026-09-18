using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class MedicalReferrals : System.Web.UI.Page
    {
        private CMedicalReferrals refObj = new CMedicalReferrals();

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
            gvReferrals.DataSource = refObj.GetAll();
            gvReferrals.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                refObj.ExaminationId = Convert.ToInt32(txtExaminationId.Text);
                refObj.Type = txtType.Text;
                refObj.Notes = txtNotes.Text;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    refObj.Insert();
                }
                else
                {
                    refObj.Id = Convert.ToInt32(hfId.Value);
                    refObj.Update();
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
            txtExaminationId.Text = "";
            txtType.Text = "";
            txtNotes.Text = "";
        }

        protected void gvReferrals_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = refObj.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtExaminationId.Text = dt.Rows[0]["ExaminationId"].ToString();
                    txtType.Text = dt.Rows[0]["Type"].ToString();
                    txtNotes.Text = dt.Rows[0]["Notes"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                refObj.Delete(id);
                LoadData();
            }
        }
    }
}