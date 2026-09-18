using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class PerformedExaminationProcedures : System.Web.UI.Page
    {
        private CPerformedExaminationProcedures pep = new CPerformedExaminationProcedures();

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
            gvProcedures.DataSource = pep.GetAll();
            gvProcedures.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pep.ExaminationId = Convert.ToInt32(txtExaminationId.Text);
                pep.ServiceId = Convert.ToInt32(txtServiceId.Text);

                decimal price = 0;
                decimal.TryParse(txtPrice.Text, out price);
                pep.Price = price;

                pep.Notes = txtNotes.Text;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    pep.Insert();
                }
                else
                {
                    pep.Id = Convert.ToInt32(hfId.Value);
                    pep.Update();
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
            txtServiceId.Text = "";
            txtPrice.Text = "";
            txtNotes.Text = "";
        }

        protected void gvProcedures_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = pep.GetAll(); // لأن GetById غير موجود

                DataRow[] row = dt.Select("Id=" + id);

                if (row.Length > 0)
                {
                    hfId.Value = row[0]["Id"].ToString();
                    txtExaminationId.Text = row[0]["ExaminationId"].ToString();
                    txtServiceId.Text = row[0]["ServiceId"].ToString();
                    txtPrice.Text = row[0]["Price"].ToString();
                    txtNotes.Text = row[0]["Notes"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                pep.Delete(id);
                LoadData();
            }
        }
    }
}