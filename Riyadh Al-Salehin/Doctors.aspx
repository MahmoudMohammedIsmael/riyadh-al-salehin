<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="Doctors.aspx.cs" Inherits="Riyadh_Al_Salehin.Doctors" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-5 mb-5" dir="rtl">

    <!-- بطاقة بيانات الطبيب -->
    <div class="card shadow-lg border-0 mb-4">
        <div class="card-header bg-primary text-white">
            <h4 class="mb-0">إدارة الأطباء</h4>
        </div>
        <div class="card-body">
            <asp:HiddenField ID="hfDoctorId" runat="server" />

            <div class="row">
                <div class="col-md-3 mb-3">
                    <label>اسم الطبيب <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtDoctorName" runat="server" CssClass="form-control" placeholder="أدخل اسم الطبيب" />
                </div>
                <div class="col-md-3 mb-3">
                    <label>حرف / كود الانتظار</label>
                    <asp:TextBox ID="txtDoctorCode" runat="server" CssClass="form-control text-uppercase" placeholder="تلقائي: A, B, C..." MaxLength="10" />
                </div>
                <div class="col-md-3 mb-3">
                    <label>التخصص</label>
                    <asp:TextBox ID="txtSpecialty" runat="server" CssClass="form-control" placeholder="مثال: أسنان، عيون..." />
                </div>
                <div class="col-md-3 mb-3">
                    <label>رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="05xxxxxxxx" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label>البريد الإلكتروني</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="example@email.com" />
                </div>
                <div class="col-md-4 mb-3">
                    <label>نسبة العمولة %</label>
                    <asp:TextBox ID="txtCommissionRate" runat="server" CssClass="form-control" placeholder="مثال: 10.00" Text="0" />
                </div>
                <div class="col-md-4 mb-3">
                    <label>رسوم الكشف</label>
                    <asp:TextBox ID="txtConsultationFee" runat="server" CssClass="form-control" placeholder="مثال: 100.00" Text="0" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label>نوع الطبيب</label>
                    <asp:DropDownList ID="ddlDoctorType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="طبيب عيادات" Value="Clinic" />
                        <asp:ListItem Text="طبيب عمليات" Value="Surgery" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-4 mb-3">
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" onchange="previewImage(this);" />
                    <br />
                    <asp:Image ID="imgPreview" runat="server" Width="120" Height="120" ImageUrl="~/Images/no-image.png" />
                </div>
            </div>

            <hr />

            <!-- إضافة جدول مواعيد -->
            <div class="card bg-light">
                <div class="card-header bg-secondary text-white">
                    <h5 class="mb-0">إضافة جدول مواعيد للطبيب</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label>أيام العمل</label>
                            <asp:CheckBoxList ID="cblDays" runat="server" RepeatDirection="Horizontal" CssClass="list-inline">
                                <asp:ListItem Value="0" Text="الأحد" />
                                <asp:ListItem Value="1" Text="الإثنين" />
                                <asp:ListItem Value="2" Text="الثلاثاء" />
                                <asp:ListItem Value="3" Text="الأربعاء" />
                                <asp:ListItem Value="4" Text="الخميس" />
                                <asp:ListItem Value="5" Text="الجمعة" />
                                <asp:ListItem Value="6" Text="السبت" />
                            </asp:CheckBoxList>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>وقت البداية</label>
                            <asp:TextBox ID="txtStartTime" runat="server" TextMode="Time" CssClass="form-control" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>وقت النهاية</label>
                            <asp:TextBox ID="txtEndTime" runat="server" TextMode="Time" CssClass="form-control" />
                        </div>
                        <div class="col-md-2 mb-3">
                            <label>عدد الأسابيع</label>
                            <asp:TextBox ID="txtWeeks" runat="server" Text="52" CssClass="form-control" TextMode="Number" />
                        </div>
                    </div>
                    <div class="text-center">
                        <asp:Button ID="btnAddSchedule" runat="server" Text="إضافة مواعيد متكررة" CssClass="btn btn-primary" OnClick="btnAddSchedule_Click" />
                    </div>
                </div>
            </div>

            <hr />

            <div class="text-center mb-3">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Size="Large" />
            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap">
                <asp:Button ID="btnSave"   runat="server" Text="حفظ"   CssClass="btn btn-success px-4" OnClick="btnSave_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="تعديل" CssClass="btn btn-warning px-4"  OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="حذف"   CssClass="btn btn-danger px-4"  OnClientClick="return confirm('هل تريد حذف هذا الطبيب؟');" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear"  runat="server" Text="جديد"  CssClass="btn btn-secondary px-4" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>

    <!-- بطاقة قائمة الأطباء -->
    <div class="card shadow-lg border-0 mb-4">
        <div class="card-header bg-dark text-white">
            <h5 class="mb-0">قائمة الأطباء</h5>
        </div>
        <div class="card-body">
            <div class="row justify-content-center mb-4">
                <div class="col-md-9">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="ابحث باسم الطبيب أو التخصص أو الهاتف..." />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                </div>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="gvDoctors" runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-hover table-bordered align-middle text-center"
                    HeaderStyle-CssClass="table-primary"
                    DataKeyNames="Id"
                    OnSelectedIndexChanged="gvDoctors_SelectedIndexChanged"
                    EmptyDataText="لا توجد بيانات للعرض">
                    <Columns>
                        <asp:TemplateField HeaderText="الصورة">
                            <ItemTemplate>
                                <img src='Uploads/Doctors/<%# Eval("Id") %>.jpg' width="60" height="60" style="border-radius:50%;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Id" HeaderText="#" />
                        <asp:BoundField DataField="DoctorCode" HeaderText="حرف الانتظار" />
                        <asp:BoundField DataField="DoctorType" HeaderText="نوع الطبيب" />
                        <asp:BoundField DataField="DoctorName" HeaderText="اسم الطبيب" />
                        <asp:BoundField DataField="Specialty" HeaderText="التخصص" />
                        <asp:BoundField DataField="Phone" HeaderText="الهاتف" />
                        <asp:BoundField DataField="Email" HeaderText="البريد الإلكتروني" />
                        <asp:BoundField DataField="CommissionRate" HeaderText="نسبة العمولة %" DataFormatString="{0:F2}" />
                        <asp:CommandField ShowSelectButton="True" SelectText="اختيار" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- بطاقة المواعيد القادمة -->
    <div class="card shadow-lg border-0">
        <div class="card-header bg-success text-white d-flex justify-content-between align-items-center">
            <asp:Label ID="lblScheduleTitle" runat="server" CssClass="h5 mb-0" Text="جميع المواعيد القادمة" />
            <asp:Button ID="btnDeleteUpcoming" runat="server" Text="حذف المواعيد القادمة" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteUpcoming_Click" Visible="false" />
        </div>
        <div class="card-body">
            <!-- فلترة المواعيد -->
            <div class="row mb-4">
                <div class="col-md-3">
                    <label>الطبيب</label>
                    <asp:DropDownList ID="ddlDoctorFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDoctorFilter_SelectedIndexChanged">
                        <asp:ListItem Text="جميع الأطباء" Value="0" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>من تاريخ</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <label>إلى تاريخ</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnFilter" runat="server" Text="بحث" CssClass="btn btn-primary w-100" OnClick="btnFilter_Click" />
                </div>
                <div class="col-md-3 d-flex align-items-end justify-content-end">
                    <asp:Button ID="btnClearFilter" runat="server" Text="إلغاء الفلتر" CssClass="btn btn-secondary" OnClick="btnClearFilter_Click" />
                </div>
            </div>

            <!-- عرض المواعيد المجمعة -->
            <asp:Repeater ID="rptSchedules" runat="server" OnItemDataBound="rptSchedules_ItemDataBound" OnItemCommand="rptSchedules_ItemCommand">
                <ItemTemplate>
                    <div class="doctor-group mb-4 border rounded p-3">
                        <h5><%# Eval("DoctorNameDisplay") %></h5>
                        <p class="text-muted"><%# Eval("Specialty") %></p>
                        <asp:Repeater ID="rptDays" runat="server" OnItemDataBound="rptDays_ItemDataBound" OnItemCommand="rptSchedules_ItemCommand">
                            <ItemTemplate>
                                <div class="day-card mb-3 border-bottom pb-3">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div>
                                            <strong><%# Eval("DayName") %></strong>
                                            <span class="text-muted"><%# Eval("DateDisplay") %></span>
                                        </div>
                                        <div>
                                            <span class="badge bg-secondary">الإجمالي: <%# Eval("Total") %></span>
                                            <span class="badge bg-success">المتاح: <%# Eval("Available") %></span>
                                            <span class="badge bg-danger">المحجوز: <%# Eval("Booked") %></span>
                                        </div>
                                        <div>
                                            <asp:LinkButton ID="btnShowDetails" runat="server" CssClass="btn btn-info btn-sm" OnClientClick="return false;" data-toggle="collapse" data-target='<%# "details-" + Eval("DoctorId") + "-" + Eval("Date", "{0:yyyy-MM-dd}") %>' aria-expanded="false">عرض المواعيد</asp:LinkButton>
                                            <asp:LinkButton ID="btnDeleteDay" runat="server" CssClass="btn btn-danger btn-sm" CommandName="DeleteDay" CommandArgument='<%# Eval("DoctorId") + "|" + Eval("Date", "{0:yyyy-MM-dd}") %>' OnClientClick="return confirm('هل أنت متأكد من حذف جميع المواعيد غير المحجوزة لهذا اليوم؟');">حذف اليوم</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div id='<%# "details-" + Eval("DoctorId") + "-" + Eval("Date", "{0:yyyy-MM-dd}") %>' class="collapse mt-2">
                                        <asp:GridView ID="gvDayDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-sm table-bordered">
                                            <Columns>
                                                <asp:BoundField DataField="StartTime" HeaderText="البداية" DataFormatString="{0:HH:mm}" />
                                                <asp:BoundField DataField="EndTime" HeaderText="النهاية" DataFormatString="{0:HH:mm}" />
                                                <asp:TemplateField HeaderText="الحالة">
                                                    <ItemTemplate>
                                                        <span class='badge <%# (bool)Eval("IsBooked") ? "bg-danger" : "bg-success" %>'>
                                                            <%# (bool)Eval("IsBooked") ? "محجوز" : "متاح" %>
                                                        </span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="حذف">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnDeleteSchedule" runat="server" CssClass="btn btn-danger btn-sm" CommandName="DeleteSchedule" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('حذف هذا الموعد؟');">حذف</asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoSchedules" runat="server" Text="لا توجد مواعيد قادمة" CssClass="text-center d-block" Visible="false" />
        </div>
    </div>
</div>

<style>
    .card { border: none; border-radius: 15px; box-shadow: 0 4px 20px rgba(0,0,0,.1); }
    .card-header { padding: 18px 25px; border-radius: 15px 15px 0 0 !important; }
    .card-header h4, .card-header h5, .card-header .h5 { font-size: 22px; font-weight: bold; }
    .card-body { padding: 30px; }
    label { font-size: 17px; font-weight: 600; margin-bottom: 6px; display: block; }
    .form-control { height: 48px; font-size: 17px; border-radius: 10px; }
    .btn { font-size: 16px; min-width: 130px; padding: 10px 18px; border-radius: 10px; }
    .table { font-size: 16px; }
    .table th { font-size: 17px; font-weight: bold; }
    .table-hover tbody tr:hover { background-color: #f0f4ff; cursor: pointer; }
    .day-card { background: #f8f9fa; padding: 10px 15px; border-radius: 10px; }
    .doctor-group { background: #fff; }
</style>

<script type="text/javascript">
    function previewImage(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('<%= imgPreview.ClientID %>').src = e.target.result;
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
</script>

</asp:Content>