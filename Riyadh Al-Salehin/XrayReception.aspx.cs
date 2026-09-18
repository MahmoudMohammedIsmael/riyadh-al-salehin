using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class XrayReception : System.Web.UI.Page
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
                LoadData();
        }

        private void LoadData()
        {
            gvRequests.DataSource = req.GetPendingForReception();
            gvRequests.DataBind();
        }

        protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "PrintInvoice")
            {
                DataTable allData = req.GetAll();
                DataRow[] rows = allData.Select("Id = " + id);

                if (rows.Length > 0)
                {
                    DataRow row = rows[0];

                    Session["XrayInv_RequestId"] = row["Id"].ToString();
                    Session["XrayInv_Patient"] = row["PatientName"]?.ToString() ?? "غير معروف";
                    Session["XrayInv_Service"] = row["XrayName"]?.ToString() ?? "غير محدد";
                    Session["XrayInv_Price"] = row["Price"]?.ToString() ?? "0.00";
                    Session["XrayInv_Date"] = Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd");

                    Response.Redirect("XrayInvoicePrint.aspx");
                }
                else
                {
                    lblMessage.Text = "❌ لم يتم العثور على الطلب.";
                    lblMessage.Visible = true;
                }
            }
            else if (e.CommandName == "ConfirmPayment")
            {
                string result = req.UpdateBilling(id, true);
                if (result == "OK")
                {
                    lblMessage.Text = "✅ تم تأكيد الدفع بنجاح.";
                    lblMessage.Visible = true;
                    LoadData(); // تحديث القائمة
                }
                else
                {
                    lblMessage.Text = "❌ حدث خطأ: " + result;
                    lblMessage.Visible = true;
                }
            }
        }
    }
}