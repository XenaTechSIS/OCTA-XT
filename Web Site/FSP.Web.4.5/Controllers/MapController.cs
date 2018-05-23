using FSP.Web.Helpers;
using FSP.Web.TowTruckServiceRef;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
                    var segments = service.RetreiveAllSegments().OrderBy(p => p.BeatSegmentNumber).ToList().Select(s => new
                    {
                        s.BeatSegmentID,
                        s.BeatSegmentNumber,
                        s.BeatSegmentDescription,
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
                using (var service = new TowTruckServiceClient())
                {
                    service.UpdateSegment(data);
                    return Json("true", JsonRequestBehavior.AllowGet);
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
                return this.Coordinates.Max(p => p.Lat);
            }
        }

        public double MinLat
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Min(p => p.Lat);
            }
        }

        public double MaxLon
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Max(p => p.Lon);
            }
        }

        public double MinLon
        {
            get
            {
                if (!this.Coordinates.Any()) return 0;
                return this.Coordinates.Min(p => p.Lon);
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

            var pointsArray = _beatExtent.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var s in pointsArray)
            {
                var coord = s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (coord.Length != 2) continue;

                returnList.Add(new Coordinate
                {
                    Lat = Convert.ToDouble(coord[1]),
                    Lon = Convert.ToDouble(coord[0])
                });
            }

            return returnList;
        }
    }

    public class Coordinate
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}