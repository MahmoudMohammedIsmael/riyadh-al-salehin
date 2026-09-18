<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="PerformedExaminationProcedures.aspx.cs" Inherits="Riyadh_Al_Salehin.PerformedExaminationProcedures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-info text-white">
                <h5 class="mb-0">🧪 إجراءات الفحص المنفذة</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>الفحص الطبي (Examination ID)</label>
                        <asp:TextBox ID="txtExaminationId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الخدمة (Service ID)</label>
                        <asp:TextBox ID="txtServiceId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>السعر</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-12">
                        <label>ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" />
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
                <h6 class="mb-0">📋 قائمة الإجراءات المنفذة</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvProcedures" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvProcedures_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="InitialDiagnosis" HeaderText="التشخيص" />
                        <asp:BoundField DataField="ServiceName" HeaderText="الخدمة" />
                        <asp:BoundField DataField="Price" HeaderText="السعر" />
                        <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تريد حذف الإجراء؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>