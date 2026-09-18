using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPatients : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string PatientName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string InsuranceCompany { get; set; }
    public string InsuranceNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM Patients ORDER BY PatientName");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect($"SELECT * FROM Patients WHERE Id = {id}");
    }

    public DataTable Search(string keyword)
    {
        return wt.RunSelect($@"
            SELECT * FROM Patients
            WHERE PatientName LIKE '%{keyword}%' OR Phone LIKE '%{keyword}%'
            ORDER BY PatientName");
    }

    public DataTable GetByInsurance(string company)
    {
        return wt.RunSelect($"SELECT * FROM Patients WHERE InsuranceCompany = '{company}'");
    }

    public int InsertAndReturnId()
    {
        int id = wt.RunInsertAndReturnId(@"
    INSERT INTO Patients
    (
        PatientName,
        Phone,
        Address,
        DateOfBirth,
        InsuranceCompany,
        InsuranceNumber
    )
    VALUES
    (
        @PatientName,
        @Phone,
        @Address,
        @DateOfBirth,
        @InsuranceCompany,
        @InsuranceNumber
    )",
        new List<SqlParameter>
        {
        new SqlParameter("@PatientName", PatientName),
        new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
        new SqlParameter("@Address", (object)Address ?? DBNull.Value),
        new SqlParameter("@DateOfBirth", (object)DateOfBirth ?? DBNull.Value),
        new SqlParameter("@InsuranceCompany", (object)InsuranceCompany ?? DBNull.Value),
        new SqlParameter("@InsuranceNumber", (object)InsuranceNumber ?? DBNull.Value)
        });

        return id;
    }



    public int GetLastPatientId()
    {
        DataTable dt =
            wt.RunSelect("SELECT MAX(Id) AS Id FROM Patients");

        if (dt.Rows.Count > 0)
            return Convert.ToInt32(dt.Rows[0]["Id"]);

        return 0;
    }














    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@PatientName", PatientName),
            new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
            new SqlParameter("@Address", (object)Address ?? DBNull.Value),
            new SqlParameter("@DateOfBirth", (object)DateOfBirth ?? DBNull.Value),
            new SqlParameter("@InsuranceCompany", (object)InsuranceCompany ?? DBNull.Value),
            new SqlParameter("@InsuranceNumber", (object)InsuranceNumber ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            UPDATE Patients SET PatientName=@PatientName, Phone=@Phone, Address=@Address,
            DateOfBirth=@DateOfBirth, InsuranceCompany=@InsuranceCompany, InsuranceNumber=@InsuranceNumber
            WHERE Id=@Id",
            parameters);
    }



   


    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM Patients WHERE Id = {id}");
    }

    public string Add()
    {
        throw new NotImplementedException();
    }

    public string Delete()
    {
        throw new NotImplementedException();
    }

    public DataTable Search()
    {
        throw new NotImplementedException();
    }


   
}