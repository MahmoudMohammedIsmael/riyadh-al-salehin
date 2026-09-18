using System;
using System.Data;
using System.Web.UI;

namespace Riyadh_Al_Salehin
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
                Session.Abandon();

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Expires =
                        DateTime.Now.AddDays(-1);
                }
            }
        }


        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.Visible = true;
        }




        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowError("يرجى إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            CUsers objUser = new CUsers();

            DataTable dt = objUser.Login(username, password);

            if (dt.Rows.Count == 0)
            {
                ShowError("اسم المستخدم أو كلمة المرور غير صحيحة");
                txtPassword.Text = "";
                return;
            }

            DataRow row = dt.Rows[0];

            Session["UserId"] = row["Id"];
            Session["FullName"] = row["FullName"];
            Session["Username"] = row["Username"];
            Session["RoleId"] = row["RoleId"];
            Session["RoleName"] = row["RoleName"];

            objUser.UpdateLastLogin(Convert.ToInt32(row["Id"]));

            string page = GetHomePage();

            Response.Redirect(page, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private string GetHomePage()
        {
            //// المدير

            if (CPermissions.Check("Patients", "View"))
                return "~/Patients.aspx";


            if (CPermissions.Check("PatientsSurgeries", "View"))
                return "~/PatientsSurgeries.aspx";




            if (CPermissions.Check("Doctors", "View"))
                return "~/Doctors.aspx";

            if (CPermissions.Check("Appointments", "View"))
                return "~/Appointments.aspx";

            if (CPermissions.Check("DoctorWaitingPatients", "View"))
                return "~/DoctorWaitingPatients.aspx";

            if (CPermissions.Check("MedicalExamination", "View"))
                return "~/MedicalExamination.aspx";

            if (CPermissions.Check("PerformedExaminationProcedures", "View"))
                return "~/PerformedExaminationProcedures.aspx";

            if (CPermissions.Check("InvoiceItems", "View"))
                return "~/InvoiceItems.aspx";

            if (CPermissions.Check("Invoices", "View"))
                return "~/InvoicesList.aspx";

            if (CPermissions.Check("Surgeries", "View"))
                return "~/Surgeries.aspx";

            if (CPermissions.Check("SurgeryAccounts", "View"))
                return "~/SurgeryAccounts.aspx";

            if (CPermissions.Check("SurgerySupplies", "View"))
                return "~/SurgerySupplies.aspx";

            
            if (CPermissions.Check("MedicalCenters", "View"))
                return "~/MedicalCenters.aspx";

            if (CPermissions.Check("LabRequests", "View"))
                return "~/LabRequests.aspx";

            if (CPermissions.Check("LabResults", "View"))
                return "~/LabResults.aspx";

            if (CPermissions.Check("Users", "View"))
                return "~/Users.aspx";

            if (CPermissions.Check("Roles", "View"))
                return "~/Roles.aspx";

            if (CPermissions.Check("Permissions", "View"))
                return "~/PermissionsI.aspx";

            if (CPermissions.Check("RolesPermissions", "View"))
                return "~/Rolespermissions.aspx";

            if (CPermissions.Check("ServicesAndPrices", "View"))
                return "~/ServicesAndPrices.aspx";

            if (CPermissions.Check("MedicalReferrals", "View"))
                return "~/MedicalReferrals.aspx";

            if (CPermissions.Check("OperationLogs", "View"))
                return "~/OperationLogs.aspx";

            if (CPermissions.Check("OperationRooms", "View"))
                return "~/OperationRooms.aspx";

            if (CPermissions.Check("MedicalSupplies", "View"))
                return "~/MedicalSupplies.aspx";

            return "~/Login.aspx";
        }


    }
}