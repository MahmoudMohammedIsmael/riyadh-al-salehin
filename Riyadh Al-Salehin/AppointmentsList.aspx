<%@ Page Title="إدارة المواعيد" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="AppointmentsList.aspx.cs" Inherits="Riyadh_Al_Salehin.AppointmentsList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- إضافة مكتبات DataTables (إن لم تكن موجودة في Master) -->
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />

    <!-- تحميل jQuery فقط إذا لم يكن محملاً بالفعل من MasterPage، لتفادي تحميله مرتين -->
    <script>
        if (typeof jQuery === 'undefined') {
            document.write('<script src="https://code.jquery.com/jquery-3.6.0.min.js"><\/script>');
        }
    </script>
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>

    <style>
        /* تحسينات إضافية */
        .stat-card {
            background: #fff;
            border-radius: 12px;
            padding: 20px 15px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
            border-right: 4px solid #0d6efd;
            transition: 0.3s;
            height: 100%;
        }
        .stat-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); }
        .stat-number { font-size: 28px; font-weight: 700; color: #1e293b; }
        .stat-label { font-size: 14px; color: #64748b; }
        .stat-icon { font-size: 32px; opacity: 0.3; }

        .filter-bar {
            background: #f8fafc;
            padding: 20px;
            border-radius: 12px;
            border: 1px solid #e9edf4;
        }

        .badge-status {
            font-size: 13px;
            padding: 6px 14px;
            border-radius: 30px;
        }

        .action-btn {
            border: none;
            background: transparent;
            cursor: pointer;
            font-size: 18px;
            margin: 0 4px;
            transition: 0.2s;
        }
        .action-btn:hover { transform: scale(1.1); }
        .action-btn.edit { color: #0d6efd; }
        .action-btn.delete { color: #dc3545; }

        .modal-header-custom {
            background: linear-gradient(135deg, #0d6efd, #0a58ca);
            color: #fff;
            border-radius: 8px 8px 0 0;
        }
        .modal-header-custom .btn-close { filter: brightness(0) invert(1); }

        /* تنسيق الجدول */
        #gvAppointments_wrapper .dataTables_filter { float: left; text-align: left; }
        #gvAppointments_wrapper .dataTables_length { float: right; }
        @media (max-width: 768px) {
            #gvAppointments_wrapper .dataTables_filter { float: none; text-align: center; }
            #gvAppointments_wrapper .dataTables_length { float: none; text-align: center; }
        }
    </style>

    <div class="container-fluid mt-4" dir="rtl">
        <div class="row">
            <div class="col-12">

                <!-- ====== بطاقات الإحصاء ====== -->
                <div class="row g-3 mb-4">
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card" style="border-right-color: #0d6efd;">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <div class="stat-number"><asp:Label ID="lblTotal" runat="server" Text="0" /></div>
                                    <div class="stat-label">إجمالي المواعيد</div>
                                </div>
                                <i class="bi bi-calendar3 stat-icon" style="color:#0d6efd;"></i>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card" style="border-right-color: #0dcaf0;">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <div class="stat-number"><asp:Label ID="lblToday" runat="server" Text="0" /></div>
                                    <div class="stat-label">مواعيد اليوم</div>
                                </div>
                                <i class="bi bi-calendar-day stat-icon" style="color:#0dcaf0;"></i>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card" style="border-right-color: #ffc107;">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <div class="stat-number"><asp:Label ID="lblPending" runat="server" Text="0" /></div>
                                    <div class="stat-label">قيد الانتظار</div>
                                </div>
                                <i class="bi bi-clock-history stat-icon" style="color:#ffc107;"></i>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card" style="border-right-color: #198754;">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <div class="stat-number"><asp:Label ID="lblDone" runat="server" Text="0" /></div>
                                    <div class="stat-label">منتهية</div>
                                </div>
                                <i class="bi bi-check2-circle stat-icon" style="color:#198754;"></i>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- ====== شريط الفلترة ====== -->
                <div class="filter-bar mb-4">
                    <div class="row g-2 align-items-end">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">بحث عام</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="اسم المريض / الطبيب / رقم الموعد" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-bold">الحالة</label>
                            <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select">
                                <asp:ListItem Text="الكل" Value="" />
                                <asp:ListItem Text="pending" Value="pending" />
                                <asp:ListItem Text="confirmed" Value="confirmed" />
                                <asp:ListItem Text="done" Value="done" />
                                <asp:ListItem Text="cancelled" Value="cancelled" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-bold">من تاريخ</label>
                            <asp:TextBox ID="txtDateFrom" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-bold">إلى تاريخ</label>
                            <asp:TextBox ID="txtDateTo" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>
                        <div class="col-md-3 d-flex gap-2">
                            <asp:Button ID="btnFilter" runat="server" Text="بحث" CssClass="btn btn-primary w-50" OnClick="btnFilter_Click" />
                            <asp:Button ID="btnResetFilter" runat="server" Text="إعادة ضبط" CssClass="btn btn-secondary w-50" OnClick="btnResetFilter_Click" />
                        </div>
                    </div>
                </div>

                <!-- ====== زر إضافة جديد + زر الحذف الجماعي + الجدول ====== -->
                <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
                    <h5 class="mb-0"><i class="bi bi-table"></i> قائمة المواعيد</h5>
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnDeleteSelected" runat="server" Text="حذف المواعيد المحددة"  CssClass="btn btn-danger" Enabled="false" OnClientClick="return confirmBulkDelete();" OnClick="btnDeleteSelected_Click" />
                        <asp:Button ID="btnAddNew" runat="server" Text="+ إضافة موعد جديد" CssClass="btn btn-success" OnClick="btnAddNew_Click" />
                    </div>
                </div>

                <asp:HiddenField ID="hfSelectedAppointmentIds" runat="server" />

                <!-- ====== رسالة عدم وجود بيانات (خارج الجدول تماماً) ====== -->
                <asp:Panel ID="pnlNoData" runat="server" CssClass="alert alert-info text-center" Visible="false">
                    <i class="bi bi-info-circle"></i> لا توجد مواعيد
                </asp:Panel>

                <div class="table-responsive">
                    <asp:GridView ID="gvAppointments" runat="server"
                        CssClass="table table-hover table-striped display nowrap"
                        AutoGenerateColumns="False"
                        DataKeyNames="Id"
                        OnRowCommand="gvAppointments_RowCommand"
                        AllowPaging="false"
                        GridLines="None"
                        CellPadding="4"
                        ForeColor="#333">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <input type="checkbox" id="selectAllAppointments" title="تحديد الكل" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <input type="checkbox" class="appointment-checkbox" data-id='<%# Eval("Id") %>' />
                                </ItemTemplate>
                                <ItemStyle Width="40px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:BoundField DataField="Id" HeaderText="# الموعد" SortExpression="Id" />
                            <asp:BoundField DataField="PatientName" HeaderText="المريض" SortExpression="PatientName" />
                            <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" SortExpression="DoctorName" />
                            <asp:BoundField DataField="CenterName" HeaderText="المركز" SortExpression="CenterName" />
                            <asp:BoundField DataField="AppointmentDate" HeaderText="التاريخ والوقت" DataFormatString="{0:yyyy/MM/dd HH:mm}" SortExpression="AppointmentDate" />

                            <asp:TemplateField HeaderText="الحالة" SortExpression="Status">
                                <ItemTemplate>
                                    <span class='badge badge-status <%# GetStatusClass(Eval("Status").ToString()) %>'>
                                        <%# GetStatusText(Eval("Status").ToString()) %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="action-btn edit" CommandName="EditRow" CommandArgument='<%# Eval("Id") %>' ToolTip="تعديل">
                                        <i class="bi bi-pencil-square"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="action-btn delete" CommandName="DeleteRow" CommandArgument='<%# Eval("Id") %>' ToolTip="حذف" OnClientClick="return confirm('هل أنت متأكد من حذف هذا الموعد؟');">
                                        <i class="bi bi-trash3"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- ====== رسائل ====== -->
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" CssClass="mt-2 d-block" />

            </div>
        </div>
    </div>

    <!-- ========================================================= -->
    <!-- ====== نافذة (مودال) لإضافة / تعديل الموعد ====== -->
    <!-- ========================================================= -->
    <div class="modal fade" id="appointmentModal" tabindex="-1" aria-labelledby="modalTitleLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header modal-header-custom">
                    <h5 class="modal-title" id="modalTitleLabel">إضافة موعد جديد</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfId" runat="server" />

                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-bold">المريض</label>
                            <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-bold">الطبيب</label>
                            <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-bold">المركز</label>
                            <asp:DropDownList ID="ddlCenter" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-bold">تاريخ ووقت الموعد</label>
                            <asp:TextBox ID="txtDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-bold">الحالة</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Text="pending" Value="pending" />
                                <asp:ListItem Text="confirmed" Value="confirmed" />
                                <asp:ListItem Text="done" Value="done" />
                                <asp:ListItem Text="cancelled" Value="cancelled" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-12 mb-3">
                            <label class="form-label fw-bold">ملاحظات</label>
                            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
                    <asp:Button ID="btnSave" runat="server" Text="حفظ" CssClass="btn btn-success" OnClick="btnSave_Click" />
                    <asp:Button ID="btnUpdate" runat="server" Text="تعديل" CssClass="btn btn-warning" OnClick="btnUpdate_Click" Visible="false" />
                </div>
            </div>
        </div>
    </div>

    <!-- ====== سكربت التحديد الجماعي + تفعيل DataTables ====== -->
    <script type="text/javascript">
        // مجموعة IDs المحددة، محفوظة عبر صفحات DataTables (client-side فقط، لأن AllowPaging=false على GridView)
        var selectedAppointmentIds = new Set();

        function getCheckboxes() {
            return $('.appointment-checkbox');
        }

        function syncSelectAllState() {
            var visible = getCheckboxes().filter(':visible');
            var selectAll = $('#selectAllAppointments');

            if (visible.length === 0) {
                selectAll.prop('checked', false).prop('indeterminate', false);
                return;
            }

            var checkedCount = visible.filter(':checked').length;

            if (checkedCount === 0) {
                selectAll.prop('checked', false).prop('indeterminate', false);
            } else if (checkedCount === visible.length) {
                selectAll.prop('checked', true).prop('indeterminate', false);
            } else {
                selectAll.prop('checked', false).prop('indeterminate', true);
            }
        }

        function syncCheckboxesWithSelection() {
            getCheckboxes().each(function () {
                var id = String($(this).data('id'));
                $(this).prop('checked', selectedAppointmentIds.has(id));
            });
            syncSelectAllState();
        }

        function updateBulkDeleteButton() {
            var count = selectedAppointmentIds.size;
            var btn = $('[id$="btnDeleteSelected"]');

            if (count > 0) {
                btn.prop('disabled', false);
                btn.val('حذف المواعيد المحددة (' + count + ')');
                btn.text('حذف المواعيد المحددة (' + count + ')');
            } else {
                btn.prop('disabled', true);
                btn.val('حذف المواعيد المحددة');
                btn.text('حذف المواعيد المحددة');
            }
        }

        function bindCheckboxEvents() {
            $(document).off('change.apptSelect', '.appointment-checkbox')
                .on('change.apptSelect', '.appointment-checkbox', function () {
                    var id = String($(this).data('id'));
                    if ($(this).is(':checked')) {
                        selectedAppointmentIds.add(id);
                    } else {
                        selectedAppointmentIds.delete(id);
                    }
                    syncSelectAllState();
                    updateBulkDeleteButton();
                });

            $(document).off('change.apptSelectAll', '#selectAllAppointments')
                .on('change.apptSelectAll', '#selectAllAppointments', function () {
                    var checked = $(this).is(':checked');
                    getCheckboxes().filter(':visible').each(function () {
                        var id = String($(this).data('id'));
                        $(this).prop('checked', checked);
                        if (checked) {
                            selectedAppointmentIds.add(id);
                        } else {
                            selectedAppointmentIds.delete(id);
                        }
                    });
                    updateBulkDeleteButton();
                });
        }

        function confirmBulkDelete() {
            if (selectedAppointmentIds.size === 0) {
                alert('يرجى تحديد موعد واحد على الأقل.');
                return false;
            }

            if (!confirm('هل أنت متأكد من حذف المواعيد المحددة؟ سيتم حذف المواعيد المسموح بحذفها فقط.')) {
                return false;
            }

            $('#<%= hfSelectedAppointmentIds.ClientID %>').val(Array.from(selectedAppointmentIds).join(','));
            return true;
        }

        function initializeAppointmentsTable() {
            var table = $('#<%= gvAppointments.ClientID %>');

            if (!table.length) {
                updateBulkDeleteButton();
                return;
            }

            if ($.fn.DataTable.isDataTable(table)) {
                table.DataTable().destroy();
            }

            var dt = table.DataTable({
                language: {
                    search: "بحث:",
                    lengthMenu: "عرض _MENU_",
                    info: "عرض _START_ إلى _END_ من أصل _TOTAL_",
                    infoEmpty: "لا توجد بيانات",
                    zeroRecords: "لا توجد نتائج",
                    paginate: {
                        first: "الأول",
                        last: "الأخير",
                        next: "التالي",
                        previous: "السابق"
                    }
                },
                order: [[1, "desc"]],
                responsive: true,
                pageLength: 15,
                lengthMenu: [[5, 10, 15, 25, -1], [5, 10, 15, 25, "الكل"]],
                columnDefs: [
                    { targets: 0, orderable: false, searchable: false },
                    { targets: -1, orderable: false, searchable: false }
                ]
            });

            dt.on('draw', function () {
                syncCheckboxesWithSelection();
            });

            bindCheckboxEvents();
            syncCheckboxesWithSelection();
            updateBulkDeleteButton();
        }

        $(document).ready(function () {
            initializeAppointmentsTable();
        });
    </script>

</asp:Content>
