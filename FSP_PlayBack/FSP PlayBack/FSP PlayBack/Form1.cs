using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;

namespace FSP_PlayBack
{
    public partial class Form1 : Form
    {
        public Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        public string url = "";
        int iEventCount = 0;
        int iNum = 0;
        GPSTracks gpstraxx;
        public Timer tmrPlayback = new Timer();

        public Form1()
        {
            InitializeComponent();
            splitContainer1.SplitterDistance = groupBox1.Size.Width + 30;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer2.SplitterDistance = splitContainer1.Size.Height/2 + 100;
            url = config.AppSettings.Settings["url"].Value;
            gvData.SelectionChanged += new EventHandler(gvData_SelectionChanged);
            tmrPlayback.Tick += new EventHandler(tmrPlayback_Tick);
            InitMap();
        }

        private void InitMap()
        {
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMapControl1.Position = new GMap.NET.PointLatLng(33.6846, -117.8265);
            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 13;
            gMapControl1.CanDragMap = true;

            GMapOverlay markers = new GMapOverlay(gMapControl1, "markers");
            PointLatLng mrkr = new PointLatLng(33.6846, -117.8265);
            GMapMarker marker = new GMapMarkerGoogleGreen(mrkr);
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);
        }

        void gvData_SelectionChanged(object sender, EventArgs e)
        {
            var gv = (DataGridView)sender;

            foreach (DataGridViewRow row in gv.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells[0].Value);
                GPSTrack currentTrack = gpstraxx.d[id];

                gMapControl1.Overlays.Clear();
                GMapOverlay markers = new GMapOverlay(gMapControl1, "markers");
                GMapMarker marker = new GMapMarkerGoogleRed(new PointLatLng(gpstraxx.d[id].Lat, gpstraxx.d[id].Lon));
                markers.Markers.Add(marker);
                gMapControl1.Overlays.Add(markers);
            }
        }

        private bool checkTimeRange()
        {
            DateTime dtStart = Convert.ToDateTime(datePickerFrom.Text + " " + timePickerFrom.Text);
            DateTime dtEnd = Convert.ToDateTime(datePickerTo.Text + " " + timePickerTo.Text);

            TimeSpan ts = dtStart - DateTime.Now;
            int startDay = ts.Days;
            if (startDay < 0)
            {
                //Current day, limit to two hour interval
                ts = dtEnd - dtStart;
                if (ts.Days > 1)
                {
                    MessageBox.Show("You can only access 24 hours' worth of data at a time.", "Aborting");
                    return false;
                }
                else
                {
                    return true;
                }
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
                    Client.BaseAddress = new Uri(url);
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
                        cboBeats.Sorted = true;
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
                    Client.BaseAddress = new Uri(url);
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

        private async void LoadPlayback()
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
                }
                
                try
                {
                    using (var Client = new HttpClient())
                    {
                        Client.BaseAddress = new Uri(url);
                        Client.DefaultRequestHeaders.Accept.Clear();
                        Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        string call = "GetTrackingData?dtStart=" + dtStart + "&dtEnd=" + dtEnd + "&VehicleID=" + cboTrucks.Text + "&OnlyLoggedOn=" + LogonOnly;
                        HttpResponseMessage response = await Client.GetAsync(call);

                        if (response.IsSuccessStatusCode)
                        {
                            dynamic myObject = new ExpandoObject();
                            var Json = await response.Content.ReadAsStringAsync();
                            gpstraxx = JsonConvert.DeserializeObject<GPSTracks>(Json);
                            gvData.DataSource = gpstraxx.d;
                            foreach (DataGridViewRow row in gvData.Rows)
                            {
                                row.DefaultCellStyle.BackColor = Color.White;
                            }
                            gMapControl1.Overlays.Clear();
                            gMapControl1.Position = new PointLatLng(gpstraxx.d[0].Lat, gpstraxx.d[0].Lon);

                            GMapOverlay markers = new GMapOverlay(gMapControl1, "markers");
                            GMapMarker marker = new GMapMarkerGoogleRed(new GMap.NET.PointLatLng(gpstraxx.d[0].Lat, gpstraxx.d[0].Lon));
                            markers.Markers.Add(marker);
                            gMapControl1.Overlays.Add(markers);

                            //GMapOverlay overlay = new GMapOverlay(gMapControl1, "base");
                            //GMapMarkerGoogleGreen home = new GMapMarkerGoogleGreen(new PointLatLng(firstTrack.Lat, firstTrack.Lon));
                            //overlay.Markers.Add(home);
                            //gMapControl1.Overlays.Add(overlay);
                            iEventCount = gpstraxx.d.Length - 1;
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void LoadPlaybackByBeat()
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
                }

                try
                {
                    using (var Client = new HttpClient())
                    {
                        Client.BaseAddress = new Uri(url);
                        Client.DefaultRequestHeaders.Accept.Clear();
                        Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        string call = "GetTrackingDataByBeat?dtStart=" + dtStart + "&dtEnd=" + dtEnd + "&BeatNumber=" + cboBeats.Text + "&OnlyLoggedOn=" + LogonOnly;
                        HttpResponseMessage response = await Client.GetAsync(call);

                        if (response.IsSuccessStatusCode)
                        {
                            dynamic myObject = new ExpandoObject();
                            var Json = await response.Content.ReadAsStringAsync();
                            gpstraxx = JsonConvert.DeserializeObject<GPSTracks>(Json);
                            gvData.DataSource = gpstraxx.d;
                            foreach (DataGridViewRow row in gvData.Rows)
                            {
                                row.DefaultCellStyle.BackColor = Color.White;
                            }
                            gMapControl1.Overlays.Clear();
                            gMapControl1.Position = new PointLatLng(gpstraxx.d[0].Lat, gpstraxx.d[0].Lon);

                            GMapOverlay markers = new GMapOverlay(gMapControl1, "markers");
                            GMapMarker marker = new GMapMarkerGoogleRed(new PointLatLng(gpstraxx.d[0].Lat, gpstraxx.d[0].Lon));
                            markers.Markers.Add(marker);
                            gMapControl1.Overlays.Add(markers);

                            iEventCount = gpstraxx.d.Length - 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
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
                LoadPlayback();
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
                LoadPlaybackByBeat();
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

        private void BtnStartPlayback_Click(object sender, EventArgs e)
        {
            int speed = 10000;
            if (gvData.Rows.Count > 1)
            {
                //find out if we have a selected row
                if (gvData.SelectedRows.Count > 0)
                {
                    //use the first selected row to set the start point
                    foreach (DataGridViewRow row in gvData.SelectedRows)
                    {
                        iNum = Convert.ToInt32(row.Cells[0].Value);
                    }
                }
                string pbSpeed = cboPlaybackSpeed.Text;
                if (string.IsNullOrEmpty(pbSpeed))
                {
                    tmrPlayback.Interval = speed;
                }
                else
                {
                    switch (pbSpeed)
                    {
                        case "1x":
                            speed = speed / 1;
                            break;
                        case "2x":
                            speed = speed / 2;
                            break;
                        case "3x":
                            speed = speed / 3;
                            break;
                        case "4x":
                            speed = speed / 4;
                            break;
                        case "5X":
                            speed = speed / 5;
                            break;
                        case "10x":
                            speed = speed / 10;
                            break;
                        case "20x":
                            speed = speed / 20;
                            break;
                        case "50x":
                            speed = speed / 50;
                            break;
                        case "LightSpeed":
                            speed = speed / 10000;
                            break;
                    }
                    tmrPlayback.Interval = speed;
                }
                btnStartPlayback.BackColor = System.Drawing.Color.Red;
                btnStartPlayback.Enabled = false;
                btnStopPlayback.Enabled = true;
                btnStopPlayback.BackColor = System.Drawing.Color.LightGreen;
                tmrPlayback.Start();
                MessageBox.Show("Started");
            }
            else
            {
                MessageBox.Show("No data loaded for play");
            }
        }

        private void tmrPlayback_Tick(object sender, EventArgs e)
        {
            if (iNum <= iEventCount)
            {
                PlaybackTickTock();
                iNum += 1;
            }
            else
            {
                tmrPlayback.Stop();
                MessageBox.Show("Finished playback");
            }
        }

        private void PlaybackTickTock()
        {
            GPSTrack currentTrack = gpstraxx.d[iNum];
            gMapControl1.Position = new PointLatLng(gpstraxx.d[iNum].Lat, gpstraxx.d[iNum].Lon);
            if (chkLeaveTrail.Checked == false)
            {
                gMapControl1.Overlays.Clear();
            }

            GMapOverlay markers = new GMapOverlay(gMapControl1, "markers");
            GMapMarker marker = new GMapMarkerGoogleRed(new PointLatLng(gpstraxx.d[iNum].Lat, gpstraxx.d[iNum].Lon));
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);

            foreach (DataGridViewRow row in gvData.Rows)
            {
                if (Convert.ToInt32(row.Cells[0].Value) == iNum)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
                else if (Convert.ToInt32(row.Cells[0].Value) < iNum)
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                }
            }
        }

        private void BtnStopPlayback_Click(object sender, EventArgs e)
        {
            tmrPlayback.Stop();
            btnStopPlayback.Enabled = false;
            btnStopPlayback.BackColor = System.Drawing.Color.Red;
            btnStartPlayback.Enabled = true;
            btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
            MessageBox.Show("Stopped");
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

    class GPSTracks
    {
        public GPSTrack[] d { get; set; }
    }
}
