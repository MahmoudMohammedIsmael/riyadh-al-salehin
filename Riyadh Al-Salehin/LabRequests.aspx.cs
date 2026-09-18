using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class LabRequests : System.Web.UI.Page
    {
        private CLabRequests lab = new CLabRequests();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        void LoadData()
        {
            gvLabRequests.DataSource = lab.GetAll();
            gvLabRequests.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lab.ReferralId = Convert.ToInt32(txtReferralId.Text);
                lab.PatientId = Convert.ToInt32(txtPatientId.Text);
                lab.TestType = txtTestType.Text;
                lab.Status = txtStatus.Text;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    lab.Insert();
                }
                else
                {
                    lab.Id = Convert.ToInt32(hfId.Value);
                    lab.Update();
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
            txtReferralId.Text = "";
            txtPatientId.Text = "";
            txtTestType.Text = "";
            txtStatus.Text = "";
        }

        protected void gvLabRequests_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = lab.GetAll();

                DataRow[] row = dt.Select("Id=" + id);

                if (row.Length > 0)
                {
                    hfId.Value = row[0]["Id"].ToString();
                    txtReferralId.Text = row[0]["ReferralId"].ToString();
                    txtPatientId.Text = row[0]["PatientId"].ToString();
                    txtTestType.Text = row[0]["TestType"].ToString();
                    txtStatus.Text = row[0]["Status"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                lab.Delete(id);
                LoadData();
            }
        }
    }
}