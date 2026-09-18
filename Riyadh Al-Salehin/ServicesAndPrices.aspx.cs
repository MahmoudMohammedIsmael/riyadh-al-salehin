using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class ServicesAndPrices : System.Web.UI.Page
    {
        private CServicesAndPrices srv = new CServicesAndPrices();

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
            gvServices.DataSource = srv.GetAll();
            gvServices.DataBind();
        }

        protected void txtBasePrice_TextChanged(object sender, EventArgs e)
        {
            CalculateShares();
        }

        protected void Rates_TextChanged(object sender, EventArgs e)
        {
            CalculateShares();
        }

        void CalculateShares()
        {
            decimal basePrice = 0;
            decimal doctorRate = 0;
            decimal centerRate = 0;

            decimal.TryParse(txtBasePrice.Text, out basePrice);
            decimal.TryParse(txtDoctorRate.Text, out doctorRate);
            decimal.TryParse(txtCenterRate.Text, out centerRate);

            decimal doctorShare = basePrice * (doctorRate / 100);
            decimal centerShare = basePrice * (centerRate / 100);
            decimal totalCash = doctorShare + centerShare;

            txtDoctorShare.Text = doctorShare.ToString("N2");
            txtCenterShare.Text = centerShare.ToString("N2");
            txtCashRate.Text = totalCash.ToString("N2");

            if (string.IsNullOrEmpty(txtInsuranceRate.Text))
            {
                txtInsuranceRate.Text = totalCash.ToString("N2");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                srv.ServiceName = txtServiceName.Text.Trim();
                srv.Category = txtCategory.Text.Trim();

                decimal basePrice = 0;
                decimal cash = 0;
                decimal insurance = 0;
                decimal doctorRate = 0;
                decimal centerRate = 0;

                decimal.TryParse(txtBasePrice.Text, out basePrice);
                decimal.TryParse(txtCashRate.Text, out cash);
                decimal.TryParse(txtInsuranceRate.Text, out insurance);
                decimal.TryParse(txtDoctorRate.Text, out doctorRate);
                decimal.TryParse(txtCenterRate.Text, out centerRate);

                srv.BasePrice = basePrice;
                srv.CashRate = cash;
                srv.InsuranceRate = insurance;
                srv.DoctorRate = doctorRate;
                srv.CenterRate = centerRate;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    srv.Insert();
                }
                else
                {
                    srv.Id = Convert.ToInt32(hfId.Value);
                    srv.Update();
                }

                Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ: " + ex.Message.Replace("'", "''") + "')</script>");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            hfId.Value = "";
            txtServiceName.Text = "";
            txtCategory.Text = "";
            txtBasePrice.Text = "";
            txtCashRate.Text = "";
            txtInsuranceRate.Text = "";
            txtDoctorRate.Text = "";
            txtCenterRate.Text = "";
            txtDoctorShare.Text = "";
            txtCenterShare.Text = "";
        }

        protected void gvServices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = srv.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtServiceName.Text = dt.Rows[0]["ServiceName"].ToString();
                    txtCategory.Text = dt.Rows[0]["Category"].ToString();
                    txtBasePrice.Text = dt.Rows[0]["BasePrice"].ToString();
                    txtCashRate.Text = dt.Rows[0]["CashRate"].ToString();
                    txtInsuranceRate.Text = dt.Rows[0]["InsuranceRate"].ToString();
                    txtDoctorRate.Text = (dt.Rows[0]["DoctorRate"] != DBNull.Value) ? dt.Rows[0]["DoctorRate"].ToString() : "0";
                    txtCenterRate.Text = (dt.Rows[0]["CenterRate"] != DBNull.Value) ? dt.Rows[0]["CenterRate"].ToString() : "0";

                    CalculateShares();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                srv.Delete(id);
                LoadData();
            }
        }
    }
}