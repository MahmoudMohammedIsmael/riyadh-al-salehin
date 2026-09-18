<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="SurgeryAccounts.aspx.cs" Inherits="Riyadh_Al_Salehin.SurgeryAccounts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-dark text-white">
                <h5 class="mb-0">💰 حسابات العمليات الجراحية</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>العملية (Surgery ID)</label>
                        <asp:TextBox ID="txtSurgeryId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المريض (Patient ID)</label>
                        <asp:TextBox ID="txtPatientId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>إجمالي التكلفة</label>
                       <asp:TextBox
ID="txtTotalCost"
runat="server"
CssClass="form-control"
ReadOnly="true" />
                        
                    
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>المقدم</label>
                        <asp:TextBox ID="txtAdvancePayment" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المتبقي</label>
                        <asp:TextBox ID="txtRemainingAmount" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>حالة الدفع</label>
                        <asp:TextBox ID="txtPaymentStatus" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

    <div class="col-md-6">
        <label>غرفة العمليات</label>
        <asp:TextBox ID="txtRoomName"
            runat="server"
            CssClass="form-control"
            ReadOnly="true" />
    </div>

    <div class="col-md-6">
        <label>تكلفة غرفة العمليات</label>
        <asp:TextBox ID="txtRoomCost"
            runat="server"
            CssClass="form-control"
            ReadOnly="true" />
    </div>

</div>


                <div class="row mt-2">

                    <div class="col-md-6">
                        <label>عمولة الطبيب</label>
                       <asp:TextBox
ID="txtDoctorCommission"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnTextChanged="CostChanged" />

  
                    
                    </div>

                    <div class="col-md-6">
                        <label>عمولة المركز</label>
                        <asp:TextBox
ID="txtCenterCommission"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnTextChanged="CostChanged" />        </div>

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

            <div class="card-header bg-primary text-white">
                <h6 class="mb-0">📋 حسابات العمليات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvAccounts" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvAccounts_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="SurgeryName" HeaderText="العملية" />

                        <asp:BoundField DataField="TotalCost" HeaderText="الإجمالي" />
                        <asp:BoundField DataField="AdvancePayment" HeaderText="المقدم" />
                        <asp:BoundField DataField="RemainingAmount" HeaderText="المتبقي" />

                        <asp:BoundField DataField="PaymentStatus" HeaderText="الحالة" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل الدفع"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تريد حذف الحساب؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>
