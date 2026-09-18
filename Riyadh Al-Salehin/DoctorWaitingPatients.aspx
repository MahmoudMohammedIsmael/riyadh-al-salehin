<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="DoctorWaitingPatients.aspx.cs" Inherits="Riyadh_Al_Salehin.DoctorWaitingPatients" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">







    <meta charset="utf-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />


    <link href="assets/Newcss/all.min.css" rel="stylesheet" />

    <link href="assets/Newcss/bootstrap.min.css" rel="stylesheet" />

   <%--<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css"
        rel="stylesheet" />--%>

    <style>
        body {
            background: #f5f7fb;
            font-family: Tahoma;
        }

        .headerCard {
            background: linear-gradient(45deg,#0d6efd,#198754);
            color: white;
            border-radius: 15px;
            padding: 20px;
            margin-top: 20px;
        }

        .searchCard {
            background: white;
            border-radius: 12px;
            padding: 15px;
            margin-top: 20px;
            box-shadow: 0px 3px 10px #ddd;
        }

       

        .badgeWaiting {
            background: orange;
            color: white;
            padding: 6px 12px;
            border-radius: 15px;
        }

        

        

        .grid td {
            text-align: center;
            vertical-align: middle;
        }

        .counter {
            font-size: 35px;
            font-weight: bold;
            color: #198754;
        }

        .tableCard {
    position: relative;
    z-index: 1;
    overflow: visible !important;
}

.grid {
    position: relative;
    z-index: 2;
}

.btnStart {
    position: relative;
    z-index: 9999;
    pointer-events: auto !important;
}

/* تحسينات الهاتف فقط - لا تؤثر على تصميم الكمبيوتر */
@media (max-width: 767.98px) {
    .container-fluid {
        padding-left: 10px;
        padding-right: 10px;
    }

    .headerCard {
        margin-top: 10px;
        padding: 15px 14px;
        border-radius: 14px;
    }

    .headerCard .row > div {
        text-align: center !important;
    }

    .headerCard h2 {
        font-size: 1.2rem;
        line-height: 1.6;
        margin-bottom: 8px;
    }

    .headerCard h5 {
        font-size: .95rem;
        margin-bottom: 0;
    }

    .searchCard {
        margin-top: 12px;
        padding: 12px;
        border-radius: 12px;
    }

    .searchCard .row {
        --bs-gutter-x: .5rem;
        --bs-gutter-y: .5rem;
    }

    .searchCard .col-md-8,
    .searchCard .col-md-2 {
        width: 100%;
    }

    .searchCard .form-control-lg,
    .searchCard .btn-lg {
        min-height: 44px;
        font-size: .95rem;
    }

    .searchCard .counter {
        font-size: 2rem;
        line-height: 1.2;
    }

    .tableCard {
        margin-top: 12px;
        width: 100%;
        overflow-x: auto !important;
        -webkit-overflow-scrolling: touch;
        border-radius: 10px;
    }

    .tableCard .grid {
        min-width: 760px;
        margin-bottom: 0;
        white-space: nowrap;
    }

    .tableCard .grid th,
    .tableCard .grid td {
        padding: 8px 7px;
        font-size: .82rem;
        vertical-align: middle;
    }

    .tableCard .grid th {
        white-space: nowrap;
    }

    .tableCard .grid td:nth-child(2) {
        white-space: normal;
        min-width: 145px;
    }

    .tableCard .btnStart {
        min-width: 90px;
        padding: 7px 8px;
        font-size: .82rem;
    }

    #<%= lblMessage.ClientID %> {
        display: block;
        padding: 0 8px;
        font-size: .85rem;
        line-height: 1.7;
    }
}


    </style>







<div class="container-fluid">


    <div class="headerCard">

        <div class="row">

            <div class="col-md-8">

                <h2>

                    <i class="fa-solid fa-user-doctor"></i>

                    المرضى المنتظرون للطبيب

                </h2>

            </div>

            <div class="col-md-4 text-end">

                <h5>

                    د /

                    <asp:Label
                        ID="lblDoctor"
                        runat="server"
                        Text="اسم الطبيب" />

                </h5>

            </div>

        </div>

    </div>



    <div class="searchCard">

        <div class="row">

            <div class="col-md-8">

                <asp:TextBox
                    ID="txtSearch"
                    runat="server"
                    CssClass="form-control form-control-lg"
                    placeholder="ابحث باسم المريض..." />
                <asp:Button
    ID="btnRefresh"
    runat="server"
    Text="تحديث"
    CssClass="btn btn-success btn-lg w-100"
    OnClick="btnRefresh_Click" />

            </div>

            <div class="col-md-2">

                <asp:Button
                    ID="btnSearch"
                    runat="server"
                    Text="بحث"
                    CssClass="btn btn-primary btn-lg w-100"
                    OnClick="btnSearch_Click" />

            </div>

            <div class="col-md-2 text-center">

                <div>

                    <small>المرضى المنتظرون</small>

                    <div class="counter">

                        <asp:Label
                            ID="lblCount"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

        </div>

    </div>

 

    <div class="tableCard">

        <asp:GridView

            ID="gvPatients"

            runat="server"

            CssClass="table table-bordered table-hover grid"

            AutoGenerateColumns="False"

            DataKeyNames="AppointmentId"

            OnRowCommand="gvPatients_RowCommand">

            <Columns>

                <asp:BoundField
                    DataField="QueueNumber"
                    HeaderText="الدور" />

                <asp:BoundField
                    DataField="PatientName"
                    HeaderText="اسم المريض" />

                <asp:BoundField
                    DataField="Phone"
                    HeaderText="الهاتف" />

                <asp:BoundField
                    DataField="AppointmentDate"
                    HeaderText="الموعد"
                    DataFormatString="{0:hh:mm tt}" />

                <asp:BoundField
                    DataField="InsuranceCompany"
                    HeaderText="التأمين" />

                  <asp:BoundField DataField="VisitType" HeaderText="نوع الكشف" /> 

                <asp:TemplateField HeaderText="الحالة">

                    <ItemTemplate>

                        <span class="badgeWaiting">

                            <%# Eval("Status") %>

                        </span>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="الإجراء">

                    <ItemTemplate>

                        <asp:Button

                            ID="btnStart"

                            runat="server"

                            Text="بدء الكشف"

                            CssClass="btn btn-success btnStart"

                            CommandName="Start"

                            CommandArgument='<%# Eval("AppointmentId") %>' />

                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

    <div class="text-center mt-3">

        <asp:Label

            ID="lblMessage"

            runat="server"

            ForeColor="Red"

            Font-Bold="true" />

    </div>

</div>





</asp:Content>
