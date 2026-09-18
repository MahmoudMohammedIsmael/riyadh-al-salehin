<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="Surgeries.aspx.cs" Inherits="Riyadh_Al_Salehin.Surgeries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-3">

    <div class="card shadow-sm">

        <div class="card-header bg-danger text-white">
            <h5 class="mb-0">🏥 إدارة العمليات الجراحية</h5>
        </div>

        <div class="card-body">

            <asp:HiddenField ID="hfId" runat="server" />

            <!-- رسالة النتيجة: في الأعلى وبشكل واضح حتى لا تُفوّت -->
            <div class="alert" id="msgBox" runat="server" style="display:block;">
                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    EnableViewState="false"
                    ForeColor="Red"
                    Font-Bold="true" />
            </div>

            <div class="row">

                <div class="col-md-3">
                    <label>المريض ID</label>
                    <asp:TextBox ID="txtPatientId" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>

                <div class="col-md-3">
                    <label>الطبيب ID</label>
                    <asp:TextBox ID="txtDoctorId" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>

                <div class="col-md-3">
                    <label>المركز ID</label>
                    <asp:TextBox ID="txtCenterId" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>

                <div class="col-md-3">
                    <label>غرفة العمليات</label>

                    <asp:DropDownList
                        ID="ddlRoomId"
                        runat="server"
                        CssClass="form-control">
                    </asp:DropDownList>
                </div>

            </div>

            <div class="row mt-2">

                <div class="col-md-4">
                    <label>نوع العملية</label>
                    <asp:TextBox ID="txtSurgeryType" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-4">
                    <label>اسم العملية</label>
                    <asp:TextBox ID="txtSurgeryName" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-4">
                    <label>تاريخ العملية</label>
                    <asp:TextBox ID="txtSurgeryDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

            </div>

            <div class="row mt-2">

                <div class="col-md-3">
                    <label>المدة (ساعات)</label>
                    <asp:TextBox
                        ID="txtDuration"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Number"
                        min="1"
                        step="1" />
                </div>

                <div class="col-md-3">
                    <label class="fw-bold text-primary">💰 إجمالي تكلفة العملية الكلية</label>
                    <asp:TextBox
                        ID="txtTotalCost"
                        runat="server"
                        CssClass="form-control border-primary fw-bold"
                        placeholder="مثال: 10000" />
                </div>

                <div class="col-md-3">
                    <label>الحالة</label>
                    <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3">
                    <label>أنشئ بواسطة</label>
                    <asp:TextBox ID="txtCreatedBy" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>

            </div>

            <div class="row mt-2">

                <div class="col-md-12">
                    <label>ملاحظات</label>
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" />
                </div>

            </div>

            <div class="mt-3">

                <asp:Button ID="btnSave" runat="server" Text="💾 حفظ"
                    CssClass="btn btn-success"
                    OnClick="btnSave_Click"
                    CausesValidation="false" />

                <asp:Button ID="btnNew" runat="server" Text="🆕 جديد"
                    CssClass="btn btn-secondary"
                    OnClick="btnNew_Click"
                    CausesValidation="false" />

            </div>

        </div>

        <!-- GRID -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة العمليات الجراحية</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvSurgeries" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvSurgeries_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />
                        <asp:BoundField DataField="RoomName" HeaderText="الغرفة" />

                        <asp:BoundField DataField="SurgeryName" HeaderText="العملية" />
                        <asp:BoundField DataField="SurgeryDate" HeaderText="التاريخ" />
                        <asp:BoundField DataField="Status" HeaderText="الحالة" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل الحالة"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CausesValidation="false" />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CausesValidation="false"
                                    OnClientClick="return confirm('هل تريد حذف العملية؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>


    </div>

</div>

</asp:Content>
