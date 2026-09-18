<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XrayInvoicePrint.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayInvoicePrint" %>

<!DOCTYPE html>

<html dir="rtl">
<head runat="server">
    <title>طباعة فاتورة أشعة</title>
    <style>
        /* ===== إعدادات عامة لورق طابعة حرارية 80mm ===== */
        * {
            box-sizing: border-box;
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
        }

        .invoice-table td:first-child {
            width: 30mm;
            font-weight: bold;
        }

        /* تفاصيل التكلفة */
        .costs-table {
            width: 100%;
            border-collapse: collapse;
            table-layout: fixed;
            margin-top: 4px;
            border-top: 1px dashed #000;
            padding-top: 2px;
        }

        .costs-table td {
            padding: 2px 0;
            font-size: 11px;
        }

        .costs-table td:last-child {
            text-align: left;
        }

        .total-row td {
            font-size: 13px;
            font-weight: bold;
            border-top: 1px dashed #000;
            padding-top: 4px;
        }

        .barcode-area {
            text-align: center;
            margin-top: 6px;
        }

        .barcode-area img {
            max-width: 100%;
            height: auto;
        }

        .small-barcode { max-width:60mm; height:auto; display:block; margin-top:4px; }

        .barcode-text {
            text-align: center;
            font-size: 11px;
            margin-top: 2px;
            letter-spacing: 1px;
        }

        .footer-note {
            text-align: center;
            font-size: 10px;
            border-top: 1px dashed #000;
            margin-top: 6px;
            padding-top: 4px;
        }

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

            <div class="invoice-title">
                <h2>مركز الرياض الطبي</h2>
                <div>فاتورة أشعة</div>
            </div>

            <table class="invoice-table">
                <tr>
                    <td>رقم الطلب</td>
                    <td><asp:Label ID="lblRequestId" runat="server" /></td>
                </tr>
                <tr>
                    <td>المريض</td>
                    <td><asp:Label ID="lblPatient" runat="server" /></td>
                </tr>
                <tr>
                    <td>رقم المريض</td>
                    <td><asp:Label ID="lblPatientId" runat="server" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Image ID="imgPatientBarcode" runat="server" CssClass="small-barcode" /></td>
                </tr>
                <tr>
                    <td>رقم الحجز</td>
                    <td><asp:Label ID="lblAppointmentId" runat="server" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Image ID="imgAppointmentBarcode" runat="server" CssClass="small-barcode" /></td>
                </tr>
                <tr>
                    <td>نوع الخدمة</td>
                    <td><asp:Label ID="lblService" runat="server" /></td>
                </tr>
                <tr>
                    <td>السعر</td>
                    <td><asp:Label ID="lblPrice" runat="server" /></td>
                </tr>
                <tr>
                    <td>تاريخ الطلب</td>
                    <td><asp:Label ID="lblDate" runat="server" /></td>
                </tr>
                <tr>
                    <td>تاريخ الطباعة</td>
                    <td><asp:Label ID="lblPrintDate" runat="server" /></td>
                </tr>
            </table>

            <!-- جدول التكاليف (للتنسيق فقط، لأن الأشعة لها سعر واحد) -->
            <table class="costs-table">
                <tr>
                    <td>سعر الأشعة</td>
                    <td><asp:Label ID="lblPriceCost" runat="server" /></td>
                </tr>
                <tr class="total-row">
                    <td>الإجمالي</td>
                    <td><asp:Label ID="lblTotal" runat="server" /></td>
                </tr>
            </table>

            <!-- باركود (اختياري) -->
            <div class="barcode-area">
                <div>
                    <b>باركود الطلب</b><br />
                    <asp:Image ID="imgBarcode" runat="server" />
                    <br />
                    <asp:Label ID="lblBarcodeText" runat="server" />
                </div>
            </div>

            <div class="footer-note">
                رقم الهاتف: 01000000000
            </div>
            <div class="footer-note">
                نتمنى لكم الشفاء العاجل
            </div>

        </div>

        <div class="no-print">
            <button type="button" onclick="window.print();">طباعة</button>
            <button type="button" onclick="window.close();">إغلاق</button>
        </div>
    </form>

</body>
</html>