using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CAccountsReport : WorkTable
{
    private WorkTable wt = new WorkTable();

    public DataTable GetInvoiceData(DateTime fromDate, DateTime toDate, int? doctorId = null)
    {
        string doctorFilter = doctorId.HasValue ? " AND i.DoctorId = @DoctorId" : "";

        string query = @"
            -- الجزء الأول: الخدمة الرئيسية من Invoices
            SELECT 
                i.Id AS InvoiceId,
                i.TotalAmount,
                i.PaidAmount,
                i.InvoiceDate,
                i.DoctorId,
                ms.ServiceName,
                ms.DoctorAmount AS FixedDoctor,
                ms.CenterAmount AS FixedCenter,
                sp.Category,
                sp.DoctorRate,
                sp.CenterRate,
                sp.BasePrice,
                CASE 
                    WHEN sp.DoctorRate IS NOT NULL THEN i.TotalAmount * (sp.DoctorRate / 100.0)
                    ELSE ms.DoctorAmount
                END AS ActualDoctor,
                CASE 
                    WHEN sp.CenterRate IS NOT NULL THEN i.TotalAmount * (sp.CenterRate / 100.0)
                    ELSE ms.CenterAmount
                END AS ActualCenter
            FROM Invoices i
            INNER JOIN MedicalServices ms ON i.ServiceId = ms.Id
            LEFT JOIN ServicesAndPrices sp ON ms.ServiceName = sp.ServiceName
            WHERE i.InvoiceDate BETWEEN @FromDate AND @ToDate
              AND LOWER(LTRIM(RTRIM(i.PaymentStatus))) = 'paid'
              " + doctorFilter + @"

            UNION ALL

            -- الجزء الثاني: الخدمات الإضافية من InvoiceItems
            SELECT 
                ii.InvoiceId AS InvoiceId,
                ii.TotalPrice AS TotalAmount,
                0 AS PaidAmount,
                i.InvoiceDate,
                i.DoctorId,
                sp.ServiceName,
                0 AS FixedDoctor,
                0 AS FixedCenter,
                'extra' AS Category,   -- تصنيف الخدمات الإضافية
                sp.DoctorRate,
                sp.CenterRate,
                sp.BasePrice,
                CASE 
                    WHEN sp.DoctorRate IS NOT NULL THEN ii.TotalPrice * (sp.DoctorRate / 100.0)
                    ELSE 0
                END AS ActualDoctor,
                CASE 
                    WHEN sp.CenterRate IS NOT NULL THEN ii.TotalPrice * (sp.CenterRate / 100.0)
                    ELSE 0
                END AS ActualCenter
            FROM InvoiceItems ii
            INNER JOIN Invoices i ON ii.InvoiceId = i.Id
            INNER JOIN ServicesAndPrices sp ON ii.ServiceId = sp.Id
            WHERE i.InvoiceDate BETWEEN @FromDate AND @ToDate
              AND LOWER(LTRIM(RTRIM(i.PaymentStatus))) = 'paid'
              " + doctorFilter;

        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate)
        };

        if (doctorId.HasValue)
            parameters.Add(new SqlParameter("@DoctorId", doctorId.Value));

        return wt.RunSelect(query, parameters);
    }

    public DataTable GetDoctors()
    {
        return wt.RunSelect("SELECT Id, DoctorName FROM Doctors ORDER BY DoctorName");
    }

    public decimal? GetDoctorCommissionRate(int doctorId)
    {
        DataTable dt = wt.RunSelect($"SELECT CommissionRate FROM Doctors WHERE Id = {doctorId}");
        if (dt.Rows.Count > 0 && dt.Rows[0]["CommissionRate"] != DBNull.Value)
            return Convert.ToDecimal(dt.Rows[0]["CommissionRate"]);
        return null;
    }

    public AccountsReportResult CalculateReport(DataTable dt)
    {
        var result = new AccountsReportResult();

        if (dt.Rows.Count == 0)
            return result;

        int totalVisits = dt.Rows.Count;
        int normalVisits = 0, consultVisits = 0, extraVisits = 0;
        decimal totalRevenue = 0, totalDoctor = 0, totalCenter = 0;

        var serviceGroups = new Dictionary<string, ServiceGroup>();

        foreach (DataRow row in dt.Rows)
        {
            string serviceName = row["ServiceName"].ToString().Trim();
            string category = row["Category"]?.ToString()?.Trim() ?? "";
            decimal total = Convert.ToDecimal(row["TotalAmount"]);
            decimal doctorAmt = Convert.ToDecimal(row["ActualDoctor"]);
            decimal centerAmt = Convert.ToDecimal(row["ActualCenter"]);

            totalRevenue += total;
            totalDoctor += doctorAmt;
            totalCenter += centerAmt;

            if (category.Equals("extra", StringComparison.OrdinalIgnoreCase))
                extraVisits++;
            else if (category.Equals("consultation", StringComparison.OrdinalIgnoreCase))
                consultVisits++;
            else if (category.Equals("examination", StringComparison.OrdinalIgnoreCase))
                normalVisits++;
            else
            {
                if (serviceName.IndexOf("استشارة", StringComparison.OrdinalIgnoreCase) >= 0)
                    consultVisits++;
                else if (serviceName.IndexOf("كشف", StringComparison.OrdinalIgnoreCase) >= 0)
                    normalVisits++;
                else
                    extraVisits++; // افتراضياً خدمة إضافية
            }

            if (!serviceGroups.ContainsKey(serviceName))
                serviceGroups[serviceName] = new ServiceGroup();

            var g = serviceGroups[serviceName];
            g.Count++;
            g.TotalAmount += total;
            g.DoctorTotal += doctorAmt;
            g.CenterTotal += centerAmt;
        }

        result.TotalVisits = totalVisits;
        result.NormalVisits = normalVisits;
        result.ConsultVisits = consultVisits;
        result.ExtraVisits = extraVisits;
        result.TotalRevenue = totalRevenue;
        result.TotalDoctor = totalDoctor;
        result.TotalCenter = totalCenter;
        result.DoctorPercent = totalRevenue > 0 ? (totalDoctor / totalRevenue) * 100 : 0;
        result.CenterPercent = totalRevenue > 0 ? (totalCenter / totalRevenue) * 100 : 0;

        result.Categories = new string[] { "كشف عادي", "استشارة", "خدمات إضافية" };
        if (totalVisits > 0)
        {
            result.RevenueByCategory = new decimal[]
            {
                totalRevenue * (normalVisits / (decimal)totalVisits),
                totalRevenue * (consultVisits / (decimal)totalVisits),
                totalRevenue * (extraVisits / (decimal)totalVisits)
            };
        }
        else
        {
            result.RevenueByCategory = new decimal[] { 0, 0, 0 };
        }
        result.VisitsByCategory = new int[] { normalVisits, consultVisits, extraVisits };

        result.DetailTable = new DataTable();
        result.DetailTable.Columns.Add("ServiceName", typeof(string));
        result.DetailTable.Columns.Add("Count", typeof(int));
        result.DetailTable.Columns.Add("TotalAmount", typeof(decimal));
        result.DetailTable.Columns.Add("DoctorTotal", typeof(decimal));
        result.DetailTable.Columns.Add("CenterTotal", typeof(decimal));
        result.DetailTable.Columns.Add("DoctorPercent", typeof(decimal));
        result.DetailTable.Columns.Add("CenterPercent", typeof(decimal));


        foreach (var kv in serviceGroups)
        {
            var g = kv.Value;
            decimal docPct = g.TotalAmount > 0 ? (g.DoctorTotal / g.TotalAmount) * 100 : 0;
            decimal centerPct = g.TotalAmount > 0 ? (g.CenterTotal / g.TotalAmount) * 100 : 0;
            result.DetailTable.Rows.Add(kv.Key, g.Count, g.TotalAmount, g.DoctorTotal, g.CenterTotal, docPct, centerPct);
        }

        return result;
    }
}

public class AccountsReportResult
{
    public int TotalVisits { get; set; }
    public int NormalVisits { get; set; }
    public int ConsultVisits { get; set; }
    public int ExtraVisits { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDoctor { get; set; }
    public decimal TotalCenter { get; set; }
    public decimal DoctorPercent { get; set; }
    public decimal CenterPercent { get; set; }
    public string[] Categories { get; set; }
    public decimal[] RevenueByCategory { get; set; }
    public int[] VisitsByCategory { get; set; }
    public DataTable DetailTable { get; set; }
}

public class ServiceGroup
{
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DoctorTotal { get; set; }
    public decimal CenterTotal { get; set; }
}