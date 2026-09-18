using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CPermissions : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string Module { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM Permissions ORDER BY Module, Action");
    }

    public DataTable GetByModule(string module)
    {
        List<SqlParameter> parameters = new List<SqlParameter>();

        parameters.Add(new SqlParameter("@Module", module));

        return wt.RunSelect(
            "SELECT * FROM Permissions WHERE Module=@Module",
            parameters);
    }
    public string Insert()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Module", Module),
            new SqlParameter("@Action", Action),
            new SqlParameter("@Description", (object)Description ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(
            "INSERT INTO Permissions (Module, Action, Description) VALUES (@Module, @Action, @Description)",
            parameters);
    }

    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@Module", Module),
            new SqlParameter("@Action", Action),
            new SqlParameter("@Description", (object)Description ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(
            "UPDATE Permissions SET Module=@Module, Action=@Action, Description=@Description WHERE Id=@Id",
            parameters);
    }
    public static bool Check(string module, string action)
    {
        if (System.Web.HttpContext.Current.Session["RoleId"] == null)
            return false;

        int roleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]);

        WorkTable wt = new WorkTable();

        string sql = @"
SELECT *
FROM RolePermissions rp
INNER JOIN Permissions p
ON rp.PermissionId = p.Id
WHERE rp.RoleId = " + roleId + @"
AND p.Module = '" + module + @"'
AND p.Action = '" + action + "'";

        DataTable dt = wt.RunSelect(sql);

        return dt.Rows.Count > 0;
    }

    public static bool HasPermission(string module, string action)
    {
        if (System.Web.HttpContext.Current.Session["RoleId"] == null)
            return false;

        int roleId =
            Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]);

        WorkTable wt = new WorkTable();

        DataTable dt =
            wt.RunSelect(@"
SELECT *
FROM RolePermissions rp
INNER JOIN Permissions p
ON rp.PermissionId=p.Id
WHERE
rp.RoleId=" + roleId +
" AND p.Module='" + module +
"' AND p.Action='" + action + "'");

        return dt.Rows.Count > 0;
    }
    public DataTable GetById(int id)
    {
        List<SqlParameter> parameters = new List<SqlParameter>();

        parameters.Add(new SqlParameter("@Id", id));

        return wt.RunSelect(
            "SELECT * FROM Permissions WHERE Id=@Id",
            parameters);
    }


    public string Delete(int id)
    {
        List<SqlParameter> parameters = new List<SqlParameter>();

        parameters.Add(new SqlParameter("@Id", id));

        return wt.RunInsDelUpd(
            "DELETE FROM Permissions WHERE Id=@Id",
            parameters);
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