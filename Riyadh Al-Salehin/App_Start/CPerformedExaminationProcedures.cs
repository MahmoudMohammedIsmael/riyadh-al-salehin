using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPerformedExaminationProcedures : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }

    public int ExaminationId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public string Notes { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT pep.*,
               me.InitialDiagnosis,
               sp.ServiceName
        FROM PerformedExaminationProcedures pep
        INNER JOIN MedicalExaminations me
            ON pep.ExaminationId = me.Id
        INNER JOIN ServicesAndPrices sp
            ON pep.ServiceId = sp.Id");
    }

    public DataTable GetByExamination(int examinationId)
    {
        return wt.RunSelect(@"
        SELECT *
        FROM PerformedExaminationProcedures
        WHERE ExaminationId=" + examinationId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@ExaminationId", ExaminationId));
        prm.Add(new SqlParameter("@ServiceId", ServiceId));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(@"
        INSERT INTO PerformedExaminationProcedures
        (
            ExaminationId,
            ServiceId,
            Price,
            Notes
        )
        VALUES
        (
            @ExaminationId,
            @ServiceId,
            @Price,
            @Notes
        )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(@"
        UPDATE PerformedExaminationProcedures
        SET Price=@Price,
            Notes=@Notes
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
            "DELETE FROM PerformedExaminationProcedures WHERE Id=" + id);
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