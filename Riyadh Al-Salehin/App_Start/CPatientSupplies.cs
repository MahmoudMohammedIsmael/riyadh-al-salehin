using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPatientSupplies : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int SurgeryId { get; set; }
    public int PatientId { get; set; }
    public int SupplyId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT ps.*,
               p.PatientName,
               ms.SupplyName
        FROM PatientSupplies ps
        INNER JOIN Patients p ON ps.PatientId=p.Id
        INNER JOIN MedicalSupplies ms ON ps.SupplyId=ms.Id");
    }

    public DataTable GetByPatient(int patientId)
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@PatientId", patientId));

        return wt.RunSelect(@"
    SELECT
        ps.Id,
        ps.SurgeryId,
        ps.PatientId,
        ps.SupplyId,
        ms.SupplyName,
        ps.Quantity,
        ps.Price
    FROM PatientSupplies ps
    INNER JOIN MedicalSupplies ms
        ON ps.SupplyId = ms.Id
    WHERE ps.PatientId = @PatientId", prm);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@SurgeryId", SurgeryId));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@SupplyId", SupplyId));
        prm.Add(new SqlParameter("@Quantity", Quantity));
        prm.Add(new SqlParameter("@Price", Price));

        return wt.RunInsDelUpd(@"
        INSERT INTO PatientSupplies
        (
            SurgeryId,
            PatientId,
            SupplyId,
            Quantity,
            Price
        )
        VALUES
        (
            @SurgeryId,
            @PatientId,
            @SupplyId,
            @Quantity,
            @Price
        )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Quantity", Quantity));
        prm.Add(new SqlParameter("@Price", Price));

        return wt.RunInsDelUpd(@"
        UPDATE PatientSupplies
        SET Quantity=@Quantity,
            Price=@Price
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM PatientSupplies WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}