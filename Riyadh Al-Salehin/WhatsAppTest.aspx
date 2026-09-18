<%@ Page Language="C#" AutoEventWireup="true"
    Async="true"
    CodeBehind="WhatsAppTest.aspx.cs"
    Inherits="Riyadh_Al_Salehin.WhatsAppTest" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>WhatsApp Test</title>
    <style type="text/css">
        @media (max-width: 767.98px) {
            body { margin: 0; padding: 12px; font-family: Tahoma, sans-serif; }
            #form1 { width: 100%; }
            input[type="submit"],
            input[type="button"] { width: 100%; min-height: 44px; font-size: 15px; }
            #<%= lblResult.ClientID %> { display: block; margin-top: 14px; overflow-wrap: anywhere; line-height: 1.7; }
        }
    </style>
</head>

<body>

<form id="form1" runat="server">

    <asp:Button
        ID="btnSend"
        runat="server"
        Text="إرسال رسالة واتساب"
        OnClick="btnSend_Click" />

    <br />
    <br />

    <asp:Label
        ID="lblResult"
        runat="server" />

</form>

</body>
</html>