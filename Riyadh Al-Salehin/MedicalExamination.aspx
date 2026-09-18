<%@ Page Title="الكشف الطبي"
    Language="C#"
    MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true"
    CodeBehind="MedicalExamination.aspx.cs"
    Inherits="Riyadh_Al_Salehin.MedicalExamination" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.1/css/all.min.css" rel="stylesheet" />

    <style>
        body {
            background: #f4f7fb;
            font-family: 'Segoe UI', Tahoma, sans-serif;
        }
        .pageHeader {
            background: linear-gradient(135deg, #0d6efd, #198754);
            color: #fff;
            border-radius: 20px;
            padding: 25px 30px;
            margin-top: 20px;
            margin-bottom: 25px;
            box-shadow: 0 10px 30px rgba(0,0,0,.15);
        }
        .infoCard {
            background: #fff;
            border-radius: 18px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 6px 18px rgba(0,0,0,.06);
            transition: all 0.2s;
        }
        .infoCard:hover {
            box-shadow: 0 8px 25px rgba(0,0,0,.1);
        }
        .sectionTitle {
            font-size: 22px;
            color: #0d6efd;
            font-weight: 600;
            margin-bottom: 20px;
            border-bottom: 2px solid #e9ecef;
            padding-bottom: 10px;
        }
        .sectionTitle i {
            margin-left: 10px;
        }
        .form-label {
            font-weight: 600;
            color: #2c3e50;
        }
        .form-control, .form-select {
            border-radius: 10px;
            border: 1px solid #ced4da;
            transition: 0.2s;
        }
        .form-control:focus, .form-select:focus {
            border-color: #0d6efd;
            box-shadow: 0 0 0 0.2rem rgba(13,110,253,.25);
        }
        .readonly {
            background: #f1f3f5 !important;
            color: #212529;
            font-weight: 500;
        }
        .btn-purple {
            background-color: #6f42c1;
            color: white;
        }
        .btn-purple:hover {
            background-color: #5a32a3;
            color: white;
        }
        .grid-history {
            font-size: 0.95rem;
        }
        .grid-history th {
            background: #e9ecef;
            color: #1e2a3a;
        }
        textarea {
            resize: none;
        }
        .auto-complete-style {
            border-radius: 10px;
        }
        .progress-overlay {
            background: rgba(255,255,255,0.7);
            padding: 10px;
            border-radius: 10px;
            text-align: center;
        }
        .nav-tabs .nav-link {
            border-radius: 10px 10px 0 0;
            font-weight: 600;
            color: #2c3e50;
            padding: 12px 25px;
        }
        .nav-tabs .nav-link.active {
            background-color: #0d6efd;
            color: #fff;
            border-color: #0d6efd;
        }
        .nav-tabs .nav-link i {
            margin-left: 8px;
        }
        .tab-pane {
            padding-top: 20px;
        }
        .action-bar {
            background: #fff;
            border-radius: 18px;
            padding: 20px 25px;
            box-shadow: 0 6px 18px rgba(0,0,0,.06);
        }

        /* تحسينات الهاتف فقط - لا تؤثر على تصميم الكمبيوتر */
        @media (max-width: 767.98px) {
            .container-fluid {
                padding-left: 10px;
                padding-right: 10px;
            }

            .pageHeader {
                margin-top: 10px;
                margin-bottom: 14px;
                padding: 16px 14px;
                border-radius: 14px;
            }

            .pageHeader h2 {
                font-size: 1.25rem;
                line-height: 1.6;
                margin-bottom: 10px;
            }

            .pageHeader h5 {
                font-size: .9rem;
                margin-bottom: 0;
            }

            .infoCard {
                padding: 12px;
                margin-bottom: 14px;
                border-radius: 14px;
            }

            .nav-tabs {
                display: flex;
                flex-wrap: nowrap;
                overflow-x: auto;
                overflow-y: hidden;
                -webkit-overflow-scrolling: touch;
                scrollbar-width: thin;
            }

            .nav-tabs .nav-item {
                flex: 0 0 auto;
            }

            .nav-tabs .nav-link {
                white-space: nowrap;
                font-size: .82rem;
                padding: 9px 11px;
            }

            .tab-pane {
                padding-top: 14px;
            }

            .form-label {
                font-size: .9rem;
                margin-bottom: 4px;
            }

            .form-control,
            .form-select {
                min-height: 42px;
                font-size: .95rem;
            }

            textarea.form-control {
                min-height: 96px;
            }

            .sectionTitle {
                font-size: 1.05rem !important;
                margin-bottom: 12px;
            }

            .action-bar {
                padding: 12px;
                border-radius: 14px;
            }

            .action-bar .row {
                --bs-gutter-x: .5rem;
                --bs-gutter-y: .5rem;
            }

            .action-bar [class*="col-"] {
                flex: 0 0 50%;
                max-width: 50%;
            }

            .action-bar .btn-lg {
                min-height: 48px;
                padding: 8px 4px;
                font-size: .88rem;
                white-space: normal;
                line-height: 1.35;
            }

            .table-responsive,
            .grid-history,
            #<%= gvSelectedMedicines.ClientID %>,
            #<%= gvSuggestedMedicines.ClientID %> {
                max-width: 100%;
                overflow-x: auto;
                -webkit-overflow-scrolling: touch;
            }

            .grid-history,
            #<%= gvSelectedMedicines.ClientID %>,
            #<%= gvSuggestedMedicines.ClientID %> {
                display: block;
                white-space: nowrap;
            }

            .grid-history th,
            .grid-history td {
                min-width: 120px;
                font-size: .82rem;
            }

            .btn {
                min-width: 0;
            }
        }
    </style>

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

    <div class="container-fluid">
        <!-- Header -->
        <div class="pageHeader">
            <div class="row align-items-center">
                <div class="col-md-8">
                    <h2><i class="fa-solid fa-stethoscope me-2"></i>شاشة الكشف الطبي</h2>
                </div>
                <div class="col-md-4 text-md-end">
                    <h5><span class="badge bg-light text-dark">رقم الموعد :</span>
                        <asp:Label ID="lblAppointmentId" runat="server" Text="0" CssClass="fw-bold" />
                    </h5>
                </div>
            </div>
        </div>

        <!-- رسائل عامة -->
        <div class="mb-3">
            <asp:Label ID="lblMessage" runat="server" Font-Bold="true" ForeColor="Red" />
        </div>

        <!-- حقول مخفية -->
        <asp:HiddenField ID="hfAppointmentId" runat="server" />
        <asp:HiddenField ID="hfPatientId" runat="server" />
        <asp:HiddenField ID="hfDoctorId" runat="server" />
        <asp:HiddenField ID="hfDiagnosisId" runat="server" Value="0" />

        <!-- ========== التبويبات ========== -->
        <div class="infoCard">
            <ul class="nav nav-tabs" id="examTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class="nav-link active" id="tabAppointment-tab" data-bs-toggle="tab" href="#tabAppointment" role="tab" aria-controls="tabAppointment" aria-selected="true">
                        <i class="fa-solid fa-address-card"></i> بيانات الموعد
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link" id="tabExam-tab" data-bs-toggle="tab" href="#tabExam" role="tab" aria-controls="tabExam" aria-selected="false">
                        <i class="fa-solid fa-notes-medical"></i> الكشف الطبي
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link" id="tabMedicines-tab" data-bs-toggle="tab" href="#tabMedicines" role="tab" aria-controls="tabMedicines" aria-selected="false">
                        <i class="fa-solid fa-prescription-bottle"></i> الأدوية المقترحة
                    </a>
                </li>
            </ul>

            <div class="tab-content">
                <!-- ===== تبويب 1: بيانات الموعد ===== -->
                <div class="tab-pane fade show active" id="tabAppointment" role="tabpanel" aria-labelledby="tabAppointment-tab">
                    <!-- بيانات المريض والطبيب -->
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label">اسم المريض</label>
                            <asp:TextBox ID="txtPatientName" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">رقم الملف</label>
                            <asp:TextBox ID="txtPatientId" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">الهاتف</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">التأمين</label>
                            <asp:TextBox ID="txtInsurance" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">حالة الفاتورة</label>
                            <asp:TextBox ID="txtPaymentStatus" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>

                        <div class="col-md-3">
                            <label class="form-label">اسم الطبيب</label>
                            <asp:TextBox ID="txtDoctor" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">التخصص</label>
                            <asp:TextBox ID="txtSpecialty" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">رقم الدور</label>
                            <asp:TextBox ID="txtQueue" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">موعد الكشف</label>
                            <asp:TextBox ID="txtAppointmentDate" runat="server" CssClass="form-control readonly" ReadOnly="true" />
                        </div>
                        <div class="col-md-12 text-md-end">
                            <asp:Button ID="btnViewXray" runat="server" Text="🩻 تقارير الأشعة" CssClass="btn btn-purple" Visible="false" OnClick="btnViewXray_Click" />
                        </div>
                    </div>

                    <hr class="my-4" />

                    <!-- التاريخ المرضي السابق -->
                    <div id="divHistory" runat="server" visible="false">
                        <div class="sectionTitle"><i class="fa-solid fa-clock-rotate-left"></i> التاريخ المرضي السابق</div>
                        <asp:GridView ID="gvPatientHistory" runat="server" CssClass="table table-bordered table-hover grid-history" AutoGenerateColumns="False" EmptyDataText="لا توجد سجلات سابقة لهذا المريض.">
                            <Columns>
                                <asp:BoundField DataField="ExaminationDate" HeaderText="تاريخ الكشف" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:BoundField DataField="InitialDiagnosis" HeaderText="التشخيص" />
                                <asp:BoundField DataField="Notes" HeaderText="الملاحظات" />
                                <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <!-- ===== تبويب 2: الكشف الطبي ===== -->
                <div class="tab-pane fade" id="tabExam" role="tabpanel" aria-labelledby="tabExam-tab">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">الشكوى الرئيسية</label>
                            <asp:TextBox ID="txtComplaint" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="اكتب الشكوى الرئيسية..." />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">التاريخ المرضي الحالي</label>
                            <asp:TextBox ID="txtMedicalHistory" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="التاريخ المرضي..." />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">الفحص السريري</label>
                            <asp:TextBox ID="txtClinicalExam" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="نتائج الفحص السريري..." />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">التشخيص النهائي</label>
                            <asp:TextBox ID="txtDiagnosis" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="التشخيص النهائي..." />
                        </div>
                        <div class="col-md-12">
                            <label class="form-label">ملاحظات الطبيب</label>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="أي ملاحظات إضافية..." />
                        </div>
                    </div>
                </div>

              <!-- ===== تبويب 3: الأدوية المقترحة ===== -->
