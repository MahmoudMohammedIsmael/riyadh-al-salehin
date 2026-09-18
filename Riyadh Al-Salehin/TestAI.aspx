<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="TestAI.aspx.cs" Inherits="Riyadh_Al_Salehin.TestAI" %><!DOCTYPE html>
<html dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>اختبار الذكاء الاصطناعي</title>
    <style type="text/css">
        @media (max-width: 767.98px) {
            body { margin: 0; padding: 12px; font-family: Tahoma, sans-serif; }
            #form1 { width: 100%; }
            textarea,
            input[type="text"] { width: 100% !important; max-width: 100%; box-sizing: border-box; }
            input[type="submit"],
            input[type="button"] { min-height: 42px; padding: 8px 16px; font-size: 15px; }
            #<%= lblResult.ClientID %> { display: block; overflow-wrap: anywhere; line-height: 1.7; }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <asp:TextBox
        ID="txtMessage"
        runat="server"
        TextMode="MultiLine"
        Rows="5"
        Width="500px"
        Text="السلام عليكم، أريد حجز موعد عند دكتور قلب غدا">
    </asp:TextBox>

    <br /><br />

    <asp:Button
        ID="btnTest"
        runat="server"
        Text="اختبار Qwen"
        OnClick="btnTest_Click" />

    <br /><br />

    <asp:Label
        ID="lblResult"
        runat="server">
    </asp:Label>

</form>
</body>
</html>