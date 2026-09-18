using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CMedicalReferrals : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int ExaminationId { get; set; }
    public string Type { get; set; }
    public string Notes { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT mr.*,me.InitialDiagnosis
          FROM MedicalReferrals mr
          INNER JOIN MedicalExaminations me
          ON mr.ExaminationId=me.Id");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        "SELECT * FROM MedicalReferrals WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@ExaminationId", ExaminationId));
        prm.Add(new SqlParameter("@Type", Type));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"INSERT INTO MedicalReferrals
        (ExaminationId,Type,Notes)
        VALUES
        (@ExaminationId,@Type,@Notes)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Type", Type));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"UPDATE MedicalReferrals
          SET Type=@Type,
              Notes=@Notes
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM MedicalReferrals WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}