<!-- ===== تبويب 3: الأدوية المقترحة ===== -->
<div class="tab-pane fade" id="tabMedicines" role="tabpanel" aria-labelledby="tabMedicines-tab">
    <asp:UpdatePanel ID="upMedicines" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
        <ContentTemplate>
            <!-- قسم: اقتراح حسب التشخيص -->
            <div class="row mb-3">
                <div class="col-md-6 mb-3">
                    <label class="form-label">ابحث عن التشخيص (اختياري)</label>
                    <asp:TextBox ID="txtDiagnosisSearch" runat="server" CssClass="form-control" 
                        placeholder="اكتب اسم التشخيص..." AutoPostBack="true" 
                        OnTextChanged="txtDiagnosisSearch_TextChanged" />
                    <ajaxToolkit:AutoCompleteExtender ID="aceDiagnosis" runat="server"
                        TargetControlID="txtDiagnosisSearch"
                        ServiceMethod="GetDiagnoses"
                        ServicePath=""
                        MinimumPrefixLength="2"
                        CompletionInterval="100"
                        EnableCaching="true"
                        CompletionSetCount="15"
                        FirstRowSelected="true"
                        CompletionListCssClass="auto-complete-style" />
                    <small class="text-muted">ابحث باسم التشخيص (عربي أو إنجليزي)</small>
                </div>
                <div class="col-md-6 mb-3 d-flex align-items-end">
                    <asp:Label ID="lblDiagnosisMessage" runat="server" Font-Bold="true" />
                </div>
            </div>

            <!-- جدول الأدوية المقترحة -->
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="gvSuggestedMedicines" runat="server"
                        CssClass="table table-bordered table-striped align-middle"
                        AutoGenerateColumns="False"
                        EmptyDataText="لا توجد أدوية مقترحة لهذا التشخيص."
                        OnRowCommand="gvSuggestedMedicines_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="MedicineName" HeaderText="اسم الدواء" />
                            <asp:BoundField DataField="GenericName" HeaderText="الاسم العلمي" />
                            <asp:BoundField DataField="Strength" HeaderText="التركيز" />
                            <asp:BoundField DataField="Form" HeaderText="الشكل" />
                            <asp:CheckBoxField DataField="IsPreferred" HeaderText="مفضل" />
                            <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />
                            <asp:TemplateField HeaderText="اختيار">
                                <ItemTemplate>
                                    <asp:Button ID="btnSelect" runat="server" Text="اختيار" 
                                        CommandName="SelectMedicine" 
                                        CommandArgument='<%# Eval("MedicineName") + "|" + Eval("GenericName") + "|" + Eval("Strength") + "|" + Eval("Form") %>'
                                        CssClass="btn btn-sm btn-success" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <hr class="my-4" />

            <!-- قسم: بحث يدوي عن دواء (مع AutoComplete) -->
            <div class="sectionTitle" style="font-size:16px;">
                <i class="fa-solid fa-magnifying-glass"></i>
                بحث يدوي عن دواء وإضافته مباشرة
            </div>

            <div class="row mb-3">
    <div class="col-md-8">
        <asp:TextBox ID="txtManualMedicineSearch" runat="server" CssClass="form-control"
            placeholder="اكتب اسم الدواء أو الاسم العلمي..." />
        <ajaxToolkit:AutoCompleteExtender ID="aceManualMedicine" runat="server"
            TargetControlID="txtManualMedicineSearch"
            ServiceMethod="GetMedicines"
            ServicePath=""
            MinimumPrefixLength="2"
            CompletionInterval="100"
            EnableCaching="true"
            CompletionSetCount="15"
            FirstRowSelected="true"
            CompletionListCssClass="auto-complete-style"
            OnClientItemSelected="onMedicineSelected" />
        <small class="text-muted">اكتب اسم الدواء للبحث، ثم اختر من القائمة لإضافته تلقائياً</small>
    </div>
    <div class="col-md-2">
        <asp:Button ID="btnClearManualSearch" runat="server" Text="✖ مسح"
            CssClass="btn btn-outline-secondary w-100" OnClick="btnClearManualSearch_Click"
            CausesValidation="false" />
    </div>
