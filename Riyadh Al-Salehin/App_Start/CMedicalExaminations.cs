using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CMedicalExaminations : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string InitialDiagnosis { get; set; }
    public string DecisionType { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT me.*,
                 p.PatientName,
                 d.DoctorName
          FROM MedicalExaminations me
          INNER JOIN Appointments a ON me.AppointmentId=a.Id
          INNER JOIN Patients p ON a.PatientId=p.Id
          INNER JOIN Doctors d ON a.DoctorId=d.Id");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        "SELECT * FROM MedicalExaminations WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@AppointmentId", AppointmentId));
        prm.Add(new SqlParameter("@InitialDiagnosis", InitialDiagnosis));
        prm.Add(new SqlParameter("@DecisionType", DecisionType));
        prm.Add(new SqlParameter("@Status", Status));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"INSERT INTO MedicalExaminations
        (AppointmentId,InitialDiagnosis,DecisionType,Status,Notes)
        VALUES
        (@AppointmentId,@InitialDiagnosis,@DecisionType,@Status,@Notes)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@InitialDiagnosis", InitialDiagnosis));
        prm.Add(new SqlParameter("@DecisionType", DecisionType));
        prm.Add(new SqlParameter("@Status", Status));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"UPDATE MedicalExaminations
          SET InitialDiagnosis=@InitialDiagnosis,
              DecisionType=@DecisionType,
              Status=@Status,
              Notes=@Notes
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM MedicalExaminations WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}