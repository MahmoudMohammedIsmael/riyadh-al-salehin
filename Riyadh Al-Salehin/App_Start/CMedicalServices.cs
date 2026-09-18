using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CMedicalServices : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public string ServiceName { get; set; }
    public decimal? DoctorAmount { get; set; }
    public decimal? CenterAmount { get; set; }
    public decimal? TotalAmount { get; private set; }   // للقراءة فقط – يُملأ من قاعدة البيانات
    public bool? IsActive { get; set; } = true;         // القيمة الافتراضية true


    public DataTable GetById(int id)
    {
        return wt.RunSelect($"SELECT * FROM MedicalServices WHERE Id = {id}");
    }





    public DataTable GetAll()
    {
        return wt.RunSelect("SELECT * FROM MedicalServices ORDER BY ServiceName");
    }








    public DataTable Search(string keyword)
    {
        List<SqlParameter> p = new List<SqlParameter>();
        p.Add(new SqlParameter("@Keyword", "%" + keyword + "%"));

        return wt.RunSelect(@"
        SELECT * FROM MedicalServices
        WHERE ServiceName LIKE @Keyword
        ORDER BY ServiceName", p);
    }


    public int InsertAndReturnId()
    {
        int id = wt.RunInsertAndReturnId(@"
            INSERT INTO MedicalServices
            (
                ServiceName,
                DoctorAmount,
                CenterAmount,
                IsActive
            )
            VALUES
            (
                @ServiceName,
                @DoctorAmount,
                @CenterAmount,
                @IsActive
            )",
            new List<SqlParameter>
            {
                new SqlParameter("@ServiceName", (object)ServiceName ?? DBNull.Value),
                new SqlParameter("@DoctorAmount", (object)DoctorAmount ?? DBNull.Value),
                new SqlParameter("@CenterAmount", (object)CenterAmount ?? DBNull.Value),
                new SqlParameter("@IsActive", (object)IsActive ?? DBNull.Value)
            });

        return id;
    }

    public int GetLastServiceId()
    {
        DataTable dt = wt.RunSelect("SELECT MAX(Id) AS Id FROM MedicalServices");
        if (dt.Rows.Count > 0 && dt.Rows[0]["Id"] != DBNull.Value)
            return Convert.ToInt32(dt.Rows[0]["Id"]);
        return 0;
    }

    public string Update()
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", Id),
            new SqlParameter("@ServiceName", (object)ServiceName ?? DBNull.Value),
            new SqlParameter("@DoctorAmount", (object)DoctorAmount ?? DBNull.Value),
            new SqlParameter("@CenterAmount", (object)CenterAmount ?? DBNull.Value),
            new SqlParameter("@IsActive", (object)IsActive ?? DBNull.Value)
        };
        return wt.RunInsDelUpd(@"
            UPDATE MedicalServices
            SET ServiceName = @ServiceName,
                DoctorAmount = @DoctorAmount,
                CenterAmount = @CenterAmount,
                IsActive = @IsActive
            WHERE Id = @Id",
            parameters);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd($"DELETE FROM MedicalServices WHERE Id = {id}");
    }

    public string Add()
    {
        throw new NotImplementedException();
    }

    public string Delete()
    {
        return Delete(this.Id);
    }

    public DataTable Search()
    {
        return GetAll();
    }

}