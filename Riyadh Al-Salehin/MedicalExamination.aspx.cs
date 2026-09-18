using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Services;
using System.Linq;

namespace Riyadh_Al_Salehin
{
    public partial class MedicalExamination : System.Web.UI.Page
    {
        WorkTable wt = new WorkTable();

        private List<Dictionary<string, string>> SelectedMedicines
        {
            get
            {
                if (ViewState["SelectedMedicines"] == null)
                    ViewState["SelectedMedicines"] = new List<Dictionary<string, string>>();
                return (List<Dictionary<string, string>>)ViewState["SelectedMedicines"];
            }
            set
            {
                ViewState["SelectedMedicines"] = value;
            }
        }

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
                if (Request.QueryString["AppointmentId"] == null)
                {
                    Response.Redirect("DoctorWaitingPatients.aspx");
                    return;
                }
                LoadAppointment();
                LoadPatientHistory();
                gvSuggestedMedicines.DataSource = null;
                gvSuggestedMedicines.DataBind();
                gvSelectedMedicines.DataSource = null;
                gvSelectedMedicines.DataBind();
                txtDiagnosisSearch.Text = "";
                txtManualMedicineSearch.Text = "";
            }
        }

        [WebMethod]
        public static List<string> GetDiagnoses(string prefixText, int count)
        {
            List<string> result = new List<string>();
            WorkTable wt = new WorkTable();

            string sql = @"
SELECT TOP (@Count) 
    CAST(d.Id AS NVARCHAR(10)) + '|' + 
    ISNULL(d.DiagnosisArabic, '') + ' - ' + 
    ISNULL(d.DiagnosisEnglish, '') AS DisplayText
FROM Diagnoses d
WHERE d.IsActive = 1
  AND (d.DiagnosisArabic LIKE @Prefix + '%' OR d.DiagnosisEnglish LIKE @Prefix + '%')
ORDER BY d.DiagnosisEnglish";

            var prm = new List<SqlParameter>
    {
        new SqlParameter("@Prefix", prefixText),
        new SqlParameter("@Count", count)
    };

            DataTable dt = wt.RunSelect(sql, prm);
            foreach (DataRow row in dt.Rows)
            {
                string display = row["DisplayText"].ToString();
                if (!string.IsNullOrEmpty(display))
                    result.Add(display);
            }
            return result;
        }


        [WebMethod]
        public static List<string> GetMedicines(string prefixText, int count)
        {
            List<string> result = new List<string>();
            WorkTable wt = new WorkTable();

            string sql = @"
SELECT TOP (@Count) 
    CAST(Id AS NVARCHAR(10)) + '|' + 
    MedicineName + ' - ' + ISNULL(GenericName, '') AS DisplayText
FROM Medicines
WHERE IsActive = 1 
  AND (MedicineName LIKE @Prefix + '%' OR GenericName LIKE @Prefix + '%')
ORDER BY MedicineName";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@Prefix", prefixText),
                new SqlParameter("@Count", count)
            };

            DataTable dt = wt.RunSelect(sql, prm);
            foreach (DataRow row in dt.Rows)
            {
                result.Add(row["DisplayText"].ToString());
            }
            return result;
        }

        protected void txtDiagnosisSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtDiagnosisSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                gvSuggestedMedicines.DataSource = null;
                gvSuggestedMedicines.DataBind();
                lblDiagnosisMessage.Text = "";
                return;
            }

            int diagnosisId = 0;
            if (searchText.Contains("|"))
            {
                string idPart = searchText.Split('|')[0];
                if (int.TryParse(idPart, out diagnosisId))
                {
                    hfDiagnosisId.Value = diagnosisId.ToString();
                    BindSuggestedMedicines(diagnosisId);
                    lblDiagnosisMessage.Text = "تم عرض الأدوية المقترحة.";
                    lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    diagnosisId = GetDiagnosisIdByName(searchText);
                    if (diagnosisId > 0)
                    {
                        hfDiagnosisId.Value = diagnosisId.ToString();
                        BindSuggestedMedicines(diagnosisId);
                        lblDiagnosisMessage.Text = "تم عرض الأدوية المقترحة.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        gvSuggestedMedicines.DataSource = null;
                        gvSuggestedMedicines.DataBind();
                        lblDiagnosisMessage.Text = "لم يتم العثور على تشخيص مطابق.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else
            {
                diagnosisId = GetDiagnosisIdByName(searchText);
                if (diagnosisId > 0)
                {
                    hfDiagnosisId.Value = diagnosisId.ToString();
                    BindSuggestedMedicines(diagnosisId);
                    lblDiagnosisMessage.Text = "تم عرض الأدوية المقترحة.";
                    lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    gvSuggestedMedicines.DataSource = null;
                    gvSuggestedMedicines.DataBind();
                    lblDiagnosisMessage.Text = "لم يتم العثور على تشخيص مطابق.";
                    lblDiagnosisMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private int GetDiagnosisIdByName(string name)
        {
            string sql = @"
SELECT TOP 1 Id 
FROM Diagnoses 
WHERE IsActive = 1 
  AND (DiagnosisArabic LIKE @Name + '%' OR DiagnosisEnglish LIKE @Name + '%')
ORDER BY DiagnosisEnglish";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@Name", name)
            };
            DataTable dt = wt.RunSelect(sql, prm);
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["Id"]);
            return 0;
        }



        private string GetDiagnosisName(int diagnosisId)
        {
            string sql = "SELECT DiagnosisArabic, DiagnosisEnglish FROM Diagnoses WHERE Id = @Id";
            var prm = new List<SqlParameter> { new SqlParameter("@Id", diagnosisId) };
            DataTable dt = wt.RunSelect(sql, prm);
            if (dt.Rows.Count > 0)
            {
                string arabic = dt.Rows[0]["DiagnosisArabic"]?.ToString();
                string english = dt.Rows[0]["DiagnosisEnglish"]?.ToString();
                return !string.IsNullOrEmpty(arabic) ? arabic : english;
            }
            return "";
        }

        private void BindSuggestedMedicines(int diagnosisId)
        {
            string sql = @"
SELECT 
    M.MedicineName,
    M.GenericName,
    M.Strength,
    M.Form,
    DM.IsPreferred,
    DM.Notes
FROM DiagnosisMedicines DM
INNER JOIN Medicines M ON DM.MedicineId = M.Id
WHERE DM.DiagnosisId = @DiagnosisId 
  AND M.IsActive = 1
ORDER BY DM.IsPreferred DESC, M.MedicineName";

            var prm1 = new List<SqlParameter> { new SqlParameter("@DiagnosisId", diagnosisId) };
            DataTable dt = wt.RunSelect(sql, prm1);

            string diagnosisName = GetDiagnosisName(diagnosisId);
            if (string.IsNullOrEmpty(diagnosisName))
                diagnosisName = "تشخيص غير معروف";

            if (dt.Rows.Count > 0)
            {
                DataRow newRow = dt.NewRow();
                newRow["MedicineName"] = "▶ " + diagnosisName;
                newRow["GenericName"] = "التشخيص المختار";
                newRow["Strength"] = "";
                newRow["Form"] = "";
                newRow["IsPreferred"] = false;
                newRow["Notes"] = "انقر اختيار لإضافة هذا التشخيص إلى القائمة";
                dt.Rows.InsertAt(newRow, 0);

                gvSuggestedMedicines.DataSource = dt;
                gvSuggestedMedicines.DataBind();
                lblDiagnosisMessage.Text = "✅ تم عرض الأدوية المقترحة مع التشخيص.";
                lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                return;
            }

            string fallbackSql = @"
SELECT DISTINCT
    M.MedicineName,
    M.GenericName,
    M.Strength,
    M.Form,
    DM.IsPreferred,
    DM.Notes
FROM Diagnoses D2
INNER JOIN DiagnosisMedicines DM ON DM.DiagnosisId = D2.Id
INNER JOIN Medicines M ON DM.MedicineId = M.Id
WHERE D2.Category = (SELECT Category FROM Diagnoses WHERE Id = @DiagnosisId)
  AND D2.Category IS NOT NULL
  AND M.IsActive = 1
ORDER BY M.MedicineName";

            var prm2 = new List<SqlParameter> { new SqlParameter("@DiagnosisId", diagnosisId) };
            DataTable dtFallback = wt.RunSelect(fallbackSql, prm2);

            if (dtFallback.Rows.Count > 0)
            {
                DataRow newRow = dtFallback.NewRow();
                newRow["MedicineName"] = "▶ " + diagnosisName;
                newRow["GenericName"] = "التشخيص المختار";
                newRow["Strength"] = "";
                newRow["Form"] = "";
                newRow["IsPreferred"] = false;
                newRow["Notes"] = "انقر اختيار لإضافة هذا التشخيص إلى القائمة";
                dtFallback.Rows.InsertAt(newRow, 0);

                gvSuggestedMedicines.DataSource = dtFallback;
                gvSuggestedMedicines.DataBind();
                lblDiagnosisMessage.Text = "⚠ لا توجد أدوية مرتبطة مباشرة، هذه أدوية من نفس التصنيف، مع إضافة التشخيص.";
                lblDiagnosisMessage.ForeColor = System.Drawing.Color.OrangeRed;
                return;
            }

            DataTable dtEmpty = new DataTable();
            dtEmpty.Columns.Add("MedicineName");
            dtEmpty.Columns.Add("GenericName");
            dtEmpty.Columns.Add("Strength");
            dtEmpty.Columns.Add("Form");
            dtEmpty.Columns.Add("IsPreferred", typeof(bool));
            dtEmpty.Columns.Add("Notes");

            DataRow rowDiagnosis = dtEmpty.NewRow();
            rowDiagnosis["MedicineName"] = "▶ " + diagnosisName;
            rowDiagnosis["GenericName"] = "التشخيص المختار";
            rowDiagnosis["Strength"] = "";
            rowDiagnosis["Form"] = "";
            rowDiagnosis["IsPreferred"] = false;
            rowDiagnosis["Notes"] = "انقر اختيار لإضافة هذا التشخيص إلى القائمة";
            dtEmpty.Rows.Add(rowDiagnosis);

            gvSuggestedMedicines.DataSource = dtEmpty;
            gvSuggestedMedicines.DataBind();
            lblDiagnosisMessage.Text = "❌ لا توجد أدوية مقترحة لهذا التشخيص، ولكن يمكنك اختيار التشخيص نفسه.";
            lblDiagnosisMessage.ForeColor = System.Drawing.Color.Orange;
        }

        protected void gvSuggestedMedicines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectMedicine")
            {
                string args = e.CommandArgument.ToString();
                string[] parts = args.Split('|');

                if (parts.Length >= 4 && parts[0].StartsWith("▶ "))
                {
                    string diagnosisName = parts[0].Replace("▶ ", "");
                    var medicine = new Dictionary<string, string>
            {
                { "MedicineName", diagnosisName },
                { "GenericName", "تشخيص" },
                { "Strength", "" },
                { "Form", "" }
            };
                    var list = SelectedMedicines;
                    if (!list.Any(m => m["MedicineName"] == diagnosisName && m["GenericName"] == "تشخيص"))
                    {
                        list.Add(medicine);
                        SelectedMedicines = list;
                        BindSelectedMedicines();
                        lblDiagnosisMessage.Text = "✅ تم إضافة التشخيص إلى القائمة.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblDiagnosisMessage.Text = "⚠ هذا التشخيص مضاف مسبقاً.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Orange;
                    }
                    return;
                }

                if (parts.Length >= 4)
                {
                    var medicine = new Dictionary<string, string>
            {
                { "MedicineName", parts[0] },
                { "GenericName", parts[1] },
                { "Strength", parts[2] },
                { "Form", parts[3] }
            };
                    var list = SelectedMedicines;
                    if (!list.Any(m => m["MedicineName"] == parts[0] && m["GenericName"] == parts[1]))
                    {
                        list.Add(medicine);
                        SelectedMedicines = list;
                        BindSelectedMedicines();
                        lblDiagnosisMessage.Text = "✅ تم إضافة الدواء.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblDiagnosisMessage.Text = "⚠ هذا الدواء مضاف مسبقاً.";
                        lblDiagnosisMessage.ForeColor = System.Drawing.Color.Orange;
                    }
                }
            }
        }









        protected void txtManualMedicineSearch_TextChanged(object sender, EventArgs e)
        {
        }

        protected void btnAddMedicineFromManual_Click(object sender, EventArgs e)
        {
            string value = hfSelectedMedicine.Value;
            if (!string.IsNullOrEmpty(value) && value.Contains("|"))
            {
                string[] parts = value.Split('|');
                if (parts.Length >= 2)
                {
                    int medicineId = Convert.ToInt32(parts[0]);
                    AddMedicineById(medicineId);
                }
            }
            hfSelectedMedicine.Value = ""; // مسح القيمة بعد الإضافة
        }

        private void AddMedicineById(int medicineId)
        {
            string sql = "SELECT MedicineName, GenericName, Strength, Form FROM Medicines WHERE Id = @Id";
            var prm = new List<SqlParameter> { new SqlParameter("@Id", medicineId) };
            DataTable dt = wt.RunSelect(sql, prm);
            if (dt.Rows.Count > 0)
            {
                var medicine = new Dictionary<string, string>
                {
                    { "MedicineName", dt.Rows[0]["MedicineName"].ToString() },
                    { "GenericName", dt.Rows[0]["GenericName"].ToString() },
                    { "Strength", dt.Rows[0]["Strength"].ToString() },
                    { "Form", dt.Rows[0]["Form"].ToString() }
                };
                var list = SelectedMedicines;
                if (!list.Any(m => m["MedicineName"] == medicine["MedicineName"] && m["GenericName"] == medicine["GenericName"]))
                {
                    list.Add(medicine);
                    SelectedMedicines = list;
                    BindSelectedMedicines();
                }
                else
                {
                }
            }
        }

        private void BindSelectedMedicines()
        {
            var list = SelectedMedicines;
            DataTable dt = new DataTable();
            dt.Columns.Add("MedicineName");
            dt.Columns.Add("GenericName");
            dt.Columns.Add("Strength");
            dt.Columns.Add("Form");
            foreach (var item in list)
            {
                dt.Rows.Add(item["MedicineName"], item["GenericName"], item["Strength"], item["Form"]);
            }
            gvSelectedMedicines.DataSource = dt;
            gvSelectedMedicines.DataBind();
        }

        protected void gvSelectedMedicines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveMedicine")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var list = SelectedMedicines;
                if (index < list.Count)
                {
                    list.RemoveAt(index);
                    SelectedMedicines = list;
                    BindSelectedMedicines();
                }
            }
        }

        protected void btnClearManualSearch_Click(object sender, EventArgs e)
        {
            txtManualMedicineSearch.Text = "";
        }

        private void LoadAppointment()
        {
            try
            {
                int appointmentId = Convert.ToInt32(Request.QueryString["AppointmentId"]);

                string sql = @"
SELECT 
    A.Id,
    A.PatientId,
    A.DoctorId,
    A.QueueNumber,
    A.AppointmentDate,
    A.Status,
    P.PatientName,
    P.Phone,
    P.InsuranceCompany,
    D.DoctorName,
    D.Specialty,
    ISNULL(I.PaymentStatus, N'غير مدفوع') AS PaymentStatus,
    MS.ServiceName,
    CASE 
        WHEN MS.ServiceName LIKE N'%استشارة%' 
        THEN 'Consultation'
        ELSE 'Exam'
    END AS VisitType
FROM Appointments A
INNER JOIN Patients P ON A.PatientId = P.Id
INNER JOIN Doctors D ON A.DoctorId = D.Id
LEFT JOIN Invoices I ON I.AppointmentId = A.Id
LEFT JOIN MedicalServices MS ON I.ServiceId = MS.Id
WHERE A.Id = @AppointmentId";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@AppointmentId", appointmentId)
                };

                DataTable dt = wt.RunSelect(sql, prm);

                if (dt.Rows.Count == 0)
                {
                    lblMessage.Text = "الموعد غير موجود";
                    return;
                }

                DataRow r = dt.Rows[0];

                hfAppointmentId.Value = r["Id"].ToString();
                hfPatientId.Value = r["PatientId"].ToString();
                hfDoctorId.Value = r["DoctorId"].ToString();

                lblAppointmentId.Text = r["Id"].ToString();
                txtPatientName.Text = r["PatientName"].ToString();
                txtPatientId.Text = r["PatientId"].ToString();
                txtDoctor.Text = r["DoctorName"].ToString();
                txtPhone.Text = r["Phone"].ToString();
                txtSpecialty.Text = r["Specialty"].ToString();
                txtQueue.Text = r["QueueNumber"].ToString();
                txtAppointmentDate.Text = Convert.ToDateTime(r["AppointmentDate"]).ToString("yyyy-MM-dd HH:mm");
                txtPaymentStatus.Text = r["PaymentStatus"].ToString();
                txtInsurance.Text = r["InsuranceCompany"]?.ToString() ?? "";

                string visitType = r["VisitType"]?.ToString() ?? "";
                if (visitType == "Consultation")
                {
                    int patientId = Convert.ToInt32(r["PatientId"]);
                    string lastComplaint = GetLastComplaint(patientId);
                    if (!string.IsNullOrEmpty(lastComplaint))
                    {
                        txtComplaint.Text = lastComplaint;
                        txtNotes.Text = "هذه الشكوى من آخر زيارة (استشارة سابقة)";
                    }
                }

                int patientIdForXray = Convert.ToInt32(r["PatientId"]);
                CheckPatientXrayHistory(patientIdForXray);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private string GetLastComplaint(int patientId)
        {
            string sql = @"
SELECT TOP 1 
    me.InitialDiagnosis AS Complaint
FROM MedicalExaminations me
INNER JOIN Appointments a ON me.AppointmentId = a.Id
WHERE a.PatientId = @PatientId
ORDER BY me.CreatedAt DESC";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@PatientId", patientId)
            };
            DataTable dt = wt.RunSelect(sql, prm);
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["Complaint"].ToString();
            return ""; // إضافة return فارغ لإصلاح الخطأ
        }

        private void CheckPatientXrayHistory(int patientId)
        {
            try
            {
                string sql = @"
SELECT COUNT(*) 
FROM XrayRequests r
INNER JOIN XrayResults res ON res.RequestId = r.Id
WHERE r.PatientId = @PatientId";

                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@PatientId", patientId)
                };

                DataTable dt = wt.RunSelect(sql, prm);
                int count = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : 0;
                btnViewXray.Visible = (count > 0);
            }
            catch
            {
                btnViewXray.Visible = false;
            }
        }

        private void LoadPatientHistory()
        {
            if (string.IsNullOrEmpty(hfPatientId.Value))
                return;

            int patientId = Convert.ToInt32(hfPatientId.Value);
            string sql = @"
SELECT TOP 5 
    me.CreatedAt AS ExaminationDate,
    me.InitialDiagnosis,
    me.Notes,
    d.DoctorName
FROM MedicalExaminations me
INNER JOIN Appointments a ON me.AppointmentId = a.Id
INNER JOIN Doctors d ON a.DoctorId = d.Id
WHERE a.PatientId = @PatientId
ORDER BY me.CreatedAt DESC";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@PatientId", patientId)
            };

            DataTable dt = wt.RunSelect(sql, prm);
            if (dt.Rows.Count > 0)
            {
                gvPatientHistory.DataSource = dt;
                gvPatientHistory.DataBind();
                divHistory.Visible = true;
            }
            else
            {
                divHistory.Visible = false;
            }
        }

        protected void btnViewXray_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/XrayReportsView.aspx?PatientId={hfPatientId.Value}&AppointmentId={hfAppointmentId.Value}");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int appointmentId = Convert.ToInt32(hfAppointmentId.Value);

                string decisionType = "New"; // قيمة افتراضية
                int diagnosisId = 0;
                if (int.TryParse(hfDiagnosisId.Value, out diagnosisId) && diagnosisId > 0)
                {
                    string diagnosisName = GetDiagnosisName(diagnosisId);
                    if (!string.IsNullOrEmpty(diagnosisName))
                        decisionType = diagnosisName;
                }

                string notes =
                    "Complaint: " + txtComplaint.Text + "\n" +
                    "History: " + txtMedicalHistory.Text + "\n" +
                    "Clinical: " + txtClinicalExam.Text + "\n" +
                    "Diagnosis: " + txtDiagnosis.Text + "\n" +
                    "Doctor Notes: " + txtNotes.Text;

                string sql = @"INSERT INTO MedicalExaminations
                       (AppointmentId, InitialDiagnosis, DecisionType, Status, Notes)
                       VALUES
                       (@AppointmentId, @InitialDiagnosis, @DecisionType, 'Active', @Notes)";

                var prm = new List<SqlParameter>
        {
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@InitialDiagnosis", txtDiagnosis.Text),
            new SqlParameter("@DecisionType", decisionType),
            new SqlParameter("@Notes", notes)
        };

                string result = wt.RunInsDelUpd(sql, prm);

                if (result == "OK")
                {
                    lblMessage.Text = "تم حفظ الكشف بنجاح ✔";
                    btnFinish.Enabled = true;
                }
                else
                {
                    lblMessage.Text = result;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

protected void btnServices_Click(object sender, EventArgs e)
        {
            int patientId = Convert.ToInt32(hfPatientId.Value);
            int appointmentId = Convert.ToInt32(hfAppointmentId.Value);

            Response.Redirect(
                "~/AllPatientsInvoices.aspx?PatientId=" + patientId + "&AppointmentId=" + appointmentId,
                false
            );
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                int appointmentId = Convert.ToInt32(hfAppointmentId.Value);
                string sql = @"UPDATE Appointments SET Status = 'Finished' WHERE Id = @AppointmentId";
                var prm = new List<SqlParameter>
                {
                    new SqlParameter("@AppointmentId", appointmentId)
                };
                wt.RunInsDelUpd(sql, prm);
                lblMessage.Text = "تم إنهاء الكشف ✔";
                btnFinish.Enabled = false;

                Response.Redirect("~/DoctorWaitingPatients.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnRay_Click(object sender, EventArgs e)
        {
            try
            {
                int appointmentId = Convert.ToInt32(hfAppointmentId.Value);
                int patientId = Convert.ToInt32(hfPatientId.Value);
                int doctorId = Convert.ToInt32(hfDoctorId.Value);

                string fullNotes =
                    "تم التحويل للأشعة." + Environment.NewLine +
                    "التشخيص: " + txtDiagnosis.Text + Environment.NewLine +
                    "الشكوى: " + txtComplaint.Text + Environment.NewLine +
                    "ملاحظات الطبيب: " + txtNotes.Text;

                int examinationId = GetOrCreateExamination(appointmentId, txtDiagnosis.Text, fullNotes);

                string sql = @"
INSERT INTO MedicalReferrals (ExaminationId, Type, Notes)
VALUES (@ExaminationId, 'xray', @Notes);
SELECT SCOPE_IDENTITY();";

                List<SqlParameter> prm = new List<SqlParameter>
                {
                    new SqlParameter("@ExaminationId", examinationId),
                    new SqlParameter("@Notes", "تحويل للأشعة بواسطة الطبيب. التشخيص: " + txtDiagnosis.Text)
                };

                DataTable dt = wt.RunSelect(sql, prm);
                if (dt.Rows.Count == 0)
                    throw new Exception("تعذر إنشاء تحويل الأشعة.");

                int referralId = Convert.ToInt32(dt.Rows[0][0]);

                Response.Redirect(
                    "~/XrayRequests.aspx?PatientId=" + patientId +
                    "&ReferralId=" + referralId +
                    "&DoctorId=" + doctorId +
                    "&AppointmentId=" + appointmentId +
                    "&Diagnosis=" + Server.UrlEncode(txtDiagnosis.Text),
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "خطأ أثناء تحويل المريض للأشعة : " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        private int GetOrCreateExamination(int appointmentId, string diagnosis, string notes)
        {
            string checkSql = "SELECT Id FROM MedicalExaminations WHERE AppointmentId = @AppointmentId";
            var checkPrm = new List<SqlParameter> { new SqlParameter("@AppointmentId", appointmentId) };
            DataTable dt = wt.RunSelect(checkSql, checkPrm);

            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["Id"]);
            }

            string insertSql = @"
INSERT INTO MedicalExaminations (AppointmentId, InitialDiagnosis, DecisionType, Status, Notes)
VALUES (@AppointmentId, @InitialDiagnosis, 'New', 'Active', @Notes);
SELECT SCOPE_IDENTITY();";

            var prm = new List<SqlParameter>
            {
                new SqlParameter("@AppointmentId", appointmentId),
                new SqlParameter("@InitialDiagnosis", string.IsNullOrEmpty(diagnosis) ? "تحويل للأشعة" : diagnosis),
                new SqlParameter("@Notes", notes)
            };

            DataTable resultDt = wt.RunSelect(insertSql, prm);
            if (resultDt.Rows.Count > 0)
            {
                return Convert.ToInt32(resultDt.Rows[0][0]);
            }
            else
            {
                throw new Exception("تعذر إنشاء سجل الكشف الطبي.");
            }
        }
    }
}