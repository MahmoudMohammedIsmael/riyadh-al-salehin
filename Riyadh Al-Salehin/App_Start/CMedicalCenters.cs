using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CMedicalCenters : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string CenterName { get; set; }
    public string Address { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal? CommissionRate { get; set; }
    public string Phone { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM MedicalCenters ORDER BY CenterName");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect($"SELECT * FROM MedicalCenters WHERE Id = {id}");
    }

    public string Insert()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@CenterName", CenterName),
            new SqlParameter("@Address", (object)Address ?? DBNull.Value),
            new SqlParameter("@CommissionRate", (object)CommissionRate ?? DBNull.Value),
            new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
             new SqlParameter("@ConsultationFee", ConsultationFee)
        };
        return wt.RunInsDelUpd(@"
            INSERT INTO MedicalCenters (CenterName, Address,ConsultationFee , CommissionRate, Phone)
            VALUES (@CenterName, @ConsultationFee ,@Address, @CommissionRate, @Phone)",
            parameters);
    }

    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@CenterName", CenterName),
            new SqlParameter("@Address", (object)Address ?? DBNull.Value),
            new SqlParameter("@CommissionRate", (object)CommissionRate ?? DBNull.Value),
            new SqlParameter("@Phone", (object)Phone ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            UPDATE MedicalCenters SET CenterName=@CenterName, Address=@Address,
            CommissionRate=@CommissionRate, Phone=@Phone WHERE Id=@Id",
            parameters);
    }


    public DataTable Search(string txt)
    {
        return wt.RunSelect(
            "SELECT * FROM MedicalCenters WHERE CenterName LIKE N'%" +
            txt +
            "%'");
    }


   

    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM MedicalCenters WHERE Id = {id}");
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