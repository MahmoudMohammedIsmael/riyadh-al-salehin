<%@ Page Title=" جميع فواتير المرضى " Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="AllPatientsInvoices.aspx.cs" Inherits="Riyadh_Al_Salehin.AllPatientsInvoices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .main-card { border-radius: 14px; box-shadow: 0 8px 20px rgba(0,0,0,0.06); background: #fff; padding: 20px; margin-bottom: 20px; }
        .main-header { background: linear-gradient(135deg, #0d6efd, #0a58ca); border-radius: 14px; padding: 18px 24px; color: #fff; margin-bottom: 24px; }
        .main-header h3 { margin: 0; font-weight: 700; }
        .main-header small { opacity: 0.85; }
        .service-card { cursor: pointer; border: 2px solid #dee2e6; border-radius: 8px; padding: 8px 14px; margin: 4px; display: inline-block; transition: 0.1s; }
        .service-card.selected { border-color: #0d6efd; background: #e9f0ff; }
    </style>

    <div class="container-fluid mt-3" dir="rtl">
        <div class="main-header">
            <h3>📋 جميع فواتير المرضى</h3>
            <small>اختر مريضاً لعرض فواتيره، ثم اختر فاتورة لإضافة بنود إضافية</small>
        </div>

        <!-- ====== STEP 1: قائمة المرضى ====== -->
        <asp:Panel ID="pnlPatients" runat="server">
            <div class="main-card">
                <!-- فلتر البحث -->
                <div class="row g-3 align-items-end mb-3">
                    <div class="col-md-3">
                        <label>من تاريخ</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-3">
                        <label>إلى تاريخ</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-4">
                        <label>🔍 بحث باسم المريض</label>
                        <asp:TextBox ID="txtSearchPatient" runat="server" CssClass="form-control" placeholder="اكتب اسم المريض..." />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvPatients" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover align-middle"
                        GridLines="None"
                        OnRowCommand="gvPatients_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="PatientName" HeaderText="اسم المريض" />
                            <asp:BoundField DataField="Phone" HeaderText="الهاتف" />
                            <asp:BoundField DataField="InvoiceCount" HeaderText="عدد الفواتير" />
                            <asp:BoundField DataField="TotalSpent" HeaderText="إجمالي المصروف" DataFormatString="{0:N2}" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnSelect" runat="server"
                                        Text="عرض الفواتير"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="SelectPatient"
                                        CommandArgument='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-4">لا يوجد مرضى لديهم فواتير في الفترة المحددة</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2" />
            </div>
        </asp:Panel>

        <!-- ====== STEP 2: قائمة فواتير المريض المختار ====== -->
        <asp:Panel ID="pnlInvoices" runat="server" Visible="false">
            <div class="main-card">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h5 class="mb-0">🧾 فواتير المريض: <asp:Label ID="lblPatientName" runat="server" CssClass="text-primary" /></h5>
                    <div>
                        <asp:Button ID="btnNewInvoice" runat="server" Text="➕ إنشاء فاتورة جديدة" CssClass="btn btn-success me-2" OnClick="btnNewInvoice_Click" />
                        <asp:LinkButton ID="lnkBackToPatients" runat="server" CssClass="btn btn-outline-secondary" OnClick="lnkBackToPatients_Click">⬅ رجوع</asp:LinkButton>
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvInvoices" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover align-middle"
                        GridLines="None"
                        OnRowCommand="gvInvoices_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="رقم الفاتورة" />
                            <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ" DataFormatString="{0:yyyy/MM/dd HH:mm}" />
                            <asp:BoundField DataField="QueueNumber" HeaderText="رقم الانتظار" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="PaidAmount" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="Remaining" HeaderText="المتبقي" DataFormatString="{0:N2}" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnSelectInvoice" runat="server"
                                        Text="اختيار"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="SelectInvoice"
                                        CommandArgument='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-4">لا توجد فواتير لهذا المريض</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </asp:Panel>

        <!-- ====== STEP 3: تفاصيل الفاتورة + إضافة بنود ====== -->
        <asp:Panel ID="pnlInvoiceDetails" runat="server" Visible="false">
            <asp:HiddenField ID="hfInvoiceId" runat="server" />
            <asp:HiddenField ID="hfPatientId" runat="server" />
            <asp:HiddenField ID="hfEditItemId" runat="server" Value="0" />
            <asp:HiddenField ID="hfSelectedServiceId" runat="server" />

            <div class="main-card">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h5 class="mb-0">📌 بيانات الفاتورة</h5>
                    <div>
                        <asp:LinkButton ID="lnkBackToInvoices" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkBackToInvoices_Click">⬅ عودة للفواتير</asp:LinkButton>
                    </div>
                </div>

                <div class="row bg-light p-3 rounded-3 mb-3">
                    <div class="col-md-3"><span class="fw-bold">المريض:</span> <asp:Label ID="lblDetailPatient" runat="server" /></div>
                    <div class="col-md-2"><span class="fw-bold">رقم الانتظار:</span> <asp:Label ID="lblDetailQueue" runat="server" /></div>
                    <div class="col-md-2"><span class="fw-bold">الإجمالي:</span> <asp:Label ID="lblDetailTotal" runat="server" CssClass="text-success fw-bold" /></div>
                    <div class="col-md-2"><span class="fw-bold">المتبقي:</span> <asp:Label ID="lblDetailRemaining" runat="server" CssClass="text-danger fw-bold" /></div>
                    <div class="col-md-3"><span class="fw-bold">تاريخ الفاتورة:</span> <asp:Label ID="lblDetailDate" runat="server" /></div>
                </div>

                <!-- اختيار الخدمة -->
                <div class="mb-3">
                    <label class="fw-bold">الخدمة الإضافية</label>
                    <div class="d-flex flex-wrap gap-2">
                        <asp:Repeater ID="rptServices" runat="server">
                            <ItemTemplate>
                                <div class="service-card"
                                     data-id='<%# Eval("Id") %>'
                                     data-price='<%# Eval("BasePrice") %>'
                                     onclick="selectService(this)">
                                    <div><%# Eval("ServiceName") %></div>
                                    <small><%# Eval("BasePrice", "{0:N2}") %> ج.م</small>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <!-- إضافة بند -->
                <div class="row g-3 align-items-end">
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
                    <asp:Button ID="btnConfirmInvoice" runat="server" Text="💾 حفظ وتثبيت الفاتورة" CssClass="btn btn-sm btn-success me-2" OnClick="btnConfirmInvoice_Click" />
                    <asp:Button ID="btnPrintInvoice" runat="server" Text="🖨️ طباعة الفاتورة" CssClass="btn btn-sm btn-outline-primary" OnClick="btnPrintInvoice_Click" />
                </div>

                <hr />
                <h6 class="mt-3">📋 البنود الإضافية</h6>
                <div class="table-responsive">
                    <asp:GridView ID="gvItems" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover align-middle"
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
                                        OnClientClick="return confirm('هل تريد حذف هذا البند؟');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-3">لا توجد بنود إضافية</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script>
        // تم تعديل الدوال لتتوافق مع TypeScript بإضافة أنواع وتحويل العناصر إلى HTMLInputElement
        function selectService(el) {
            document.querySelectorAll('.service-card').forEach(function(c) { c.classList.remove('selected'); });
            el.classList.add('selected');
            var hf = document.getElementById('<%= hfSelectedServiceId.ClientID %>');
            if (hf) hf.value = el.getAttribute('data-id');
            var price = parseFloat(el.getAttribute('data-price')) || 0;
            var unit = document.getElementById('<%= txtUnitPrice.ClientID %>');
            if (unit) unit.value = price.toFixed(2);
            calculateTotal();
        }

        function calculateTotal() {
            var qtyInput = document.getElementById('<%= txtQuantity.ClientID %>');
            var priceInput = document.getElementById('<%= txtUnitPrice.ClientID %>');
            var totalInput = document.getElementById('<%= txtTotalPrice.ClientID %>');
            var qty = parseFloat(qtyInput ? qtyInput.value : '0') || 0;
            var price = parseFloat(priceInput ? priceInput.value : '0') || 0;
            if (totalInput) totalInput.value = (qty * price).toFixed(2);
        }

        document.addEventListener('DOMContentLoaded', function () {
            var hf = document.getElementById('<%= hfSelectedServiceId.ClientID %>');
            if (!hf || !hf.value) return;
            document.querySelectorAll('.service-card').forEach(function(el) {
                if (el.getAttribute('data-id') === hf.value) {
                    el.classList.add('selected');
                    var price = parseFloat(el.getAttribute('data-price')) || 0;
                    var unit = document.getElementById('<%= txtUnitPrice.ClientID %>');
                    if (unit) unit.value = price.toFixed(2);
                    calculateTotal();
                }
            });
        });
</script>
</asp:Content>