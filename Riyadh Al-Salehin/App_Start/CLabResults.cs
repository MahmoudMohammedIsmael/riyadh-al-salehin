using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CLabResults : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int RequestId { get; set; }
    public string Result { get; set; }
    public string NormalRange { get; set; }
    public DateTime ResultDate { get; set; }
    public bool IsSMSSent { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT lr.*,lq.TestType,p.PatientName
          FROM LabResults lr
          INNER JOIN LabRequests lq ON lr.RequestId=lq.Id
          INNER JOIN Patients p ON lq.PatientId=p.Id");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        "SELECT * FROM LabResults WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@RequestId", RequestId));
        prm.Add(new SqlParameter("@Result", Result));
        prm.Add(new SqlParameter("@NormalRange", NormalRange));
        prm.Add(new SqlParameter("@ResultDate", ResultDate));
        prm.Add(new SqlParameter("@IsSMSSent", IsSMSSent));

        return wt.RunInsDelUpd(
        @"INSERT INTO LabResults
        (RequestId,Result,NormalRange,ResultDate,IsSMSSent)
        VALUES
        (@RequestId,@Result,@NormalRange,@ResultDate,@IsSMSSent)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Result", Result));
        prm.Add(new SqlParameter("@NormalRange", NormalRange));
        prm.Add(new SqlParameter("@ResultDate", ResultDate));
        prm.Add(new SqlParameter("@IsSMSSent", IsSMSSent));

        return wt.RunInsDelUpd(
        @"UPDATE LabResults
          SET Result=@Result,
              NormalRange=@NormalRange,
              ResultDate=@ResultDate,
              IsSMSSent=@IsSMSSent
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM LabResults WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}