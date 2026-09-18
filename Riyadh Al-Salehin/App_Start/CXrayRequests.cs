using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CXrayRequests : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int? ReferralId { get; set; }
    public int PatientId { get; set; }
    public int XrayServiceId { get; set; }
    public string Status { get; set; }
    public DateTime? CreatedAt { get; set; }

    public int? AppointmentId { get; set; }
    public int? DoctorId { get; set; }

    public bool IsBilled { get; set; }

    public string PatientName { get; set; }
    public string XrayName { get; set; }
    public decimal Price { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(
        @"SELECT r.*, p.PatientName, s.XrayName, s.Price
          FROM XrayRequests r
          INNER JOIN Patients p ON r.PatientId = p.Id
          INNER JOIN XrayServices s ON r.XrayServiceId = s.Id");
    }

    public DataTable GetPendingForReception()
    {
        return wt.RunSelect(
        @"SELECT r.*, p.PatientName, s.XrayName, s.Price, ISNULL(r.IsBilled,0) AS IsBilled
          FROM XrayRequests r
          INNER JOIN Patients p ON r.PatientId = p.Id
          INNER JOIN XrayServices s ON r.XrayServiceId = s.Id
          WHERE r.Status = 'Pending' AND (ISNULL(r.IsBilled,0) = 0)");
    }




    public DataTable GetRequestsForTech(string statusFilter = null)
    {
        string sql = @"SELECT r.*, p.PatientName, s.XrayName, s.Price
                   FROM XrayRequests r
                   INNER JOIN Patients p ON r.PatientId = p.Id
                   INNER JOIN XrayServices s ON r.XrayServiceId = s.Id
                   WHERE IsBilled = 1";   // ✅ أضف هذا الشرط

        if (!string.IsNullOrEmpty(statusFilter))
            sql += " AND r.Status = @Status";

        sql += " ORDER BY r.CreatedAt DESC";

        var prm = new List<SqlParameter>();
        if (!string.IsNullOrEmpty(statusFilter))
            prm.Add(new SqlParameter("@Status", statusFilter));

        return wt.RunSelect(sql, prm);
    }


















    public DataTable GetById(int id)
    {
        return wt.RunSelect("SELECT * FROM XrayRequests WHERE Id=" + id);
    }

    public DataTable GetByReferralId(int referralId)
    {
        return wt.RunSelect("SELECT * FROM XrayRequests WHERE ReferralId = " + referralId);
    }

    public string UpdateStatus(int id, string status)
    {
        var prm = new List<SqlParameter>
        {
            new SqlParameter("@Id", id),
            new SqlParameter("@Status", status)
        };
        return wt.RunInsDelUpd("UPDATE XrayRequests SET Status=@Status WHERE Id=@Id", prm);
    }

    public string UpdateBilling(int id, bool isBilled)
    {
        var prm = new List<SqlParameter>
        {
            new SqlParameter("@Id", id),
            new SqlParameter("@IsBilled", isBilled)
        };
        return wt.RunInsDelUpd("UPDATE XrayRequests SET IsBilled=@IsBilled WHERE Id=@Id", prm);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@ReferralId", ReferralId.HasValue ? (object)ReferralId.Value : DBNull.Value));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@XrayServiceId", XrayServiceId));
        prm.Add(new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? "Pending" : Status));
        prm.Add(new SqlParameter("@IsBilled", IsBilled));
        prm.Add(new SqlParameter("@AppointmentId", AppointmentId.HasValue ? (object)AppointmentId.Value : DBNull.Value));
        prm.Add(new SqlParameter("@DoctorId", DoctorId.HasValue ? (object)DoctorId.Value : DBNull.Value));

        return wt.RunInsDelUpd(
        @"INSERT INTO XrayRequests (ReferralId, PatientId, XrayServiceId, Status, IsBilled , AppointmentId , DoctorId )
          VALUES (@ReferralId, @PatientId, @XrayServiceId, @Status, @IsBilled , @AppointmentId , @DoctorId )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();
        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@ReferralId", ReferralId.HasValue ? (object)ReferralId.Value : DBNull.Value));
        prm.Add(new SqlParameter("@PatientId", PatientId));
        prm.Add(new SqlParameter("@XrayServiceId", XrayServiceId));
        prm.Add(new SqlParameter("@Status", Status));
        prm.Add(new SqlParameter("@IsBilled", IsBilled));

        return wt.RunInsDelUpd(
        @"UPDATE XrayRequests 
          SET ReferralId = @ReferralId, 
              PatientId = @PatientId, 
              XrayServiceId = @XrayServiceId, 
              Status = @Status,
              IsBilled = @IsBilled
          WHERE Id = @Id", prm);
    }

    public string Delete(int id) => wt.RunInsDelUpd("DELETE FROM XrayRequests WHERE Id=" + id);
    public string Add() => Insert();
    public string Delete() => Delete(Id);
    public DataTable Search() => GetAll();
}