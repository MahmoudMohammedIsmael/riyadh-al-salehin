using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CUsers : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int RoleId { get; set; }
    public int? DoctorId { get; set; }
    public int? CenterId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
            SELECT u.*, r.Name AS RoleName,
                   d.DoctorName, mc.CenterName
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.Id
            LEFT JOIN Doctors d ON u.DoctorId = d.Id
            LEFT JOIN MedicalCenters mc ON u.CenterId = mc.Id
            ORDER BY u.FullName");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect($"SELECT * FROM Users WHERE Id = {id}");
    }

    public DataTable GetByUsername(string keyword)
    {
        string safe = keyword.Replace("'", "''");
        return wt.RunSelect($@"
            SELECT u.*, r.Name AS RoleName, d.DoctorName, mc.CenterName
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.Id
            LEFT JOIN Doctors d ON u.DoctorId = d.Id
            LEFT JOIN MedicalCenters mc ON u.CenterId = mc.Id
            WHERE u.Username LIKE '%{safe}%' OR u.FullName LIKE '%{safe}%'
            ORDER BY u.FullName");
    }


  

    public DataTable GetActive()
    {
        return wt.RunSelect("SELECT * FROM Users WHERE IsActive = 1 ORDER BY FullName");
    }

    public string Insert()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@FullName",     FullName),
            new SqlParameter("@Username",     Username),
            new SqlParameter("@PasswordHash", PasswordHash),
            new SqlParameter("@Email",        (object)Email    ?? DBNull.Value),
            new SqlParameter("@Phone",        (object)Phone    ?? DBNull.Value),
            new SqlParameter("@RoleId",       RoleId),
            new SqlParameter("@DoctorId",     (object)DoctorId ?? DBNull.Value),
            new SqlParameter("@CenterId",     (object)CenterId ?? DBNull.Value),
            new SqlParameter("@IsActive",     IsActive),
            new SqlParameter("@CreatedBy",    (object)CreatedBy ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            INSERT INTO Users (FullName, Username, PasswordHash, Email, Phone, RoleId, DoctorId, CenterId, IsActive, CreatedBy)
            VALUES (@FullName, @Username, @PasswordHash, @Email, @Phone, @RoleId, @DoctorId, @CenterId, @IsActive, @CreatedBy)",
            parameters);
    }

    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id",       Id),
            new SqlParameter("@FullName", FullName),
            new SqlParameter("@Email",    (object)Email    ?? DBNull.Value),
            new SqlParameter("@Phone",    (object)Phone    ?? DBNull.Value),
            new SqlParameter("@RoleId",   RoleId),
            new SqlParameter("@DoctorId", (object)DoctorId ?? DBNull.Value),
            new SqlParameter("@CenterId", (object)CenterId ?? DBNull.Value),
            new SqlParameter("@IsActive", IsActive)
        };
        return wt.RunInsDelUpd(@"
            UPDATE Users
            SET FullName=@FullName, Email=@Email, Phone=@Phone,
                RoleId=@RoleId, DoctorId=@DoctorId, CenterId=@CenterId, IsActive=@IsActive
            WHERE Id=@Id",
            parameters);
    }




    public DataTable Login(string username, string password)
    {
        string safeUser = username.Replace("'", "''");
        string safePass = password.Replace("'", "''");
        return wt.RunSelect($@"
        SELECT u.*, r.Name AS RoleName
        FROM Users u
        LEFT JOIN Roles r ON u.RoleId = r.Id
        WHERE u.Username     = '{safeUser}'
          AND u.PasswordHash = '{safePass}'
          AND u.IsActive     = 1");
    }











    public string UpdatePassword(int id, string newPasswordHash)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id",           id),
            new SqlParameter("@PasswordHash", newPasswordHash)
        };
        return wt.RunInsDelUpd("UPDATE Users SET PasswordHash=@PasswordHash WHERE Id=@Id", parameters);
    }

    public string UpdateLastLogin(int id)
    {
        return wt.RunInsDelUpd($"UPDATE Users SET LastLogin=GETDATE() WHERE Id={id}");
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM Users WHERE Id = {id}");
    }

    public string Add() { throw new NotImplementedException(); }
    public string Delete() { throw new NotImplementedException(); }
    public DataTable Search() { throw new NotImplementedException(); }
}