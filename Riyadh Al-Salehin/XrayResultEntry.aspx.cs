using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Riyadh_Al_Salehin
{
    public partial class XrayResultEntry : System.Web.UI.Page
    {
        private CXrayRequests req = new CXrayRequests();
        private CXrayResults res = new CXrayResults();
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
                if (Request.QueryString["RequestId"] == null)
                {
                    Response.Redirect("XrayTechInbox.aspx");
                    return;
                }

                int requestId = Convert.ToInt32(Request.QueryString["RequestId"]);
                hfRequestId.Value = requestId.ToString();
                LoadRequestDetails(requestId);
            }
        }

        private void LoadRequestDetails(int requestId)
        {
            DataTable dt = req.GetAll();
            DataRow[] rows = dt.Select("Id = " + requestId);
            if (rows.Length > 0)
            {
                txtPatientName.Text = rows[0]["PatientName"]?.ToString() ?? "غير معروف";
                txtServiceName.Text = rows[0]["XrayName"]?.ToString() ?? "غير محدد";
            }
            else
            {
                lblMessage.Text = "الطلب غير موجود.";
                lblMessage.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int requestId = Convert.ToInt32(hfRequestId.Value);

                res.RequestId = requestId;
                res.ResultText = txtResult.Text;

                if (fileUpload.HasFile)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string folderPath = Server.MapPath("~/Uploads/Xray/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string savePath = Path.Combine(folderPath, requestId + "_" + fileName);
                    fileUpload.SaveAs(savePath);
                    res.FilePath = "~/Uploads/Xray/" + requestId + "_" + fileName;
                }

                string resultInsert = res.Insert();

                if (resultInsert == "OK")
                {
                    req.UpdateStatus(requestId, "Completed");

                    DataTable dt = req.GetById(requestId);
                    if (dt.Rows.Count > 0)
                    {
                        object appointmentIdObj = dt.Rows[0]["AppointmentId"];
                        if (appointmentIdObj != DBNull.Value && appointmentIdObj != null)
                        {
                            int appointmentId = Convert.ToInt32(appointmentIdObj);
                            string updateApp = "UPDATE Appointments SET Status = 'XrayDone' WHERE Id = @AppointmentId";
                            var prm = new List<SqlParameter> { new SqlParameter("@AppointmentId", appointmentId) };
                            wt.RunInsDelUpd(updateApp, prm);

                            lblMessage.Text = "✅ تم حفظ التقرير وتحديث حالة الموعد إلى 'XrayDone'.";
                        }
                        else
                        {
                            lblMessage.Text = "⚠️ تم حفظ التقرير، ولكن لم يتم العثور على موعد مرتبط لتحديث حالته.";
                        }
                    }

                    lblMessage.Visible = true;
                    btnSave.Enabled = false;
                }
                else
                {
                    lblMessage.Text = "❌ خطأ في حفظ التقرير: " + resultInsert;
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ خطأ: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("XrayTechInbox.aspx");
        }
    }
}