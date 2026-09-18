using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

public class WorkTable
{
    private static string _connectionString;
    private static readonly object _csLock = new object();

    public static string GetConnectionString()
    {
        if (!string.IsNullOrEmpty(_connectionString))
            return _connectionString;

        lock (_csLock)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;

            if (ConfigurationManager.ConnectionStrings == null)
            {
                throw new Exception("ConfigurationManager.ConnectionStrings = NULL");
            }

            ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["RiyadhConnection"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                string names = "";
                foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
                {
                    names += item.Name + "\n";
                }
                throw new Exception("لم يتم العثور على RiyadhConnection.\n\nالموجود هو:\n" + names);
            }

            _connectionString = cs.ConnectionString.Trim();
            return _connectionString;
        }
    }

    public WorkTable()
    {
        GetConnectionString();
    }

    public string RunInsDelUpd(string statement)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(statement, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return "OK";
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627) return "Exist User Name";
            return ex.Message;
        }
    }

    public object RunScalar(string query, List<SqlParameter> prm)
    {
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            if (prm != null)
            {
                foreach (var p in prm)
                {
                    cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                }
            }
            conn.Open();
            return cmd.ExecuteScalar();
        }
    }

    public string RunInsDelUpd(string statement, List<SqlParameter> parameters)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(statement, conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                    }
                }
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return "OK";
        }
        catch (SqlException ex)
        {
            return ex.Message;
        }
    }

    public DataTable RunSelect(string sql, List<SqlParameter> parameters = null)
    {
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                }
            }
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }
        return dt;
    }

    public string InsertArabicData(string name)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO MyTable (Name) VALUES (@Name)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return "تم الإدخال بنجاح";
        }
        catch (SqlException ex)
        {
            return ex.Message;
        }
    }

    public void RunUpdate(string updateQuery)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new Exception("خطأ أثناء التحديث", ex);
        }
    }

    public int RunInsertAndReturnId(string query, List<SqlParameter> parameters)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(query + "\r\nSELECT CAST(SCOPE_IDENTITY() AS INT)", conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                    }
                }
                conn.Open();
                object res = cmd.ExecuteScalar();
                return (res != null && res != DBNull.Value) ? Convert.ToInt32(res) : 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
