using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Riyadh_Al_Salehin
{
    public class CDoctorSchedules : WorkTable
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; }

        public DataTable GetAll()
        {
            return RunSelect(@"SELECT
                                MIN(ds.Id) AS Id,
                                ds.DoctorId,
                                d.DoctorName,
                                DATENAME(WEEKDAY, MIN(ds.StartTime)) AS DayName,
                                MIN(ds.StartTime) AS StartTime,
                                MAX(ds.EndTime) AS EndTime,
                                CASE
                                    WHEN MAX(CAST(ds.IsBooked AS INT)) = 1
                                    THEN N'يوجد حجز'
                                    ELSE N'متاح'
                                END AS Status
                            FROM DoctorSchedules ds
                            INNER JOIN Doctors d ON d.Id = ds.DoctorId
                            GROUP BY ds.DoctorId, d.DoctorName, DATEPART(WEEKDAY, ds.StartTime)
                            ORDER BY d.DoctorName, DATEPART(WEEKDAY, MIN(ds.StartTime));");
        }

        public DataTable GetSchedulesByDate(int doctorId, DateTime date)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@Date", date.Date)
            };

            return RunSelect(@"SELECT Id, DoctorId, StartTime, EndTime, IsBooked
                               FROM DoctorSchedules
                               WHERE DoctorId = @DoctorId
                                 AND StartTime >= @Date
                                 AND StartTime < DATEADD(DAY, 1, @Date)
                               ORDER BY StartTime", parameters);
        }

        public DataTable GetUpcomingSchedules(DateTime? fromDate = null, DateTime? toDate = null, int? doctorId = null)
        {
            string sql = @"SELECT 
                            ds.Id, ds.DoctorId, ds.StartTime, ds.EndTime, ds.IsBooked,
                            d.DoctorName, d.Specialty,
                            CAST(ds.StartTime AS DATE) AS [Date]
                           FROM DoctorSchedules ds
                           INNER JOIN Doctors d ON ds.DoctorId = d.Id
                           WHERE ds.StartTime >= GETDATE()";

            var parameters = new List<SqlParameter>();

            if (doctorId.HasValue)
            {
                sql += " AND ds.DoctorId = @DoctorId";
                parameters.Add(new SqlParameter("@DoctorId", doctorId.Value));
            }

            if (fromDate.HasValue)
            {
                sql += " AND ds.StartTime >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
            }

            if (toDate.HasValue)
            {
                sql += " AND ds.StartTime < DATEADD(DAY, 1, @ToDate)";
                parameters.Add(new SqlParameter("@ToDate", toDate.Value));
            }

            sql += " ORDER BY d.DoctorName, ds.StartTime";

            return RunSelect(sql, parameters);
        }

        public bool Exists(int doctorId, DateTime start, DateTime end)
        {
            var p = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@Start", start),
                new SqlParameter("@End", end)
            };
            DataTable dt = RunSelect(@"SELECT COUNT(*) FROM DoctorSchedules
                                       WHERE DoctorId = @DoctorId AND StartTime = @Start AND EndTime = @End", p);
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        public DataTable GetById(int id)
        {
            var p = new List<SqlParameter> { new SqlParameter("@Id", id) };
            return RunSelect("SELECT * FROM DoctorSchedules WHERE Id = @Id", p);
        }

        public string Add()
        {
            List<SqlParameter> p = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", DoctorId),
                new SqlParameter("@StartTime", StartTime),
                new SqlParameter("@EndTime", EndTime),
                new SqlParameter("@IsBooked", IsBooked)
            };

            return RunInsDelUpd(@"INSERT INTO DoctorSchedules (DoctorId, StartTime, EndTime, IsBooked)
                                 VALUES (@DoctorId, @StartTime, @EndTime, @IsBooked)", p);
        }

        public string Update()
        {
            List<SqlParameter> p = new List<SqlParameter>
            {
                new SqlParameter("@Id", Id),
                new SqlParameter("@DoctorId", DoctorId),
                new SqlParameter("@StartTime", StartTime),
                new SqlParameter("@EndTime", EndTime),
                new SqlParameter("@IsBooked", IsBooked)
            };

            return RunInsDelUpd(@"UPDATE DoctorSchedules
                                 SET DoctorId=@DoctorId, StartTime=@StartTime, EndTime=@EndTime, IsBooked=@IsBooked
                                 WHERE Id=@Id", p);
        }

        /// <summary>
        /// حذف موعد واحد (يعيد عدد الصفوف المحذوفة، أو -1 في حال الخطأ)
        /// </summary>
        public int DeleteSchedule(int id)
        {
            var countParams = new List<SqlParameter> { new SqlParameter("@Id", id) };
            string countSql = "SELECT COUNT(*) FROM DoctorSchedules WHERE Id = @Id AND IsBooked = 0 AND StartTime > GETDATE()";
            DataTable dt = RunSelect(countSql, countParams);
            int count = Convert.ToInt32(dt.Rows[0][0]);
            if (count == 0) return 0;

            var deleteParams = new List<SqlParameter> { new SqlParameter("@Id", id) };
            string deleteSql = "DELETE FROM DoctorSchedules WHERE Id = @Id AND IsBooked = 0 AND StartTime > GETDATE()";
            string result = RunInsDelUpd(deleteSql, deleteParams);
            return result == "OK" ? count : -1;
        }

        /// <summary>
        /// حذف مواعيد يوم كامل (يعيد عدد الصفوف المحذوفة، أو -1 في حال الخطأ)
        /// </summary>
        public int DeleteDaySchedules(int doctorId, DateTime date)
        {
            var countParams = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@Start", date.Date),
                new SqlParameter("@End", date.Date.AddDays(1))
            };
            string countSql = @"SELECT COUNT(*) FROM DoctorSchedules 
                                WHERE DoctorId = @DoctorId 
                                  AND StartTime >= @Start 
                                  AND StartTime < @End 
                                  AND IsBooked = 0 
                                  AND StartTime > GETDATE()";
            DataTable dt = RunSelect(countSql, countParams);
            int count = Convert.ToInt32(dt.Rows[0][0]);
            if (count == 0) return 0;

            var deleteParams = new List<SqlParameter>
            {
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@Start", date.Date),
                new SqlParameter("@End", date.Date.AddDays(1))
            };
            string deleteSql = @"DELETE FROM DoctorSchedules 
                                 WHERE DoctorId = @DoctorId 
                                   AND StartTime >= @Start 
                                   AND StartTime < @End 
                                   AND IsBooked = 0 
                                   AND StartTime > GETDATE()";
            string result = RunInsDelUpd(deleteSql, deleteParams);
            return result == "OK" ? count : -1;
        }

        /// <summary>
        /// حذف جميع المواعيد القادمة غير المحجوزة حسب الفلتر (يعيد عدد الصفوف المحذوفة، أو -1 في حال الخطأ)
        /// </summary>
        public int DeleteUpcomingSchedules(int? doctorId, DateTime? fromDate, DateTime? toDate)
        {
            var countParams = new List<SqlParameter>();
            string countSql = "SELECT COUNT(*) FROM DoctorSchedules WHERE IsBooked = 0 AND StartTime > GETDATE()";

            if (doctorId.HasValue)
            {
                countSql += " AND DoctorId = @DoctorId";
                countParams.Add(new SqlParameter("@DoctorId", doctorId.Value));
            }
            if (fromDate.HasValue)
            {
                countSql += " AND StartTime >= @FromDate";
                countParams.Add(new SqlParameter("@FromDate", fromDate.Value.Date));
            }
            if (toDate.HasValue)
            {
                countSql += " AND StartTime < DATEADD(DAY, 1, @ToDate)";
                countParams.Add(new SqlParameter("@ToDate", toDate.Value.Date));
            }

            DataTable dt = RunSelect(countSql, countParams);
            int count = Convert.ToInt32(dt.Rows[0][0]);
            if (count == 0) return 0;

            var deleteParams = new List<SqlParameter>();
            string deleteSql = "DELETE FROM DoctorSchedules WHERE IsBooked = 0 AND StartTime > GETDATE()";

            if (doctorId.HasValue)
            {
                deleteSql += " AND DoctorId = @DoctorId";
                deleteParams.Add(new SqlParameter("@DoctorId", doctorId.Value));
            }
            if (fromDate.HasValue)
            {
                deleteSql += " AND StartTime >= @FromDate";
                deleteParams.Add(new SqlParameter("@FromDate", fromDate.Value.Date));
            }
            if (toDate.HasValue)
            {
                deleteSql += " AND StartTime < DATEADD(DAY, 1, @ToDate)";
                deleteParams.Add(new SqlParameter("@ToDate", toDate.Value.Date));
            }

            string result = RunInsDelUpd(deleteSql, deleteParams);
            return result == "OK" ? count : -1;
        }

        public bool TryBook(int scheduleId)
        {
            List<SqlParameter> p = new List<SqlParameter> { new SqlParameter("@Id", scheduleId) };
            DataTable dt = RunSelect(@"UPDATE DoctorSchedules
                                       SET BookedCount = BookedCount + 1,
                                           IsBooked = CASE WHEN BookedCount + 1 >= Capacity THEN 1 ELSE 0 END
                                       OUTPUT INSERTED.Id
                                       WHERE Id = @Id AND BookedCount < Capacity;", p);
            return dt != null && dt.Rows.Count > 0;
        }

        public DataTable GetAvailableByDoctor(int doctorId)
        {
            List<SqlParameter> p = new List<SqlParameter> { new SqlParameter("@DoctorId", doctorId) };
            return RunSelect(@"SELECT TOP 7 Id, DoctorId, StartTime, EndTime, IsBooked, Capacity, BookedCount
                       FROM DoctorSchedules
                       WHERE DoctorId = @DoctorId AND BookedCount < Capacity AND EndTime >= GETDATE()
                       ORDER BY StartTime;", p);
        }

        public string UnBook(int id)
        {
            return RunInsDelUpd(@"UPDATE DoctorSchedules
                                 SET BookedCount = CASE WHEN BookedCount > 0 THEN BookedCount - 1 ELSE 0 END,
                                     IsBooked = 0
                                 WHERE Id = @Id", new List<SqlParameter> { new SqlParameter("@Id", id) });
        }

        public DataTable GetByDoctor(int doctorId)
        {
            List<SqlParameter> p = new List<SqlParameter> { new SqlParameter("@DoctorId", doctorId) };
            return RunSelect(@"SELECT Id, DoctorId, StartTime, EndTime, N'متاح' AS Status
                               FROM DoctorSchedules
                               WHERE DoctorId = @DoctorId
                                 AND CAST(StartTime AS DATE) = CAST(GETDATE() AS DATE)
                                 AND EndTime > GETDATE()
                               ORDER BY StartTime;", p);
        }

        public DataTable Search(string value)
        {
            return RunSelect(@"SELECT ds.Id, d.DoctorName, ds.StartTime, ds.EndTime, ds.IsBooked
                               FROM DoctorSchedules ds
                               INNER JOIN Doctors d ON ds.DoctorId = d.Id
                               WHERE d.DoctorName LIKE N'%" + value + @"%'
                               ORDER BY ds.StartTime");
        }

        public string Delete(int id) => DeleteSchedule(id).ToString();
        public string Delete() => Delete(Id);
        public string Insert() => Add();
        public DataTable Search() => GetAll();
        public DataTable GetAllSchedules() => GetAll();
        public string Book(int id)
        {
            List<SqlParameter> p = new List<SqlParameter>
    {
        new SqlParameter("@Id", id)
    };

            DataTable dt = RunSelect(@"
        UPDATE DoctorSchedules
        SET 
            BookedCount = ISNULL(BookedCount, 0) + 1,
            IsBooked =
                CASE 
                    WHEN ISNULL(BookedCount, 0) + 1 >= Capacity 
                    THEN 1 
                    ELSE 0 
                END
        OUTPUT INSERTED.Id
        WHERE Id = @Id
          AND ISNULL(BookedCount, 0) < Capacity;
    ", p);

            if (dt != null && dt.Rows.Count > 0)
                return "OK";

            return "FULL";
        }
    }
}