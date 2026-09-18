<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="SurgerySupplies.aspx.cs" Inherits="Riyadh_Al_Salehin.SurgerySupplies" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-info text-white">
                <h5 class="mb-0">🧰 مستلزمات العمليات الجراحية</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>العملية (Surgery ID)</label>
                        <asp:TextBox ID="txtSurgeryId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المستلزم (Supply ID)</label>
                        <asp:TextBox ID="txtSupplyId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الكمية المستخدمة</label>
                        <asp:TextBox ID="txtUsedQuantity" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>سعر الوحدة</label>
                        <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الإجمالي</label>
                        <asp:TextBox ID="txtTotalPrice" runat="server" CssClass="form-control" />
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
                <h6 class="mb-0">📋 مستلزمات العمليات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvSupplies" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvSupplies_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="SurgeryName" HeaderText="العملية" />
                        <asp:BoundField DataField="SupplyName" HeaderText="المستلزم" />
                        <asp:BoundField DataField="UsedQuantity" HeaderText="الكمية" />
                        <asp:BoundField DataField="UnitPrice" HeaderText="السعر" />
                        <asp:BoundField DataField="TotalPrice" HeaderText="الإجمالي" />

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
                                    OnClientClick="return confirm('هل تريد حذف السجل؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>
