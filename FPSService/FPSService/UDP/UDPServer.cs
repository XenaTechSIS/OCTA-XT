﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Web.Script;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Microsoft.SqlServer.Types;
using System.IO;
using System.Configuration;

namespace FPSService.UDP
{
    public class UDPServer
    {
        private Logging.EventLogger logger; //error logging
        private Thread listenThread; //each truck gets its own thread
        private byte[] data = new byte[4096]; //incoming packet data
        private List<string> OtherServers = new List<string>();
        private bool forward = false;
        private SendMessage udpSend = new SendMessage();

        public UDPServer()
        {
            string others = ConfigurationManager.AppSettings["OtherServers"].ToString();
            string[] listOthers = others.Split('|');
            for (int i = 0; i < listOthers.Count(); i++)
            {
                OtherServers.Add(listOthers[i].ToString());
            }
            string fwd = ConfigurationManager.AppSettings["forward"].ToString();
            if (fwd.ToUpper() == "TRUE")
            {
                forward = true;
            }
            listenThread = new Thread(new ThreadStart(UDPListenThread));
            listenThread.Start();
        }

        private void UDPListenThread()
        {
            logger = new Logging.EventLogger(); //for the initial dev phase we're going to log a lot, disable this before
            //rolling out to production
            Socket udpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int ListenPort = Convert.ToInt32(ConfigurationManager.AppSettings["ListenPort"]);
            IPEndPoint ourEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);
            IPEndPoint end = new IPEndPoint(IPAddress.Any, ListenPort);
            EndPoint Identifier = (EndPoint)end;

