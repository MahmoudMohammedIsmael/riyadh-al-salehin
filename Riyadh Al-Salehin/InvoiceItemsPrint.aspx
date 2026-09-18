<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceItemsPrint.aspx.cs" Inherits="Riyadh_Al_Salehin.InvoiceItemsPrint" %>
<!DOCTYPE html>
<html dir="rtl">
<head runat="server">
<title>طباعة فاتورة</title>
<style>
    * { box-sizing: border-box; }

    html, body {
        margin: 0;
        padding: 0;
        font-family: Tahoma, Arial, sans-serif;
        width: 80mm;
    }

    .invoice-box {
        width: 80mm;
        max-width: 80mm;
        margin: 0 auto;
        padding: 2mm 3mm;
        overflow: hidden;
    }

    .invoice-title {
        text-align: center;
        border-bottom: 1px dashed #000;
        padding-bottom: 4px;
        margin-bottom: 4px;
    }
    .invoice-title h2 { margin:0; font-size:16px; font-weight:bold; word-wrap:break-word; }
    .invoice-title div { font-size:12px; margin-top:2px; }

    .invoice-table { width:100%; border-collapse:collapse; table-layout:fixed; margin-bottom:6px; }
    .invoice-table td { padding:2px 0; font-size:12px; vertical-align:top; word-wrap:break-word; word-break:break-word; }
    .invoice-table td:first-child { width:30mm; font-weight:bold; white-space:nowrap; }

    .items-table { width:100%; border-collapse:collapse; margin-top:4px; }
    .items-table th, .items-table td {
        font-size:11px; padding:2px 1px; text-align:right; border-bottom:1px dashed #ccc;
    }
    .items-table th { border-bottom:1px solid #000; }
    .items-table td.num, .items-table th.num { text-align:center; }

    .total-row {
        display:flex; justify-content:space-between;
        font-size:13px; font-weight:bold;
        border-top:1px solid #000; margin-top:4px; padding-top:4px;
    }

    .queue-label { text-align:center; font-size:11px; margin-top:6px; }
    .queue-big {
        text-align:center; font-size:34px; font-weight:bold;
        border:2px solid #000; border-radius:4px; margin:4px 0 8px 0; padding:4px 0; line-height:1.1;
    }

    .footer-note { text-align:center; font-size:10px; border-top:1px dashed #000; margin-top:6px; padding-top:4px; }

    .no-print { text-align:center; margin-top:8px; }
    .no-print button { font-family: Tahoma, Arial, sans-serif; font-size:13px; padding:6px 16px; cursor:pointer; }

    .small-barcode { max-width:60mm; height:auto; display:block; margin-top:4px; }

    @media print { .no-print { display:none; } }
    @page { size: 80mm auto; margin:0; }
</style>
</head>
<body onload="window.print();">
<form id="form1" runat="server">
<div class="invoice-box">

    <div class="invoice-title">
        <h2>عيادات الرياض الصالحين</h2>
        <div>فاتورة إضافية</div>
    </div>

    <table class="invoice-table">
        <tr><td>رقم الفاتورة</td><td><asp:Label ID="lblInvoiceId" runat="server" /></td></tr>
        <tr><td>رقم الحجز</td><td><asp:Label ID="lblAppointmentId" runat="server" /></td></tr>
        <tr><td>رقم المريض</td><td><asp:Label ID="lblPatientId" runat="server" /></td></tr>
        <tr><td>المريض</td><td><asp:Label ID="lblPatient" runat="server" /></td></tr>
        <tr><td></td><td><asp:Image ID="imgPatientBarcode" runat="server" CssClass="small-barcode" /></td></tr>
        <tr><td>الطبيب</td><td><asp:Label ID="lblDoctor" runat="server" /></td></tr>
        <tr><td></td><td><asp:Image ID="imgAppointmentBarcode" runat="server" CssClass="small-barcode" /></td></tr>
        <tr><td>التاريخ</td><td><asp:Label ID="lblDate" runat="server" /></td></tr>
        <tr><td>حالة الدفع</td><td><asp:Label ID="lblPaymentStatus" runat="server" /></td></tr>
    </table>

    <asp:Repeater ID="rptItems" runat="server">
        <HeaderTemplate>
            <table class="items-table">
                <tr>
                    <th>الخدمة</th>
                    <th class="num">كمية</th>
                    <th class="num">سعر</th>
                    <th class="num">إجمالي</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("ServiceName") %></td>
                <td class="num"><%# Eval("Quantity") %></td>
                <td class="num"><%# Eval("UnitPrice", "{0:N2}") %></td>
                <td class="num"><%# Eval("TotalPrice", "{0:N2}") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <div class="total-row">
        <span>الإجمالي الكلي</span>
        <span><asp:Label ID="lblGrandTotal" runat="server" /></span>
    </div>

    <div class="queue-label">رقم الانتظار</div>
    <div class="queue-big"><asp:Label ID="lblQueue" runat="server" /></div>

    <div class="footer-note">نتمنى لكم الشفاء العاجل</div>
</div>

<div class="no-print">
    <button type="button" onclick="window.print();">طباعة</button>
    <button type="button" onclick="window.close();">إغلاق</button>
</div>
</form>
    <script>
window.onload = function () {

    setTimeout(function () {

        window.print();

    }, 800);

};
    </script>
</body>
</html>