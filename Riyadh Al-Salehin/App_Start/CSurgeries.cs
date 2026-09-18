using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CSurgeries : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int CenterId { get; set; }
    public int RoomId { get; set; }
    public int CreatedBy { get; set; }

    public decimal DoctorCost { get; set; }

    public decimal RoomCost { get; set; }


    public string SurgeryType { get; set; }
    public string SurgeryName { get; set; }

    public DateTime SurgeryDate { get; set; }

    public int DurationInHours { get; set; }

    public string Status { get; set; }
    public string Notes { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT s.*,
               p.PatientName,
               d.DoctorName,
               r.RoomName
        FROM Surgeries s
        INNER JOIN Patients p ON s.PatientId = p.Id
        INNER JOIN Doctors d ON s.DoctorId = d.Id
        INNER JOIN OperationRooms r ON s.RoomId = r.Id
        ORDER BY s.SurgeryDate DESC");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
            "SELECT * FROM Surgeries WHERE Id=@Id",
            new List<SqlParameter> { new SqlParameter("@Id", id) }
        );
    }

    public int Insert()
    {
        try
        {
            List<SqlParameter> prm = new List<SqlParameter>();

            prm.Add(new SqlParameter("@PatientId", PatientId));
            prm.Add(new SqlParameter("@DoctorId", DoctorId));
            prm.Add(new SqlParameter("@CenterId", CenterId));
            prm.Add(new SqlParameter("@RoomId", RoomId));
            prm.Add(new SqlParameter("@CreatedBy", CreatedBy));

            prm.Add(new SqlParameter("@DoctorCost", DoctorCost));

            prm.Add(new SqlParameter("@RoomCost", RoomCost));


            prm.Add(new SqlParameter("@SurgeryType", (object)SurgeryType ?? DBNull.Value));
            prm.Add(new SqlParameter("@SurgeryName", (object)SurgeryName ?? DBNull.Value));

            prm.Add(new SqlParameter("@SurgeryDate",
                SurgeryDate == DateTime.MinValue ? DateTime.Now : SurgeryDate));

            prm.Add(new SqlParameter("@DurationInHours", DurationInHours));
            prm.Add(new SqlParameter("@Status", (object)Status ?? DBNull.Value));
            prm.Add(new SqlParameter("@Notes", (object)Notes ?? DBNull.Value));

            string sql = @"
INSERT INTO Surgeries
(
PatientId,
DoctorId,
CenterId,
RoomId,
CreatedBy,

DoctorCost,
RoomCost,

SurgeryType,
SurgeryName,
SurgeryDate,
DurationInHours,
Status,
Notes
)
VALUES
(
@PatientId,
@DoctorId,
@CenterId,
@RoomId,
@CreatedBy,

@DoctorCost,
@RoomCost,

@SurgeryType,
@SurgeryName,
@SurgeryDate,
@DurationInHours,
@Status,
@Notes
);

SELECT SCOPE_IDENTITY();";

            object result = wt.RunScalar(sql, prm);

            if (result == null)
                throw new Exception("Insert failed: SCOPE_IDENTITY is NULL");

            return Convert.ToInt32(Convert.ToDecimal(result));
        }
        catch (Exception ex)
        {
            throw new Exception("INSERT ERROR: " + ex.Message);
        }
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Status", Status ?? ""));
        prm.Add(new SqlParameter("@Notes", Notes ?? ""));
        prm.Add(new SqlParameter("@RoomId", RoomId));

        return wt.RunInsDelUpd(@"
        UPDATE Surgeries
        SET 
            Status = @Status,
            Notes = @Notes,
            RoomId = @RoomId
        WHERE Id = @Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
            "DELETE FROM Surgeries WHERE Id=@Id",
            new List<SqlParameter> { new SqlParameter("@Id", id) }
        );
    }

    public string Add() { return Insert().ToString(); }
    public string Delete() { return Delete(Id); }
    public DataTable Search() { return GetAll(); }
}