            udpListener.Bind(ourEndPoint);
            while (true)
            {
                string message = null; ;
                try
                {
                    bool fwdThisMessage = true;
                    int length = udpListener.ReceiveFrom(data, ref Identifier);
                    message = System.Text.Encoding.UTF8.GetString(data, 0, length);
                    string _ipaddr = null;

                    if (message.Contains(">RPV"))
                    {
                        SQL.SQLCode mySQL = new SQL.SQLCode();
                        var TAIPID = message.Substring(38, 4);
                        _ipaddr = mySQL.GetMACAddress(TAIPID);
                    }
                    else
                    {
                        _ipaddr = ((IPEndPoint)Identifier).Address.ToString();
                    }

                    if (message.Contains("<FWD>"))
                    {
                        //it's a forwarded message, remove the <FWD> tags and find the specified IPAddress
                        //then treat it like a normal packet
                        message = message.Replace("<FWD>", "");
                        int firstLT = message.IndexOf('<');
                        _ipaddr = message.Substring(0, firstLT);
                        message = message.Replace(_ipaddr + "</FWD>", "");
                        fwdThisMessage = false;
                        //done, let it move on
                    }
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    TowTruck.TowTruck thisTruck;
                    thisTruck = DataClasses.GlobalData.FindTowTruck(_ipaddr);
                    string type = "";
                    try
                    {
                        if (message.Contains("<GPS>"))
                        {
                            type = "GPS";
                            if (forward == true && fwdThisMessage == true)
                            {
                                string fwdMsg = "<FWD>" + _ipaddr + "</FWD>" + message;
                                foreach (string s in OtherServers)
                                {
                                    udpSend.ForwardMessage(fwdMsg, s);
                                }
                            }
                        }
                        if (message.Contains(">RPV"))
                        {
                            type = "TAIP";
                            if (forward == true && fwdThisMessage == true)
                            {
                                string fwdMsg = "<FWD>" + _ipaddr + "</FWD>" + message;
                                foreach (string s in OtherServers)
                                {
                                    udpSend.ForwardMessage(fwdMsg, s);
                                }
                            }
                        }
                    }
                    catch
                    { }
                    try
                    {
                        if (message.Contains("<State>"))
                        {
                            type = "State";
                            if (forward == true && fwdThisMessage == true)
                            {
                                string fwdMsg = "<FWD>" + _ipaddr + "</FWD>" + message;
                                foreach (string s in OtherServers)
                                {
                                    udpSend.ForwardMessage(fwdMsg, s);
                                }
                            }
                        }
                    }
                    catch
                    { }
                    /*
                    string[] splitMsg = message.Split('|');
                    string type = splitMsg[0].ToString();
                    string msg = splitMsg[1].ToString();
                     * */
                    TowTruck.GPS thisGPS = null;
                    TowTruck.State thisState = null;
                    if (type == "GPS")
                    {
                        //thisGPS = js.Deserialize<TowTruck.GPS>(msg);
                        XmlSerializer ser = new XmlSerializer(typeof(TowTruck.GPS));
                        StringReader rdr = new StringReader(message);
                        thisGPS = (TowTruck.GPS)ser.Deserialize(rdr);
                        SqlGeographyBuilder builder = new SqlGeographyBuilder();
                        builder.SetSrid(4326);
                        builder.BeginGeography(OpenGisGeographyType.Point);
                        builder.BeginFigure(thisGPS.Lat, thisGPS.Lon);
                        builder.EndFigure();
                        builder.EndGeography();
                        thisGPS.Position = builder.ConstructedGeography;
                    }
                    if (type == "TAIP")
                    {
                        Random RandomID = new Random();
                        string GPSDT = getGPSTimeOfDay(Convert.ToInt32(message.Substring(4, 5)));
                        string LAT = message.Substring(9, 8);
                        string LON = message.Substring(17, 9);
                        string SPEED = message.Substring(26, 3);
                        string DIR = message.Substring(29, 3);
                        string SOURCE = message.Substring(32, 1);
                        string AGE = message.Substring(33, 1);
                        string ID = message.Substring(38, 4);
                        string CHKSUM = message.Substring(33, 3);
                        string LAT1 = LAT.Substring(0, 3) + ".";
                        string LAT2 = LAT1 + ((LAT.Length > 4) ? LAT.Substring(LAT.Length - 5, 5) : LAT);
                        string LON1 = LON.Substring(0, 4) + ".";
                        string LON2 = LON1 + ((LON.Length > 4) ? LON.Substring(LON.Length - 5, 5) : LON);

                        //build GPS
                        thisGPS = new TowTruck.GPS();
                        thisGPS.Id = RandomID.Next(0, 9999);
                        thisGPS.Speed = Convert.ToDouble(SPEED);
                        thisGPS.Lat = Convert.ToDouble(LAT2);
                        thisGPS.Lon = Convert.ToDouble(LON2);
                        thisGPS.MaxSpd = Convert.ToDouble(SPEED);
                        thisGPS.MLat = Convert.ToDouble(LAT2);
                        thisGPS.MLon = Convert.ToDouble(LON2);
                        thisGPS.Time = GPSDT;
                        thisGPS.Status = "Valid";
                        thisGPS.DOP = 6;
                        thisGPS.Alt = 0;
                        thisGPS.Head = Convert.ToInt32(DIR);

                        SqlGeographyBuilder builder = new SqlGeographyBuilder();
                        builder.SetSrid(4326);
                        builder.BeginGeography(OpenGisGeographyType.Point);
                        builder.BeginFigure(thisGPS.Lat, thisGPS.Lon);
                        builder.EndFigure();
                        builder.EndGeography();
                        thisGPS.Position = builder.ConstructedGeography;
                    }
                    else if (type == "State")
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(TowTruck.State));
                        //testing, this was causing an error on parse, but couldn't find a reason why
                        message = message.Replace(" xmlns=''", "");
                        StringReader rdr = new StringReader(message);
                        thisState = (TowTruck.State)ser.Deserialize(rdr);
                    }
                    if (thisTruck != null)
                    {
                        try
                        {
                            thisTruck.LastMessage.LastMessageReceived = DateTime.Now;
                            DataClasses.GlobalData.UpdateTowTruck(_ipaddr, thisTruck);
                            //DataClasses.GlobalData.AddTowTruck(thisTruck);
                            if (string.IsNullOrEmpty(thisTruck.Extended.TruckNumber))
                            {
                                SQL.SQLCode mySQL = new SQL.SQLCode();
                                TowTruck.TowTruckExtended thisExtended = mySQL.GetExtendedData(thisTruck.Identifier);
                                thisTruck.Extended = thisExtended;
                                thisTruck.TruckNumber = thisExtended.TruckNumber;
                            }
                            /*if (thisTruck.assignedBeat.Loaded == false)  //
                            {
                                SQL.SQLCode mySQL = new SQL.SQLCode();
                                TowTruck.AssignedBeat thisAssignedBeat = mySQL.GetAssignedBeat(thisTruck.Extended.FleetVehicleID);
                                if (thisAssignedBeat != null)
                                {
                                    thisTruck.assignedBeat = thisAssignedBeat;
                                }
                            }*/
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            thisTruck = new TowTruck.TowTruck(_ipaddr);
                            DataClasses.GlobalData.AddTowTruck(thisTruck);
                            SQL.SQLCode mySQL = new SQL.SQLCode();
                            TowTruck.TowTruckExtended thisExtended = mySQL.GetExtendedData(thisTruck.Identifier);
                            thisTruck.Extended = thisExtended;
                            thisTruck.TruckNumber = thisExtended.TruckNumber;
                            thisTruck.Status.StatusStarted = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            logger.LogEvent(DateTime.Now.ToString() + " Error in generating truck" + Environment.NewLine + ex.ToString() +
                            Environment.NewLine + "Original Message:" + Environment.NewLine + message, true);
                        }
                    }
                    if (type == "GPS")
                    {
                        thisTruck.UpdateGPS(thisGPS);
                        thisTruck.TowTruckGPSUpdate();
                    }
                    if (type == "TAIP")
                    {
                        thisTruck.UpdateGPS(thisGPS);
                        thisTruck.TowTruckGPSUpdate();
                    }
                    if (type == "State")
                    {
                        thisTruck.UpdateState(thisState);
                        thisTruck.TowTruckChanged();
                    }
                    thisTruck.TTQueue.Enqueue(message);
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + " Error in UDP Listening Thread" + Environment.NewLine + ex.ToString() +
                    Environment.NewLine + "Original Message:" + Environment.NewLine + message, true);
                }
            }
        }

        private string getGPSTimeOfDay(int GPSSeconds)
        {
            try
            {
                string cDate = DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy 00:00:00");
                DateTime dt = Convert.ToDateTime(cDate).AddSeconds(GPSSeconds);
                return dt.ToString();
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now.ToString() + " Error in UDP Listening Thread" + Environment.NewLine + ex.ToString() +
                Environment.NewLine + "Original Message:" + Environment.NewLine + ex.Message, true);
                return "ERROR";
            }
        }

        private void queueMessage(string _message, string _ipaddr)
        {
            TowTruck.TowTruck towTruck = DataClasses.GlobalData.FindTowTruck(_ipaddr);
            if (towTruck == null)
            {
                try
                {
                    towTruck = new TowTruck.TowTruck(_ipaddr);
                    towTruck.TowTruckChangedEventHandler += this.TowTruckChanged;
                    towTruck.TowTruckGPSUpdateEventHandler += this.TowTruckGPSUpdated;
                }
                catch (Exception ex)
                {
                    logger.LogEvent(DateTime.Now.ToString() + Environment.NewLine + "Error queueing message" + Environment.NewLine + ex.ToString(), true);
                }
            }
        }

        public void TowTruckChanged(TowTruck.TowTruck towTruck)
        {

        }

        private void UpdateTowTruck(TowTruck.TowTruck towTruck)
        {
            towTruck.LastMessage.LastMessageReceived = DateTime.Now;

        }

        public void TowTruckGPSUpdated(TowTruck.TowTruck towTruck)
        {

        }
    }
}