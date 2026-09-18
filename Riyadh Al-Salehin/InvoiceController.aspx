<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="InvoiceController.aspx.cs" Inherits="Riyadh_Al_Salehin.InvoiceController" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.rtl.min.css" />
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
<script src="https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js"></script>

<style>
    body { background: #f0f2f5; font-family: 'Segoe UI', Tahoma, Arial, sans-serif; }

    .page-title-bar {
        background: linear-gradient(135deg, #0d6efd, #0a58ca);
        color: #fff; padding: 16px 24px; border-radius: 12px 12px 0 0;
        display: flex; align-items: center; gap: 10px;
        box-shadow: 0 4px 12px rgba(13,110,253,.25);
    }
    .page-title-bar h4 { color: #fff; font-size: 20px; font-weight: 700; margin: 0; }

    .control-panel { background: #fff; border-radius: 0 0 12px 12px; padding: 24px; box-shadow: 0 4px 20px rgba(0,0,0,.08); }
    .section-title { font-weight: 700; color: #2c3e50; border-bottom: 2px solid #0d6efd; padding-bottom: 6px; margin-bottom: 16px; font-size: 15px; }
    .section-title i { margin-left: 6px; color: #0d6efd; }

    .preview-container {
        background: #fff; border-radius: 12px; padding: 20px;
        box-shadow: 0 4px 20px rgba(0,0,0,.08); min-height: 600px;
        display: flex; align-items: flex-start; justify-content: center;
        position: relative;
    }
    .preview-wrapper {
        background: #e8e8e8; padding: 20px; border-radius: 8px; min-height: 550px;
        display: flex; align-items: flex-start; justify-content: center; width: 100%;
        overflow: auto;
    }

    /* Invoice Preview Styles */
    .inv-preview {
        background: #fff; color: #000; font-family: Tahoma, Arial, sans-serif;
        padding: 3mm; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,.15);
        border: 1px solid #ddd;
    }
    .inv-preview.w-58 { width: 58mm; }
    .inv-preview.w-80 { width: 80mm; }
    .inv-preview.w-a4 { width: 190mm; padding: 10mm; }

    .inv-title { text-align: center; border-bottom: 1px dashed #000; padding-bottom: 4px; margin-bottom: 6px; }
    .inv-title h2 { margin: 0; font-size: 16px; font-weight: bold; }
    .inv-title .inv-sub { font-size: 12px; margin-top: 2px; }
    .inv-title .inv-custom-header { font-size: 13px; color: #333; margin-top: 4px; font-style: italic; }

    .inv-table { width: 100%; border-collapse: collapse; table-layout: fixed; margin-bottom: 6px; }
    .inv-table td { padding: 2px 0; font-size: 12px; vertical-align: top; word-wrap: break-word; }
    .inv-table td:first-child { font-weight: bold; white-space: nowrap; }

    .inv-custom-line {
        padding: 3px 0; font-size: 12px; border-bottom: 1px dotted #ccc;
        display: flex; justify-content: space-between;
    }
    .inv-custom-line .line-label { font-weight: bold; }
    .inv-custom-line .line-value { text-align: left; }

    .inv-queue-label { text-align: center; font-size: 11px; margin-top: 6px; }
    .inv-queue-big {
        text-align: center; font-size: 34px; font-weight: bold;
        border: 2px solid #000; border-radius: 4px;
        margin: 4px 0 8px 0; padding: 4px 0; line-height: 1.1;
    }

    .inv-barcode { text-align: center; margin-top: 4px; }
    .inv-barcode svg { max-width: 100%; }
    .inv-barcode canvas { max-width: 100%; }

    .inv-footer { text-align: center; font-size: 10px; border-top: 1px dashed #000; margin-top: 6px; padding-top: 4px; }
    .inv-custom-footer { text-align: center; font-size: 11px; color: #555; margin-top: 4px; font-style: italic; }
    .inv-custom-watermark {
        text-align: center; font-size: 10px; color: #999;
        margin-top: 8px; border-top: 1px dotted #ccc; padding-top: 4px;
    }

    .inv-items-table { width: 100%; border-collapse: collapse; margin-top: 4px; }
    .inv-items-table th, .inv-items-table td { font-size: 11px; padding: 2px 1px; text-align: right; border-bottom: 1px dashed #ccc; }
    .inv-items-table th { border-bottom: 1px solid #000; }
    .inv-items-table td.num, .inv-items-table th.num { text-align: center; }

    .inv-total-row {
        display: flex; justify-content: space-between; font-size: 13px; font-weight: bold;
        border-top: 1px solid #000; margin-top: 4px; padding-top: 4px;
    }

    /* Print styles */
    @media print {
        body * { visibility: hidden !important; }
        #printArea, #printArea * { visibility: visible !important; }
        #printArea { position: absolute; left: 0; top: 0; }
    }

    .btn-action { padding: 10px 24px; border-radius: 8px; font-weight: 700; transition: all .2s; border: none; font-size: 14px; }
    .btn-action:hover { transform: translateY(-1px); box-shadow: 0 4px 12px rgba(0,0,0,.15); }

    .custom-lines-list { max-height: 200px; overflow-y: auto; }
    .custom-line-item { display: flex; gap: 6px; margin-bottom: 6px; align-items: center; }
    .custom-line-item input { flex: 1; font-size: 13px; }
    .custom-line-item .btn-remove { background: #dc3545; color: #fff; border: none; border-radius: 50%; width: 26px; height: 26px; cursor: pointer; font-size: 12px; }

    .font-size-preview { font-size: 11px; color: #666; }
</style>

<div class="container-fluid py-4" dir="rtl">

    <div class="page-title-bar mb-0">
        <i class="fas fa-print" style="font-size:22px;"></i>
        <h4>لوحة تحكم الفواتير المتقدمة</h4>
    </div>

    <div class="control-panel mb-4">

        <asp:HiddenField ID="hfInvoiceData" runat="server" />
        <asp:HiddenField ID="hfCustomLines" runat="server" />

        <asp:Panel ID="pnlMessage" runat="server" CssClass="alert alert-warning mb-3" Visible="false">
            <asp:Label ID="lblMessage" runat="server" />
        </asp:Panel>

        <div class="row g-3 mb-4">
            <div class="col-md-3">
                <label class="form-label fw-bold"><i class="fas fa-file-invoice me-1"></i>نوع الفاتورة</label>
                <asp:DropDownList ID="ddlInvoiceType" runat="server" CssClass="form-select" onchange="loadInvoice();">
                    <asp:ListItem Value="Consultation" Text="كشف طبي" />
                    <asp:ListItem Value="Surgery" Text="عملية جراحية" />
                    <asp:ListItem Value="Xray" Text="أشعة" />
                    <asp:ListItem Value="Additional" Text="فاتورة إضافية" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label fw-bold"><i class="fas fa-hashtag me-1"></i>رقم الفاتورة</label>
                <div class="input-group">
                    <asp:TextBox ID="txtInvoiceId" runat="server" CssClass="form-control" placeholder="رقم الفاتورة" TextMode="Number" />
                    <button type="button" class="btn btn-primary" onclick="loadInvoice();" title="بحث"><i class="fas fa-search"></i></button>
                </div>
            </div>
            <div class="col-md-2">
                <label class="form-label fw-bold"><i class="fas fa-expand me-1"></i>حجم الورق</label>
                <asp:DropDownList ID="ddlPaperSize" runat="server" CssClass="form-select" onchange="applyPaperSize();">
                    <asp:ListItem Value="58mm" Text="58 مم (صغير)" />
                    <asp:ListItem Value="80mm" Text="80 مم (حراري)" Selected="True" />
                    <asp:ListItem Value="A4" Text="A4 (عادي)" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label fw-bold"><i class="fas fa-cog me-1"></i>نوع الطباعة</label>
                <asp:DropDownList ID="ddlPrintType" runat="server" CssClass="form-select" onchange="applyPaperSize();">
                    <asp:ListItem Value="Thermal" Text="حرارية" />
                    <asp:ListItem Value="Standard" Text="ليزر / حبر" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label fw-bold"><i class="fas fa-text-height me-1"></i>حجم الخط</label>
                <select id="ddlFontSize" class="form-select" onchange="applyFontSize();">
                    <option value="10">10px (صغير جداً)</option>
                    <option value="11">11px (صغير)</option>
                    <option value="12" selected>12px (متوسط)</option>
                    <option value="13">13px (كبير)</option>
                    <option value="14">14px (أكبر)</option>
                    <option value="16">16px (كبير جداً)</option>
                </select>
            </div>
            <div class="col-md-1 d-flex align-items-end">
                <button type="button" class="btn btn-success btn-action w-100" onclick="printInvoice();">
                    <i class="fas fa-print"></i> طباعة
                </button>
            </div>
        </div>

        <!-- خيارات العرض -->
        <div class="section-title"><i class="fas fa-sliders-h"></i>خيارات العرض</div>
        <div class="row g-3 mb-4">
            <div class="col-md-6">
                <div class="row g-2">
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowHeader" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowHeader">الترويسة (اسم المركز)</label>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowBarcode" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowBarcode">الباركود</label>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowFooter" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowFooter">التذييل</label>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowQueue" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowQueue">رقم الانتظار</label>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowPaymentStatus" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowPaymentStatus">حالة الدفع</label>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chkShowAmountDetails" checked onchange="renderPreview();" />
                            <label class="form-check-label fw-bold" for="chkShowAmountDetails">تفاصيل المبالغ</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="row g-2">
                    <div class="col-12">
                        <label class="form-label fw-bold mb-1" style="font-size:13px;">نص الترويسة الإضافي</label>
                        <input type="text" id="txtHeaderExtra" class="form-control form-control-sm" placeholder="نص إضافي أسفل اسم المركز" oninput="renderPreview();" />
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-bold mb-1" style="font-size:13px;">نص التذييل الإضافي</label>
                        <input type="text" id="txtFooterExtra" class="form-control form-control-sm" placeholder="نص إضافي في التذييل" oninput="renderPreview();" />
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-bold mb-1" style="font-size:13px;">الختم / العلامة المائية</label>
                        <input type="text" id="txtWatermark" class="form-control form-control-sm" placeholder="مثال: COPY / صورة" oninput="renderPreview();" />
                    </div>
                </div>
            </div>
        </div>

        <!-- إضافة حقول مخصصة -->
        <div class="section-title"><i class="fas fa-plus-circle"></i>إضافة حقول مخصصة</div>
        <div class="row g-3">
            <div class="col-md-8">
                <div class="custom-lines-list" id="customLinesContainer"></div>
                <button type="button" class="btn btn-sm btn-outline-primary mt-2" onclick="addCustomLine();">
                    <i class="fas fa-plus"></i> إضافة حقل جديد
                </button>
            </div>
            <div class="col-md-4">
                <label class="form-label fw-bold" style="font-size:13px;">موقع الحقول المخصصة</label>
                <select id="ddlCustomLinesPosition" class="form-select form-select-sm" onchange="renderPreview();">
                    <option value="top">أعلى الفاتورة</option>
                    <option value="middle" selected>أسفل البيانات الأساسية</option>
                    <option value="bottom">أسفل الفاتورة</option>
                </select>
            </div>
        </div>
    </div>

    <!-- المعاينة -->
    <div class="preview-container">
        <div class="w-100">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="fw-bold mb-0"><i class="fas fa-eye me-2"></i>المعاينة المباشرة</h5>
                <div>
                    <button type="button" class="btn btn-sm btn-outline-secondary me-2" onclick="zoomIn();"><i class="fas fa-search-plus"></i></button>
                    <button type="button" class="btn btn-sm btn-outline-secondary me-2" onclick="zoomOut();"><i class="fas fa-search-minus"></i></button>
                    <button type="button" class="btn btn-sm btn-outline-primary" onclick="printInvoice();"><i class="fas fa-print"></i> طباعة</button>
                </div>
            </div>
            <div class="preview-wrapper" id="previewWrapper">
                <div id="previewContent" class="inv-preview w-80">
                    <div style="display:flex;align-items:center;justify-content:center;height:400px;color:#aaa;">
                        <div class="text-center">
                            <i class="fas fa-file-invoice fa-4x mb-3"></i>
                            <p>أدخل رقم الفاتورة واضغط بحث لعرض المعاينة</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- منطقة الطباعة المخفية -->
    <div id="printArea" style="display:none;"></div>

</div>

<script>
    var invoiceData = null;
    var previewZoom = 1;
    var customLineCounter = 0;

    function loadInvoice() {
        var type = document.getElementById('<%= ddlInvoiceType.ClientID %>').value;
        var id = document.getElementById('<%= txtInvoiceId.ClientID %>').value;
        if (!id || isNaN(id)) { showAlert('أدخل رقم فاتورة صحيح'); return; }

        var url = 'InvoiceController.aspx/GetInvoiceData';
        var data = JSON.stringify({ invoiceType: type, invoiceId: parseInt(id) });

        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: data
        })
        .then(function(r) { return r.json(); })
        .then(function(d) {
            if (d.d && d.d.IsSuccess) {
                invoiceData = JSON.parse(d.d.JsonData);
                renderPreview();
                hideAlert();
            } else {
                showAlert(d.d ? d.d.Message : 'لم يتم العثور على الفاتورة');
                invoiceData = null;
                renderPlaceholder();
            }
        })
        .catch(function(e) {
            showAlert('خطأ في الاتصال: ' + e.message);
        });
    }

    function renderPreview() {
        if (!invoiceData) { renderPlaceholder(); return; }
        var paperSize = document.getElementById('<%= ddlPaperSize.ClientID %>').value;
        var showHeader = document.getElementById('chkShowHeader').checked;
        var showBarcode = document.getElementById('chkShowBarcode').checked;
        var showFooter = document.getElementById('chkShowFooter').checked;
        var showQueue = document.getElementById('chkShowQueue').checked;
        var showPaymentStatus = document.getElementById('chkShowPaymentStatus').checked;
        var showAmountDetails = document.getElementById('chkShowAmountDetails').checked;
        var headerExtra = document.getElementById('txtHeaderExtra').value;
        var footerExtra = document.getElementById('txtFooterExtra').value;
        var watermark = document.getElementById('txtWatermark').value;
        var fontSize = document.getElementById('ddlFontSize').value;
        var customLinesPos = document.getElementById('ddlCustomLinesPosition').value;

        var html = '';
        var wClass = paperSize === '58mm' ? 'w-58' : (paperSize === 'A4' ? 'w-a4' : 'w-80');

        html += '<div class="inv-preview ' + wClass + '" id="invoiceBox" style="font-size:' + fontSize + 'px;">';

        // الترويسة
        if (showHeader) {
            html += '<div class="inv-title">';
            html += '<h2>' + (invoiceData.ClinicName || 'عيادات الرياض الصالحين') + '</h2>';
            html += '<div class="inv-sub">' + (invoiceData.ClinicPhone || 'هاتف: 01000000000') + '</div>';
            html += '<div class="inv-sub">فاتورة ' + (invoiceData.InvoiceTypeName || 'طبية') + '</div>';
            if (headerExtra) html += '<div class="inv-custom-header">' + escHtml(headerExtra) + '</div>';
            html += '</div>';
        }

        // الحقول المخصصة (أعلى)
        if (customLinesPos === 'top') html += renderCustomLines();

        // بيانات الفاتورة
        html += '<table class="inv-table">';
        html += tr('رقم الفاتورة', invoiceData.InvoiceId);
        html += tr('المريض', invoiceData.PatientName);
        html += tr('الطبيب', invoiceData.DoctorName);
        if (invoiceData.ServiceName) html += tr('الخدمة', invoiceData.ServiceName);
        if (invoiceData.BookingType) html += tr('نوع الحجز', translateBooking(invoiceData.BookingType));
        if (showPaymentStatus) html += tr('حالة الدفع', invoiceData.PaymentStatus === 'Paid' ? 'مدفوع' : 'غير مدفوع');
        if (invoiceData.PaymentMethod) html += tr('طريقة الدفع', invoiceData.PaymentMethod);
        html += tr('التاريخ', invoiceData.InvoiceDate);
        if (showAmountDetails) {
            html += tr('القيمة', invoiceData.TotalAmount + ' ج.م');
            if (invoiceData.DiscountAmount > 0) html += tr('الخصم', invoiceData.DiscountAmount + ' ج.م');
            html += tr('المدفوع', invoiceData.PaidAmount + ' ج.م');
            var remaining = (parseFloat(invoiceData.TotalAmount) - parseFloat(invoiceData.PaidAmount)).toFixed(2);
            html += tr('المتبقي', remaining + ' ج.م');
        }
        html += '</table>';

        // الحقول المخصصة (وسط)
        if (customLinesPos === 'middle') html += renderCustomLines();

        // رقم الانتظار
        if (showQueue && invoiceData.QueueNumber) {
            html += '<div class="inv-queue-label">رقم الانتظار</div>';
            html += '<div class="inv-queue-big">' + escHtml(invoiceData.QueueNumber) + '</div>';
        }

        // الباركود
        if (showBarcode && invoiceData.BarcodeValue) {
            html += '<div class="inv-barcode"><svg id="barcodeSvg"></svg></div>';
        }

        // التذييل
        if (showFooter) {
            html += '<div class="inv-footer">نتمنى لكم الشفاء العاجل</div>';
            if (footerExtra) html += '<div class="inv-custom-footer">' + escHtml(footerExtra) + '</div>';
        }

        // العلامة المائية
        if (watermark) {
            html += '<div class="inv-custom-watermark">' + escHtml(watermark) + '</div>';
        }

        // الحقول المخصصة (أسفل)
        if (customLinesPos === 'bottom') html += renderCustomLines();

        html += '</div>';

        document.getElementById('previewContent').outerHTML = html;

        // رسم الباركود
        if (showBarcode && invoiceData.BarcodeValue) {
            try {
                JsBarcode('#barcodeSvg', invoiceData.BarcodeValue.toString(), {
                    format: 'CODE128',
                    width: 1.5,
                    height: 40,
                    displayValue: true,
                    fontSize: 12,
                    margin: 2
                });
            } catch(e) {}
        }

        // حفظ بيانات الفاتورة
        document.getElementById('<%= hfInvoiceData.ClientID %>').value = JSON.stringify(invoiceData);
    }

    function renderPlaceholder() {
        var html = '<div class="inv-preview w-80" id="invoiceBox">';
        html += '<div style="display:flex;align-items:center;justify-content:center;height:400px;color:#aaa;">';
        html += '<div class="text-center"><i class="fas fa-file-invoice fa-4x mb-3"></i>';
        html += '<p>أدخل رقم الفاتورة واضغط بحث لعرض المعاينة</p></div></div></div>';
        document.getElementById('previewContent').outerHTML = html;
    }

    function tr(label, value) {
        return '<tr><td>' + label + '</td><td>' + escHtml(value || '-') + '</td></tr>';
    }

    function escHtml(s) {
        if (!s) return '';
        var d = document.createElement('div');
        d.textContent = s;
        return d.innerHTML;
    }

    function translateBooking(bt) {
        var map = { 'WalkIn': 'حضور مباشر', 'Phone': 'حجز هاتفي', 'Online': 'حجز إلكتروني', 'WhatsApp': 'حجز واتساب' };
        return map[bt] || bt;
    }

    // Custom Lines
    function addCustomLine() {
        customLineCounter++;
        var container = document.getElementById('customLinesContainer');
        var div = document.createElement('div');
        div.className = 'custom-line-item';
        div.id = 'customLine_' + customLineCounter;
        div.innerHTML = '<input type="text" class="form-control form-control-sm" placeholder="الاسم (مثال: رقم العضوية)" oninput="renderPreview();" data-role="label" />' +
                        '<input type="text" class="form-control form-control-sm" placeholder="القيمة" oninput="renderPreview();" data-role="value" />' +
                        '<button type="button" class="btn-remove" onclick="removeCustomLine(' + customLineCounter + ')"><i class="fas fa-times"></i></button>';
        container.appendChild(div);
        renderPreview();
    }

    function removeCustomLine(id) {
        var el = document.getElementById('customLine_' + id);
        if (el) el.remove();
        renderPreview();
    }

    function renderCustomLines() {
        var container = document.getElementById('customLinesContainer');
        var items = container.querySelectorAll('.custom-line-item');
        if (items.length === 0) return '';
        var html = '';
        items.forEach(function(item) {
            var label = item.querySelector('[data-role="label"]').value;
            var value = item.querySelector('[data-role="value"]').value;
            if (label || value) {
                html += '<div class="inv-custom-line"><span class="line-label">' + escHtml(label) + '</span><span class="line-value">' + escHtml(value) + '</span></div>';
            }
        });
        return html;
    }

    // Zoom
    function zoomIn() {
        previewZoom = Math.min(previewZoom + 0.1, 2);
        document.getElementById('previewContent').style.transform = 'scale(' + previewZoom + ')';
        document.getElementById('previewContent').style.transformOrigin = 'top center';
    }
    function zoomOut() {
        previewZoom = Math.max(previewZoom - 0.1, 0.5);
        document.getElementById('previewContent').style.transform = 'scale(' + previewZoom + ')';
        document.getElementById('previewContent').style.transformOrigin = 'top center';
    }

    function applyPaperSize() { renderPreview(); }
    function applyFontSize() { renderPreview(); }

    // Print
    function printInvoice() {
        var box = document.getElementById('invoiceBox');
        if (!box) { showAlert('لا توجد فاتورة للمعاينة'); return; }

        var printType = document.getElementById('<%= ddlPrintType.ClientID %>').value;
        var paperSize = document.getElementById('<%= ddlPaperSize.ClientID %>').value;
        var fontSize = document.getElementById('ddlFontSize').value;

        var printHtml = '<!DOCTYPE html><html dir="rtl"><head><meta charset="utf-8">';
        printHtml += '<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.rtl.min.css" />';
        printHtml += '<style>';
        printHtml += '*{box-sizing:border-box;margin:0;padding:0;}';
        printHtml += 'body{font-family:Tahoma,Arial,sans-serif;font-size:' + fontSize + 'px;}';
        printHtml += '.inv-preview{margin:0 auto;padding:3mm;overflow:hidden;}';
        printHtml += '.inv-preview.w-58{width:58mm;}.inv-preview.w-80{width:80mm;}.inv-preview.w-a4{width:190mm;padding:10mm;}';
        printHtml += '.inv-title{text-align:center;border-bottom:1px dashed #000;padding-bottom:4px;margin-bottom:6px;}';
        printHtml += '.inv-title h2{margin:0;font-size:16px;font-weight:bold;}.inv-title .inv-sub{font-size:12px;margin-top:2px;}';
        printHtml += '.inv-title .inv-custom-header{font-size:13px;color:#333;margin-top:4px;font-style:italic;}';
        printHtml += '.inv-table{width:100%;border-collapse:collapse;table-layout:fixed;margin-bottom:6px;}';
        printHtml += '.inv-table td{padding:2px 0;font-size:' + fontSize + 'px;vertical-align:top;word-wrap:break-word;}';
        printHtml += '.inv-table td:first-child{font-weight:bold;white-space:nowrap;}';
        printHtml += '.inv-custom-line{padding:3px 0;font-size:' + fontSize + 'px;border-bottom:1px dotted #ccc;display:flex;justify-content:space-between;}';
        printHtml += '.inv-custom-line .line-label{font-weight:bold;}.inv-custom-line .line-value{text-align:left;}';
        printHtml += '.inv-queue-label{text-align:center;font-size:11px;margin-top:6px;}';
        printHtml += '.inv-queue-big{text-align:center;font-size:34px;font-weight:bold;border:2px solid #000;border-radius:4px;margin:4px 0 8px 0;padding:4px 0;line-height:1.1;}';
        printHtml += '.inv-barcode{text-align:center;margin-top:4px;}.inv-barcode svg{max-width:100%;}';
        printHtml += '.inv-footer{text-align:center;font-size:10px;border-top:1px dashed #000;margin-top:6px;padding-top:4px;}';
        printHtml += '.inv-custom-footer{text-align:center;font-size:11px;color:#555;margin-top:4px;font-style:italic;}';
        printHtml += '.inv-custom-watermark{text-align:center;font-size:10px;color:#999;margin-top:8px;border-top:1px dotted #ccc;padding-top:4px;}';
        printHtml += '@page{margin:0;';

        if (paperSize === '58mm') printHtml += 'size:58mm auto;';
        else if (paperSize === '80mm') printHtml += 'size:80mm auto;';
        else printHtml += 'size:A4;';

        printHtml += '}</style></head><body>';
        printHtml += box.outerHTML;
        printHtml += '<script>window.onload=function(){window.print();window.close();}<\/script>';
        printHtml += '</body></html>';

        var w = window.open('', '_blank', 'width=800,height=600');
        w.document.write(printHtml);
        w.document.close();
    }

    // Alerts
    function showAlert(msg) {
        var pnl = document.getElementById('<%= pnlMessage.ClientID %>');
        document.getElementById('<%= lblMessage.ClientID %>').textContent = msg;
        pnl.style.display = 'block';
    }
    function hideAlert() {
        document.getElementById('<%= pnlMessage.ClientID %>').style.display = 'none';
    }
</script>

</asp:Content>
