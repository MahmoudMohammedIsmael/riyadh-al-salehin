using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class COperationLogs : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; }
    public string AffectedModule { get; set; }
    public int RecordId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public string IPAddress { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT ol.*,
               u.FullName
        FROM OperationLogs ol
        INNER JOIN Users u
            ON ol.UserId=u.Id
        ORDER BY ol.LogDate DESC");
    }

    public DataTable GetByUser(int userId)
    {
        return wt.RunSelect(
        "SELECT * FROM OperationLogs WHERE UserId=" + userId);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@UserId", UserId));
        prm.Add(new SqlParameter("@Action", Action));
        prm.Add(new SqlParameter("@AffectedModule", AffectedModule));
        prm.Add(new SqlParameter("@RecordId", RecordId));
        prm.Add(new SqlParameter("@OldValue", OldValue));
        prm.Add(new SqlParameter("@NewValue", NewValue));
        prm.Add(new SqlParameter("@IPAddress", IPAddress));

        return wt.RunInsDelUpd(@"
        INSERT INTO OperationLogs
        (
            UserId,
            Action,
            AffectedModule,
            RecordId,
            OldValue,
            NewValue,
            IPAddress
        )
        VALUES
        (
            @UserId,
            @Action,
            @AffectedModule,
            @RecordId,
            @OldValue,
            @NewValue,
            @IPAddress
        )", prm);
    }

    public DataTable GetAllLogs()
    {
        DataTable dt = wt.RunSelect(@"
        SELECT Id, RoomName 
        FROM OperationRooms");

        if (dt == null)
            dt = new DataTable();

        return dt;
    }










    public string Update()
    {
        return "Operation Logs Cannot Be Updated";
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
        "DELETE FROM OperationLogs WHERE Id=" + id);
    }

    public string Add() { return Insert(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}