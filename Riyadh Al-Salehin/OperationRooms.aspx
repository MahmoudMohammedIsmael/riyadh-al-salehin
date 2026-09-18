<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="OperationRooms.aspx.cs" Inherits="Riyadh_Al_Salehin.OperationRooms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">🏥 إدارة غرف العمليات</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>اسم الغرفة</label>
                        <asp:TextBox ID="txtRoomName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>النوع</label>
                        <asp:TextBox ID="txtType" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>السعر اليومي</label>
                        <asp:TextBox ID="txtDailyPrice" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="form-check mt-3">
                    <asp:CheckBox ID="chkAvailable" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label">متاحة</label>
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

        <!-- الجدول -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة غرف العمليات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvRooms" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvRooms_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />

                        <asp:BoundField DataField="RoomName" HeaderText="اسم الغرفة" />
                        <asp:BoundField DataField="Type" HeaderText="النوع" />
                        <asp:BoundField DataField="DailyPrice" HeaderText="السعر" />

                        <asp:CheckBoxField DataField="IsAvailable" HeaderText="متاحة" />

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
                                    OnClientClick="return confirm('هل تريد حذف هذه الغرفة؟');" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>