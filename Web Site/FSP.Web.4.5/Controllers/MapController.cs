using FSP.Web.Helpers;
using FSP.Web.TowTruckServiceRef;
using Newtonsoft.Json;
using System;
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
                    var segments = service.RetreiveAllSegments().OrderBy(p => p.BeatSegmentID).ToList();
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
}