using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CSurgeryInvoices : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }

    public int SurgeryId { get; set; }

    public int PatientId { get; set; }

    public decimal DoctorCost { get; set; }

    public decimal RoomCost { get; set; }

    public decimal SuppliesCost { get; set; }

    public decimal OtherCost { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }

    public int CreatedBy { get; set; }

    public DateTime InvoiceDate { get; set; }


    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@SurgeryId", SurgeryId));
        prm.Add(new SqlParameter("@PatientId", PatientId));

        prm.Add(new SqlParameter("@DoctorCost", DoctorCost));
        prm.Add(new SqlParameter("@RoomCost", RoomCost));
        prm.Add(new SqlParameter("@SuppliesCost", SuppliesCost));
        prm.Add(new SqlParameter("@OtherCost", OtherCost));
        prm.Add(new SqlParameter("@InvoiceDate", InvoiceDate));

        prm.Add(new SqlParameter("@Discount", Discount));

        prm.Add(new SqlParameter("@TotalAmount", TotalAmount));

        prm.Add(new SqlParameter("@PaidAmount", PaidAmount));

        prm.Add(new SqlParameter("@RemainingAmount", RemainingAmount));

        prm.Add(new SqlParameter("@PaymentMethod", PaymentMethod));

        prm.Add(new SqlParameter("@PaymentStatus", PaymentStatus));

        prm.Add(new SqlParameter("@CreatedBy", CreatedBy));

        return wt.RunInsDelUpd(@"

INSERT INTO SurgeryInvoices
(
SurgeryId,
PatientId,

DoctorCost,
RoomCost,
SuppliesCost,
OtherCost,

Discount,

TotalAmount,

PaidAmount,

RemainingAmount,

PaymentMethod,

PaymentStatus,

InvoiceDate,
CreatedBy
)

VALUES
(
@SurgeryId,
@PatientId,

@DoctorCost,
@RoomCost,
@SuppliesCost,
@OtherCost,

@Discount,

@TotalAmount,

@PaidAmount,

@RemainingAmount,

@PaymentMethod,

@PaymentStatus,

@InvoiceDate,
@CreatedBy
)

", prm);

    }


    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));

        prm.Add(new SqlParameter("@DoctorCost", DoctorCost));
        prm.Add(new SqlParameter("@RoomCost", RoomCost));
        prm.Add(new SqlParameter("@SuppliesCost", SuppliesCost));
        prm.Add(new SqlParameter("@OtherCost", OtherCost));

        prm.Add(new SqlParameter("@Discount", Discount));

        prm.Add(new SqlParameter("@TotalAmount", TotalAmount));

        prm.Add(new SqlParameter("@PaidAmount", PaidAmount));

        prm.Add(new SqlParameter("@RemainingAmount", RemainingAmount));

        prm.Add(new SqlParameter("@PaymentMethod", PaymentMethod));

        prm.Add(new SqlParameter("@PaymentStatus", PaymentStatus));

        return wt.RunInsDelUpd(@"

UPDATE SurgeryInvoices

SET

DoctorCost=@DoctorCost,

RoomCost=@RoomCost,

SuppliesCost=@SuppliesCost,

OtherCost=@OtherCost,

Discount=@Discount,

TotalAmount=@TotalAmount,

PaidAmount=@PaidAmount,

RemainingAmount=@RemainingAmount,

PaymentMethod=@PaymentMethod,

PaymentStatus=@PaymentStatus

WHERE Id=@Id

", prm);

    }


    public string Delete(int id)
    {
        return wt.RunInsDelUpd(

        "DELETE FROM SurgeryInvoices WHERE Id=" + id);

    }


    public DataTable GetAll()
    {
        return wt.RunSelect(@"

SELECT

si.*,

p.PatientName,

d.DoctorName,

s.SurgeryName

FROM SurgeryInvoices si

INNER JOIN Surgeries s

ON si.SurgeryId=s.Id

INNER JOIN Patients p

ON si.PatientId=p.Id

INNER JOIN Doctors d

ON s.DoctorId=d.Id

ORDER BY si.Id DESC

");
    }


    public DataTable GetBySurgery(int surgeryId)
    {
        return wt.RunSelect(

        "SELECT * FROM SurgeryInvoices WHERE SurgeryId=" + surgeryId);

    }


    public DataTable GetAccounts()
    {
        return wt.RunSelect(@"
SELECT
    si.Id,
    si.InvoiceDate,
    s.Id AS SurgeryNo,
    p.PatientName,
    d.DoctorName,
    s.SurgeryName,
    si.TotalAmount,
    si.PaidAmount,
    si.RemainingAmount,
    si.PaymentStatus
FROM SurgeryInvoices si
INNER JOIN Surgeries s ON si.SurgeryId=s.Id
INNER JOIN Patients p ON si.PatientId=p.Id
INNER JOIN Doctors d ON s.DoctorId=d.Id
ORDER BY si.InvoiceDate DESC");
    }



    public DataTable GetSummary(DateTime from, DateTime to)
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@From", from));
        prm.Add(new SqlParameter("@To", to));

        return wt.RunSelect(@"

SELECT

COUNT(*) AS TotalInvoices,

SUM(TotalAmount) AS TotalSales,

SUM(PaidAmount) AS TotalPaid,

SUM(RemainingAmount) AS TotalRemaining

FROM SurgeryInvoices

WHERE
CAST(InvoiceDate AS DATE)
BETWEEN
@From AND @To

", prm);
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