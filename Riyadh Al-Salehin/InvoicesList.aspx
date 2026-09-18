<%@ Page Title="قائمة الفواتير" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="InvoicesList.aspx.cs" Inherits="Riyadh_Al_Salehin.InvoicesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>

    <script src="assets/css/css1/jquery.dataTables.min.js"></script>
    <script src="assets/css/css1/jquery-3.6.0.min.js"></script>
    <link  rel="stylesheet" type="text/css"  href="assets/css/css1/jquery.dataTables.min.css"  />
    <style>
        .page-title-bar {
            background: linear-gradient(135deg, #0d6efd, #0a58ca);
            color: #fff;
            padding: 16px 24px;
            border-radius: 12px 12px 0 0;
            display: flex;
            align-items: center;
            gap: 10px;
            box-shadow: 0 4px 12px rgba(13, 110, 253, 0.25);
        }
        .page-title-bar h4, .page-title-bar span { color: #fff !important; margin: 0; }
        .stat-card {
            background: #f8f9fc;
            border-radius: 10px;
            padding: 15px;
            text-align: center;
            border: 1px solid #e0e0e0;
            transition: 0.3s;
        }
        .stat-card:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
        .stat-number { font-size: 28px; font-weight: bold; }
        .stat-label { font-size: 14px; color: #6c757d; }
        .table-responsive { margin-top: 20px; }
        .filter-bar { background: #f1f3f5; padding: 15px; border-radius: 10px; margin-bottom: 20px; }
        .btn-print { background: none; border: none; color: #0d6efd; cursor: pointer; }
        .btn-print:hover { color: #0a58ca; }
    </style>

    <div class="container mt-4" dir="rtl">
        <div class="card shadow">
            <div class="page-title-bar">
                <i class="bi bi-receipt"></i>
                <h4>جميع الفواتير</h4>
                <span class="badge bg-light text-dark ms-auto">آخر تحديث: <%= DateTime.Now.ToString("yyyy/MM/dd HH:mm") %></span>
            </div>

            <div class="card-body">
               
                <div class="row mb-4">
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card">
                            <div class="stat-number" style="color:#0d6efd;"><asp:Label ID="lblTotalInvoices" runat="server" Text="0" /></div>
                            <div class="stat-label">إجمالي الفواتير</div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card">
                            <div class="stat-number" style="color:#198754;"><asp:Label ID="lblTotalAmount" runat="server" Text="0 ج.م" /></div>
                            <div class="stat-label">إجمالي المبالغ</div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card">
                            <div class="stat-number" style="color:#0dcaf0;"><asp:Label ID="lblTotalPaid" runat="server" Text="0 ج.م" /></div>
                            <div class="stat-label">المدفوع</div>
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <div class="stat-card">
                            <div class="stat-number" style="color:#ffc107;"><asp:Label ID="lblTotalPending" runat="server" Text="0 ج.م" /></div>
                            <div class="stat-label">المتبقي</div>
                        </div>
                    </div>
                </div>

                
                <div class="filter-bar row g-2 align-items-end">
                    <div class="col-md-4">
                        <label>بحث</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="اسم المريض / الطبيب / رقم الفاتورة" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnReset" runat="server" Text="إعادة تعيين" CssClass="btn btn-secondary w-100" OnClick="btnSearch_Click" CommandArgument="Reset" />
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvInvoices" runat="server" CssClass="table table-striped table-hover" 
                        AutoGenerateColumns="false" AllowPaging="true" PageSize="20"
                        OnPageIndexChanging="gvInvoices_PageIndexChanging"
                        GridLines="None" CellPadding="4" ForeColor="#333">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="رقم الفاتورة" SortExpression="Id" />
                            <asp:BoundField DataField="PatientName" HeaderText="المريض" SortExpression="PatientName" />
                            <asp:BoundField DataField="DoctorName" HeaderText="الطبيب" SortExpression="DoctorName" />
                            <asp:BoundField DataField="CenterName" HeaderText="المركز" SortExpression="CenterName" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="المبلغ" DataFormatString="{0:N2} ج.م" SortExpression="TotalAmount" />
                            <asp:BoundField DataField="PaidAmount" HeaderText="المدفوع" DataFormatString="{0:N2} ج.م" SortExpression="PaidAmount" />
                            <asp:BoundField DataField="PaymentStatus" HeaderText="حالة الدفع" SortExpression="PaymentStatus" />
                            <asp:BoundField DataField="InvoiceDate" HeaderText="التاريخ" DataFormatString="{0:yyyy/MM/dd HH:mm}" SortExpression="InvoiceDate" />
                            <asp:TemplateField HeaderText="طباعة">
                                <ItemTemplate>
                                    <asp:Button ID="btnPrint" runat="server" Text="طباعة" CssClass="btn-print" 
                                        OnClick="btnPrintInvoice_Click" CommandArgument='<%# Eval("Id") %>' 
                                        ToolTip="طباعة الفاتورة" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
      
        $(document).ready(function () {
            
        });
    </script>
</asp:Content>