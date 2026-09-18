<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="LabRequests.aspx.cs" Inherits="Riyadh_Al_Salehin.LabRequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-secondary text-white">
                <h5 class="mb-0">🧪 طلبات التحاليل (Lab Requests)</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>التحويل (Referral ID)</label>
                        <asp:TextBox ID="txtReferralId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المريض (Patient ID)</label>
                        <asp:TextBox ID="txtPatientId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>نوع التحليل</label>
                        <asp:TextBox ID="txtTestType" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>الحالة</label>
                        <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" />
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
                <h6 class="mb-0">📋 قائمة طلبات التحاليل</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvLabRequests" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvLabRequests_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="TestType" HeaderText="نوع التحليل" />
                        <asp:BoundField DataField="Status" HeaderText="الحالة" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل الحالة"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تريد حذف الطلب؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>