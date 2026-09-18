using System;
using System.Configuration;
using System.IO;

namespace Riyadh_Al_Salehin
{
    public partial class ImportICD : System.Web.UI.Page
    {
        protected void btnImport_Click(object sender, EventArgs e)
        {
            string file = @"D:\ICD\icd102019syst_codes.txt";

            if (!File.Exists(file))
            {
                lblResult.Text = "الملف غير موجود: " + file;
                return;
            }

            string cs = ConfigurationManager
                .ConnectionStrings["RiyadhAlSalehinDBConnectionString"]
                .ConnectionString;

            ICDImporter imp = new ICDImporter(cs);

            int total = imp.Import(file);

            lblResult.Text = "تم استيراد " + total + " تشخيص بنجاح.";
        }
    }
}