using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class PatientDoctorSurgeries : System.Web.UI.Page
    {
        private CDoctors doc = new CDoctors();

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
                if (Request.QueryString["pid"] == null)
                {
                    Response.Redirect("Patients.aspx");
                    return;
                }

                hfPatientId.Value = Request.QueryString["pid"];

                LoadDoctors();
            }
        }

        private void LoadDoctors()
        {
            DataTable dt = doc.GetAll();

            DataView dv = dt.DefaultView;

            dv.RowFilter = "DoctorType = 'Surgery'";

            rptDoctors.DataSource = dv;
            rptDoctors.DataBind();

            
        }


        protected void rptDoctors_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectDoctor")
            {
                int patientId =
                    Convert.ToInt32(hfPatientId.Value);

                int doctorId =
                    Convert.ToInt32(e.CommandArgument);

                int centerId =
                    GetDoctorCenterId(doctorId);

                Response.Redirect(
                    "~/Surgeries.aspx?pid=" + patientId +
                    "&did=" + doctorId +
                    "&cid=" + centerId,
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private int GetDoctorCenterId(int doctorId)
        {
            try
            {
                DataTable dt = doc.GetById(doctorId);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("CenterId"))
                    {
                        return Convert.ToInt32(
                            dt.Rows[0]["CenterId"]);
                    }
                }

                return 1;
            }
            catch
            {
                return 1;
            }
        }
    }
}