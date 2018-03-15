using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Web.Script.Serialization;
using Microsoft.SqlServer.Types;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Timers;

namespace OctaHarness
{
    public partial class Form1 : Form
    {
        string IPAddr = "";
        ReceiveUDP myUDP = new ReceiveUDP();
        Thread runKMLThread;
        Thread runCSVThread;
        private bool alarmCheckStarted = false;
        private System.Windows.Forms.Timer tmrAlarms;
        
        public Form1()
        {
            InitializeComponent();
            txtTime.Text = DateTime.Now.ToString();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPAddr = ip.ToString();
                }
            }
            txtIPAddress.Text = IPAddr;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            runKMLThread = new Thread(new ThreadStart(RunKMLThread));
            runKMLThread.IsBackground = true;
            runCSVThread = new Thread(new ThreadStart(RunCSVThread));
            runCSVThread.IsBackground = true;
            
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myUDP.stopThread();
            myUDP = null;
        }

        public void setText(string Msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setText), new object[] { Msg });
                return;
            }
            txtMessageStatus.Text += Environment.NewLine + Msg + Environment.NewLine;
        }

        private void btnSendPacket_Click_1(object sender, EventArgs e)
        {
            txtTime.Text = DateTime.Now.ToString();
            GPS thisGPS = new GPS();
            string StartDate = "01/01/1970 00:00:00";
            TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
            thisGPS.Id = Math.Abs(nowSpan.Milliseconds);
            thisGPS.Speed = Convert.ToDouble(txtSpeed.Text);
            thisGPS.Lat = Convert.ToDouble(txtLat.Text);
            thisGPS.Lon = Convert.ToDouble(txtLon.Text);
            thisGPS.MaxSpd = Convert.ToDouble(txtMaxSpeed.Text);
            thisGPS.MLat = Convert.ToDouble(txtMLat.Text);
            thisGPS.MLon = Convert.ToDouble(txtMLon.Text);
            thisGPS.Time = Convert.ToDateTime(txtTime.Text);
            thisGPS.Status = "Valid";
            thisGPS.DOP = 7;
            thisGPS.Alt = 5280;
            thisGPS.Head = Convert.ToInt32(txtHead.Text);
            XmlSerializer ser = new XmlSerializer(typeof(GPS));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringWriter str = new StringWriter();
            ser.Serialize(str, thisGPS, namespaces);
            string msg = str.ToString();
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg,txtServiceAddress.Text);
        }

        private void btnSendAbePacket_Click(object sender, EventArgs e)
        {
            string msg = "<GPS><Id>67546</Id><MLon>-118.109129734817</MLon><Speed>0</Speed><Head>0</Head><Lon>-118.109129734817</Lon><Alt>1307</Alt><Time>09/06/2012 18:45:46</Time><MLat>33.7978439719052</MLat><Status>Valid</Status><MaxSpd>0</MaxSpd><Lat>33.7978439719052</Lat><DOP>6</DOP></GPS>";
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg, txtServiceAddress.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string msg = "<GPS><Id>67546</Id><MLon>-118.109129734817</MLon><Speed>0</Speed><Head>0</Head><Lon>-118.109129734817</Lon><Alt>1307</Alt><Time>09/06/2012 18:45:46</Time><MLat>33.7978439719052</MLat><Status>Valid</Status><MaxSpd>0</MaxSpd><Lat>33.7978439719052</Lat><DOP>6</DOP></GPS>";
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg,txtServiceAddress.Text);
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void btnSendState_Click(object sender, EventArgs e)
        {
            State thisState = new State();
            string StartDate = "01/01/1970 00:00:00";
            TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
            thisState.Id = Math.Abs(nowSpan.Milliseconds);
            thisState.CarID = "NA";
            thisState.GpsRate = 30;
            thisState.Log = "F";
            thisState.ServerIP = "127.10.0.1";
            thisState.Version = "1.0.0";
            thisState.SFTPServerIP = "127.10.0.1";
            XmlSerializer ser = new XmlSerializer(typeof(State));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringWriter str = new StringWriter();
            ser.Serialize(str, thisState, namespaces);
            //ser.Serialize(str, thisState);
            string msg = str.ToString();
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg, txtServiceAddress.Text);

        }

        private void btnSendAssistRequest_Click(object sender, EventArgs e)
        {
            AssistRequest myAssist = new AssistRequest();
            myAssist.Show();
        }

        private void btnParseKML_Click(object sender, EventArgs e)
        {
            ParseKML myParser = new ParseKML();
            myParser.OpenKMLFile();
            if (runKMLThread.ThreadState != ThreadState.Running && runKMLThread.ThreadState != ThreadState.Suspended
                && runKMLThread.ThreadState != ThreadState.Stopped && runKMLThread.ThreadState != ThreadState.Aborted)
            {
                runKMLThread.Start();
            }
            else if (runKMLThread.ThreadState == ThreadState.Stopped || runKMLThread.ThreadState == ThreadState.Aborted)
            {
                runKMLThread = null;
                runKMLThread = new Thread(new ThreadStart(RunKMLThread));
                runKMLThread.IsBackground = true;
                runKMLThread.Start();
            }
            else
            {
                runKMLThread.Abort();
                runKMLThread = null;
                runKMLThread = new Thread(new ThreadStart(RunKMLThread));
                runKMLThread.Start();
            }

        }

        public void RunKMLThread()
        {
            for (int i = 0; i < RunKML.Coords.Count(); i++)
            {

                GPS thisGPS = new GPS();
                string StartDate = "01/01/1970 00:00:00";
                TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
                thisGPS.Id = Math.Abs(nowSpan.Milliseconds);
                thisGPS.Speed = Convert.ToDouble(txtSpeed.Text);
                thisGPS.Lat = RunKML.Coords[i].lat;
                thisGPS.Lon = RunKML.Coords[i].lon;
                thisGPS.MaxSpd = Convert.ToDouble(txtMaxSpeed.Text);
                thisGPS.MLat = Convert.ToDouble(txtMLat.Text);
                thisGPS.MLon = Convert.ToDouble(txtMLon.Text);
                thisGPS.Time = Convert.ToDateTime(txtTime.Text);
                thisGPS.Status = "Valid";
                thisGPS.DOP = 7;
                thisGPS.Alt = 5280;
                thisGPS.Head = Convert.ToInt32(txtHead.Text);
                XmlSerializer ser = new XmlSerializer(typeof(GPS));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                StringWriter str = new StringWriter();
                ser.Serialize(str, thisGPS, namespaces);
                string msg = str.ToString();
                SendUDP myUDP = new SendUDP();
                SendUDP playbackUdp = new SendUDP();
                txtMessageStatus.Invoke(new MethodInvoker(delegate {
                    txtMessageStatus.Text += playbackUdp.SendUDPPacket(msg, txtServiceAddress.Text);
                }));

                if (i == RunKML.Coords.Count() - 1)
                {
                    txtMessageStatus.Invoke(new MethodInvoker(delegate {
                        txtMessageStatus.Text = "Playback finished " + DateTime.Now.ToString();
                    }));
                    runKMLThread.Abort();
                    break;
                }

                double distBetween = 0.0;  //should be miles
                if ((i + 1) < RunKML.Coords.Count())
                {
                    distBetween = GeoCodeCalc.CalcDistance(RunKML.Coords[i].lat, RunKML.Coords[i].lon, RunKML.Coords[i + 1].lat, RunKML.Coords[i + 1].lon);
                }
                int Pause = Convert.ToInt32(thisGPS.Speed * distBetween);
                System.Threading.Thread.Sleep(Pause * 1000);
            }

        }

        private void btnStopPlay_Click(object sender, EventArgs e)
        {
            runKMLThread.Abort();
            MessageBox.Show("Playback aborted");
        }

        private void btnClearStatus_Click(object sender, EventArgs e)
        {
            txtMessageStatus.Text = "";
        }

        private void btnPlayCSV_Click(object sender, EventArgs e)
        {
            ParseCSV myParser = new ParseCSV();
            myParser.ProcessData();
            if (runCSVThread.ThreadState != ThreadState.Running && runCSVThread.ThreadState != ThreadState.Suspended
                && runCSVThread.ThreadState != ThreadState.Stopped && runCSVThread.ThreadState != ThreadState.Aborted)
            {
                runCSVThread.Start();
            }
            else if (runCSVThread.ThreadState == ThreadState.Stopped || runCSVThread.ThreadState == ThreadState.Aborted)
            {
                runCSVThread = null;
                runCSVThread = new Thread(new ThreadStart(RunCSVThread));
                runCSVThread.IsBackground = true;
                runCSVThread.Start();
            }
            else
            {
                runCSVThread.Abort();
                runCSVThread = null;
                runCSVThread = new Thread(new ThreadStart(RunCSVThread));
                runCSVThread.Start();
            }
        }

        private void RunCSVThread()
        {
            for (int i = 0; i < RunCSV.playbackTrucks.Count; i++)
            {
                GPS thisGPS = new GPS();
                string StartDate = "01/01/1970 00:00:00";
                TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
                thisGPS.Id = Math.Abs(nowSpan.Milliseconds);
                thisGPS.Speed = RunCSV.playbackTrucks[i].Speed;
                thisGPS.Lat = RunCSV.playbackTrucks[i].Lat;
                thisGPS.Lon = RunCSV.playbackTrucks[i].Lon;
                thisGPS.MaxSpd = RunCSV.playbackTrucks[i].MaxSpeed;
                thisGPS.MLat = RunCSV.playbackTrucks[i].Lat;
                thisGPS.MLon = RunCSV.playbackTrucks[i].Lon;
                thisGPS.Time = RunCSV.playbackTrucks[i].timeStamp;
                thisGPS.Status = RunCSV.playbackTrucks[i].VehicleStatus;
                thisGPS.DOP = 7;
                thisGPS.Alt = 5280;
                thisGPS.Head = RunCSV.playbackTrucks[i].Direction;
                XmlSerializer ser = new XmlSerializer(typeof(GPS));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                StringWriter str = new StringWriter();
                ser.Serialize(str, thisGPS, namespaces);
                string msg = str.ToString();
                SendUDP myUDP = new SendUDP();
                SendUDP playbackUdp = new SendUDP();

                txtMessageStatus.Invoke(new MethodInvoker(delegate
                {
                    txtMessageStatus.Text += playbackUdp.SendUDPPacket(msg, txtServiceAddress.Text);
                }));

                if (i == RunCSV.playbackTrucks.Count() - 1)
                {
                    txtMessageStatus.Invoke(new MethodInvoker(delegate
                    {
                        txtMessageStatus.Text = "Playback finished " + DateTime.Now.ToString() + Environment.NewLine;
                    }));
                    runCSVThread.Abort();
                    break;
                }

                double distBetween = 0.0;  //should be miles
                if ((i + 1) < RunCSV.playbackTrucks.Count())
                {
                    distBetween = GeoCodeCalc.CalcDistance(RunCSV.playbackTrucks[i].Lat, RunCSV.playbackTrucks[i].Lon, RunCSV.playbackTrucks[i + 1].Lat, RunCSV.playbackTrucks[i + 1].Lon);
                }
                int Pause = Convert.ToInt32(thisGPS.Speed * distBetween);
                System.Threading.Thread.Sleep(Pause * 1000);
            }
            txtMessageStatus.Invoke(new MethodInvoker(delegate
                {
                    txtMessageStatus.Text = "Restarting.  Press Stop CSV to stop playback" + Environment.NewLine;
                    runCSVThread.Abort();
                    runCSVThread.Start();
                }));
        }

        private void btnStopCSV_Click(object sender, EventArgs e)
        {
            runCSVThread.Abort();
        }

        private void btnNewIncident_Click(object sender, EventArgs e)
        {
            ServiceRef.ServiceAddress = txtServiceRef.Text;
            Incidents frmIncidents = new Incidents();
            frmIncidents.Show();
        }

        private void btnNewAssist_Click(object sender, EventArgs e)
        {
            ServiceRef.ServiceAddress = txtServiceRef.Text;
            Assists frmAssists = new Assists();
            frmAssists.Show();
        }

        private void btnCurrentTrucks_Click(object sender, EventArgs e)
        {
            ServiceRef.ServiceAddress = txtServiceRef.Text;
            TruckList myTruckList = new TruckList();
            myTruckList.Show();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            ServiceRef.ServiceAddress = txtServiceRef.Text;
            SendMessage myMessage = new SendMessage();
            myMessage.Show();
        }

        private void btnAlarmCheck_Click(object sender, EventArgs e)
        {

            if (alarmCheckStarted == false)
            {
                if (tmrAlarms == null)
                {
                    tmrAlarms = new System.Windows.Forms.Timer();
                }
                alarmCheckStarted = true;
                //start the timer
                tmrAlarms.Tick += new EventHandler(tmrAlarms_Tick);
                tmrAlarms.Interval = 30 * 1000; //30 seconds
                tmrAlarms.Start();
                MessageBox.Show("Timer Started, click again to stop");
                RunAlarmCheck();
            }
            else
            {
                alarmCheckStarted = false;
                tmrAlarms.Stop();
                if (tmrAlarms != null)
                {
                    tmrAlarms = null;
                }
                MessageBox.Show("Timer Stopped, click again to restart");
            }
        }

        private void RunAlarmCheck()
        {

            SendTimerGPS();
            SendTimerState();
        }

        private void SendTimerGPS()
        {
            txtTime.Text = DateTime.Now.ToString();
            GPS thisGPS = new GPS();
            string StartDate = "01/01/1970 00:00:00";
            TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
            thisGPS.Id = Math.Abs(nowSpan.Milliseconds);
            thisGPS.Speed = Convert.ToDouble(txtSpeed.Text);
            thisGPS.Lat = Convert.ToDouble(txtLat.Text);
            thisGPS.Lon = Convert.ToDouble(txtLon.Text);
            thisGPS.MaxSpd = Convert.ToDouble(txtMaxSpeed.Text);
            thisGPS.MLat = Convert.ToDouble(txtMLat.Text);
            thisGPS.MLon = Convert.ToDouble(txtMLon.Text);
            thisGPS.Time = Convert.ToDateTime(txtTime.Text);
            thisGPS.Status = "Valid";
            thisGPS.DOP = 7;
            thisGPS.Alt = 5280;
            thisGPS.Head = Convert.ToInt32(txtHead.Text);
            XmlSerializer ser = new XmlSerializer(typeof(GPS));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringWriter str = new StringWriter();
            ser.Serialize(str, thisGPS, namespaces);
            string msg = str.ToString();
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg, txtServiceAddress.Text);
        }

        private void SendTimerState()
        {
            State thisState = new State();
            string StartDate = "01/01/1970 00:00:00";
            TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
            thisState.Id = Math.Abs(nowSpan.Milliseconds);
            thisState.CarID = "NA";
            thisState.GpsRate = 30;
            thisState.Log = "F";
            thisState.ServerIP = "127.10.0.1";
            thisState.Version = "1.0.0";
            thisState.SFTPServerIP = "127.10.0.1";
            XmlSerializer ser = new XmlSerializer(typeof(State));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringWriter str = new StringWriter();
            ser.Serialize(str, thisState, namespaces);
            //ser.Serialize(str, thisState);
            string msg = str.ToString();
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket(msg, txtServiceAddress.Text);
        }

        void tmrAlarms_Tick(object sender, EventArgs e)
        {
            RunAlarmCheck();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtTime.Text = DateTime.Now.ToString();
            GPS thisGPS = new GPS();
            string StartDate = "01/01/1970 00:00:00";
            TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
            thisGPS.Id = Math.Abs(nowSpan.Milliseconds);
            thisGPS.Speed = Convert.ToDouble(txtSpeed.Text);
            thisGPS.Lat = Convert.ToDouble(txtLat.Text);
            thisGPS.Lon = Convert.ToDouble(txtLon.Text);
            thisGPS.MaxSpd = Convert.ToDouble(txtMaxSpeed.Text);
            thisGPS.MLat = Convert.ToDouble(txtMLat.Text);
            thisGPS.MLon = Convert.ToDouble(txtMLon.Text);
            thisGPS.Time = Convert.ToDateTime(txtTime.Text);
            thisGPS.Status = "Valid";
            thisGPS.DOP = 7;
            thisGPS.Alt = 5280;
            thisGPS.Head = Convert.ToInt32(txtHead.Text);
            XmlSerializer ser = new XmlSerializer(typeof(GPS));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringWriter str = new StringWriter();
            ser.Serialize(str, thisGPS, namespaces);
            string msg = str.ToString();
            SendUDP myUDP = new SendUDP();
            txtMessageStatus.Text += myUDP.SendUDPPacket("<FWD>" + txtIPAddress.Text + "</FWD>" + msg, txtServiceAddress.Text);
        }
    }

    #region " old code "
    //old code
    //SqlGeographyBuilder builder = new SqlGeographyBuilder();
    //SqlGeography geography;
    //builder.SetSrid(4326);
    //builder.BeginGeography(OpenGisGeographyType.Point);
    //builder.BeginFigure(Convert.ToDouble(txtLat.Text), Convert.ToDouble(txtLon.Text),0,0);
    //builder.EndFigure();
    //builder.EndGeography();

    //geography = builder.ConstructedGeography;
    //string StartDate = "01/01/1970 00:00:00";
    //TimeSpan nowSpan = DateTime.Now - Convert.ToDateTime(StartDate);
    //int msgID = Math.Abs(nowSpan.Milliseconds);
    //txtLastUpdate.Text = DateTime.Now.ToString();
    //TowTruck thisTruck = new TowTruck();
    //thisTruck.MessageID = msgID;
    //thisTruck.IPAddress = txtIPAddress.Text;
    //thisTruck.VehicleNumber = txtVehicleNumber.Text;
    //thisTruck.VehicleStatus = txtVehicleStatus.Text;
    //thisTruck.Direction = txtDirection.Text;
    //thisTruck.LastUpdate = Convert.ToDateTime(txtLastUpdate.Text);
    //thisTruck.BeatSegmetID = txtBeatSegmentID.Text;
    //thisTruck.Speed = Convert.ToInt32(txtSpeed.Text);
    //if (chkAlarms.Checked == true)
    //{
    //    thisTruck.Alarms = true;
    //}
    //else
    //{
    //    thisTruck.Alarms = false;
    //}
    //thisTruck.Lat = Convert.ToDouble(txtLat.Text);
    //thisTruck.Lon = Convert.ToDouble(txtLon.Text);
    //thisTruck.DriverID = new Guid(txtDriverID.Text);
    //thisTruck.BeatID = new Guid();
    ////thisTruck.Position = geography;
    //JavaScriptSerializer js = new JavaScriptSerializer();
    //string msg = js.Serialize(thisTruck);
    //msg = "GPS|" + msg;

    #endregion
}
