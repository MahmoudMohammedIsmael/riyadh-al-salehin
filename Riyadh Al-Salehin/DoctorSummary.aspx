<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true"
    CodeBehind="DoctorSummary.aspx.cs" Inherits="Riyadh_Al_Salehin.DoctorSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="assets/Newcss/all.min.css" rel="stylesheet" />
    <link href="assets/Newcss/bootstrap.min.css" rel="stylesheet" />

    <style>
        body { background: #f5f7fb; font-family: Tahoma; }

        .headerCard {
            background: linear-gradient(45deg, #0d6efd, #198754);
            color: white;
            border-radius: 15px;
            padding: 20px;
            margin-top: 20px;
        }
        .headerCard h2 { font-size: 1.4rem; margin: 0; }
        .headerCard h5 { font-size: 1rem; margin: 0; }

        .filterCard {
            background: white;
            border-radius: 12px;
            padding: 15px;
            margin-top: 16px;
            box-shadow: 0 3px 10px #ddd;
        }

        .statCard {
            background: white;
            border-radius: 14px;
            padding: 18px 14px;
            margin-top: 16px;
            box-shadow: 0 3px 12px rgba(0,0,0,.09);
            text-align: center;
        }
        .statCard .icon { font-size: 2rem; margin-bottom: 6px; }
        .statCard .num  { font-size: 2.2rem; font-weight: bold; line-height: 1.1; }
        .statCard .lbl  { font-size: .85rem; color: #666; margin-top: 4px; }

        .c-blue   { color: #0d6efd; }
        .c-green  { color: #198754; }
        .c-orange { color: #fd7e14; }
        .c-purple { color: #6f42c1; }

        .tableCard {
            background: white;
            border-radius: 12px;
            margin-top: 16px;
            box-shadow: 0 3px 10px #ddd;
            overflow-x: auto;
        }
        .grid { margin-bottom: 0; }
        .grid th {
            background: #0d6efd;
            color: white;
            text-align: center;
            vertical-align: middle;
            white-space: nowrap;
        }
        .grid td { text-align: center; vertical-align: middle; }

        .badge-kshf    { background:#0d6efd; color:white; padding:4px 10px; border-radius:12px; font-size:.82rem; }
        .badge-consult { background:#6f42c1; color:white; padding:4px 10px; border-radius:12px; font-size:.82rem; }
        .badge-paid    { background:#198754; color:white; padding:4px 10px; border-radius:12px; font-size:.82rem; }
        .badge-unpaid  { background:#dc3545; color:white; padding:4px 10px; border-radius:12px; font-size:.82rem; }

        @media (max-width: 767.98px) {
            .headerCard { margin-top:10px; padding:14px; border-radius:12px; }
            .headerCard .row > div { text-align: center !important; }
            .headerCard h2 { font-size: 1.1rem; }
            .statCard { margin-top: 10px; }
            .statCard .num { font-size: 1.8rem; }
            .tableCard { border-radius: 10px; }
            .tableCard .grid { min-width: 720px; white-space: nowrap; }
            .tableCard .grid th,
            .tableCard .grid td { padding: 7px 6px; font-size: .8rem; }
        }
    </style>

    <div class="container-fluid pb-4">

        <!-- Header -->
        <div class="headerCard">
            <div class="row align-items-center">
                <div class="col-8">
                    <h2>
                        <i class="fa-solid fa-clipboard-list"></i>
                        ملخص أعمال الطبيب
                    </h2>
                </div>
                <div class="col-4 text-end">
                    <h5>
                        د /
                        <asp:Label ID="lblDoctor" runat="server" Text="..." />
                    </h5>
                </div>
            </div>
        </div>

        <!-- Filter -->
        <div class="filterCard">
            <div class="row g-2 align-items-end">
                <div class="col-6 col-md-3">
                    <label class="form-label mb-1">من تاريخ</label>
                    <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-6 col-md-3">
                    <label class="form-label mb-1">إلى تاريخ</label>
                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-8 col-md-4">
                    <label class="form-label mb-1">اسم المريض</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="ابحث باسم المريض..." />
                </div>
                <div class="col-4 col-md-2">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث"
                        CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <!-- Stat Cards -->
        <div class="row g-2 mt-1">
            <div class="col-6 col-md-3">
                <div class="statCard">
                    <div class="icon c-blue"><i class="fa-solid fa-users"></i></div>
                    <div class="num c-blue"><asp:Label ID="lblTotalCount" runat="server" Text="0" /></div>
                    <div class="lbl">إجمالي المرضى</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="statCard">
                    <div class="icon c-orange"><i class="fa-solid fa-stethoscope"></i></div>
                    <div class="num c-orange"><asp:Label ID="lblKshfCount" runat="server" Text="0" /></div>
                    <div class="lbl">كشف</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="statCard">
                    <div class="icon c-purple"><i class="fa-solid fa-comments"></i></div>
                    <div class="num c-purple"><asp:Label ID="lblConsultCount" runat="server" Text="0" /></div>
                    <div class="lbl">استشارة</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="statCard">
                    <div class="icon c-green"><i class="fa-solid fa-money-bill-wave"></i></div>
                    <div class="num c-green"><asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
                    <div class="lbl">إجمالي الحساب (ر.س)</div>
                </div>
            </div>
        </div>

        <!-- Table -->
        <div class="tableCard mt-3">
            <asp:GridView
                ID="gvSummary"
                runat="server"
                CssClass="table table-bordered table-hover grid"
                AutoGenerateColumns="False"
                EmptyDataText="لا توجد بيانات للعرض">
                <Columns>
                    <asp:BoundField DataField="RowNum"          HeaderText="#" />
                    <asp:BoundField DataField="PatientName"     HeaderText="اسم المريض" />
                    <asp:BoundField DataField="AppointmentDate" HeaderText="التاريخ"    DataFormatString="{0:yyyy/MM/dd}" />
                    <asp:BoundField DataField="AppointmentTime" HeaderText="الوقت"      DataFormatString="{0:hh:mm tt}" />
                    <asp:TemplateField HeaderText="نوع الزيارة">
                        <ItemTemplate>
                            <span class='<%# Eval("VisitType").ToString() == "استشارة" ? "badge-consult" : "badge-kshf" %>'>
                                <%# Eval("VisitType") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ServiceName"     HeaderText="الخدمة الرئيسية" />
                    <asp:BoundField DataField="ExtraServices"   HeaderText="الخدمات الإضافية" />
                    <asp:BoundField DataField="TotalAmount"     HeaderText="المبلغ (ر.س)" DataFormatString="{0:N2}" />
                    <asp:TemplateField HeaderText="حالة الدفع">
                        <ItemTemplate>
                            <span class='<%# Eval("PaymentStatus").ToString() == "Paid" ? "badge-paid" : "badge-unpaid" %>'>
                                <%# Eval("PaymentStatus").ToString() == "Paid" ? "مدفوع" : "غير مدفوع" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="InsuranceCompany" HeaderText="التأمين" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- Message -->
        <div class="text-center mt-3">
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true" />
        </div>

    </div>

</asp:Content>