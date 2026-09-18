using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Riyadh_Al_Salehin
{
    public partial class DiagnosisMedicines : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager
            .ConnectionStrings["RiyadhAlSalehinDBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDiagnoses();
                LoadLinkedMedicines();
            }
        }

        private void LoadDiagnoses()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT Id, DiagnosisEnglish FROM Diagnoses WHERE IsActive = 1 ORDER BY DiagnosisEnglish", conn))
            {
                conn.Open();
                ddlDiagnosis.DataSource = cmd.ExecuteReader();
                ddlDiagnosis.DataTextField = "DiagnosisEnglish";
                ddlDiagnosis.DataValueField = "Id";
                ddlDiagnosis.DataBind();
            }
        }

        protected void ddlDiagnosis_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLinkedMedicines();
        }

        private void LoadLinkedMedicines()
        {
            if (ddlDiagnosis.SelectedValue == "") return;

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT dm.Id, m.MedicineName, dm.IsPreferred, dm.Notes
                FROM DiagnosisMedicines dm
                JOIN Medicines m ON m.Id = dm.MedicineId
                WHERE dm.DiagnosisId = @DiagnosisId
                ORDER BY dm.IsPreferred DESC, m.MedicineName", conn))
            {
                cmd.Parameters.AddWithValue("@DiagnosisId", int.Parse(ddlDiagnosis.SelectedValue));
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                gvLinked.DataSource = dt;
                gvLinked.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnMedicineId.Value))
            {
                lblMsg.Text = "من فضلك اختر دواء من نتائج البحث.";
                return;
            }

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM DiagnosisMedicines WHERE DiagnosisId=@DiagnosisId AND MedicineId=@MedicineId)
                INSERT INTO DiagnosisMedicines (DiagnosisId, MedicineId, IsPreferred, Notes)
                VALUES (@DiagnosisId, @MedicineId, @IsPreferred, @Notes)", conn))
            {
                cmd.Parameters.AddWithValue("@DiagnosisId", int.Parse(ddlDiagnosis.SelectedValue));
                cmd.Parameters.AddWithValue("@MedicineId", int.Parse(hdnMedicineId.Value));
                cmd.Parameters.AddWithValue("@IsPreferred", chkPreferred.Checked);
                cmd.Parameters.AddWithValue("@Notes",
                    string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMedicineSearch.Text = "";
            hdnMedicineId.Value = "";
            txtNotes.Text = "";
            lblMsg.Text = "تم الربط بنجاح.";
            LoadLinkedMedicines();
        }

        protected void gvLinked_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteLink")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("DELETE FROM DiagnosisMedicines WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadLinkedMedicines();
            }
        }

        [WebMethod]
        public static List<MedicineResult> SearchMedicines(string term)
        {
            var results = new List<MedicineResult>();
            string cs = ConfigurationManager.ConnectionStrings["RiyadhAlSalehinDBConnectionString"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 15 Id, MedicineName 
                FROM Medicines 
                WHERE MedicineName LIKE @Term AND IsActive = 1
                ORDER BY MedicineName", conn))
            {
                cmd.Parameters.AddWithValue("@Term", "%" + term + "%");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new MedicineResult
                        {
                            Id = (int)reader["Id"],
                            MedicineName = reader["MedicineName"].ToString()
                        });
                    }
                }
            }
            return results;
        }

        public class MedicineResult
        {
            public int Id { get; set; }
            public string MedicineName { get; set; }
        }
    }
}