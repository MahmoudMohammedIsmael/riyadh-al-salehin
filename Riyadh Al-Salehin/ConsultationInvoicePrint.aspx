<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ConsultationInvoicePrint.aspx.cs"
    Inherits="Riyadh_Al_Salehin.ConsultationInvoicePrint" %>

<!DOCTYPE html>

<html dir="rtl">
<head runat="server">

<title>طباعة فاتورة</title>

<style>

/* ===== إعدادات عامة لورق طابعة حرارية 80mm ===== */

* {
    box-sizing: border-box;
}


.logo .logo-img {
    width: 50px;
    height: 50px;
    border-radius: 14px;
    object-fit: cover;
    box-shadow: 0 4px 12px rgba(13, 110, 253, 0.3);
    display: block;
}

        .top-header .system-title .logo-img {
            width: 34px;
            height: 34px;
            object-fit: cover;
            border-radius: 6px;
            display: inline-block;
            box-shadow: 0 2px 6px rgba(0,0,0,0.12);
        }



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

.invoice-title h2 {
    margin: 0;
    font-size: 16px;
    font-weight: bold;
    word-wrap: break-word;
}

.invoice-title div {
    font-size: 12px;
    margin-top: 2px;
}

.invoice-table {
    width: 100%;
    border-collapse: collapse;
    table-layout: fixed;
}

.invoice-table td {
    padding: 2px 0;
    font-size: 12px;
    vertical-align: top;
    word-wrap: break-word;
    word-break: break-word;
}

.invoice-table td:first-child {
    width: 30mm;
    font-weight: bold;
    white-space: nowrap;
}

.queue-label {
    text-align: center;
    font-size: 11px;
    margin-top: 6px;
}

.queue-big {
    text-align: center;
    /* حجم مناسب لورق 80mm حتى مع أرقام من 3 خانات */
    font-size: 34px;
    font-weight: bold;
    border: 2px solid #000;
    border-radius: 4px;
    margin: 4px 0 8px 0;
    padding: 4px 0;
    line-height: 1.1;
}

.barcode-area {
    text-align: center;
    margin-top: 4px;
}

.barcode-area img {
    max-width: 100%;
    height: auto;
}

.footer-note {
    text-align: center;
    font-size: 10px;
    border-top: 1px dashed #000;
    margin-top: 6px;
    padding-top: 4px;
}

.footer-note img, .footer-qr {
    max-width: 48mm;
    height: auto;
    display: block;
    margin: 4px auto 0 auto;
}

.small-barcode {
    max-width: 60mm;
    height: auto;
    display: block;
}

/* زر الإغلاق يظهر على الشاشة فقط ولا يُطبع */
.no-print {
    text-align: center;
    margin-top: 8px;
}

.no-print button {
    font-family: Tahoma, Arial, sans-serif;
    font-size: 13px;
    padding: 6px 16px;
    cursor: pointer;
}

@media print {
    .no-print {
        display: none;
    }
}

@page {
    size: 80mm auto;
    margin: 0;
}

</style>

</head>

<body onload="window.print();">

<form id="form1" runat="server">

<div class="invoice-box">

<div class="invoice-title" id="invoiceTitle">


     <asp:Image ID="Image2" runat="server" CssClass="footer-qr" ImageUrl="~/IMG/RD.jpg" />

       
        <h2>عيادات الرياض الصالحين</h2>

        <div>فاتورة كشف طبي</div>
        <asp:Label ID="headerExtraText" runat="server" Visible="false" CssClass="inv-custom-header" />

    </div>

    <table class="invoice-table">
        
        <tr>
            
           
        </tr>

        <tr>
            <td>الطبيب</td>
            <td>
                <asp:Label ID="lblDoctor" runat="server" />
            </td>
        </tr>

        <tr>
    <td>المريض</td>
    <td>
        <asp:Label ID="lblPatient" runat="server" />
    </td>
</tr>
        <tr>
           
        </tr>

         <tr id="paymentStatusRow">
      <td>حالة الدفع</td>
      <td>
           <asp:Label ID="lblPaymentStatus" runat="server" />
      </td>
  </tr>

        <tr>
    <td>رقم الفاتورة</td>
    <td>
        <asp:Label ID="lblInvoiceId" runat="server" />
    </td>
</tr>

<tr>
    <td>رقم الحجز</td>
    <td>
        <asp:Label ID="lblAppointmentId" runat="server" />
    </td>
</tr>

<tr>
    <td>رقم المريض</td>
    <td>
        <asp:Label ID="lblPatientId" runat="server" />
    </td>
</tr>





        <tr id="amountDetails">
            <td>القيمة</td>
            <td>
                <asp:Label ID="lblAmount" runat="server" />
            </td>
        </tr>

        <tr>
            <td>التاريخ</td>
            <td>
                <asp:Label ID="lblDate" runat="server" />
            </td>
        </tr>

    </table>

    <div id="queueArea">
    <div class="queue-label">رقم الانتظار</div>

    <div class="queue-big">

        <asp:Label ID="lblQueue" runat="server" />

    </div>
    </div>

    <div class="barcode-area" id="barcodeArea">


        <asp:Image ID="imgBarcode" runat="server" />

        <p>رقم المريض</p>

        <asp:Image ID="Image1" runat="server" CssClass="small-barcode" />

         
 <p>رقم الحجز </p>
     <asp:Image ID="imgAppointmentBarcode" runat="server" CssClass="small-barcode" />
 
    </div>

    <div class="footer-note" id="footerNote1">رقم التواصل :01157596992</div>
    <div class="footer-note" id="footerNote">
        نتمنى لكم الشفاء العاجل
    </div>

    <div class="footer-note" id="footerNote2">
       <p> امسح QR Code الخاص بالصفحة</p>
       <asp:Image ID="imgFooterQR" runat="server" CssClass="footer-qr" ImageUrl="~/IMG/https_www_facebook_com_profile_php_id_100064742527140_locale_ar_AR.png" />

    </div>
    <asp:Label ID="footerExtraText" runat="server" Visible="false" CssClass="footer-note inv-custom-footer" />
    <asp:Label ID="watermarkText" runat="server" Visible="false" CssClass="inv-custom-watermark" />

</div>

<div class="no-print">
    <button type="button" onclick="window.print();">طباعة</button>
    <button type="button" onclick="window.close();">إغلاق</button>
</div>

</form>

</body>
</html>
