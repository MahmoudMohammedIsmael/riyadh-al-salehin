using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CInvoices : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int CenterId { get; set; }
    public int CreatedBy { get; set; }

    public string BookingType { get; set; }

    public string PaymentStatus { get; set; }

    public string QueueNumber { get; set; }

    public bool Printed { get; set; }

    public DateTime? PrintedDate { get; set; }


    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string DiscountType { get; set; }
    public string PaymentMethod { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DoctorCommission { get; set; }
    public decimal CenterCommission { get; set; }
    public int ServiceId { get; set; }
    public DateTime InvoiceDate { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT i.*,
               p.PatientName,
               d.DoctorName,
               mc.CenterName
        FROM Invoices i
        INNER JOIN Patients p ON i.PatientId=p.Id
        INNER JOIN Doctors d ON i.DoctorId=d.Id
        INNER JOIN MedicalCenters mc ON i.CenterId=mc.Id
        ORDER BY i.InvoiceDate DESC");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
            "SELECT * FROM Invoices WHERE Id=" + id);
    }

    public DataTable GetByPatient(int patientId)
    {
        return wt.RunSelect(@"
        SELECT *
        FROM Invoices
        WHERE PatientId=" + patientId +
        " ORDER BY InvoiceDate DESC");
    }

    public DataTable GetByDate(DateTime date)
    {
        return wt.RunSelect(@"
        SELECT *
        FROM Invoices
        WHERE CAST(InvoiceDate AS DATE)='" +
        date.ToString("yyyy-MM-dd") + "'");
    }

    public string Insert()
    {
        if (string.IsNullOrWhiteSpace(QueueNumber) && DoctorId > 0)
        {
            QueueNumber = GenerateQueueNumber(DoctorId, AppointmentId > 0 ? (int?)AppointmentId : null);
        }

        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@AppointmentId", AppointmentId));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@DoctorId", DoctorId));
        prm.Add(new SqlParameter("@CenterId", CenterId));
        prm.Add(new SqlParameter("@CreatedBy", CreatedBy));

        prm.Add(new SqlParameter("@ServiceId", ServiceId));

        prm.Add(new SqlParameter("@TotalAmount", TotalAmount));
        prm.Add(new SqlParameter("@DiscountAmount", DiscountAmount));
        prm.Add(new SqlParameter(
            "@DiscountType",
            (object)DiscountType ?? DBNull.Value));

        prm.Add(new SqlParameter(
            "@PaymentMethod",
            (object)PaymentMethod ?? DBNull.Value));

        prm.Add(new SqlParameter("@PaidAmount", PaidAmount));
        prm.Add(new SqlParameter("@DoctorCommission", DoctorCommission));
        prm.Add(new SqlParameter("@CenterCommission", CenterCommission));

        prm.Add(new SqlParameter(
            "@BookingType",
            (object)BookingType ?? DBNull.Value));

        prm.Add(new SqlParameter(
            "@PaymentStatus",
            (object)PaymentStatus ?? DBNull.Value));

        prm.Add(new SqlParameter(
            "@QueueNumber",
            (object)QueueNumber ?? DBNull.Value));

        prm.Add(new SqlParameter("@Printed", Printed));

        prm.Add(new SqlParameter(
            "@PrintedDate",
            (object)PrintedDate ?? DBNull.Value));

        return wt.RunInsDelUpd(@"
INSERT INTO Invoices
(
    AppointmentId,
    PatientId,
    DoctorId,
    CenterId,
    CreatedBy,
    ServiceId,
    TotalAmount,
    DiscountAmount,
    DiscountType,
    PaymentMethod,
    PaidAmount,
    DoctorCommission,
    CenterCommission,
    BookingType,
    PaymentStatus,
    QueueNumber,
    Printed,
    PrintedDate
)
VALUES
(
    @AppointmentId,
    @PatientId,
    @DoctorId,
    @CenterId,
    @CreatedBy,
    @ServiceId,
    @TotalAmount,
    @DiscountAmount,
    @DiscountType,
    @PaymentMethod,
    @PaidAmount,
    @DoctorCommission,
    @CenterCommission,
    @BookingType,
    @PaymentStatus,
    @QueueNumber,
    @Printed,
    @PrintedDate
)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@TotalAmount", TotalAmount));
        prm.Add(new SqlParameter("@DiscountAmount", DiscountAmount));
        prm.Add(new SqlParameter("@DiscountType", DiscountType));
        prm.Add(new SqlParameter("@PaymentMethod", PaymentMethod));
        prm.Add(new SqlParameter("@PaidAmount", PaidAmount));

        return wt.RunInsDelUpd(@"
        UPDATE Invoices
        SET
            TotalAmount=@TotalAmount,
            DiscountAmount=@DiscountAmount,
            DiscountType=@DiscountType,
            PaymentMethod=@PaymentMethod,
            PaidAmount=@PaidAmount
        WHERE Id=@Id", prm);
    }


    public int GetLastInvoiceId()
    {
        DataTable dt =
            wt.RunSelect(
            "SELECT ISNULL(MAX(Id),0) Id FROM Invoices");

        return Convert.ToInt32(dt.Rows[0]["Id"]);
    }



    public string GenerateQueueNumber(int doctorId, int? appointmentId = null)
    {
        string doctorCode = Riyadh_Al_Salehin.CDoctors.GetDoctorCode(doctorId);

        if (appointmentId.HasValue && appointmentId.Value > 0)
        {
            try
            {
                DataTable apptDt = wt.RunSelect($"SELECT QueueNumber FROM Appointments WHERE Id = {appointmentId.Value}");
                if (apptDt != null && apptDt.Rows.Count > 0 && apptDt.Rows[0]["QueueNumber"] != DBNull.Value)
                {
                    int qNum = Convert.ToInt32(apptDt.Rows[0]["QueueNumber"]);
                    if (qNum > 0)
                        return $"{doctorCode}{qNum}";
                }
            }
            catch { }
        }

        DataTable dt = wt.RunSelect($@"
            SELECT ISNULL(COUNT(*), 0) + 1 AS Num
            FROM Invoices
            WHERE DoctorId = {doctorId}
              AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)");

        int num = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["Num"]) : 1;
        return $"{doctorCode}{num}";
    }

    public string GenerateQueueNumber(int doctorId, string doctorCode)
    {
        if (string.IsNullOrWhiteSpace(doctorCode))
            doctorCode = Riyadh_Al_Salehin.CDoctors.GetDoctorCode(doctorId);
        doctorCode = doctorCode.Trim().ToUpper();

        DataTable dt = wt.RunSelect($@"
            SELECT ISNULL(COUNT(*), 0) + 1 AS Num
            FROM Invoices
            WHERE DoctorId = {doctorId}
              AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)");

        int num = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["Num"]) : 1;
        return $"{doctorCode}{num}";
    }

    public string GenerateQueueNumber()
    {
        if (DoctorId > 0)
            return GenerateQueueNumber(DoctorId, AppointmentId > 0 ? (int?)AppointmentId : null);

        DataTable dt = wt.RunSelect(@"
            SELECT ISNULL(COUNT(*), 0) + 1 AS Num
            FROM Invoices
            WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)");

        int num = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["Num"]) : 1;
        return "A" + num.ToString();
    }
    public string AdjustTotalAmount(int invoiceId, decimal delta)
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", invoiceId));
        prm.Add(new SqlParameter("@Delta", delta));

        return wt.RunInsDelUpd(@"
        UPDATE Invoices
        SET TotalAmount = TotalAmount + @Delta
        WHERE Id = @Id", prm);
    }


    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
            "DELETE FROM Invoices WHERE Id=" + id);
    }

    public string Add()
    {
        return Insert();
    }

    public string Delete()
    {
        return Delete(Id);
    }

    public DataTable Search()
    {
        return GetAll();
    }
}