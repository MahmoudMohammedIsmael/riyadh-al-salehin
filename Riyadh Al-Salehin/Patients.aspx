<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="Patients.aspx.cs" Inherits="Riyadh_Al_Salehin.Patients" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    /* ===========================
   Page Title Bar (موحّد لكل الشاشات)
=========================== */
.page-title-bar {
    background: linear-gradient(135deg, #0d6efd, #0a58ca);
    color: #ffffff !important;
    padding: 16px 24px;
    border-radius: 12px 12px 0 0;
    display: flex;
    align-items: center;
    gap: 10px;
    box-shadow: 0 4px 12px rgba(13,110,253,.25);
}

.page-title-bar h4,
.page-title-bar span {
    color: #ffffff !important;
    font-size: 20px !important;
    font-weight: 700 !important;
    margin: 0 !important;
    line-height: 1.4 !important;
}

.page-title-bar i {
    font-size: 22px;
    color: #ffffff;
}

.page-title-bar.dark {
    background: linear-gradient(135deg, #212529, #343a40);
    box-shadow: 0 4px 12px rgba(0,0,0,.2);
}

.form-card{
    background:#fff;
    border:none;
    border-radius:15px;
    box-shadow:0 5px 15px rgba(0,0,0,.08);
    margin-bottom:20px;
}

.form-card .card-header{
    background:#0d6efd;
    color:#fff;
    font-weight:bold;
    border-radius:15px 15px 0 0;
}

.form-control{
    border-radius:10px;
    height:45px;
}

.form-control:focus{
    border-color:#0d6efd;
    box-shadow:0 0 0 .2rem rgba(13,110,253,.15);
}

.btn{
    border-radius:10px;
    padding:10px 20px;
    font-weight:bold;
}

.table{
    border-radius:12px;
    overflow:hidden;
}

.table thead{
    background:#0d6efd;
    color:white;
}

label{
    font-weight:bold;
    margin-bottom:6px;
}









</style>  


<div class="container mt-4" dir="rtl">

  <div class="card shadow">
    <div class="page-title-bar">
        <i class="bi bi-people-fill"></i>
         <h4>إدارة المرضى</h4>

           
        </div>

        <div class="card-body">

            <asp:HiddenField ID="hfId" runat="server" />

            <div class="row">
                  <div class="card form-card">
    <div class="card-header">
        <i class="bi bi-search"></i>
        البحث السريع
    </div>

    <div class="card-body">

        <label>رقم المريض</label>

        <asp:TextBox
            ID="txtPrinted"
            runat="server"
            CssClass="form-control"
            placeholder="اكتب رقم المريض ثم اضغط Enter"
            AutoPostBack="true"
            OnTextChanged="txtPrinted_TextChanged">
        </asp:TextBox>

    </div>
</div>

                <div class="col-md-6 mb-3">
                    <label>اسم المريض</label>
                    <asp:TextBox ID="txtPatientName" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>العنوان</label>
                    <asp:TextBox ID="txtAddress" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>تاريخ الميلاد</label>
                    <asp:TextBox ID="txtDateOfBirth" runat="server"
                        TextMode="Date"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
    <label>نوع الخدمة</label>

    <asp:DropDownList
        ID="ddlService"
        runat="server"
        CssClass="form-control">
    </asp:DropDownList>
</div>


                <div class="col-md-6 mb-3">
                    <label>شركة التأمين</label>
                    <asp:TextBox ID="txtInsuranceCompany" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-6 mb-3">
                    <label>رقم التأمين</label>
                    <asp:TextBox ID="txtInsuranceNumber" runat="server"
                        CssClass="form-control"></asp:TextBox>
                </div>

            </div>

            <asp:Label ID="lblMessage" runat="server"
                Font-Bold="true"></asp:Label>

            <hr />

           <div class="text-center mt-4">

<asp:Button ID="btnSave"
runat="server"
Text="💾 حفظ"
CssClass="btn btn-success mx-1"
OnClick="btnSave_Click"/>

<asp:Button ID="btnContinue"
runat="server"
Text="➡ متابعة"
CssClass="btn btn-primary mx-1"
OnClick="btnContinue_Click"/>

<asp:Button ID="btnUpdate"
runat="server"
Text="✏ تعديل"
CssClass="btn btn-warning mx-1"
OnClick="btnUpdate_Click"/>

<asp:Button ID="btnDelete"
runat="server"
Text="🗑 حذف"
CssClass="btn btn-danger mx-1"
OnClick="btnDelete_Click"/>

<asp:Button ID="btnNew"
runat="server"
Text="🆕 جديد"
CssClass="btn btn-secondary mx-1"
OnClick="btnNew_Click"/>

</div>

        </div>
    </div>

    <br />


    <!-- قسم الأشعة المنفذة اليوم -->
    <!-- قسم الأشعة المكتملة اليوم -->
<div class="card shadow mt-4">
    <div class="card-header bg-success text-white">
        <h5 class="mb-0">🩻 طلبات الأشعة المكتملة اليوم (في انتظار إرسالها للطبيب)</h5>
    </div>
    <div class="card-body">
        <asp:GridView ID="gvXrayCompleted" runat="server"
            CssClass="table table-bordered table-hover"
            AutoGenerateColumns="False"
            OnRowCommand="gvXrayCompleted_RowCommand">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="رقم الطلب" />
                <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                <asp:BoundField DataField="XrayName" HeaderText="الخدمة" />
                <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                <asp:BoundField DataField="CreatedAt" HeaderText="تاريخ التنفيذ" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="الإجراء">
                    <ItemTemplate>
                        <asp:Button ID="btnCallDoctor" runat="server"
    Text="📩 طلب دخول للطبيب"
    CssClass="btn btn-primary btn-sm"
    CommandName="CallDoctor"
    CommandArgument='<%# Eval("Id") + "," + (Eval("AppointmentId") == DBNull.Value ? "0" : Eval("AppointmentId")) + "," + (Eval("DoctorId") == DBNull.Value ? "0" : Eval("DoctorId")) %>'
    OnClientClick="return confirm('هل تريد إرسال إشعار للطبيب؟');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- قسم الخدمات الإضافية المضافة اليوم -->
<div class="card shadow mt-4">
    <div class="card-header bg-info text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">🦷 الخدمات الإضافية المضافة اليوم</h5>
        <asp:Label ID="lblPendingBadge" runat="server" CssClass="badge bg-warning text-dark fs-6" Visible="false" />
    </div>
    <div class="card-body">
        <asp:GridView ID="gvAdditionalServices" runat="server"
            CssClass="table table-bordered table-hover"
            AutoGenerateColumns="False"
            OnRowCommand="gvAdditionalServices_RowCommand">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                <asp:BoundField DataField="ServiceName" HeaderText="الخدمة" />
                <asp:BoundField DataField="Quantity" HeaderText="الكمية" />
                <asp:BoundField DataField="UnitPrice" HeaderText="سعر الوحدة" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="TotalPrice" HeaderText="الإجمالي" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="InvoiceId" HeaderText="رقم الفاتورة" />
                <asp:BoundField DataField="InvoiceDate" HeaderText="تاريخ الفاتورة" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="إجراءات">
                    <ItemTemplate>
                        <asp:Button ID="btnPrintService" runat="server"
                            Text="🖨 طباعة"
                            CssClass="btn btn-primary btn-sm me-1"
                            CommandName="PrintService"
                            CommandArgument='<%# Eval("InvoiceId") %>' />
                        <asp:Button ID="btnConfirmService" runat="server"
                            Text="💾 حفظ"
                            CssClass="btn btn-success btn-sm"
                            CommandName="ConfirmService"
                            CommandArgument='<%# Eval("InvoiceId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="text-center text-muted py-4">لا توجد خدمات إضافية مضافة اليوم</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>






    <div class="card shadow">

        <div class="card-header bg-dark text-white">
            قائمة المرضى
        </div>

        <div class="card-body">

            <div class="row mb-3">

                <div class="col-md-9">
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="form-control"
                        placeholder="ابحث باسم المريض أو الهاتف"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <asp:Button ID="btnSearch" runat="server"
                        Text="بحث"
                        CssClass="btn btn-primary w-100"
                        OnClick="btnSearch_Click" />
                </div>

            </div>

            <asp:GridView
ID="gvPatients"
runat="server"
CssClass="table table-striped table-hover table-bordered align-middle"
HeaderStyle-CssClass="table-primary"
GridLines="None"
AutoGenerateColumns="False">

                <Columns>

                    <asp:BoundField DataField="Id"
                        HeaderText="م" />

                    <asp:BoundField DataField="PatientName"
                        HeaderText="اسم المريض" />

                    <asp:BoundField DataField="Phone"
                        HeaderText="الهاتف" />

                    <asp:BoundField DataField="InsuranceCompany"
                        HeaderText="شركة التأمين" />

                    <asp:CommandField
                        ShowSelectButton="True"
                        SelectText="اختيار" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>


<!-- 🔔 Toast Notification: إشعار الفواتير غير المؤكدة -->
<asp:HiddenField ID="hfPendingCount" runat="server" Value="0" />

<div id="invoiceToast" style="
    position: fixed;
    bottom: 30px;
    left: 30px;
    z-index: 9999;
    min-width: 340px;
    max-width: 420px;
    background: linear-gradient(135deg, #ff6b35, #f7c59f);
    color: #5c1a00;
    border-radius: 16px;
    padding: 18px 22px;
    box-shadow: 0 8px 32px rgba(255,107,53,0.35);
    display: none;
    font-size: 15px;
    font-weight: 700;
    direction: rtl;
    text-align: right;
    border: 2px solid #ff6b35;
    animation: slideIn 0.4s ease;
">
    <div style="display:flex; align-items:center; gap:10px;">
        <span style="font-size:28px;">🔔</span>
        <div>
            <div style="font-size:17px; font-weight:800; margin-bottom:4px;">تنبيه: فواتير إضافية بانتظار التأكيد</div>
            <div id="toastMsg" style="font-size:14px; font-weight:600; color:#7a2900;"></div>
        </div>
        <button onclick="document.getElementById('invoiceToast').style.display='none';"
                style="margin-right:auto; background:none; border:none; font-size:20px; color:#7a2900; cursor:pointer; line-height:1;">✕</button>
    </div>
    <div style="margin-top:10px; font-size:13px; opacity:0.85;">
        📌 يرجى مراجعة جدول الخدمات الإضافية أدناه وتأكيد الفاتورة أو طباعتها.
    </div>
</div>

<style>
@keyframes slideIn {
    from { transform: translateX(-60px); opacity: 0; }
    to   { transform: translateX(0);    opacity: 1; }
}
@keyframes slideOut {
    from { transform: translateX(0);    opacity: 1; }
    to   { transform: translateX(-60px); opacity: 0; }
}
</style>

<script>
(function () {
    var pendingCount = parseInt(document.getElementById('<%= hfPendingCount.ClientID %>').value) || 0;
    if (pendingCount <= 0) return;

    var toast = document.getElementById('invoiceToast');
    var msg = document.getElementById('toastMsg');
    msg.textContent = 'يوجد ' + pendingCount + ' فاتورة خدمة إضافية بانتظار التأكيد';

    function showToast() {
        toast.style.display = 'block';
        toast.style.animation = 'slideIn 0.4s ease';
        setTimeout(function () {
            toast.style.animation = 'slideOut 0.4s ease forwards';
            setTimeout(function () {
                toast.style.display = 'none';
            }, 400);
        }, 5000);
    }

    // يظهر فور تحميل الصفحة
    setTimeout(showToast, 1500);
    // ثم يتكرر كل 12 ثانية
    setInterval(showToast, 12000);
})();
</script>

</asp:Content>
