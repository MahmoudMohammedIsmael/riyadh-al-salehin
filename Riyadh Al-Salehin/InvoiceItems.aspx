<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="InvoiceItems.aspx.cs" Inherits="Riyadh_Al_Salehin.InvoiceItems" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .ii-page { --brand:#0f9d58; --brand-dark:#0a7a43; position:relative; z-index:999; }

    .ii-header {
        background: linear-gradient(135deg, var(--brand) 0%, var(--brand-dark) 100%);
        border-radius: 14px;
        padding: 20px 26px;
        color: #fff;
        margin-bottom: 20px;
        box-shadow: 0 8px 20px rgba(15,157,88,0.25);
    }
    .ii-header h3 { margin:0; font-weight:700; }
    .ii-header small { opacity:.85; }

    .ii-card {
        border:none; border-radius:14px;
        box-shadow:0 3px 10px rgba(0,0,0,0.06);
        background:#fff; padding:20px; margin-bottom:20px;
    }

    .ii-card .card-title {
        font-weight:700; color:#343a40; margin-bottom:16px;
        display:flex; align-items:center; gap:8px;
    }

    .patient-info-box {
        background:#f6fbf8;
        border-radius:10px;
        padding:14px 18px;
        border-right:4px solid var(--brand);
        margin-bottom:18px;
    }

    .patient-info-box .info-label { font-size:.78rem; color:#6c757d; font-weight:600; }
    .patient-info-box .info-value { font-size:1.05rem; font-weight:700; color:#212529; }

    .table thead th {
        background:#f6f7fb; color:#495057; font-weight:700;
        border-bottom:2px solid #e9ecef; white-space:nowrap;
    }
    .table tbody td { vertical-align:middle; }
    .table tbody tr:hover { background-color:#f2fbf6; }

    .form-control, .form-select { border-radius:8px; }
    label { font-size:.82rem; font-weight:600; color:#495057; margin-bottom:4px; }

    /* ===== Service Cards ===== */
    .service-cards-wrapper {
        display:flex; flex-wrap:wrap; gap:10px;
        max-height:230px; overflow-y:auto;
        padding:12px; border:1px solid #e9ecef; border-radius:10px; background:#fafbfc;
    }
    .service-card {
        cursor:pointer;
        background:#fff;
        border:2px solid #e2e6ea;
        border-radius:10px;
        padding:10px 16px;
        min-width:135px;
        text-align:center;
        transition:all .15s ease;
        user-select:none;
    }
    .service-card:hover {
        border-color:var(--brand);
        box-shadow:0 2px 10px rgba(15,157,88,0.18);
        transform:translateY(-1px);
    }
    .service-card.selected {
        border-color:var(--brand);
        background:linear-gradient(135deg, var(--brand) 0%, var(--brand-dark) 100%);
        color:#fff;
        box-shadow:0 4px 12px rgba(15,157,88,0.3);
    }
    .service-card.selected .service-price { color:#eafff2; }
    .service-card .service-name { font-weight:700; font-size:.88rem; margin-bottom:4px; }
    .service-card .service-price { font-size:.78rem; color:#6c757d; }
</style>

<div class="container-fluid mt-3 ii-page" dir="rtl">

    <div class="ii-header">
        <h3>🦷 إضافة بنود إضافية على الفاتورة</h3>
        <small>اختر مريضاً تم كشفه، أو ابحث عن فاتورة سابقة لإضافة بنود لها</small>
    </div>

    <!-- ============ STEP 1: SEARCH / LIST ============ -->
    <asp:Panel ID="pnlSearch" runat="server">
        <div class="ii-card">
            <div class="card-title">🔍 اختيار المريض / الفاتورة</div>

            <!-- شريط البحث السريع (للبحث في الفواتير) -->
            <div class="row g-3 align-items-end">
                <div class="col-md-8">
                    <label>اسم المريض أو رقم الانتظار</label>
                    <asp:TextBox ID="txtSearchInvoice" runat="server" CssClass="form-control" placeholder="اكتب للبحث في الفواتير..." />
                </div>
                <div class="col-md-4">
                    <asp:Button ID="btnSearchInvoice" runat="server" Text="بحث في الفواتير" CssClass="btn btn-outline-success w-100" OnClick="btnSearchInvoice_Click" />
                </div>
            </div>

            <!-- ===== قائمة الحجوزات المنجزة (المرضى الذين تم الكشف عليهم) ===== -->
            <div class="mt-3">
                <div class="d-flex justify-content-between align-items-center">
                    <h6 class="mb-2">📋 قائمة المرضى الذين تم الكشف عليهم (اختر لإضافة بنود)</h6>
                    <asp:LinkButton ID="lnkRefreshAppointments" runat="server" CssClass="btn btn-sm btn-outline-secondary" OnClick="lnkRefreshAppointments_Click">تحديث</asp:LinkButton>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvAppointments" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover align-middle mb-0"
                        GridLines="None"
                        OnRowCommand="gvAppointments_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="AppointmentDate" HeaderText="تاريخ الكشف" DataFormatString="{0:yyyy/MM/dd HH:mm}" />
                            <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                            <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                            <asp:TemplateField HeaderText="الفاتورة">
                                <ItemTemplate>
                                    <span class="badge bg-<%# (Eval("HasInvoice") != DBNull.Value && Convert.ToBoolean(Eval("HasInvoice"))) ? "success" : "warning" %>">
                                        <%# (Eval("HasInvoice") != DBNull.Value && Convert.ToBoolean(Eval("HasInvoice"))) ? "موجودة" : "جديدة" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:Button ID="btnSelectAppointment" runat="server"
                                        Text="اختيار"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="SelectAppointment"
                                        CommandArgument='<%# Eval("AppointmentId") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-3">لا توجد حجوزات منتهية لهذا الطبيب</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- ===== نتائج البحث في الفواتير (يظهر فقط عند الضغط على زر البحث) ===== -->
            <div class="mt-3" id="divSearchResults" runat="server" visible="false">
                <hr />
                <h6 class="mb-2">🔎 نتائج البحث في الفواتير</h6>
                <div class="table-responsive">
                    <asp:GridView ID="gvSearchResults" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover align-middle mb-0"
                        GridLines="None"
                        OnRowCommand="gvSearchResults_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="QueueNumber" HeaderText="رقم الانتظار" />
                            <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ" DataFormatString="{0:yyyy/MM/dd hh:mm tt}" />
                            <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                            <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:Button ID="btnSelect" runat="server"
                                        Text="اختيار"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="SelectInvoice"
                                        CommandArgument='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-3">لا توجد فواتير مطابقة</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- ============ STEP 2: SELECTED INVOICE + ADD ITEM (visible after selection) ============ -->
    <asp:Panel ID="pnlInvoice" runat="server" Visible="false">

        <asp:HiddenField ID="hfInvoiceId" runat="server" />
        <asp:HiddenField ID="hfEditItemId" runat="server" Value="0" />

        <!-- ===== HiddenField للخدمة المختارة ===== -->
        <asp:HiddenField ID="hfSelectedServiceId" runat="server" />

        <div class="ii-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <div class="card-title mb-0">📌 بيانات الفاتورة المختارة</div>
                <asp:LinkButton ID="lnkBackToSearch" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkBackToSearch_Click">⬅ بحث عن فاتورة أخرى</asp:LinkButton>
            </div>

            <div class="patient-info-box">
                <div class="row">
                    <div class="col-md-3">
                        <div class="info-label">المريض</div>
                        <div class="info-value"><asp:Label ID="lblPatientName" runat="server" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-label">الطبيب</div>
                        <div class="info-value"><asp:Label ID="lblDoctorName" runat="server" /></div>
                    </div>
                    <div class="col-md-2">
                        <div class="info-label">رقم الانتظار</div>
                        <div class="info-value"><asp:Label ID="lblQueueNumber" runat="server" /></div>
                    </div>
                    <div class="col-md-2">
                        <div class="info-label">الإجمالي الحالي</div>
                        <div class="info-value text-success"><asp:Label ID="lblCurrentTotal" runat="server" /></div>
                    </div>
                    <div class="col-md-2">
                        <div class="info-label">المتبقي</div>
                        <div class="info-value text-danger"><asp:Label ID="lblCurrentRemaining" runat="server" /></div>
                    </div>
                </div>
            </div>

            <!-- اختيار الخدمة -->
            <div class="row g-3">
                <div class="col-12">
                    <label>الخدمة الإضافية</label>
                    <div class="service-cards-wrapper">
                        <asp:Repeater ID="rptServices" runat="server">
                            <ItemTemplate>
                                <div class="service-card"
                                     data-id='<%# Eval("Id") %>'
                                     data-price='<%# Eval("BasePrice") %>'
                                     onclick="selectService(this)">
                                    <div class="service-name"><%# Eval("ServiceName") %></div>
                                    <div class="service-price"><%# Eval("BasePrice", "{0:N2}") %> ج.م</div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <!-- نموذج الإضافة / التعديل -->
            <div class="row g-3 align-items-end mt-1">
                <div class="col-md-3">
                    <label>الكمية</label>
                    <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" Text="1" oninput="calculateTotal()" />
                </div>
                <div class="col-md-3">
                    <label>سعر الوحدة</label>
                    <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" oninput="calculateTotal()" />
                </div>
                <div class="col-md-3">
                    <label>الإجمالي</label>
                    <asp:TextBox ID="txtTotalPrice" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnSaveItem" runat="server" Text="💾 إضافة" CssClass="btn btn-success w-100" OnClick="btnSaveItem_Click" />
                </div>
            </div>

            <div class="mt-2">
                <asp:Button ID="btnCancelEdit" runat="server" Text="إلغاء التعديل" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnCancelEdit_Click" Visible="false" />
                <asp:Button ID="btnPrintInvoice" runat="server" Text="🖨️ طباعة الفاتورة" CssClass="btn btn-sm btn-outline-primary" OnClick="btnPrintInvoice_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2" />
            </div>
        </div>

        <!-- قائمة البنود المضافة -->
        <div class="ii-card">
            <div class="card-title">📋 البنود الإضافية المضافة على هذه الفاتورة</div>
            <div class="table-responsive">
                <asp:GridView ID="gvItems" runat="server"
                    CssClass="table table-hover align-middle mb-0"
                    AutoGenerateColumns="False"
                    GridLines="None"
                    OnRowCommand="gvItems_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="ServiceName" HeaderText="الخدمة" />
                        <asp:BoundField DataField="Quantity" HeaderText="الكمية" />
                        <asp:BoundField DataField="UnitPrice" HeaderText="سعر الوحدة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalPrice" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
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
                                    OnClientClick="return confirm('هل تريد حذف هذا البند؟ سيتم خصم قيمته من إجمالي الفاتورة');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-3">لا توجد بنود إضافية على هذه الفاتورة بعد</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

    </asp:Panel>

</div>

<script>
    function selectService(el) {
        document.querySelectorAll('.service-card').forEach(function (c) {
            c.classList.remove('selected');
        });
        el.classList.add('selected');

        var hf = document.getElementById('<%= hfSelectedServiceId.ClientID %>');
        if (hf) hf.value = el.getAttribute('data-id');

        var price = parseFloat(el.getAttribute('data-price')) || 0;
        var txtUnitPrice = document.getElementById('<%= txtUnitPrice.ClientID %>');
        if (txtUnitPrice) txtUnitPrice.value = price.toFixed(2);

        calculateTotal();
    }

    function calculateTotal() {
        var txtQuantity = document.getElementById('<%= txtQuantity.ClientID %>');
        var txtUnitPrice = document.getElementById('<%= txtUnitPrice.ClientID %>');
        var txtTotalPrice = document.getElementById('<%= txtTotalPrice.ClientID %>');

        var qty = parseFloat(txtQuantity ? txtQuantity.value : 0) || 0;
        var price = parseFloat(txtUnitPrice ? txtUnitPrice.value : 0) || 0;
        if (txtTotalPrice) txtTotalPrice.value = (qty * price).toFixed(2);
    }

    // عند تحميل الصفحة، إذا كانت هناك خدمة محددة مسبقاً (في حالة التعديل)
    document.addEventListener('DOMContentLoaded', function () {
        var hf = document.getElementById('<%= hfSelectedServiceId.ClientID %>');
        if (!hf || !hf.value) return;

        document.querySelectorAll('.service-card').forEach(function (c) {
            if (c.getAttribute('data-id') === hf.value) {
                c.classList.add('selected');
                var price = parseFloat(c.getAttribute('data-price')) || 0;
                var txtUnitPrice = document.getElementById('<%= txtUnitPrice.ClientID %>');
                if (txtUnitPrice) txtUnitPrice.value = price.toFixed(2);
                calculateTotal();
            }
        });
    });
</script>

</asp:Content>