<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="Appointments.aspx.cs" Inherits="Riyadh_Al_Salehin.Appointments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   
<div class="container mt-4" dir="rtl">

    <div class="card shadow">
        <div class="card-header bg-primary text-white">
            <h4>إدارة المواعيد</h4>
        </div>

        <div class="card-body">

            <asp:HiddenField ID="hfId" runat="server" />

            <div class="row">

                <!-- المريض -->
                <div class="col-md-4 mb-3">
                    <label>المريض</label>
                    <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-control" />
                </div>

                <!-- الطبيب -->
                <div class="col-md-4 mb-3">
                    <label>الطبيب</label>
                    <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-control" />
                </div>

                <!-- المركز -->
                <div class="col-md-4 mb-3">
                    <label>المركز</label>
                    <asp:DropDownList ID="ddlCenter" runat="server" CssClass="form-control" />
                </div>

                <!-- التاريخ -->
                <div class="col-md-4 mb-3">
                    <label>تاريخ الموعد</label>
                    <asp:TextBox ID="txtDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control" />
                </div>

                <!-- الحالة -->
                <div class="col-md-4 mb-3">
                    <label>الحالة</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                        <asp:ListItem Text="pending" Value="pending" />
                        <asp:ListItem Text="confirmed" Value="confirmed" />
                        <asp:ListItem Text="done" Value="done" />
                        <asp:ListItem Text="cancelled" Value="cancelled" />
                    </asp:DropDownList>
                </div>

                <!-- ملاحظات -->
                <div class="col-md-12 mb-3">
                    <label>ملاحظات</label>
                    <asp:TextBox ID="txtNotes" runat="server"
                        TextMode="MultiLine"
                        Rows="3"
                        CssClass="form-control" />
                </div>

            </div>

            <div class="text-center mb-3">

                <asp:Button ID="btnSave" runat="server"
                    Text="حفظ"
                    CssClass="btn btn-success"
                    OnClick="btnSave_Click" />

                <asp:Button ID="btnUpdate" runat="server"
                    Text="تعديل"
                    CssClass="btn btn-warning"
                    OnClick="btnUpdate_Click" />

                <asp:Button ID="btnDelete" runat="server"
                    Text="حذف"
                    CssClass="btn btn-danger"
                    OnClick="btnDelete_Click"
                    OnClientClick="return confirm('تأكيد الحذف؟');" />

                <asp:Button ID="btnNew" runat="server"
                    Text="جديد"
                    CssClass="btn btn-secondary"
                    OnClick="btnNew_Click" />

            </div>

            <asp:Label ID="lblMessage" runat="server" Font-Bold="true" />

        </div>
    </div>

    <br />

    <div class="card shadow">

        <div class="card-header bg-dark text-white">
            قائمة المواعيد
        </div>

        <div class="card-body">

            <asp:TextBox ID="txtSearch" runat="server"
                CssClass="form-control mb-2"
                placeholder="بحث بالحالة (pending, confirmed...)" />

            <asp:Button ID="btnSearch" runat="server"
                Text="بحث"
                CssClass="btn btn-primary mb-3"
                OnClick="btnSearch_Click" />

            <asp:GridView ID="gvAppointments"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                DataKeyNames="Id"
                OnSelectedIndexChanged="gvAppointments_SelectedIndexChanged">

                <Columns>

                    <asp:BoundField DataField="Id" HeaderText="#" />

                    <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                    <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                    <asp:BoundField DataField="CenterName" HeaderText="المركز" />

                    <asp:BoundField DataField="AppointmentDate" HeaderText="التاريخ" />
                    <asp:BoundField DataField="Status" HeaderText="الحالة" />

                    <asp:CommandField ShowSelectButton="True" SelectText="اختيار" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>

</asp:Content>
