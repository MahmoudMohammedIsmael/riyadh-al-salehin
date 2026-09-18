<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master"
AutoEventWireup="true"
CodeBehind="PatientSupplies.aspx.cs"
Inherits="Riyadh_Al_Salehin.PatientSupplies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-3">

<div class="card shadow">

<div class="card-header bg-warning">
<h4 class="mb-0">
💊 صرف مستلزمات العملية
</h4>
</div>

<div class="card-body">

<asp:HiddenField ID="hfId" runat="server"/>

<div class="row">

<div class="col-md-3">

<label>رقم العملية</label>

<asp:TextBox
ID="txtSurgeryId"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

<div class="col-md-3">

<label>رقم المريض</label>

<asp:TextBox
ID="txtPatientId"
runat="server"
CssClass="form-control"
ReadOnly="true"/>

</div>

</div>

</div>

</div>


<div class="card mt-3 shadow">

<div class="card-header bg-primary text-white">

<h5 class="mb-0">

حدد المستلزمات المطلوبة

</h5>

    <div class="text-center mt-4">

    <asp:Button ID="btnInvoice"
        runat="server"
        Text="💰 إنشاء فاتورة العملية"
        CssClass="btn btn-success btn-lg"
        OnClick="btnInvoice_Click" />

</div>

</div>

<!--====================== المستلزمات ======================-->

<div class="card shadow mt-4">

    <div class="card-header bg-primary text-white">
        <h5 class="mb-0">
            <i class="fa fa-medkit"></i>
            المستلزمات الموجودة بالمخزن
        </h5>
    </div>

    <div class="card-body">

        <div class="row">

            <asp:Repeater ID="rptMedicalSupplies"
                runat="server"
                OnItemCommand="rptMedicalSupplies_ItemCommand">

                <ItemTemplate>

                    <div class="col-lg-3 col-md-4 col-sm-6 mb-4">

                        <div class="card shadow h-100 border-0 rounded-4">

                            <div class="card-body">

                                <div class="text-center mb-3">

                                    <img src="images/medical.png"
                                         style="width:70px;height:70px;" />

                                </div>

                                <h5 class="text-center fw-bold">

                                    <%# Eval("SupplyName") %>

                                </h5>

                                <hr />

                                <p>

                                    <b>💰 السعر :</b>

                                    <%# Eval("Price") %>

                                </p>

                                <p>

                                    <b>📦 المتوفر :</b>

                                    <span class="badge bg-success">

                                        <%# Eval("AvailableQuantity") %>

                                    </span>

                                </p>

                                <p>

                                    <b>📏 الوحدة :</b>

                                    <%# Eval("Unit") %>

                                </p>

                                <div class="mt-3">

                                    <asp:TextBox
                                        ID="txtQty"
                                        runat="server"
                                        Text="1"
                                        CssClass="form-control text-center" />

                                </div>

                                <div class="d-grid mt-3">

                                    <asp:Button
                                        ID="btnAdd"
                                        runat="server"
                                        Text="➕ إضافة للمريض"
                                        CssClass="btn btn-success rounded-pill"
                                        CommandName="AddSupply"
                                        CommandArgument='<%# Eval("Id") %>' />

                                </div>

                            </div>

                        </div>

                    </div>

                </ItemTemplate>

            </asp:Repeater>

        </div>

    </div>

</div>


<div class="card mt-3 shadow">

<div class="card-header bg-dark text-white">

<h5 class="mb-0">

📋 مستلزمات المريض

</h5>

</div>

<div class="card-body p-0">

<asp:GridView
ID="gvPatientSupplies"
runat="server"
AutoGenerateColumns="False"
CssClass="table table-bordered table-hover mb-0"
OnRowCommand="gvPatientSupplies_RowCommand">

<Columns>

<asp:BoundField
DataField="Id"
HeaderText="#"/>

<asp:BoundField
DataField="SupplyName"
HeaderText="المستلزم"/>

<asp:BoundField
DataField="Quantity"
HeaderText="الكمية"/>

<asp:BoundField
DataField="Price"
HeaderText="السعر"/>

<asp:TemplateField HeaderText="حذف">

<ItemTemplate>

<asp:Button
ID="btnDelete"
runat="server"
Text="🗑"
CssClass="btn btn-danger btn-sm"
CommandName="DeleteRow"
CommandArgument='<%# Eval("Id") %>'
OnClientClick="return confirm('هل تريد حذف المستلزم؟');"/>

</ItemTemplate>

</asp:TemplateField>

</Columns>

</asp:GridView>

</div>

</div>

</div>

</asp:Content>