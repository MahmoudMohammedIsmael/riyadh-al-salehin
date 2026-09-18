using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Riyadh_Al_Salehin
{
    public partial class SurgeryAccounts : System.Web.UI.Page
    {
        private CSurgeryAccounts acc = new CSurgeryAccounts();
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
                LoadParameters();
                LoadData();
            }
        }

        void LoadParameters()
        {
            string sid = Request.QueryString["sid"];
            string pid = Request.QueryString["pid"];
            string did = Request.QueryString["did"];
            string cid = Request.QueryString["cid"];

            if (!string.IsNullOrEmpty(sid))
                txtSurgeryId.Text = sid;

            if (!string.IsNullOrEmpty(sid))
            {
                LoadRoomData(Convert.ToInt32(sid));
            }


            if (!string.IsNullOrEmpty(pid))
                txtPatientId.Text = pid;

            if (!string.IsNullOrEmpty(did) && int.TryParse(did, out int doctorId))
            {
                decimal? doctorCommission = GetDoctorCommission(doctorId);
                if (doctorCommission.HasValue)
                    txtDoctorCommission.Text = doctorCommission.Value.ToString();
            }

            if (!string.IsNullOrEmpty(cid) && int.TryParse(cid, out int centerId))
            {
                decimal? centerCommission = GetCenterCommission(centerId);
                if (centerCommission.HasValue)
                    txtCenterCommission.Text = centerCommission.Value.ToString();
            }
        }

        protected void CostChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            decimal doctor = 0;
            decimal center = 0;
            decimal room = 0;

            decimal.TryParse(txtDoctorCommission.Text, out doctor);
            decimal.TryParse(txtCenterCommission.Text, out center);
            decimal.TryParse(txtRoomCost.Text, out room);

            txtTotalCost.Text =
                (doctor + center + room).ToString("0.00");
        }


        private void LoadRoomData(int surgeryId)
        {
            DataTable dt = wt.RunSelect(@"
    SELECT
        r.RoomName,
        r.DailyPrice
    FROM Surgeries s
    INNER JOIN OperationRooms r
        ON s.RoomId=r.Id
    WHERE s.Id=" + surgeryId);

            if (dt.Rows.Count > 0)
            {
                txtRoomName.Text = dt.Rows[0]["RoomName"].ToString();
                txtRoomCost.Text = dt.Rows[0]["DailyPrice"].ToString();

                CalculateTotal();
            }
        }
        private decimal? GetDoctorCommission(int doctorId)
        {
            try
            {
                object result = wt.RunScalar(
                    "SELECT CommissionRate FROM Doctors WHERE Id = @Id",
                    new List<SqlParameter> { new SqlParameter("@Id", doctorId) });

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ في جلب عمولة الطبيب: " + ex.Message.Replace("'", "\\'") + "');</script>");
                return null;
            }
        }

        private decimal? GetCenterCommission(int centerId)
        {
            try
            {
                object result = wt.RunScalar(
                    "SELECT CommissionRate FROM MedicalCenters WHERE Id = @Id",
                    new List<SqlParameter> { new SqlParameter("@Id", centerId) });

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ في جلب عمولة المركز: " + ex.Message.Replace("'", "\\'") + "');</script>");
                return null;
            }
        }

        void LoadData()
        {
            gvAccounts.DataSource = acc.GetAll();
            gvAccounts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtSurgeryId.Text, out int surgeryId))
                {
                    Response.Write("<script>alert('رقم العملية (Surgery ID) غير صحيح');</script>");
                    return;
                }

                if (!int.TryParse(txtPatientId.Text, out int patientId))
                {
                    Response.Write("<script>alert('رقم المريض (Patient ID) غير صحيح');</script>");
                    return;
                }

                acc.SurgeryId = surgeryId;
                acc.PatientId = patientId;

                decimal total = 0, advance = 0, remaining = 0;
                decimal doctor = 0, center = 0;

                decimal.TryParse(txtTotalCost.Text, out total);
                decimal.TryParse(txtAdvancePayment.Text, out advance);
                decimal.TryParse(txtRemainingAmount.Text, out remaining);
                decimal.TryParse(txtDoctorCommission.Text, out doctor);
                decimal.TryParse(txtCenterCommission.Text, out center);

                acc.TotalCost = total;
                acc.AdvancePayment = advance;
                acc.RemainingAmount = remaining;
                acc.DoctorCommission = doctor;
                acc.CenterCommission = center;

                decimal room = 0;
                decimal.TryParse(txtRoomCost.Text, out room);

                acc.RoomCost = room;



                acc.PaymentStatus = string.IsNullOrWhiteSpace(txtPaymentStatus.Text) ? "غير محدد" : txtPaymentStatus.Text;

                string result = "";

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    result = acc.Insert();
                }
                else
                {
                    acc.Id = Convert.ToInt32(hfId.Value);
                    result = acc.Update();
                }

                if (result == "OK")
                {
                    Response.Redirect(
                        "PatientSupplies.aspx?sid=" +
                        surgeryId +
                        "&pid=" +
                        patientId,
                        false);

                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    Response.Write("<script>alert('" + result.Replace("'", "") + "');</script>");
                }


            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ: " + ex.Message.Replace("'", "\\'") + "')</script>");
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
            txtPatientId.Text = "";
            txtTotalCost.Text = "";
            txtAdvancePayment.Text = "";
            txtRemainingAmount.Text = "";
            txtPaymentStatus.Text = "";
            txtDoctorCommission.Text = "";
            txtCenterCommission.Text = "";
        }

        protected void gvAccounts_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = acc.GetBySurgery(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtSurgeryId.Text = dt.Rows[0]["SurgeryId"].ToString();
                    txtPatientId.Text = dt.Rows[0]["PatientId"].ToString();
                    txtAdvancePayment.Text = dt.Rows[0]["AdvancePayment"].ToString();
                    txtRemainingAmount.Text = dt.Rows[0]["RemainingAmount"].ToString();
                    txtPaymentStatus.Text = dt.Rows[0]["PaymentStatus"].ToString();

                    if (dt.Columns.Contains("TotalCost"))
                        txtTotalCost.Text = dt.Rows[0]["TotalCost"].ToString();

                    if (dt.Columns.Contains("DoctorCommission"))
                        txtDoctorCommission.Text = dt.Rows[0]["DoctorCommission"].ToString();

                    if (dt.Columns.Contains("CenterCommission"))
                        txtCenterCommission.Text = dt.Rows[0]["CenterCommission"].ToString();
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                acc.Delete(id);
                LoadData();
            }
        }
    }
}