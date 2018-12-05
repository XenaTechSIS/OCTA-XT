using System;
using System.Web;
using FPSService.BeatData;
using FPSService.DataClasses;
using FPSService.Logging;
using FPSService.MiscData;
using FPSService.SQL;
using FPSService.UDP;
using SqlServerTypes;

namespace FPSService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

            var myServer = new UDPServer();
            var myCleanser = new TowTruckCleanser();

            var myCheck = new LogonCheck();
            var myLogger = new EventLogger();
            myLogger.LogEvent(DateTime.Now + Environment.NewLine + "FSP Service Started", false);
            //Data loaded once during run of application.
            var mySql = new SQLCode();
            var speedingValue = Convert.ToInt32(mySql.GetVarValue("Speeding"));
            GlobalData.SpeedingValue = speedingValue;
            mySql.LoadCode1098s();
            mySql.LoadFreeways();
            mySql.LoadIncidentTypes();
            mySql.LoadLocationCoding();
            mySql.LoadServiceTypes();
            mySql.LoadTowLocations();
            mySql.LoadTrafficSpeeds();
            mySql.LoadVehiclePositions();
            mySql.LoadVehicleTypes();
            mySql.LoadContractors();
            mySql.LoadLeeways();
            mySql.LoadBeatSchedules();
            mySql.LoadDropZones();
            Beats.LoadBeats();
            Beats.LoadBeatSegments();
            YardClass.LoadYards();
            var myBulkLogger = new BulkLogger();
            var myDumper = new TruckDumper();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //This section allows clients other than IE to make sure it's OK to do cross domain scripting.  IE doesn't care and will
            //attemp cross-domain scripting without needing this pre-header information.
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods",
                    "GET, POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers",
                    "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age",
                    "1728000");
                HttpContext.Current.Response.End();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            var myLogger = new EventLogger();
            myLogger.LogEvent(DateTime.Now + Environment.NewLine + "FSP Service Has Stopped", false);
        }
    }
}