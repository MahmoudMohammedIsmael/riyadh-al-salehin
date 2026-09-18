using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class COperationRooms : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string RoomName { get; set; }
    public string Type { get; set; }
    public decimal DailyPrice { get; set; }
    public bool IsAvailable { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        "SELECT * FROM OperationRooms ORDER BY RoomName");
    }

    public DataTable GetAvailableRooms()
    {
        return wt.RunSelect(
        "SELECT * FROM OperationRooms WHERE IsAvailable=1");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        "SELECT * FROM OperationRooms WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@RoomName", RoomName));
        prm.Add(new SqlParameter("@Type", Type));
        prm.Add(new SqlParameter("@DailyPrice", DailyPrice));
        prm.Add(new SqlParameter("@IsAvailable", IsAvailable));

        return wt.RunInsDelUpd(
        @"INSERT INTO OperationRooms
        (RoomName,Type,DailyPrice,IsAvailable)
        VALUES
        (@RoomName,@Type,@DailyPrice,@IsAvailable)", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@RoomName", RoomName));
        prm.Add(new SqlParameter("@Type", Type));
        prm.Add(new SqlParameter("@DailyPrice", DailyPrice));
        prm.Add(new SqlParameter("@IsAvailable", IsAvailable));

        return wt.RunInsDelUpd(
        @"UPDATE OperationRooms
        SET RoomName=@RoomName,
            Type=@Type,
            DailyPrice=@DailyPrice,
            IsAvailable=@IsAvailable
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM OperationRooms WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}