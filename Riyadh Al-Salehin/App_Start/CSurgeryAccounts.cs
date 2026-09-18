using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CSurgeryAccounts : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int SurgeryId { get; set; }
    public int PatientId { get; set; }

    public decimal TotalCost { get; set; }
    public decimal AdvancePayment { get; set; }
    public decimal RemainingAmount { get; set; }

    public decimal RoomCost { get; set; }
    public decimal DoctorCommission { get; set; }
    public decimal CenterCommission { get; set; }

    public string PaymentStatus { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT sa.*,
               p.PatientName,
               s.SurgeryName
        FROM SurgeryAccounts sa
        INNER JOIN Patients p ON sa.PatientId=p.Id
        INNER JOIN Surgeries s ON sa.SurgeryId=s.Id");
    }

    public DataTable GetBySurgery(int surgeryId)
    {
        return wt.RunSelect(
        "SELECT * FROM SurgeryAccounts WHERE SurgeryId=" + surgeryId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@SurgeryId", SurgeryId));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@TotalCost", TotalCost));
        prm.Add(new SqlParameter("@RoomCost", RoomCost));
        prm.Add(new SqlParameter("@AdvancePayment", AdvancePayment));
        prm.Add(new SqlParameter("@RemainingAmount", RemainingAmount));
        prm.Add(new SqlParameter("@DoctorCommission", DoctorCommission));
        prm.Add(new SqlParameter("@CenterCommission", CenterCommission));
        prm.Add(new SqlParameter("@PaymentStatus", PaymentStatus));

        return wt.RunInsDelUpd(@"
       INSERT INTO SurgeryAccounts
(
    SurgeryId,
    PatientId,
    RoomCost,
    TotalCost,
    AdvancePayment,
    RemainingAmount,
    DoctorCommission,
    CenterCommission,
    PaymentStatus
)
VALUES
(
    @SurgeryId,
    @PatientId,
    @RoomCost,
    @TotalCost,
    @AdvancePayment,
    @RemainingAmount,
    @DoctorCommission,
    @CenterCommission,
    @PaymentStatus
)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@AdvancePayment", AdvancePayment));
        prm.Add(new SqlParameter("@RemainingAmount", RemainingAmount));
        prm.Add(new SqlParameter("@PaymentStatus", PaymentStatus));

        return wt.RunInsDelUpd(@"
        UPDATE SurgeryAccounts
        SET AdvancePayment=@AdvancePayment,
            RemainingAmount=@RemainingAmount,
            PaymentStatus=@PaymentStatus
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM SurgeryAccounts WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}