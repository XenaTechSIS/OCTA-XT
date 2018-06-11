using FSP.Web.Helpers;
using FSP.Web.TowTruckServiceRef;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace FSP.Web.Controllers
{
    public class MapController : Controller
    {
        #region segments

        [HttpGet]
        public ActionResult GetSegmentPolygons()
        {
            try
            {
                Util.LogInfo("segments requested");
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
                        PolygonData = new PolygonData(s.BeatSegmentExtent)
                        //BeatSegmentPolygonCoordinates = this.CreatePolygon(s.BeatSegmentExtent)
                    }).ToList();

                    var jsonResult = Json(segments, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    Util.LogInfo("segments returned");
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"GetSegments Error: {ex.Message}");
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

                data.LastUpdate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                data.LastUpdateBy = HttpContext.User.Identity.Name;

                using (var service = new TowTruckServiceClient())
                {
                    var updateSegmentResult = service.UpdateSegment(data);
                    return Json(updateSegmentResult == "success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"SaveSegment Error: {ex.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AddSegmentPolygon(BeatSegment_New data)
        {
            try
            {
                var stringData = JsonConvert.SerializeObject(data);

                using (var service = new TowTruckServiceClient())
                {
                    //data.ID = Guid.NewGuid();
                    service.UpdateSegment(data);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"AddSegment Error: {ex.Message}");
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
                    service.DeleteSegment(id);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Util.LogError($"DeleteSegment Error: {ex.Message}, {ex.Message}");
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

            //until JON is fixing the LAT/LNG switch

            var newReturnList = new List<Coordinate>();
            foreach (var coordinate in returnList)
            {
                newReturnList.Add(new Coordinate
                {
                    lat = coordinate.lng,
                    lng = coordinate.lat
                });
            }
            return newReturnList;
        }
    }

    public class Coordinate
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}