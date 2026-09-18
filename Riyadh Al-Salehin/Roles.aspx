<%@ Page Title="إدارة الأدوار" Language="C#" MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true" CodeBehind="Roles.aspx.cs"
    Inherits="Riyadh_Al_Salehin.Roles" %>

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
    box-shadow:0 5px 20px rgba(0,0,0,.1);
}

.form-control{
    border-radius:10px;
}

textarea{
    resize:none;
}

.btn{
    border-radius:10px;
    font-weight:bold;
}

.grid{
    margin-top:20px;
}

.grid th{
    background:#0d6efd;
    color:white;
    text-align:center;
}

.grid td{
    text-align:center;
    vertical-align:middle;
}

.searchBox{
    width:300px;
    float:left;
}

.permModule{
    border-bottom:1px solid #e9ecef;
    padding:12px 0;
}

.permModule h6{
    color:#0d6efd;
    font-weight:bold;
    margin-bottom:10px;
}
</style>

<div class="container-fluid mt-4">

<div class="card">

<div class="card-header bg-primary text-white">

<div class="row">

<div class="col-md-6">

<h3 class="page-title text-white">
<i class="fa fa-user-shield"></i>
إدارة الأدوار والصلاحيات
</h3>

</div>

<div class="col-md-6 text-end">

<asp:Label
ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</div>

<div class="card-body">

<div class="row">

<div class="col-md-6">

<label>اسم الدور</label>

<asp:TextBox
ID="txtName"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-6">

<label>الوصف</label>

<asp:TextBox
ID="txtDescription"
runat="server"
CssClass="form-control"
TextMode="MultiLine"
Rows="2">
</asp:TextBox>

</div>

</div>

<asp:HiddenField
ID="hfId"
runat="server" />

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
OnClientClick="return confirm('هل تريد حذف الدور؟');" />

</div>

<hr />

<div class="row">

<div class="col-md-6">

<asp:TextBox
ID="txtSearch"
runat="server"
CssClass="form-control searchBox"
placeholder="بحث باسم الدور..."
AutoPostBack="true"
OnTextChanged="txtSearch_TextChanged">
</asp:TextBox>

</div>

</div>

<div class="grid">

<asp:GridView
ID="gvRoles"
runat="server"
CssClass="table table-bordered table-hover"
AutoGenerateColumns="False"
DataKeyNames="Id"
OnSelectedIndexChanged="gvRoles_SelectedIndexChanged"
OnRowDataBound="gvRoles_RowDataBound">

<Columns>

<asp:BoundField
DataField="Id"
HeaderText="م" />

<asp:BoundField
DataField="Name"
HeaderText="اسم الدور" />

<asp:BoundField
DataField="Description"
HeaderText="الوصف" />

<asp:BoundField
DataField="CreatedAt"
HeaderText="تاريخ الإنشاء"
DataFormatString="{0:yyyy-MM-dd}" />

<asp:CommandField
ShowSelectButton="True"
SelectText="اختيار" />

</Columns>

<HeaderStyle CssClass="table-primary" />

</asp:GridView>

</div>

</div>

</div>

<!-- ============ كارت الصلاحيات (يظهر بعد اختيار دور) ============ -->
<asp:Panel ID="pnlPermissions" runat="server" Visible="false" CssClass="mt-4">
    <div class="card">
        <div class="card-header bg-info text-white d-flex justify-content-between align-items-center">
            <span>
                <i class="fa fa-key"></i>
                صلاحيات الدور: <asp:Label ID="lblSelectedRoleName" runat="server" Font-Bold="true"></asp:Label>
            </span>
            <asp:Button ID="btnSavePermissions" runat="server" Text="حفظ الصلاحيات"
                CssClass="btn btn-light btn-sm" OnClick="btnSavePermissions_Click" CausesValidation="false" />
        </div>
        <div class="card-body" style="max-height:500px; overflow-y:auto;">

            <asp:Repeater ID="rptModules" runat="server" OnItemDataBound="rptModules_ItemDataBound">
                <ItemTemplate>
                    <div class="permModule">
                        <h6><i class="fa fa-folder-open"></i> <%# Eval("Module") %></h6>
                        <asp:CheckBoxList ID="cblActions" runat="server"
                            RepeatDirection="Horizontal" RepeatColumns="4" CssClass="ps-3">
                        </asp:CheckBoxList>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoPermissions" runat="server" CssClass="text-muted"
                Text="لا توجد صلاحيات معرفة في النظام بعد." Visible="false">
            </asp:Label>

        </div>
    </div>
</asp:Panel>

</div>

</asp:Content>
