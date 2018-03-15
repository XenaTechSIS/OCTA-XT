using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPSService.Admin
{
    public partial class Yards : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logon"] == null)
            {
                Response.Redirect("Logon.aspx");
            }
            string logon = Session["Logon"].ToString();
            if (logon != "true")
            {
                Response.Redirect("Logon.aspx");
            }
            LoadYards();
        }

        private void LoadYards()
        {
            List<BeatData.Yard> theseYards = BeatData.YardClass.AllYards;
            gvYards.DataSource = theseYards;
            gvYards.DataBind();
        }

        protected void btnReloadYards_Click(object sender, EventArgs e)
        {
            try
            {
                BeatData.YardClass.LoadYards();
                LoadYards();
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('All yard data reloaded')</script>");
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('" + ex.Message + "')</script>");
            }
        }
    }
}