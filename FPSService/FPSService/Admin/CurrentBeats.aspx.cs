using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPSService.Admin
{
    public partial class CurrentBeats : System.Web.UI.Page
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
            LoadBeats();
            LoadBeatSegments();
        }

        private void LoadBeats()
        {
            List<BeatData.Beat> theseBeats = BeatData.Beats.AllBeats;
            gvBeats.DataSource = theseBeats;
            gvBeats.DataBind();
        }

        private void LoadBeatSegments()
        {
            List<BeatData.BeatSegment> theseBeatSegments = BeatData.Beats.AllBeatSegments;
            gvBeatSegments.DataSource = theseBeatSegments;
            gvBeatSegments.DataBind();
        }

        protected void btnReloadBeats_Click(object sender, EventArgs e)
        {
            try
            {
                BeatData.Beats.LoadBeats();
                BeatData.Beats.LoadBeatSegments();
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('All beat data reloaded')</script>");
                LoadBeats();
                LoadBeatSegments();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('" + ex.Message + "')</script>");
            }
        }
    }
}