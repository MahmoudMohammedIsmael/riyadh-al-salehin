<%@ Page Title="إدارة خدمات الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayServices.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayServices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-success text-white">
                <h5 class="mb-0">⚙️ إدارة خدمات الأشعة</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>اسم الخدمة</label>
                        <asp:TextBox ID="txtXrayName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>السعر</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الوصف</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text="مفعل" Checked="true" />
                    </div>

                </div>

                <div class="mt-3">

                    <asp:Button ID="btnSave" runat="server" Text="💾 حفظ" CssClass="btn btn-success" OnClick="btnSave_Click" />
                    <asp:Button ID="btnNew" runat="server" Text="🆕 جديد" CssClass="btn btn-secondary" OnClick="btnNew_Click" />

                </div>

            </div>
        </div>

        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة الخدمات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvServices" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvServices_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="XrayName" HeaderText="الاسم" />
                        <asp:BoundField DataField="Price" HeaderText="السعر" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Description" HeaderText="الوصف" />
                        <asp:CheckBoxField DataField="IsActive" HeaderText="مفعل" />

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
                                    OnClientClick="return confirm('هل تريد حذف الخدمة؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>