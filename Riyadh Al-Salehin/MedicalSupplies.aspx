<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="MedicalSupplies.aspx.cs" Inherits="Riyadh_Al_Salehin.MedicalSupplies" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-success text-white">
                <h5 class="mb-0">💊 إدارة المستلزمات الطبية</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>اسم المستلزم</label>
                        <asp:TextBox ID="txtSupplyName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الوحدة</label>
                        <asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>السعر</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>الكمية الأساسية (المبدئية)</label>
                        <asp:TextBox ID="txtInitialQty" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الكمية المتاحة حالياً</label>
                        <asp:TextBox ID="txtQty" runat="server" CssClass="form-control" />
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

        <!-- Grid -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة المستلزمات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvSupplies" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvSupplies_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="SupplyName" HeaderText="المستلزم" />
                        <asp:BoundField DataField="Unit" HeaderText="الوحدة" />
                        <asp:BoundField DataField="Price" HeaderText="السعر" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="InitialQuantity" HeaderText="الكمية الأساسية" />
                        <asp:BoundField DataField="AvailableQuantity" HeaderText="الكمية المتاحة" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="تاريخ الإضافة" DataFormatString="{0:yyyy/MM/dd HH:mm}" />

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
                                    OnClientClick="return confirm('هل تريد الحذف؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>