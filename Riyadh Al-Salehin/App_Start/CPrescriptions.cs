using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPrescriptions : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int ExaminationId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string FinalDiagnosis { get; set; }
    public string Notes { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT p.*,
                 pt.PatientName,
                 d.DoctorName
          FROM Prescriptions p
          INNER JOIN Patients pt ON p.PatientId=pt.Id
          INNER JOIN Doctors d ON p.DoctorId=d.Id");
    }

    public DataTable GetByPatient(int patientId)
    {
        return wt.RunSelect(
        @"SELECT * FROM Prescriptions
          WHERE PatientId=" + patientId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@ExaminationId", ExaminationId));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@DoctorId", DoctorId));
        prm.Add(new SqlParameter("@FinalDiagnosis", FinalDiagnosis));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"INSERT INTO Prescriptions
        (ExaminationId,PatientId,DoctorId,FinalDiagnosis,Notes)
        VALUES
        (@ExaminationId,@PatientId,@DoctorId,@FinalDiagnosis,@Notes)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@FinalDiagnosis", FinalDiagnosis));
        prm.Add(new SqlParameter("@Notes", Notes));

        return wt.RunInsDelUpd(
        @"UPDATE Prescriptions
          SET FinalDiagnosis=@FinalDiagnosis,
              Notes=@Notes
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM Prescriptions WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}