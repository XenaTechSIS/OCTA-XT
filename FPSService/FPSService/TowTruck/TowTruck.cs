using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Microsoft.SqlServer.Types;
using System.Threading;
using System.IO;
using System.Text;

namespace FPSService.TowTruck
{
    
    public class TowTruck
    {
        public GPS GPSPosition { get; set; }
        public TowTruckExtended Extended { get; set; }
        public LastMessageRec LastMessage { get; set; }
        public TowTruckStatus Status { get; set; }
        public State State { get; set; }
        public TowTruckDriver Driver { get; set; }
        private Logging.EventLogger logger;
        public string Identifier;
        public DateTime LastUpdateSrv { get; set; }
        public delegate void TowTruckChangedDelegate(TowTruck towTruck);
        public string TruckNumber;
        public TowTruckChangedDelegate TowTruckChangedEventHandler;
        public TowTruckChangedDelegate TowTruckGPSUpdateEventHandler;
        public Hashtable IDHash = new Hashtable();
        private MessageQueue ttMsgQueue = new MessageQueue();
        public MessageQueue TTQueue { get { return ttMsgQueue; } }
        public bool Reflect { get; set; }
        private Thread processMessageThread;
        private IPEndPoint ttEndPoint;
        private object lockObject = new object();
        public object TowTruckLock { get { return lockObject; } }
        private UdpClient udpClient;
        private char[] delimitCharacters = { '<', '>' };
        private Queue<string> TokenList = new Queue<string>();
        private Stack<String> tokenStack = new Stack<string>();
        private Stack<AttributeNode> attributeStack = new Stack<AttributeNode>();
        private AttributeNode CurrentAttribute = null;
        private AttributeNode BaseNode = null;
        private String currentValue = "";
        public List<MiscData.Assist> theseRequests;
        public AssignedBeat assignedBeat;
        public string currentBeatSegment = "Not Set";
        public MiscData.BeatSchedule thisSchedule;
        public string earlyRollin = "";
        public string CHPEarlyRollInNumber = "";
        public string beatSegment = "NA";

        public TowTruck(string _ipaddr)
        {
            SqlGeographyBuilder builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.Point);
            builder.BeginFigure(0.0, 0.0);
            builder.EndFigure();
            builder.EndGeography();
            GPSPosition = new GPS();
            GPSPosition.Time = DateTime.Now.ToString();
            GPSPosition.Lat = 0.0;
            GPSPosition.Lon = 0.0;
            GPSPosition.MLat = 0.0;
            GPSPosition.MLon = 0.0;
            Extended = new TowTruckExtended();
            logger = new Logging.EventLogger();
            LastMessage = new LastMessageRec();
            Status = new TowTruckStatus();
            Driver = new TowTruckDriver();
            Driver.LastName = "Not Logged On";
            //Driver.AssignedBeat = new Guid("00000000-0000-0000-0000-000000000000");
            //Driver.BeatScheduleID = new Guid("00000000-0000-0000-0000-000000000000");
            assignedBeat = new AssignedBeat();
            assignedBeat.Loaded = false;
            Identifier = _ipaddr;
            TruckNumber = "NOID";
            Status.VehicleStatus = "Waiting for Driver Login";
            Status.OutOfBoundsAlarm = false;
            Status.OutOfBoundsMessage = "NA";

            Status.SpeedingLocation = builder.ConstructedGeography;
            Status.SpeedingAlarm = false;
            Status.SpeedingValue = 0.0;
            Status.SpeedingTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OutOfBoundsTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OutOfBoundsStartTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OutOfBoundsTimeCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OutOfBoundsTimeExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.StatusStarted = DateTime.Now;
            Status.LogOnAlarm = false;
            Status.LogOnAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOnAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOnAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOnAlarmComments = "NA";
            Status.RollOutAlarm = false;
            Status.RollInAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmComments = "NA";
            Status.OnPatrolAlarm = false;
            Status.OnPatrolAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OnPatrolAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OnPatrolAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.OnPatrolAlarmComments = "NA";
            Status.RollInAlarm = false;
            Status.RollInAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.RollInAlarmComments = "NA";
            Status.LogOffAlarm = false;
            Status.LogOffAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOffAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOffAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.LogOffAlarmComments = "NA";
            Status.GPSIssueAlarm = false;
            Status.GPSIssueAlarmStart = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.GPSIssueAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.GPSIssueAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.GPSIssueAlarmComments = "NA";
            Status.IncidentAlarm = false;
            Status.IncidentAlarmTime = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.IncidentAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.IncidentAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.IncidentAlarmComments = "NA";
            Status.StationaryAlarm = false;
            Status.StationaryAlarmStart = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.StationaryAlarmCleared = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.StationaryAlarmExcused = Convert.ToDateTime("01/01/2001 00:00:00");
            Status.StationaryAlarmComments = "NA";

