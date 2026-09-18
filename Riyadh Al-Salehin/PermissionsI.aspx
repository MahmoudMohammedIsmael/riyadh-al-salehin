<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="PermissionsI.aspx.cs" Inherits="Riyadh_Al_Salehin.PermissionsI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>

body{
    background:#f4f7fb;
}

.page-title{
    font-size:28px;
    font-weight:bold;
    color:#0d6efd;
}

.card{
    border:none;
    border-radius:15px;
    box-shadow:0 5px 20px rgba(0,0,0,.10);
}

.form-control{
    border-radius:10px;
}

.btn{
    border-radius:10px;
    font-weight:bold;
    min-width:120px;
}

.grid{
    margin-top:20px;
}

.grid th{
    background:#0d6efd;
    color:#fff;
    text-align:center;
}

.grid td{
    text-align:center;
    vertical-align:middle;
}

.searchBox{
    width:320px;
}

</style>

<div class="container-fluid mt-4">

<div class="card">

<div class="card-header bg-primary text-white">

<div class="row">

<div class="col-md-6">

<h3 class="page-title text-white">

<i class="fa fa-lock"></i>

إدارة الصلاحيات

</h3>

</div>

<div class="col-md-6 text-end">

<asp:Label ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</div>

<div class="card-body">

<asp:HiddenField
ID="hfId"
runat="server" />

<div class="row">

<div class="col-md-4">

<label>اسم الشاشة (Module)</label>

<asp:TextBox
ID="txtModule"
runat="server"
CssClass="form-control"
placeholder="مثال : Patients">
</asp:TextBox>

</div>

<div class="col-md-4">

<label>نوع العملية (Action)</label>

<asp:DropDownList
ID="ddlAction"
runat="server"
CssClass="form-control">

<asp:ListItem Text="عرض" Value="View"></asp:ListItem>

<asp:ListItem Text="إضافة" Value="Add"></asp:ListItem>

<asp:ListItem Text="تعديل" Value="Edit"></asp:ListItem>

<asp:ListItem Text="حذف" Value="Delete"></asp:ListItem>

<asp:ListItem Text="طباعة" Value="Print"></asp:ListItem>

<asp:ListItem Text="اعتماد" Value="Approve"></asp:ListItem>

<asp:ListItem Text="إلغاء" Value="Cancel"></asp:ListItem>

</asp:DropDownList>

</div>

<div class="col-md-4">

<label>الوصف</label>

<asp:TextBox
ID="txtDescription"
runat="server"
CssClass="form-control"
placeholder="وصف الصلاحية">
</asp:TextBox>

</div>

</div>

<hr />

<div class="text-center">

<asp:Button
ID="btnNew"
runat="server"
Text="جديد"
CssClass="btn btn-secondary"
OnClick="btnNew_Click" />

&nbsp;

<asp:Button
ID="btnSave"
runat="server"
Text="حفظ"
CssClass="btn btn-success"
OnClick="btnSave_Click" />

&nbsp;

<asp:Button
ID="btnUpdate"
runat="server"
Text="تعديل"
CssClass="btn btn-warning"
OnClick="btnUpdate_Click" />

&nbsp;

<asp:Button
ID="btnDelete"
runat="server"
Text="حذف"
CssClass="btn btn-danger"
OnClick="btnDelete_Click"
OnClientClick="return confirm('هل تريد حذف هذه الصلاحية ؟');" />

</div>

<hr />

<div class="row">

<div class="col-md-6">

<asp:TextBox
ID="txtSearch"
runat="server"
CssClass="form-control searchBox"
placeholder="بحث باسم الشاشة..."
AutoPostBack="true"
OnTextChanged="txtSearch_TextChanged">
</asp:TextBox>

</div>

</div>
    <div class="grid">

    <asp:GridView
        ID="gvPermissions"
        runat="server"
        CssClass="table table-bordered table-hover"
        AutoGenerateColumns="False"
        DataKeyNames="Id"
        OnSelectedIndexChanged="gvPermissions_SelectedIndexChanged"
        OnRowDataBound="gvPermissions_RowDataBound">

        <Columns>

            <asp:BoundField
                DataField="Id"
                HeaderText="م">
                <ItemStyle Width="60px" />
            </asp:BoundField>

            <asp:BoundField
                DataField="Module"
                HeaderText="اسم الشاشة">
                <ItemStyle Width="220px" />
            </asp:BoundField>

            <asp:BoundField
                DataField="Action"
                HeaderText="نوع العملية">
                <ItemStyle Width="150px" />
            </asp:BoundField>

            <asp:BoundField
                DataField="Description"
                HeaderText="الوصف" />

            <asp:CommandField
                ShowSelectButton="True"
                SelectText="اختيار">
                <ItemStyle Width="90px" />
            </asp:CommandField>

        </Columns>

        <HeaderStyle CssClass="table-primary" />

        <EmptyDataTemplate>

            <div class="alert alert-warning text-center m-3">

                <i class="fa fa-info-circle"></i>

                لا توجد صلاحيات مسجلة حتى الآن

            </div>

        </EmptyDataTemplate>

    </asp:GridView>

</div>

</div>

</div>

</div>

</asp:Content>