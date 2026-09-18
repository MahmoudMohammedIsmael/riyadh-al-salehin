using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CMedicalSupplies : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string SupplyName { get; set; }
    public string Unit { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int InitialQuantity { get; set; }
    public DateTime? CreatedDate { get; set; }

    public void EnsureColumnsExist()
    {
        try
        {
            string sql = @"
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalSupplies' AND COLUMN_NAME = 'InitialQuantity')
BEGIN
    ALTER TABLE MedicalSupplies ADD InitialQuantity INT NULL;
    EXEC('UPDATE MedicalSupplies SET InitialQuantity = AvailableQuantity WHERE InitialQuantity IS NULL');
END;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalSupplies' AND COLUMN_NAME = 'CreatedDate')
BEGIN
    ALTER TABLE MedicalSupplies ADD CreatedDate DATETIME NULL;
    EXEC('UPDATE MedicalSupplies SET CreatedDate = GETDATE() WHERE CreatedDate IS NULL');
END;";
            wt.RunInsDelUpd(sql);
        }
        catch { }
    }

    public DataTable GetAll()
    {
        EnsureColumnsExist();
        return wt.RunSelect(@"
    SELECT
        Id,
        SupplyName,
        Unit,
        Price,
        AvailableQuantity,
        ISNULL(InitialQuantity, AvailableQuantity) AS InitialQuantity,
        ISNULL(CreatedDate, GETDATE()) AS CreatedDate
    FROM MedicalSupplies
    ORDER BY SupplyName");
    }

    public DataTable GetById(int id)
    {
        EnsureColumnsExist();
        return wt.RunSelect(@"
    SELECT 
        Id, SupplyName, Unit, Price, AvailableQuantity,
        ISNULL(InitialQuantity, AvailableQuantity) AS InitialQuantity,
        ISNULL(CreatedDate, GETDATE()) AS CreatedDate
    FROM MedicalSupplies WHERE Id=" + id);
    }

    public string Insert()
    {
        EnsureColumnsExist();
        if (InitialQuantity <= 0 && AvailableQuantity > 0)
        {
            InitialQuantity = AvailableQuantity;
        }
        if (!CreatedDate.HasValue)
        {
            CreatedDate = DateTime.Now;
        }

        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@SupplyName", SupplyName ?? ""));
        prm.Add(new SqlParameter("@Unit", Unit ?? ""));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@AvailableQuantity", AvailableQuantity));
        prm.Add(new SqlParameter("@InitialQuantity", InitialQuantity));
        prm.Add(new SqlParameter("@CreatedDate", CreatedDate.Value));

        return wt.RunInsDelUpd(
        @"INSERT INTO MedicalSupplies
        (SupplyName,Unit,Price,AvailableQuantity,InitialQuantity,CreatedDate)
        VALUES
        (@SupplyName,@Unit,@Price,@AvailableQuantity,@InitialQuantity,@CreatedDate)", prm);
    }

    public string Update()
    {
        EnsureColumnsExist();
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@SupplyName", SupplyName ?? ""));
        prm.Add(new SqlParameter("@Unit", Unit ?? ""));
        prm.Add(new SqlParameter("@Price", Price));
        prm.Add(new SqlParameter("@AvailableQuantity", AvailableQuantity));
        prm.Add(new SqlParameter("@InitialQuantity", InitialQuantity));

        return wt.RunInsDelUpd(
        @"UPDATE MedicalSupplies
        SET SupplyName=@SupplyName,
            Unit=@Unit,
            Price=@Price,
            AvailableQuantity=@AvailableQuantity,
            InitialQuantity=@InitialQuantity
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM MedicalSupplies WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}