            processMessageThread = new Thread(new ThreadStart(ParseMessage));
            processMessageThread.Name = "TTProcessMessage";
            processMessageThread.Start();

            if(!_ipaddr.Contains(":"))
            {
                ttEndPoint = new IPEndPoint(IPAddress.Parse(_ipaddr), 9009);
            }
            
            udpClient = new UdpClient();
            State = new State();
            State.CarID = "NOID";
            State.GpsRate = 0;
            State.Log = "F";
            State.ServerIP = "127.0.0.1";
            State.SFTPServerIP = "127.0.0.1";
            theseRequests = new List<MiscData.Assist>();
        }

        private void ParseMessage()
        {
            string message = TTQueue.WaitForMessage();
            if (message.Contains("</Id>"))
            {
                int ID = GetID(message);
                if (!IDHash.ContainsValue(ID))
                {
                    IDHash.Add(ID, ID);
                    if (Reflect) WriteMessage(message);
                    //logger.writeToLogFileFromTT(message) need to implement to track message sending
                    string[] parsed = message.Split(delimitCharacters);
                    foreach (string item in parsed)
                    {
                        if (item.Trim() != "") parseElement(item.Trim());
                    }
                    if (tokenStack.Count > 0)
                    {
                        IDHash.Remove(ID);
                        tokenStack = new Stack<string>();
                    }
                }
                else if (!message.Contains("</Ack>"))
                {
                    string MyAck = "<Ack><Id>" + ID.ToString() + "</Id></Ack>";
                    WriteMessage(MyAck);
                }
            }
        }
        /* assists stand on their own now and, while trucks are assigned to them, are not part of the truck per se.
        public void AddAssistRequest(MiscData.Assist thisRequest)
        {
            //TowTruck.TowTruck foundTruck = currentTrucks.Find(delegate(TowTruck.TowTruck myTruck)
            MiscData.Assist chkAssist = theseRequests.Find(delegate(MiscData.Assist myRequest){
                return myRequest.AssistID == thisRequest.AssistID;
            });
            if (chkAssist != null)
            {
                lock (theseRequests)
                {
                    chkAssist = thisRequest;
                }
            }
            else
            {
                lock (theseRequests)
                {
                    theseRequests.Add(thisRequest);
                }
            }
        }
        
        public List<MiscData.Assist> GetAssistRequests()
        {
            return theseRequests;
        }

        public void UpdateAssist(MiscData.Assist thisRequest)
        {
            MiscData.Assist chkAssist = theseRequests.Find(delegate(MiscData.Assist myRequest)
            {
                return myRequest.AssistID == thisRequest.AssistID;
            });
            if (chkAssist != null)
            {
                lock (theseRequests)
                {
                    chkAssist = thisRequest;
                }
            }
        }

        public void ClearAssistRequest(Guid AssistID)
        {
            MiscData.Assist chkAssist = theseRequests.Find(delegate(MiscData.Assist myRequest) {
                return myRequest.AssistID == AssistID;
            });
            if (chkAssist != null)
            {
                theseRequests.Remove(chkAssist);
            }
        }
        */
        public void parseElement(String token)
        {
            if (token[0] == '/')
            {
                if (tokenStack.Count > 0 && token == "/" + tokenStack.Last())
                {
                    // check if it is a message
                    TowTruckMessage msg = checkIfMessage(tokenStack.Last());
                    if (msg != null)
                    {
                        AddAttributesToMessage(msg, BaseNode);
                        msg.Execute(this);
                        CurrentAttribute = null;
                        BaseNode = null;
                    }
                    tokenStack = new Stack<string>();
                    return;
                }

                bool found = false;
                String element = "";
                while (!found && tokenStack.Count > 1)
                {
                    // find attribute pair
                    if (tokenStack.Count == 0) return; // ignore
                    element = tokenStack.Pop();
                    if (token == "/" + element)
                    {
                        found = true;
                    }
                    else
                    {
                        currentValue = element;
                    }
                }
                createSiblingNode(element, currentValue);
            }
            else
            {
                tokenStack.Push(token);
            }
        }

