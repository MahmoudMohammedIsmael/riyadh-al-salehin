using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class Patients : System.Web.UI.Page
    {
        private CPatients objPatient = new CPatients();
        private WorkTable wt = new WorkTable(); // تعريف wt

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.IsUserLoggedIn())
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                if (!CPermissions.Check("Patients", "View"))
                {
                    Response.Redirect("~/Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                btnSave.Visible = CPermissions.Check("Patients", "Create");
                btnUpdate.Visible = CPermissions.Check("Patients", "Edit");
                btnDelete.Visible = CPermissions.Check("Patients", "Delete");

                LoadServices();
                LoadPatients();
                LoadCompletedXrayToday();
                LoadAdditionalServicesToday();
            }
        }

        private void LoadServices()
        {
            CMedicalServices service = new CMedicalServices();
            ddlService.DataSource = service.GetAll();
            ddlService.DataTextField = "ServiceName";
            ddlService.DataValueField = "Id";
            ddlService.DataBind();
        }

        private void LoadPatients()
        {
            gvPatients.DataSource = objPatient.GetAll();
            gvPatients.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("Patients", "Create"))
            {
                ShowMessage("ليس لديك صلاحية الحفظ", "error");
                return;
            }

            if (txtPatientName.Text.Trim() == "")
            {
                ShowMessage("يرجى إدخال اسم المريض", "error");
                return;
            }

            objPatient.PatientName = txtPatientName.Text.Trim();
            objPatient.Phone = txtPhone.Text.Trim();
            objPatient.Address = txtAddress.Text.Trim();
            objPatient.DateOfBirth = string.IsNullOrEmpty(txtDateOfBirth.Text) ? (DateTime?)null : Convert.ToDateTime(txtDateOfBirth.Text);
            objPatient.InsuranceCompany = txtInsuranceCompany.Text.Trim();
            objPatient.InsuranceNumber = txtInsuranceNumber.Text.Trim();

            int newPatientId = objPatient.InsertAndReturnId();

            if (newPatientId > 0)
            {
                Response.Redirect(
                    "~/PatientDoctorSchedule.aspx?pid=" + newPatientId +
                    "&ServiceId=" + ddlService.SelectedValue,
                    false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ShowMessage("تعذر الحصول على رقم المريض", "error");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("Patients", "Edit"))
            {
                ShowMessage("ليس لديك صلاحية التعديل", "error");
                return;
            }

            if (hfId.Value == "")
            {
                ShowMessage("اختر مريض أولاً", "warning");
                return;
            }

            objPatient.Id = Convert.ToInt32(hfId.Value);
            objPatient.PatientName = txtPatientName.Text.Trim();
            objPatient.Phone = txtPhone.Text.Trim();
            objPatient.Address = txtAddress.Text.Trim();
            objPatient.DateOfBirth = string.IsNullOrEmpty(txtDateOfBirth.Text) ? (DateTime?)null : Convert.ToDateTime(txtDateOfBirth.Text);
            objPatient.InsuranceCompany = txtInsuranceCompany.Text.Trim();
            objPatient.InsuranceNumber = txtInsuranceNumber.Text.Trim();

            string result = objPatient.Update();
            if (result == "OK")
            {
                ShowMessage("تم التعديل بنجاح", "success");
                LoadPatients();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!CPermissions.Check("Patients", "Delete"))
            {
                ShowMessage("ليس لديك صلاحية الحذف", "error");
                return;
            }

            if (hfId.Value == "")
            {
                ShowMessage("اختر مريض أولاً", "warning");
                return;
            }

            string result = objPatient.Delete(Convert.ToInt32(hfId.Value));
            if (result == "OK")
            {
                ShowMessage("تم الحذف بنجاح", "success");
                LoadPatients();
                ClearForm();
            }
            else
            {
                ShowMessage(result, "error");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e) { ClearForm(); }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() == "")
            {
                LoadPatients();
                return;
            }
            gvPatients.DataSource = objPatient.Search(txtSearch.Text.Trim());
            gvPatients.DataBind();
        }

        protected void gvPatients_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvPatients.SelectedRow;
            hfId.Value = gvPatients.DataKeys[row.RowIndex].Value.ToString();
            txtPatientName.Text = Server.HtmlDecode(row.Cells[1].Text);
            txtPhone.Text = row.Cells[2].Text == "&nbsp;" ? "" : row.Cells[2].Text;
            txtInsuranceCompany.Text = row.Cells[3].Text == "&nbsp;" ? "" : row.Cells[3].Text;

            DataTable dt = objPatient.GetById(Convert.ToInt32(hfId.Value));
            if (dt.Rows.Count > 0)
            {
                txtAddress.Text = dt.Rows[0]["Address"].ToString();
                txtInsuranceNumber.Text = dt.Rows[0]["InsuranceNumber"].ToString();
                if (dt.Rows[0]["DateOfBirth"] != DBNull.Value)
                    txtDateOfBirth.Text = Convert.ToDateTime(dt.Rows[0]["DateOfBirth"]).ToString("yyyy-MM-dd");
            }
        }

        protected void txtPrinted_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrinted.Text))
                return;

            int id;
            if (!int.TryParse(txtPrinted.Text.Trim(), out id))
            {
                ShowMessage("رقم المريض غير صحيح", "warning");
                return;
            }

            DataTable dt = objPatient.GetById(id);
            if (dt.Rows.Count == 0)
            {
                ShowMessage("المريض غير موجود", "warning");
                return;
            }

            DataRow row = dt.Rows[0];
            hfId.Value = row["Id"].ToString();
            txtPatientName.Text = row["PatientName"].ToString();
            txtPhone.Text = row["Phone"].ToString();
            txtAddress.Text = row["Address"].ToString();
            txtInsuranceCompany.Text = row["InsuranceCompany"].ToString();
            txtInsuranceNumber.Text = row["InsuranceNumber"].ToString();
            if (row["DateOfBirth"] != DBNull.Value)
                txtDateOfBirth.Text = Convert.ToDateTime(row["DateOfBirth"]).ToString("yyyy-MM-dd");
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (hfId.Value == "")
            {
                ShowMessage("قم بالاستعلام عن المريض أولاً", "warning");
                return;
            }

            Response.Redirect(
                "~/PatientDoctorSchedule.aspx?pid=" + hfId.Value +
                "&ServiceId=" + ddlService.SelectedValue,
                false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string msg, string type)
        {
            string css = "alert alert-info";
            if (type == "success") css = "alert alert-success";
            if (type == "error") css = "alert alert-danger";
            if (type == "warning") css = "alert alert-warning";
            lblMessage.CssClass = css;
            lblMessage.Text = msg;
        }

        private void ClearForm()
        {
            hfId.Value = "";
            txtPatientName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtDateOfBirth.Text = "";
            txtInsuranceCompany.Text = "";
            txtInsuranceNumber.Text = "";
            lblMessage.Text = "";
        }

        private void LoadCompletedXrayToday()
        {
            try
            {
                string sql = @"
    SELECT r.Id, r.PatientId, 
           ISNULL(r.AppointmentId, 0) AS AppointmentId, 
           ISNULL(r.DoctorId, 0) AS DoctorId, 
           ISNULL(r.IsCalled, 0) AS IsCalled,
           p.PatientName, s.XrayName, d.DoctorName, r.CreatedAt
    FROM XrayRequests r
    INNER JOIN Patients p ON r.PatientId = p.Id
    INNER JOIN XrayServices s ON r.XrayServiceId = s.Id
    LEFT JOIN Doctors d ON r.DoctorId = d.Id
    WHERE r.Status = 'Completed' 
      AND CAST(r.CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
      AND (r.IsCalled IS NULL OR r.IsCalled = 0)
    ORDER BY r.CreatedAt DESC
    ";
                DataTable dt = wt.RunSelect(sql);

                if (dt.Rows.Count == 0)
                {
                    ShowMessage("لا توجد طلبات أشعة مكتملة اليوم في انتظار الإرسال.", "warning");
                }

                gvXrayCompleted.DataSource = dt;
                gvXrayCompleted.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحميل قائمة الأشعة: " + ex.Message, "error");
            }
        }

        protected void gvXrayCompleted_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CallDoctor")
            {
                string arg = e.CommandArgument?.ToString();
                if (string.IsNullOrEmpty(arg))
                {
                    ShowMessage("بيانات الطلب غير صالحة.", "error");
                    return;
                }

                string[] args = arg.Split(',');
                if (args.Length < 3)
                {
                    ShowMessage("بيانات الطلب غير مكتملة.", "error");
                    return;
                }

                int requestId, appointmentId, doctorId;
                if (!int.TryParse(args[0], out requestId) || !int.TryParse(args[1], out appointmentId) || !int.TryParse(args[2], out doctorId))
                {
                    ShowMessage("قيم غير صحيحة في بيانات الطلب.", "error");
                    return;
                }

                if (appointmentId == 0 || doctorId == 0)
                {
                    ShowMessage("لا يمكن إرسال الإشعار لأن بيانات الموعد أو الطبيب غير مكتملة.", "error");
                    return;
                }

                string updateAppointment = "UPDATE Appointments SET Status = 'WaitingForXrayResult' WHERE Id = @AppointmentId";
                var prm1 = new List<SqlParameter> { new SqlParameter("@AppointmentId", appointmentId) };
                string result1 = wt.RunInsDelUpd(updateAppointment, prm1);

                string updateRequest = "UPDATE XrayRequests SET IsCalled = 1 WHERE Id = @RequestId";
                var prm2 = new List<SqlParameter> { new SqlParameter("@RequestId", requestId) };
                string result2 = wt.RunInsDelUpd(updateRequest, prm2);

                if (result1 == "OK" && result2 == "OK")
                {
                    ShowMessage("تم إرسال إشعار للطبيب", "success");
                }
                else
                {
                    ShowMessage("حدث خطأ أثناء التحديث: " + result1 + " / " + result2, "error");
                }

                LoadCompletedXrayToday();
            }
        }

        private void LoadAdditionalServicesToday()
        {
            try
            {
                string sql = @"
SELECT 
    ii.Id,
    ii.InvoiceId,
    p.PatientName,
    s.ServiceName,
    ii.Quantity,
    ii.UnitPrice,
    ii.TotalPrice,
    i.InvoiceDate,
    ISNULL(i.TotalAmount - i.PaidAmount, i.TotalAmount) AS RemainingAmount,
    ISNULL(i.PaymentStatus, 'Unpaid') AS PaymentStatus
FROM InvoiceItems ii
INNER JOIN Invoices i ON ii.InvoiceId = i.Id
INNER JOIN Patients p ON i.PatientId = p.Id
INNER JOIN ServicesAndPrices s ON ii.ServiceId = s.Id
WHERE CAST(i.InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY i.InvoiceDate DESC, ii.Id DESC";

                DataTable dt = wt.RunSelect(sql);
                gvAdditionalServices.DataSource = dt;
                gvAdditionalServices.DataBind();

                int pendingCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    decimal remaining = 0;
                    decimal.TryParse(row["RemainingAmount"].ToString(), out remaining);
                    if (remaining > 0) pendingCount++;
                }

                hfPendingCount.Value = pendingCount.ToString();
                if (pendingCount > 0)
                {
                    lblPendingBadge.Visible = true;
                    lblPendingBadge.Text = "⚠ " + pendingCount + " فاتورة بانتظار التأكيد";
                }
                else
                {
                    lblPendingBadge.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحميل الخدمات الإضافية: " + ex.Message, "error");
            }
        }

        protected void gvAdditionalServices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "PrintService")
            {
                string invoiceId = e.CommandArgument?.ToString();
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    Response.Redirect("~/InvoiceItemsPrint.aspx?InvoiceId=" + invoiceId);
                }
            }
            else if (e.CommandName == "ConfirmService")
            {
                string invoiceId = e.CommandArgument?.ToString();
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    string sql = @"UPDATE Invoices 
                                   SET PaidAmount = TotalAmount, 
                                       PaymentStatus = N'مدفوع' 
                                   WHERE Id = " + invoiceId;
                    
                    string res = wt.RunInsDelUpd(sql);
                    if (res == "OK")
                    {
                        ShowMessage("تم تثبيت الفاتورة بنجاح", "success");
                        LoadAdditionalServicesToday(); // إعادة تحميل الجدول
                    }
                    else
                    {
                        ShowMessage("خطأ أثناء تثبيت الفاتورة: " + res, "error");
                    }
                }
            }
        }
    }
}