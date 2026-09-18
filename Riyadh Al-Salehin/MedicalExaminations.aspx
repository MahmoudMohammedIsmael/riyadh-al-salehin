<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="MedicalExaminations.aspx.cs" Inherits="Riyadh_Al_Salehin.MedicalExaminations" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-danger text-white">
                <h5 class="mb-0">🩺 الفحص الطبي والتشخيص</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>الموعد (Appointment)</label>
                        <asp:TextBox ID="txtAppointmentId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-8">
                        <label>التشخيص المبدئي</label>
                        <asp:TextBox ID="txtInitialDiagnosis" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>نوع القرار</label>
                        <asp:TextBox ID="txtDecisionType" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>الحالة</label>
                        <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="mt-3">

                    <asp:Button ID="btnSave" runat="server" Text="💾 حفظ"
                        CssClass="btn btn-success"
                        OnClick="btnSave_Click" />

                    <asp:Button ID="btnNew" runat="server" Text="🆕 جديد"
                        CssClass="btn btn-secondary"
                        OnClick="btnNew_Click" />

                </div>

            </div>
        </div>

        <!-- GRID -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة الفحوصات الطبية</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvExaminations" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvExaminations_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" />

                        <asp:BoundField DataField="InitialDiagnosis" HeaderText="التشخيص" />
                        <asp:BoundField DataField="DecisionType" HeaderText="القرار" />
                        <asp:BoundField DataField="Status" HeaderText="الحالة" />
                        <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />

                        <asp:TemplateField HeaderText="العمليات">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server"
                                    Text="تعديل"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="EditRow"
                                    CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnDelete" runat="server"
                                    Text="حذف"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('هل تريد حذف الفحص؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>