        private void createSiblingNode(String attribute, String value)
        {
            AttributeNode newNode = new AttributeNode();
            newNode.Attribute = attribute;
            newNode.Value = value;
            if (BaseNode == null)
            {
                BaseNode = newNode;
            }
            else
            {
                CurrentAttribute.NextAttribute = newNode;
            }
            CurrentAttribute = newNode;
            currentValue = "";
        }

        private void AddAttributesToMessage(TowTruckMessage msg, AttributeNode node)
        {
            // currently only support on level deep XML packets
            while (node != null)
            {
                msg.AddAttribute(node.Attribute, node.Value);
                AttributeNode next = node.NextAttribute;
                node = null;
                node = next;
            }
        }

        private TowTruckMessage checkIfMessage(string token)
        {
            switch (token)
            {
                case "GPS":
                    return new GPSMessage();
            }
            return null;
        }

        public void WriteMessage(String message)
        {
            lock (this)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                try
                {
                    //logger.writeToLogFileToLMT(message); need to implement to track message sending
                    udpClient.Send(buffer, buffer.Length, ttEndPoint);
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private int GetID(string msg)
        {
            int fID = msg.IndexOf("<Id>");
            fID += 4;
            int lID = msg.IndexOf("</Id>");
            string ID = msg.Substring(fID, lID - fID);
            return Convert.ToInt32(ID);
        }

        public void UpdateGPS(GPS thisGPS)
        {
            //thisGPS.BeatID = BeatData.Beats.FindBeatID(thisGPS.Position);
            //if (thisGPS.BeatID != new Guid("00000000-0000-0000-0000-000000000000"))
            //{
            //    thisGPS.BeatSegmentID = BeatData.Beats.FindBeatSegmentID(thisGPS.BeatID, thisGPS.Position);
            //}
            this.GPSPosition = thisGPS;
            //Find a way to quickly iterate beats and look for intersections of GPS info.  IF a beat is found, use that data
            //to look for a beat segment.  

            //Not acking anymore.
            //UDP.SendMessage msgAck = new UDP.SendMessage();
            //msgAck.SendMyMessage("<Ack><Id>" + this.GPSPosition.Id.ToString() + "</Id></Ack>", this.Identifier);
        }

        public void UpdateState(State thisState)
        {
            if (!string.IsNullOrEmpty(thisState.CarID))
            { this.State.CarID = thisState.CarID; }
            if (thisState.GpsRate != 0)
            { this.State.GpsRate = thisState.GpsRate; }
            if (thisState.Log != null)
            { this.State.Log = thisState.Log; }
            if (!string.IsNullOrEmpty(thisState.ServerIP))
            { this.State.ServerIP = thisState.ServerIP; }
            if (!string.IsNullOrEmpty(thisState.SFTPServerIP))
            {
                this.State.SFTPServerIP = thisState.SFTPServerIP;
            }
            if (!string.IsNullOrEmpty(thisState.Version))
            {
                this.State.Version = thisState.Version;
            }
            UDP.SendMessage msgAck = new UDP.SendMessage();
            msgAck.SendMyMessage("<Ack><Id>" + thisState.Id.ToString() + "</Id></Ack>", this.Identifier);
        }

        public void SendMessage(string _msg)
        {
            UDP.SendMessage msg = new UDP.SendMessage();
            msg.SendMyMessage(_msg, this.Identifier);
        }

        public void TowTruckChanged()
        {
            if (TowTruckChangedEventHandler != null)
            {
                TowTruckChangedEventHandler(this);
            }
        }

        public void TowTruckGPSUpdate()
        {
            if (TowTruckGPSUpdateEventHandler != null)
            {
                TowTruckGPSUpdateEventHandler(this);
            }

            if (this.Status.VehicleStatus == "Waiting for Driver Login")
            {
                DataClasses.GlobalData.RemoveCover(this.TruckNumber);
            }

            if (this.Status.VehicleStatus == "On Incident") //track time spent on incident, if over x minutes, raise an alarm.
            {
                if (this.Status.StatusStarted.AddMinutes(DataClasses.GlobalData.ExtendedLeeway) < DateTime.Now)
                {
                    SQL.SQLCode mySQL = new SQL.SQLCode();
                    this.Status.IncidentAlarm = true;
                    this.Status.IncidentAlarmTime = DateTime.Now;
                    mySQL.LogAlarm("Incident", DateTime.Now, this.Driver.DriverID, this.Extended.FleetVehicleID, this.assignedBeat.BeatID);
                }
            }

            //this will probably need to be changed to On Patrol
            if (this.GPSPosition.Speed == 0 && this.Status.VehicleStatus.ToUpper() == "ON PATROL")
            {
                if (this.Status.StationaryAlarmStart != DateTime.Parse("01/01/2001 00:00:00"))
                {
                    //we have a start time, no need to update it
                    if (this.Status.StationaryAlarmStart.AddMinutes(DataClasses.GlobalData.StationaryLeeway) < DateTime.Now)
                    {
                        //we have a no motion alert
                        this.Status.StationaryAlarm = true;
                        SQL.SQLCode mySQL = new SQL.SQLCode();
                        mySQL.LogAlarm("Stationary", DateTime.Now, this.Driver.DriverID, this.Extended.FleetVehicleID, this.assignedBeat.BeatID);
                    }
                }
                else
                {
                    this.Status.StationaryAlarmStart = DateTime.Now;
                    //since this is the first instance we've got of 0mph, we probably don't need to raise an alarm
                }
            }

            if (this.GPSPosition.Speed > 0 || this.Status.VehicleStatus.ToUpper() != "ON PATROL")
            {
                this.Status.StationaryAlarm = false;
                this.Status.StationaryAlarmStart = DateTime.Parse("01/01/2001 00:00:00"); //reset the alarm status
            }

            if (this.GPSPosition.Lat == 0.0 && this.GPSPosition.Lon == 0.0)
            {
                if (this.Status.GPSIssueAlarmStart != DateTime.Parse("01/01/2001 00:00:00"))
                {
                    //got a start time, no need to update it
                    if (this.Status.GPSIssueAlarmStart.AddMinutes(DataClasses.GlobalData.GPSIssueLeeway) < DateTime.Now)
                    {
                        this.Status.GPSIssueAlarm = true;
                        SQL.SQLCode mySQL = new SQL.SQLCode();
                        mySQL.LogAlarm("GPS Issue", DateTime.Now, this.Driver.DriverID, this.Extended.FleetVehicleID, this.assignedBeat.BeatID);
                    }
                }
                else
                {
                    this.Status.GPSIssueAlarmStart = DateTime.Now;
                }
            }

            if (this.GPSPosition.Lat > 0.0 && this.GPSPosition.Lon == 0.0)
            {
                this.Status.GPSIssueAlarm = false;
                this.Status.GPSIssueAlarmStart = DateTime.Parse("01/01/2001 00:00:00");
            }

            this.LastMessage.LastMessageReceived = DateTime.Now;

            //Generate the GPS Location for SqlGeography
            SqlGeographyBuilder builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.Point);
            builder.BeginFigure(this.GPSPosition.MLat, this.GPSPosition.MLon);
            builder.EndFigure();
            builder.EndGeography();

            bool HasAlarms = false;
            string loc = "Off Beat"; //truck's current location

            //Check for in yard status when no beat assigned
            if (this.assignedBeat.BeatID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                string YardName = BeatData.YardClass.FindInYard(this.Extended.ContractorName, builder.ConstructedGeography);
                if (YardName != "Not In Yard")
                {
                    loc = YardName;
                }
                else
                {
                    loc = "Off Beat";
                }
            }

            //Check for possible speeding problems
            if (this.GPSPosition.MaxSpd > DataClasses.GlobalData.SpeedingValue || this.GPSPosition.Speed > DataClasses.GlobalData.SpeedingValue)
            {
                this.Status.SpeedingAlarm = true;
                if (this.GPSPosition.MaxSpd > this.Status.SpeedingValue || this.GPSPosition.Speed > this.Status.SpeedingValue)
                {
                    this.Status.SpeedingValue = this.GPSPosition.MaxSpd;
                }
                this.Status.SpeedingTime = DateTime.Now;
                //generate speeding location


                this.Status.SpeedingLocation = builder.ConstructedGeography;
                //Log it to the database
                SQL.SQLCode mySQL = new SQL.SQLCode();
                mySQL.LogSpeeding(this.Driver.DriverID, this.TruckNumber, this.GPSPosition.Speed, this.GPSPosition.MaxSpd,
                    this.Status.SpeedingTime, this.Status.SpeedingLocation);
                HasAlarms = true;
            }
            else
            {
                //no speeding has occurred, but we also need to make sure the geography isn't null, which will cause an error when we try
                //to load into db.  If no val: set lat/lon = 0.0/0.0
                SqlGeographyBuilder nullbuilder = new SqlGeographyBuilder();
                nullbuilder.SetSrid(4326);
                nullbuilder.BeginGeography(OpenGisGeographyType.Point);
                nullbuilder.BeginFigure(0.0, 0.0);
                nullbuilder.EndFigure();
                nullbuilder.EndGeography();

                if (this.Status.SpeedingLocation == builder.ConstructedGeography)
                {
                    this.Status.SpeedingLocation = builder.ConstructedGeography;
                }

                
                this.Status.SpeedingAlarm = false; //No need to update any data.  We want to hold onto the last speeding value
            }

            //check for out-of-bounds problems

            if (this.assignedBeat.BeatID != new Guid("00000000-0000-0000-0000-000000000000") && this.Status.VehicleStatus.ToUpper() == "ON PATROL")
            {
                //SqlGeographyBuilder builder = new SqlGeographyBuilder();
                //builder.SetSrid(4326);
                //builder.BeginGeography(OpenGisGeographyType.Point);
                //builder.BeginFigure(this.GPSPosition.Lat, this.GPSPosition.Lon);
                //builder.EndFigure();
                //builder.EndGeography();
                bool OnBeat = BeatData.Beats.FindOnBeat(this.assignedBeat.BeatID, builder.ConstructedGeography);
                currentBeatSegment = BeatData.Beats.FindCurrentBeatSegment(this.assignedBeat.BeatID, builder.ConstructedGeography);
                if (OnBeat == true)
                {
                    this.Status.OutOfBoundsAlarm = false;
                    this.Status.OutOfBoundsMessage = currentBeatSegment;
                    this.Status.OutOfBoundsStartTime = DateTime.Parse("01/01/2001 00:00:00");
                    this.beatSegment = currentBeatSegment;
                }
                else
                {
                    //check whether the truck is in a yard, get the contractor for the truck and use it for lookup
                    string YardName = BeatData.YardClass.FindInYard(this.Extended.ContractorName, builder.ConstructedGeography);
                    string obMsg = "Off beat";
                    this.beatSegment = "NA";
                    if (YardName != "Not In Yard")
                    {
                        obMsg = YardName;
                    }
                    else
                    {
                        obMsg = "Off beat";
                    }
                    if (this.Status.OutOfBoundsStartTime != DateTime.Parse("01/01/2001 00:00:00"))
                    {
                        //check the leeway time
                        if (this.Status.OutOfBoundsStartTime.AddMinutes(DataClasses.GlobalData.OffBeatLeeway) < DateTime.Now)
                        {
                            //truck is officially out of beat and has been for a while
                            this.Status.OutOfBoundsAlarm = true;
                            this.Status.OutOfBoundsMessage = obMsg;
                            this.Status.OutOfBoundsTime = DateTime.Now;
                            //Log off beat into OffBeatAlerts table only if actually off-beat and not in yard.  Truck must be On Patrol or On Incident to actually
                            //be out of beat and must have been out of beat for longer than the leeway time
                            if (YardName == "Not In Yard" && (this.Status.VehicleStatus.ToUpper() == "ON PATROL" || this.Status.VehicleStatus.ToUpper() == "ON INCIDENT"))
                            {
                                SQL.SQLCode mySQL = new SQL.SQLCode();
                                mySQL.LogOffBeat(this.Driver.DriverID, this.TruckNumber, DateTime.Now, builder.ConstructedGeography);
                                //vehicle is off beat, need to check off beat time
                            }
                        }
                    }
                    else
                    {
                        //truck is off beat, but not for long enough to worry about
                        this.Status.OutOfBoundsStartTime = DateTime.Now;
                        this.Status.OutOfBoundsAlarm = false;
                        //go ahead and set the out of bounds message anyway
                        this.Status.OutOfBoundsMessage = obMsg;
                    }


                }
            }
            else //truck has not been assigned a beat in the database or is not on patrol
            {
                this.Status.OutOfBoundsMessage = loc;
                this.Status.OutOfBoundsAlarm = false;
            }
            
        }
    }
}