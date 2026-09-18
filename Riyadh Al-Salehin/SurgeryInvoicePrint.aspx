<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SurgeryInvoicePrint.aspx.cs"
    Inherits="Riyadh_Al_Salehin.SurgeryInvoicePrint" %>

<!DOCTYPE html>
<html dir="rtl">
<head runat="server">
<title>طباعة فاتورة عملية</title>
<style>
    * { box-sizing: border-box; }
    html, body { margin: 0; padding: 0; font-family: Tahoma, Arial, sans-serif; width: 80mm; }
    .invoice-box { width: 80mm; max-width: 80mm; margin: 0 auto; padding: 2mm 3mm; overflow: hidden; }
    .invoice-title { text-align: center; border-bottom: 1px dashed #000; padding-bottom: 4px; margin-bottom: 4px; }
    .invoice-title h2 { margin: 0; font-size: 16px; font-weight: bold; word-wrap: break-word; }
    .invoice-title div { font-size: 12px; margin-top: 2px; }
    .invoice-table { width: 100%; border-collapse: collapse; table-layout: fixed; }
    .invoice-table td { padding: 2px 0; font-size: 12px; vertical-align: top; word-wrap: break-word; word-break: break-word; }
    .invoice-table td:first-child { width: 30mm; font-weight: bold; white-space: nowrap; }
    .costs-table { width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 4px; border-top: 1px dashed #000; padding-top: 2px; }
    .costs-table td { padding: 2px 0; font-size: 11px; }
    .costs-table td:last-child { text-align: left; }
    .total-row td { font-size: 13px; font-weight: bold; border-top: 1px dashed #000; padding-top: 4px; }
    .barcode-area { text-align: center; margin-top: 6px; }
    .barcode-area img { max-width: 100%; height: auto; }
    .barcode-text { text-align: center; font-size: 11px; margin-top: 2px; letter-spacing: 1px; }
    .small-barcode { max-width:60mm; height:auto; display:block; margin-top:4px; }
    .footer-note { text-align: center; font-size: 10px; border-top: 1px dashed #000; margin-top: 6px; padding-top: 4px; }
    .footer-note img, .footer-qr { max-width: 48mm; height: auto; display: block; margin: 4px auto 0 auto; }
    .no-print { text-align: center; margin-top: 8px; }
    .no-print button { font-family: Tahoma, Arial, sans-serif; font-size: 13px; padding: 6px 16px; cursor: pointer; }
    @media print { .no-print { display: none; } }
    @page { size: 80mm auto; margin: 0; }
</style>
</head>
<body onload="window.print();">
<form id="form1" runat="server">
<div class="invoice-box">
    <div class="invoice-title">
        <h2>مركز الحله للجراحة العامه</h2>
        <div>فاتورة عملية جراحية</div>
    </div>

    <table class="invoice-table">
        <tr><td>رقم العملية</td><td><asp:Label ID="lblSurgery" runat="server" /></td></tr>
        <tr><td>اسم العملية</td><td><asp:Label ID="lblSurgeryName" runat="server" /></td></tr>
        <tr><td>المريض</td><td><asp:Label ID="lblPatient" runat="server" /></td></tr>
        <tr><td>رقم المريض</td><td><asp:Label ID="lblPatientId" runat="server" /></td></tr>
        <tr><td>الطبيب</td><td><asp:Label ID="lblDoctor" runat="server" /></td></tr>
        <tr><td>الغرفة</td><td><asp:Label ID="lblRoom" runat="server" /></td></tr>
        <tr><td>حالة الدفع</td><td><asp:Label ID="lblPaymentStatus" runat="server" /></td></tr>
        <tr><td>التاريخ</td><td><asp:Label ID="lblDate" runat="server" /></td></tr>
        <tr><td>رقم الحجز</td><td><asp:Label ID="lblAppointmentId" runat="server" /></td></tr>
        <tr><th>تاريخ الطباعة</th><td><asp:Label ID="lblPrintDate" runat="server" /></td></tr>
    </table>

    <!-- جدول المستلزمات: تم تصحيح أسماء الحقول -->
    <table class="costs-table" style="margin-top:6px;">
        <thead>
            <tr>
                <td style="font-weight:bold;">المستلزم</td>
                <td style="font-weight:bold;">الكمية</td>
                <td style="font-weight:bold;">سعر الوحدة</td>
                <td style="font-weight:bold;">الإجمالي</td>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptSupplies" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("SupplyName") %></td>
                        <td><%# Eval("UsedQuantity") %></td>
                        <td><%# Eval("UnitPrice") %></td>
                        <td><%# Eval("TotalPrice") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>

    <table class="costs-table">
        <tr><td>أجر الطبيب</td><td><asp:Label ID="lblDoctorCost" runat="server" /></td></tr>
        <tr><td>غرفة العمليات</td><td><asp:Label ID="lblRoomCost" runat="server" /></td></tr>
        <tr><td>المستلزمات</td><td><asp:Label ID="lblSuppliesCost" runat="server" /></td></tr>
        <tr><td>تكاليف التمريض</td><td><asp:Label ID="lblOtherCost" runat="server" /></td></tr>
        <tr><td>الخصم</td><td><asp:Label ID="lblDiscount" runat="server" /></td></tr>
        <tr class="total-row"><td>الإجمالي</td><td><asp:Label ID="lblTotal" runat="server" /></td></tr>
        <tr><td>المدفوع</td><td><asp:Label ID="lblPaid" runat="server" /></td></tr>
        <tr><td>المتبقي</td><td><asp:Label ID="lblRemain" runat="server" /></td></tr>
    </table>

    <div class="barcode-area">
        <div><b>باركود العملية</b><br /><asp:Image ID="imgSurgeryBarcode" runat="server" /><br /><asp:Label ID="lblSurgeryBarcode" runat="server" /></div>
        <div class="barcode-area"><b>باركود المريض</b><br /><asp:Image ID="imgPatientBarcode" runat="server" CssClass="small-barcode" /><br /><asp:Label ID="Label4" runat="server" /></div>
        <div class="barcode-area"><b>باركود الحجز</b><br /><asp:Image ID="imgAppointmentBarcode" runat="server" CssClass="small-barcode" /><br /><asp:Label ID="Label3" runat="server" /></div>
        <br />
        <div><b>باركود الفاتورة</b><br /><asp:Image ID="imgInvoiceBarcode" runat="server" /><br /><asp:Label ID="lblInvoiceBarcode" runat="server" /></div>
    </div>

    <div class="footer-note" id="footerNote1">رقم التواصل :01157596992</div>
    <div class="footer-note">نتمنى لكم الشفاء العاجل</div>
    <div class="footer-note" id="footerNote2">
        <p>امسح QR Code الخاص بالصفحة</p>
        <asp:Image ID="imgFooterQR" runat="server" CssClass="footer-qr" ImageUrl="~/IMG/https_www_facebook_com_profile_php_id_100064742527140_locale_ar_AR.png" />
    </div>
</div>

<div class="no-print">
    <button type="button" onclick="window.print();">طباعة</button>
    <button type="button" onclick="window.close();">إغلاق</button>
</div>
</form>
</body>
</html>