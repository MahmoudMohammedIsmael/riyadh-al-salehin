# Riyadh Al-Salehin Medical Center Management System
## نظام إدارة مركز رياض الصالحين الطبي

![ASP.NET](https://img.shields.io/badge/ASP.NET-WebForms%204.8.1-blue?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-red?logo=microsoftsqlserver)
![C#](https://img.shields.io/badge/C%23-.NET%204.8.1-purple?logo=csharp)
![License](https://img.shields.io/badge/License-Proprietary-orange)
![Language](https://img.shields.io/badge/Language-Arabic%20%7C%20English-green)

---

## 📋 Project Overview | نظرة عامة على المشروع

**Riyadh Al-Salehin** is a comprehensive, full-featured **Medical Center Management System** built with **ASP.NET WebForms (.NET 4.8.1)** and **SQL Server**. The system is designed to digitize and streamline all operations of a multi-specialty medical center, handling everything from patient registration to surgical invoicing, doctor scheduling, laboratory requests, X-ray management, and WhatsApp-integrated appointment notifications.

**رياض الصالحين** هو نظام شامل لإدارة المركز الطبي مبني على تقنية **ASP.NET WebForms** مع قاعدة بيانات **SQL Server**. يُغطي النظام جميع عمليات المركز الطبي من تسجيل المرضى حتى فوترة العمليات الجراحية وإدارة الجداول الطبية والمختبرات والأشعة مع تكامل إشعارات **WhatsApp**.

---

## 🏥 Core Features | الميزات الرئيسية

### 👤 Patient Management | إدارة المرضى
- Complete patient registration with demographic data (name, phone, address, date of birth)
- Insurance company and insurance number tracking
- Patient medical history and examination records
- Patient-specific surgical procedures and invoices

### 👨‍⚕️ Doctor Management | إدارة الأطباء
- Doctor profiles with specialties and contact details
- Weekly and daily schedule management
- Doctor commission rules configuration
- Doctor waiting queue (real-time patient queue per doctor)
- Doctor summary reports (revenue, patient count)

### 📅 Appointments | الحجوزات والمواعيد
- Online and in-center appointment booking
- Appointment list management with status tracking
- Patient-doctor schedule view
- Doctor waiting room patient management

### 🔬 Medical Examinations | الكشوفات الطبية
- Full examination workflow: diagnosis, medicines, procedures
- ICD (International Classification of Diseases) integration with import capability
- Diagnosis-linked medicines prescriptions
- Performed examination procedures tracking

### 💊 Medical Services & Supplies | الخدمات والمستلزمات
- Medical services catalog with pricing
- Medical supplies inventory management
- Surgery-specific supplies tracking
- Patient supplies management

### 🧾 Invoicing & Billing | الفوترة والحسابات
- **Consultation Invoices** — post-examination billing
- **Surgery Invoices** — surgical procedure billing with itemized costs
- **Invoice Items** — detailed line-item billing per service
- **All Patient Invoices** — aggregate billing view
- **Unpaid Patients** — outstanding balance tracking
- Invoice printing (consultation, surgery, X-ray)
- Accounts dashboard with financial summaries
- Accounts reports

### 🏨 Surgery Management | إدارة العمليات
- Full surgical procedure records
- Operation rooms management
- Surgery accounts and financial tracking
- Surgery invoices and print-ready reports
- Surgery supplies and patient surgeries tracking

### 🩺 Laboratory | المختبر
- Lab request creation linked to patient/examination
- Lab results entry and tracking
- Lab request-to-result workflow

### ☢️ X-Ray (Radiology) | الأشعة
- X-ray service catalog
- X-ray reception and request management
- X-ray result entry by radiology technicians
- Technician inbox for pending requests
- X-ray report viewer
- X-ray invoice printing

### 📱 WhatsApp Integration | تكامل واتساب
- WhatsApp Business API integration (Meta Graph API v25.0)
- Automated appointment notification messages
- AI-powered WhatsApp chatbot using **Google Gemini AI**
- Webhook support for incoming WhatsApp messages

### 🔐 Users, Roles & Permissions | المستخدمون والأدوار والصلاحيات
- Multi-user authentication system with session management
- Role-based access control (RBAC)
- Fine-grained permission system (View, Create, Edit, Delete per module)
- Operation logs for audit trail
- Last login tracking

### 📊 Reports & Dashboard | التقارير ولوحة التحكم
- Accounts dashboard with financial KPIs
- Accounts reports
- Doctor summary reports
- Operation logs report

---

## 🏗️ Architecture | البنية التقنية

```
Riyadh Al-Salehin/
├── *.aspx                  # WebForms pages (UI layer)
├── *.aspx.cs               # Code-behind files (business logic)
├── Controllers/
│   └── WhatsAppController.cs   # WebAPI controller for WhatsApp webhook
├── Services/
│   ├── WhatsAppService.cs      # WhatsApp Business API service
│   └── WhatsAppAIService.cs    # Gemini AI integration service
├── App_Start/
│   └── WebApiConfig.cs         # WebAPI routing configuration
├── Models/                     # Data models
├── Content/                    # CSS stylesheets
├── Scripts/                    # JavaScript files
├── assets/                     # Static assets (images, fonts)
├── IMG/                        # Uploaded images
├── Uploads/                    # File uploads directory
├── Web.config                  # Application configuration (see SETUP.md)
└── packages.config             # NuGet dependencies
```

### Technology Stack | التقنيات المستخدمة

| Component | Technology |
|-----------|-----------|
| Backend Framework | ASP.NET WebForms 4.8.1 |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM / Data Access | ADO.NET (DataTable, SqlClient) |
| UI Framework | Bootstrap + jQuery + AjaxControlToolkit |
| API Layer | ASP.NET WebAPI 2 |
| AI Integration | Google Gemini API (gemini-1.5-pro) |
| Messaging | WhatsApp Business API (Meta Graph API v25.0) |
| Authentication | ASP.NET Session-based |
| Localization | Arabic (ar-SA) with Gregorian calendar |

---

## ⚙️ Setup & Configuration | الإعداد والتكوين

### Prerequisites | المتطلبات
- **Windows Server** or Windows 10/11
- **IIS** (Internet Information Services) with ASP.NET 4.8 enabled
- **Microsoft SQL Server** 2019 or later
- **.NET Framework 4.8.1**
- **Visual Studio 2022** (for development)

### 1. Database Setup | إعداد قاعدة البيانات
1. Open **SQL Server Management Studio (SSMS)**
2. Create a new database named `RiyadhAlSalehinDB`
3. Run the database migration/schema scripts (contact the maintainer for the SQL schema)

### 2. Application Configuration | إعداد التطبيق
Copy `Web.config` and update the following settings:

```xml
<!-- Database Connection -->
<connectionStrings>
  <add name="RiyadhConnection"
       connectionString="Data Source=YOUR_SQL_SERVER;Initial Catalog=RiyadhAlSalehinDB;
                         Integrated Security=True;Encrypt=False;TrustServerCertificate=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>

<!-- External API Keys -->
<appSettings>
  <add key="WhatsAppAccessToken"    value="YOUR_WHATSAPP_BUSINESS_ACCESS_TOKEN" />
  <add key="WhatsAppPhoneNumberId"  value="YOUR_PHONE_NUMBER_ID" />
  <add key="WhatsAppGraphVersion"   value="v25.0" />
  <add key="GeminiApiKey"           value="YOUR_GOOGLE_GEMINI_API_KEY" />
  <add key="GeminiModel"            value="gemini-1.5-pro" />
</appSettings>
```

### 3. IIS Deployment | النشر على IIS
1. Publish the project to a local folder via Visual Studio
2. Create a new IIS site pointing to the published folder
3. Set the Application Pool to **.NET CLR v4.0**, **Integrated Pipeline**
4. Ensure the app pool identity has **read/write** access to the `Uploads/` and `IMG/` folders

---

## 🔒 Security Notes | ملاحظات الأمان

> **⚠️ IMPORTANT — Sensitive Configuration**

The following values **must never** be committed to source control. Always use environment variables or a secrets manager in production:

| Setting | Description |
|---------|-------------|
| `WhatsAppAccessToken` | WhatsApp Business API Bearer Token (Meta) |
| `WhatsAppPhoneNumberId` | WhatsApp registered phone number ID |
| `GeminiApiKey` | Google Gemini AI API Key |
| `connectionString` | SQL Server credentials / server name |

**Best Practices Applied:**
- Session cookies are `HttpOnly` with `SameSite=Lax`
- Server version header is disabled (`enableVersionHeader="false"`)
- All user inputs are validated server-side before database operations
- Role-based permission checks on every page load and every action
- Session cleared on logout and on login page load

---

## 📦 NuGet Dependencies | المكتبات المستخدمة

| Package | Purpose |
|---------|---------|
| `AjaxControlToolkit` | Rich AJAX UI controls (date pickers, modals) |
| `Newtonsoft.Json` | JSON serialization for API responses |
| `Microsoft.AspNet.WebApi` | REST API for WhatsApp webhook |
| `Microsoft.AspNet.Mvc` | MVC scaffolding support |
| `System.Net.Http` | HTTP client for external API calls |
| `ClosedXML` / `EPPlus` | Excel export (if applicable) |

---

## 📸 Screenshots | لقطات الشاشة

> Screenshots will be added in future releases.

---

## 🤝 Contributing | المساهمة

This project is proprietary. For bug reports or feature requests, please contact the maintainer directly.

---

## 📄 License | الرخصة

© 2024–2026 Riyadh Al-Salehin Medical Center. All rights reserved.  
This software is proprietary and not licensed for redistribution or modification without explicit written permission.

---

## 👨‍💻 Maintainer | المطوّر

**Mahmoud Mohammed Ismael**  
GitHub: [@MahmoudMohammedIsmael](https://github.com/MahmoudMohammedIsmael)
