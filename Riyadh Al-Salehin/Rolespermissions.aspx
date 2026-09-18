<%@ Page Title="إدارة الأدوار والصلاحيات" Language="C#" MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true" CodeBehind="Rolespermissions.aspx.cs" Inherits="Riyadh_Al_Salehin.Rolespermissions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4" dir="rtl">

        <h3 class="mb-4"><i class="bi bi-shield-lock"></i> إدارة الأدوار والصلاحيات</h3>

        <div class="row">
            <!-- ============ عمود الأدوار ============ -->
            <div class="col-lg-5">

                <!-- كارت نموذج الدور -->
                <div class="card shadow-sm mb-4">
                    <div class="card-header bg-primary text-white">
                        بيانات الدور
                    </div>
                    <div class="card-body">
                        <asp:HiddenField ID="hfRoleId" runat="server" Value="0" />

                        <div class="mb-3">
                            <label class="form-label">اسم الدور *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                CssClass="text-danger small" ErrorMessage="اسم الدور مطلوب" Display="Dynamic"
                                ValidationGroup="RoleGroup" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">الوصف</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>

                        <div class="d-flex gap-2">
                            <asp:Button ID="btnSave" runat="server" Text="حفظ" CssClass="btn btn-success"
                                ValidationGroup="RoleGroup" OnClick="btnSave_Click" />
                            <asp:Button ID="btnUpdate" runat="server" Text="تعديل" CssClass="btn btn-warning"
                                ValidationGroup="RoleGroup" OnClick="btnUpdate_Click" Enabled="false" />
                            <asp:Button ID="btnDelete" runat="server" Text="حذف" CssClass="btn btn-danger"
                                OnClick="btnDelete_Click" Enabled="false"
                                OnClientClick="return confirm('هل أنت متأكد من حذف هذا الدور؟');" />
                            <asp:Button ID="btnClear" runat="server" Text="جديد / مسح" CssClass="btn btn-secondary"
                                OnClick="btnClear_Click" CausesValidation="false" />
                        </div>

                        <asp:Label ID="lblMsg" runat="server" CssClass="d-block mt-3"></asp:Label>
                    </div>
                </div>

                <!-- كارت البحث + قائمة الأدوار -->
                <div class="card shadow-sm">
                    <div class="card-header bg-light">
                        <div class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="بحث باسم الدور..."></asp:TextBox>
                            <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-outline-primary"
                                OnClick="btnSearch_Click" CausesValidation="false" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="false" CssClass="table table-hover mb-0"
                            DataKeyNames="Id" OnSelectedIndexChanging="gvRoles_SelectedIndexChanging" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="#" ItemStyle-Width="40px" />
                                <asp:BoundField DataField="Name" HeaderText="اسم الدور" />
                                <asp:BoundField DataField="Description" HeaderText="الوصف" />
                                <asp:ButtonField CommandName="Select" Text="تحديد" ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <!-- ============ عمود الصلاحيات ============ -->
            <div class="col-lg-7">
                <asp:Panel ID="pnlPermissions" runat="server" Visible="false">
                    <div class="card shadow-sm">
                        <div class="card-header bg-info text-white d-flex justify-content-between align-items-center">
                            <span>صلاحيات الدور: <asp:Label ID="lblSelectedRole" runat="server" Font-Bold="true"></asp:Label></span>
                            <asp:Button ID="btnSavePermissions" runat="server" Text="حفظ الصلاحيات"
                                CssClass="btn btn-light btn-sm" OnClick="btnSavePermissions_Click" CausesValidation="false" />
                        </div>
                        <div class="card-body" style="max-height: 600px; overflow-y: auto;">
                            <asp:Repeater ID="rptModules" runat="server" OnItemDataBound="rptModules_ItemDataBound">
                                <ItemTemplate>
                                    <div class="mb-3 border-bottom pb-2">
                                        <h6 class="text-primary mb-2">
                                            <i class="bi bi-folder2-open"></i>
                                            <%# Eval("Module") %>
                                        </h6>
                                        <asp:CheckBoxList ID="cblActions" runat="server" RepeatDirection="Horizontal"
                                            RepeatColumns="4" CssClass="ps-3"></asp:CheckBoxList>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Label ID="lblNoModules" runat="server" CssClass="text-muted" Visible="false"
                                Text="لا توجد صلاحيات معرفة بعد."></asp:Label>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlNoSelection" runat="server" Visible="true">
                    <div class="alert alert-secondary text-center">
                        الرجاء اختيار دور من القائمة لعرض/تعديل صلاحياته، أو إضافة دور جديد أولاً.
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
