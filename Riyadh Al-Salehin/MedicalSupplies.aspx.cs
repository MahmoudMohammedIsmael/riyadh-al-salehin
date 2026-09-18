using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class MedicalSupplies : System.Web.UI.Page
    {
        private CMedicalSupplies sup = new CMedicalSupplies();

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
                sup.SupplyName = txtSupplyName.Text;
                sup.Unit = txtUnit.Text;

                decimal price = 0;
                decimal.TryParse(txtPrice.Text, out price);
                sup.Price = price;

                int qty = 0;
                int.TryParse(txtQty.Text, out qty);
                sup.AvailableQuantity = qty;

                int initialQty = 0;
                if (!int.TryParse(txtInitialQty.Text, out initialQty) || initialQty <= 0)
                {
                    initialQty = qty;
                }
                sup.InitialQuantity = initialQty;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    sup.CreatedDate = DateTime.Now;
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
            txtSupplyName.Text = "";
            txtUnit.Text = "";
            txtPrice.Text = "";
            txtQty.Text = "";
            txtInitialQty.Text = "";
        }

        protected void gvSupplies_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = sup.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtSupplyName.Text = dt.Rows[0]["SupplyName"].ToString();
                    txtUnit.Text = dt.Rows[0]["Unit"].ToString();
                    txtPrice.Text = dt.Rows[0]["Price"].ToString();
                    txtQty.Text = dt.Rows[0]["AvailableQuantity"].ToString();
                    txtInitialQty.Text = dt.Rows[0]["InitialQuantity"].ToString();
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