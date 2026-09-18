<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="AccountsDashboard.aspx.cs" Inherits="Riyadh_Al_Salehin.AccountsDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- تضمين مكتبات CSS/JS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.rtl.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        body { background: #f4f7fc; font-family: 'Segoe UI', Tahoma, Arial, sans-serif; }
        .card { border: none; border-radius: 16px; box-shadow: 0 8px 20px rgba(0,0,0,0.05); transition: all 0.2s; }
        .card:hover { transform: translateY(-3px); }
        .card-icon { font-size: 2rem; opacity: 0.6; }
        .stat-number { font-size: 1.8rem; font-weight: 700; }
        .filter-section { background: white; border-radius: 16px; padding: 20px; margin-bottom: 30px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .table th { background: #e9eff5; color: #2c3e50; }
        .badge-status { font-size: 0.8rem; }
        .modal-lg { max-width: 1000px; }
        .print-only { display: none; }
        @media print {
            .no-print { display: none !important; }
            .print-only { display: block !important; }
            .card { box-shadow: none !important; border: 1px solid #ddd; }
            body { background: white; }
        }
        .chart-container { height: 250px; }
    </style>

    <div class="container-fluid py-4">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

        <!-- عنوان الصفحة -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="fw-bold text-primary"><i class="fas fa-chart-pie me-2"></i>لوحة الحسابات المالية</h2>
            <div>
                <asp:Button ID="btnRefresh" runat="server" Text="تحديث البيانات" CssClass="btn btn-primary me-2" OnClick="btnRefresh_Click" />
                <asp:Button ID="btnPrint" runat="server" Text="طباعة التقرير" CssClass="btn btn-secondary me-2" OnClientClick="window.print(); return false;" />
                <asp:Button ID="btnExportExcel" runat="server" Text="تصدير Excel" CssClass="btn btn-success" OnClick="btnExportExcel_Click" />
            </div>
        </div>

        <!-- فلتر -->
        <div class="filter-section no-print">
            <div class="row g-3">
                <div class="col-md-2">
                    <label class="form-label fw-bold">من تاريخ</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">إلى تاريخ</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">الطبيب</label>
                    <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                        <asp:ListItem Text="الكل" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">المركز</label>
                    <asp:DropDownList ID="ddlCenter" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                        <asp:ListItem Text="الكل" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">نوع الطبيب</label>
                    <asp:DropDownList ID="ddlDoctorType" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                        <asp:ListItem Text="الكل" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold">طريقة الدفع</label>
                    <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                        <asp:ListItem Text="الكل" Value="" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-12 text-start">
                    <asp:Button ID="btnFilter" runat="server" Text="بحث" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="إعادة تعيين" CssClass="btn btn-outline-secondary me-2" OnClick="btnReset_Click" />
                </div>
            </div>
        </div>

        <!-- بطاقات الملخص -->
        <div class="row g-4 mb-4">
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-coins fa-2x text-warning"></i></div>
                        <span class="badge bg-light text-dark">الإيرادات</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalRevenue" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-money-bill-wave fa-2x text-success"></i></div>
                        <span class="badge bg-light text-dark">المدفوع</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalPaid" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-percent fa-2x text-danger"></i></div>
                        <span class="badge bg-light text-dark">الخصومات</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalDiscount" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-hourglass-half fa-2x text-primary"></i></div>
                        <span class="badge bg-light text-dark">المتبقي</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalRemaining" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-user-md fa-2x text-info"></i></div>
                        <span class="badge bg-light text-dark">عمولة الأطباء</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalDoctorCommission" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-hospital fa-2x text-secondary"></i></div>
                        <span class="badge bg-light text-dark">نصيب المركز</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblTotalCenterCommission" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-file-invoice fa-2x text-dark"></i></div>
                        <span class="badge bg-light text-dark">عدد الفواتير</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblInvoiceCount" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-users fa-2x text-primary"></i></div>
                        <span class="badge bg-light text-dark">عدد المرضى</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblPatientCount" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-stethoscope fa-2x text-success"></i></div>
                        <span class="badge bg-light text-dark">عدد الكشوفات</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblVisitCount" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-user-tie fa-2x text-warning"></i></div>
                        <span class="badge bg-light text-dark">الأطباء الفعليين</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblActiveDoctors" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-check-circle fa-2x text-success"></i></div>
                        <span class="badge bg-light text-dark">فواتير مدفوعة</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblPaidInvoiceCount" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-times-circle fa-2x text-danger"></i></div>
                        <span class="badge bg-light text-dark">فواتير غير مدفوعة</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblUnpaidInvoiceCount" runat="server">0</span></div>
                </div>
            </div>
            <div class="col-xl-3 col-lg-4 col-md-6">
                <div class="card p-3 bg-white h-100">
                    <div class="d-flex justify-content-between">
                        <div><i class="fas fa-hand-holding-usd fa-2x text-info"></i></div>
                        <span class="badge bg-light text-dark">إجمالي المدفوع فقط</span>
                    </div>
                    <div class="mt-2"><span class="stat-number" id="lblPaidInvoicesTotal" runat="server">0</span> <small>ج.م</small></div>
                </div>
            </div>
        </div>

        <!-- مخططات -->
        <div class="row g-4 mb-4 no-print">
            <div class="col-md-6">
                <div class="card p-3">
                    <h5 class="fw-bold"><i class="fas fa-chart-line me-2"></i>إيرادات الأيام</h5>
                    <div class="chart-container"><canvas id="dailyRevenueChart"></canvas></div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card p-3">
                    <h5 class="fw-bold"><i class="fas fa-chart-pie me-2"></i>نسبة دخل الأطباء vs المركز</h5>
                    <div class="chart-container"><canvas id="doctorCenterPieChart"></canvas></div>
                </div>
            </div>
        </div>
        <div class="row g-4 mb-4 no-print">
            <div class="col-md-6">
                <div class="card p-3">
                    <h5 class="fw-bold"><i class="fas fa-chart-bar me-2"></i>إيرادات الأطباء</h5>
                    <div class="chart-container"><canvas id="doctorRevenueChart"></canvas></div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card p-3">
                    <h5 class="fw-bold"><i class="fas fa-credit-card me-2"></i>طرق الدفع</h5>
                    <div class="chart-container"><canvas id="paymentMethodChart"></canvas></div>
                </div>
            </div>
        </div>

        <!-- جدول حسابات الأطباء -->
        <div class="card p-3 mb-4">
            <h4 class="fw-bold mb-3"><i class="fas fa-user-md me-2"></i>حساب كل طبيب <small class="text-muted">(العمولة محسوبة على الفواتير المدفوعة فقط)</small></h4>
            <div class="table-responsive">
                <asp:GridView ID="gvDoctors" runat="server" CssClass="table table-hover table-striped" AutoGenerateColumns="False"
                    OnRowDataBound="gvDoctors_RowDataBound" OnRowCommand="gvDoctors_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="DoctorId" HeaderText="رقم الطبيب" />
                        <asp:BoundField DataField="DoctorName" HeaderText="اسم الطبيب" />
                        <asp:BoundField DataField="Specialty" HeaderText="التخصص" />
                        <asp:BoundField DataField="DoctorType" HeaderText="نوع الطبيب" />
                        <asp:BoundField DataField="PatientCount" HeaderText="عدد المرضى" />
                        <asp:BoundField DataField="VisitCount" HeaderText="عدد الكشوفات" />
                        <asp:BoundField DataField="InvoiceCount" HeaderText="إجمالي الفواتير" />
                        <asp:BoundField DataField="PaidInvoiceCount" HeaderText="فواتير مدفوعة" />
                        <asp:BoundField DataField="UnpaidInvoiceCount" HeaderText="فواتير غير مدفوعة" />
                        <asp:BoundField DataField="TotalInvoicesAmount" HeaderText="إجمالي الكشوفات" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="AdditionalServicesTotal" HeaderText="الخدمات الإضافية" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="RadiologyTotal" HeaderText="خدمات الأشعة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalRevenue" HeaderText="إجمالي الإيرادات" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="PaidInvoicesTotal" HeaderText="إجمالي المدفوع" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalDiscount" HeaderText="الخصومات" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalPaid" HeaderText="المبلغ المدفوع" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalRemaining" HeaderText="المتبقي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="DoctorCommissionRate" HeaderText="نسبة الطبيب" DataFormatString="{0:P2}" />
                        <asp:BoundField DataField="DoctorCommission" HeaderText="عمولة الطبيب (مدفوع فقط)" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="DoctorNet" HeaderText="نصيب الطبيب النهائي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="CenterNet" HeaderText="نصيب المركز (مدفوع فقط)" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="تفاصيل">
                            <ItemTemplate>
                                <asp:Button ID="btnDoctorDetails" runat="server" Text="تفاصيل" CommandName="DoctorDetails"
                                    CommandArgument='<%# Eval("DoctorId") %>' CssClass="btn btn-sm btn-outline-info" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- جدول تفصيل الكشفات والاستشارات -->
        <div class="card p-3 mb-4">
            <h4 class="fw-bold mb-3">
                <i class="fas fa-stethoscope me-2 text-primary"></i>
                تفصيل الكشفات والاستشارات لكل طبيب
                <small class="text-muted">(الأنصبة محسوبة على المدفوع فقط)</small>
            </h4>
            <div class="table-responsive">
                <asp:GridView ID="gvVisitBreakdown" runat="server" CssClass="table table-hover table-striped" 
                    AutoGenerateColumns="False" OnRowDataBound="gvVisitBreakdown_RowDataBound">
                    <HeaderStyle BackColor="#2c3e50" ForeColor="White" Font-Bold="true" />
                    <Columns>
                        <asp:BoundField DataField="DoctorName" HeaderText="اسم الطبيب" />
                        <asp:BoundField DataField="Specialty" HeaderText="التخصص" />
                        <asp:TemplateField HeaderText="الكشوفات">
                            <ItemTemplate>
                                <div class="text-center">
                                    <div class="badge bg-primary"><%# Eval("ExaminationCount") %> كشف</div>
                                    <div class="small text-muted"><%# Convert.ToDecimal(Eval("ExaminationTotal")).ToString("N2") %> ج.م</div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الاستشارات">
                            <ItemTemplate>
                                <div class="text-center">
                                    <div class="badge bg-success"><%# Eval("ConsultationCount") %> استشارة</div>
                                    <div class="small text-muted"><%# Convert.ToDecimal(Eval("ConsultationTotal")).ToString("N2") %> ج.م</div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الخدمات الإضافية">
                            <ItemTemplate>
                                <div class="text-center">
                                    <div class="badge bg-info text-dark"><%# Eval("AdditionalCount") %> خدمة</div>
                                    <div class="small text-muted"><%# Convert.ToDecimal(Eval("AdditionalTotal")).ToString("N2") %> ج.م</div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="أخرى">
                            <ItemTemplate>
                                <div class="text-center">
                                    <div class="badge bg-secondary"><%# Eval("OtherCount") %></div>
                                    <div class="small text-muted"><%# Convert.ToDecimal(Eval("OtherTotal")).ToString("N2") %> ج.م</div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TotalInvoices" HeaderText="إجمالي الفواتير" DataFormatString="{0}" />
                        <asp:BoundField DataField="ExaminationDoctorNet" HeaderText="نصيب الطبيب (كشوفات)" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="ConsultationDoctorNet" HeaderText="نصيب الطبيب (استشارات)" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="AdditionalDoctorNet" HeaderText="نصيب الطبيب (خدمات إضافية)" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="إجمالي نصيب الطبيب">
                            <ItemTemplate>
                                <strong class="text-success"><%# Convert.ToDecimal(Eval("TotalDoctorNet")).ToString("N2") %> ج.م</strong>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="إجمالي نصيب المركز">
                            <ItemTemplate>
                                <strong class="text-info"><%# Convert.ToDecimal(Eval("TotalCenterNet")).ToString("N2") %> ج.م</strong>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="GrandTotal" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="GrandPaid" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="GrandRemaining" HeaderText="المتبقي" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- الخدمات الإضافية -->
        <div class="card p-3 mb-4">
            <h4 class="fw-bold mb-3"><i class="fas fa-plus-circle me-2"></i>الخدمات الإضافية</h4>
            <div class="table-responsive">
                <asp:GridView ID="gvAdditionalServices" runat="server" CssClass="table table-hover table-striped" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="ServiceName" HeaderText="اسم الخدمة" />
                        <asp:BoundField DataField="Category" HeaderText="التصنيف" />
                        <asp:BoundField DataField="UsageCount" HeaderText="عدد مرات الاستخدام" />
                        <asp:BoundField DataField="PatientCount" HeaderText="عدد المرضى" />
                        <asp:BoundField DataField="TotalQuantity" HeaderText="إجمالي الكمية" />
                        <asp:BoundField DataField="TotalValue" HeaderText="إجمالي القيمة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalPaid" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="المتبقي (غير مسدد)">
                            <ItemTemplate>
                                <span class='<%# Convert.ToDecimal(Eval("TotalRemaining")) > 0 ? "badge bg-danger fs-6" : "badge bg-success fs-6" %>'>
                                    <%# Convert.ToDecimal(Eval("TotalRemaining")).ToString("N2") %> ج.م
                                </span>
                            </ItemTemplate>
                            <HeaderStyle CssClass="text-danger fw-bold" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DoctorShare" HeaderText="نصيب الطبيب" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="CenterShare" HeaderText="نصيب المركز" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- الأشعة -->
        <div class="card p-3 mb-4">
            <h4 class="fw-bold mb-3"><i class="fas fa-x-ray me-2"></i>خدمات الأشعة</h4>
            <div class="alert alert-warning" id="radiologyWarning" runat="server" visible="false">
                <i class="fas fa-exclamation-triangle me-2"></i>
                لا توجد طلبات أشعة مسددة في هذه الفترة.
            </div>
            <div class="table-responsive">
                <asp:GridView ID="gvRadiology" runat="server" CssClass="table table-hover table-striped" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="RequestDate" HeaderText="التاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="PatientName" HeaderText="صاحب الأشعة (المريض)" />
                        <asp:BoundField DataField="ServiceName" HeaderText="اسم الأشعة" />
                        <asp:BoundField DataField="TotalPaid" HeaderText="التكلفة (المدفوع)" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب الطالب للأشعة" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- طرق الدفع -->
        <div class="card p-3 mb-4">
            <h4 class="fw-bold mb-3"><i class="fas fa-wallet me-2"></i>طرق الدفع</h4>
            <div class="table-responsive">
                <asp:GridView ID="gvPaymentMethods" runat="server" CssClass="table table-hover table-striped" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="PaymentMethod" HeaderText="طريقة الدفع" />
                        <asp:BoundField DataField="TransactionCount" HeaderText="عدد العمليات" />
                        <asp:BoundField DataField="TotalInvoices" HeaderText="إجمالي الفواتير" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalPaid" HeaderText="إجمالي المدفوع" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalRemaining" HeaderText="إجمالي المتبقي" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- إجمالي المركز -->
        <div class="card p-3 mb-4 bg-light">
            <h4 class="fw-bold mb-3"><i class="fas fa-building me-2"></i>إجمالي حساب المركز</h4>
            <div class="row">
                <div class="col-md-3"><strong>إجمالي الإيرادات:</strong> <span id="spnCenterRevenue" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>إجمالي الخصومات:</strong> <span id="spnCenterDiscount" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>إجمالي المدفوع:</strong> <span id="spnCenterPaid" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>إجمالي المتبقي:</strong> <span id="spnCenterRemaining" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>عمولات الأطباء:</strong> <span id="spnCenterDoctorCommission" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>نصيب المركز:</strong> <span id="spnCenterNet" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>الخدمات الإضافية:</strong> <span id="spnCenterAdditional" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>الأشعة:</strong> <span id="spnCenterRadiology" runat="server">0</span> ج.م</div>
                <div class="col-md-3"><strong>الكشوفات:</strong> <span id="spnCenterVisits" runat="server">0</span></div>
                <div class="col-md-3"><strong>عدد المرضى:</strong> <span id="spnCenterPatients" runat="server">0</span></div>
                <div class="col-md-3"><strong>عدد الفواتير:</strong> <span id="spnCenterInvoices" runat="server">0</span></div>
                <div class="col-md-3"><strong>عدد الأطباء:</strong> <span id="spnCenterDoctors" runat="server">0</span></div>
            </div>
        </div>

        <!-- Modal -->
        <div class="modal fade" id="doctorDetailsModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><i class="fas fa-user-md me-2"></i>تفاصيل الطبيب</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" id="doctorDetailsBody"></div>
                </div>
            </div>
        </div>

    </div>

</asp:Content>