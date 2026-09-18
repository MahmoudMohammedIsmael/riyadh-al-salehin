<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="OperationLogs.aspx.cs" Inherits="Riyadh_Al_Salehin.OperationLogs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-danger text-white">
                <h5 class="mb-0">📜 سجل العمليات (Operation Logs)</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-3">
                        <label>المستخدم ID</label>
                        <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-3">
                        <label>العملية</label>
                        <asp:TextBox ID="txtAction" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-3">
                        <label>الموديول</label>
                        <asp:TextBox ID="txtModule" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-3">
                        <label>Record ID</label>
                        <asp:TextBox ID="txtRecordId" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-6">
                        <label>القيمة القديمة</label>
                        <asp:TextBox ID="txtOldValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>

                    <div class="col-md-6">
                        <label>القيمة الجديدة</label>
                        <asp:TextBox ID="txtNewValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-6">
                        <label>IP Address</label>
                        <asp:TextBox ID="txtIP" runat="server" CssClass="form-control" />
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
                <h6 class="mb-0">📋 سجل العمليات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvLogs" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvLogs_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="FullName" HeaderText="المستخدم" />
                        <asp:BoundField DataField="Action" HeaderText="العملية" />
                        <asp:BoundField DataField="AffectedModule" HeaderText="الموديول" />
                        <asp:BoundField DataField="RecordId" HeaderText="Record" />

                        <asp:TemplateField HeaderText="التفاصيل">
                            <ItemTemplate>
                                <asp:Button ID="btnView" runat="server"
                                    Text="عرض"
                                    CssClass="btn btn-info btn-sm"
                                    CommandName="ViewRow"
                                    CommandArgument='<%# Eval("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>