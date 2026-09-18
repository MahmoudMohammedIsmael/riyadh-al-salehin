using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CRolePermissions : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
            SELECT rp.Id, r.Name AS RoleName, p.Module, p.Action
            FROM RolePermissions rp
            INNER JOIN Roles r ON rp.RoleId = r.Id
            INNER JOIN Permissions p ON rp.PermissionId = p.Id
            ORDER BY r.Name, p.Module");
    }

    public DataTable GetByRole(int roleId)
    {
        return wt.RunSelect($@"
            SELECT rp.Id, p.Module, p.Action, p.Description
            FROM RolePermissions rp
            INNER JOIN Permissions p ON rp.PermissionId = p.Id
            WHERE rp.RoleId = {roleId}");
    }

    public string Insert()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@RoleId", RoleId),
            new SqlParameter("@PermissionId", PermissionId)
        };
        return wt.RunInsDelUpd(
            "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)",
            parameters);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM RolePermissions WHERE Id = {id}");
    }

    public string DeleteByRole(int roleId)
    {
        return wt.RunInsDelUpd($"DELETE FROM RolePermissions WHERE RoleId = {roleId}");
    }

    public string Add()
    {
        throw new NotImplementedException();
    }

    public string Update()
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