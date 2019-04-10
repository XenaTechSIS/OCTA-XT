using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace FSPPlayback
{
    public partial class Form1 : Form
    {
        int iEventCount = 0;
        int iNum = 0;
        public Form1()
        {
            InitializeComponent();
            btnLoadTrucks.BackColor = System.Drawing.Color.LightGreen;
            btnLoadPlayback.BackColor = System.Drawing.Color.LightGreen;
            btnStartPlayback.BackColor = System.Drawing.Color.Red;
            btnStopPlayback.BackColor = System.Drawing.Color.Red;
            btnStartPlayback.Enabled = false;
            btnStopPlayback.Enabled = false;
            gvData.SelectionChanged += new EventHandler(gvData_SelectionChanged);
            InitMap();
        }

        void gvData_SelectionChanged(object sender, EventArgs e)
        {
            var gv = (DataGridView)sender;

            foreach (DataGridViewRow row in gv.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells[0].Value);
                GPSTrack currentTrack = GlobalData.allTrack[id];
                //gMapControl1.Position = new PointLatLng(currentTrack.Lat, currentTrack.Lon);
                gMapControl1.Overlays.Clear();
                gMapControl1.Position = new PointLatLng(currentTrack.Lat, currentTrack.Lon);
                GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                GMap.NET.WindowsForms.GMapMarker marker =
                    new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new GMap.NET.PointLatLng(currentTrack.Lat, currentTrack.Lon),
                        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);
                markers.Markers.Add(marker);
                gMapControl1.Overlays.Add(markers);
                //GMapOverlay overlay = new GMapOverlay(gMapControl1, "base");
                //GMapMarkerGoogleGreen home = new GMapMarkerGoogleGreen(new PointLatLng(currentTrack.Lat, currentTrack.Lon));
                //overlay.Markers.Add(home);
                //gMapControl1.Overlays.Add(overlay);
            }
        }

        private void InitMap()
        {
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new PointLatLng(33.779576, -117.867921);
            gMapControl1.Zoom = 13;
            gMapControl1.ShowCenter = false;
            GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
            GMap.NET.WindowsForms.GMapMarker marker =
                new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                    new GMap.NET.PointLatLng(33.779576, -117.867921),
                    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);

            //GMapOverlay overlay = new GMapOverlay(gMapControl1, "base");
            //GMapMarkerGoogleGreen home = new GMapMarkerGoogleGreen(new PointLatLng(35.0844, -106.6506));
            //overlay.Markers.Add(home);
            //gMapControl1.Overlays.Add(overlay);
        }

        private void LoadBeats()
        {
            DateTime dtStart = new DateTime();
            DateTime dtEnd = new DateTime();
            try
            {
                dtStart = Convert.ToDateTime(dtpStart.Text + " " + txtStartHour.Text);
                dtEnd = Convert.ToDateTime(dtpEnd.Text + " " + txtEndHour.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SQLCode mySQL = new SQLCode();
            cboBeats.Items.Clear();
            try
            {
                List<string> beats = mySQL.GetBeats(dtStart, dtEnd);
                foreach (string s in beats)
                {
                    cboBeats.Items.Add(s);
                }
                cboBeats.Text = "Select";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void LoadTrucks()
        {
            DateTime dtStart = new DateTime();
            DateTime dtEnd = new DateTime();
            try
            {
                dtStart = Convert.ToDateTime(dtpStart.Text + " " + txtStartHour.Text);
                dtEnd = Convert.ToDateTime(dtpEnd.Text + " " + txtEndHour.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SQLCode mySQL = new SQLCode();
            cboTrucks.Items.Clear();
            try
            {
                List<string> trucks = mySQL.GetTrucks(dtStart, dtEnd);
                foreach (string s in trucks)
                {
                    cboTrucks.Items.Add(s);
                }
                cboTrucks.Text = "Select";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void PlaybackTickTock()
        {
            GPSTrack currentTrack = GlobalData.allTrack[iNum];
            gMapControl1.Position = new PointLatLng(currentTrack.Lat, currentTrack.Lon);
            if (chkLeaveTrail.Checked == false)
            {
                gMapControl1.Overlays.Clear();
            }
            GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
            GMap.NET.WindowsForms.GMapMarker marker =
                new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                    new GMap.NET.PointLatLng(currentTrack.Lat, currentTrack.Lon),
                    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);

            //GMapOverlay overlay = new GMapOverlay(gMapControl1, "base");
            //GMapMarkerGoogleGreen home = new GMapMarkerGoogleGreen(new PointLatLng(currentTrack.Lat, currentTrack.Lon));
            //overlay.Markers.Add(home);
            //gMapControl1.Overlays.Add(overlay);

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
                    dtStart = Convert.ToDateTime(dtpStart.Text + " " + txtStartHour.Text);
                    dtEnd = Convert.ToDateTime(dtpEnd.Text + " " + txtEndHour.Text);
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

        private void LoadPlaybackByBeat()
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
                    dtStart = Convert.ToDateTime(dtpStart.Text + " " + txtStartHour.Text);
                    dtEnd = Convert.ToDateTime(dtpEnd.Text + " " + txtEndHour.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                SQLCode mySQL = new SQLCode();
                mySQL.LoadTrackingDataByBeat(cboBeats.Text, dtStart, dtEnd, LogonOnly);
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

        private void btnLoadTrucks_Click_1(object sender, EventArgs e)
        {
            try {
                bool timeOK = checkTimeRange();
                if (timeOK) {
                    btnLoadTrucks.BackColor = System.Drawing.Color.Red;
                    btnLoadTrucks.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    LoadTrucks();
                    LoadBeats();
                    Cursor = Cursors.Arrow;
                    btnLoadTrucks.Enabled = true;
                    btnLoadTrucks.BackColor = System.Drawing.Color.LightGreen;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }

        private bool checkTimeRange() {
            DateTime dtStart = Convert.ToDateTime(dtpStart.Text + " " + txtStartHour.Text);
            DateTime dtEnd = Convert.ToDateTime(dtpEnd.Text + " " + txtEndHour.Text);

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
                else {
                    return true;
                }
            }
        }

        private void btnLoadPlayback_Click_1(object sender, EventArgs e)
        {
            bool timeCheck = checkTimeRange();
            if (!timeCheck) {
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
                btnStartPlayback.Enabled = true;
                btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
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
                btnStartPlayback.Enabled = true;
                btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
            }
            if (cboTrucks.Text == "Select" && cboBeats.Text == "Select")
            {
                MessageBox.Show("You must select either a truck or a beat to load data", "Aborting");
                return;
            }
        }

        private void btnStartPlayback_Click_1(object sender, EventArgs e)
        {
            int speed = 30000;
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

        private void btnStopPlayback_Click_1(object sender, EventArgs e)
        {
            tmrPlayback.Stop();
            btnStopPlayback.Enabled = false;
            btnStopPlayback.BackColor = System.Drawing.Color.Red;
            btnStartPlayback.Enabled = true;
            btnStartPlayback.BackColor = System.Drawing.Color.LightGreen;
            MessageBox.Show("Stopped");
        }
    }
}
