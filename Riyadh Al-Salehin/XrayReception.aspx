<%@ Page Title="استقبال طلبات الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayReception.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayReception" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">
            <div class="card-header bg-info text-white">
                <h5 class="mb-0">💰 استقبال طلبات الأشعة (الفاتورة والطباعة)</h5>
            </div>
            <div class="card-body">

                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success w-100" Visible="false" />

                <asp:GridView ID="gvRequests" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvRequests_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="رقم الطلب" />
                        <asp:BoundField DataField="PatientName" HeaderText="اسم المريض" />
                        <asp:BoundField DataField="XrayName" HeaderText="نوع الخدمة" />
                        <asp:BoundField DataField="Price" HeaderText="السعر" DataFormatString="{0:N2} ر.س" />
                        <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ الطلب" DataFormatString="{0:yyyy-MM-dd}" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>
                                <asp:Button ID="btnPrint" runat="server" Text="🖨️ طباعة الفاتورة"
                                    CssClass="btn btn-secondary btn-sm"
                                    CommandName="PrintInvoice"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnConfirm" runat="server" Text="✅ تأكيد الدفع"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="ConfirmPayment"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تم استلام المبلغ بالكامل؟');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>