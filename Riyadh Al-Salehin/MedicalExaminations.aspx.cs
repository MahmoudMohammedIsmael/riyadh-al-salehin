using System;
using System.Data;

namespace Riyadh_Al_Salehin
{
    public partial class MedicalExaminations : System.Web.UI.Page
    {
        private CMedicalExaminations ex = new CMedicalExaminations();

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
            gvExaminations.DataSource = ex.GetAll();
            gvExaminations.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ex.AppointmentId = Convert.ToInt32(txtAppointmentId.Text);
                ex.InitialDiagnosis = txtInitialDiagnosis.Text;
                ex.DecisionType = txtDecisionType.Text;
                ex.Status = txtStatus.Text;
                ex.Notes = txtNotes.Text;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    ex.Insert();
                }
                else
                {
                    ex.Id = Convert.ToInt32(hfId.Value);
                    ex.Update();
                }

                Clear();
                LoadData();
            }
            catch (Exception exx)
            {
                Response.Write("<script>alert('خطأ: " + exx.Message + "')</script>");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            hfId.Value = "";
            txtAppointmentId.Text = "";
            txtInitialDiagnosis.Text = "";
            txtDecisionType.Text = "";
            txtStatus.Text = "";
            txtNotes.Text = "";
        }

        protected void gvExaminations_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = ex.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtAppointmentId.Text = dt.Rows[0]["AppointmentId"].ToString();
                    txtInitialDiagnosis.Text = dt.Rows[0]["InitialDiagnosis"].ToString();
                    txtDecisionType.Text = dt.Rows[0]["DecisionType"].ToString();
                    txtStatus.Text = dt.Rows[0]["Status"].ToString();
                    txtNotes.Text = dt.Rows[0]["Notes"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                ex.Delete(id);
                LoadData();
            }
        }
    }
}