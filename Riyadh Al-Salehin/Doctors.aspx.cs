using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Doctors : System.Web.UI.Page
    {
        public class DoctorScheduleItemView
        {
            public int Id { get; set; }
            public int DoctorId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsBooked { get; set; }
            public string Status => IsBooked ? "محجوز" : "متاح";
        }

        public class DoctorScheduleDayView
        {
            public int DoctorId { get; set; }
            public DateTime Date { get; set; }
            public string DayName { get; set; }
            public string DateDisplay { get; set; }
            public int Total { get; set; }
            public int Available { get; set; }
            public int Booked { get; set; }
            public List<DoctorScheduleItemView> Schedules { get; set; }
        }

        public class DoctorScheduleDoctorView
        {
            public int DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string DoctorNameDisplay { get; set; }
            public string Specialty { get; set; }
            public List<DoctorScheduleDayView> Days { get; set; }
        }

        private CDoctors objDoctor = new CDoctors();
        private CDoctorSchedules objSchedule = new CDoctorSchedules();

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
                objDoctor.EnsureDoctorCodeColumnExists();
                LoadDoctors();
                LoadDoctorFilter();
                LoadSchedules();
            }
        }

        #region تحميل البيانات
        private void LoadDoctors()
        {
            gvDoctors.DataSource = objDoctor.GetAll();
            gvDoctors.DataBind();
        }

        private void LoadDoctorFilter()
        {
            DataTable dt = objDoctor.GetAll();
            ddlDoctorFilter.Items.Clear();
            ddlDoctorFilter.Items.Add(new ListItem("جميع الأطباء", "0"));
            foreach (DataRow row in dt.Rows)
            {
                ddlDoctorFilter.Items.Add(new ListItem(row["DoctorName"].ToString(), row["Id"].ToString()));
            }
            ddlDoctorFilter.SelectedValue = "0";
        }

        private void LoadSchedules()
        {
            int? doctorId = null;
            if (ddlDoctorFilter.SelectedValue != "0")
                doctorId = Convert.ToInt32(ddlDoctorFilter.SelectedValue);

            DateTime? fromDate = null, toDate = null;
            if (DateTime.TryParse(txtFromDate.Text, out DateTime fd))
                fromDate = fd.Date;
            else
                fromDate = DateTime.Today;

            if (DateTime.TryParse(txtToDate.Text, out DateTime td))
                toDate = td.Date;
            else
                toDate = DateTime.Today.AddDays(14);

            DataTable dt = objSchedule.GetUpcomingSchedules(fromDate, toDate, doctorId);

            if (dt == null || dt.Rows.Count == 0)
            {
                rptSchedules.DataSource = null;
                rptSchedules.DataBind();
                lblNoSchedules.Visible = true;
                lblScheduleTitle.Text = "لا توجد مواعيد قادمة";
                btnDeleteUpcoming.Visible = false;
                return;
            }
            lblNoSchedules.Visible = false;

            var doctors = dt.AsEnumerable()
                .GroupBy(r => Convert.ToInt32(r["DoctorId"]))
                .Select(g =>
                {
                    var first = g.First();
                    return new DoctorScheduleDoctorView
                    {
                        DoctorId = Convert.ToInt32(first["DoctorId"]),
                        DoctorName = first["DoctorName"]?.ToString() ?? "",
                        DoctorNameDisplay = "د. " + (first["DoctorName"]?.ToString() ?? ""),
                        Specialty = first["Specialty"]?.ToString() ?? "",
                        Days = g.GroupBy(r => Convert.ToDateTime(r["StartTime"]).Date)
                                .Select(dg =>
                                {
                                    var allSchedulesForDay = dg.Select(r => new DoctorScheduleItemView
                                    {
                                        Id = Convert.ToInt32(r["Id"]),
                                        DoctorId = Convert.ToInt32(r["DoctorId"]),
                                        StartTime = Convert.ToDateTime(r["StartTime"]),
                                        EndTime = Convert.ToDateTime(r["EndTime"]),
                                        IsBooked = Convert.ToBoolean(r["IsBooked"])
                                    }).ToList();

                                    var schedules = allSchedulesForDay
                                        .Where(s => s.StartTime >= DateTime.Now) // فقط المستقبلية
                                        .ToList();

                                    if (schedules.Count == 0) return null;

                                    return new DoctorScheduleDayView
                                    {
                                        DoctorId = Convert.ToInt32(first["DoctorId"]),
                                        Date = dg.Key,
                                        DayName = GetArabicDayName(dg.Key.DayOfWeek),
                                        DateDisplay = dg.Key.ToString("dd/MM/yyyy"),
                                        Total = schedules.Count,
                                        Available = schedules.Count(s => !s.IsBooked),
                                        Booked = schedules.Count(s => s.IsBooked),
                                        Schedules = schedules.OrderBy(s => s.StartTime).ToList()
                                    };
                                })
                                .Where(d => d != null)
                                .OrderBy(d => d.Date)
                                .ToList()
                    };
                })
                .Where(d => d.Days.Count > 0)
                .OrderBy(d => d.DoctorName)
                .ToList();

            rptSchedules.DataSource = doctors;
            rptSchedules.DataBind();

            if (doctorId.HasValue && doctors.Count > 0)
                lblScheduleTitle.Text = "جدول مواعيد " + doctors.First().DoctorNameDisplay;
            else
                lblScheduleTitle.Text = "جميع المواعيد القادمة";

            btnDeleteUpcoming.Visible = doctors.Count > 0;
            if (btnDeleteUpcoming.Visible)
            {
                if (doctorId.HasValue)
                    btnDeleteUpcoming.Text = "حذف مواعيد الطبيب القادمة";
                else
                    btnDeleteUpcoming.Text = "حذف جميع المواعيد القادمة";

                string confirmMsg = doctorId.HasValue ?
                    "هل أنت متأكد من حذف جميع المواعيد القادمة غير المحجوزة للطبيب المحدد؟" :
                    "هل أنت متأكد من حذف جميع المواعيد القادمة غير المحجوزة لجميع الأطباء؟";
                btnDeleteUpcoming.OnClientClick = $"return confirm('{confirmMsg}');";
            }
        }

        private string GetArabicDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday: return "الأحد";
                case DayOfWeek.Monday: return "الإثنين";
                case DayOfWeek.Tuesday: return "الثلاثاء";
                case DayOfWeek.Wednesday: return "الأربعاء";
                case DayOfWeek.Thursday: return "الخميس";
                case DayOfWeek.Friday: return "الجمعة";
                case DayOfWeek.Saturday: return "السبت";
                default: return day.ToString();
            }
        }
        #endregion

        #region أحداث الأطباء
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                objDoctor.DoctorName = txtDoctorName.Text.Trim();
                objDoctor.DoctorCode = txtDoctorCode.Text.Trim().ToUpper();
                objDoctor.Specialty = txtSpecialty.Text.Trim();
                objDoctor.Phone = txtPhone.Text.Trim();
                objDoctor.Email = txtEmail.Text.Trim();
                objDoctor.DoctorType = ddlDoctorType.SelectedValue;
                objDoctor.CommissionRate = string.IsNullOrEmpty(txtCommissionRate.Text) ? 0 : Convert.ToDecimal(txtCommissionRate.Text);
                objDoctor.ConsultationFee = string.IsNullOrEmpty(txtConsultationFee.Text) ? 0 : Convert.ToDecimal(txtConsultationFee.Text);

                int doctorId = objDoctor.InsertAndReturnId();

                if (fuImage.HasFile)
                {
                    string folder = Server.MapPath("~/Uploads/Doctors/");
                    if (!System.IO.Directory.Exists(folder))
                        System.IO.Directory.CreateDirectory(folder);
                    fuImage.SaveAs(System.IO.Path.Combine(folder, doctorId + ".jpg"));
                }

                ShowMessage("تم حفظ الطبيب بنجاح", "success");
                LoadDoctors();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "error");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfDoctorId.Value))
            {
                ShowMessage("يرجى اختيار طبيب من الجدول أولاً", "warning");
                return;
            }
            try
            {
                objDoctor.Id = Convert.ToInt32(hfDoctorId.Value);
                objDoctor.DoctorName = txtDoctorName.Text.Trim();
                objDoctor.DoctorCode = txtDoctorCode.Text.Trim().ToUpper();
                objDoctor.Specialty = txtSpecialty.Text.Trim();
                objDoctor.Phone = txtPhone.Text.Trim();
                objDoctor.Email = txtEmail.Text.Trim();
                objDoctor.DoctorType = ddlDoctorType.SelectedValue;
                objDoctor.CommissionRate = string.IsNullOrEmpty(txtCommissionRate.Text) ? 0 : Convert.ToDecimal(txtCommissionRate.Text);
                objDoctor.ConsultationFee = string.IsNullOrEmpty(txtConsultationFee.Text) ? 0 : Convert.ToDecimal(txtConsultationFee.Text);

                string result = objDoctor.Update();
                if (result == "OK")
                {
                    ShowMessage("تم تعديل بيانات الطبيب بنجاح", "success");
                    LoadDoctors();
                    ClearForm();
                }
                else ShowMessage("حدث خطأ: " + result, "error");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfDoctorId.Value))
            {
                ShowMessage("يرجى اختيار طبيب من الجدول أولاً", "warning");
                return;
            }
            string result = objDoctor.Delete(Convert.ToInt32(hfDoctorId.Value));
            if (result == "OK")
            {
                ShowMessage("تم حذف الطبيب بنجاح", "success");
                LoadDoctors();
                LoadDoctorFilter();
                ClearForm();
            }
            else ShowMessage("لا يمكن الحذف: الطبيب مرتبط ببيانات أخرى", "error");
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowMessage("", "");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                LoadDoctors();
            else
            {
                gvDoctors.DataSource = objDoctor.Search(txtSearch.Text.Trim());
                gvDoctors.DataBind();
            }
        }

        protected void gvDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            int doctorId = Convert.ToInt32(gvDoctors.SelectedDataKey.Value);
            DataTable dt = objDoctor.GetById(doctorId);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                hfDoctorId.Value = doctorId.ToString();
                txtDoctorName.Text = row["DoctorName"].ToString();
                txtDoctorCode.Text = row["DoctorCode"]?.ToString() ?? "";
                txtSpecialty.Text = row["Specialty"].ToString();
                txtPhone.Text = row["Phone"].ToString();
                txtEmail.Text = row["Email"].ToString();
                txtCommissionRate.Text = row["CommissionRate"].ToString();
                txtConsultationFee.Text = row["ConsultationFee"].ToString();
                ddlDoctorType.SelectedValue = row["DoctorType"].ToString();

                string imagePath = "~/Uploads/Doctors/" + doctorId + ".jpg";
                imgPreview.ImageUrl = System.IO.File.Exists(Server.MapPath(imagePath)) ? imagePath : "~/Images/no-image.png";

                ddlDoctorFilter.SelectedValue = doctorId.ToString();
                LoadSchedules();
            }
        }
        #endregion

        #region إضافة المواعيد المتكررة
        protected void btnAddSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfDoctorId.Value))
                {
                    ShowMessage("اختر الطبيب أولاً", "warning");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtStartTime.Text) || string.IsNullOrWhiteSpace(txtEndTime.Text))
                {
                    ShowMessage("يرجى إدخال وقت البداية ووقت النهاية", "warning");
                    return;
                }

                List<DayOfWeek> selectedDays = new List<DayOfWeek>();
                foreach (ListItem item in cblDays.Items)
                    if (item.Selected)
                        selectedDays.Add((DayOfWeek)Convert.ToInt32(item.Value));
                if (selectedDays.Count == 0)
                {
                    ShowMessage("يرجى اختيار يوم واحد على الأقل", "warning");
                    return;
                }

                TimeSpan startTime = ParseTime(txtStartTime.Text);
                TimeSpan endTime = ParseTime(txtEndTime.Text);
                int weeks = int.TryParse(txtWeeks.Text, out int w) && w > 0 ? w : 52;
                int doctorId = Convert.ToInt32(hfDoctorId.Value);
                int addedCount = 0;

                DateTime startDate = DateTime.Today;
                DateTime endDate = startDate.AddDays(weeks * 7);

                for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
                {
                    if (selectedDays.Contains(date.DayOfWeek))
                    {
                        DateTime startDateTime = date.Date.Add(startTime);
                        DateTime endDateTime = date.Date.Add(endTime);
                        if (endDateTime <= startDateTime)
                            endDateTime = endDateTime.AddDays(1);

                        if (!objSchedule.Exists(doctorId, startDateTime, endDateTime))
                        {
                            objSchedule.DoctorId = doctorId;
                            objSchedule.StartTime = startDateTime;
                            objSchedule.EndTime = endDateTime;
                            objSchedule.IsBooked = false;
                            if (objSchedule.Add() == "OK")
                                addedCount++;
                        }
                    }
                }
                ShowMessage($"تم إضافة {addedCount} موعد متكرر بنجاح", "success");
                LoadSchedules();
            }
            catch (FormatException fex)
            {
                ShowMessage("خطأ في تنسيق الوقت: " + fex.Message, "error");
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message, "error");
            }
        }

        private TimeSpan ParseTime(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                throw new FormatException("الرجاء إدخال وقت صحيح.");
            timeString = timeString.Trim();
            string[] formats = { "hh:mm tt", "h:mm tt", "hh:mm:ss tt", "h:mm:ss tt", "HH:mm", "H:mm", "HH:mm:ss", "H:mm:ss" };
            if (DateTime.TryParseExact(timeString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                return dt.TimeOfDay;
            if (DateTime.TryParse(timeString, out dt))
                return dt.TimeOfDay;
            throw new FormatException($"تنسيق الوقت غير معروف: '{timeString}'.");
        }
        #endregion

        #region أحداث Repeater
        protected void rptSchedules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DoctorScheduleDoctorView doctor = e.Item.DataItem as DoctorScheduleDoctorView;
            if (doctor == null) return;

            Repeater rptDays = e.Item.FindControl("rptDays") as Repeater;
            if (rptDays != null)
            {
                rptDays.DataSource = doctor.Days;
                rptDays.DataBind();
            }
        }

        protected void rptDays_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DoctorScheduleDayView day = e.Item.DataItem as DoctorScheduleDayView;
            if (day == null) return;

            GridView gvDetails = e.Item.FindControl("gvDayDetails") as GridView;
            if (gvDetails != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("StartTime", typeof(DateTime));
                dt.Columns.Add("EndTime", typeof(DateTime));
                dt.Columns.Add("IsBooked", typeof(bool));
                foreach (var item in day.Schedules)
                {
                    dt.Rows.Add(item.Id, item.StartTime, item.EndTime, item.IsBooked);
                }
                gvDetails.DataSource = dt;
                gvDetails.DataBind();
            }
        }

        protected void rptSchedules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteDay")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int doctorId = Convert.ToInt32(args[0]);
                DateTime date = DateTime.ParseExact(args[1], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                DeleteDay(doctorId, date);
            }
            else if (e.CommandName == "DeleteSchedule")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                DeleteSingleSchedule(id);
            }
        }
        #endregion

        #region عمليات الحذف
        private void DeleteDay(int doctorId, DateTime date)
        {
            int deleted = objSchedule.DeleteDaySchedules(doctorId, date);
            if (deleted > 0)
                ShowMessage($"تم حذف {deleted} موعد غير محجوز لهذا اليوم.", "success");
            else if (deleted == 0)
                ShowMessage("لا توجد مواعيد غير محجوزة أو مستقبلية لهذا اليوم.", "warning");
            else
                ShowMessage("حدث خطأ أثناء حذف اليوم.", "error");
            LoadSchedules();
        }

        private void DeleteSingleSchedule(int id)
        {
            DataTable dt = objSchedule.GetById(id);
            if (dt.Rows.Count == 0)
            {
                ShowMessage("الموعد غير موجود.", "warning");
                LoadSchedules();
                return;
            }

            DataRow row = dt.Rows[0];
            bool isBooked = Convert.ToBoolean(row["IsBooked"]);
            DateTime startTime = Convert.ToDateTime(row["StartTime"]);

            if (isBooked)
            {
                ShowMessage("لا يمكن حذف الموعد لأنه محجوز.", "warning");
                LoadSchedules();
                return;
            }

            if (startTime <= DateTime.Now)
            {
                ShowMessage("لا يمكن حذف موعد انتهى.", "warning");
                LoadSchedules();
                return;
            }

            int deleted = objSchedule.DeleteSchedule(id);
            if (deleted > 0)
                ShowMessage("تم حذف الموعد بنجاح.", "success");
            else
                ShowMessage("لم يتم حذف الموعد (ربما تم حجزه أو انتهى أثناء المعالجة).", "warning");
            LoadSchedules();
        }

        protected void btnDeleteUpcoming_Click(object sender, EventArgs e)
        {
            DateTime? fromDate = null, toDate = null;
            if (!TryGetDateFilters(out fromDate, out toDate))
                return;

            int? doctorId = null;
            if (ddlDoctorFilter.SelectedValue != "0")
                doctorId = Convert.ToInt32(ddlDoctorFilter.SelectedValue);

            int deleted = objSchedule.DeleteUpcomingSchedules(doctorId, fromDate, toDate);
            if (deleted > 0)
                ShowMessage($"تم حذف {deleted} موعد غير محجوز.", "success");
            else if (deleted == 0)
                ShowMessage("لا توجد مواعيد غير محجوزة أو مستقبلية تطابق الفلتر.", "warning");
            else
                ShowMessage("حدث خطأ أثناء حذف المواعيد.", "error");

            LoadSchedules();
        }
        #endregion

        #region فلترة المواعيد
        protected void ddlDoctorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSchedules();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadSchedules();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlDoctorFilter.SelectedValue = "0";
            LoadSchedules();
        }

        private bool TryGetDateFilters(out DateTime? fromDate, out DateTime? toDate)
        {
            fromDate = null;
            toDate = null;

            if (!string.IsNullOrEmpty(txtFromDate.Text))
            {
                if (!DateTime.TryParse(txtFromDate.Text, out DateTime fd))
                {
                    ShowMessage("تاريخ البداية غير صحيح.", "error");
                    return false;
                }
                fromDate = fd.Date;
            }

            if (!string.IsNullOrEmpty(txtToDate.Text))
            {
                if (!DateTime.TryParse(txtToDate.Text, out DateTime td))
                {
                    ShowMessage("تاريخ النهاية غير صحيح.", "error");
                    return false;
                }
                toDate = td.Date;
            }

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                ShowMessage("تاريخ البداية يجب ألا يكون بعد تاريخ النهاية.", "error");
                return false;
            }

            return true;
        }
        #endregion

        #region دوال مساعدة
        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = Color.Black;

            switch (type)
            {
                case "success": lblMessage.ForeColor = Color.Green; break;
                case "error": lblMessage.ForeColor = Color.Red; break;
                case "warning": lblMessage.ForeColor = Color.Orange; break;
                default: lblMessage.Text = ""; break;
            }
        }

        private void ClearForm()
        {
            hfDoctorId.Value = "";
            txtDoctorName.Text = "";
            txtDoctorCode.Text = "";
            txtSpecialty.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtCommissionRate.Text = "0";
            txtConsultationFee.Text = "0";
            ddlDoctorType.SelectedValue = "Clinic";
            imgPreview.ImageUrl = "~/Images/no-image.png";
            foreach (ListItem item in cblDays.Items)
                item.Selected = false;
        }
        #endregion
    }
}