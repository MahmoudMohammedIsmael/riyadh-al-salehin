<%@ Page Title="رفع نتيجة الأشعة" Language="C#" MasterPageFile="~/Riyadh.Master" AutoEventWireup="true" CodeBehind="XrayResultEntry.aspx.cs" Inherits="Riyadh_Al_Salehin.XrayResultEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-3">

        <div class="card shadow-sm">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0">📄 كتابة تقرير نتيجة الأشعة</h5>
            </div>
            <div class="card-body">

                <asp:HiddenField ID="hfRequestId" runat="server" />

                <div class="row">
                    <div class="col-md-6">
                        <label>اسم المريض</label>
                        <asp:TextBox ID="txtPatientName" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-6">
                        <label>نوع الخدمة</label>
                        <asp:TextBox ID="txtServiceName" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>

                <div class="row mt-3">
                    <div class="col-md-12">
                        <label>تقرير النتيجة</label>
                        <asp:TextBox ID="txtResult" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" />
                    </div>
                </div>

                <div class="row mt-3">
                    <div class="col-md-12">
                        <label>رفع ملف (صورة / PDF)</label>
                        <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-12">
                        <asp:Button ID="btnSave" runat="server" Text="💾 حفظ التقرير وإنهاء الطلب" CssClass="btn btn-success btn-lg" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="🔙 رجوع" CssClass="btn btn-secondary btn-lg" OnClick="btnCancel_Click" />
                        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info d-block mt-2" Visible="false" />
                    </div>
                </div>

            </div>
        </div>

    </div>

</asp:Content>