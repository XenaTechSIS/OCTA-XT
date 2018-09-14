using FSP.Web.Helpers;
using FSP.Web.TowTruckServiceRef;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using FSP.Web.Filters;

namespace FSP.Web.Controllers
{
    [CustomAuthorization]
    public class MapController : MyController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexOld()
        {
            return View();
        }

        #region yards

        [HttpGet]
        public ActionResult GetYardPolygons()
        {
            try
            {
                Util.LogInfo("yard polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawYards = service.RetreiveAllYards();
                    var yards = rawYards.OrderBy(p => p.TowTruckCompanyName).ToList().Select(s => new
                    {
                        s.YardID,
                        s.YardDescription,
                        s.Comments,
                        s.Location,
                        s.TowTruckCompanyName,
                        s.TowTruckCompanyPhoneNumber,
                        PolygonData = new PolygonData(s.Position)
                    }).ToList();

                    var jsonResult = Json(yards, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("yard polygons returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get Yard Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveYardPolygon(Yard_New data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Position))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                if (data.YardID == Guid.Empty)
                    data.YardID = Guid.NewGuid();

                using (var service = new TowTruckServiceClient())
                {
                    var updateResult = service.UpdateYard(data);
                    var response = new
                    {
                        result = updateResult == "success",
                        record = data
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Save Yard Polygon Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteYard(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteSResult = service.DeleteYard(id);
                    return Json(deleteSResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Delete Yard Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region segments

        [HttpGet]
        public ActionResult GetSegments()
        {
            try
            {
                Util.LogInfo("segment requested");
                using (var service = new TowTruckServiceClient())
                {
                    //var segments = service.RetreiveAllSegments().OrderBy(p => p.BeatSegmentNumber).ToList();

                    var rawSegments = service.RetreiveAllSegments();
                    var segments = rawSegments.OrderBy(p => p.BeatSegmentNumber).ToList().Select(s => new
                    {
                        s.BeatSegmentID,
                        s.BeatSegmentNumber,
                        s.BeatSegmentDescription,
                        s.CHPDescription,
                        s.CHPDescription2,
                        s.Color,
                        s.BeatSegmentExtent
                    }).ToList();

                    var jsonResult = Json(segments, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("segments polygons returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get Segment Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetSegmentPolygons()
        {
            try
            {
                Util.LogInfo("segment polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawSegments = service.RetreiveAllSegments();
                    var segments = rawSegments.OrderBy(p => p.BeatSegmentNumber).ToList().Select(s => new
                    {
                        s.BeatSegmentID,
                        s.BeatSegmentNumber,
                        s.BeatSegmentDescription,
                        s.CHPDescription,
                        s.CHPDescription2,
                        s.Color,
                        PolygonData = new PolygonData(s.BeatSegmentExtent)
                    }).ToList();

                    var jsonResult = Json(segments, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("segments polygons returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get Segment Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveSegmentPolygon(BeatSegment_New data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.BeatSegmentExtent))
                    return Json("false", JsonRequestBehavior.AllowGet);

                if (data.BeatSegmentID == Guid.Empty)
                    data.BeatSegmentID = Guid.NewGuid();

                data.LastUpdate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                data.LastUpdateBy = HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(data.CHPDescription))
                    data.CHPDescription = "";
                if (string.IsNullOrEmpty(data.CHPDescription2))
                    data.CHPDescription2 = "";
                using (var service = new TowTruckServiceClient())
                {
                    var updateResult = service.UpdateSegment(data);
                    var response = new
                    {
                        result = updateResult == "success",
                        record = data
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"SaveSegment Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteSegment(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteSResult = service.DeleteSegment(id);
                    return Json(deleteSResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"DeleteSegment Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region beats

        [HttpGet]
        public ActionResult GetBeatPolygons()
        {
            try
            {
                Util.LogInfo("beat polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawBeats = service.RetreiveAllBeats();
                    var beats = rawBeats.Where(p => p.Active).OrderBy(p => p.BeatNumber).ToList().Select(s => new
                    {
                        s.BeatID,
                        s.BeatDescription,
                        s.BeatNumber,
                        s.BeatColor,
                        BeatSegments = s.BeatSegments.OrderBy(p => p.BeatSegmentNumber).Select(seg => new
                        {
                            seg.BeatSegmentID,
                            seg.BeatSegmentNumber,
                            seg.BeatSegmentDescription,
                            seg.Color,
                            PolygonData = new PolygonData(seg.BeatSegmentExtent)
                        }).ToList()
                    }).ToList();

                    var jsonResult = Json(beats, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("beat polygons returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get Beat Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveBeatPolygon(Beats_New data)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    if (data.BeatID != Guid.Empty)
                    {
                        var dbBeat = service.RetreiveAllBeats().FirstOrDefault(p => p.BeatID == data.BeatID);
                        if (dbBeat != null)
                        {
                            dbBeat.BeatNumber = data.BeatNumber;
                            dbBeat.BeatDescription = data.BeatDescription;
                            dbBeat.BeatColor = data.BeatColor;

                            if (data.BeatSegments != null)
                            {
                                dbBeat.BeatSegments = data.BeatSegments;

                                var rawSegments = service.RetreiveAllSegments();

                                foreach (var segment in dbBeat.BeatSegments)
                                {
                                    try
                                    {
                                        var dbSegment = rawSegments.FirstOrDefault(p => p.BeatSegmentID == segment.BeatSegmentID);
                                        if (dbSegment == null) continue;
                                        dbSegment.Color = data.BeatColor;
                                        dbSegment.LastUpdate = DateTime.Now.ToString();
                                        dbSegment.LastUpdateBy = HttpContext.User.Identity.Name;
                                        var updateSegmentResult = service.UpdateSegment(dbSegment);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);                                       
                                    }

                                    //segment.Color = data.BeatColor;
                                    //segment.LastUpdate = DateTime.Now.ToString();
                                    //segment.LastUpdateBy = HttpContext.User.Identity.Name;
                                  
                                }
                            }
                            else
                            {
                                dbBeat.BeatSegments = new BeatSegment_New[0];
                            }

                            dbBeat.LastUpdate = DateTime.Now;
                            dbBeat.LastUpdateBy = HttpContext.User.Identity.Name;

                            if (dbBeat.StartDate == DateTime.MinValue)
                                dbBeat.StartDate = DateTime.Now;

                            if (dbBeat.EndDate == DateTime.MinValue)
                                dbBeat.EndDate = DateTime.Now;

                            var updateResult = service.UpdateBeat(dbBeat);

                            var response = new
                            {
                                result = updateResult == "success",
                                record = dbBeat
                            };

                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //new beat

                    if (data.BeatID == Guid.Empty)
                        data.BeatID = Guid.NewGuid();

                    data.StartDate = DateTime.Now;
                    data.EndDate = DateTime.Now;
                    data.LastUpdate = DateTime.Now;
                    data.LastUpdateBy = HttpContext.User.Identity.Name;
                    data.FreewayID = 0;
                    data.Active = true;

                    var createResult = service.UpdateBeat(data);

                    var newBeat = service.RetreiveAllBeats().FirstOrDefault(p => p.BeatID == data.BeatID);

                    var resp = new
                    {
                        result = createResult == "success",
                        record = newBeat
                    };

                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Save Beat Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteBeat(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteResult = service.DeleteBeat(id);
                    return Json(deleteResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Delete Beat Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region call boxes

        [HttpGet]
        public ActionResult GetCallBoxPolygons()
        {
            try
            {
                Util.LogInfo("call box polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawCallBoxes = service.RetreiveCallBoxes();
                    var callBoxes = rawCallBoxes.OrderBy(p => p.SignNumber).ToList().Select(s => new
                    {
                        s.CallBoxID,
                        s.Comments,
                        s.FreewayID,
                        s.Location,
                        s.SignNumber,
                        s.SiteType,
                        s.TelephoneNumber,
                        s.Position,
                        PolygonData = new PolygonData(s.Position)
                    }).ToList();

                    var jsonResult = Json(callBoxes, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("callboxes returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get CallBox Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveCallBoxPolygon(CallBoxes_New data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Position))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                if (data.CallBoxID == Guid.Empty)
                    data.CallBoxID = Guid.NewGuid();

                using (var service = new TowTruckServiceClient())
                {
                    var updateResult = service.UpdateCallBox(data);
                    return Json(updateResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Save CallBox Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteCallBox(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteResult = service.DeleteCallBox(id);
                    return Json(deleteResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Delete CallBox Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region 511

        [HttpGet]
        public ActionResult Get511Polygons()
        {
            try
            {
                Util.LogInfo("511 polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawCallBoxes = service.RetreiveFive11Signs();
                    var callBoxes = rawCallBoxes.OrderBy(p => p.SignNumber).ToList().Select(s => new
                    {
                        s.Five11SignID,
                        s.Comments,
                        s.FreewayID,
                        s.Location,
                        s.SignNumber,
                        s.SiteType,
                        s.TelephoneNumber,
                        s.Position,
                        PolygonData = new PolygonData(s.Position)
                    }).ToList();

                    var jsonResult = Json(callBoxes, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("511s returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get 511 Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Save511Polygon(Five11Signs data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Position))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                if (data.Five11SignID == Guid.Empty)
                    data.Five11SignID = Guid.NewGuid();

                using (var service = new TowTruckServiceClient())
                {
                    var updateResult = service.UpdateFive11Sign(data);
                    return Json(updateResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Save 511 Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete511(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteResult = service.DeleteFive11Sign(id);
                    return Json(deleteResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Delete 511 Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region drop zones

        [HttpGet]
        public ActionResult GetDropZonePolygons()
        {
            try
            {
                Util.LogInfo("dropzone polygons requested");
                using (var service = new TowTruckServiceClient())
                {
                    var rawDropZones = service.RetreiveAllDZs();
                    var dropZones = rawDropZones.OrderBy(p => p.DropZoneNumber).ToList().Select(s => new
                    {
                        s.DropZoneID,
                        s.DropZoneDescription,
                        s.DropZoneNumber,
                        s.Comments,
                        s.Location,
                        s.Restrictions,
                        s.Capacity,
                        s.City,
                        s.PDPhoneNumber,
                        PolygonData = new PolygonData(s.Position)
                    }).ToList();

                    var jsonResult = Json(dropZones, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("dropzone polygons returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Get Dropzone Polygons Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveDropZonePolygon(DropZone_New data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Position))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                if (data.DropZoneID == Guid.Empty)
                    data.DropZoneID = Guid.NewGuid();

                using (var service = new TowTruckServiceClient())
                {
                    var updateResult = service.UpdateDropZone(data);
                    return Json(updateResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Save Dropzone Polygon Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteDropZone(Guid id)
        {
            try
            {
                using (var service = new TowTruckServiceClient())
                {
                    var deleteResult = service.DeleteDropZone(id);
                    return Json(deleteResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"Delete DropZone Error: {ex.Message}, {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

    }

    public class PolygonData
    {
        public List<Coordinate> Coordinates { get; set; }

        public double MaxLat
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Max(p => p.lat);
            }
        }

        public double MinLat
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Min(p => p.lat);
            }
        }

        public double MaxLon
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Max(p => p.lng);
            }
        }

        public double MinLon
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Min(p => p.lng);
            }
        }

        public double MiddleLat
        {
            get { return this.MinLat + ((this.MaxLat - this.MinLat) / 2); }
        }

        public double MiddleLon
        {
            get { return this.MinLon + ((this.MaxLon - this.MinLon) / 2); }
        }

        private readonly string _beatExtent;

        public PolygonData(string beatExtent)
        {
            _beatExtent = beatExtent;
            this.Coordinates = this.BuildCoordinates();
        }

        private List<Coordinate> BuildCoordinates()
        {
            var returnList = new List<Coordinate>();

            try
            {
                Debug.WriteLine(this._beatExtent);
                returnList = JsonConvert.DeserializeObject<List<Coordinate>>(this._beatExtent);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return returnList;
        }
    }

    public class Coordinate
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}