using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CRoles : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM Roles ORDER BY Id");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect($"SELECT * FROM Roles WHERE Id = {id}");
    }

    public string Insert()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Name", Name),
            new SqlParameter("@Description", (object)Description ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(
            "INSERT INTO Roles (Name, Description) VALUES (@Name, @Description)",
            parameters);
    }

    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@Name", Name),
            new SqlParameter("@Description", (object)Description ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(
            "UPDATE Roles SET Name=@Name, Description=@Description WHERE Id=@Id",
            parameters);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM Roles WHERE Id = {id}");
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