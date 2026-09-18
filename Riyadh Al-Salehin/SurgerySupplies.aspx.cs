using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class SurgerySupplies : System.Web.UI.Page
    {
        private CSurgerySupplies sup = new CSurgerySupplies();

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
            gvSupplies.DataSource = sup.GetAll();
            gvSupplies.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                sup.SurgeryId = Convert.ToInt32(txtSurgeryId.Text);
                sup.SupplyId = Convert.ToInt32(txtSupplyId.Text);

                int qty = 0;
                decimal unit = 0;
                decimal total = 0;

                int.TryParse(txtUsedQuantity.Text, out qty);
                decimal.TryParse(txtUnitPrice.Text, out unit);
                decimal.TryParse(txtTotalPrice.Text, out total);

                sup.UsedQuantity = qty;
                sup.UnitPrice = unit;
                sup.TotalPrice = total;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    sup.Insert();
                }
                else
                {
                    sup.Id = Convert.ToInt32(hfId.Value);
                    sup.Update();
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
            txtSurgeryId.Text = "";
            txtSupplyId.Text = "";
            txtUsedQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtTotalPrice.Text = "";
        }

        protected void gvSupplies_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = sup.GetBySurgery(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtSurgeryId.Text = dt.Rows[0]["SurgeryId"].ToString();
                    txtSupplyId.Text = dt.Rows[0]["SupplyId"].ToString();
                    txtUsedQuantity.Text = dt.Rows[0]["UsedQuantity"].ToString();
                    txtUnitPrice.Text = dt.Rows[0]["UnitPrice"].ToString();
                    txtTotalPrice.Text = dt.Rows[0]["TotalPrice"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                sup.Delete(id);
                LoadData();
            }
        }
    }
}