<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="PatientsSurgeries.aspx.cs" Inherits="Riyadh_Al_Salehin.PatientsSurgeries" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    
<div class="container mt-4" dir="rtl">

    <!-- Quick search: search by patient number or name (auto-search) -->
    <div class="row mb-3">
        <div class="col-md-8">
            <asp:TextBox ID="txtSearchPatient" runat="server" CssClass="form-control"
                Placeholder="ابحث بالرقم أو بالاسم"
                AutoPostBack="true"
                OnTextChanged="txtSearchPatient_TextChanged" />
        </div>
        <div class="col-md-4">
            <asp:Button ID="btnContinue" runat="server" Text="➡ متابعة" CssClass="btn btn-primary mx-1" OnClick="btnContinue_Click" />
        </div>
    </div>

    <div class="card shadow">
        <div class="card-header bg-primary text-white">
            <h4>إدارة المرضى العمليات</h4>
        </div>

        <div class="card-body">

            <asp:HiddenField ID="hfId" runat="server" />

            <div class="row">

                <div class="col-md-6 mb-3">
                    <label>اسم المريض</label>
                    <asp:TextBox ID="txtPatientName" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>العنوان</label>
                    <asp:TextBox ID="txtAddress" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>تاريخ الميلاد</label>
                    <asp:TextBox ID="txtDateOfBirth" runat="server"
                        TextMode="Date"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>شركة التأمين</label>
                    <asp:TextBox ID="txtInsuranceCompany" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>رقم التأمين</label>
                    <asp:TextBox ID="txtInsuranceNumber" runat="server"
                        CssClass="form-control"></asp:TextBox>
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
            قائمة المرضى
        </div>

        <div class="card-body">

            <div class="row mb-3">

                <div class="col-md-9">
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="form-control"
                        placeholder="ابحث باسم المريض أو الهاتف"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <asp:Button ID="btnSearch" runat="server"
                        Text="بحث"
                        CssClass="btn btn-primary w-100"
                        OnClick="btnSearch_Click" />
                </div>

            </div>

            <asp:GridView ID="gvPatients"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                DataKeyNames="Id"
                OnSelectedIndexChanged="gvPatients_SelectedIndexChanged">

                <Columns>

                    <asp:BoundField DataField="Id"
                        HeaderText="م" />

                    <asp:BoundField DataField="PatientName"
                        HeaderText="اسم المريض" />

                    <asp:BoundField DataField="Phone"
                        HeaderText="الهاتف" />

                    <asp:BoundField DataField="InsuranceCompany"
                        HeaderText="شركة التأمين" />

                    <asp:CommandField
                        ShowSelectButton="True"
                        SelectText="اختيار" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>









</asp:Content>
