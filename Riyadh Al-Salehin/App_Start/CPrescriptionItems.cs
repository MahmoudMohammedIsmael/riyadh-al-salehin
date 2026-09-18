using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPrescriptionItems : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public string MedicineName { get; set; }
    public string Dosage { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT pi.*,p.PatientId
          FROM PrescriptionItems pi
          INNER JOIN Prescriptions p
          ON pi.PrescriptionId=p.Id");
    }

    public DataTable GetByPrescription(int prescriptionId)
    {
        return wt.RunSelect(
        @"SELECT *
          FROM PrescriptionItems
          WHERE PrescriptionId=" + prescriptionId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@PrescriptionId", PrescriptionId));
        prm.Add(new SqlParameter("@MedicineName", MedicineName));
        prm.Add(new SqlParameter("@Dosage", Dosage));
        prm.Add(new SqlParameter("@Duration", Duration));
        prm.Add(new SqlParameter("@Instructions", Instructions));

        return wt.RunInsDelUpd(
        @"INSERT INTO PrescriptionItems
        (PrescriptionId,MedicineName,Dosage,Duration,Instructions)
        VALUES
        (@PrescriptionId,@MedicineName,@Dosage,@Duration,@Instructions)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@MedicineName", MedicineName));
        prm.Add(new SqlParameter("@Dosage", Dosage));
        prm.Add(new SqlParameter("@Duration", Duration));
        prm.Add(new SqlParameter("@Instructions", Instructions));

        return wt.RunInsDelUpd(
        @"UPDATE PrescriptionItems
          SET MedicineName=@MedicineName,
              Dosage=@Dosage,
              Duration=@Duration,
              Instructions=@Instructions
          WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM PrescriptionItems WHERE Id=" + id);
    }

    public string Add() { return Insert(); }

    public string Delete() { return Delete(Id); }

    public DataTable Search() { return GetAll(); }
}