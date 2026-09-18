using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CXrayResults : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int RequestId { get; set; }
    public string ResultText { get; set; }
    public DateTime? ReportDate { get; set; }
    public int? TechnicianId { get; set; }
    public string FilePath { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM XrayResults");
    }

    public DataTable GetByRequestId(int requestId)
    {
        return wt.RunSelect("SELECT * FROM XrayResults WHERE RequestId=" + requestId);
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect("SELECT * FROM XrayResults WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@RequestId", RequestId));
        prm.Add(new SqlParameter("@ResultText", string.IsNullOrEmpty(ResultText) ? (object)DBNull.Value : ResultText));
        prm.Add(new SqlParameter("@TechnicianId", TechnicianId.HasValue ? (object)TechnicianId.Value : DBNull.Value));
        prm.Add(new SqlParameter("@FilePath", string.IsNullOrEmpty(FilePath) ? (object)DBNull.Value : FilePath));

        return wt.RunInsDelUpd(
        @"INSERT INTO XrayResults (RequestId, ResultText, TechnicianId, FilePath)
          VALUES (@RequestId, @ResultText, @TechnicianId, @FilePath)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@ResultText", ResultText));
        prm.Add(new SqlParameter("@FilePath", string.IsNullOrEmpty(FilePath) ? (object)DBNull.Value : FilePath));

        return wt.RunInsDelUpd(
        @"UPDATE XrayResults 
          SET ResultText = @ResultText, 
              FilePath = @FilePath
          WHERE Id = @Id", prm);
    }

    public string Delete(int id) => wt.RunInsDelUpd("DELETE FROM XrayResults WHERE Id=" + id);
    public string Add() => Insert();
    public string Delete() => Delete(Id);
    public DataTable Search() => GetAll();
}