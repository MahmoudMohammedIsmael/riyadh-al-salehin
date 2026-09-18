<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master"
AutoEventWireup="true"
CodeBehind="SurgeryInvoicesList.aspx.cs"
Inherits="Riyadh_Al_Salehin.SurgeryInvoicesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .modal { z-index: 99999 !important; }
        .modal-backdrop { z-index: 99990 !important; }

        /* ===== Header ===== */
        .surg-inv-header {
            background: linear-gradient(135deg, #0b2b4a 0%, #1a5276 50%, #0d6efd 100%);
            border-radius: 20px;
            padding: 28px 32px;
            margin-bottom: 24px;
            color: #fff;
            position: relative;
            overflow: hidden;
        }
        .surg-inv-header::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -30%;
            width: 200px;
            height: 200px;
            background: rgba(255,255,255,0.05);
            border-radius: 50%;
        }
        .surg-inv-header::after {
            content: '';
            position: absolute;
            bottom: -60%;
            right: -20%;
            width: 250px;
            height: 250px;
            background: rgba(255,255,255,0.03);
            border-radius: 50%;
        }
        .surg-inv-header h3 { font-weight: 800; margin: 0; font-size: 26px; position: relative; z-index: 1; }
        .surg-inv-header small { opacity: 0.85; font-size: 14px; position: relative; z-index: 1; }

        /* ===== Stat Cards ===== */
        .stat-card {
            background: #fff;
            border-radius: 16px;
            padding: 20px;
            box-shadow: 0 2px 16px rgba(11, 43, 74, 0.06);
            border: 1px solid #eef2f7;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
        }
        .stat-card::before {
            content: '';
            position: absolute;
            top: 0; right: 0;
            width: 4px;
            height: 100%;
            background: var(--accent, #0d6efd);
            border-radius: 0 16px 16px 0;
        }
        .stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 28px rgba(11, 43, 74, 0.12);
        }
        .stat-label { font-size: 12px; color: #6c7a8a; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
        .stat-value { font-size: 22px; font-weight: 800; color: #0b2b4a; margin-top: 6px; }
        .stat-icon { font-size: 32px; opacity: 0.15; }

        /* ===== Filter Panel ===== */
        .filter-panel {
            background: #fff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 2px 16px rgba(11, 43, 74, 0.06);
            margin-bottom: 24px;
            overflow: hidden;
        }
        .filter-panel .filter-header {
            background: linear-gradient(135deg, #f8fafc, #eef2f7);
            padding: 14px 20px;
            border-bottom: 1px solid #e2e8f0;
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .filter-panel .filter-header i { color: #0d6efd; font-size: 18px; }
        .filter-panel .filter-header span { font-weight: 700; color: #0b2b4a; font-size: 15px; }
        .filter-panel .filter-body { padding: 20px; }
        .filter-panel label {
            font-weight: 700;
            color: #0b2b4a;
            font-size: 13px;
            margin-bottom: 6px;
            display: block;
        }
        .filter-panel label i { color: #0d6efd; margin-left: 4px; }
        .filter-panel .form-control,
        .filter-panel .form-select {
            border-radius: 10px;
            border: 1px solid #e2e8f0;
            padding: 10px 14px;
            font-size: 14px;
            transition: all 0.2s;
        }
        .filter-panel .form-control:focus,
        .filter-panel .form-select:focus {
            border-color: #0d6efd;
            box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
        }
        .btn-search {
            background: linear-gradient(135deg, #0d6efd, #0b5ed7);
            border: none;
            padding: 10px 28px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 14px;
            color: #fff;
            transition: all 0.2s;
        }
        .btn-search:hover { transform: translateY(-2px); box-shadow: 0 4px 16px rgba(13, 110, 253, 0.35); color: #fff; }
        .btn-reset {
            background: #fff;
            border: 2px solid #e2e8f0;
            padding: 10px 28px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 14px;
            color: #6c757d;
            transition: all 0.2s;
        }
        .btn-reset:hover { border-color: #dc3545; color: #dc3545; }

        /* ===== Table Card ===== */
        .table-card {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 2px 16px rgba(11, 43, 74, 0.06);
            border: 1px solid #eef2f7;
            overflow: hidden;
        }
        .table-card .card-header {
            background: linear-gradient(135deg, #0b2b4a 0%, #1a4a7a 100%);
            color: #fff;
            padding: 14px 20px;
            font-weight: 700;
            font-size: 15px;
            border: none;
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .table-card .card-header i { font-size: 18px; }
        .table-responsive-wrap { padding: 0; }
        .table-responsive-wrap .table { margin-bottom: 0; }
        .table-responsive-wrap .table th {
            background: #f8fafc;
            border-bottom: 2px solid #eef2f7;
            font-weight: 700;
            color: #0b2b4a;
            font-size: 12px;
            padding: 12px 10px;
            white-space: nowrap;
        }
        .table-responsive-wrap .table td {
            padding: 10px;
            vertical-align: middle;
            border-color: #f1f5f9;
            font-size: 13px;
        }
        .table-responsive-wrap .table tbody tr { transition: background 0.15s; }
        .table-responsive-wrap .table tbody tr:hover { background: #f8fafc; }

        /* ===== Badge Status ===== */
        .badge-status {
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 700;
            display: inline-block;
        }
        .bs-paid { background: #d1fae5; color: #065f46; }
        .bs-unpaid { background: #fee2e2; color: #991b1b; }
        .bs-partial { background: #fef3c7; color: #92400e; }
        .bs-unknown { background: #e5e7eb; color: #374151; }

        /* ===== Buttons ===== */
        .btn-print-sm {
            background: linear-gradient(135deg, #dc3545, #c82333);
            border: none;
            padding: 6px 14px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 12px;
            color: #fff;
            transition: all 0.2s;
        }
        .btn-print-sm:hover { transform: translateY(-1px); box-shadow: 0 3px 10px rgba(220, 53, 69, 0.3); color: #fff; }
        .btn-details-sm {
            background: linear-gradient(135deg, #0d6efd, #0b5ed7);
            border: none;
            padding: 6px 14px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 12px;
            color: #fff;
            transition: all 0.2s;
        }
        .btn-details-sm:hover { transform: translateY(-1px); box-shadow: 0 3px 10px rgba(13, 110, 253, 0.3); color: #fff; }
        .btn-supplies-sm {
            background: linear-gradient(135deg, #6f42c1, #5a32a3);
            border: none;
            padding: 6px 14px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 12px;
            color: #fff;
            transition: all 0.2s;
        }
        .btn-supplies-sm:hover { transform: translateY(-1px); box-shadow: 0 3px 10px rgba(111, 66, 193, 0.3); color: #fff; }

        /* ===== Details Panel ===== */
        .details-panel {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 2px 16px rgba(11, 43, 74, 0.08);
            border: 1px solid #eef2f7;
            margin-bottom: 24px;
            overflow: hidden;
        }
        .details-panel .panel-header {
            background: linear-gradient(135deg, #0d6efd, #0b5ed7);
            color: #fff;
            padding: 14px 20px;
            font-weight: 700;
            font-size: 15px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .details-panel .panel-body { padding: 20px; }

        /* ===== Supplies Table ===== */
        .supplies-section { margin-top: 16px; }
        .supplies-section h6 {
            font-weight: 700;
            color: #6f42c1;
            margin-bottom: 12px;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .supplies-table .table th {
            background: linear-gradient(135deg, #6f42c1, #5a32a3);
            color: #fff;
            font-size: 12px;
            padding: 10px;
        }
        .supplies-table .table td { padding: 8px 10px; font-size: 13px; }
        .supplies-table .table tbody tr:nth-child(even) { background: #faf8ff; }
        .supplies-table .table tbody tr:hover { background: #f3eefa; }

        /* ===== Action Buttons in Room Table ===== */
        .room-action-btns { display: flex; gap: 4px; flex-wrap: wrap; }
        .room-action-btns .btn { font-size: 11px; padding: 4px 8px; white-space: nowrap; }

        /* ===== Cost Breakdown Cards ===== */
        .cost-breakdown { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr)); gap: 12px; }
        .cost-item {
            background: #f8fafc;
            border-radius: 12px;
            padding: 14px;
            text-align: center;
            border: 1px solid #eef2f7;
        }
        .cost-item .cost-label { font-size: 11px; color: #6c7a8a; font-weight: 600; text-transform: uppercase; }
        .cost-item .cost-value { font-size: 18px; font-weight: 800; color: #0b2b4a; margin-top: 4px; }
        .cost-item .cost-icon { font-size: 24px; margin-bottom: 4px; }

        /* ===== Responsive ===== */
        @media (max-width: 767.98px) {
            .surg-inv-header { padding: 20px; border-radius: 14px; }
            .surg-inv-header h3 { font-size: 20px; }
            .stat-card { padding: 14px; }
            .stat-value { font-size: 18px; }
            .filter-panel .filter-body { padding: 14px; }
            .cost-breakdown { grid-template-columns: repeat(2, 1fr); }
        }
    </style>

    <script type="text/javascript">
        function printInvoice(id) {
            window.open('SurgeryInvoicePrint.aspx?id=' + id, '_blank');
        }

        function openAddSupplyModal(sid) {
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            currentSupplySurgeryId = sid;
            PageMethods.GetSupplies(onGetSuppliesSuccess, onAjaxError);
            var modal = new bootstrap.Modal(document.getElementById('supplyModal'));
            modal.show();
        }

        function dischargePatient(sid) {
            if (confirm('هل أنت متأكد من إنهاء إقامة هذا المريض في الغرفة؟')) {
                PageMethods.DischargePatientAjax(sid, function (res) {
                    if (res && res.Status === 'OK') {
                        alert('تم إنهاء إقامة المريض بنجاح');
                        location.reload();
                    } else {
                        alert('خطأ: ' + (res && res.Message ? res.Message : 'تعذر تنفيذ العملية'));
                    }
                }, onAjaxError);
            }
        }

        function confirmStay(sid) {
            if (confirm('هل أنت متأكد من تأكيد بقاء المريض لليوم الحالي؟')) {
                PageMethods.ConfirmStayAjax(sid, function (res) {
                    if (res && res.Status === 'OK') {
                        alert('تم تأكيد بقاء المريض بنجاح');
                        location.reload();
                    } else {
                        alert('خطأ: ' + (res && res.Message ? res.Message : 'تعذر تنفيذ العملية'));
                    }
                }, onAjaxError);
            }
        }

        function onAjaxError(err) {
            alert('خطأ في الاتصال: ' + (err.get_message ? err.get_message() : err));
        }

        var currentSupplySurgeryId = 0;

        function onGetSuppliesSuccess(result) {
            var ddl = document.getElementById('ddlSupplies');
            ddl.innerHTML = '';
            result.forEach(function (s) {
                var opt = document.createElement('option');
                opt.value = s.Id;
                opt.text = s.SupplyName + ' - ' + s.Price;
                opt.setAttribute('data-price', s.Price);
                ddl.appendChild(opt);
            });
            if (ddl.options.length > 0) {
                var p = ddl.options[0].getAttribute('data-price');
                document.getElementById('txtSupplyUnitPrice').value = p;
                computeSupplyTotal();
            }
        }

        document.addEventListener('change', function (e) {
            if (e.target && e.target.id === 'ddlSupplies') {
                var p = e.target.options[e.target.selectedIndex].getAttribute('data-price');
                document.getElementById('txtSupplyUnitPrice').value = p;
                computeSupplyTotal();
            }
        });

        document.getElementById && document.getElementById('txtSupplyQty') && document.getElementById('txtSupplyQty').addEventListener('input', computeSupplyTotal);
        document.getElementById && document.getElementById('txtSupplyUnitPrice') && document.getElementById('txtSupplyUnitPrice').addEventListener('input', computeSupplyTotal);

        function computeSupplyTotal() {
            var qty = parseFloat(document.getElementById('txtSupplyQty').value) || 0;
            var unit = parseFloat(document.getElementById('txtSupplyUnitPrice').value) || 0;
            document.getElementById('txtSupplyTotal').value = (qty * unit).toFixed(2);
        }

        document.addEventListener('DOMContentLoaded', function () {
            var btn = document.getElementById('btnSaveSupply');
            if (btn) btn.addEventListener('click', function () {
                var supplyId = parseInt(document.getElementById('ddlSupplies').value);
                var qty = parseInt(document.getElementById('txtSupplyQty').value) || 0;
                var unit = parseFloat(document.getElementById('txtSupplyUnitPrice').value) || 0;
                if (!supplyId || qty <= 0) { alert('اختر مستلزم وادخل كمية صحيحة'); return; }
                PageMethods.AddSupplyAjax(currentSupplySurgeryId, supplyId, qty, unit, onAddSupplySuccess, onAjaxError);
            });
        });

        function onAddSupplySuccess(result) {
            if (result && result.Status === 'OK') {
                location.reload();
            } else {
                alert('خطأ: ' + (result && result.Message ? result.Message : 'failed'));
            }
        }

        function addRoomCharge(sid) {
            var extraEl = document.getElementById('extraDays_' + sid);
            var priceEl = document.getElementById('dailyPrice_' + sid);
            var extra = extraEl ? parseInt(extraEl.value) || 0 : 0;
            var price = priceEl ? parseFloat(priceEl.value) || 0 : 0;
            if (extra <= 0) { alert('حدد عدد الأيام الإضافية'); return; }
            PageMethods.AddRoomChargeAjax(sid, extra, price, function (res) { if (res.Status === 'OK') location.reload(); else alert('خطأ: ' + res.Message); }, onAjaxError);
        }
    </script>

    <div class="container-fluid mt-3" dir="rtl">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <!-- ===== Header ===== -->
        <div class="surg-inv-header">
            <div class="d-flex justify-content-between align-items-center flex-wrap">
                <div>
                    <h3><i class="bi bi-hospital"></i> حسابات العمليات الجراحية</h3>
                    <small>عرض ومتابعة جميع فواتير وتفاصيل العمليات الجراحية - مستشفى الرياض الصالحين</small>
                </div>
                <div class="text-start">
                    <asp:Label ID="lblRecordCount" runat="server" CssClass="badge bg-light text-dark" Style="font-size:14px;padding:8px 16px;" />
                </div>
            </div>
        </div>

        <!-- ===== Filter Panel ===== -->
        <div class="filter-panel">
            <div class="filter-header">
                <i class="bi bi-funnel-fill"></i>
                <span>خيار البحث والفلترة</span>
            </div>
            <div class="filter-body">
                <div class="row g-3 align-items-end">
                    <div class="col-12 col-md-3">
                        <label><i class="bi bi-person-fill"></i> اسم المريض</label>
                        <asp:TextBox ID="txtPatientName" runat="server" CssClass="form-control" placeholder="ابحث باسم المريض..." />
                    </div>
                    <div class="col-12 col-md-3">
                        <label><i class="bi bi-person-badge-fill"></i> اسم الطبيب</label>
                        <asp:TextBox ID="txtDoctorName" runat="server" CssClass="form-control" placeholder="ابحث باسم الطبيب..." />
                    </div>
                    <div class="col-6 col-md-2">
                        <label><i class="bi bi-calendar-event"></i> من تاريخ</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label><i class="bi bi-calendar-check"></i> إلى تاريخ</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-6 col-md-1">
                        <asp:Button ID="btnSearch" runat="server" Text="🔍 بحث" CssClass="btn btn-search w-100" OnClick="btnSearch_Click" />
                    </div>
                    <div class="col-6 col-md-1">
                        <asp:Button ID="btnRefresh" runat="server" Text="🔄 عرض الكل" CssClass="btn btn-reset w-100" OnClick="btnRefresh_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- ===== Stat Cards Row 1 ===== -->
        <div class="row g-3 mb-3">
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#0d6efd;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">إجمالي الفواتير</div>
                            <div class="stat-value"><asp:Label ID="lblTotal" runat="server" /></div>
                        </div>
                        <div class="stat-icon">💰</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#198754;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">إجمالي المدفوع</div>
                            <div class="stat-value"><asp:Label ID="lblPaid" runat="server" /></div>
                        </div>
                        <div class="stat-icon">✅</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#dc3545;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">إجمالي المتبقي</div>
                            <div class="stat-value"><asp:Label ID="lblRemain" runat="server" /></div>
                        </div>
                        <div class="stat-icon">⏳</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#ffc107;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">إجمالي الخصومات</div>
                            <div class="stat-value"><asp:Label ID="lblDiscount" runat="server" /></div>
                        </div>
                        <div class="stat-icon">🏷️</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ===== Stat Cards Row 2 ===== -->
        <div class="row g-3 mb-4">
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#6f42c1;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">أجر الطبيب</div>
                            <div class="stat-value"><asp:Label ID="lblDoctorCost" runat="server" /></div>
                        </div>
                        <div class="stat-icon">👨‍⚕️</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#0dcaf0;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">رسوم الغرف</div>
                            <div class="stat-value"><asp:Label ID="lblRoomCost" runat="server" /></div>
                        </div>
                        <div class="stat-icon">🛏️</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#fd7e14;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">المستلزمات</div>
                            <div class="stat-value"><asp:Label ID="lblSuppliesCost" runat="server" /></div>
                        </div>
                        <div class="stat-icon">🧪</div>
                    </div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#6c757d;">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="stat-label">تكاليف أخرى</div>
                            <div class="stat-value"><asp:Label ID="lblOtherCost" runat="server" /></div>
                        </div>
                        <div class="stat-icon">📋</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ===== Invoice Details Panel ===== -->
        <asp:Panel ID="pnlInvoiceDetails" runat="server" Visible="false">
            <div class="details-panel">
                <div class="panel-header">
                    <span><i class="bi bi-receipt"></i> تفاصيل الفاتورة رقم: <asp:Label ID="lblSelectedInvoiceId" runat="server" /></span>
                    <asp:Button ID="btnCloseDetails" runat="server" Text="✕ إغلاق" CssClass="btn btn-sm btn-light" OnClick="btnCloseDetails_Click" />
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-12">
                            <h6 style="font-weight:700;color:#0b2b4a;margin-bottom:12px;"><i class="bi bi-cash-stack"></i> تفاصيل التكاليف</h6>
                            <asp:GridView ID="gvInvoiceDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" GridLines="None">
                                <HeaderStyle CssClass="table-primary" />
                                <Columns>
                                    <asp:BoundField DataField="Item" HeaderText="البند" />
                                    <asp:BoundField DataField="Amount" HeaderText="المبلغ" DataFormatString="{0:N2}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <!-- المستلزمات الخاصة بالعملية -->
                    <div class="supplies-section" id="suppliesSection" runat="server" visible="false">
                        <h6><i class="bi bi-box-seam-fill"></i> المستلزمات المستخدمة في العملية</h6>
                        <div class="supplies-table">
                            <asp:GridView ID="gvSurgerySupplies" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="SupplyName" HeaderText="اسم المستلزم" />
                                    <asp:BoundField DataField="UsedQuantity" HeaderText="الكمية" />
                                    <asp:BoundField DataField="UnitPrice" HeaderText="سعر الوحدة" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="TotalPrice" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- ===== Occupied Rooms ===== -->
        <div class="table-card mb-3">
            <div class="card-header"><i class="bi bi-door-open-fill"></i> المرضى الموجودون في غرف العمليات</div>
            <div class="table-responsive p-3">
                <asp:GridView ID="gvOccupiedRooms" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False" DataKeyNames="SurgeryId">
                    <Columns>
                        <asp:BoundField DataField="SurgeryId" HeaderText="رقم العملية" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="RoomName" HeaderText="الغرفة" />
                        <asp:BoundField DataField="SurgeryDate" HeaderText="تاريخ الدخول" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:TemplateField HeaderText="أيام الإقامة">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DaysStayed") %>' />
                                <span style='<%# (bool)Eval("NeedsConfirmation") ? "" : "display:none;" %>' class="badge bg-warning text-dark ms-1" title="يجب تأكيد بقاء المريض لليوم الجديد"><i class="bi bi-exclamation-triangle"></i> تأكيد؟</span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="سعر الغرفة/يوم">
                            <ItemTemplate><asp:Label runat="server" Text='<%# Eval("DailyPrice", "{0:N2}") %>' /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الإضافي">
                            <ItemTemplate>
                                <input type="number" min="0" id='extraDays_<%# Eval("SurgeryId") %>' class="form-control form-control-sm" style="width:70px;display:inline-block;margin-left:4px;" placeholder="أيام" />
                                <input type="number" step="0.01" min="0" id='dailyPrice_<%# Eval("SurgeryId") %>' value='<%# Eval("DailyPrice") %>' class="form-control form-control-sm" style="width:90px;display:inline-block;" placeholder="سعر/يوم" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <div class="room-action-btns">
                                    <button type="button" class="btn btn-sm btn-info" onclick="confirmStay(<%# Eval("SurgeryId") %>)" style='<%# (bool)Eval("NeedsConfirmation") ? "" : "display:none;" %>'><i class="bi bi-check-circle"></i> تأكيد</button>
                                    <button type="button" class="btn btn-sm btn-primary" onclick="addRoomCharge(<%# Eval("SurgeryId") %>)"><i class="bi bi-plus-circle"></i> رسوم غرفة</button>
                                    <button type="button" class="btn btn-sm btn-warning" onclick="openAddSupplyModal(<%# Eval("SurgeryId") %>)"><i class="bi bi-plus-square"></i> مستلزمات</button>
                                    <button type="button" class="btn btn-sm btn-danger" onclick="dischargePatient(<%# Eval("SurgeryId") %>)"><i class="bi bi-box-arrow-left"></i> مغادرة</button>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- ===== All Supplies Details Table ===== -->
        <div class="table-card mb-4">
            <div class="card-header" style="background: linear-gradient(135deg, #2b5876 0%, #4e4376 100%);"><i class="bi bi-box-seam-fill"></i> تفاصيل المستلزمات الطبية المنصرفة</div>
            <div class="table-responsive p-3">
                <asp:GridView ID="gvAllSuppliesDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover align-middle mb-0" GridLines="None">
                    <HeaderStyle CssClass="table-dark" />
                    <Columns>
                        <asp:BoundField DataField="SupplyName" HeaderText="اسم المستلزم" />
                        <asp:BoundField DataField="BaseQuantity" HeaderText="الكمية الأساسية" />
                        <asp:BoundField DataField="DispensedQuantity" HeaderText="الكمية المنصرفة" />
                        <asp:BoundField DataField="UnitPrice" HeaderText="سعر الوحدة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalBaseCost" HeaderText="التكلفة الكلية" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="DispensedCost" HeaderText="تكلفة المنصرف" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="PatientName" HeaderText="اسم المريض المنصرف له" />
                        <asp:BoundField DataField="DispensedBy" HeaderText="المستخدم الصارف" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-4">
                            <i class="bi bi-box-seam" style="font-size:36px;opacity:0.4;"></i>
                            <p class="mt-2 mb-0 fw-bold">لا توجد بيانات مستلزمات منصرفة حتى الآن</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- ===== All Rooms Details Table ===== -->
        <div class="table-card mb-4">
            <div class="card-header" style="background: linear-gradient(135deg, #0f766e 0%, #0d9488 100%); color: #fff;"><i class="bi bi-building-check"></i> تفاصيل الغرف والأيام المشغولة وتكلفتها</div>
            <div class="table-responsive p-3">
                <asp:GridView ID="gvAllRoomsDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover align-middle mb-0" GridLines="None">
                    <HeaderStyle CssClass="table-dark" />
                    <Columns>
                        <asp:BoundField DataField="RoomName" HeaderText="اسم الغرفة" />
                        <asp:BoundField DataField="PatientName" HeaderText="اسم المريض" />
                        <asp:BoundField DataField="OccupiedDates" HeaderText="بتاريخ الأيام الموجود فيها" />
                        <asp:BoundField DataField="DaysStayed" HeaderText="عدد الأيام المشغولة" />
                        <asp:BoundField DataField="DailyPrice" HeaderText="تكلفة اليوم" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalRoomCost" HeaderText="إجمالي تكلفة الغرفة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="RoomStatus" HeaderText="حالة الإقامة / المغادرة" ItemStyle-CssClass="fw-bold" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-4">
                            <i class="bi bi-door-closed" style="font-size:36px;opacity:0.4;"></i>
                            <p class="mt-2 mb-0 fw-bold">لا توجد بيانات غرف مشغولة حالياً</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- ===== Supplies Modal ===== -->
        <div id="supplyModal" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header" style="background:linear-gradient(135deg,#6f42c1,#5a32a3);color:#fff;">
                        <h5 class="modal-title"><i class="bi bi-box-seam"></i> إضافة مستلزمات للعملية</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="fw-bold">المستلزم</label>
                            <select id="ddlSupplies" class="form-select"></select>
                        </div>
                        <div class="row g-2">
                            <div class="col-6">
                                <label class="fw-bold">الكمية</label>
                                <input type="number" id="txtSupplyQty" class="form-control" value="1" min="1" />
                            </div>
                            <div class="col-6">
                                <label class="fw-bold">سعر الوحدة</label>
                                <input type="number" id="txtSupplyUnitPrice" class="form-control" step="0.01" />
                            </div>
                        </div>
                        <div class="mt-2">
                            <label class="fw-bold">الإجمالي</label>
                            <input type="text" id="txtSupplyTotal" class="form-control fw-bold" readonly style="background:#f8fafc;" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                        <button type="button" class="btn btn-primary" id="btnSaveSupply" style="background:linear-gradient(135deg,#6f42c1,#5a32a3);border:none;"><i class="bi bi-check-lg"></i> حفظ المستلزم</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- ===== Main Invoices Table ===== -->
        <div class="table-card mb-4">
            <div class="card-header"><i class="bi bi-table"></i> قائمة الفواتير التفصيلية</div>
            <div class="table-responsive">
                <asp:GridView ID="gvInvoices" runat="server" AutoGenerateColumns="False" CssClass="table table-hover align-middle mb-0" GridLines="None" DataKeyNames="Id" OnRowCommand="gvInvoices_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="رقم الفاتورة" />
                        <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                        <asp:BoundField DataField="SurgeryName" HeaderText="العملية" />
                        <asp:BoundField DataField="DoctorCost" HeaderText="أجر الطبيب" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="RoomCost" HeaderText="الغرف" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="SuppliesCost" HeaderText="المستلزمات" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="OtherCost" HeaderText="أخرى" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Discount" HeaderText="الخصم" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="PaidAmount" HeaderText="المدفوع" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="RemainingAmount" HeaderText="المتبقي" DataFormatString="{0:N2}" ItemStyle-CssClass="fw-bold text-danger" HeaderStyle-CssClass="text-danger fw-bold" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge-status <%# GetStatusBadgeClass(Eval("PaymentStatus").ToString()) %>'><%# Eval("PaymentStatus") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <div class="d-flex gap-1 flex-wrap">
                                    <asp:LinkButton runat="server" CommandName="ShowDetails" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-details-sm" title="عرض التفاصيل"><i class="bi bi-eye"></i></asp:LinkButton>
                                    <button type="button" class="btn btn-print-sm" onclick="printInvoice(<%# Eval("Id") %>)" title="طباعة الفاتورة"><i class="bi bi-printer"></i></button>
                                    <asp:LinkButton runat="server" Text="💰 دفع المتبقي" CssClass="btn btn-success btn-sm" CommandName="PayRemaining" CommandArgument='<%# Eval("Id") + "|" + Eval("SurgeryId") + "|" + Eval("RemainingAmount") + "|" + Eval("PatientName") + "|" + Eval("DoctorName") + "|" + Eval("SurgeryName") %>' OnClientClick="return confirm('هل تريد إنشاء فاتورة دفع للمتبقي فقط؟');" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-5">
                            <i class="bi bi-inbox" style="font-size:48px;opacity:0.3;"></i>
                            <p class="mt-2 fw-bold">لا توجد فواتير مطابقة لمعايير البحث</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- ===== Summary Row ===== -->
        <div class="row g-3 mb-4">
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#343a40;">
                    <div class="stat-label">إجمالي الخصومات</div>
                    <div class="stat-value"><asp:Label ID="lblSumDiscount" runat="server" /></div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#198754;">
                    <div class="stat-label">إجمالي الفواتير</div>
                    <div class="stat-value"><asp:Label ID="lblSumTotal" runat="server" /></div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#0d6efd;">
                    <div class="stat-label">إجمالي المدفوع</div>
                    <div class="stat-value"><asp:Label ID="lblSumPaid" runat="server" /></div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="stat-card" style="--accent:#dc3545;">
                    <div class="stat-label">إجمالي المتبقي</div>
                    <div class="stat-value"><asp:Label ID="lblSumRemain" runat="server" /></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>