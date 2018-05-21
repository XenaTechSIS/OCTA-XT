using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Filters;
using FSP.Web.Helpers;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;
using Microsoft.SqlServer.Types;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class TruckController : MyController
    {

        public ActionResult Grid()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult MapOld()
        {
            return View();
        }

        [OutputCache(Duration = 10)]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult HaveAlarms()
        {
            var returnValue = false;
            using (var service = new TowTruckServiceClient())
            {
                var alarms = service.GetAllAlarms().Where(p =>
                    p.RollInAlarm ||
                    p.RollOutAlarm ||
                    p.IncidentAlarm ||
                    p.LogOffAlarm ||
                    p.LogOnAlarm ||
                    p.GPSIssueAlarm ||
                    p.SpeedingAlarm ||
                    p.OutOfBoundsAlarm ||
                    p.StationaryAlarm).ToList();

                if (alarms.Any())
                    if (alarms.Any(p => p.RollInAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.RollOutAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.IncidentAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.LogOffAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.LogOnAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.OnPatrolAlarmTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.GPSIssueAlarmStart.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.StationaryAlarmStart.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.SpeedingTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
                    else if (alarms.Any(p => p.OutOfBoundsStartTime.ToString("MM/dd/yyyy") != "01/01/2001"))
                        returnValue = true;
            }

            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public List<TruckState> GetTruckStates()
        {
            var returnList = new List<TruckState>();

            if (HttpContext.Cache["truckStates"] == null)
            {
                using (var dc = new FSPDataContext())
                {
                    returnList = dc.TruckStates.ToList();
                }

                HttpContext.Cache.Insert("truckStates",
                    returnList,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(60));
            }
            else
            {
                returnList = (List<TruckState>)HttpContext.Cache["truckStates"];
            }

            return returnList;
        }

        [HttpGet]
        public ActionResult GetTruckRefreshRate()
        {
#if (DEBUG)
            return Json(5000, JsonRequestBehavior.AllowGet);
#else
            int.TryParse(ConfigurationManager.AppSettings["ServerRefreshRate"], out var refreshRate);
            return Json(refreshRate, JsonRequestBehavior.AllowGet);
#endif
        }

        [HttpGet]
        public ActionResult UpdateAllTrucks()
        {
            var towTrucks = new List<UITowTruck>();
            var truckStates = GetTruckStates();

            try
            {
                if (HttpContext.Cache["Trucks"] != null)
                {
                    towTrucks = (List<UITowTruck>)HttpContext.Cache["Trucks"];
                }
                else
                {
                    using (var service = new TowTruckServiceClient())
                    {
                        var serviceTowTrucks = service.CurrentTrucks();

                        for (var i = 0; i < serviceTowTrucks.Length; i++)
                        {
                            #region

                            var serviceTowTruck = serviceTowTrucks[i];

                            var truck = new UITowTruck();

                            try
                            {
                                truck.LastUpdate = 0;
                                truck.Old = false;

                                #region Set Truck Properties

                                truck.TruckNumber = serviceTowTruck.TruckNumber ?? "Not set";
                                truck.BeatNumber = serviceTowTruck.BeatNumber ?? "Not set";
                                truck.BeatSegmentNumber = "Not set";
                                truck.IpAddress = serviceTowTruck.IPAddress;

                                truck.Speed = serviceTowTruck.Speed;
                                truck.Lat = serviceTowTruck.Lat;
                                truck.Lon = serviceTowTruck.Lon;
                                truck.Heading = serviceTowTruck.Heading;

                                //truck.UserContractorName = this.UsersContractorCompanyName; //the logged in user's contractor name association
                                truck.DriverName = serviceTowTruck.DriverName;
                                truck.ContractorName = serviceTowTruck.ContractorName;
                                truck.Location = serviceTowTruck.OutOfBoundsMessage;

                                if (serviceTowTruck.SpeedingTime.ToString(CultureInfo.InvariantCulture) != "1/1/0001 12:00:00 AM" && serviceTowTruck.SpeedingTime.ToString() != "1/1/2001 12:00:00 AM")
                                    truck.SpeedingTime = serviceTowTruck.SpeedingTime.ToString("hh:mm:ss tt");

                                truck.SpeedingValue = serviceTowTruck.SpeedingValue.ToString(CultureInfo.InvariantCulture);
                                truck.OutOfBoundsMessage = serviceTowTruck.OutOfBoundsMessage;

                                if (serviceTowTruck.OutOfBoundsTime.ToString(CultureInfo.InvariantCulture) != "1/1/0001 12:00:00 AM")
                                    truck.OutOfBoundsTime = serviceTowTruck.OutOfBoundsTime.ToString("hh:mm:ss tt");
                                truck.HasAlarm = serviceTowTruck.Alarms;

                                if (serviceTowTruck.LastMessage.ToString(CultureInfo.InvariantCulture) != "1/1/0001 12:00:00 AM")
                                    truck.LastMessage = serviceTowTruck.LastMessage.ToString("hh:mm:ss");

                                if (serviceTowTruck.StatusStarted.ToString(CultureInfo.InvariantCulture) != "1/1/0001 12:00:00 AM")
                                    truck.LastStatusChanged = serviceTowTruck.StatusStarted.ToString("hh:mm:ss tt");


                                truck.VehicleState = serviceTowTruck.VehicleState;
                                truck.VehicleStateIconUrl = truckStates.FirstOrDefault(p => p.TruckState1 == truck.VehicleState)?.TruckIcon;

                                if (truck.HasAlarm)
                                    if (truck.VehicleStateIconUrl != null) truck.VehicleStateIconUrl = truck.VehicleStateIconUrl.Replace(".png", "") + "_Alarm.png";

                                if (serviceTowTruck.Alarms && string.IsNullOrEmpty(truck.VehicleState))
                                {
                                    //All alarms red (OnAlarm.png) expect speeding is yellow
                                    truck.VehicleStateIconUrl = "Alarm.png"; //Red

                                    if (serviceTowTruck.SpeedingAlarm)
                                        truck.VehicleStateIconUrl = "Speeding.png"; //Yellow
                                }

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Util.WriteToLog(ex.Message, "Error.txt");
                            }

                            towTrucks.Add(truck);

                            #endregion
                        }

                        int.TryParse(ConfigurationManager.AppSettings["ServerRefreshRate"], out var refreshRate);
                        HttpContext.Cache.Add("Trucks", towTrucks, null, DateTime.Now.AddMilliseconds(refreshRate), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.WriteToLog(ex.Message, "Error.txt");
            }

            //check to see if current user is a contractor. Then filter to only "his/her" trucks"
            if (!string.IsNullOrEmpty(UsersContractorCompanyName))
                towTrucks = towTrucks.Where(p => p.ContractorName == UsersContractorCompanyName).ToList();

            return Json(towTrucks.OrderBy(p => p.BeatNumber).ThenBy(p => p.TruckNumber), JsonRequestBehavior.AllowGet);
        }

        #region Other Calls

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetDropZones()
        {
            var returnList = new List<UIDropZone>();
            try
            {
                using (var dc = new FSPDataContext())
                {
                    var dropZones = dc.vDropZones.ToList();

                    foreach (var dropZone in dropZones)
                    {
                        var geo = new SqlGeography();
                        geo = SqlGeography.Parse(dropZone.Position);

                        var linePoints = new List<UIMapPolygonLinePoint>();
                        for (var i = 1; i < geo.STNumPoints(); i++)
                            try
                            {
                                var point = geo.STPointN(i);
                                var lat = Convert.ToDouble(point.Lat.ToString());
                                var lon = Convert.ToDouble(point.Long.ToString());
                                linePoints.Add(new UIMapPolygonLinePoint
                                {
                                    Lat = lat,
                                    Lon = lon
                                });
                            }
                            catch (Exception ex)
                            {
                                Util.WriteToLog(ex.Message, "Error.txt");
                                Util.WriteToEventLog(ex.Message);
                            }

                        returnList.Add(new UIDropZone
                        {
                            DropZoneID = dropZone.DropZoneID,
                            DropZoneNumber = dropZone.DropZoneNumber,
                            Comments = dropZone.Comments,
                            DropZoneDescription = dropZone.DropZoneDescription,
                            PolygonPoints = linePoints
                        });
                    }
                }
            }
            catch
            {
            }
            Debug.WriteLine("Drop zones loaded: " + DateTime.Now);
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetCallBoxes()
        {
            var returnList = new List<UICallBox>();
            try
            {
                using (var dc = new FSPDataContext())
                {
                    var callBoxes = dc.vCallBoxes;

                    foreach (var callBox in callBoxes)
                    {
                        var geo = new SqlGeography();
                        geo = SqlGeography.Parse(callBox.Position);

                        var lat = Convert.ToDouble(geo.STPointN(1).Lat.ToString());
                        var lon = Convert.ToDouble(geo.STPointN(1).Long.ToString());

                        returnList.Add(new UICallBox
                        {
                            CallBoxId = callBox.CallBoxID,
                            FreewayId = callBox.FreewayID,
                            Lat = lat,
                            Lon = lon,
                            LocationDescription = callBox.Location,
                            TelephoneNumber = callBox.TelephoneNumber,
                            SignNumber = callBox.SignNumber,
                            SiteType = callBox.SiteType,
                            Comments = callBox.Comments
                        });
                    }
                }
            }
            catch
            {
            }
            Debug.WriteLine("Call boxes loaded: " + DateTime.Now);
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetBeats()
        {
            var returnList = new List<UIMapPolygonLine>();

            try
            {
                using (var dc = new FSPDataContext())
                {
                    var beatsQuery = (from q in dc.vBeats
                                      where q.Active
                                      select q).ToList();

                    foreach (var beat in beatsQuery)
                    {
                        var geo = new SqlGeography();
                        geo = SqlGeography.Parse(beat.BeatExtent);


                        var linePoints = new List<UIMapPolygonLinePoint>();
                        for (var i = 1; i < geo.STNumPoints(); i++)
                            try
                            {
                                var point = geo.STPointN(i);
                                var lat = Convert.ToDouble(point.Lat.ToString());
                                var lon = Convert.ToDouble(point.Long.ToString());
                                linePoints.Add(new UIMapPolygonLinePoint
                                {
                                    Lat = lat,
                                    Lon = lon
                                });
                            }
                            catch (Exception ex)
                            {
                                Util.WriteToLog(ex.Message, "Error.txt");
                                Util.WriteToEventLog(ex.Message);
                            }

                        returnList.Add(new UIMapPolygonLine
                        {
                            Number = beat.BeatNumber.Substring(beat.BeatNumber.Length - 3, 3),
                            Description = beat.BeatNumber + ": " + beat.BeatDescription,
                            Points = linePoints,
                            Color = beat.BeatColor
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Util.WriteToLog(ex.Message, "Error.txt");
                Util.WriteToEventLog(ex.Message);
            }
            Debug.WriteLine("Beats loaded: " + DateTime.Now);
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetBeatNumbers(string name_startsWith)
        {
            using (var dc = new FSPDataContext())
            {
                var beatsQuery = from q in dc.vBeats
                                 where q.Active && q.BeatNumber.Contains(name_startsWith)
                                 orderby q.BeatNumber
                                 select new
                                 {
                                     //Number = q.BeatNumber,
                                     Number = q.BeatNumber.Substring(q.BeatNumber.Length - 3, 3),
                                     Description = q.BeatDescription
                                 };

                Debug.WriteLine("Beat numbers loaded: " + DateTime.Now);
                return Json(beatsQuery, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetTowTruckContractors(string name_startsWith)
        {
            using (var dc = new FSPDataContext())
            {
                var contractorQuery = from q in dc.Contractors
                                      where q.ContractCompanyName.Contains(name_startsWith)
                                      orderby q.ContractCompanyName
                                      select new
                                      {
                                          Number = q.ContractCompanyName,
                                          Description = q.ContractCompanyName
                                      };
                Debug.WriteLine("Tow Truck Contractors: " + DateTime.Now);
                return Json(contractorQuery.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(CacheProfile = "OtherDataCalls")]
        public ActionResult GetBeatSegments()
        {
            var returnList = new List<UIMapPolygonLine>();

            using (var dc = new FSPDataContext())
            {
                try
                {
                    var beatsQuery = (from q in dc.vBeatSegments
                                      select q).ToList();

                    foreach (var beat in beatsQuery)
                    {
                        var geo = new SqlGeography();
                        geo = SqlGeography.Parse(beat.BeatSegmentExtent);


                        var linePoints = new List<UIMapPolygonLinePoint>();
                        for (var i = 1; i < geo.STNumPoints(); i++)
                            try
                            {
                                var point = geo.STPointN(i);
                                var lat = Convert.ToDouble(point.Lat.ToString());
                                var lon = Convert.ToDouble(point.Long.ToString());
                                linePoints.Add(new UIMapPolygonLinePoint
                                {
                                    Lat = lat,
                                    Lon = lon
                                });
                            }
                            catch (Exception ex)
                            {
                                Util.WriteToLog(ex.Message, "Error.txt");
                                Util.WriteToEventLog(ex.Message);
                            }

                        returnList.Add(new UIMapPolygonLine
                        {
                            Number = beat.BeatSegmentNumber,
                            Description = beat.BeatSegmentNumber + ": " + beat.BeatSegmentDescription,
                            Points = linePoints
                        });
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteToLog(ex.Message, "Error.txt");
                    Util.WriteToEventLog(ex.Message);
                }
            }


            Debug.WriteLine("Beats segments loaded: " + DateTime.Now);
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}