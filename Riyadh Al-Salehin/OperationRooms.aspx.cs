using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Riyadh_Al_Salehin
{
    public partial class OperationRooms : System.Web.UI.Page
    {
        private COperationRooms room = new COperationRooms();

        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["UserId"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;

                Response.Redirect("~/Login.aspx");
                return;
            }



            if (!IsPostBack)
            {
                LoadData();
            }
        }

        void LoadData()
        {
            gvRooms.DataSource = room.GetAll();
            gvRooms.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                room.RoomName = txtRoomName.Text;
                room.Type = txtType.Text;

                decimal price = 0;
                decimal.TryParse(txtDailyPrice.Text, out price);
                room.DailyPrice = price;

                room.IsAvailable = chkAvailable.Checked;

                if (string.IsNullOrEmpty(hfId.Value))
                {
                    room.Insert();
                }
                else
                {
                    room.Id = Convert.ToInt32(hfId.Value);
                    room.Update();
                }

                Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('خطأ: " + ex.Message + "')</script>");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            hfId.Value = "";
            txtRoomName.Text = "";
            txtType.Text = "";
            txtDailyPrice.Text = "";
            chkAvailable.Checked = false;
        }

        protected void gvRooms_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                DataTable dt = room.GetById(id);

                if (dt.Rows.Count > 0)
                {
                    hfId.Value = dt.Rows[0]["Id"].ToString();
                    txtRoomName.Text = dt.Rows[0]["RoomName"].ToString();
                    txtType.Text = dt.Rows[0]["Type"].ToString();
                    txtDailyPrice.Text = dt.Rows[0]["DailyPrice"].ToString();
                    chkAvailable.Checked = Convert.ToBoolean(dt.Rows[0]["IsAvailable"]);
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                room.Delete(id);
                LoadData();
            }
        }
    }
}