<%@ Page Title="استيراد ICD-10" Language="C#" MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true"
    CodeBehind="ImportICD.aspx.cs"
    Inherits="Riyadh_Al_Salehin.ImportICD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">

        <h3>استيراد تشخيصات ICD-10</h3>

        <br />

        <asp:Button
            ID="btnImport"
            runat="server"
            CssClass="btn btn-success"
            Text="بدء الاستيراد"
            OnClick="btnImport_Click" />

        <br /><br />

        <asp:Label
            ID="lblResult"
            runat="server"
            Font-Bold="true"
            ForeColor="Blue" />

    </div>

</asp:Content>