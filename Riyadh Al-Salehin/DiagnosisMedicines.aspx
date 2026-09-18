<%@ Page Title="ربط الأدوية بالتشخيصات" Language="C#" MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true"
    CodeBehind="DiagnosisMedicines.aspx.cs"
    Inherits="Riyadh_Al_Salehin.DiagnosisMedicines" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">
        <h3>ربط الأدوية بالتشخيصات</h3>

        <div class="card p-3 mb-4">
            <div class="row g-2">
                <div class="col-md-4">
                    <label>التشخيص</label>
                    <asp:DropDownList ID="ddlDiagnosis" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlDiagnosis_SelectedIndexChanged" />
                </div>
                <div class="col-md-4">
                    <label>الدواء (اكتب للبحث)</label>
                    <asp:TextBox ID="txtMedicineSearch" runat="server" CssClass="form-control" autocomplete="off" />
                    <asp:HiddenField ID="hdnMedicineId" runat="server" />
                    <div id="medicineResults" class="list-group position-absolute" style="z-index:1000;"></div>
                </div>
                <div class="col-md-2">
                    <label>مفضّل؟</label><br />
                    <asp:CheckBox ID="chkPreferred" runat="server" Checked="true" />
                </div>
                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnAdd" runat="server" Text="إضافة" CssClass="btn btn-success w-100"
                        OnClick="btnAdd_Click" />
                </div>
                <div class="col-12 mt-2">
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" placeholder="ملاحظات (اختياري)" />
                </div>
            </div>
            <asp:Label ID="lblMsg" runat="server" CssClass="mt-2 d-block text-primary fw-bold" />
        </div>

        <asp:GridView ID="gvLinked" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
            DataKeyNames="Id" OnRowCommand="gvLinked_RowCommand">
            <Columns>
                <asp:BoundField DataField="MedicineName" HeaderText="الدواء" />
                <asp:CheckBoxField DataField="IsPreferred" HeaderText="مفضّل" />
                <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button runat="server" Text="حذف" CommandName="DeleteLink"
                            CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-danger" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).on('input', '#<%= txtMedicineSearch.ClientID %>', function () {
            var term = $(this).val();
            if (term.length < 2) { $('#medicineResults').empty(); return; }

            $.ajax({
                type: "POST",
                url: "DiagnosisMedicines.aspx/SearchMedicines",
                data: JSON.stringify({ term: term }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var results = response.d;
                    var html = '';
                    results.forEach(function (item) {
                        html += '<a href="#" class="list-group-item list-group-item-action med-item" data-id="' + item.Id + '">' + item.MedicineName + '</a>';
                    });
                    $('#medicineResults').html(html);
                }
            });
        });

        $(document).on('click', '.med-item', function (e) {
            e.preventDefault();
            $('#<%= txtMedicineSearch.ClientID %>').val($(this).text());
            $('#<%= hdnMedicineId.ClientID %>').val($(this).data('id'));
            $('#medicineResults').empty();
        });
</script>

</asp:Content>