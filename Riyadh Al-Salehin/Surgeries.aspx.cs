using System;
using System.Globalization;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Surgeries : System.Web.UI.Page
    {
        private CSurgeries surg = new CSurgeries();
        private COperationRooms rooms = new COperationRooms();

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
                LoadRooms();
                LoadParameters();
                LoadData();
            }
        }

        private void ShowMessage(string msg, bool isError = true)
        {
            lblMessage.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            lblMessage.Text = msg;
        }

        private void LoadRooms()
        {
            try
            {
                DataTable dt = rooms.GetAll();

                ddlRoomId.DataSource = dt;
                ddlRoomId.DataTextField = "RoomName";
                ddlRoomId.DataValueField = "Id";
                ddlRoomId.DataBind();

                ddlRoomId.Items.Insert(0, new ListItem("-- اختر غرفة --", "0"));
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحميل الغرف: " + ex.Message);
            }
        }

        private void LoadParameters()
        {
            txtPatientId.Text = Request.QueryString["pid"] ?? "";
            txtDoctorId.Text = Request.QueryString["did"] ?? "";
            txtCenterId.Text = Request.QueryString["cid"] ?? "";

            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            txtCreatedBy.Text = Session["UserId"].ToString();

            if (string.IsNullOrEmpty(txtPatientId.Text) ||
                string.IsNullOrEmpty(txtDoctorId.Text) ||
                string.IsNullOrEmpty(txtCenterId.Text))
            {
                ShowMessage("تنبيه: الصفحة مفتوحة بدون تحديد المريض/الطبيب/المركز. " +
                            "افتح الصفحة بالشكل: Surgeries.aspx?pid=..&did=..&cid=..", true);
            }
        }

        private void LoadData()
        {
            try
            {
                gvSurgeries.DataSource = surg.GetAll();
                gvSurgeries.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحميل قائمة العمليات: " + ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ShowMessage("جاري الحفظ...", false);

            try
            {
                if (string.IsNullOrEmpty(txtPatientId.Text) ||
                    string.IsNullOrEmpty(txtDoctorId.Text) ||
                    string.IsNullOrEmpty(txtCenterId.Text))
                {
                    ShowMessage("بيانات المريض أو الطبيب أو المركز غير مكتملة (تأكد من فتح الصفحة بالـ pid/did/cid الصحيحة)");
                    return;
                }

                if (!int.TryParse(txtPatientId.Text, out int patientId) ||
                    !int.TryParse(txtDoctorId.Text, out int doctorId) ||
                    !int.TryParse(txtCenterId.Text, out int centerId))
                {
                    ShowMessage("قيمة المريض/الطبيب/المركز ليست رقم صحيح");
                    return;
                }

                surg.PatientId = patientId;
                surg.DoctorId = doctorId;
                surg.CenterId = centerId;

                if (string.IsNullOrEmpty(ddlRoomId.SelectedValue) || ddlRoomId.SelectedValue == "0")
                {
                    ShowMessage("اختر غرفة العمليات");
                    return;
                }

                surg.RoomId = Convert.ToInt32(ddlRoomId.SelectedValue);

                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                surg.CreatedBy = Convert.ToInt32(Session["UserId"]);

                if (string.IsNullOrWhiteSpace(txtSurgeryName.Text))
                {
                    ShowMessage("اسم العملية مطلوب");
                    return;
                }

                surg.SurgeryType = txtSurgeryType.Text;
                surg.SurgeryName = txtSurgeryName.Text;

                var dateText = txtSurgeryDate.Text?.Trim();
                if (string.IsNullOrWhiteSpace(dateText))
                {
                    ShowMessage("تاريخ العملية غير صحيح أو غير مدخل");
                    return;
                }

                DateTime surgeryDate;
                var formats = new[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy", "dd-MM-yyyy" };

                if (!DateTime.TryParseExact(dateText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out surgeryDate)
                    && !DateTime.TryParse(dateText, CultureInfo.CurrentCulture, DateTimeStyles.None, out surgeryDate)
                    && !DateTime.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out surgeryDate))
                {
                    ShowMessage("تاريخ العملية غير صحيح أو غير مدخل");
                    return;
                }

                surg.SurgeryDate = surgeryDate;

                int.TryParse(txtDuration.Text, out int duration);
                surg.DurationInHours = duration;

                surg.Status = string.IsNullOrWhiteSpace(txtStatus.Text) ? "جديدة" : txtStatus.Text;
                surg.Notes = txtNotes.Text ?? "";

                decimal roomCost = 0;
                WorkTable wt = new WorkTable();
                DataTable dtRoom = wt.RunSelect("SELECT DailyPrice FROM OperationRooms WHERE Id=" + surg.RoomId);
                if (dtRoom.Rows.Count > 0 && dtRoom.Rows[0]["DailyPrice"] != DBNull.Value)
                {
                    decimal.TryParse(dtRoom.Rows[0]["DailyPrice"].ToString(), out roomCost);
                }
                surg.RoomCost = roomCost;

                decimal totalCost = 0;
                decimal.TryParse(txtTotalCost.Text, out totalCost);

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    int id = surg.Insert();

                    if (id <= 0)
                    {
                        ShowMessage("فشل الحفظ: لم يتم إنشاء سجل جديد (Id غير صحيح)");
                        return;
                    }

                    try
                    {
                        CSurgeryAccounts acc = new CSurgeryAccounts();
                        acc.SurgeryId = id;
                        acc.PatientId = surg.PatientId;
                        acc.TotalCost = totalCost;
                        acc.RoomCost = roomCost;
                        acc.AdvancePayment = 0;
                        acc.RemainingAmount = totalCost;
                        acc.DoctorCommission = totalCost >= roomCost ? (totalCost - roomCost) : 0;
                        acc.CenterCommission = 0;
                        acc.PaymentStatus = "Unpaid";
                        acc.Insert();
                    }
                    catch { }

                    ShowMessage("تم الحفظ بنجاح، جاري التحويل للمستلزمات...", false);

                    Response.Redirect(
                        "PatientSupplies.aspx?sid=" + id +
                        "&pid=" + surg.PatientId,
                        false
                    );
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    surg.Id = Convert.ToInt32(hfId.Value);
                    string updResult = surg.Update();

                    if (updResult != "OK")
                    {
                        ShowMessage("فشل التعديل: " + updResult);
                        return;
                    }

                    try
                    {
                        DataTable dtAcc = wt.RunSelect("SELECT Id FROM SurgeryAccounts WHERE SurgeryId=" + surg.Id);
                        if (dtAcc.Rows.Count > 0)
                        {
                            int accId = Convert.ToInt32(dtAcc.Rows[0]["Id"]);
                            decimal docComm = totalCost >= roomCost ? (totalCost - roomCost) : 0;
                            wt.RunInsDelUpd($"UPDATE SurgeryAccounts SET TotalCost={totalCost}, RoomCost={roomCost}, DoctorCommission={docComm} WHERE Id={accId}");
                        }
                    }
                    catch { }

                    ShowMessage("تم التعديل بنجاح", false);
                    Clear();
                }

                LoadData();
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ غير متوقع: " + ex.Message +
                            (ex.InnerException != null ? " | التفاصيل: " + ex.InnerException.Message : ""));
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            hfId.Value = "";

            txtSurgeryType.Text = "";
            txtSurgeryName.Text = "";
            txtSurgeryDate.Text = "";
            txtDuration.Text = "";
            txtTotalCost.Text = "";
            txtStatus.Text = "";
            txtNotes.Text = "";

            ddlRoomId.SelectedIndex = 0;
        }

        protected void gvSurgeries_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "EditRow")
                {
                    DataTable dt = surg.GetById(id);

                    if (dt.Rows.Count > 0)
                    {
                        hfId.Value = dt.Rows[0]["Id"].ToString();

                        txtPatientId.Text = dt.Rows[0]["PatientId"].ToString();
                        txtDoctorId.Text = dt.Rows[0]["DoctorId"].ToString();
                        txtCenterId.Text = dt.Rows[0]["CenterId"].ToString();

                        if (dt.Rows[0]["RoomId"] != DBNull.Value)
                            ddlRoomId.SelectedValue = dt.Rows[0]["RoomId"].ToString();

                        txtCreatedBy.Text = dt.Rows[0]["CreatedBy"].ToString();
                        txtSurgeryType.Text = dt.Rows[0]["SurgeryType"].ToString();
                        txtSurgeryName.Text = dt.Rows[0]["SurgeryName"].ToString();

                        if (dt.Rows[0]["SurgeryDate"] != DBNull.Value)
                            txtSurgeryDate.Text = Convert.ToDateTime(dt.Rows[0]["SurgeryDate"]).ToString("yyyy-MM-dd");

                        txtDuration.Text = dt.Rows[0]["DurationInHours"].ToString();
                        txtStatus.Text = dt.Rows[0]["Status"].ToString();
                        txtNotes.Text = dt.Rows[0]["Notes"].ToString();

                        WorkTable wt = new WorkTable();
                        DataTable dtAcc = wt.RunSelect("SELECT TotalCost FROM SurgeryAccounts WHERE SurgeryId=" + id);
                        if (dtAcc.Rows.Count > 0 && dtAcc.Rows[0]["TotalCost"] != DBNull.Value)
                        {
                            txtTotalCost.Text = Convert.ToDecimal(dtAcc.Rows[0]["TotalCost"]).ToString("0.##");
                        }
                        else
                        {
                            txtTotalCost.Text = "";
                        }

                        ShowMessage("تم تحميل بيانات العملية للتعديل", false);
                    }
                }
                else if (e.CommandName == "DeleteRow")
                {
                    surg.Delete(id);
                    LoadData();
                    ShowMessage("تم الحذف", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ: " + ex.Message);
            }
        }
    }
}