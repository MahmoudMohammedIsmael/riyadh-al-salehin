<%@ Page Title="إدارة طلبات الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayRequests.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayRequests" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <!-- بطاقة الإدخال -->
        <div class="card shadow-sm">


            <asp:HiddenField ID="HiddenField1" runat="server" />
    
    <!-- أضف هذا السطر لعرض رسالة الحالة/التوجيه -->
    <asp:Label ID="lblInfo" runat="server" CssClass="alert alert-info w-100 d-block" Visible="false" />





            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">📋 إضافة / تعديل طلب أشعة</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>المريض</label>
                        <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-control" DataTextField="PatientName" DataValueField="Id">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label>خدمة الأشعة</label>
                        <asp:DropDownList ID="ddlXrayService" runat="server" CssClass="form-control" DataTextField="XrayName" DataValueField="Id">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label>الحالة</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Pending">قيد الانتظار</asp:ListItem>
                            <asp:ListItem Value="Completed">مكتمل</asp:ListItem>
                            <asp:ListItem Value="Cancelled">ملغي</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>رقم الإحالة (اختياري)</label>
                        <asp:TextBox ID="txtReferralId" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="mt-3">

                    <asp:Button ID="btnSave" runat="server" Text="💾 حفظ" CssClass="btn btn-success" OnClick="btnSave_Click" />
                    <asp:Button ID="btnNew" runat="server" Text="🆕 جديد" CssClass="btn btn-secondary" OnClick="btnNew_Click" />

                </div>

            </div>
        </div>

        <!-- بطاقة عرض البيانات -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة طلبات الأشعة</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvRequests" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvRequests_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="XrayName" HeaderText="الخدمة" />
                        <asp:BoundField DataField="Status" HeaderText="الحالة" />
                        <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ الطلب" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

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
                                    OnClientClick="return confirm('هل تريد حذف هذا الطلب؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>