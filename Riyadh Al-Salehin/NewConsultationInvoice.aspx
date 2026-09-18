<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="NewConsultationInvoice.aspx.cs" Inherits="Riyadh_Al_Salehin.NewConsultationInvoice" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .page-title-bar {
        background: linear-gradient(135deg, #0d6efd, #0a58ca);
        color: #ffffff !important;
        padding: 16px 24px;
        border-radius: 12px 12px 0 0;
        display: flex;
        align-items: center;
        gap: 10px;
        box-shadow: 0 4px 12px rgba(13,110,253,.25);
    }
    .page-title-bar h4, .page-title-bar span {
        color: #ffffff !important;
        font-size: 20px !important;
        font-weight: 700 !important;
        margin: 0 !important;
        line-height: 1.4 !important;
    }
    .page-title-bar i { font-size: 22px; color: #ffffff; }
    .invoice-box {
        background: #f8f9fc;
        padding: 20px;
        border-radius: 10px;
        border: 1px solid #e0e0e0;
    }
    .invoice-label {
        font-weight: 600;
        color: #495057;
        display: block;
        margin-bottom: 4px;
    }
    .invoice-value {
        background: #ffffff;
        padding: 8px 12px;
        border-radius: 6px;
        border: 1px solid #ced4da;
        min-height: 38px;
        font-weight: 500;
        color: #212529;
    }
    #printArea {
        display: none;
        margin-top: 20px;
        border-top: 2px dashed #ccc;
        padding-top: 20px;
    }
    .invoice-print-preview {
        width: 76mm;
        max-width: 100%;
        margin: 0 auto;
        padding: 3mm;
        background: #fff;
        color: #000;
        font-family: Tahoma, Arial, sans-serif;
        box-sizing: border-box;
        border: 1px solid #ddd;
        border-radius: 8px;
    }
    .invoice-print-preview .title {
        text-align: center;
        border-bottom: 1px dashed #000;
        padding-bottom: 5px;
        margin-bottom: 8px;
    }
    .invoice-print-preview .title h2 { margin: 0; font-size: 22px; font-weight: bold; }
    .invoice-print-preview .title h4 { margin: 4px 0; font-size: 16px; }
    .invoice-print-preview table { width: 100%; border-collapse: collapse; }
    .invoice-print-preview table td { padding: 4px; font-size: 15px; }
    .invoice-print-preview table td:first-child { width: 35%; font-weight: bold; }
    .queue-big {
        text-align: center;
        font-size: 70px;
        font-weight: bold;
        border: 2px solid #000;
        padding: 8px;
        margin: 10px 0;
        line-height: 1;
    }
    .barcode-area { text-align: center; margin-top: 10px; }
    .barcode-area img { width: 240px; max-width: 100%; height: auto; }
    .footer-text {
        text-align: center;
        margin-top: 10px;
        padding-top: 5px;
        border-top: 1px dashed #000;
        font-size: 13px;
    }
    @media print {
        .no-print { display: none !important; }
        .invoice-print-preview { border: none !important; }
    }
</style>  

<div class="container mt-4" dir="rtl">
    <div class="card shadow">
        <div class="page-title-bar">
            <i class="bi bi-receipt-cutoff"></i>
            <h4>فاتورة الكشف</h4>
        </div>

        <div class="card-body">
            <!-- Hidden Fields -->
            <asp:HiddenField ID="hfAppointmentId" runat="server" />
            <asp:HiddenField ID="hfServiceId" runat="server" />
            <asp:HiddenField ID="hfPatientId" runat="server" />
            <asp:HiddenField ID="hfDoctorId" runat="server" />
            <asp:HiddenField ID="hfCenterId" runat="server" />

            <div class="row invoice-box">
                <!-- العمود الأيمن -->
                <div class="col-md-6">
                    <div class="mb-3">
                        <span class="invoice-label">الطبيب</span>
                        <div class="invoice-value" id="lblDoctor" runat="server">---</div>
                    </div>
                    <div class="mb-3">
                        <span class="invoice-label">نوع الخدمة</span>
                        <div class="invoice-value" id="lblServiceName" runat="server" style="font-weight:bold; color:#0d6efd;">---</div>
                    </div>
                    <div class="mb-3">
                        <span class="invoice-label">طريقة الدفع</span>
                        <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select">
                            <asp:ListItem Text="نقدي" Value="Cash" />
                            <asp:ListItem Text="فيزا" Value="Visa" />
                            <asp:ListItem Text="تحويل" Value="Transfer" />
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <span class="invoice-label">حالة الدفع</span>
                        <asp:DropDownList ID="ddlPaymentStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Text="مدفوع" Value="Paid" />
                            <asp:ListItem Text="غير مدفوع" Value="Unpaid" />
                            <asp:ListItem Text="جزئي" Value="Partial" />
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- العمود الأيسر -->
                <div class="col-md-6">
                    <div class="mb-3">
                        <span class="invoice-label">المريض</span>
                        <div class="invoice-value" id="lblPatient" runat="server">---</div>
                    </div>
                    <div class="mb-3">
                        <span class="invoice-label">قيمة الكشف</span>
                        <div class="invoice-value" id="lblAmount" runat="server" style="font-size:20px; font-weight:bold; color:#0d6efd;">0.00</div>
                    </div>
                    <div class="mb-3">
                        <span class="invoice-label">نوع الحجز</span>
                        <asp:DropDownList ID="ddlBookingType" runat="server" CssClass="form-select">
                            <asp:ListItem Text="حضور مباشر" Value="WalkIn" />
                            <asp:ListItem Text="حجز هاتفي" Value="Phone" />
                            <asp:ListItem Text="حجز إلكتروني" Value="Online" />
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- المبلغ المدفوع والمتبقي -->
                <div class="col-md-12 mt-2">
                    <div class="row">
                        <div class="col-md-4">
                            <span class="invoice-label">المبلغ المدفوع</span>
                            <asp:TextBox ID="txtPaidAmount" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                        </div>
                        <div class="col-md-4">
                            <span class="invoice-label">المبلغ المتبقي</span>
                            <asp:TextBox ID="txtRemaining" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-4 d-flex align-items-end">
                            <asp:Button ID="btnSaveInvoice" runat="server" 
                                Text="حفظ الفاتورة" 
                                CssClass="btn btn-success w-100"
                                OnClick="btnSaveInvoice_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>

            <!-- منطقة الطباعة (تظهر بعد الحفظ أو عند وجود بيانات) -->
            <div id="printArea" runat="server" class="no-print">
                <hr />
                <div class="text-center">
                    <asp:Button ID="btnPrint" runat="server" 
                        Text="طباعة الفاتورة" 
                        CssClass="btn btn-primary"
                        OnClick="btnPrint_Click" />
                </div>

                <!-- معاينة الفاتورة -->
                <div class="invoice-print-preview">
                    <div class="title">
                        <h2>عيادات الرياض الصالحين</h2>
                        <div>هاتف: 01000000000</div>
                        <h4>فاتورة كشف طبي</h4>
                    </div>
                    <table>
                        <tr><td>المريض</td><td><asp:Label ID="lblPatientPrint" runat="server" /></td></tr>
                        <tr><td>الطبيب</td><td><asp:Label ID="lblDoctorPrint" runat="server" /></td></tr>
                        <tr><td>التاريخ</td><td><asp:Label ID="lblDatePrint" runat="server" /></td></tr>
                        <tr><td>القيمة</td><td><asp:Label ID="lblAmountPrint" runat="server" /></td></tr>
                        <tr><td>المدفوع</td><td><asp:Label ID="lblPaidPrint" runat="server" /></td></tr>
                        <tr><td>المتبقي</td><td><asp:Label ID="lblRemainingPrint" runat="server" /></td></tr>
                        <tr><td>طريقة الدفع</td><td><asp:Label ID="lblPaymentMethodPrint" runat="server" /></td></tr>
                        <tr><td>حالة الدفع</td><td><asp:Label ID="lblPaymentStatusPrint" runat="server" /></td></tr>
                    </table>
                    <div style="text-align:center;font-size:16px;font-weight:bold">رقم الانتظار</div>
                    <div class="queue-big">
                        <asp:Label ID="lblQueuePrint" runat="server" />
                    </div>
                    <div class="barcode-area">
                        <asp:Image ID="imgBarcode" runat="server" />
                    </div>
                    <div class="footer-text">
                        شكراً لزيارتكم<br />يرجى الاحتفاظ بالفاتورة
                    </div>
                </div>
            </div>

            <hr />
            <div class="text-center">
                <asp:Button ID="btnBack" runat="server" 
                    Text="العودة إلى القائمة" 
                    CssClass="btn btn-secondary"
                    OnClick="btnBack_Click" />
            </div>
        </div>
    </div>
</div>

</asp:Content>