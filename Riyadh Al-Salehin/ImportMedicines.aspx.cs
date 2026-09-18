using System;
using System.Configuration;
using System.IO;

namespace Riyadh_Al_Salehin
{
    public partial class ImportMedicines : System.Web.UI.Page
    {
        protected void btnImport_Click(object sender, EventArgs e)
        {
            string file = @"D:\ICD\RXNCONSO.RRF";

            if (!File.Exists(file))
            {
                lblResult.Text = "الملف غير موجود: " + file;
                lblResult.Text = "الملف غير موجود: " + file;
                return;
            }

            string cs = ConfigurationManager
                .ConnectionStrings["RiyadhAlSalehinDBConnectionString"]
                .ConnectionString;

            MedicineImporter imp = new MedicineImporter(cs);

            int total = imp.Import(file);

            lblResult.Text = "تم استيراد " + total + " دواء بنجاح.";
        }
    }
}