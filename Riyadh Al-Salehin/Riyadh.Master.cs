using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Riyadh_Al_Salehin
{
    public partial class Riyadh : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMenu();
            }
        }

        private void LoadMenu()
        {
            menuItems.Controls.Clear();
            mobileMenuItems.Controls.Clear();

            AddMenu("الرئيسية", "Patients.aspx", "Dashboard");

            AddDropdown("إدارة المرضى", new MenuItem[]
            {
        new MenuItem("المرضى", "Patients.aspx", "Patients"),
        new MenuItem("الحجوزات", "Appointments.aspx", "Appointments"),
        new MenuItem("انتظار الطبيب", "DoctorWaitingPatients.aspx", "DoctorWaitingPatients"),
        new MenuItem("الكشف الطبي", "MedicalExamination.aspx", "MedicalExamination"),

        new MenuItem("مرضى العمليات", "PatientsSurgeries.aspx", "PatientsSurgeries"),
        new MenuItem("اختيار طبيب العملية", "PatientDoctorSurgeries.aspx", "PatientDoctorSurgeries")
            });

            AddDropdown("الفواتير", new MenuItem[]
            {
        new MenuItem(" الاستعلام وتأكيد الحجز ", "UnpaidPatients.aspx", "UnpaidPatients"),
        new MenuItem("كشف حسابات وتقارير ", "AccountsDashboard.aspx", "AccountsDashboard"),
        new MenuItem(" عرض الفواتير ", "InvoicesList.aspx", "InvoicesList"),
        new MenuItem(" عرض اجمالي الحسابات ", "AccountsReport.aspx", "AccountsReport"),
        new MenuItem("الفواتير الإضافية", "AllPatientsInvoices.aspx", "AllPatientsInvoices"),
        new MenuItem("إدارة الخدمات الإضافية والأسعار", "ServicesAndPrices.aspx", "ServicesAndPrices")
            });



            AddDropdown("تقرير الحجوزات", new MenuItem[]
            {
        new MenuItem("عرض الحجزات ", "AppointmentsList.aspx", "AppointmentsList"),
            });







            AddDropdown("إدارة العمليات", new MenuItem[]
            {
        new MenuItem("حجز العمليات", "Surgeries.aspx", "Surgeries"),
        new MenuItem("تكليف العمليات", "SurgeryAccounts.aspx", "SurgeryAccounts"),
        new MenuItem("فواتير العمليات", "SurgeryInvoice.aspx", "SurgeryInvoice"),
        new MenuItem("حسابات العمليات", "SurgeryInvoicesList.aspx", "SurgeryInvoicesList"),
        new MenuItem("مستلزمات العمليات", "MedicalSupplies.aspx", "MedicalSupplies"),
        new MenuItem("غرف العمليات", "OperationRooms.aspx", "OperationRooms")
            });

            AddDropdown("قسم الاشعات", new MenuItem[]
            {
        new MenuItem(" فواتير طلبات الاشعات ", "XrayReception.aspx", "XrayReception"),
        new MenuItem("استقبال طلبات الاشعة", "XrayTechInbox.aspx", "XrayTechInbox"),
         new MenuItem("اضافة خدمات اشعات جديده", "XrayServices.aspx", "XrayServices"),
          new MenuItem("نتائج المعامل", "LabResults.aspx", "LabResults"),
           new MenuItem("نتائج المعامل", "LabResults.aspx", "LabResults")




            });

            AddDropdown("الإدارة", new MenuItem[]
            {




       new MenuItem("إدارة الخدمات الطبية", "MedicalServices.aspx", "MedicalServices"),
        new MenuItem("الأطباء", "Doctors.aspx", "Doctors"),
        new MenuItem("عرض البيانات الفواتير ", "DoctorSummary.aspx", "DoctorSummary"),
        new MenuItem("المراكز الطبية", "MedicalCenters.aspx", "MedicalCenters"),
        new MenuItem("المستخدمون", "Users.aspx", "Users"),
        new MenuItem("الأدوار", "Roles.aspx", "Roles"),
        new MenuItem("الصلاحيات", "PermissionsI.aspx", "PermissionsI"),
        new MenuItem("صلاحيات الأدوار", "RolesPermissions.aspx", "RolesPermissions")
            });
        }

        private void AddMenu(string text, string url, string module)
        {
            if (!CPermissions.Check(module, "View"))
                return;

            HtmlGenericControl li = new HtmlGenericControl("li");

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes["href"] = url;
            a.InnerText = text;

            li.Controls.Add(a);

            menuItems.Controls.Add(li);

            HtmlGenericControl mobileLi = new HtmlGenericControl("li");
            HtmlGenericControl mobileA = new HtmlGenericControl("a");
            mobileA.Attributes["href"] = url;
            mobileA.InnerText = text;
            mobileLi.Controls.Add(mobileA);
            mobileMenuItems.Controls.Add(mobileLi);
        }

        private void AddDropdown(string title, MenuItem[] items)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            li.Attributes["class"] = "dropdown";

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes["href"] = "#";
            a.InnerHtml = "<span>" + title + "</span> <i class='bi bi-chevron-down toggle-dropdown'></i>";

            li.Controls.Add(a);

            HtmlGenericControl ul = new HtmlGenericControl("ul");

            foreach (MenuItem item in items)
            {
                if (!CPermissions.Check(item.Module, "View"))
                    continue;

                HtmlGenericControl childLi = new HtmlGenericControl("li");

                HtmlGenericControl childA = new HtmlGenericControl("a");
                childA.Attributes["href"] = item.Url;
                childA.InnerText = item.Text;

                childLi.Controls.Add(childA);
                ul.Controls.Add(childLi);
            }

            li.Controls.Add(ul);

            menuItems.Controls.Add(li);

            HtmlGenericControl mobileLi = new HtmlGenericControl("li");
            mobileLi.Attributes["class"] = "mobile-menu-section";

            HtmlGenericControl mobileTitle = new HtmlGenericControl("div");
            mobileTitle.Attributes["class"] = "mobile-menu-title";
            mobileTitle.InnerText = title;
            mobileLi.Controls.Add(mobileTitle);

            HtmlGenericControl mobileUl = new HtmlGenericControl("ul");
            foreach (MenuItem item in items)
            {
                if (!CPermissions.Check(item.Module, "View"))
                    continue;

                HtmlGenericControl mobileChildLi = new HtmlGenericControl("li");
                HtmlGenericControl mobileChildA = new HtmlGenericControl("a");
                mobileChildA.Attributes["href"] = item.Url;
                mobileChildA.InnerText = item.Text;
                mobileChildLi.Controls.Add(mobileChildA);
                mobileUl.Controls.Add(mobileChildLi);
            }

            mobileLi.Controls.Add(mobileUl);
            mobileMenuItems.Controls.Add(mobileLi);
        }

        private class MenuItem
        {
            public string Text;
            public string Url;
            public string Module;

            public MenuItem(string text, string url, string module)
            {
                Text = text;
                Url = url;
                Module = module;
            }
        }
    }
}