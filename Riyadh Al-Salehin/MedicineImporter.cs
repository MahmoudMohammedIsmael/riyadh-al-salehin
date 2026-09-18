using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Riyadh_Al_Salehin
{
    public class MedicineImporter
    {
        private readonly string _connectionString;

        private static readonly Regex StrengthFormRegex = new Regex(
            @"^(?<name>.+?)\s+(?<strength>[\d.\/]+\s*(MG|ML|MG/ML|MCG|UNT|%)[^ ]*)\s+(?<form>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public MedicineImporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        public int Import(string filePath)
        {
            var table = new DataTable();
            table.Columns.Add("MedicineName", typeof(string));
            table.Columns.Add("GenericName", typeof(string));
            table.Columns.Add("Strength", typeof(string));
            table.Columns.Add("Form", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));

            var seen = new HashSet<string>();

            using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var cols = line.Split('|');
                    if (cols.Length < 15) continue;

                    string lat = cols[1];
                    string sab = cols[11];
                    string tty = cols[12];
                    string str = cols[14];

                    if (lat != "ENG") continue;
                    if (sab != "RXNORM") continue;
                    if (tty != "SCD" && tty != "SBD") continue;
                    if (!seen.Add(str)) continue;

                    string genericName = str;
                    string strength = null;
                    string form = null;

                    var match = StrengthFormRegex.Match(str);
                    if (match.Success)
                    {
                        genericName = match.Groups["name"].Value.Trim();
                        strength = match.Groups["strength"].Value.Trim();
                        form = match.Groups["form"].Value.Trim();
                    }

                    DataRow row = table.NewRow();
                    row["MedicineName"] = Truncate(str, 1000);
                    row["GenericName"] = Truncate(genericName, 1000);
                    row["Strength"] = (object)Truncate(strength, 300) ?? DBNull.Value;
                    row["Form"] = (object)Truncate(form, 300) ?? DBNull.Value;
                    row["IsActive"] = true;
                    table.Rows.Add(row);
                }
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var bulk = new SqlBulkCopy(conn))
                {
                    bulk.DestinationTableName = "dbo.Medicines";
                    bulk.ColumnMappings.Add("MedicineName", "MedicineName");
                    bulk.ColumnMappings.Add("GenericName", "GenericName");
                    bulk.ColumnMappings.Add("Strength", "Strength");
                    bulk.ColumnMappings.Add("Form", "Form");
                    bulk.ColumnMappings.Add("IsActive", "IsActive");
                    bulk.BatchSize = 5000;
                    bulk.WriteToServer(table);
                }
            }

            return table.Rows.Count;
        }
    }
}