using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CInvoiceItems : WorkTable, BasicOperetoin
{
    private WorkTable wt = new WorkTable();

    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ServiceId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public DataTable GetAll()
    {
        return wt.RunSelect(@"
        SELECT ii.*,
               s.ServiceName
        FROM InvoiceItems ii
        INNER JOIN ServicesAndPrices s
            ON ii.ServiceId=s.Id");
    }

    public DataTable GetByInvoice(int invoiceId)
    {
        return wt.RunSelect(@"
        SELECT ii.*,
               s.ServiceName
        FROM InvoiceItems ii
        INNER JOIN ServicesAndPrices s
            ON ii.ServiceId=s.Id
        WHERE ii.InvoiceId=" + invoiceId);
    }

    public DataTable GetById(int id)
    {
        return wt.RunSelect(
        "SELECT * FROM InvoiceItems WHERE Id=" + id);
    }

    public string Insert()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@InvoiceId", InvoiceId));
        prm.Add(new SqlParameter("@ServiceId", ServiceId));
        prm.Add(new SqlParameter("@Quantity", Quantity));
        prm.Add(new SqlParameter("@UnitPrice", UnitPrice));
        prm.Add(new SqlParameter("@TotalPrice", TotalPrice));

        return wt.RunInsDelUpd(@"
        INSERT INTO InvoiceItems
        (
            InvoiceId,
            ServiceId,
            Quantity,
            UnitPrice,
            TotalPrice
        )
        VALUES
        (
            @InvoiceId,
            @ServiceId,
            @Quantity,
            @UnitPrice,
            @TotalPrice
        )", prm);
    }

    public string Update()
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", Id));
        prm.Add(new SqlParameter("@Quantity", Quantity));
        prm.Add(new SqlParameter("@UnitPrice", UnitPrice));
        prm.Add(new SqlParameter("@TotalPrice", TotalPrice));

        return wt.RunInsDelUpd(@"
        UPDATE InvoiceItems
        SET
            Quantity=@Quantity,
            UnitPrice=@UnitPrice,
            TotalPrice=@TotalPrice
        WHERE Id=@Id", prm);
    }

    public string Delete(int id)
    {
        return wt.RunInsDelUpd(
            "DELETE FROM InvoiceItems WHERE Id=" + id);
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

    

    /// <summary>
    /// يعدّل إجمالي الفاتورة بمقدار delta (موجب = إضافة، سالب = خصم)
    /// يُستخدم عند إضافة/تعديل/حذف بند إضافي من InvoiceItems (مثل حشو ضرس)
    /// حتى يبقى Invoices.TotalAmount متزامن دائمًا مع مجموع البنود
    /// </summary>
    public string AdjustTotalAmount(int invoiceId, decimal delta)
    {
        List<SqlParameter> prm = new List<SqlParameter>();

        prm.Add(new SqlParameter("@Id", invoiceId));
        prm.Add(new SqlParameter("@Delta", delta));

        return wt.RunInsDelUpd(@"
        UPDATE Invoices
        SET TotalAmount = TotalAmount + @Delta
        WHERE Id = @Id", prm);
    }









}