<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="MedicalServices.aspx.cs" Inherits="Riyadh_Al_Salehin.MedicalServices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    /* ===========================
   Page Title Bar (موحّد لكل الشاشات)
=========================== */
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

.page-title-bar h4,
.page-title-bar span {
    color: #ffffff !important;
    font-size: 20px !important;
    font-weight: 700 !important;
    margin: 0 !important;
    line-height: 1.4 !important;
}

.page-title-bar i {
    font-size: 22px;
    color: #ffffff;
}

.page-title-bar.dark {
    background: linear-gradient(135deg, #212529, #343a40);
    box-shadow: 0 4px 12px rgba(0,0,0,.2);
}
</style>  

<div class="container mt-4" dir="rtl">

  <div class="card shadow">
    <div class="page-title-bar">
        <i class="bi bi-heart-pulse-fill"></i>
        <h4>إدارة الخدمات الطبية</h4>
    </div>

    <div class="card-body">

        <asp:HiddenField ID="hfId" runat="server" />

        <div class="row">

            <div class="col-md-6 mb-3">
                <label>اسم الخدمة</label>
                <asp:TextBox ID="txtServiceName" runat="server"
                    CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-6 mb-3">
                <label>قيمة الطبيب</label>
                <asp:TextBox ID="txtDoctorAmount" runat="server"
                    CssClass="form-control"
                    TextMode="Number" step="0.01"></asp:TextBox>
            </div>

            <div class="col-md-6 mb-3">
                <label>قيمة المركز</label>
                <asp:TextBox ID="txtCenterAmount" runat="server"
                    CssClass="form-control"
                    TextMode="Number" step="0.01"></asp:TextBox>
            </div>

            <div class="col-md-6 mb-3">
                <label>الإجمالي (محسوب)</label>
                <asp:TextBox ID="txtTotalAmount" runat="server"
                    CssClass="form-control"
                    ReadOnly="true"
                    BackColor="#f0f0f0"></asp:TextBox>
            </div>

            <div class="col-md-6 mb-3">
                <label>الحالة</label>
                <asp:CheckBox ID="chkIsActive" runat="server"
                    Text="مفعل"
                    Checked="true"
                    CssClass="form-check-input" />
                <span style="margin-right:10px;">(مفعل)</span>
            </div>

        </div>

        <asp:Label ID="lblMessage" runat="server"
            Font-Bold="true"></asp:Label>

        <hr />

        <asp:Button ID="btnSave" runat="server"
            Text="حفظ"
            CssClass="btn btn-success"
            OnClick="btnSave_Click" />

        <asp:Button ID="btnUpdate" runat="server"
            Text="تعديل"
            CssClass="btn btn-warning"
            OnClick="btnUpdate_Click" />

        <asp:Button ID="btnDelete" runat="server"
            Text="حذف"
            CssClass="btn btn-danger"
            OnClick="btnDelete_Click" />

        <asp:Button ID="btnNew" runat="server"
            Text="جديد"
            CssClass="btn btn-secondary"
            OnClick="btnNew_Click" />

    </div>
  </div>

  <br />

  <div class="card shadow">

    <div class="card-header bg-dark text-white">
        قائمة الخدمات الطبية
    </div>

    <div class="card-body">

        <div class="row mb-3">

            <div class="col-md-9">
                <asp:TextBox ID="txtSearch" runat="server"
                    CssClass="form-control"
                    placeholder="ابحث باسم الخدمة"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <asp:Button ID="btnSearch" runat="server"
                    Text="بحث"
                    CssClass="btn btn-primary w-100"
                    OnClick="btnSearch_Click" />
            </div>

        </div>

        <asp:GridView ID="gvServices"
            runat="server"
            CssClass="table table-bordered table-hover"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            OnSelectedIndexChanged="gvServices_SelectedIndexChanged">

            <Columns>

                <asp:BoundField DataField="Id"
                    HeaderText="م" />

                <asp:BoundField DataField="ServiceName"
                    HeaderText="اسم الخدمة" />

                <asp:BoundField DataField="DoctorAmount"
                    HeaderText="قيمة الطبيب"
                    DataFormatString="{0:N2}" />

                <asp:BoundField DataField="CenterAmount"
                    HeaderText="قيمة المركز"
                    DataFormatString="{0:N2}" />

                <asp:BoundField DataField="TotalAmount"
                    HeaderText="الإجمالي"
                    DataFormatString="{0:N2}" />

                <asp:CheckBoxField DataField="IsActive"
                    HeaderText="مفعل"
                    SortExpression="IsActive" />

                <asp:CommandField
                    ShowSelectButton="True"
                    SelectText="اختيار" />

            </Columns>

        </asp:GridView>

    </div>

  </div>

</div>

</asp:Content>