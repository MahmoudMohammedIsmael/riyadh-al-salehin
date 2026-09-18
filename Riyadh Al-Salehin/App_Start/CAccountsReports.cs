using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public class CAccountsReports
    {
       

        private static DataTable ExecuteQuery(string sql, List<SqlParameter> parameters)
        {
            WorkTable wt = new WorkTable();
            return wt.RunSelect(sql, parameters);
        }

        
        public static void LoadDoctors(DropDownList ddl)
        {
            string sql = "SELECT Id, DoctorName FROM Doctors ORDER BY DoctorName";
            DataTable dt = ExecuteQuery(sql, null);
            ddl.DataTextField = "DoctorName";
            ddl.DataValueField = "Id";
            ddl.DataSource = dt;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("الكل", ""));
        }

        public static void LoadCenters(DropDownList ddl)
        {
            string sql = "SELECT Id, CenterName FROM MedicalCenters ORDER BY CenterName";
            DataTable dt = ExecuteQuery(sql, null);
            ddl.DataTextField = "CenterName";
            ddl.DataValueField = "Id";
            ddl.DataSource = dt;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("الكل", ""));
        }

        public static void LoadDoctorTypes(DropDownList ddl)
        {
            string sql = "SELECT DISTINCT DoctorType FROM Doctors WHERE DoctorType IS NOT NULL ORDER BY DoctorType";
            DataTable dt = ExecuteQuery(sql, null);
            ddl.DataTextField = "DoctorType";
            ddl.DataValueField = "DoctorType";
            ddl.DataSource = dt;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("الكل", ""));
        }

        public static void LoadPaymentMethods(DropDownList ddl)
        {
            string sql = "SELECT DISTINCT PaymentMethod FROM Invoices WHERE PaymentMethod IS NOT NULL ORDER BY PaymentMethod";
            DataTable dt = ExecuteQuery(sql, null);
            ddl.DataTextField = "PaymentMethod";
            ddl.DataValueField = "PaymentMethod";
            ddl.DataSource = dt;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("الكل", ""));
        }

        
        private static void AppendInvoiceFilter(StringBuilder sql, List<SqlParameter> parameters,
            string alias, string doctorId, string centerId, string paymentMethod)
        {
            if (!string.IsNullOrEmpty(doctorId))
            {
                sql.Append($" AND {alias}.DoctorId = @DoctorId");
                parameters.Add(new SqlParameter("@DoctorId", doctorId));
            }
            if (!string.IsNullOrEmpty(centerId))
            {
                sql.Append($" AND {alias}.CenterId = @CenterId");
                parameters.Add(new SqlParameter("@CenterId", centerId));
            }
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                sql.Append($" AND {alias}.PaymentMethod = @PaymentMethod");
                parameters.Add(new SqlParameter("@PaymentMethod", paymentMethod));
            }
        }

        private static void AppendDoctorTypeFilter(StringBuilder sql, List<SqlParameter> parameters,
            string invoiceAlias, string doctorType)
        {
            if (!string.IsNullOrEmpty(doctorType))
            {
                sql.Append($" AND {invoiceAlias}.DoctorId IN (SELECT Id FROM Doctors WHERE DoctorType = @DoctorType)");
                parameters.Add(new SqlParameter("@DoctorType", doctorType));
            }
        }

        private static void AppendDoctorFilter(StringBuilder sql, List<SqlParameter> parameters,
            string doctorId, string doctorType)
        {
            if (!string.IsNullOrEmpty(doctorId))
            {
                sql.Append(" AND d.Id = @DoctorId");
                parameters.Add(new SqlParameter("@DoctorId", doctorId));
            }
            if (!string.IsNullOrEmpty(doctorType))
            {
                sql.Append(" AND d.DoctorType = @DoctorType");
                parameters.Add(new SqlParameter("@DoctorType", doctorType));
            }
        }

        public static DataTable GetDashboardSummary(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT
                        i.Id, i.PatientId, i.DoctorId, i.AppointmentId,
                        i.TotalAmount, i.PaidAmount, i.DiscountAmount,
                        ms.DoctorAmount AS MainDoctorAmount,
                        ms.CenterAmount AS MainCenterAmount
                    FROM Invoices i
                    LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, paymentMethod);
            AppendDoctorTypeFilter(sql, parameters, "i", doctorType);

            sql.Append(@"
                ),
                ItemsAgg AS (
                    SELECT ii.InvoiceId, SUM(ii.TotalPrice) AS ItemsTotal
                    FROM InvoiceItems ii
                    INNER JOIN FilteredInvoices fi ON fi.Id = ii.InvoiceId
                    GROUP BY ii.InvoiceId
                )
                SELECT
                    ISNULL(SUM(fi.TotalAmount), 0) AS TotalRevenue,
                    ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaid,
                    ISNULL(SUM(fi.DiscountAmount), 0) AS TotalDiscount,
                    ISNULL(SUM(fi.TotalAmount - fi.PaidAmount), 0) AS TotalRemaining,
                    -- عمولة الطبيب تُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainDoctorAmount ELSE 0 END), 0) AS TotalDoctorCommission,
                    -- نصيب المركز يُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainCenterAmount ELSE 0 END), 0) AS TotalCenterCommission,
                    ISNULL((SELECT SUM(ItemsTotal) FROM ItemsAgg), 0) AS TotalAdditionalServices,
                    COUNT(DISTINCT fi.Id) AS InvoiceCount,
                    COUNT(DISTINCT fi.PatientId) AS PatientCount,
                    COUNT(DISTINCT fi.AppointmentId) AS VisitCount,
                    COUNT(DISTINCT fi.DoctorId) AS ActiveDoctorCount,
                    -- عدد الفواتير المدفوعة وغير المدفوعة
                    COUNT(DISTINCT CASE WHEN fi.PaidAmount > 0 THEN fi.Id END) AS PaidInvoiceCount,
                    COUNT(DISTINCT CASE WHEN fi.PaidAmount = 0 OR fi.PaidAmount IS NULL THEN fi.Id END) AS UnpaidInvoiceCount,
                    -- إجمالي المدفوع فقط (لحساب النسب)
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.TotalAmount ELSE 0 END), 0) AS PaidInvoicesTotal
                FROM FilteredInvoices fi
            ");

            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetDoctorsAccounts(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT
                        i.Id, i.DoctorId, i.PatientId, i.AppointmentId, i.CenterId,
                        i.TotalAmount, i.PaidAmount, i.DiscountAmount, i.PaymentMethod,
                        ms.DoctorAmount AS MainDoctorAmount,
                        ms.CenterAmount AS MainCenterAmount
                    FROM Invoices i
                    LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", null, centerId, paymentMethod);

            sql.Append(@"
                ),
                ItemsAgg AS (
                    SELECT ii.InvoiceId, SUM(ii.TotalPrice) AS ItemsTotal
                    FROM InvoiceItems ii
                    INNER JOIN FilteredInvoices fi ON fi.Id = ii.InvoiceId
                    GROUP BY ii.InvoiceId
                ),
                InvoiceWithItems AS (
                    SELECT fi.*, ISNULL(ia.ItemsTotal, 0) AS ItemsTotal
                    FROM FilteredInvoices fi
                    LEFT JOIN ItemsAgg ia ON ia.InvoiceId = fi.Id
                )
                SELECT
                    d.Id AS DoctorId,
                    d.DoctorName,
                    d.Specialty,
                    d.DoctorType,
                    d.CommissionRate AS DoctorCommissionRate,
                    COUNT(DISTINCT iwi.PatientId) AS PatientCount,
                    COUNT(DISTINCT iwi.AppointmentId) AS VisitCount,
                    COUNT(DISTINCT iwi.Id) AS InvoiceCount,
                    -- إجمالي كل الفواتير (مدفوعة + غير مدفوعة)
                    ISNULL(SUM(iwi.TotalAmount), 0) AS TotalInvoicesAmount,
                    ISNULL(SUM(iwi.ItemsTotal), 0) AS AdditionalServicesTotal,
                    0 AS RadiologyTotal,
                    ISNULL(SUM(iwi.TotalAmount), 0) AS TotalRevenue,
                    ISNULL(SUM(iwi.DiscountAmount), 0) AS TotalDiscount,
                    ISNULL(SUM(iwi.PaidAmount), 0) AS TotalPaid,
                    ISNULL(SUM(iwi.TotalAmount - iwi.PaidAmount), 0) AS TotalRemaining,
                    -- عمولة الطبيب تُحسب فقط على الفواتير المدفوعة (PaidAmount > 0)
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS DoctorCommission,
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS DoctorNet,
                    -- نصيب المركز يُحسب فقط على الفواتير المدفوعة (PaidAmount > 0)
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.MainCenterAmount ELSE 0 END), 0) AS CenterNet,
                    -- أعمدة إضافية للتوضيح
                    COUNT(DISTINCT CASE WHEN iwi.PaidAmount > 0 THEN iwi.Id END) AS PaidInvoiceCount,
                    COUNT(DISTINCT CASE WHEN iwi.PaidAmount = 0 OR iwi.PaidAmount IS NULL THEN iwi.Id END) AS UnpaidInvoiceCount,
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.TotalAmount ELSE 0 END), 0) AS PaidInvoicesTotal
                FROM Doctors d
                LEFT JOIN InvoiceWithItems iwi ON iwi.DoctorId = d.Id
                WHERE 1 = 1
            ");

            AppendDoctorFilter(sql, parameters, doctorId, doctorType);

            sql.Append(@"
                GROUP BY d.Id, d.DoctorName, d.Specialty, d.DoctorType, d.CommissionRate
                HAVING COUNT(DISTINCT iwi.Id) > 0
                ORDER BY d.DoctorName
            ");

            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetAdditionalServicesReport(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                SELECT
                    sap.ServiceName,
                    sap.Category,
                    COUNT(ii.Id) AS UsageCount,
                    COUNT(DISTINCT i.PatientId) AS PatientCount,
                    SUM(ii.Quantity) AS TotalQuantity,
                    SUM(ii.TotalPrice) AS TotalValue,
                    ISNULL(SUM(CASE WHEN (i.TotalAmount - i.PaidAmount) > 0 THEN ii.TotalPrice ELSE 0 END), 0) AS TotalRemaining,
                    0 AS TotalPaid,
                    0 AS DoctorShare,
                    0 AS CenterShare
                FROM InvoiceItems ii
                INNER JOIN Invoices i ON ii.InvoiceId = i.Id
                INNER JOIN ServicesAndPrices sap ON ii.ServiceId = sap.Id
                WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                  AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, paymentMethod);

            sql.Append(" GROUP BY sap.ServiceName, sap.Category ORDER BY TotalValue DESC");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetRadiologyReport(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                SELECT 
                    r.CreatedAt AS RequestDate,
                    p.PatientName,
                    s.XrayName AS ServiceName,
                    s.Price AS TotalPaid,
                    ISNULL(d.DoctorName, 'غير محدد') AS DoctorName
                FROM XrayRequests r
                INNER JOIN XrayServices s ON r.XrayServiceId = s.Id
                LEFT JOIN Patients p ON r.PatientId = p.Id
                LEFT JOIN Doctors d ON r.DoctorId = d.Id
                WHERE r.IsBilled = 1 
                  AND r.CreatedAt >= @FromDate AND r.CreatedAt < DATEADD(DAY, 1, @ToDate)
            ");
            
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            if (!string.IsNullOrEmpty(doctorId))
            {
                sql.Append(" AND r.DoctorId = @DoctorId");
                parameters.Add(new SqlParameter("@DoctorId", doctorId));
            }

            sql.Append(" ORDER BY r.CreatedAt DESC");
            
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetPaymentMethodsSummary(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType = null)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                SELECT
                    i.PaymentMethod,
                    COUNT(*) AS TransactionCount,
                    ISNULL(SUM(i.TotalAmount), 0) AS TotalInvoices,
                    ISNULL(SUM(i.PaidAmount), 0) AS TotalPaid,
                    ISNULL(SUM(i.TotalAmount - i.PaidAmount), 0) AS TotalRemaining
                FROM Invoices i
                WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                  AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, null);
            AppendDoctorTypeFilter(sql, parameters, "i", doctorType);

            sql.Append(" GROUP BY i.PaymentMethod ORDER BY i.PaymentMethod");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetCenterSummary(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT
                        i.Id, i.PatientId, i.DoctorId, i.AppointmentId,
                        i.TotalAmount, i.PaidAmount, i.DiscountAmount,
                        ms.DoctorAmount AS MainDoctorAmount,
                        ms.CenterAmount AS MainCenterAmount
                    FROM Invoices i
                    LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, paymentMethod);
            AppendDoctorTypeFilter(sql, parameters, "i", doctorType);

            sql.Append(@"
                ),
                ItemsAgg AS (
                    SELECT ii.InvoiceId, SUM(ii.TotalPrice) AS ItemsTotal
                    FROM InvoiceItems ii
                    INNER JOIN FilteredInvoices fi ON fi.Id = ii.InvoiceId
                    GROUP BY ii.InvoiceId
                )
                SELECT
                    ISNULL(SUM(fi.TotalAmount), 0) AS TotalRevenue,
                    ISNULL(SUM(fi.DiscountAmount), 0) AS TotalDiscount,
                    ISNULL(SUM(fi.PaidAmount), 0) AS TotalPaid,
                    ISNULL(SUM(fi.TotalAmount - fi.PaidAmount), 0) AS TotalRemaining,
                    -- عمولة الطبيب تُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainDoctorAmount ELSE 0 END), 0) AS TotalDoctorCommission,
                    -- نصيب المركز يُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainCenterAmount ELSE 0 END), 0) AS CenterNet,
                    ISNULL((SELECT SUM(ItemsTotal) FROM ItemsAgg), 0) AS AdditionalTotal,
                    0 AS RadiologyTotal,
                    COUNT(DISTINCT fi.AppointmentId) AS VisitCount,
                    COUNT(DISTINCT fi.PatientId) AS PatientCount,
                    COUNT(DISTINCT fi.Id) AS InvoiceCount,
                    COUNT(DISTINCT fi.DoctorId) AS ActiveDoctorCount
                FROM FilteredInvoices fi
            ");

            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetDoctorDetails(DateTime fromDate, DateTime toDate, string doctorId)
        {
            string sql = @"
                SELECT
                    i.Id AS InvoiceId,
                    i.PatientId,
                    p.PatientName,
                    i.InvoiceDate,
                    i.BookingType,
                    i.ServiceId,
                    ms.ServiceName,
                    ms.TotalAmount AS ServicePrice,
                    i.TotalAmount,
                    i.DiscountAmount,
                    i.PaidAmount,
                    (i.TotalAmount - i.PaidAmount) AS Remaining,
                    i.PaymentMethod,
                    ISNULL(ms.DoctorAmount, 0) AS DoctorCommission,
                    ISNULL(ms.CenterAmount, 0) AS CenterCommission,
                    d.DoctorName,
                    d.Specialty
                FROM Invoices i
                INNER JOIN Doctors d ON i.DoctorId = d.Id
                LEFT JOIN Patients p ON i.PatientId = p.Id
                LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                WHERE i.DoctorId = @DoctorId
                  AND i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                  AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
                ORDER BY i.InvoiceDate DESC
            ";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            return ExecuteQuery(sql, parameters);
        }


        public static DataTable GetDailyRevenueChart(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                SELECT
                    CONVERT(DATE, i.InvoiceDate) AS DateLabel,
                    ISNULL(SUM(i.TotalAmount), 0) AS Revenue
                FROM Invoices i
                WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                  AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, paymentMethod);
            sql.Append(" GROUP BY CONVERT(DATE, i.InvoiceDate) ORDER BY DateLabel");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetDoctorRevenueChart(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT i.Id, i.DoctorId, i.CenterId, i.PaymentMethod, i.TotalAmount
                    FROM Invoices i
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            AppendInvoiceFilter(sql, parameters, "i", null, centerId, paymentMethod);
            sql.Append(@"
                )
                SELECT
                    d.DoctorName,
                    ISNULL(SUM(fi.TotalAmount), 0) AS Revenue
                FROM Doctors d
                LEFT JOIN FilteredInvoices fi ON fi.DoctorId = d.Id
                WHERE 1 = 1
            ");
            AppendDoctorFilter(sql, parameters, doctorId, doctorType);
            sql.Append(" GROUP BY d.DoctorName ORDER BY Revenue DESC");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetPaymentMethodChart(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                SELECT
                    i.PaymentMethod,
                    ISNULL(SUM(i.PaidAmount), 0) AS TotalPaid
                FROM Invoices i
                WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                  AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, null);
            sql.Append(" GROUP BY i.PaymentMethod");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetDoctorCenterPieChart(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT
                        i.Id,
                        i.PaidAmount,
                        ms.DoctorAmount AS MainDoctorAmount,
                        ms.CenterAmount AS MainCenterAmount
                    FROM Invoices i
                    LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            AppendInvoiceFilter(sql, parameters, "i", doctorId, centerId, paymentMethod);
            AppendDoctorTypeFilter(sql, parameters, "i", doctorType);
            sql.Append(@"
                )
                SELECT
                    -- عمولة الطبيب تُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainDoctorAmount ELSE 0 END), 0) AS DoctorTotal,
                    -- نصيب المركز يُحسب فقط على الفواتير المدفوعة
                    ISNULL(SUM(CASE WHEN fi.PaidAmount > 0 THEN fi.MainCenterAmount ELSE 0 END), 0) AS CenterTotal
                FROM FilteredInvoices fi
            ");
            return ExecuteQuery(sql.ToString(), parameters);
        }

        public static DataTable GetDoctorVisitBreakdown(DateTime fromDate, DateTime toDate,
            string doctorId, string centerId, string doctorType, string paymentMethod)
        {
            var sql = new StringBuilder();
            sql.Append(@"
                WITH FilteredInvoices AS (
                    SELECT
                        i.Id,
                        i.DoctorId,
                        i.TotalAmount,
                        i.PaidAmount,
                        i.DiscountAmount,
                        i.ServiceId,
                        ms.ServiceName,
                        ms.DoctorAmount AS MainDoctorAmount,
                        ms.CenterAmount AS MainCenterAmount,
                        CASE
                            WHEN ms.ServiceName LIKE N'%استشارة%' THEN 'Consultation'
                            WHEN ms.ServiceName LIKE N'%كشف%' THEN 'Examination'
                            ELSE 'Other'
                        END AS VisitType
                    FROM Invoices i
                    LEFT JOIN MedicalServices ms ON i.ServiceId = ms.Id
                    WHERE i.InvoiceDate >= @FromDate AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
                      AND (i.PaymentStatus <> 'Cancelled' OR i.PaymentStatus IS NULL)
            ");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            AppendInvoiceFilter(sql, parameters, "i", null, centerId, paymentMethod);

            sql.Append(@"
                ),
                ItemsAgg AS (
                    SELECT ii.InvoiceId, COUNT(ii.Id) AS AdditionalCount, SUM(ii.TotalPrice) AS ItemsTotal
                    FROM InvoiceItems ii
                    INNER JOIN FilteredInvoices fi ON fi.Id = ii.InvoiceId
                    GROUP BY ii.InvoiceId
                ),
                InvoiceWithItems AS (
                    SELECT fi.*, ISNULL(ia.AdditionalCount, 0) AS AdditionalCount, ISNULL(ia.ItemsTotal, 0) AS ItemsTotal
                    FROM FilteredInvoices fi
                    LEFT JOIN ItemsAgg ia ON ia.InvoiceId = fi.Id
                )
                SELECT
                    d.Id AS DoctorId,
                    d.DoctorName,
                    d.Specialty,
                    d.DoctorType,
                    d.CommissionRate AS DoctorCommissionRate,

                    -- إجمالي الكشفات (كل الفواتير)
                    COUNT(DISTINCT iwi.Id) AS TotalInvoices,

                    -- عدد الكشوفات
                    COUNT(DISTINCT CASE WHEN iwi.VisitType = 'Examination' THEN iwi.Id END) AS ExaminationCount,
                    -- عدد الاستشارات
                    COUNT(DISTINCT CASE WHEN iwi.VisitType = 'Consultation' THEN iwi.Id END) AS ConsultationCount,
                    -- عدد الأخرى
                    COUNT(DISTINCT CASE WHEN iwi.VisitType = 'Other' THEN iwi.Id END) AS OtherCount,

                    -- إجمالي الكشوفات (المبلغ)
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Examination' THEN iwi.TotalAmount ELSE 0 END), 0) AS ExaminationTotal,
                    -- إجمالي الاستشارات (المبلغ)
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Consultation' THEN iwi.TotalAmount ELSE 0 END), 0) AS ConsultationTotal,
                    -- إجمالي الأخرى (المبلغ)
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Other' THEN iwi.TotalAmount ELSE 0 END), 0) AS OtherTotal,

                    -- إجمالي المدفوع للكشوفات
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Examination' AND iwi.PaidAmount > 0 THEN iwi.TotalAmount ELSE 0 END), 0) AS ExaminationPaidTotal,
                    -- إجمالي المدفوع للاستشارات
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Consultation' AND iwi.PaidAmount > 0 THEN iwi.TotalAmount ELSE 0 END), 0) AS ConsultationPaidTotal,
                    -- إجمالي المدفوع للأخرى
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Other' AND iwi.PaidAmount > 0 THEN iwi.TotalAmount ELSE 0 END), 0) AS OtherPaidTotal,

                    -- نصيب الطبيب من الكشوفات المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Examination' AND iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS ExaminationDoctorNet,
                    -- نصيب المركز من الكشوفات المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Examination' AND iwi.PaidAmount > 0 THEN iwi.MainCenterAmount ELSE 0 END), 0) AS ExaminationCenterNet,

                    -- نصيب الطبيب من الاستشارات المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Consultation' AND iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS ConsultationDoctorNet,
                    -- نصيب المركز من الاستشارات المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Consultation' AND iwi.PaidAmount > 0 THEN iwi.MainCenterAmount ELSE 0 END), 0) AS ConsultationCenterNet,

                    -- نصيب الطبيب من الأخرى المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Other' AND iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS OtherDoctorNet,
                    -- نصيب المركز من الأخرى المدفوعة
                    ISNULL(SUM(CASE WHEN iwi.VisitType = 'Other' AND iwi.PaidAmount > 0 THEN iwi.MainCenterAmount ELSE 0 END), 0) AS OtherCenterNet,

                    -- الخدمات الإضافية
                    ISNULL(SUM(iwi.AdditionalCount), 0) AS AdditionalCount,
                    ISNULL(SUM(iwi.ItemsTotal), 0) AS AdditionalTotal,
                    0 AS AdditionalDoctorNet,

                    -- إجمالي نصيب الطبيب (من المدفوع فقط)
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.MainDoctorAmount ELSE 0 END), 0) AS TotalDoctorNet,
                    -- إجمالي نصيب المركز (من المدفوع فقط)
                    ISNULL(SUM(CASE WHEN iwi.PaidAmount > 0 THEN iwi.MainCenterAmount ELSE 0 END), 0) AS TotalCenterNet,

                    -- إجمالي كل الفواتير
                    ISNULL(SUM(iwi.TotalAmount), 0) AS GrandTotal,
                    -- إجمالي المدفوع
                    ISNULL(SUM(iwi.PaidAmount), 0) AS GrandPaid,
                    -- إجمالي المتبقي
                    ISNULL(SUM(iwi.TotalAmount - iwi.PaidAmount), 0) AS GrandRemaining

                FROM Doctors d
                LEFT JOIN InvoiceWithItems iwi ON iwi.DoctorId = d.Id
                WHERE 1 = 1
            ");

            AppendDoctorFilter(sql, parameters, doctorId, doctorType);

            sql.Append(@"
                GROUP BY d.Id, d.DoctorName, d.Specialty, d.DoctorType, d.CommissionRate
                HAVING COUNT(DISTINCT iwi.Id) > 0
                ORDER BY d.DoctorName
            ");

            return ExecuteQuery(sql.ToString(), parameters);
        }
    }
}