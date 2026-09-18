<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="CommissionRules.aspx.cs" Inherits="Riyadh_Al_Salehin.CommissionRules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-success text-white">
                <h5 class="mb-0">💰 قواعد العمولات (Commission Rules)</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>الطبيب ID</label>
                        <asp:TextBox ID="txtDoctorId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المركز ID</label>
                        <asp:TextBox ID="txtCenterId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>نوع الخدمة</label>
                        <asp:TextBox ID="txtServiceType" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-6">
                        <label>نسبة الطبيب (%)</label>
                        <asp:TextBox ID="txtDoctorShare" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label>نسبة المركز (%)</label>
                        <asp:TextBox ID="txtCenterShare" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="mt-3">

                    <asp:Button ID="btnSave" runat="server" Text="💾 حفظ"
                        CssClass="btn btn-success"
                        OnClick="btnSave_Click" />

                    <asp:Button ID="btnNew" runat="server" Text="🆕 جديد"
                        CssClass="btn btn-secondary"
                        OnClick="btnNew_Click" />

                </div>

            </div>
        </div>

        <!-- GRID -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قواعد العمولات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvCommission" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvCommission_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                        <asp:BoundField DataField="CenterName" HeaderText="المركز" />
                        <asp:BoundField DataField="ServiceType" HeaderText="نوع الخدمة" />
                        <asp:BoundField DataField="DoctorShare" HeaderText="نسبة الطبيب" />
                        <asp:BoundField DataField="CenterShare" HeaderText="نسبة المركز" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل النسب"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تريد حذف القاعدة؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>