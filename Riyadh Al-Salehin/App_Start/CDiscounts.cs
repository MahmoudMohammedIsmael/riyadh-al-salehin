using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CDiscounts : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    private int _id;
    private string _discountName;
    private string _discountType;
    private decimal _discountValue;
    private bool _isActive;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string DiscountName
    {
        get { return _discountName; }
        set { _discountName = value; }
    }

    public string DiscountType
    {
        get { return _discountType; }
        set { _discountType = value; }
    }

    public decimal DiscountValue
    {
        get { return _discountValue; }
        set { _discountValue = value; }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public DataTable GetAll()
    {
        return wt.RunSelect(
            "SELECT * FROM Discounts ORDER BY DiscountName");
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
            "SELECT * FROM Discounts WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@DiscountName", DiscountName));
        prm.Add(new SqlParameter("@DiscountType", DiscountType));
        prm.Add(new SqlParameter("@DiscountValue", DiscountValue));
        prm.Add(new SqlParameter("@IsActive", IsActive));

        return wt.RunInsDelUpd(@"
            INSERT INTO Discounts
            (
                DiscountName,
                DiscountType,
                DiscountValue,
                IsActive
            )
            VALUES
            (
                @DiscountName,
                @DiscountType,
                @DiscountValue,
                @IsActive
            )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@DiscountName", DiscountName));
        prm.Add(new SqlParameter("@DiscountType", DiscountType));
        prm.Add(new SqlParameter("@DiscountValue", DiscountValue));
        prm.Add(new SqlParameter("@IsActive", IsActive));

        return wt.RunInsDelUpd(@"
            UPDATE Discounts
            SET
                DiscountName=@DiscountName,
                DiscountType=@DiscountType,
                DiscountValue=@DiscountValue,
                IsActive=@IsActive
            WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
            "DELETE FROM Discounts WHERE Id=" + id);
    }

    public string Add()
    {
        return Insert();
    }

    public string Delete()
    {
        return Delete(Id);
    }

    public DataTable Search()
    {
        return GetAll();
    }
}