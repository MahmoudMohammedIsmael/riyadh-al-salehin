<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="Riyadh_Al_Salehin.Login" %>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>تسجيل الدخول - رياض الصالحين</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            min-height: 100vh;
            background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 50%, #084298 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', Tahoma, sans-serif;
        }

        .login-wrapper {
            width: 100%;
            max-width: 440px;
            padding: 20px;
        }

        .login-card {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            overflow: hidden;
        }

        .login-header {
            background: linear-gradient(135deg, #0d6efd, #084298);
            padding: 35px 30px;
            text-align: center;
            color: white;
        }

        .login-header .logo-icon {
            width: 75px;
            height: 75px;
            background: rgba(255,255,255,0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 15px;
            font-size: 36px;
        }

        .login-header h2 {
            font-size: 22px;
            font-weight: bold;
            margin-bottom: 5px;
        }

        .login-header p {
            font-size: 14px;
            opacity: 0.85;
        }

        .login-body {
            padding: 35px 30px;
        }

        .form-label {
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 8px;
            color: #333;
            display: block;
        }

        .field-wrapper {
            position: relative;
            margin-bottom: 24px;
        }

        .input-icon {
            position: absolute;
            right: 14px;
            top: 50%;
            transform: translateY(-50%);
            font-size: 18px;
            color: #6c757d;
            z-index: 5;
            pointer-events: none;
        }

        .form-control {
            height: 52px;
            font-size: 16px;
            border-radius: 12px !important;
            border: 2px solid #e0e0e0;
            padding-right: 45px;
            padding-left: 15px;
            width: 100%;
            transition: border-color 0.3s;
        }

        .form-control:focus {
            border-color: #0d6efd;
            box-shadow: 0 0 0 3px rgba(13,110,253,0.15);
            outline: none;
        }

        .btn-login {
            width: 100%;
            height: 52px;
            font-size: 18px;
            font-weight: bold;
            border-radius: 12px;
            background: linear-gradient(135deg, #0d6efd, #084298);
            border: none;
            color: white;
            margin-top: 5px;
            cursor: pointer;
            transition: opacity 0.3s, transform 0.1s;
        }

        .btn-login:hover  { opacity: 0.92; }
        .btn-login:active { transform: scale(0.98); }

        .alert-error {
            background: #fff0f0;
            border: 1px solid #f5c6c6;
            border-radius: 10px;
            padding: 12px 15px;
            color: #dc3545;
            font-size: 15px;
            font-weight: 600;
            text-align: center;
            margin-bottom: 20px;
        }

        .login-footer {
            text-align: center;
            padding: 15px;
            background: #f8f9fa;
            color: #6c757d;
            font-size: 13px;
        }

        /* Loader styles (local to login page) */
        .page-loader { position: fixed; inset: 0; display: flex; align-items: center; justify-content: center; background: rgba(0,0,0,0.55); z-index: 10500; }
        .page-loader .loader { width: 140px; height: 140px; }
        .loader { width: 120px; max-height: 900px; transform-origin: 50% 50%; overflow: visible; }
        .ci1 { fill: #ffffff; animation: toBig 3s infinite -1.5s; transform-box: fill-box; transform-origin: 50% 50%; }
        .ciw { transform-box: fill-box; transform-origin: 50% 50%; animation: breath 3s infinite; fill: rgba(255,255,255,0.95); }
        .ci2 { fill: #ffffff; animation: toBig2 3s infinite; transform-box: fill-box; transform-origin: 50% 50%; }
        .points { animation: rot 3s infinite; transform-box: fill-box; transform-origin: 50% 50%; }
        @keyframes rot { 0% { transform: rotate(0deg); } 30% { transform: rotate(360deg); } 50% { transform: rotate(360deg); } 80% { transform: rotate(0deg); } 100% { transform: rotate(0deg); } }
        @keyframes toBig { 0% { transform: scale(1) translateX(0px); } 30% { transform: scale(1) translateX(0px); } 50% { transform: scale(10) translateX(-4.5px); } 80% { transform: scale(10) translateX(-4.5px); } 100% { transform: scale(1) translateX(0px); } }
        @keyframes toBig2 { 0% { transform: scale(1) translateX(0px); } 30% { transform: scale(1) translateX(0px); } 50% { transform: scale(10) translateX(4.5px); } 80% { transform: scale(10) translateX(4.5px); } 100% { transform: scale(1) translateX(0px); } }
        @keyframes breath { 15% { transform: scale(1); } 40% { transform: scale(1.1); } 65% { transform: scale(1); } 90% { transform: scale(1.1); } }
    </style>
</head>
<body>

    <!-- Page loader for Login.aspx -->
    <div id="pageLoader" class="page-loader" role="status" aria-label="Loading">
        <svg viewBox="0 0 100 100" class="loader" aria-hidden="true">
            <g class="points">
              <circle r="50" cy="50" cx="50" class="ciw"></circle>
            <circle r="4" cy="50" cx="5" class="ci2"></circle>
            <circle r="4" cy="50" cx="95" class="ci1"></circle>
            </g>
        </svg>
    </div>

<form id="form1" runat="server">
<div class="login-wrapper">
    <div class="login-card">

        <!-- رأس الصفحة -->
        <div class="login-header">
            <div class="logo-icon">   <img src="IMG/RD.jpg" alt="Logo"  class="logo-icon" />
             </div>
            <h2>مركز رياض الصالحين</h2>
            <p>نظام إدارة المركز الطبي</p>
        </div>

        <!-- نموذج الدخول -->
        <div class="login-body">

            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div class="alert-error">
                    <asp:Label ID="lblError" runat="server" />
                </div>
            </asp:Panel>

            <div class="field-wrapper">
                <label class="form-label">اسم المستخدم</label>
                <span class="input-icon">👤</span>
                <asp:TextBox ID="txtUsername" runat="server"
                    CssClass="form-control"
                    placeholder="أدخل اسم المستخدم" />
            </div>

            <div class="field-wrapper">
                <label class="form-label">كلمة المرور</label>
                <span class="input-icon">🔒</span>
                <asp:TextBox ID="txtPassword" runat="server"
                    TextMode="Password"
                    CssClass="form-control"
                    placeholder="أدخل كلمة المرور" />
            </div>

            <asp:Button ID="btnLogin" runat="server"
                Text="تسجيل الدخول"
                CssClass="btn-login"
                OnClick="btnLogin_Click" />

        </div>

        <asp:Panel ID="Panel1" runat="server" Visible="false">
    <asp:Label ID="Label1" runat="server"></asp:Label>
</asp:Panel>
        <div class="login-footer">
            &copy; 2025 مركز رياض الصالحين الطبي &mdash; جميع الحقوق محفوظة
        </div>

    </div>
</div>
</form>

    <script>
        (function () {
            var loader = document.getElementById('pageLoader');
            if (!loader) return;
            function hideLoader() {
                try {
                    var l = loader;
                    if (!l) return;
                    l.style.opacity = '0';
                    l.style.transition = 'opacity 0.45s ease';
                    setTimeout(function () { if (l) l.style.display = 'none'; }, 500);
                } catch (e) { }
            }
            if (document.readyState === 'complete') hideLoader(); else window.addEventListener('load', hideLoader);
            setTimeout(hideLoader, 8000);
        })();
    </script>

</body>
</html>
