using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace Riyadh_Al_Salehin
{
    public partial class SurgeryInvoicesList : Page
    {
        WorkTable wt = new WorkTable();
        CSurgeryInvoices inv = new CSurgeryInvoices();
        COperationRooms rooms = new COperationRooms();
        CSurgerySupplies ss = new CSurgerySupplies();

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
                LoadInvoices();
            }
        }

        protected string GetStatusBadgeClass(object statusObj)
        {
            string status = statusObj == null ? string.Empty : statusObj.ToString();
            if (string.IsNullOrWhiteSpace(status)) return "bs-unknown";
            switch (status.Trim())
            {
                case "مدفوع": return "bs-paid";
                case "غير مدفوع": return "bs-unpaid";
                case "مدفوع جزئياً":
                case "مدفوع جزئيا": return "bs-partial";
                default: return "bs-unknown";
            }
        }

        private static string GenerateBarcodeBase64Local(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Width = 350, Height = 80, Margin = 2 }
            };
            using (var bmp = writer.Write(text))
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string SyncRoomChargesToInvoice(int surgeryId)
        {
            try
            {
                WorkTable wt = new WorkTable();
                string sql = @"
SELECT s.Id, s.PatientId, s.DoctorId, s.RoomId, s.SurgeryDate, s.DoctorCost, s.Status, r.DailyPrice
FROM Surgeries s
LEFT JOIN OperationRooms r ON s.RoomId = r.Id
WHERE s.Id = " + surgeryId;

                DataTable dtS = wt.RunSelect(sql);
                if (dtS == null || dtS.Rows.Count == 0) return "Surgery not found";

                DataRow sr = dtS.Rows[0];
                int patientId = sr["PatientId"] != DBNull.Value ? Convert.ToInt32(sr["PatientId"]) : 0;

                DateTime surgeryDate = sr["SurgeryDate"] != DBNull.Value ? Convert.ToDateTime(sr["SurgeryDate"]) : DateTime.Now;
                decimal dailyPrice = (sr["RoomId"] != DBNull.Value && sr["DailyPrice"] != DBNull.Value) ? Convert.ToDecimal(sr["DailyPrice"]) : 0;
                string statusStr = sr["Status"] != DBNull.Value ? sr["Status"].ToString() : "";

                DateTime endDate = surgeryDate;
                if (!string.IsNullOrWhiteSpace(statusStr))
                {
                    if (statusStr.StartsWith("Discharged") || statusStr.StartsWith("Confirmed"))
                    {
                        if (statusStr.Contains(":") && DateTime.TryParse(statusStr.Split(':')[1], out DateTime parsedEnd))
                        {
                            endDate = parsedEnd;
                        }
                    }
                }

                int days = (endDate.Date - surgeryDate.Date).Days + 1;
                if (days < 1) days = 1;

                decimal day1RoomCost = (sr["RoomId"] != DBNull.Value) ? dailyPrice : 0;
                decimal calculatedRoomCost = (sr["RoomId"] != DBNull.Value) ? (days * dailyPrice) : 0;

                string surgDateStr = surgeryDate.ToString("yyyy-MM-dd");
                string day1SuppliesSql = $@"
SELECT ISNULL(SUM(TotalPrice), 0) 
FROM SurgerySupplies 
WHERE SurgeryId={surgeryId}";

                DataTable dtDay1Supplies = wt.RunSelect(day1SuppliesSql);
                decimal day1SuppliesCost = (dtDay1Supplies != null && dtDay1Supplies.Rows.Count > 0 && dtDay1Supplies.Rows[0][0] != DBNull.Value) 
                    ? Convert.ToDecimal(dtDay1Supplies.Rows[0][0]) : 0;

                DataTable dtAllSupplies = wt.RunSelect("SELECT ISNULL(SUM(TotalPrice), 0) FROM SurgerySupplies WHERE SurgeryId=" + surgeryId);
                decimal totalSuppliesCost = (dtAllSupplies != null && dtAllSupplies.Rows.Count > 0 && dtAllSupplies.Rows[0][0] != DBNull.Value) 
                    ? Convert.ToDecimal(dtAllSupplies.Rows[0][0]) : 0;

                decimal totalAgreed = 0;
                DataTable dtAcc = wt.RunSelect("SELECT TotalCost, AdvancePayment, RoomCost FROM SurgeryAccounts WHERE SurgeryId=" + surgeryId);
                if (dtAcc != null && dtAcc.Rows.Count > 0 && dtAcc.Rows[0]["TotalCost"] != DBNull.Value)
                {
                    totalAgreed = Convert.ToDecimal(dtAcc.Rows[0]["TotalCost"]);
                }

                DataTable invDt = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE SurgeryId=" + surgeryId);
                if (invDt.Rows.Count > 0)
                {
                    DataRow invRow = invDt.Rows[0];
                    int invId = Convert.ToInt32(invRow["Id"]);
                    decimal othCost = invRow["OtherCost"] != DBNull.Value ? Convert.ToDecimal(invRow["OtherCost"]) : 0;
                    decimal disc = invRow["Discount"] != DBNull.Value ? Convert.ToDecimal(invRow["Discount"]) : 0;
                    decimal paid = invRow["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(invRow["PaidAmount"]) : 0;

                    decimal currentRoomCost = invRow["RoomCost"] != DBNull.Value ? Convert.ToDecimal(invRow["RoomCost"]) : 0;
                    decimal finalRoomCost = statusStr.StartsWith("Discharged") ? calculatedRoomCost : Math.Max(currentRoomCost, calculatedRoomCost);

                    decimal docCost = 0;
                    if (totalAgreed > 0)
                    {
                        docCost = totalAgreed - day1RoomCost - day1SuppliesCost;
                        if (docCost < 0) docCost = 0;
                    }
                    else
                    {
                        docCost = invRow["DoctorCost"] != DBNull.Value ? Convert.ToDecimal(invRow["DoctorCost"]) : 0;
                    }

                    decimal newTotal = docCost + finalRoomCost + totalSuppliesCost + othCost - disc;
                    if (newTotal < 0) newTotal = 0;
                    decimal newRemaining = newTotal - paid;
                    if (newRemaining < 0) newRemaining = 0;

                    string status = "غير مدفوع";
                    if (newRemaining <= 0) status = "مدفوع";
                    else if (paid > 0) status = "مدفوع جزئياً";

                    wt.RunInsDelUpd($"UPDATE SurgeryInvoices SET DoctorCost={docCost}, RoomCost={finalRoomCost}, SuppliesCost={totalSuppliesCost}, OtherCost={othCost}, Discount={disc}, TotalAmount={newTotal}, RemainingAmount={newRemaining}, PaymentStatus=N'{status}' WHERE Id={invId}");

                    wt.RunInsDelUpd($"UPDATE SurgeryAccounts SET DoctorCommission={docCost}, RoomCost={finalRoomCost}, RemainingAmount={newRemaining}, PaymentStatus=N'{status}' WHERE SurgeryId={surgeryId}");
                }
                else
                {
                    decimal docCost = totalAgreed > 0 ? (totalAgreed - day1RoomCost - day1SuppliesCost) : 0;
                    if (docCost < 0) docCost = 0;

                    CSurgeryInvoices inv = new CSurgeryInvoices();
                    inv.SurgeryId = surgeryId;
                    inv.PatientId = patientId;
                    inv.DoctorCost = docCost;
                    inv.RoomCost = calculatedRoomCost;
                    inv.SuppliesCost = totalSuppliesCost;
                    inv.OtherCost = 0;
                    inv.Discount = 0;
                    inv.TotalAmount = docCost + calculatedRoomCost + totalSuppliesCost;
                    inv.PaidAmount = 0;
                    inv.RemainingAmount = inv.TotalAmount;
                    inv.PaymentMethod = "";
                    inv.PaymentStatus = "غير مدفوع";
                    inv.CreatedBy = 0;
                    inv.InvoiceDate = DateTime.Now;
                    inv.Insert();
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void SyncAllActiveRoomCharges()
        {
            try
            {
                string sql = "SELECT Id FROM Surgeries WHERE RoomId IS NOT NULL";
                DataTable dt = wt.RunSelect(sql);
                if (dt != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        int sid = Convert.ToInt32(r["Id"]);
                        SyncRoomChargesToInvoice(sid);
                    }
                }
            }
            catch { }
        }

        private void LoadInvoices()
        {
            SyncAllActiveRoomCharges();

            string sql = @"
SELECT si.Id, si.SurgeryId, si.InvoiceDate, p.PatientName, d.DoctorName, s.SurgeryName,
si.DoctorCost, si.RoomCost, si.SuppliesCost, si.OtherCost, si.Discount,
si.TotalAmount, si.PaidAmount, si.RemainingAmount, si.PaymentStatus
FROM SurgeryInvoices si
INNER JOIN Surgeries s ON si.SurgeryId=s.Id
INNER JOIN Patients p ON p.Id=si.PatientId
INNER JOIN Doctors d ON d.Id=s.DoctorId
ORDER BY si.Id DESC";

            DataTable dt = wt.RunSelect(sql);
            BindData(dt);
            LoadOccupiedRooms();
            LoadAllSuppliesDetails();
            LoadAllRoomsDetails();
        }

        private void LoadOccupiedRooms()
        {
            string sql = @"
SELECT s.Id AS SurgeryId, p.PatientName, r.RoomName, r.DailyPrice, s.SurgeryDate, s.Status
FROM Surgeries s
INNER JOIN Patients p ON s.PatientId = p.Id
INNER JOIN OperationRooms r ON s.RoomId = r.Id
WHERE s.RoomId IS NOT NULL AND (s.Status IS NULL OR (s.Status NOT LIKE 'Discharged%' AND s.Status NOT LIKE 'Completed%'))
ORDER BY s.SurgeryDate DESC";

            DataTable dt = wt.RunSelect(sql);
            DataTable outDt = new DataTable();
            outDt.Columns.Add("SurgeryId");
            outDt.Columns.Add("PatientName");
            outDt.Columns.Add("RoomName");
            outDt.Columns.Add("SurgeryDate", typeof(DateTime));
            outDt.Columns.Add("DaysStayed", typeof(int));
            outDt.Columns.Add("DailyPrice", typeof(decimal));
            outDt.Columns.Add("NeedsConfirmation", typeof(bool));

            foreach (DataRow r in dt.Rows)
            {
                int sid = Convert.ToInt32(r["SurgeryId"]);
                DateTime date = Convert.ToDateTime(r["SurgeryDate"]);
                
                DateTime endDate = date;
                string statusStr = r["Status"] != DBNull.Value ? r["Status"].ToString() : "";
                if (!string.IsNullOrWhiteSpace(statusStr) && statusStr.StartsWith("Confirmed"))
                {
                    if (statusStr.Contains(":") && DateTime.TryParse(statusStr.Split(':')[1], out DateTime parsedEnd))
                    {
                        endDate = parsedEnd;
                    }
                }

                int days = (endDate.Date - date.Date).Days + 1;
                if (days < 1) days = 1;
                
                bool needsConfirmation = (DateTime.Now.Date > endDate.Date);

                decimal daily = 0;
                decimal.TryParse(r["DailyPrice"].ToString(), out daily);
                outDt.Rows.Add(sid, r["PatientName"].ToString(), r["RoomName"].ToString(), date, days, daily, needsConfirmation);
            }

            gvOccupiedRooms.DataSource = outDt;
            gvOccupiedRooms.DataBind();
        }

        private void LoadAllSuppliesDetails(string patientName = "", string fromDate = "", string toDate = "")
        {
            string sql = @"
SELECT 
    ss.Id,
    ms.SupplyName,
    ISNULL(ms.InitialQuantity, ms.AvailableQuantity) AS BaseQuantity,
    ss.UsedQuantity AS DispensedQuantity,
    ss.UnitPrice,
    (ISNULL(ms.InitialQuantity, ms.AvailableQuantity) * ss.UnitPrice) AS TotalBaseCost,
    (ss.UsedQuantity * ss.UnitPrice) AS DispensedCost,
    p.PatientName,
    ISNULL(u.FullName, ISNULL(u.UserName, N'النظام')) AS DispensedBy
FROM SurgerySupplies ss
INNER JOIN MedicalSupplies ms ON ss.SupplyId = ms.Id
INNER JOIN Surgeries s ON ss.SurgeryId = s.Id
INNER JOIN Patients p ON s.PatientId = p.Id
LEFT JOIN Users u ON s.CreatedBy = u.Id
WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(patientName))
                sql += " AND p.PatientName LIKE N'%" + patientName.Trim() + "%'";

            if (!string.IsNullOrWhiteSpace(fromDate))
                sql += " AND CONVERT(date, s.SurgeryDate) >= CONVERT(date, '" + fromDate + "')";

            if (!string.IsNullOrWhiteSpace(toDate))
                sql += " AND CONVERT(date, s.SurgeryDate) <= CONVERT(date, '" + toDate + "')";

            sql += " ORDER BY ss.Id DESC";

            DataTable dt = wt.RunSelect(sql);
            gvAllSuppliesDetails.DataSource = dt;
            gvAllSuppliesDetails.DataBind();
        }

        private void LoadAllRoomsDetails(string patientName = "", string fromDate = "", string toDate = "")
        {
            string sql = @"
SELECT 
    s.Id AS SurgeryId,
    r.RoomName,
    p.PatientName,
    s.SurgeryDate,
    r.DailyPrice,
    s.Status
FROM Surgeries s
INNER JOIN OperationRooms r ON s.RoomId = r.Id
INNER JOIN Patients p ON s.PatientId = p.Id
WHERE s.RoomId IS NOT NULL";

            if (!string.IsNullOrWhiteSpace(patientName))
                sql += " AND p.PatientName LIKE N'%" + patientName.Trim() + "%'";

            if (!string.IsNullOrWhiteSpace(fromDate))
                sql += " AND CONVERT(date, s.SurgeryDate) >= CONVERT(date, '" + fromDate + "')";

            if (!string.IsNullOrWhiteSpace(toDate))
                sql += " AND CONVERT(date, s.SurgeryDate) <= CONVERT(date, '" + toDate + "')";

            sql += " ORDER BY s.SurgeryDate DESC";

            DataTable dt = wt.RunSelect(sql);
            DataTable outDt = new DataTable();
            outDt.Columns.Add("SurgeryId");
            outDt.Columns.Add("RoomName");
            outDt.Columns.Add("PatientName");
            outDt.Columns.Add("OccupiedDates");
            outDt.Columns.Add("DaysStayed", typeof(int));
            outDt.Columns.Add("DailyPrice", typeof(decimal));
            outDt.Columns.Add("TotalRoomCost", typeof(decimal));
            outDt.Columns.Add("RoomStatus");

            foreach (DataRow r in dt.Rows)
            {
                int sid = Convert.ToInt32(r["SurgeryId"]);
                DateTime date = r["SurgeryDate"] != DBNull.Value ? Convert.ToDateTime(r["SurgeryDate"]) : DateTime.Now;
                string statusStr = r["Status"] != DBNull.Value ? r["Status"].ToString() : "";

                DateTime endDate = DateTime.Now;
                bool isDischarged = false;
                if (!string.IsNullOrWhiteSpace(statusStr) && statusStr.StartsWith("Discharged"))
                {
                    isDischarged = true;
                    if (statusStr.Contains(":") && DateTime.TryParse(statusStr.Split(':')[1], out DateTime parsedEnd))
                    {
                        endDate = parsedEnd;
                    }
                }

                int days = (endDate.Date - date.Date).Days + 1;
                if (days < 1) days = 1;

                decimal daily = 0;
                decimal.TryParse(r["DailyPrice"].ToString(), out daily);

                decimal totalRoomCost = days * daily;
                string datesStr = date.ToString("yyyy/MM/dd") + " - " + endDate.ToString("yyyy/MM/dd");
                string roomStatus = isDischarged ? "غادر يوم " + endDate.ToString("yyyy/MM/dd") : "متواجد في الغرف";

                outDt.Rows.Add(sid, r["RoomName"].ToString(), r["PatientName"].ToString(), datesStr, days, daily, totalRoomCost, roomStatus);
            }

            gvAllRoomsDetails.DataSource = outDt;
            gvAllRoomsDetails.DataBind();
        }

        private void BindData(DataTable dt)
        {
            if (dt == null) dt = new DataTable();
            gvInvoices.DataSource = dt;
            gvInvoices.DataBind();

            decimal doctor = 0, room = 0, supplies = 0, other = 0, discount = 0;
            decimal total = 0, paid = 0, remain = 0;

            foreach (DataRow r in dt.Rows)
            {
                doctor += Convert.ToDecimal(r["DoctorCost"]);
                room += Convert.ToDecimal(r["RoomCost"]);
                supplies += Convert.ToDecimal(r["SuppliesCost"]);
                other += Convert.ToDecimal(r["OtherCost"]);
                discount += Convert.ToDecimal(r["Discount"]);
                total += Convert.ToDecimal(r["TotalAmount"]);
                paid += Convert.ToDecimal(r["PaidAmount"]);
                remain += Convert.ToDecimal(r["RemainingAmount"]);
            }

            lblDoctorCost.Text = doctor.ToString("N2");
            lblRoomCost.Text = room.ToString("N2");
            lblSuppliesCost.Text = supplies.ToString("N2");
            lblOtherCost.Text = other.ToString("N2");
            lblDiscount.Text = discount.ToString("N2");
            lblTotal.Text = total.ToString("N2");
            lblPaid.Text = paid.ToString("N2");
            lblRemain.Text = remain.ToString("N2");

            lblSumDiscount.Text = discount.ToString("N2");
            lblSumTotal.Text = total.ToString("N2");
            lblSumPaid.Text = paid.ToString("N2");
            lblSumRemain.Text = remain.ToString("N2");

            lblRecordCount.Text = "عدد الفواتير: " + dt.Rows.Count.ToString();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtPatientName.Text = "";
            txtDoctorName.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            LoadInvoices();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SyncAllActiveRoomCharges();

            string sql = @"
SELECT si.Id, si.SurgeryId, si.InvoiceDate, p.PatientName, d.DoctorName, s.SurgeryName,
si.DoctorCost, si.RoomCost, si.SuppliesCost, si.OtherCost, si.Discount,
si.TotalAmount, si.PaidAmount, si.RemainingAmount, si.PaymentStatus
FROM SurgeryInvoices si
INNER JOIN Surgeries s ON si.SurgeryId=s.Id
INNER JOIN Patients p ON p.Id=si.PatientId
INNER JOIN Doctors d ON d.Id=s.DoctorId
WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                sql += " AND p.PatientName LIKE N'%" + txtPatientName.Text.Trim() + "%'";

            if (!string.IsNullOrWhiteSpace(txtDoctorName.Text))
                sql += " AND d.DoctorName LIKE N'%" + txtDoctorName.Text.Trim() + "%'";

            if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
                sql += " AND CONVERT(date, si.InvoiceDate) >= CONVERT(date, '" + txtFromDate.Text + "')";

            if (!string.IsNullOrWhiteSpace(txtToDate.Text))
                sql += " AND CONVERT(date, si.InvoiceDate) <= CONVERT(date, '" + txtToDate.Text + "')";

            sql += " ORDER BY si.Id DESC";

            DataTable dt = wt.RunSelect(sql);
            BindData(dt);
            LoadAllSuppliesDetails(txtPatientName.Text, txtFromDate.Text, txtToDate.Text);
            LoadAllRoomsDetails(txtPatientName.Text, txtFromDate.Text, txtToDate.Text);
        }

        protected void gvInvoices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowDetails")
            {
                int invoiceId = Convert.ToInt32(e.CommandArgument);
                ShowInvoiceDetails(invoiceId);
            }
            else if (e.CommandName == "PayRemaining")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                if (args.Length >= 6)
                {
                    int invoiceId = Convert.ToInt32(args[0]);
                    int surgeryId = Convert.ToInt32(args[1]);
                    decimal remainingAmount = Convert.ToDecimal(args[2]);
                    string patientName = args[3];
                    string doctorName = args[4];
                    string surgeryName = args[5];
                    
                    Session["PayRemaining_InvoiceId"] = invoiceId;
                    Session["PayRemaining_SurgeryId"] = surgeryId;
                    Session["PayRemaining_Amount"] = remainingAmount;
                    Session["PayRemaining_PatientName"] = patientName;
                    Session["PayRemaining_DoctorName"] = doctorName;
                    Session["PayRemaining_SurgeryName"] = surgeryName;
                    Session["PayRemaining_Mode"] = true; // وضع دفع المتبقي فقط
                    
                    Response.Redirect("~/SurgeryInvoice.aspx");
                }
            }
        }

        private void ShowInvoiceDetails(int invoiceId)
        {
            DataTable dtInv = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE Id=" + invoiceId);
            if (dtInv.Rows.Count == 0) return;
            DataRow invRow = dtInv.Rows[0];
            int surgeryId = invRow["SurgeryId"] == DBNull.Value ? 0 : Convert.ToInt32(invRow["SurgeryId"]);

            DataTable details = new DataTable();
            details.Columns.Add("Item");
            details.Columns.Add("Amount");
            details.Rows.Add("أجر الطبيب", Convert.ToDecimal(invRow["DoctorCost"]).ToString("N2"));
            details.Rows.Add("غرفة العمليات", Convert.ToDecimal(invRow["RoomCost"]).ToString("N2"));
            details.Rows.Add("المستلزمات", Convert.ToDecimal(invRow["SuppliesCost"]).ToString("N2"));
            details.Rows.Add("تكاليف إضافية", Convert.ToDecimal(invRow["OtherCost"]).ToString("N2"));
            details.Rows.Add("الخصم", Convert.ToDecimal(invRow["Discount"]).ToString("N2"));
            details.Rows.Add("الإجمالي", Convert.ToDecimal(invRow["TotalAmount"]).ToString("N2"));

            gvInvoiceDetails.DataSource = details;
            gvInvoiceDetails.DataBind();

            if (surgeryId > 0)
            {
                DataTable dtSup = ss.GetBySurgery(surgeryId);
                if (dtSup != null && dtSup.Rows.Count > 0)
                {
                    gvSurgerySupplies.DataSource = dtSup;
                    gvSurgerySupplies.DataBind();
                    suppliesSection.Visible = true;
                }
                else
                {
                    suppliesSection.Visible = false;
                }
            }
            else
            {
                suppliesSection.Visible = false;
            }

            pnlInvoiceDetails.Visible = true;
            lblSelectedInvoiceId.Text = invoiceId.ToString();
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            pnlInvoiceDetails.Visible = false;
        }

        protected void gvOccupiedRooms_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetSupplies()
        {
            try
            {
                CMedicalSupplies ms = new CMedicalSupplies();
                DataTable dt = ms.GetAll();
                var list = new List<object>();
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new { Id = Convert.ToInt32(r["Id"]), SupplyName = r["SupplyName"].ToString(), Price = Convert.ToDecimal(r["Price"]).ToString("0.##") });
                }
                return list.ToArray();
            }
            catch (Exception ex) { return new { Error = ex.Message }; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object AddSupplyAjax(int surgeryId, int supplyId, int qty, decimal unitPrice)
        {
            try
            {
                CSurgerySupplies ss = new CSurgerySupplies();
                ss.SurgeryId = surgeryId;
                ss.SupplyId = supplyId;
                ss.UsedQuantity = qty;
                ss.UnitPrice = unitPrice;
                ss.TotalPrice = qty * unitPrice;
                string res = ss.Insert();

                if (res != "OK")
                    return new { Status = "Error", Message = res };

                WorkTable wt = new WorkTable();
                wt.RunInsDelUpd($"UPDATE MedicalSupplies SET AvailableQuantity = AvailableQuantity - {qty} WHERE Id = {supplyId}");

                string syncRes = SyncRoomChargesToInvoice(surgeryId);
                if (syncRes != "OK")
                    return new { Status = "Error", Message = syncRes };

                return new { Status = "OK" };
            }
            catch (Exception ex) { return new { Status = "Error", Message = ex.Message }; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object AddRoomChargeAjax(int surgeryId, int extraDays, decimal dailyPrice)
        {
            try
            {
                decimal amount = extraDays * dailyPrice;
                WorkTable wt = new WorkTable();
                DataTable invDt = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE SurgeryId=" + surgeryId);
                if (invDt.Rows.Count > 0)
                {
                    int invId = Convert.ToInt32(invDt.Rows[0]["Id"]);
                    decimal roomCost = Convert.ToDecimal(invDt.Rows[0]["RoomCost"]);
                    decimal total = Convert.ToDecimal(invDt.Rows[0]["TotalAmount"]);
                    decimal remaining = Convert.ToDecimal(invDt.Rows[0]["RemainingAmount"]);

                    roomCost += amount;
                    total += amount;
                    remaining += amount;

                    wt.RunInsDelUpd($"UPDATE SurgeryInvoices SET RoomCost={roomCost}, TotalAmount={total}, RemainingAmount={remaining} WHERE Id={invId}");
                }
                else
                {
                    CSurgeryInvoices inv = new CSurgeryInvoices();
                    inv.SurgeryId = surgeryId;
                    inv.PatientId = 0;
                    inv.DoctorCost = 0;
                    inv.RoomCost = amount;
                    inv.SuppliesCost = 0;
                    inv.OtherCost = 0;
                    inv.Discount = 0;
                    inv.TotalAmount = amount;
                    inv.PaidAmount = 0;
                    inv.RemainingAmount = amount;
                    inv.PaymentMethod = "";
                    inv.PaymentStatus = "Unpaid";
                    inv.CreatedBy = 0;
                    inv.InvoiceDate = DateTime.Now;
                    inv.Insert();
                }

                return new { Status = "OK" };
            }
            catch (Exception ex) { return new { Status = "Error", Message = ex.Message }; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object DischargePatientAjax(int surgeryId)
        {
            try
            {
                WorkTable wt = new WorkTable();
                try { wt.RunInsDelUpd("ALTER TABLE Surgeries ALTER COLUMN Status NVARCHAR(50)"); } catch { }
                string todayStr = DateTime.Now.ToString("yyyy-MM-dd");
                string res = wt.RunInsDelUpd($"UPDATE Surgeries SET Status = 'Discharged:{todayStr}' WHERE Id = {surgeryId}");
                if (res != "OK")
                    return new { Status = "Error", Message = res };

                string syncRes = SyncRoomChargesToInvoice(surgeryId);
                if (syncRes != "OK")
                    return new { Status = "Error", Message = syncRes };

                return new { Status = "OK" };
            }
            catch (Exception ex) { return new { Status = "Error", Message = ex.Message }; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object ConfirmStayAjax(int surgeryId)
        {
            try
            {
                WorkTable wt = new WorkTable();
                try { wt.RunInsDelUpd("ALTER TABLE Surgeries ALTER COLUMN Status NVARCHAR(50)"); } catch { }
                string todayStr = DateTime.Now.ToString("yyyy-MM-dd");
                string res = wt.RunInsDelUpd($"UPDATE Surgeries SET Status = 'Confirmed:{todayStr}' WHERE Id = {surgeryId}");
                if (res != "OK")
                    return new { Status = "Error", Message = res };

                string syncRes = SyncRoomChargesToInvoice(surgeryId);
                if (syncRes != "OK")
                    return new { Status = "Error", Message = syncRes };

                return new { Status = "OK" };
            }
            catch (Exception ex) { return new { Status = "Error", Message = ex.Message }; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object PrepareInvoicePrint(int invoiceId)
        {
            try
            {
                WorkTable wt = new WorkTable();
                DataTable dt = wt.RunSelect("SELECT * FROM SurgeryInvoices WHERE Id=" + invoiceId);
                if (dt.Rows.Count == 0)
                    return new { Status = "Error", Message = "Invoice not found" };

                var row = dt.Rows[0];
                int surgeryId = row["SurgeryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["SurgeryId"]);

                DataTable dtS = wt.RunSelect("SELECT s.*, p.PatientName FROM Surgeries s LEFT JOIN Patients p ON s.PatientId=p.Id WHERE s.Id=" + surgeryId);

                string patientName = "", surgeryName = "", roomName = "", date = "", patientId = "";

                if (dtS.Rows.Count > 0)
                {
                    var sr = dtS.Rows[0];
                    patientName = sr.Table.Columns.Contains("PatientName") ? sr["PatientName"].ToString() : "";
                    surgeryName = sr.Table.Columns.Contains("SurgeryName") ? sr["SurgeryName"].ToString() : "";
                    date = sr.Table.Columns.Contains("SurgeryDate") ? Convert.ToDateTime(sr["SurgeryDate"]).ToString("yyyy/MM/dd HH:mm") : "";
                    patientId = sr.Table.Columns.Contains("PatientId") ? sr["PatientId"].ToString() : "";
                    if (sr.Table.Columns.Contains("RoomId") && sr["RoomId"] != DBNull.Value)
                    {
                        DataTable dr = wt.RunSelect("SELECT RoomName FROM OperationRooms WHERE Id=" + sr["RoomId"]);
                        if (dr.Rows.Count > 0) roomName = dr.Rows[0]["RoomName"].ToString();
                    }
                }

                System.Web.HttpContext.Current.Session["SurgInv_SurgeryId"] = surgeryId.ToString();
                System.Web.HttpContext.Current.Session["SurgInv_SurgeryName"] = surgeryName;
                System.Web.HttpContext.Current.Session["SurgInv_Patient"] = patientName;
                System.Web.HttpContext.Current.Session["SurgInv_PatientId"] = patientId;
                System.Web.HttpContext.Current.Session["SurgInv_Room"] = roomName;
                System.Web.HttpContext.Current.Session["SurgInv_Date"] = date;
                System.Web.HttpContext.Current.Session["SurgInv_DoctorCost"] = Convert.ToDecimal(row["DoctorCost"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_RoomCost"] = Convert.ToDecimal(row["RoomCost"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_SuppliesCost"] = Convert.ToDecimal(row["SuppliesCost"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_OtherCost"] = Convert.ToDecimal(row["OtherCost"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_Discount"] = Convert.ToDecimal(row["Discount"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_Total"] = Convert.ToDecimal(row["TotalAmount"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_Paid"] = Convert.ToDecimal(row["PaidAmount"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_Remaining"] = Convert.ToDecimal(row["RemainingAmount"]).ToString("0.##");
                System.Web.HttpContext.Current.Session["SurgInv_PaymentStatus"] = row["PaymentStatus"].ToString();
                System.Web.HttpContext.Current.Session["SurgInv_SurgeryBarcode"] = GenerateBarcodeBase64Local("SUR-" + surgeryId.ToString("000000"));
                System.Web.HttpContext.Current.Session["SurgInv_InvoiceBarcode"] = GenerateBarcodeBase64Local("INV-" + invoiceId.ToString("000000"));
                System.Web.HttpContext.Current.Session["SurgInv_SurgeryBarcodeText"] = "SUR-" + surgeryId.ToString("000000");
                System.Web.HttpContext.Current.Session["SurgInv_InvoiceBarcodeText"] = "INV-" + invoiceId.ToString("000000");

                CSurgerySupplies sss = new CSurgerySupplies();
                DataTable dtSup2 = sss.GetBySurgery(surgeryId);
                System.Web.HttpContext.Current.Session["SurgInv_Supplies"] = dtSup2;
                System.Web.HttpContext.Current.Session["SurgInv_PrintDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                return new { Status = "OK" };
            }
            catch (Exception ex) { return new { Status = "Error", Message = ex.Message }; }
        }
    }
}
