using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Riyadh_Al_Salehin
{
    public partial class SurgeryInvoice : System.Web.UI.Page
    {
        private WorkTable wt = new WorkTable();
        private CSurgeryInvoices inv = new CSurgeryInvoices();

        private decimal DoctorCost
        {
            get => ViewState["DoctorCost"] == null ? 0 : (decimal)ViewState["DoctorCost"];
            set => ViewState["DoctorCost"] = value;
        }

        private decimal RoomCost
        {
            get => ViewState["RoomCost"] == null ? 0 : (decimal)ViewState["RoomCost"];
            set => ViewState["RoomCost"] = value;
        }

        private decimal SuppliesCost
        {
            get => ViewState["SuppliesCost"] == null ? 0 : (decimal)ViewState["SuppliesCost"];
            set => ViewState["SuppliesCost"] = value;
        }

        private bool IsPayRemainingMode
        {
            get => ViewState["IsPayRemainingMode"] != null && (bool)ViewState["IsPayRemainingMode"];
            set => ViewState["IsPayRemainingMode"] = value;
        }

        private decimal RemainingBaseAmount
        {
            get => ViewState["RemainingBaseAmount"] == null ? 0 : (decimal)ViewState["RemainingBaseAmount"];
            set => ViewState["RemainingBaseAmount"] = value;
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
                if (Session["PayRemaining_Mode"] != null && (bool)Session["PayRemaining_Mode"] == true)
                {
                    LoadPayRemainingMode();
                }
                else
                {
                    LoadParameters();
                }
            }
        }

        private void LoadPayRemainingMode()
        {
            int surgeryId = Convert.ToInt32(Session["PayRemaining_SurgeryId"]);
            int originalInvoiceId = Convert.ToInt32(Session["PayRemaining_InvoiceId"]);
            decimal remainingAmount = Convert.ToDecimal(Session["PayRemaining_Amount"]);
            string patientName = Session["PayRemaining_PatientName"] as string ?? "";
            string doctorName = Session["PayRemaining_DoctorName"] as string ?? "";
            string surgeryName = Session["PayRemaining_SurgeryName"] as string ?? "";

            IsPayRemainingMode = true;
            RemainingBaseAmount = remainingAmount;

            hfSurgeryId.Value = surgeryId.ToString();
            txtSurgeryId.Text = surgeryId.ToString();
            txtPatient.Text = patientName;
            txtDoctor.Text = doctorName;
            txtSurgeryName.Text = surgeryName;
            txtDate.Text = DateTime.Now.ToString("yyyy/MM/dd");

            decimal origDoctor = 0, origRoom = 0, origSupplies = 0, origOther = 0, origDiscount = 0;
            decimal origTotal = 0, origPaid = 0, origRemaining = remainingAmount;
            string origRoomName = "";
            int origPatientId = 0;

            DataTable origInv = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE Id=" + originalInvoiceId);
            if (origInv.Rows.Count > 0)
            {
                DataRow invRow = origInv.Rows[0];
                origDoctor = ToDecimal(invRow["DoctorCost"]);
                origRoom = ToDecimal(invRow["RoomCost"]);
                origSupplies = ToDecimal(invRow["SuppliesCost"]);
                origOther = ToDecimal(invRow["OtherCost"]);
                origDiscount = ToDecimal(invRow["Discount"]);
                origTotal = ToDecimal(invRow["TotalAmount"]);
                origPaid = ToDecimal(invRow["PaidAmount"]);
                origRemaining = ToDecimal(invRow["RemainingAmount"]);
                if (origRemaining <= 0) origRemaining = remainingAmount;

                if (invRow.Table.Columns.Contains("PatientId") && invRow["PatientId"] != DBNull.Value)
                    origPatientId = Convert.ToInt32(invRow["PatientId"]);
            }

            DataTable dtSurg = wt.RunSelect(@"
SELECT s.PatientId, ISNULL(r.RoomName, '') AS RoomName, s.SurgeryDate
FROM Surgeries s
LEFT JOIN OperationRooms r ON s.RoomId = r.Id
WHERE s.Id = " + surgeryId);
            if (dtSurg.Rows.Count > 0)
            {
                origRoomName = dtSurg.Rows[0]["RoomName"].ToString();
                if (origPatientId == 0 && dtSurg.Rows[0]["PatientId"] != DBNull.Value)
                    origPatientId = Convert.ToInt32(dtSurg.Rows[0]["PatientId"]);
                if (dtSurg.Rows[0]["SurgeryDate"] != DBNull.Value)
                    txtDate.Text = Convert.ToDateTime(dtSurg.Rows[0]["SurgeryDate"]).ToString("yyyy/MM/dd");
            }

            hfPatientId.Value = origPatientId.ToString();
            txtRoom.Text = origRoomName;

            decimal remainingToAllocate = origRemaining;
            decimal paidLeft = origPaid;

            decimal unpaidDoctor = Math.Max(0, origDoctor - paidLeft);
            paidLeft = Math.Max(0, paidLeft - origDoctor);

            decimal unpaidRoom = Math.Max(0, origRoom - paidLeft);
            paidLeft = Math.Max(0, paidLeft - origRoom);

            decimal unpaidSupplies = Math.Max(0, origSupplies - paidLeft);
            paidLeft = Math.Max(0, paidLeft - origSupplies);

            decimal unpaidOther = Math.Max(0, origOther - paidLeft);

            if (unpaidDoctor + unpaidRoom + unpaidSupplies + unpaidOther <= 0 && remainingToAllocate > 0)
            {
                unpaidRoom = 0;
                unpaidDoctor = 0;
                unpaidSupplies = 0;
                unpaidOther = 0;
            }

            txtDoctorCost.Text = origDoctor.ToString("0.00");
            txtRoomCost.Text = origRoom.ToString("0.00");
            txtSuppliesCost.Text = origSupplies.ToString("0.00");
            txtOtherCost.Text = origOther.ToString("0.00");
            txtDiscount.Text = origDiscount.ToString("0.00");

            DoctorCost = origDoctor;
            RoomCost = origRoom;
            SuppliesCost = origSupplies;
            RemainingBaseAmount = origRemaining;

            txtTotal.Text = origRemaining.ToString("0.00");
            txtPaid.Text = "0.00";
            txtRemaining.Text = origRemaining.ToString("0.00");
            ddlStatus.SelectedValue = "Unpaid";

            string remainingSource = GetRemainingSourceLabel(unpaidDoctor, unpaidRoom, unpaidSupplies, unpaidOther);

            DataTable dt = new DataTable();
            dt.Columns.Add("ItemName");
            dt.Columns.Add("Amount");
            dt.Rows.Add("أجر الطبيب الأصلي", origDoctor.ToString("0.00"));
            dt.Rows.Add("تكلفة الغرفة الأصلية", origRoom.ToString("0.00"));
            dt.Rows.Add("تكلفة المستلزمات الأصلية", origSupplies.ToString("0.00"));
            dt.Rows.Add("تكاليف إضافية أصلية", origOther.ToString("0.00"));
            dt.Rows.Add("الخصم الأصلي", origDiscount.ToString("0.00"));
            dt.Rows.Add("إجمالي الفاتورة الأصلية", origTotal.ToString("0.00"));
            dt.Rows.Add("المدفوع سابقاً", origPaid.ToString("0.00"));
            dt.Rows.Add("المتبقي للدفع - " + remainingSource, origRemaining.ToString("0.00"));
            gvInvoice.DataSource = dt;
            gvInvoice.DataBind();

            FillPrintData();

            lblPayRemainingNotice.Visible = true;
            lblPayRemainingNotice.Text =
                "⚠️ وضع دفع المتبقي — " +
                "المبلغ المراد تحصيله: <strong style='font-size:20px;color:#dc3545;'>" + origRemaining.ToString("N2") + " ج.م</strong>" +
                " | إجمالي الفاتورة الأصلية: " + origTotal.ToString("N2") +
                " | مدفوع سابقاً: " + origPaid.ToString("N2") +
                "<br/><small>حقول التكاليف أعلاه معروضة للمعلومية فقط. أدخل المبلغ المدفوع الآن في حقل 'المدفوع' ثم احفظ.</small>";

            ShowAlert("وضع دفع المتبقي: المبلغ المطلوب " + origRemaining.ToString("0.00") + " ج.م");
        }

        private static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            decimal result;
            decimal.TryParse(value.ToString(), out result);
            return result;
        }

        private static string GetRemainingSourceLabel(decimal unpaidDoctor, decimal unpaidRoom, decimal unpaidSupplies, decimal unpaidOther)
        {
            var parts = new List<string>();
            if (unpaidRoom > 0) parts.Add("تكلفة الغرف " + unpaidRoom.ToString("0.00"));
            if (unpaidSupplies > 0) parts.Add("المستلزمات " + unpaidSupplies.ToString("0.00"));
            if (unpaidOther > 0) parts.Add("تكاليف إضافية " + unpaidOther.ToString("0.00"));
            if (unpaidDoctor > 0) parts.Add("أجر الطبيب " + unpaidDoctor.ToString("0.00"));
            return parts.Count > 0 ? string.Join(" + ", parts) : "المتبقي من الفاتورة الأصلية";
        }

        private void LoadParameters()
        {
            if (Request["sid"] == null)
                return;

            int sid;
            if (!int.TryParse(Request["sid"], out sid))
                return;

            hfSurgeryId.Value = sid.ToString();
            txtSurgeryId.Text = sid.ToString();

            if (Request["pid"] != null)
            {
                int pid;
                if (int.TryParse(Request["pid"], out pid))
                    hfPatientId.Value = pid.ToString();
            }

            LoadInvoice();
        }

        private void LoadInvoice()
        {
            int sid = Convert.ToInt32(hfSurgeryId.Value);

            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("@Id", sid));

            DataTable dt = wt.RunSelect(@"
SELECT *
FROM vw_SurgeryInvoice
WHERE SurgeryId=@Id", prm);

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            txtPatient.Text = dr["PatientName"].ToString();
            txtDoctor.Text = dr["DoctorName"].ToString();
            txtRoom.Text = dr["RoomName"].ToString();
            txtSurgeryName.Text = dr["SurgeryName"].ToString();

            txtDate.Text = Convert.ToDateTime(dr["SurgeryDate"]).ToString("yyyy/MM/dd");

            decimal totalAgreedCost = 0;
            decimal roomCostFromDb = 0;
            decimal advancePayment = 0;
            string paymentStatus = "Unpaid";

            DataTable dtAcc = wt.RunSelect("SELECT TotalCost, RoomCost, AdvancePayment, PaymentStatus FROM SurgeryAccounts WHERE SurgeryId=" + sid);
            if (dtAcc.Rows.Count > 0)
            {
                if (dtAcc.Rows[0]["TotalCost"] != DBNull.Value)
                    totalAgreedCost = Convert.ToDecimal(dtAcc.Rows[0]["TotalCost"]);

                if (dtAcc.Rows[0]["RoomCost"] != DBNull.Value)
                    roomCostFromDb = Convert.ToDecimal(dtAcc.Rows[0]["RoomCost"]);

                if (dtAcc.Rows[0]["AdvancePayment"] != DBNull.Value)
                    advancePayment = Convert.ToDecimal(dtAcc.Rows[0]["AdvancePayment"]);

                if (dtAcc.Rows[0]["PaymentStatus"] != DBNull.Value && !string.IsNullOrWhiteSpace(dtAcc.Rows[0]["PaymentStatus"].ToString()))
                    paymentStatus = dtAcc.Rows[0]["PaymentStatus"].ToString();
            }

            DateTime surgeryDate = dr["SurgeryDate"] != DBNull.Value ? Convert.ToDateTime(dr["SurgeryDate"]) : DateTime.Now;

            decimal dailyRoomPrice = 0;
            if (roomCostFromDb > 0)
            {
                dailyRoomPrice = roomCostFromDb;
            }
            else
            {
                dailyRoomPrice = dr["RoomCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["RoomCost"]);
            }

            int days = (DateTime.Now.Date - surgeryDate.Date).Days + 1;
            if (days < 1) days = 1;

            RoomCost = days * dailyRoomPrice;
            decimal day1RoomCost = (dr["RoomName"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["RoomName"].ToString())) ? dailyRoomPrice : 0;

            SuppliesCost = dr["SuppliesCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SuppliesCost"]);

            string surgDateStr = surgeryDate.ToString("yyyy-MM-dd");
            DataTable dtDay1Supplies = wt.RunSelect($@"
SELECT ISNULL(SUM(TotalPrice), 0) 
FROM SurgerySupplies 
WHERE SurgeryId={sid} 
  AND (CreatedAt IS NULL OR CONVERT(date, CreatedAt) <= CONVERT(date, '{surgDateStr}'))");
            decimal day1SuppliesCost = (dtDay1Supplies != null && dtDay1Supplies.Rows.Count > 0 && dtDay1Supplies.Rows[0][0] != DBNull.Value)
                ? Convert.ToDecimal(dtDay1Supplies.Rows[0][0]) : 0;

            if (totalAgreedCost > 0)
            {
                DoctorCost = totalAgreedCost - day1RoomCost - day1SuppliesCost;
                if (DoctorCost < 0) DoctorCost = 0;
            }
            else
            {
                DoctorCost = dr["DoctorCommission"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DoctorCommission"]);
                totalAgreedCost = DoctorCost + day1RoomCost + day1SuppliesCost;
            }

            txtDoctorCost.Text = DoctorCost.ToString("0.00");
            txtRoomCost.Text = RoomCost.ToString("0.00");
            txtSuppliesCost.Text = SuppliesCost.ToString("0.00");

            decimal total = DoctorCost + RoomCost + SuppliesCost;
            txtTotal.Text = total.ToString("0.00");

            txtPaid.Text = advancePayment > 0 ? advancePayment.ToString("0.00") : (dr["AdvancePayment"] == DBNull.Value ? "0.00" : Convert.ToDecimal(dr["AdvancePayment"]).ToString("0.00"));

            try
            {
                if (!string.IsNullOrEmpty(paymentStatus) && ddlStatus.Items.FindByValue(paymentStatus) != null)
                {
                    ddlStatus.SelectedValue = paymentStatus;
                }
            }
            catch { }

            CalculateRemaining();

            LoadInvoiceDetails();

            FillPrintData();
        }




        private void CalculateInvoice()
        {
            if (IsPayRemainingMode)
            {
                decimal paidNow = ParseDecimal(txtPaid.Text);
                decimal remain = RemainingBaseAmount - paidNow;
                if (remain < 0) remain = 0;
                txtTotal.Text = RemainingBaseAmount.ToString("0.00");
                txtRemaining.Text = remain.ToString("0.00");
                FillPrintData();
                return;
            }

            decimal other = ParseDecimal(txtOtherCost.Text);
            decimal discount = ParseDecimal(txtDiscount.Text);

            DoctorCost = ParseDecimal(txtDoctorCost.Text);
            RoomCost = ParseDecimal(txtRoomCost.Text);
            SuppliesCost = ParseDecimal(txtSuppliesCost.Text);

            decimal total =
                DoctorCost +
                RoomCost +
                SuppliesCost +
                other -
                discount;

            if (total < 0)
                total = 0;

            txtTotal.Text = total.ToString("0.00");

            CalculateRemaining();

            LoadInvoiceDetails();

            FillPrintData();
        }


















        protected void CalculateRemaining(object sender, EventArgs e)
        {
            decimal total = ParseDecimal(txtTotal.Text);
            decimal paid = ParseDecimal(txtPaid.Text);

            decimal remain = total - paid;

            if (remain < 0)
                remain = 0;

            txtRemaining.Text = remain.ToString("0.00");
        }

        private void CalculateRemaining()
        {
            decimal total;
            decimal.TryParse(txtTotal.Text, out total);

            decimal paid;
            decimal.TryParse(txtPaid.Text, out paid);

            decimal remain = total - paid;

            if (remain < 0)
                remain = 0;

            txtRemaining.Text = remain.ToString("0.00");
        }

        protected void CalculateTotal(object sender, EventArgs e)
        {
            CalculateInvoice();
        }

        private void FillPrintData()
        {
            lblSurgery.Text = txtSurgeryId.Text;
            lblPatient.Text = txtPatient.Text;
            lblDoctor.Text = txtDoctor.Text;
            lblRoom.Text = txtRoom.Text;

            lblTotal.Text = txtTotal.Text;
            lblPaid.Text = txtPaid.Text;
            lblRemain.Text = txtRemaining.Text;
        }

        private void LoadInvoiceDetails()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ItemName");
            dt.Columns.Add("Amount");

            dt.Rows.Add("أجر الطبيب", txtDoctorCost.Text);
            dt.Rows.Add("غرفة العمليات", txtRoomCost.Text);
            dt.Rows.Add("المستلزمات", txtSuppliesCost.Text);
            dt.Rows.Add("خدمات إضافية", txtOtherCost.Text);
            dt.Rows.Add("الخصم", txtDiscount.Text);

            gvInvoice.DataSource = dt;
            gvInvoice.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSurgeryId.Value))
            {
                ShowAlert("بيانات العملية غير مكتملة");
                return;
            }

            int surgeryId = Convert.ToInt32(hfSurgeryId.Value);

            bool isPayRemainingMode = Session["PayRemaining_Mode"] != null && (bool)Session["PayRemaining_Mode"] == true;

            if (isPayRemainingMode)
            {
                int originalInvoiceId = Convert.ToInt32(Session["PayRemaining_InvoiceId"]);
                decimal remainingAmount = Convert.ToDecimal(Session["PayRemaining_Amount"]);
                decimal paidNow = ParseDecimal(txtPaid.Text);

                DataTable origInv = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE Id=" + originalInvoiceId);
                if (origInv.Rows.Count == 0)
                {
                    ShowAlert("لم يتم العثور على الفاتورة الأصلية");
                    return;
                }

                decimal origPaid = Convert.ToDecimal(origInv.Rows[0]["PaidAmount"]);
                decimal origTotal = Convert.ToDecimal(origInv.Rows[0]["TotalAmount"]);
                decimal origRemaining = Convert.ToDecimal(origInv.Rows[0]["RemainingAmount"]);

                decimal newPaid = origPaid + paidNow;
                decimal newRemaining = origTotal - newPaid;
                if (newRemaining < 0) newRemaining = 0;

                string newStatus = newRemaining <= 0 ? "Paid" : "Partial";

                string updateSql = $"UPDATE SurgeryInvoices SET PaidAmount={newPaid}, RemainingAmount={newRemaining}, PaymentStatus='{newStatus}' WHERE Id={originalInvoiceId}";
                string updateResult = wt.RunInsDelUpd(updateSql);

                if (updateResult != "OK")
                {
                    ShowAlert("خطأ في تحديث الفاتورة: " + updateResult.Replace("'", ""));
                    return;
                }

                Session.Remove("PayRemaining_Mode");
                Session.Remove("PayRemaining_InvoiceId");
                Session.Remove("PayRemaining_SurgeryId");
                Session.Remove("PayRemaining_Amount");
                Session.Remove("PayRemaining_PatientName");
                Session.Remove("PayRemaining_DoctorName");
                Session.Remove("PayRemaining_SurgeryName");

                FillPrintData();

                string surgeryCodePayRem = "SUR-" + surgeryId.ToString("000000");
                string invoiceCodePayRem = "INV-" + originalInvoiceId.ToString("000000");

                Session["SurgInv_SurgeryBarcode"] = CreateBarcode(surgeryCodePayRem);
                Session["SurgInv_InvoiceBarcode"] = CreateBarcode(invoiceCodePayRem);
                Session["SurgInv_SurgeryBarcodeText"] = surgeryCodePayRem;
                Session["SurgInv_InvoiceBarcodeText"] = invoiceCodePayRem;
                Session["SurgInv_SurgeryId"] = txtSurgeryId.Text;
                Session["SurgInv_SurgeryName"] = txtSurgeryName.Text;
                Session["SurgInv_Patient"] = txtPatient.Text;
                Session["SurgInv_Doctor"] = txtDoctor.Text;
                Session["SurgInv_Room"] = txtRoom.Text;
                Session["SurgInv_Date"] = txtDate.Text;
                Session["SurgInv_PrintDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                Session["SurgInv_DoctorCost"] = txtDoctorCost.Text;
                Session["SurgInv_RoomCost"] = txtRoomCost.Text;
                Session["SurgInv_SuppliesCost"] = txtSuppliesCost.Text;
                Session["SurgInv_OtherCost"] = txtOtherCost.Text;
                Session["SurgInv_Discount"] = txtDiscount.Text;
                Session["SurgInv_Total"] = txtTotal.Text;
                Session["SurgInv_Paid"] = txtPaid.Text;
                Session["SurgInv_Remaining"] = txtRemaining.Text;
                Session["SurgInv_PaymentStatus"] = ddlStatus.SelectedValue;
                Session["SurgInv_PatientId"] = hfPatientId.Value;
                Session["SurgInv_AppointmentId"] = "";

                try
                {
                    CSurgerySupplies ss = new CSurgerySupplies();
                    DataTable dtSup = ss.GetBySurgery(surgeryId);
                    Session["SurgInv_Supplies"] = dtSup;
                }
                catch { Session["SurgInv_Supplies"] = null; }

                Response.Redirect("SurgeryInvoicePrint.aspx");
                return;
            }

            List<SqlParameter> chkPrm = new List<SqlParameter>();
            chkPrm.Add(new SqlParameter("@SurgeryId", surgeryId));

            DataTable chk = wt.RunSelect(
                "SELECT Id FROM SurgeryInvoices WHERE SurgeryId=@SurgeryId",
                chkPrm);

            if (chk.Rows.Count > 0)
            {
                ShowAlert("تم إنشاء فاتورة لهذه العملية مسبقاً");
                return;
            }

            inv.SurgeryId = surgeryId;

            inv.PatientId = string.IsNullOrEmpty(hfPatientId.Value)
                ? 0
                : Convert.ToInt32(hfPatientId.Value);

            inv.DoctorCost = ParseDecimal(txtDoctorCost.Text);
            inv.RoomCost = ParseDecimal(txtRoomCost.Text);
            inv.SuppliesCost = ParseDecimal(txtSuppliesCost.Text);
            inv.OtherCost = ParseDecimal(txtOtherCost.Text);
            inv.Discount = ParseDecimal(txtDiscount.Text);
            inv.TotalAmount = ParseDecimal(txtTotal.Text);
            inv.PaidAmount = ParseDecimal(txtPaid.Text);
            inv.RemainingAmount = ParseDecimal(txtRemaining.Text);

            inv.PaymentMethod = ddlPaymentMethod.SelectedValue;
            inv.PaymentStatus = ddlStatus.SelectedValue;

            inv.CreatedBy = Session["UserId"] == null
                ? 0
                : Convert.ToInt32(Session["UserId"]);

            inv.InvoiceDate = DateTime.Now;

            string result = inv.Insert();

            if (result != "OK")
            {
                ShowAlert(result.Replace("'", ""));
                return;
            }

            try
            {
                decimal docVal = ParseDecimal(txtDoctorCost.Text);
                decimal roomVal = ParseDecimal(txtRoomCost.Text);
                decimal totVal = ParseDecimal(txtTotal.Text);
                decimal paidVal = ParseDecimal(txtPaid.Text);
                decimal remVal = ParseDecimal(txtRemaining.Text);
                string statVal = ddlStatus.SelectedValue;

                DataTable dtAcc = wt.RunSelect("SELECT Id FROM SurgeryAccounts WHERE SurgeryId=" + surgeryId);
                if (dtAcc.Rows.Count > 0)
                {
                    int accId = Convert.ToInt32(dtAcc.Rows[0]["Id"]);
                    wt.RunInsDelUpd($"UPDATE SurgeryAccounts SET DoctorCommission={docVal}, RoomCost={roomVal}, TotalCost={totVal}, AdvancePayment={paidVal}, RemainingAmount={remVal}, PaymentStatus='{statVal}' WHERE Id={accId}");
                }
            }
            catch { }

            FillPrintData();

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter("@SurgeryId", surgeryId));

            DataTable dtInvoice = wt.RunSelect(@"
SELECT TOP 1 Id
FROM SurgeryInvoices
WHERE SurgeryId=@SurgeryId
ORDER BY Id DESC", p);

            int invoiceId = 0;

            if (dtInvoice.Rows.Count > 0)
            {
                invoiceId = Convert.ToInt32(dtInvoice.Rows[0]["Id"]);
            }

            string surgeryCode = "SUR-" + surgeryId.ToString("000000");
            string invoiceCode = "INV-" + invoiceId.ToString("000000");

            Session["SurgInv_SurgeryBarcode"] = CreateBarcode(surgeryCode);
            Session["SurgInv_InvoiceBarcode"] = CreateBarcode(invoiceCode);

            Session["SurgInv_SurgeryBarcodeText"] = surgeryCode;
            Session["SurgInv_InvoiceBarcodeText"] = invoiceCode;


            Session["SurgInv_SurgeryId"] = txtSurgeryId.Text;
            Session["SurgInv_SurgeryName"] = txtSurgeryName.Text;
            Session["SurgInv_Patient"] = txtPatient.Text;
            Session["SurgInv_Doctor"] = txtDoctor.Text;
            Session["SurgInv_Room"] = txtRoom.Text;
            Session["SurgInv_Date"] = txtDate.Text;
            Session["SurgInv_PrintDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            Session["SurgInv_DoctorCost"] = txtDoctorCost.Text;
            Session["SurgInv_RoomCost"] = txtRoomCost.Text;
            Session["SurgInv_SuppliesCost"] = txtSuppliesCost.Text;
            Session["SurgInv_OtherCost"] = txtOtherCost.Text;
            Session["SurgInv_Discount"] = txtDiscount.Text;

            Session["SurgInv_Total"] = txtTotal.Text;
            Session["SurgInv_Paid"] = txtPaid.Text;
            Session["SurgInv_Remaining"] = txtRemaining.Text;
            Session["SurgInv_PaymentStatus"] = ddlStatus.SelectedValue;
            Session["SurgInv_PatientId"] = hfPatientId.Value;

            Session["SurgInv_AppointmentId"] = "";

            try
            {
                CSurgerySupplies ss = new CSurgerySupplies();
                DataTable dtSup = ss.GetBySurgery(surgeryId);
                Session["SurgInv_Supplies"] = dtSup;
            }
            catch { Session["SurgInv_Supplies"] = null; }

            Response.Redirect("SurgeryInvoicePrint.aspx");


        }

        private decimal ParseDecimal(string text)
        {
            decimal value;
            decimal.TryParse(text, out value);
            return value;
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('" + message.Replace("'", "") + "');",
                true);
        }

        private string CreateBarcode(string text)
        {
            BarcodeWriter writer = new BarcodeWriter();

            writer.Format = BarcodeFormat.CODE_128;

            writer.Options = new EncodingOptions
            {
                Width = 350,
                Height = 80,
                Margin = 2
            };

            using (Bitmap bmp = writer.Write(text))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {

            Session["SurgInv_SurgeryId"] = txtSurgeryId.Text;
            Session["SurgInv_SurgeryName"] = txtSurgeryName.Text;
            Session["SurgInv_Patient"] = txtPatient.Text;
            Session["SurgInv_Doctor"] = txtDoctor.Text;
            Session["SurgInv_Room"] = txtRoom.Text;
            Session["SurgInv_Date"] = txtDate.Text;
           
            Session["SurgInv_PrintDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Session["SurgInv_DoctorCost"] = txtDoctorCost.Text;
            Session["SurgInv_RoomCost"] = txtRoomCost.Text;
            Session["SurgInv_SuppliesCost"] = txtSuppliesCost.Text;
            Session["SurgInv_OtherCost"] = txtOtherCost.Text;
            Session["SurgInv_Discount"] = txtDiscount.Text;

            Session["SurgInv_Total"] = txtTotal.Text;
            Session["SurgInv_Paid"] = txtPaid.Text;
            Session["SurgInv_Remaining"] = txtRemaining.Text;
            Session["SurgInv_PaymentStatus"] = ddlStatus.SelectedValue;

            string surgeryCode = "SUR-" +
                Convert.ToInt32(txtSurgeryId.Text).ToString("D6");

            Session["SurgInv_SurgeryBarcode"] = CreateBarcode(surgeryCode);
            Session["SurgInv_SurgeryBarcodeText"] = surgeryCode;

            string invoiceCode = "TEMP-" +
                Convert.ToInt32(txtSurgeryId.Text).ToString("D6");

            Session["SurgInv_InvoiceBarcode"] = CreateBarcode(invoiceCode);
            Session["SurgInv_InvoiceBarcodeText"] = invoiceCode;

            Response.Redirect("SurgeryInvoicePrint.aspx");
        }



    }
}