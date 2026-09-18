using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CServicesAndPrices : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string Category { get; set; }

    public decimal BasePrice { get; set; }
    public decimal CashRate { get; set; }
    public decimal InsuranceRate { get; set; }

    public decimal DoctorRate { get; set; }   // نسبة الطبيب %
    public decimal CenterRate { get; set; }   // نسبة المركز %

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT Id, ServiceName, Category, BasePrice, CashRate, InsuranceRate,
                 ISNULL(DoctorRate, 0) AS DoctorRate,
                 ISNULL(CenterRate, 0) AS CenterRate,
                 BasePrice * (ISNULL(DoctorRate, 0) / 100.0) AS DoctorShare,
                 BasePrice * (ISNULL(CenterRate, 0) / 100.0) AS CenterShare
          FROM ServicesAndPrices 
          ORDER BY ServiceName");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        @"SELECT Id, ServiceName, Category, BasePrice, CashRate, InsuranceRate,
                 ISNULL(DoctorRate, 0) AS DoctorRate,
                 ISNULL(CenterRate, 0) AS CenterRate,
                 BasePrice * (ISNULL(DoctorRate, 0) / 100.0) AS DoctorShare,
                 BasePrice * (ISNULL(CenterRate, 0) / 100.0) AS CenterShare
          FROM ServicesAndPrices 
          WHERE Id = " + id);
    }

    public DataTable GetByCategory(string category)
    {
        return wt.RunSelect(
        @"SELECT Id, ServiceName, Category, BasePrice, CashRate, InsuranceRate,
                 ISNULL(DoctorRate, 0) AS DoctorRate,
                 ISNULL(CenterRate, 0) AS CenterRate,
                 BasePrice * (ISNULL(DoctorRate, 0) / 100.0) AS DoctorShare,
                 BasePrice * (ISNULL(CenterRate, 0) / 100.0) AS CenterShare
          FROM ServicesAndPrices 
          WHERE Category = N'" + category + "'");
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@ServiceName", ServiceName));
        prm.Add(new SqlParameter("@Category", Category));
        prm.Add(new SqlParameter("@BasePrice", BasePrice));
        prm.Add(new SqlParameter("@CashRate", CashRate));
        prm.Add(new SqlParameter("@InsuranceRate", InsuranceRate));
        prm.Add(new SqlParameter("@DoctorRate", DoctorRate));   // موجود
        prm.Add(new SqlParameter("@CenterRate", CenterRate));   // موجود
        return wt.RunInsDelUpd(
        @"INSERT INTO ServicesAndPrices
    (ServiceName, Category, BasePrice, CashRate, InsuranceRate, DoctorRate, CenterRate)
    VALUES
    (@ServiceName, @Category, @BasePrice, @CashRate, @InsuranceRate, @DoctorRate, @CenterRate)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@ServiceName", ServiceName));
        prm.Add(new SqlParameter("@Category", Category));
        prm.Add(new SqlParameter("@BasePrice", BasePrice));
        prm.Add(new SqlParameter("@CashRate", CashRate));
        prm.Add(new SqlParameter("@InsuranceRate", InsuranceRate));
        prm.Add(new SqlParameter("@DoctorRate", DoctorRate));
        prm.Add(new SqlParameter("@CenterRate", CenterRate));

        return wt.RunInsDelUpd(
        @"UPDATE ServicesAndPrices
          SET ServiceName = @ServiceName,
              Category = @Category,
              BasePrice = @BasePrice,
              CashRate = @CashRate,
              InsuranceRate = @InsuranceRate,
              DoctorRate = @DoctorRate,
              CenterRate = @CenterRate
          WHERE Id = @Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM ServicesAndPrices WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}