using System;
using System.Data.SqlClient;
using System.IO;

namespace Riyadh_Al_Salehin
{
    public class ICDImporter
    {
        private readonly string _connectionString;

        public ICDImporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Import(string filePath)
        {
            int count = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                foreach (string line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(';');

                    if (parts.Length < 11)
                        continue;

                    if (parts[0].Trim() != "4")
                        continue;

                    string icdCode = parts[6].Trim();
                    string diagnosisEnglish = parts[8].Trim();
                    string category = parts[9].Trim();

                    if (string.IsNullOrWhiteSpace(icdCode))
                        continue;

                    SqlCommand cmd = new SqlCommand(@"
IF NOT EXISTS
(
    SELECT 1
    FROM Diagnoses
    WHERE ICDCode=@ICDCode
)
BEGIN

INSERT INTO Diagnoses
(
    ICDCode,
    DiagnosisEnglish,
    Category,
    IsActive
)
VALUES
(
    @ICDCode,
    @DiagnosisEnglish,
    @Category,
    1
)

END", con);

                    cmd.Parameters.AddWithValue("@ICDCode", icdCode);
                    cmd.Parameters.AddWithValue("@DiagnosisEnglish", diagnosisEnglish);
                    cmd.Parameters.AddWithValue("@Category", category);

                    cmd.ExecuteNonQuery();

                    count++;
                }

                con.Close();
            }

            return count;
        }
    }
}