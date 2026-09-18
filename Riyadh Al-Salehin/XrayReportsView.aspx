<%@ Page Title="تقارير الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayReportsView.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayReportsView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">
        <div class="card shadow-sm">
            <div class="card-header bg-dark text-white">
                <h5 class="mb-0">📋 تقارير الأشعة الخاصة بالمريض</h5>
            </div>
            <div class="card-body">

                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info w-100" Visible="false" />

                <asp:GridView ID="gvReports" runat="server"
                    CssClass="table table-bordered table-hover"
                    AutoGenerateColumns="False"
                    EmptyDataText="لا توجد تقارير أشعة لهذا المريض">
                    <Columns>
                        <asp:BoundField DataField="XrayName" HeaderText="نوع الأشعة" />
                        <asp:BoundField DataField="RequestDate" HeaderText="تاريخ الطلب" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب الطالب" />
                        <asp:BoundField DataField="ResultText" HeaderText="نص التقرير" />
                        <asp:TemplateField HeaderText="الملف">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkFile" runat="server"
                                    NavigateUrl='<%# Eval("FilePath") %>'
                                    Text="عرض الملف"
                                    Target="_blank"
                                    Visible='<%# Eval("FilePath") != DBNull.Value && !string.IsNullOrEmpty(Eval("FilePath").ToString()) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:Button ID="btnBack" runat="server" Text="🔙 رجوع" CssClass="btn btn-secondary mt-3" OnClick="btnBack_Click" />

            </div>
        </div>
    </div>

</asp:Content>