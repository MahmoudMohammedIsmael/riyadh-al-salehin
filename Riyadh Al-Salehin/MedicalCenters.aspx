<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="MedicalCenters.aspx.cs" Inherits="Riyadh_Al_Salehin.MedicalCenters" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    
<div class="container mt-4" dir="rtl">

    <div class="card shadow">
        <div class="card-header bg-primary text-white">
            <h4>إدارة المراكز الطبية</h4>
        </div>

        <div class="card-body">

            <asp:HiddenField ID="hfId" runat="server" />

            <div class="row">

                <div class="col-md-6 mb-3">
                    <label>اسم المركز</label>
                    <asp:TextBox ID="txtCenterName"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>نسبة العمولة %</label>
                    <asp:TextBox ID="txtCommissionRate"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>
                </div>

                <div class="col-md-12 mb-3">
                    <label>العنوان</label>
                    <asp:TextBox ID="txtAddress"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="3"
                        CssClass="form-control">
                    </asp:TextBox>
                </div>

            </div>

            <div class="text-center mb-3">

                <asp:Button ID="btnSave"
                    runat="server"
                    Text="حفظ"
                    CssClass="btn btn-success"
                    OnClick="btnSave_Click" />

                <asp:Button ID="btnUpdate"
                    runat="server"
                    Text="تعديل"
                    CssClass="btn btn-warning"
                    OnClick="btnUpdate_Click" />

                <asp:Button ID="btnDelete"
                    runat="server"
                    Text="حذف"
                    CssClass="btn btn-danger"
                    OnClick="btnDelete_Click"
                    OnClientClick="return confirm('هل تريد الحذف؟');" />

                <asp:Button ID="btnNew"
                    runat="server"
                    Text="جديد"
                    CssClass="btn btn-secondary"
                    OnClick="btnNew_Click" />

            </div>

            <div class="text-center">
                <asp:Label ID="lblMessage"
                    runat="server"
                    Font-Bold="true">
                </asp:Label>
            </div>

        </div>
    </div>

    <br />

    <div class="card shadow">

        <div class="card-header bg-dark text-white">
            البحث
        </div>

        <div class="card-body">

            <div class="row">

                <div class="col-md-10">
                    <asp:TextBox ID="txtSearch"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnSearch"
                        runat="server"
                        Text="بحث"
                        CssClass="btn btn-primary w-100"
                        OnClick="btnSearch_Click" />
                </div>

            </div>

            <hr />

            <asp:GridView ID="gvCenters"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                DataKeyNames="Id"
                OnSelectedIndexChanged="gvCenters_SelectedIndexChanged">

                <Columns>

                    <asp:BoundField DataField="Id"
                        HeaderText="الكود" />

                    <asp:BoundField DataField="CenterName"
                        HeaderText="اسم المركز" />

                    <asp:BoundField DataField="Phone"
                        HeaderText="الهاتف" />

                    <asp:BoundField DataField="CommissionRate"
                        HeaderText="العمولة %" />

                    <asp:CommandField
                        ShowSelectButton="True"
                        SelectText="اختيار" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>









</asp:Content>