</div>

            <!-- قائمة الأدوية المختارة -->
            <div class="row mt-4">
                <div class="col-md-12">
                    <h5 class="fw-bold text-primary"><i class="fa-solid fa-list-check"></i> الأدوية المختارة</h5>
                    <asp:GridView ID="gvSelectedMedicines" runat="server"
                        CssClass="table table-bordered table-striped align-middle"
                        AutoGenerateColumns="False"
                        EmptyDataText="لم يتم اختيار أي دواء بعد."
                        OnRowCommand="gvSelectedMedicines_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="MedicineName" HeaderText="اسم الدواء" />
                            <asp:BoundField DataField="GenericName" HeaderText="الاسم العلمي" />
                            <asp:BoundField DataField="Strength" HeaderText="التركيز" />
                            <asp:BoundField DataField="Form" HeaderText="الشكل" />
                            <asp:TemplateField HeaderText="حذف">
                                <ItemTemplate>
                                    <asp:Button ID="btnRemove" runat="server" Text="✖" 
                                        CommandName="RemoveMedicine" 
                                        CommandArgument='<%# Container.DataItemIndex %>'
                                        CssClass="btn btn-sm btn-danger" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <!-- عناصر مخفية لإضافة الدواء من البحث اليدوي -->
                    <asp:HiddenField ID="hfSelectedMedicine" runat="server" />
                    <asp:Button ID="btnAddMedicineFromManual" runat="server" style="display:none" OnClick="btnAddMedicineFromManual_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
            </div>
        </div>

        <!-- ========== أزرار العمليات (خارج التبويبات لتكون ثابتة) ========== -->
        <div class="action-bar">
            <div class="row g-2">
                <div class="col-md-2">
                    <asp:Button ID="btnSave" runat="server" Text="💾 حفظ الكشف" CssClass="btn btn-success w-100 btn-lg" OnClick="btnSave_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnPrescription" runat="server" Text="💊 الروشتة" CssClass="btn btn-primary w-100 btn-lg" Enabled="false" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnLab" runat="server" Text="🧪 التحاليل" CssClass="btn btn-warning w-100 btn-lg" Enabled="false" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnRay" runat="server" Text="🩻 الأشعة" CssClass="btn btn-info w-100 btn-lg" OnClick="btnRay_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnServices" runat="server" Text="🦷 الخدمات" CssClass="btn btn-secondary w-100 btn-lg" OnClick="btnServices_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnFinish" runat="server" Text="✔ إنهاء الكشف" CssClass="btn btn-danger w-100 btn-lg" Enabled="false" OnClick="btnFinish_Click" />
                </div>
            </div>
        </div>

    </div>

    <!-- تضمين Bootstrap JS للتبويبات -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>


    <script type="text/javascript">
function onMedicineSelected(sender, e) {
    var hf = document.getElementById('<%= hfSelectedMedicine.ClientID %>');
    if (hf) {
        hf.value = e.get_value();
        var btn = document.getElementById('<%= btnAddMedicineFromManual.ClientID %>');
        if (btn) btn.click();
    }
}    </script>

</asp:Content>