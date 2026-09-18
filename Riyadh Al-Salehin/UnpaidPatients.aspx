<%@ Page Title="استعلام الفواتير غير المدفوعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="UnpaidPatients.aspx.cs" Inherits="Riyadh_Al_Salehin.UnpaidPatients" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .headerCard {
            background: linear-gradient(45deg,#dc3545, #ff6b6b);
            color: white;
            border-radius: 15px;
            padding: 20px;
            margin-top: 20px;
        }
        .searchCard {
            background: white;
            border-radius: 12px;
            padding: 15px;
            margin-top: 20px;
            box-shadow: 0px 3px 10px #ddd;
        }
        .counter {
            font-size: 35px;
            font-weight: bold;
            color: #dc3545;
        }
        .badge-unpaid {
            background: #dc3545;
            color: white;
            padding: 6px 12px;
            border-radius: 15px;
        }
        .badge-partial {
            background: #ffc107;
            color: black;
            padding: 6px 12px;
            border-radius: 15px;
        }
        .tableCard {
            margin-top: 20px;
        }
        .grid td {
            vertical-align: middle;
            text-align: center;
        }
        .filter-label {
            font-weight: bold;
            margin-bottom: 0;
        }
    </style>

    <div class="container-fluid">
        <div class="headerCard">
            <div class="row">
                <div class="col-md-8">
                    <h2><i class="fa-solid fa-hand-holding-dollar"></i> استعلام الفواتير غير المدفوعة</h2>
                </div>
                <div class="col-md-4 text-end">
                    <h5>إدارة الفواتير</h5>
                </div>
            </div>
        </div>

        <div class="searchCard">
            <div class="row">
                <div class="col-md-3">
                    <asp:Label ID="lblPatientName" runat="server" Text="اسم المريض" CssClass="filter-label" />
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="ابحث باسم المريض..." />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblDoctor" runat="server" Text="الطبيب" CssClass="filter-label" />
                    <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="-- الكل --" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblService" runat="server" Text="الخدمة" CssClass="filter-label" />
                    <asp:DropDownList ID="ddlService" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="-- الكل --" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblDateRange" runat="server" Text="نطاق التاريخ" CssClass="filter-label" />
                    <div class="input-group">
                        <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" TextMode="Date" placeholder="من" />
                        <span class="input-group-text">إلى</span>
                        <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" TextMode="Date" placeholder="إلى" />
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12 text-center">
                    <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" placeholder="رقم الفاتورة"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnRefresh" runat="server" Text="إعادة ضبط" CssClass="btn btn-secondary ms-2" OnClick="btnRefresh_Click" />
                    <span class="ms-4">
                        <small>إجمالي غير المدفوعين</small>
                        <div class="counter d-inline-block ms-2">
                            <asp:Label ID="lblCount" runat="server" Text="0" />
                        </div>
                    </span>
                </div>
            </div>
        </div>

        <div class="tableCard">
            <asp:GridView ID="gvUnpaid" runat="server"
                CssClass="table table-bordered table-hover grid"
                AutoGenerateColumns="False"
                DataKeyNames="AppointmentId, ServiceId"
                OnRowCommand="gvUnpaid_RowCommand">
                <Columns>
                    <asp:BoundField DataField="InvoiceNumber" HeaderText="رقم الفاتورة" />
                    <asp:BoundField DataField="PatientName" HeaderText="اسم المريض" />
                    <asp:BoundField DataField="Phone" HeaderText="الهاتف" />
                    <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                    <asp:BoundField DataField="AppointmentDate" HeaderText="تاريخ الحجز" />
                    <asp:BoundField DataField="ServiceName" HeaderText="الخدمة" />
                    <asp:BoundField DataField="TotalAmount" HeaderText="القيمة" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="PaidAmount" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Remaining" HeaderText="المتبقي" DataFormatString="{0:N2}" />
                    <asp:TemplateField HeaderText="حالة الدفع">
                        <ItemTemplate>
                            <span class='<%# Eval("PaymentStatus").ToString() == "Unpaid" ? "badge-unpaid" : "badge-partial" %>'>
                                <%# Eval("PaymentStatus").ToString() == "Unpaid" ? "غير مدفوع" : "مدفوع جزئياً" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="طريقة الحجز">
    <ItemTemplate>
        <%# GetBookingTypeBadge(Container.DataItem) %>
    </ItemTemplate>
</asp:TemplateField>
                    <asp:TemplateField HeaderText="الإجراء">
                        <ItemTemplate>
                            <asp:Button ID="btnPay" runat="server" Text="تأكيد الدفع"
                                CssClass="btn btn-success btn-sm"
                                CommandName="PayInvoice"
                                CommandArgument='<%# Eval("AppointmentId") + "," + Eval("ServiceId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="text-center mt-3">
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true" />
        </div>
    </div>

</asp:Content>