using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FSP_PlayBack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            splitContainer1.SplitterDistance = groupBox1.Size.Width + 10;
        }

        private bool checkTimeRange()
        {
            DateTime dtStart = Convert.ToDateTime(datePickerFrom.Text + " " + timePickerFrom.Text);
            DateTime dtEnd = Convert.ToDateTime(datePickerTo.Text + " " + timePickerTo.Text);

            TimeSpan ts = dtStart - DateTime.Now;
            int startDay = ts.Days;
            if (startDay < 0)
            {
                return true;
            }
            else
            {
                //Current day, limit to two hour interval
                ts = dtEnd - dtStart;
                if (ts.Hours > 2)
                {
                    MessageBox.Show("You can only access two hours' worth of data at a time for the current day", "Aborting");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void BtnLoadBeatsAndTrucks_Click(object sender, EventArgs e)
        {
            try
            {
                bool timeOK = checkTimeRange();

                if (timeOK)
                {
                    btnLoadBeatsAndTrucks.BackColor = System.Drawing.Color.Red;
                    btnLoadBeatsAndTrucks.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    LoadTrucks();
                    LoadBeats();
                    Cursor = Cursors.Arrow;
                    btnLoadBeatsAndTrucks.Enabled = true;
                    btnLoadBeatsAndTrucks.BackColor = System.Drawing.Color.LightGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void LoadBeats()
        {
            DateTime dtStart = new DateTime();
            DateTime dtEnd = new DateTime();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                dtStart = Convert.ToDateTime(datePickerFrom.Text + " " + timePickerFrom.Text);
                dtEnd = Convert.ToDateTime(datePickerTo.Text + " " + timePickerTo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            cboBeats.Items.Clear();

            try
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("http://localhost:50138/AJAXFSPService.svc/");
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await Client.GetAsync("GetPlayBackBeats");

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic myObject = new ExpandoObject();
                        var Json = await response.Content.ReadAsStringAsync();
                        beats beats = JsonConvert.DeserializeObject<beats>(Json);
                        List<beat> beaties = JsonConvert.DeserializeObject<List<beat>>(beats.d);

                        foreach (beat b in beaties)
                        {
                            cboBeats.Items.Add(b.BeatName);
                        }
                        cboBeats.Text = "Select";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        
        private async void LoadTrucks()
        {
            DateTime dtStart = new DateTime();
            DateTime dtEnd = new DateTime();
            try
            {
                dtStart = Convert.ToDateTime(datePickerFrom.Text + " " + timePickerFrom.Text);
                dtEnd = Convert.ToDateTime(datePickerTo.Text + " " + timePickerTo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            cboTrucks.Items.Clear();

            try
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("http://localhost:50138/AJAXFSPService.svc/");
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    string call = "GetTrucks?dtStart=" + dtStart + "&dtEnd=" + dtEnd;
                    HttpResponseMessage response = await Client.GetAsync(call);

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic myObject = new ExpandoObject();
                        var Json = await response.Content.ReadAsStringAsync();
                        trucks trucks = JsonConvert.DeserializeObject<trucks>(Json);

                        foreach (string tNum in trucks.d)
                        {
                            cboTrucks.Items.Add(tNum);
                        }
                        cboTrucks.Text = "Select";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadPlayback()
        {
            try
            {
                gvData.DataSource = null;
                gvData.Rows.Clear();
                bool LogonOnly = false;

                if (chkLogon.Checked == true)
                {
                    LogonOnly = true;
                }

                DateTime dtStart = new DateTime();
                DateTime dtEnd = new DateTime();

                try
                {
                    dtStart = Convert.ToDateTime(datePickerFrom.Text + " " + timePickerFrom.Text);
                    dtEnd = Convert.ToDateTime(datePickerTo.Text + " " + timePickerTo.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                SQLCode mySQL = new SQLCode();
                mySQL.LoadTrackingData(cboTrucks.Text, dtStart, dtEnd, LogonOnly);
                gvData.DataSource = GlobalData.allTrack;
                GPSTrack firstTrack = GlobalData.allTrack[0];

                gMapControl1.Overlays.Clear();
                gMapControl1.Position = new PointLatLng(firstTrack.Lat, firstTrack.Lon);

                GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                GMap.NET.WindowsForms.GMapMarker marker =
                    new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new GMap.NET.PointLatLng(firstTrack.Lat, firstTrack.Lon),
                        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);
                markers.Markers.Add(marker);
                gMapControl1.Overlays.Add(markers);

                //GMapOverlay overlay = new GMapOverlay(gMapControl1, "base");
                //GMapMarkerGoogleGreen home = new GMapMarkerGoogleGreen(new PointLatLng(firstTrack.Lat, firstTrack.Lon));
                //overlay.Markers.Add(home);
                //gMapControl1.Overlays.Add(overlay);
                iEventCount = GlobalData.allTrack.Count - 1;
                foreach (DataGridViewRow row in gvData.Rows)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void btnLoadPlayback_Click(object sender, EventArgs e)
        {
            bool timeCheck = checkTimeRange();

            if (!timeCheck)
            {
                return;
            }

            if (cboTrucks.Text != "Select")
            {
                btnLoadPlayback.BackColor = System.Drawing.Color.Red;
                btnLoadPlayback.Enabled = false;
                Cursor = Cursors.WaitCursor;
                //LoadPlayback();
                Cursor = Cursors.Arrow;
                btnLoadPlayback.Enabled = true;
                btnLoadPlayback.BackColor = System.Drawing.Color.LightGreen;
                //btnStartPlayback.Enabled = true;
                //btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
            }

            if (cboTrucks.Text == "Select" && cboBeats.Text != "Select")
            {
                btnLoadPlayback.BackColor = System.Drawing.Color.Red;
                btnLoadPlayback.Enabled = false;
                Cursor = Cursors.WaitCursor;
                //LoadPlaybackByBeat();
                Cursor = Cursors.Arrow;
                btnLoadPlayback.Enabled = true;
                btnLoadPlayback.BackColor = System.Drawing.Color.LightGreen;
                //btnStartPlayback.Enabled = true;
                //btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
            }

            if (cboTrucks.Text == "Select" && cboBeats.Text == "Select")
            {
                MessageBox.Show("You must select either a truck or a beat to load data", "Aborting");
                return;
            }
        }

    }

    class beat
    {
        public Guid BeatID { get; set; }
        public string BeatName { get; set; }
    }

    class beats
    {
        public string d { get; set; }
    }

    class trucks
    {
        public List<string> d { get; set; }
    }
}
