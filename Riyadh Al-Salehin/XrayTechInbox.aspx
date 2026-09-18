<%@ Page Title="صندوق وارد الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayTechInbox.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayTechInbox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">
            <div class="card-header bg-warning text-dark">
                <h5 class="mb-0">📩 طلبات الأشعة الواردة (صندوق الرسائل)</h5>
            </div>
            <div class="card-body">

                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info w-100" Visible="false" />

                <div class="row mb-3">
                    <div class="col-md-3">
                        <asp:DropDownList ID="ddlFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                            <asp:ListItem Value="Pending">⏳ قيد الانتظار</asp:ListItem>
                            <asp:ListItem Value="InProgress">⚙️ قيد التنفيذ</asp:ListItem>
                            <asp:ListItem Value="All">📋 الجميع</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <asp:GridView ID="gvTechRequests" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvTechRequests_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="رقم الطلب" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="XrayName" HeaderText="الخدمة" />
                        <asp:BoundField DataField="Status" HeaderText="الحالة" />
                        <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ الطلب" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>
                                <asp:Button ID="btnStart" runat="server" Text="▶️ بدء التنفيذ"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="StartProcessing"
                                    CommandArgument='<%# Eval("Id") %>'
                                    Visible='<%# Eval("Status").ToString() == "Pending" %>' />

                                <asp:Button ID="btnResult" runat="server" Text="📝 رفع النتيجة"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="EnterResult"
                                    CommandArgument='<%# Eval("Id") %>'
                                    Visible='<%# Eval("Status").ToString() == "InProgress" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>