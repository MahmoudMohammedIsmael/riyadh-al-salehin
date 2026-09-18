using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class CommissionRules : System.Web.UI.Page
    {
        private CCommissionRules rule = new CCommissionRules();

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
            gvCommission.DataSource = rule.GetAll();
            gvCommission.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                rule.DoctorId = Convert.ToInt32(txtDoctorId.Text);
                rule.CenterId = Convert.ToInt32(txtCenterId.Text);
                rule.ServiceType = txtServiceType.Text;

                decimal doctor = 0, center = 0;

                decimal.TryParse(txtDoctorShare.Text, out doctor);
                decimal.TryParse(txtCenterShare.Text, out center);

                rule.DoctorShare = doctor;
                rule.CenterShare = center;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    rule.Insert();
                }
                else
                {
                    rule.Id = Convert.ToInt32(hfId.Value);
                    rule.Update();
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
            txtDoctorId.Text = "";
            txtCenterId.Text = "";
            txtServiceType.Text = "";
            txtDoctorShare.Text = "";
            txtCenterShare.Text = "";
        }

        protected void gvCommission_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = rule.GetAll();

                DataRow[] row = dt.Select("Id=" + id);

                if (row.Length > 0)
                {
                    hfId.Value = row[0]["Id"].ToString();
                    txtDoctorId.Text = row[0]["DoctorId"].ToString();
                    txtCenterId.Text = row[0]["CenterId"].ToString();
                    txtServiceType.Text = row[0]["ServiceType"].ToString();
                    txtDoctorShare.Text = row[0]["DoctorShare"].ToString();
                    txtCenterShare.Text = row[0]["CenterShare"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                rule.Delete(id);
                LoadData();
            }
        }
    }
}