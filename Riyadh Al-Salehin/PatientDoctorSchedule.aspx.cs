using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class PatientDoctorSchedule : System.Web.UI.Page
    {
        private CDoctors doc = new CDoctors();
        private CAppointments appService = new CAppointments();
        private CDoctorSchedules sch = new CDoctorSchedules();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                HttpCookie cookie = Request.Cookies["UserId"];
                if (cookie != null && int.TryParse(cookie.Value, out int uid))
                {
                    Session["UserId"] = uid;
                    HttpCookie newCookie = new HttpCookie("UserId", uid.ToString());
                    newCookie.Expires = DateTime.Now.AddHours(2);
                    Response.Cookies.Add(newCookie);
                }
            }

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

            if (dt == null)
            {
                rptDoctors.DataSource = null;
                rptDoctors.DataBind();
                return;
            }

            DataView dv = dt.DefaultView;

            if (dt.Columns.Contains("DoctorType"))
            {
                dv.RowFilter = "DoctorType = 'Clinic'";
            }

            rptDoctors.DataSource = dv;
            rptDoctors.DataBind();

            gvSlots.DataSource = null;
            gvSlots.DataBind();
        }




        protected bool IsDoctorAvailableToday(object doctorIdValue)
        {
            try
            {
                if (doctorIdValue == null ||
                    doctorIdValue == DBNull.Value)
                {
                    return false;
                }

                int doctorId;

                if (!int.TryParse(
                    doctorIdValue.ToString(),
                    out doctorId))
                {
                    return false;
                }

                DataTable dt = sch.GetByDoctor(doctorId);

                if (dt == null ||
                    dt.Rows.Count == 0)
                {
                    return false;
                }

                if (!dt.Columns.Contains("StartTime"))
                {
                    return false;
                }

                DateTime today = DateTime.Today;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["StartTime"] == DBNull.Value)
                        continue;

                    DateTime startTime;

                    if (!DateTime.TryParse(
                        row["StartTime"].ToString(),
                        out startTime))
                    {
                        continue;
                    }

                    if (startTime.Date == today)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }









        protected void rptDoctors_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectDoctor")
            {
                hfDoctorId.Value = e.CommandArgument.ToString();
                LoadSlots();
            }
        }

        private void LoadSlots()
        {
            if (string.IsNullOrEmpty(hfDoctorId.Value))
            {
                lblMessage.Text = "الرجاء اختيار طبيب";
                return;
            }

            int doctorId;

            if (!int.TryParse(hfDoctorId.Value, out doctorId))
            {
                lblMessage.Text = "خطأ في رقم الطبيب";
                return;
            }

            DataTable dt = sch?.GetByDoctor(doctorId);

            if (dt == null || dt.Rows.Count == 0)
            {
                lblMessage.Text = "لا توجد مواعيد متاحة لهذا الطبيب";
                gvSlots.DataSource = null;
                gvSlots.DataBind();
                return;
            }

            lblMessage.Text = "";
            gvSlots.DataSource = dt;
            gvSlots.DataBind();
        }

        protected void gvSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int serviceId = 0;
                if (Request.QueryString["ServiceId"] != null)
                    int.TryParse(Request.QueryString["ServiceId"], out serviceId);

                if (serviceId == 0)
                {
                    lblMessage.Text = "خطأ: لم يتم تحديد الخدمة الطبية";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (Session["UserId"] == null)
                {
                    if (ViewState["UserId"] != null)
                    {
                        Session["UserId"] = (int)ViewState["UserId"];
                    }
                    else
                    {
                        Response.Redirect("~/Login.aspx");
                        return;
                    }
                }

                int patientId = Convert.ToInt32(hfPatientId.Value);
                int doctorId = Convert.ToInt32(hfDoctorId.Value);
                int centerId = GetDoctorCenterId(doctorId);
                int userId = Convert.ToInt32(Session["UserId"]);
                int scheduleId = Convert.ToInt32(gvSlots.DataKeys[gvSlots.SelectedRow.RowIndex].Value);

                DataTable dt = sch.GetById(scheduleId);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.Text = "الموعد غير موجود";
                    return;
                }

                DateTime startTime = Convert.ToDateTime(dt.Rows[0]["StartTime"]);

                CAppointments app = new CAppointments();
                app.PatientId = patientId;
                app.DoctorId = doctorId;
                app.CenterId = centerId;
                app.CreatedBy = userId;
                app.ScheduleId = scheduleId;
                app.AppointmentDate = startTime;
                app.Status = "Pending";
                app.QueueNumber = app.GetNextQueueNumber(doctorId, startTime);

                int appointmentId = app.InsertAndReturnId();

                if (appointmentId > 0)
                {
                    lblMessage.Text = "تم الحجز بنجاح";
                    lblMessage.ForeColor = System.Drawing.Color.Green;

                    sch.Book(scheduleId);
                    LoadSlots();

                    Response.Redirect(
                        $"ConsultationInvoice.aspx?AppointmentId={appointmentId}&ServiceId={serviceId}&uid={Session["UserId"]}",
                        false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    lblMessage.Text = "حدث خطأ أثناء الحجز";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }




        private int GetDoctorCenterId(int doctorId)
        {
            try
            {
                DataTable dt = doc.GetById(doctorId);

                if (dt.Rows.Count > 0 && dt.Columns.Contains("CenterId"))
                {
                    return Convert.ToInt32(dt.Rows[0]["CenterId"]);
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