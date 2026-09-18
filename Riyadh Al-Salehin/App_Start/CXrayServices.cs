using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CXrayServices : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string XrayName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM XrayServices");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect("SELECT * FROM XrayServices WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@XrayName", XrayName));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? (object)DBNull.Value : Description));
        prm.Add(new SqlParameter("@IsActive", IsActive));

        return wt.RunInsDelUpd(
        @"INSERT INTO XrayServices (XrayName, Price, Description, IsActive)
          VALUES (@XrayName, @Price, @Description, @IsActive)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@XrayName", XrayName));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? (object)DBNull.Value : Description));
        prm.Add(new SqlParameter("@IsActive", IsActive));

        return wt.RunInsDelUpd(
        @"UPDATE XrayServices 
          SET XrayName = @XrayName, 
              Price = @Price, 
              Description = @Description, 
              IsActive = @IsActive
          WHERE Id = @Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd("DELETE FROM XrayServices WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}