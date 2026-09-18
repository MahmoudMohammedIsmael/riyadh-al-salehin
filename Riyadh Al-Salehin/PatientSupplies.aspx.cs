using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class PatientSupplies : System.Web.UI.Page
    {
        private CPatientSupplies ps = new CPatientSupplies();
        private CMedicalSupplies ms = new CMedicalSupplies();
        private WorkTable wt = new WorkTable();

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
                if (Request.QueryString["sid"] != null)
                    txtSurgeryId.Text = Request.QueryString["sid"];

                if (Request.QueryString["pid"] != null)
                    txtPatientId.Text = Request.QueryString["pid"];

                LoadMedicalSupplies();
                LoadPatientSupplies();
            }
        }

        /// <summary>
        /// تحميل جميع المستلزمات الموجودة بالمخزن
        /// </summary>
        private void LoadMedicalSupplies()
        {
            rptMedicalSupplies.DataSource = ms.GetAll();
            rptMedicalSupplies.DataBind();
        }

        /// <summary>
        /// تحميل مستلزمات المريض الحالية
        /// </summary>
        private void LoadPatientSupplies()
        {
            if (string.IsNullOrEmpty(txtPatientId.Text))
                return;

            DataTable dt = ps.GetByPatient(Convert.ToInt32(txtPatientId.Text));

            gvPatientSupplies.DataSource = dt;
            gvPatientSupplies.DataBind();
        }

        protected void rptMedicalSupplies_ItemCommand(object source,
     RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AddSupply")
                return;

            int supplyId = Convert.ToInt32(e.CommandArgument);

            TextBox txtQty =
                (TextBox)e.Item.FindControl("txtQty");

            int qty = 0;

            if (!int.TryParse(txtQty.Text, out qty))
            {
                Response.Write("<script>alert('أدخل كمية صحيحة');</script>");
                return;
            }

            SaveSupply(supplyId, qty);
        }
        private void SaveSupply(int supplyId, int qty)
        {
            try
            {
                DataTable dt = wt.RunSelect(
                @"SELECT *
          FROM MedicalSupplies
          WHERE Id=" + supplyId);

                if (dt.Rows.Count == 0)
                {
                    Response.Write("<script>alert('المستلزم غير موجود');</script>");
                    return;
                }

                int available =
                    Convert.ToInt32(dt.Rows[0]["AvailableQuantity"]);

                decimal price =
                    Convert.ToDecimal(dt.Rows[0]["Price"]);

                if (qty > available)
                {
                    Response.Write("<script>alert('الكمية المطلوبة أكبر من الكمية الموجودة بالمخزن');</script>");
                    return;
                }


                ps.SurgeryId = Convert.ToInt32(txtSurgeryId.Text);
                ps.PatientId = Convert.ToInt32(txtPatientId.Text);
                ps.SupplyId = supplyId;
                ps.Quantity = qty;
                ps.Price = price;

                string result = ps.Insert();

                if (result != "OK")
                {
                    Response.Write("<script>alert('" + result.Replace("'", "") + "');</script>");
                    return;
                }


                List<SqlParameter> prm = new List<SqlParameter>();

                prm.Add(new SqlParameter("@SurgeryId",
                    Convert.ToInt32(txtSurgeryId.Text)));

                prm.Add(new SqlParameter("@SupplyId",
                    supplyId));

                prm.Add(new SqlParameter("@UsedQuantity",
                    qty));

                prm.Add(new SqlParameter("@UnitPrice",
                    price));

                prm.Add(new SqlParameter("@TotalPrice",
                    price * qty));

                wt.RunInsDelUpd(@"

INSERT INTO SurgerySupplies
(
    SurgeryId,
    SupplyId,
    UsedQuantity,
    UnitPrice,
    TotalPrice
)
VALUES
(
    @SurgeryId,
    @SupplyId,
    @UsedQuantity,
    @UnitPrice,
    @TotalPrice
)

", prm);


                wt.RunInsDelUpd(
                    "UPDATE MedicalSupplies SET AvailableQuantity=AvailableQuantity-" +
                    qty +
                    " WHERE Id=" +
                    supplyId);


                LoadMedicalSupplies();

                LoadPatientSupplies();

                Response.Write("<script>alert('تم إضافة المستلزم بنجاح');</script>");

            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" +
                    ex.Message.Replace("'", "") +
                    "');</script>");
            }
        }



        protected void btnInvoice_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                "SurgeryInvoice.aspx?sid="
                + txtSurgeryId.Text
                + "&pid="
                + txtPatientId.Text);
        }










        protected void gvPatientSupplies_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int id = Convert.ToInt32(e.CommandArgument);

                    DataTable dt = wt.RunSelect(@"
            SELECT SupplyId,Quantity
            FROM PatientSupplies
            WHERE Id=" + id);

                    if (dt.Rows.Count > 0)
                    {
                        int supplyId = Convert.ToInt32(dt.Rows[0]["SupplyId"]);
                        int qty = Convert.ToInt32(dt.Rows[0]["Quantity"]);

                        wt.RunInsDelUpd(
                            "UPDATE MedicalSupplies SET AvailableQuantity=AvailableQuantity+" +
                            qty +
                            " WHERE Id=" + supplyId);

                        ps.Delete(id);

                        wt.RunInsDelUpd(@"
                DELETE FROM SurgerySupplies
                WHERE SurgeryId=" + txtSurgeryId.Text +
                        " AND SupplyId=" + supplyId);

                        LoadMedicalSupplies();
                        LoadPatientSupplies();

                        Response.Write("<script>alert('تم حذف المستلزم وإرجاعه إلى المخزن');</script>");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" +
                        ex.Message.Replace("'", "") +
                        "');</script>");
                }
            }
        }
    }
}
