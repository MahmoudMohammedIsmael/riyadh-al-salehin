<%@ Page Title="التقارير المحاسبية" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="AccountsReport.aspx.cs" Inherits="Riyadh_Al_Salehin.AccountsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
        /* نفس الأنماط السابقة */
        .stat-card {
            background: #fff;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 0 15px rgba(0,0,0,0.05);
            border-right: 5px solid #0d6efd;
            margin-bottom: 20px;
            transition: 0.3s;
        }
        .stat-card:hover { transform: translateY(-3px); box-shadow: 0 8px 25px rgba(0,0,0,0.1); }
        .stat-number { font-size: 32px; font-weight: 700; color: #0d6efd; }
        .stat-label { color: #6c757d; font-weight: 500; }
        .stat-icon { font-size: 28px; color: #0d6efd; opacity: 0.3; float: left; }
        .filter-box { background: #f8f9fc; padding: 20px; border-radius: 10px; margin-bottom: 30px; }
        .table th { background: #0d6efd; color: #fff; }
        .chart-container { background: #fff; border-radius: 10px; padding: 15px; box-shadow: 0 0 10px rgba(0,0,0,0.05); }
        .badge-consult { background: #0d6efd; }
        .badge-exam { background: #198754; }
        .badge-other { background: #ffc107; color: #000; }
    </style>

    <div class="container mt-4" dir="rtl">
        <h3 class="mb-3"><i class="bi bi-graph-up-arrow"></i> التقارير المحاسبية</h3>

        <!-- فلتر التاريخ والطبيب -->
        <div class="filter-box">
            <div class="row g-3 align-items-end">
                <div class="col-md-3">
                    <label class="form-label fw-bold">من تاريخ</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold">إلى تاريخ</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold">الطبيب</label>
                    <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- جميع الأطباء --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <div class="row g-2">
                        <div class="col-6">
                            <asp:Button ID="btnShow" runat="server" Text="عرض التقرير" CssClass="btn btn-primary w-100" OnClick="btnShow_Click" />
                        </div>
                        <div class="col-6">
                            <asp:Button ID="btnPrint" runat="server" Text="طباعة" CssClass="btn btn-secondary w-100" OnClientClick="window.print(); return false;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- بطاقات الإحصائيات -->
        <div class="row" id="statsCards" runat="server">
            <div class="col-lg-3 col-md-6">
                <div class="stat-card">
                    <div class="stat-icon"><i class="bi bi-clipboard2-check"></i></div>
                    <div class="stat-label">إجمالي الكشفات</div>
                    <div class="stat-number"><asp:Label ID="lblTotalVisits" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="stat-card" style="border-right-color:#198754;">
                    <div class="stat-icon" style="color:#198754;"><i class="bi bi-person"></i></div>
                    <div class="stat-label">كشف عادي</div>
                    <div class="stat-number" style="color:#198754;"><asp:Label ID="lblNormalVisits" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="stat-card" style="border-right-color:#0d6efd;">
                    <div class="stat-icon" style="color:#0d6efd;"><i class="bi bi-chat"></i></div>
                    <div class="stat-label">استشارة</div>
                    <div class="stat-number" style="color:#0d6efd;"><asp:Label ID="lblConsultVisits" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="stat-card" style="border-right-color:#ffc107;">
                    <div class="stat-icon" style="color:#ffc107;"><i class="bi bi-plus-circle"></i></div>
                    <div class="stat-label">خدمات إضافية</div>
                    <div class="stat-number" style="color:#ffc107;"><asp:Label ID="lblExtraVisits" runat="server" Text="0" /></div>
                </div>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-md-4">
                <div class="stat-card" style="border-right-color:#6610f2;">
                    <div class="stat-label">إجمالي الإيرادات</div>
                    <div class="stat-number" style="color:#6610f2;"><asp:Label ID="lblTotalRevenue" runat="server" Text="0.00" /> ج.م</div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card" style="border-right-color:#d63384;">
                    <div class="stat-label">نصيب الطبيب</div>
                    <div class="stat-number" style="color:#d63384;"><asp:Label ID="lblDoctorShare" runat="server" Text="0.00" /> ج.م</div>
                    <small class="text-muted">نسبة: <asp:Label ID="lblDoctorPercent" runat="server" Text="0%" /></small>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card" style="border-right-color:#fd7e14;">
                    <div class="stat-label">نصيب المركز</div>
                    <div class="stat-number" style="color:#fd7e14;"><asp:Label ID="lblCenterShare" runat="server" Text="0.00" /> ج.م</div>
                    <small class="text-muted">نسبة: <asp:Label ID="lblCenterPercent" runat="server" Text="0%" /></small>
                </div>
            </div>
        </div>

        <!-- معلومات الطبيب المحدد -->
        <div class="row mt-3" id="doctorInfo" runat="server" visible="false">
            <div class="col-md-12">
                <div class="alert alert-info">
                    <strong><asp:Label ID="lblSelectedDoctor" runat="server" Text="" /></strong>
                    <br />
                    <small><asp:Label ID="lblDoctorCommissionRate" runat="server" Text="" /></small>
                </div>
            </div>
        </div>

        <!-- الرسم البياني -->
        <div class="row mt-4">
            <div class="col-md-8">
                <div class="chart-container">
                    <canvas id="revenueChart" style="width:100%; height:300px;"></canvas>
                </div>
            </div>
            <div class="col-md-4">
                <div class="chart-container">
                    <canvas id="visitsChart" style="width:100%; height:300px;"></canvas>
                </div>
            </div>
        </div>

        <!-- الجدول التفصيلي -->
        <div class="mt-4">
            <h5><i class="bi bi-table"></i> تفاصيل الخدمات</h5>
            <div class="table-responsive">
                <asp:GridView ID="gvDetails" runat="server" CssClass="table table-bordered table-hover" AutoGenerateColumns="False" EmptyDataText="لا توجد بيانات">
                    <Columns>
                        <asp:BoundField DataField="ServiceName" HeaderText="اسم الخدمة" />
                        <asp:BoundField DataField="Count" HeaderText="عدد المرات" DataFormatString="{0:N0}" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="الإجمالي" DataFormatString="{0:N2} ج.م" />
                        <asp:BoundField DataField="DoctorTotal" HeaderText="نصيب الطبيب" DataFormatString="{0:N2} ج.م" />
                        <asp:BoundField DataField="CenterTotal" HeaderText="نصيب المركز" DataFormatString="{0:N2} ج.م" />
                        <asp:BoundField DataField="DoctorPercent" HeaderText="نسبة الطبيب" DataFormatString="{0:N2}%" />
                        <asp:BoundField DataField="CenterPercent" HeaderText="نسبة المركز" DataFormatString="{0:N2}%" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3" />
    </div>

    <script>
function initCharts() {
    var labels = window.ChartLabels || [];
    var revenueData = window.ChartRevenue || [];
    var visitsData = window.ChartVisits || [];

    if (labels.length === 0) return;

    var canvas1 = document.getElementById('revenueChart');
    var canvas2 = document.getElementById('visitsChart');
    if (!canvas1 || !canvas2) return;

    var ctx1 = canvas1.getContext('2d');
    new Chart(ctx1, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'الإيرادات (ج.م)',
                data: revenueData,
                backgroundColor: ['#0d6efd', '#198754', '#ffc107', '#dc3545'],
                borderColor: ['#0d6efd', '#198754', '#ffc107', '#dc3545'],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });

    var ctx2 = canvas2.getContext('2d');
    new Chart(ctx2, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: visitsData,
                backgroundColor: ['#0d6efd', '#198754', '#ffc107', '#dc3545'],
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: 'bottom' } }
        }
    });
}

window.onload = function () {
    if (typeof initCharts === 'function') initCharts();
};
</script>
</asp:Content>