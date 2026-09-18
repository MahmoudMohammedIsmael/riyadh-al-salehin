<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="LabResults.aspx.cs" Inherits="Riyadh_Al_Salehin.LabResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-success text-white">
                <h5 class="mb-0">📊 نتائج التحاليل (Lab Results)</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>طلب التحليل (Request ID)</label>
                        <asp:TextBox ID="txtRequestId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>النتيجة</label>
                        <asp:TextBox ID="txtResult" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>المدى الطبيعي</label>
                        <asp:TextBox ID="txtNormalRange" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>تاريخ النتيجة</label>
                        <asp:TextBox ID="txtResultDate" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4 mt-4">
                        <asp:CheckBox ID="chkSMSSent" runat="server" Text="تم إرسال SMS" />
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
                <h6 class="mb-0">📋 نتائج التحاليل</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvResults" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvResults_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="المريض" />
                        <asp:BoundField DataField="TestType" HeaderText="نوع التحليل" />
                        <asp:BoundField DataField="Result" HeaderText="النتيجة" />
                        <asp:BoundField DataField="NormalRange" HeaderText="الطبيعي" />
                        <asp:BoundField DataField="ResultDate" HeaderText="التاريخ" />
                        <asp:CheckBoxField DataField="IsSMSSent" HeaderText="SMS" />

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
                                    OnClientClick="return confirm('هل تريد حذف النتيجة؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>