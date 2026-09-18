using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Riyadh_Al_Salehin
{
    public class CDoctors : WorkTable
    {
        private static bool _schemaEnsured = false;
        private static readonly object _schemaLock = new object();

        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorCode { get; set; }
        public string Specialty { get; set; }
        public decimal ConsultationFee { get; set; }
        public string DoctorType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal? CommissionRate { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        public void EnsureDoctorCodeColumnExists()
        {
            if (_schemaEnsured) return;
            lock (_schemaLock)
            {
                if (_schemaEnsured) return;
                try
                {
                    RunInsDelUpd(@"
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Doctors') AND name = 'DoctorCode'
                        )
                        BEGIN
                            ALTER TABLE Doctors ADD DoctorCode NVARCHAR(10) NULL;
                        END;");

                    RunInsDelUpd(@"
                        IF EXISTS (
                            SELECT 1 FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Doctors') AND name = 'DoctorCode'
                        )
                        BEGIN
                            EXEC sp_executesql N'
                                WITH NumberedDoctors AS (
                                    SELECT Id, DoctorCode, ROW_NUMBER() OVER (ORDER BY Id) AS RowNum
                                    FROM Doctors
                                    WHERE DoctorCode IS NULL OR LTRIM(RTRIM(DoctorCode)) = ''''
                                )
                                UPDATE NumberedDoctors
                                SET DoctorCode = CASE 
                                    WHEN RowNum <= 26 THEN CHAR(64 + RowNum)
                                    ELSE CHAR(64 + ((RowNum - 1) / 26)) + CHAR(65 + ((RowNum - 1) % 26))
                                END;
                            ';
                        END;");

                    _schemaEnsured = true;
                }
                catch
                {
                }
            }
        }

        public static DataTable EnsureDoctorCodeInDataTable(DataTable dt)
        {
            if (dt == null) return null;
            if (!dt.Columns.Contains("DoctorCode"))
            {
                dt.Columns.Add("DoctorCode", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    int id = (row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value) ? Convert.ToInt32(row["Id"]) : 0;
                    row["DoctorCode"] = GetDoctorCode(id);
                }
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["DoctorCode"] == DBNull.Value || string.IsNullOrWhiteSpace(row["DoctorCode"].ToString()))
                    {
                        int id = (row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value) ? Convert.ToInt32(row["Id"]) : 0;
                        row["DoctorCode"] = GetDoctorCode(id);
                    }
                }
            }
            return dt;
        }

        public string GetNextAvailableDoctorCode()
        {
            EnsureDoctorCodeColumnExists();
            try
            {
                DataTable dt = RunSelect("SELECT DISTINCT UPPER(LTRIM(RTRIM(DoctorCode))) AS Code FROM Doctors WHERE DoctorCode IS NOT NULL AND DoctorCode <> ''");
                var usedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string code = row["Code"]?.ToString();
                        if (!string.IsNullOrEmpty(code))
                            usedCodes.Add(code.Trim().ToUpper());
                    }
                }

                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string candidate = c.ToString();
                    if (!usedCodes.Contains(candidate))
                        return candidate;
                }

                for (char c1 = 'A'; c1 <= 'Z'; c1++)
                {
                    for (char c2 = 'A'; c2 <= 'Z'; c2++)
                    {
                        string candidate = $"{c1}{c2}";
                        if (!usedCodes.Contains(candidate))
                            return candidate;
                    }
                }
            }
            catch { }
            return "A";
        }

        public static string GetDoctorCode(int doctorId)
        {
            if (doctorId <= 0) return "A";
            try
            {
                WorkTable wt = new WorkTable();
                DataTable dt = wt.RunSelect($"SELECT DoctorCode FROM Doctors WHERE Id = {doctorId}");
                if (dt != null && dt.Rows.Count > 0)
                {
                    string code = dt.Rows[0]["DoctorCode"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(code))
                        return code.Trim().ToUpper();
                }
            }
            catch { }

            int offset = (doctorId - 1) % 26;
            return ((char)('A' + Math.Max(0, offset))).ToString();
        }

        public DataTable GetAll()
        {
            EnsureDoctorCodeColumnExists();
            return EnsureDoctorCodeInDataTable(RunSelect("SELECT * FROM Doctors ORDER BY DoctorName"));
        }

        public DataTable GetById(int id)
        {
            EnsureDoctorCodeColumnExists();
            var p = new List<SqlParameter> { new SqlParameter("@Id", id) };
            return EnsureDoctorCodeInDataTable(RunSelect("SELECT * FROM Doctors WHERE Id = @Id", p));
        }

        public DataTable GetBySpecialty(string specialty)
        {
            EnsureDoctorCodeColumnExists();
            var p = new List<SqlParameter> { new SqlParameter("@Specialty", specialty) };
            return EnsureDoctorCodeInDataTable(RunSelect("SELECT * FROM Doctors WHERE Specialty = @Specialty ORDER BY DoctorName", p));
        }

        public int InsertAndReturnId()
        {
            EnsureDoctorCodeColumnExists();
            if (string.IsNullOrWhiteSpace(DoctorCode))
            {
                DoctorCode = GetNextAvailableDoctorCode();
            }
            DoctorCode = DoctorCode.Trim().ToUpper();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DoctorName", DoctorName),
                new SqlParameter("@DoctorCode", (object)DoctorCode ?? DBNull.Value),
                new SqlParameter("@Specialty", (object)Specialty ?? DBNull.Value),
                new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
                new SqlParameter("@Email", (object)Email ?? DBNull.Value),
                new SqlParameter("@DoctorType", DoctorType),
                new SqlParameter("@CommissionRate", (object)CommissionRate ?? DBNull.Value),
                new SqlParameter("@ConsultationFee", ConsultationFee)
            };
            string sql = @"INSERT INTO Doctors (DoctorName, DoctorCode, DoctorType, ConsultationFee, Specialty, Phone, Email, CommissionRate)
                           VALUES (@DoctorName, @DoctorCode, @DoctorType, @ConsultationFee, @Specialty, @Phone, @Email, @CommissionRate)";
            return RunInsertAndReturnId(sql, parameters);
        }

        public DataTable GetClinicDoctors()
        {
            EnsureDoctorCodeColumnExists();
            return EnsureDoctorCodeInDataTable(RunSelect("SELECT * FROM Doctors WHERE DoctorType='Clinic' ORDER BY DoctorName"));
        }

        /// <summary>
        /// جلب الأطباء الذين لديهم مواعيد متاحة مع عدد المواعيد المتاحة لكل طبيب
        /// </summary>
        public DataTable GetClinicDoctorsWithAvailableSchedules()
        {
            EnsureDoctorCodeColumnExists();
            return EnsureDoctorCodeInDataTable(RunSelect(@"
                SELECT 
                    d.Id,
                    d.DoctorName,
                    d.DoctorCode,
                    d.Specialty,
                    d.DoctorType,
                    d.ConsultationFee,
                    d.Phone,
                    d.Email,
                    d.CommissionRate,
                    COUNT(ds.Id) AS AvailableSchedules
                FROM Doctors d
                LEFT JOIN DoctorSchedules ds ON d.Id = ds.DoctorId 
                    AND ds.EndTime > GETDATE()
                    AND ISNULL(ds.BookedCount, 0) < ISNULL(NULLIF(ds.Capacity, 0), 1)
                WHERE d.DoctorType = 'Clinic'
                GROUP BY d.Id, d.DoctorName, d.DoctorCode, d.Specialty, d.DoctorType, d.ConsultationFee, 
                         d.Phone, d.Email, d.CommissionRate
                HAVING COUNT(ds.Id) > 0
                ORDER BY d.DoctorName"));
        }

        public DataTable GetSurgeryDoctors()
        {
            EnsureDoctorCodeColumnExists();
            return EnsureDoctorCodeInDataTable(RunSelect("SELECT * FROM Doctors WHERE DoctorType='Surgery' ORDER BY DoctorName"));
        }

        public string Update()
        {
            EnsureDoctorCodeColumnExists();
            if (string.IsNullOrWhiteSpace(DoctorCode))
            {
                DoctorCode = GetDoctorCode(Id);
            }
            DoctorCode = DoctorCode.Trim().ToUpper();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", Id),
                new SqlParameter("@DoctorName", DoctorName),
                new SqlParameter("@DoctorCode", (object)DoctorCode ?? DBNull.Value),
                new SqlParameter("@Specialty", (object)Specialty ?? DBNull.Value),
                new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
                new SqlParameter("@Email", (object)Email ?? DBNull.Value),
                new SqlParameter("@DoctorType", DoctorType),
                new SqlParameter("@CommissionRate", (object)CommissionRate ?? DBNull.Value),
                new SqlParameter("@ConsultationFee", ConsultationFee)
            };
            return RunInsDelUpd(@"UPDATE Doctors SET DoctorName=@DoctorName, DoctorCode=@DoctorCode, DoctorType=@DoctorType, Specialty=@Specialty,
                                   Phone=@Phone, Email=@Email, CommissionRate=@CommissionRate, ConsultationFee=@ConsultationFee
                                   WHERE Id=@Id", parameters);
        }

        public string Delete(int id)
        {
            return RunInsDelUpd("DELETE FROM Doctors WHERE Id = @Id", new List<SqlParameter> { new SqlParameter("@Id", id) });
        }

        public DataTable Search(string searchValue)
        {
            EnsureDoctorCodeColumnExists();
            if (string.IsNullOrWhiteSpace(searchValue))
                return GetAll();
            string search = "%" + searchValue.Trim() + "%";
            var p = new List<SqlParameter>
            {
                new SqlParameter("@Search", search),
                new SqlParameter("@Search2", search),
                new SqlParameter("@Search3", search),
                new SqlParameter("@Search4", search),
                new SqlParameter("@Search5", search)
            };
            return EnsureDoctorCodeInDataTable(RunSelect(@"SELECT * FROM Doctors WHERE DoctorName LIKE @Search OR Specialty LIKE @Search2
                               OR Phone LIKE @Search3 OR Email LIKE @Search4 OR DoctorCode LIKE @Search5 ORDER BY DoctorName", p));
        }
    }
}