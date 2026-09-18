using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CAppointments : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int CenterId { get; set; }
    public int CreatedBy { get; set; }
    public int QueueNumber { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int ScheduleId { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
            SELECT a.*, p.PatientName, d.DoctorName, d.DoctorCode,
                   ISNULL(d.DoctorCode, 'A') + CAST(a.QueueNumber AS NVARCHAR(20)) AS FormattedQueueNumber,
                   mc.CenterName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN MedicalCenters mc ON a.CenterId = mc.Id
            ORDER BY a.AppointmentDate DESC");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect($@"
            SELECT a.*, p.PatientName, d.DoctorName, d.DoctorCode,
                   ISNULL(d.DoctorCode, 'A') + CAST(a.QueueNumber AS NVARCHAR(20)) AS FormattedQueueNumber,
                   mc.CenterName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN MedicalCenters mc ON a.CenterId = mc.Id
            WHERE a.Id = {id}");
    }

    public DataTable GetByDate(DateTime date)
    {
        return wt.RunSelect($@"
            SELECT a.*, p.PatientName, d.DoctorName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            WHERE CAST(a.AppointmentDate AS DATE) = '{date:yyyy-MM-dd}'
            ORDER BY a.AppointmentDate");
    }

    public DataTable GetByPatient(int patientId)
    {
        return wt.RunSelect($@"
            SELECT a.*, d.DoctorName, mc.CenterName
            FROM Appointments a
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN MedicalCenters mc ON a.CenterId = mc.Id
            WHERE a.PatientId = {patientId}
            ORDER BY a.AppointmentDate DESC");
    }

    public DataTable GetByDoctor(int doctorId)
    {
        return wt.RunSelect($@"
            SELECT a.*, p.PatientName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            WHERE a.DoctorId = {doctorId}
            ORDER BY a.AppointmentDate DESC");
    }

    public DataTable GetByStatus(string status)
    {
        return wt.RunSelect($@"
            SELECT a.*, p.PatientName, d.DoctorName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.Id
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            WHERE a.Status = '{status}'
            ORDER BY a.AppointmentDate DESC");
    }

    public string Insert()
    {
        if (QueueNumber <= 0 && DoctorId > 0)
        {
            QueueNumber = GetNextQueueNumber(DoctorId, AppointmentDate != DateTime.MinValue ? AppointmentDate : DateTime.Now);
        }

        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@PatientId", PatientId),
            new SqlParameter("@ScheduleId", ScheduleId),
            new SqlParameter("@DoctorId", DoctorId),
            new SqlParameter("@CenterId", CenterId),
            new SqlParameter("@CreatedBy", CreatedBy),
            new SqlParameter("@AppointmentDate", AppointmentDate != DateTime.MinValue ? AppointmentDate : DateTime.Now),
            new SqlParameter("@Status", Status ?? "pending"),
            new SqlParameter("@QueueNumber", QueueNumber),
            new SqlParameter("@Notes", (object)Notes ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            INSERT INTO Appointments 
(PatientId, DoctorId, ScheduleId, CenterId, CreatedBy, QueueNumber, AppointmentDate, Status, Notes)
VALUES 
(@PatientId, @DoctorId, @ScheduleId, @CenterId, @CreatedBy, @QueueNumber, @AppointmentDate, @Status, @Notes)",
            parameters);
    }


    public DataTable GetFiltered(string search = null, string status = null, DateTime? from = null, DateTime? to = null)
    {
        string sql = @"
        SELECT 
            a.*, 
            p.PatientName, 
            d.DoctorName, 
            d.DoctorCode,
            ISNULL(d.DoctorCode, 'A') + CAST(a.QueueNumber AS NVARCHAR(20)) AS FormattedQueueNumber,
            mc.CenterName
        FROM Appointments a
        INNER JOIN Patients p ON a.PatientId = p.Id
        INNER JOIN Doctors d ON a.DoctorId = d.Id
        INNER JOIN MedicalCenters mc ON a.CenterId = mc.Id
        WHERE 1=1";

        if (!string.IsNullOrEmpty(search))
            sql += $" AND (p.PatientName LIKE N'%{search}%' OR d.DoctorName LIKE N'%{search}%' OR CAST(a.Id AS NVARCHAR) LIKE '%{search}%')";

        if (!string.IsNullOrEmpty(status))
            sql += $" AND a.Status = '{status}'";

        if (from.HasValue)
            sql += $" AND a.AppointmentDate >= '{from.Value:yyyy-MM-dd}'";

        if (to.HasValue)
            sql += $" AND a.AppointmentDate <= '{to.Value:yyyy-MM-dd}'";

        sql += " ORDER BY a.AppointmentDate DESC";

        return wt.RunSelect(sql);
    }


    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@PatientId", PatientId),
            new SqlParameter("@DoctorId", DoctorId),
            new SqlParameter("@CenterId", CenterId),
            new SqlParameter("@AppointmentDate", AppointmentDate),
            new SqlParameter("@Status", Status),
            new SqlParameter("@Notes", (object)Notes ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            UPDATE Appointments SET PatientId=@PatientId, DoctorId=@DoctorId, CenterId=@CenterId,
            AppointmentDate=@AppointmentDate, Status=@Status, Notes=@Notes WHERE Id=@Id",
            parameters);
    }

    public string UpdateStatus(int id, string status)
    {
        return wt.RunInsDelUpd($"UPDATE Appointments SET Status='{status}' WHERE Id={id}");
    }

    public string Delete(int id)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", id)
        };
        return wt.RunInsDelUpd("DELETE FROM Appointments WHERE Id = @Id", parameters);
    }

    public DataTable DeleteMultiple(List<int> ids)
    {
        DataTable emptyResult = new DataTable();
        emptyResult.Columns.Add("Id", typeof(int));
        emptyResult.Columns.Add("Success", typeof(bool));
        emptyResult.Columns.Add("Reason", typeof(string));

        if (ids == null || ids.Count == 0)
        {
            return emptyResult;
        }

        var parameters = new List<SqlParameter>();
        var valuesList = new List<string>();

        for (int i = 0; i < ids.Count; i++)
        {
            string paramName = "@Id" + i;
            parameters.Add(new SqlParameter(paramName, ids[i]));
            valuesList.Add("(" + paramName + ")");
        }

        string valuesSql = string.Join(", ", valuesList);

        string sql = @"
DECLARE @IdsToProcess TABLE (Id INT);
INSERT INTO @IdsToProcess (Id) VALUES " + valuesSql + @";

DECLARE @Results TABLE (Id INT, Success BIT, Reason NVARCHAR(200));
DECLARE @CurId INT;
DECLARE @CurStatus NVARCHAR(50);

DECLARE curIds CURSOR LOCAL FAST_FORWARD FOR SELECT Id FROM @IdsToProcess;
OPEN curIds;
FETCH NEXT FROM curIds INTO @CurId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @CurStatus = NULL;
    SELECT @CurStatus = Status FROM Appointments WHERE Id = @CurId;

    IF @CurStatus IS NULL
    BEGIN
        INSERT INTO @Results (Id, Success, Reason) VALUES (@CurId, 0, N'الموعد غير موجود');
    END
    ELSE IF LOWER(LTRIM(RTRIM(@CurStatus))) IN ('inexamination', 'finished')
    BEGIN
        INSERT INTO @Results (Id, Success, Reason) VALUES (@CurId, 0, N'لا يمكن حذف موعد بهذه الحالة: ' + @CurStatus);
    END
    ELSE
    BEGIN
        BEGIN TRY
            DELETE FROM Appointments WHERE Id = @CurId;
            INSERT INTO @Results (Id, Success, Reason) VALUES (@CurId, 1, N'تم الحذف');
        END TRY
        BEGIN CATCH
            INSERT INTO @Results (Id, Success, Reason) VALUES (@CurId, 0, N'الموعد مرتبط ببيانات أخرى ولا يمكن حذفه');
        END CATCH
    END

    FETCH NEXT FROM curIds INTO @CurId;
END

CLOSE curIds;
DEALLOCATE curIds;

SELECT Id, Success, Reason FROM @Results;";

        DataTable dt = wt.RunSelect(sql, parameters);
        return dt ?? emptyResult;
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

    public string BookAppointment(int patientId, int doctorId, int centerId, int userId, DateTime date)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        CenterId = centerId;
        CreatedBy = userId;
        AppointmentDate = date;
        Status = "Pending";

        return Insert();
    }


    public DataTable GetAppointmentDetails(int id)
    {
        return wt.RunSelect(@"
    SELECT
        a.*,
        p.PatientName,
        d.DoctorName,
        mc.CenterName
    FROM Appointments a
    INNER JOIN Patients p
        ON a.PatientId = p.Id
    INNER JOIN Doctors d
        ON a.DoctorId = d.Id
    INNER JOIN MedicalCenters mc
        ON a.CenterId = mc.Id
    WHERE a.Id = " + id);
    }



    public int InsertAndReturnId()
    {
        List<SqlParameter> parameters = new List<SqlParameter>();

        parameters.Add(new SqlParameter("@PatientId", PatientId));
        parameters.Add(new SqlParameter("@DoctorId", DoctorId));
        parameters.Add(new SqlParameter("@ScheduleId", ScheduleId));
        parameters.Add(new SqlParameter("@CenterId", CenterId));
        parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
        parameters.Add(new SqlParameter("@AppointmentDate", AppointmentDate != DateTime.MinValue ? AppointmentDate : DateTime.Now));
        parameters.Add(new SqlParameter("@Status", Status ?? "pending"));
        parameters.Add(new SqlParameter("@Notes", (object)Notes ?? DBNull.Value));

        DataTable dt = wt.RunSelect(@"
DECLARE @NextQueue INT;

SELECT @NextQueue = ISNULL(MAX(QueueNumber), 0) + 1
FROM Appointments
WHERE DoctorId = @DoctorId
  AND CAST(AppointmentDate AS DATE) = CAST(@AppointmentDate AS DATE)
  AND Status <> 'cancelled';

INSERT INTO Appointments
(
    PatientId, DoctorId, ScheduleId, CenterId, CreatedBy,
    QueueNumber, AppointmentDate, Status, Notes
)
VALUES
(
    @PatientId, @DoctorId, @ScheduleId, @CenterId, @CreatedBy,
    @NextQueue, @AppointmentDate, @Status, @Notes
);

SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id, @NextQueue AS QueueNumber;", parameters);

        if (dt == null || dt.Rows.Count == 0)
            return 0;

        QueueNumber = Convert.ToInt32(dt.Rows[0]["QueueNumber"]);
        return Convert.ToInt32(dt.Rows[0]["Id"]);
    }

    public int GetNextQueueNumber(
    int doctorId,
    DateTime date)
    {
        DataTable dt = wt.RunSelect(@"
        SELECT ISNULL(MAX(QueueNumber),0)+1 AS QNo
        FROM Appointments
        WHERE DoctorId=" + doctorId +
            " AND CAST(AppointmentDate AS DATE)='" +
            date.ToString("yyyy-MM-dd") + "'");

        return Convert.ToInt32(dt.Rows[0]["QNo"]);
    }

    public static string GetFormattedQueueNumber(int doctorId, int queueNumber)
    {
        if (queueNumber <= 0) return "";
        string doctorCode = Riyadh_Al_Salehin.CDoctors.GetDoctorCode(doctorId);
        return $"{doctorCode}{queueNumber}";
    }

    public static string GetAppointmentQueueDisplay(int appointmentId)
    {
        if (appointmentId <= 0) return "";
        try
        {
            WorkTable wt = new WorkTable();
            DataTable dt = wt.RunSelect($@"
                SELECT a.DoctorId, a.QueueNumber, d.DoctorCode
                FROM Appointments a
                LEFT JOIN Doctors d ON a.DoctorId = d.Id
                WHERE a.Id = {appointmentId}");

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                int doctorId = row["DoctorId"] != DBNull.Value ? Convert.ToInt32(row["DoctorId"]) : 0;
                int queueNum = row["QueueNumber"] != DBNull.Value ? Convert.ToInt32(row["QueueNumber"]) : 0;
                string code = row["DoctorCode"]?.ToString();
                if (string.IsNullOrWhiteSpace(code))
                    code = Riyadh_Al_Salehin.CDoctors.GetDoctorCode(doctorId);

                if (queueNum > 0)
                    return $"{code.Trim().ToUpper()}{queueNum}";
            }
        }
        catch { }
        return "";
    }
}