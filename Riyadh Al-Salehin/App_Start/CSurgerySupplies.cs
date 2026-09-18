using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CSurgerySupplies : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int SurgeryId { get; set; }
    public int SupplyId { get; set; }

    public int UsedQuantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT ss.*,
               ms.SupplyName,
               s.SurgeryName
        FROM SurgerySupplies ss
        INNER JOIN MedicalSupplies ms ON ss.SupplyId=ms.Id
        INNER JOIN Surgeries s ON ss.SurgeryId=s.Id");
    }

    public DataTable GetBySurgery(int surgeryId)
    {
        return wt.RunSelect(@"
        SELECT ss.*, ms.SupplyName
        FROM SurgerySupplies ss
        INNER JOIN MedicalSupplies ms ON ss.SupplyId = ms.Id
        WHERE ss.SurgeryId=" + surgeryId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@SurgeryId", SurgeryId));
        prm.Add(new SqlParameter("@SupplyId", SupplyId));
        prm.Add(new SqlParameter("@UsedQuantity", UsedQuantity));
        prm.Add(new SqlParameter("@UnitPrice", UnitPrice));
        prm.Add(new SqlParameter("@TotalPrice", TotalPrice));

        return wt.RunInsDelUpd(@"
        INSERT INTO SurgerySupplies
        (
            SurgeryId,
            SupplyId,
            UsedQuantity,
            UnitPrice,
            TotalPrice
        )
        VALUES
        (
            @SurgeryId,
            @SupplyId,
            @UsedQuantity,
            @UnitPrice,
            @TotalPrice
        )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@UsedQuantity", UsedQuantity));
        prm.Add(new SqlParameter("@UnitPrice", UnitPrice));
        prm.Add(new SqlParameter("@TotalPrice", TotalPrice));

        return wt.RunInsDelUpd(@"
        UPDATE SurgerySupplies
        SET UsedQuantity=@UsedQuantity,
            UnitPrice=@UnitPrice,
            TotalPrice=@TotalPrice
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM SurgerySupplies WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}