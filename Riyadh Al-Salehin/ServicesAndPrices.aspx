<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="ServicesAndPrices.aspx.cs" Inherits="Riyadh_Al_Salehin.ServicesAndPrices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">

            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">💰 إدارة الخدمات والأسعار</h5>
            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfId" runat="server" />

                <div class="row">

                    <div class="col-md-4">
                        <label>اسم الخدمة</label>
                        <asp:TextBox ID="txtServiceName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>التصنيف</label>
                        <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>السعر الأساسي</label>
                        <asp:TextBox ID="txtBasePrice" runat="server" CssClass="form-control" 
                            AutoPostBack="true" OnTextChanged="txtBasePrice_TextChanged" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-3">
                        <label>نسبة الطبيب (%)</label>
                        <asp:TextBox ID="txtDoctorRate" runat="server" CssClass="form-control" 
                            AutoPostBack="true" OnTextChanged="Rates_TextChanged" />
                    </div>

                    <div class="col-md-3">
                        <label>نسبة المركز (%)</label>
                        <asp:TextBox ID="txtCenterRate" runat="server" CssClass="form-control" 
                            AutoPostBack="true" OnTextChanged="Rates_TextChanged" />
                    </div>

                    <div class="col-md-3">
                        <label>نصيب الطبيب 💰</label>
                        <asp:TextBox ID="txtDoctorShare" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                    </div>

                    <div class="col-md-3">
                        <label>نصيب المركز 💰</label>
                        <asp:TextBox ID="txtCenterShare" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                    </div>

                </div>

                <div class="row mt-2">

                    <div class="col-md-4">
                        <label>سعر النقدي (الإجمالي)</label>
                        <asp:TextBox ID="txtCashRate" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4">
                        <label>سعر التأمين</label>
                        <asp:TextBox ID="txtInsuranceRate" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-4 d-flex align-items-end">
                        <asp:Button ID="btnSave" runat="server" Text="💾 حفظ"
                            CssClass="btn btn-success me-2"
                            OnClick="btnSave_Click" />

                        <asp:Button ID="btnNew" runat="server" Text="🆕 جديد"
                            CssClass="btn btn-secondary"
                            OnClick="btnNew_Click" />
                    </div>

                </div>

            </div>
        </div>

        <!-- GRID -->
        <div class="card mt-3 shadow-sm">

            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">📋 قائمة الخدمات</h6>
            </div>

            <div class="card-body p-0">

                <asp:GridView ID="gvServices" runat="server"
                    CssClass="table table-bordered table-hover mb-0"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvServices_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="ServiceName" HeaderText="الخدمة" />
                        <asp:BoundField DataField="Category" HeaderText="التصنيف" />
                        <asp:BoundField DataField="BasePrice" HeaderText="السعر الأساسي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="DoctorRate" HeaderText="نسبة الطبيب %" DataFormatString="{0:N0}%" />
                        <asp:BoundField DataField="CenterRate" HeaderText="نسبة المركز %" DataFormatString="{0:N0}%" />

                        <asp:TemplateField HeaderText="نصيب الطبيب">
                            <ItemTemplate>
                                <%# (Eval("BasePrice") != DBNull.Value && Eval("DoctorRate") != DBNull.Value) 
                                    ? string.Format("{0:N2}", Convert.ToDecimal(Eval("BasePrice")) * Convert.ToDecimal(Eval("DoctorRate")) / 100) 
                                    : "0.00" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="نصيب المركز">
                            <ItemTemplate>
                                <%# (Eval("BasePrice") != DBNull.Value && Eval("CenterRate") != DBNull.Value) 
                                    ? string.Format("{0:N2}", Convert.ToDecimal(Eval("BasePrice")) * Convert.ToDecimal(Eval("CenterRate")) / 100) 
                                    : "0.00" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="CashRate" HeaderText="نقدي" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="InsuranceRate" HeaderText="تأمين" DataFormatString="{0:N2}" />

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
                                    OnClientClick="return confirm('هل تريد حذف الخدمة؟');" />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>
        </div>

    </div>

</asp:Content>