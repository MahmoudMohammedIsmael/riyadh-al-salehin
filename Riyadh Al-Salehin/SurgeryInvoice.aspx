<%@ Page Language="C#" AutoEventWireup="true"
CodeBehind="SurgeryInvoice.aspx.cs"
Inherits="Riyadh_Al_Salehin.SurgeryInvoice"
MasterPageFile="~/Riyadh.Master" %>

<asp:Content ID="Content1"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container mt-4" dir="rtl">

<div class="card shadow">

<div class="card-header bg-danger text-white">
<h3 class="mb-0">🩺 فاتورة العملية الجراحية</h3>
<asp:Label ID="lblPayRemainingNotice" runat="server" Visible="false"
    CssClass="d-block mt-2 p-3 fw-bold"
    Style="font-size:16px; background:#fff3cd; color:#856404; border:2px solid #ffc107; border-radius:8px;" />
</div>

<div class="card-body">

<asp:HiddenField ID="hfSurgeryId" runat="server" />
<asp:HiddenField ID="hfPatientId" runat="server" />

<!--================ بيانات العملية ================-->

<div class="row">

<div class="col-md-4">

<label>رقم العملية</label>

<asp:TextBox
ID="txtSurgeryId"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>المريض</label>

<asp:TextBox
ID="txtPatient"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>التاريخ</label>

<asp:TextBox
ID="txtDate"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

</div>

<br />

<div class="row">

<div class="col-md-4">

<label>اسم العملية</label>

<asp:TextBox
ID="txtSurgeryName"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>الطبيب</label>

<asp:TextBox
ID="txtDoctor"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>غرفة العمليات</label>

<asp:TextBox
ID="txtRoom"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

</div>

<hr />

<h4 class="text-primary">

تفاصيل التكاليف

</h4>

<asp:GridView
ID="gvInvoice"
runat="server"
CssClass="table table-bordered table-striped"
AutoGenerateColumns="False">

<Columns>

<asp:BoundField
DataField="ItemName"
HeaderText="البند" />

<asp:BoundField
DataField="Amount"
HeaderText="القيمة"
DataFormatString="{0:N2}" />

</Columns>

</asp:GridView>

<br />

<div class="row">

<div class="col-md-4">

<label>تكلفة الطبيب</label>

<asp:TextBox
ID="txtDoctorCost"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>تكلفة الغرفة / المتبقي منها</label>

<asp:TextBox
ID="txtRoomCost"
runat="server"
CssClass="form-control bg-warning"
ReadOnly="true"/>

</div>

<div class="col-md-4">

<label>تكلفة المستلزمات</label>

<asp:TextBox
ID="txtSuppliesCost"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

</div>

<br />

<div class="row">

<div class="col-md-4">

<label>تكاليف التمريض</label>

<asp:TextBox
ID="txtOtherCost"
runat="server"
CssClass="form-control"
Text="0"
AutoPostBack="true"
OnTextChanged="CalculateTotal"/>

</div>

<div class="col-md-4">

<label>الخصم</label>

<asp:TextBox
ID="txtDiscount"
runat="server"
CssClass="form-control"
Text="0"
AutoPostBack="true"
OnTextChanged="CalculateTotal"/>

</div>

<div class="col-md-4">

<label>الإجمالي</label>

<asp:TextBox
ID="txtTotal"
runat="server"
CssClass="form-control bg-warning"
Font-Bold="true"
ReadOnly="true"/>

</div>

</div>

<hr />

<h4 class="text-success">

بيانات السداد

</h4>

<div class="row">

<div class="col-md-4">

<label>طريقة الدفع</label>

<asp:DropDownList
ID="ddlPaymentMethod"
runat="server"
CssClass="form-control">

<asp:ListItem Text="نقدي" Value="Cash"/>

<asp:ListItem Text="فيزا" Value="Visa"/>

<asp:ListItem Text="تحويل" Value="Transfer"/>

</asp:DropDownList>

</div>

<div class="col-md-4">

<label>المدفوع</label>

<asp:TextBox
ID="txtPaid"
runat="server"
CssClass="form-control"
Text="0"
AutoPostBack="true"
OnTextChanged="CalculateRemaining"/>

</div>

<div class="col-md-4">

<label>المتبقي</label>

<asp:TextBox
ID="txtRemaining"
runat="server"
CssClass="form-control bg-danger text-white fw-bold"
ReadOnly="true"/>

</div>

</div>

<br />

<div class="row">

<div class="col-md-4">

<label>حالة الدفع</label>

<asp:DropDownList
ID="ddlStatus"
runat="server"
CssClass="form-control">

<asp:ListItem Text="مدفوع" Value="Paid"/>

<asp:ListItem Text="غير مدفوع" Value="Unpaid"/>

<asp:ListItem Text="مدفوع جزئياً" Value="Partial"/>

</asp:DropDownList>

</div>

</div>

<hr />

<div class="text-center no-print">

<asp:Button
ID="btnSave"
runat="server"
Text="💾 حفظ الفاتورة"
CssClass="btn btn-success btn-lg"
OnClick="btnSave_Click"/>

<asp:Button
    ID="btnPrint"
    runat="server"
    Text="🖨 طباعة"
    CssClass="btn btn-primary btn-lg"
    OnClick="btnPrint_Click" />

</div>

<hr />

<div id="invoiceArea" class="invoice-box">

<h2 class="text-center">

مستشفى الرياض الصالحين

</h2>

<h4 class="text-center">

فاتورة عملية جراحية

</h4>

<table class="table table-bordered">

<tr>

<th>رقم العملية</th>

<td>

<asp:Label
ID="lblSurgery"
runat="server"/>

</td>

</tr>

<tr>

<th>اسم العملية</th>

<td>

<asp:Label
ID="lblSurgeryName"
runat="server"/>

</td>

</tr>

<tr>

<th>المريض</th>

<td>

<asp:Label
ID="lblPatient"
runat="server"/>

</td>

</tr>

<tr>

<th>الطبيب</th>

<td>

<asp:Label
ID="lblDoctor"
runat="server"/>

</td>

</tr>

<tr>

<th>الغرفة</th>

<td>

<asp:Label
ID="lblRoom"
runat="server"/>

</td>

</tr>

<tr>

<th>الإجمالي</th>

<td>

<asp:Label
ID="lblTotal"
runat="server"/>

</td>

</tr>

<tr>

<th>المدفوع</th>

<td>

<asp:Label
ID="lblPaid"
runat="server"/>

</td>

</tr>

<tr>

<th>المتبقي</th>

<td>

<asp:Label
ID="lblRemain"
runat="server"/>

</td>

</tr>

</table>

</div>

</div>

</div>

</div>

<style>

.invoice-box{

width:80mm;

margin:auto;

padding:10px;

background:white;

}

@media print{

.no-print{

display:none;

}

body{

background:white;

}

}

</style>

</asp:Content>
