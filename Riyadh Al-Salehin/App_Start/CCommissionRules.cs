using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CCommissionRules : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int CenterId { get; set; }

    public string ServiceType { get; set; }

    public decimal DoctorShare { get; set; }
    public decimal CenterShare { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT cr.*,
               d.DoctorName,
               mc.CenterName
        FROM CommissionRules cr
        INNER JOIN Doctors d ON cr.DoctorId=d.Id
        INNER JOIN MedicalCenters mc ON cr.CenterId=mc.Id");
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@DoctorId", DoctorId));
        prm.Add(new SqlParameter("@CenterId", CenterId));
        prm.Add(new SqlParameter("@ServiceType", ServiceType));
        prm.Add(new SqlParameter("@DoctorShare", DoctorShare));
        prm.Add(new SqlParameter("@CenterShare", CenterShare));

        return wt.RunInsDelUpd(@"
        INSERT INTO CommissionRules
        (
            DoctorId,
            CenterId,
            ServiceType,
            DoctorShare,
            CenterShare
        )
        VALUES
        (
            @DoctorId,
            @CenterId,
            @ServiceType,
            @DoctorShare,
            @CenterShare
        )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@DoctorShare", DoctorShare));
        prm.Add(new SqlParameter("@CenterShare", CenterShare));

        return wt.RunInsDelUpd(@"
        UPDATE CommissionRules
        SET DoctorShare=@DoctorShare,
            CenterShare=@CenterShare
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM CommissionRules WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}