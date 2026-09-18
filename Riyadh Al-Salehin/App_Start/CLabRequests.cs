using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CLabRequests : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int ReferralId { get; set; }
    public int PatientId { get; set; }
    public string TestType { get; set; }
    public string Status { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT lr.*,p.PatientName
          FROM LabRequests lr
          INNER JOIN Patients p
          ON lr.PatientId=p.Id");
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@ReferralId", ReferralId));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@TestType", TestType));
        prm.Add(new SqlParameter("@Status", Status));

        return wt.RunInsDelUpd(
        @"INSERT INTO LabRequests
        (ReferralId,PatientId,TestType,Status)
        VALUES
        (@ReferralId,@PatientId,@TestType,@Status)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Status", Status));

        return wt.RunInsDelUpd(
        @"UPDATE LabRequests
          SET Status=@Status
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM LabRequests WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}