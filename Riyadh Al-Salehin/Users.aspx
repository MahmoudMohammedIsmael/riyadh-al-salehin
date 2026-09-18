<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true"
    CodeBehind="Users.aspx.cs" Inherits="Riyadh_Al_Salehin.Users" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
body{ background:#f4f7fb; }
.page-title{ font-size:28px; font-weight:bold; color:#0d6efd; }
.card{ border:none; border-radius:15px; box-shadow:0 5px 20px rgba(0,0,0,.1); }
.form-control, .form-select{ border-radius:10px; }
.btn{ border-radius:10px; font-weight:bold; }
.grid{ margin-top:20px; }
.grid th{ background:#0d6efd; color:white; text-align:center; }
.grid td{ text-align:center; vertical-align:middle; }
.searchBox{ width:300px; float:left; }
</style>

<div class="container-fluid mt-4">

<div class="card">

    <div class="card-header bg-primary text-white">
        <div class="row">
            <div class="col-md-6">
                <h3 class="page-title text-white">
                    <i class="fa fa-users"></i> إدارة المستخدمين
                </h3>
            </div>
            <div class="col-md-6 text-end">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
            </div>
        </div>
    </div>

    <div class="card-body">

        <div class="row">
            <div class="col-md-4">
                <label>الاسم الكامل *</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <label>اسم المستخدم *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <label>كلمة المرور <small class="text-muted">(اتركها فارغة عند التعديل للإبقاء عليها)</small></label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="255"></asp:TextBox>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-md-4">
                <label>البريد الإلكتروني</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <label>رقم الهاتف</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <label>الدور *</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select"
                    DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- اختر الدور --" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-md-4">
                <label>الطبيب المرتبط <small class="text-muted">(اختياري)</small></label>
                <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select"
                    DataTextField="DoctorName" DataValueField="Id" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- بدون --" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>المركز الطبي <small class="text-muted">(اختياري)</small></label>
                <asp:DropDownList ID="ddlCenter" runat="server" CssClass="form-select"
                    DataTextField="CenterName" DataValueField="Id" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- بدون --" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-4 d-flex align-items-end">
                <div class="form-check">
                    <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" Checked="true" />
                    <label class="form-check-label">حساب مُفعّل</label>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hfId" runat="server" />

        <hr />

        <div class="text-center">
            <asp:Button ID="btnNew" runat="server" Text="جديد" CssClass="btn btn-secondary" OnClick="btnNew_Click" CausesValidation="false" />
            &nbsp;
            <asp:Button ID="btnSave" runat="server" Text="حفظ" CssClass="btn btn-success" OnClick="btnSave_Click" />
            &nbsp;
            <asp:Button ID="btnUpdate" runat="server" Text="تعديل" CssClass="btn btn-warning" OnClick="btnUpdate_Click" />
            &nbsp;
            <asp:Button ID="btnDelete" runat="server" Text="حذف" CssClass="btn btn-danger"
                OnClick="btnDelete_Click" OnClientClick="return confirm('هل تريد حذف هذا المستخدم؟');" />
        </div>

        <hr />

        <div class="row">
            <div class="col-md-6">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control searchBox"
                    placeholder="بحث بالاسم أو اسم المستخدم..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
            </div>
        </div>

        <div class="grid">
            <asp:GridView ID="gvUsers" runat="server" CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False" DataKeyNames="Id"
                OnSelectedIndexChanged="gvUsers_SelectedIndexChanged"
                OnRowDataBound="gvUsers_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="م" />
                    <asp:BoundField DataField="FullName" HeaderText="الاسم الكامل" />
                    <asp:BoundField DataField="Username" HeaderText="اسم المستخدم" />
                    <asp:BoundField DataField="RoleName" HeaderText="الدور" />
                    <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                    <asp:BoundField DataField="CenterName" HeaderText="المركز" />
                    <asp:CheckBoxField DataField="IsActive" HeaderText="مُفعّل" />
                    <asp:BoundField DataField="LastLogin" HeaderText="آخر دخول" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:CommandField ShowSelectButton="True" SelectText="اختيار" />
                </Columns>
                <HeaderStyle CssClass="table-primary" />
            </asp:GridView>
        </div>

    </div>
</div>
</div>

</asp:Content>
