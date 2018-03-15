using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace FPSService.Admin
{
    public partial class Dashboard : System.Web.UI.Page
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
            if (!Page.IsPostBack)
            {
                LoadTrucks();
            }
        }

        private void LoadTrucks()
        {
            ddlFTTs.Items.Clear();

            List<TowTruck.TowTruck> theseTrucks = DataClasses.GlobalData.currentTrucks;
            theseTrucks = theseTrucks.OrderBy(t => t.TruckNumber).ToList();

            foreach (TowTruck.TowTruck thisTruck in theseTrucks)
            {
                ListItem thisItem = new ListItem();
                string TruckID;
                if (string.IsNullOrEmpty(thisTruck.TruckNumber))
                {
                    TruckID = "NOID";
                }
                else
                {
                    TruckID = thisTruck.TruckNumber;
                }
                thisItem.Text = TruckID;
                thisItem.Value = thisTruck.Identifier;
                ddlFTTs.Items.Add(thisItem);
            }
            //ListItem li = new ListItem();
            //li.Text = "1234";
            //li.Value = "127.0.0.2";
            //ddlFTTs.Items.Add(li);
            //li.Text = "1235";
            //li.Value = "127.0.0.3";
            //ddlFTTs.Items.Add(li);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ConstructMessage();
        }

        private void ConstructMessage()
        {
            string msg = "<" + ddlMessages.Text + ">";
            msg += "<Id>" + MakeMsgID() + "</Id>";
            if (ddlMessages.Text == "GetVar" || ddlMessages.Text == "SetVar")
            {
                string[] splitter = txtMessageVal.Text.Split('|');
                string VarName = splitter[0].ToString();
                string VarValue = splitter[1].ToString();
                msg += "<" + VarName + ">" + VarValue + "</" + VarName + ">";
            }
            msg += "</" + ddlMessages.Text + ">";
            //ListItem li = ddlFTTs.SelectedItem;
            //string TruckNumber = li.Text;
            //string IP = li.Value;
            TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(ddlFTTs.SelectedValue.ToString());
            if (thisTruck != null)
            {
                thisTruck.SendMessage(msg);
            }
        }

        private string MakeMsgID()
        {
            DateTime dtSeventy = Convert.ToDateTime("01/01/1970 00:00:00");
            TimeSpan tsSpan = DateTime.Now - dtSeventy;
            double ID = tsSpan.TotalMilliseconds;
            Int64 id = Convert.ToInt64(ID);
            return id.ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            LoadTrucks();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string SelectedTruck = ddlFTTs.SelectedValue.ToString();
            if (string.IsNullOrEmpty(SelectedTruck))
            { return; }
            DataClasses.GlobalData.RemoveTowTruck(SelectedTruck);
            LoadTrucks();
        }

        protected void btnSetVar_Click(object sender, EventArgs e)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            mySQL.SetVarValue(txtVarName.Text, txtVarValue.Text);
            if (txtVarName.Text == "Speeding")
            {
                DataClasses.GlobalData.SpeedingValue = Convert.ToInt32(txtVarValue.Text);
            }
            if (txtVarName.Text.Contains("Leeway"))
            {
                mySQL.LoadLeeways();
            }
        }

        protected void btnReloadBeats_Click(object sender, EventArgs e)
        {
            try
            {
                BeatData.Beats.LoadBeats();
                BeatData.Beats.LoadBeatSegments();
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('All beat data reloaded')</script>");
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "<script>alert('" + ex.Message + "')</script>");
            }
        }

    }
}