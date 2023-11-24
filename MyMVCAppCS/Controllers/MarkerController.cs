using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace MyMVCAppCS.Controllers
{
    using System;
    using System.Collections.Generic;
    using MyMVCApp.DAL;

    using MyMVCAppCS.Models;

    public class MarkerController : Controller
    {
        private const int MARKERS_PAGE_SIZE = 50;

        private const int MAX_PAGINATION_LINKS = 10;

        private readonly IWalkingRepository repository;

        public MarkerController()
        {
            this.repository = new SqlWalkingRepository(SessionSingleton.Current.ConnectionString);
        }

        //
        // GET: /Marker/
        public ActionResult Index(string orderBy, int page=1) 
        {
            IQueryable<Marker> iqMarkers;
            int iShowMap = 0;
       
            // ---Use the walking repository to get a list of all the markers----
            // ---Set up the ordering of the markers------
            // -----Date Ordering-----------
            if ((orderBy == "DateAsc")) {
                iqMarkers = repository.FindAllMarkers().OrderBy(marker =>marker.DateLeft).ThenBy(marker =>marker.MarkerTitle);
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "DateDesc")) {
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker =>marker.DateLeft).ThenByDescending(marker =>marker.MarkerTitle);
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "TitleAsc")) {
                iqMarkers = repository.FindAllMarkers().OrderBy(marker =>marker.MarkerTitle).ThenBy(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Title";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "TitleDesc")) {
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker =>marker.MarkerTitle).ThenByDescending(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Title";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "StatusAsc")) {
                iqMarkers = repository.FindAllMarkers().OrderBy(marker =>marker.Status).ThenBy(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Status";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "StatusDesc")) {
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker =>marker.Status).ThenByDescending(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Status";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "WalkAsc")) {
                iqMarkers = repository.FindAllMarkers().OrderBy(marker =>marker.Walk.WalkTitle).ThenBy(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Walk";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "WalkDesc")) {
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker =>marker.Walk.WalkTitle).ThenByDescending(marker =>marker.DateLeft);
                ViewData["OrderBy"] = "Walk";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "WalkAreaAsc"))
            {
                iqMarkers = repository.FindAllMarkers().OrderBy(marker => marker.Walk.Area.Areaname).ThenBy(marker => marker.DateLeft);
                ViewData["OrderBy"] = "WalkArea";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "WalkAreaDesc"))
            {
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker => marker.Walk.Area.Areaname).ThenByDescending(marker => marker.DateLeft);
                ViewData["OrderBy"] = "WalkArea";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else {
                // ----Default to order by date ascending----
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Desc";
                iqMarkers = repository.FindAllMarkers().OrderByDescending(marker =>marker.DateLeft).ThenByDescending(marker =>marker.MarkerTitle);
            }
            // ----Create a paginated list of the walks----------------
            var iqPaginatedMarkers = new PaginatedListMVC<Marker>(iqMarkers, page, MARKERS_PAGE_SIZE, Url.Action("Index", "Marker", new { OrderBy = ViewData["OrderBy"] + ViewData["OrderAscDesc"].ToString() }), MAX_PAGINATION_LINKS, "", "");

            ///----Prepare data about markers associated the walk
            List<MapMarker> lstMarkerMarkers = new List<MapMarker>();
            foreach (Marker oMarker in iqPaginatedMarkers)
            {
                if (oMarker.GPS_Reference.Trim() != "")
                {
                    MapMarker oMM = new MapMarker
                    {
                        OSMap10 = oMarker.GPS_Reference,
                        popupText = WalkingStick.MarkerPopup(oMarker, HttpContext.Request.ApplicationPath)
                    };
                    lstMarkerMarkers.Add(oMM);
                    iShowMap = 1;
                }
            }

            ViewData["ShowMap"] = iShowMap;
            ViewData["MarkerMarkers"] = lstMarkerMarkers;
            return View(iqPaginatedMarkers);
        }

        public ActionResult Create()
        {
            var oMarker = new Marker();

            var oMarkerStatusii = this.repository.GetAllMarkerStatusOptions().AsEnumerable();
            ViewData["MarkerStatusii"] = new SelectList(oMarkerStatusii, "Marker_Status1", "Marker_Status1");
            ViewData["Model"] = oMarker;

            return this.View();
        } 

        [HttpPost]
        public ActionResult Create(Marker oFormMarker)
        {
          
            Marker oNewMarker = WalkingStick.FillMarkerFromFormVariables(oFormMarker,Request.Form);

            this.repository.CreateMarker(oNewMarker);

            var oMarkerStatusii = this.repository.GetAllMarkerStatusOptions().AsEnumerable();
            ViewData["MarkerStatusii"] = new SelectList(oMarkerStatusii, "Marker_Status1", "Marker_Status1");

            return RedirectToAction("Index"); 
        }

        public ActionResult Details(int id)
        {
            ViewData["AT_WORK"] = System.Web.Configuration.WebConfigurationManager.AppSettings["atwork"];
            int iShowMap = 0;

            List<MapMarker> lstMarkerMarkers = new List<MapMarker>();

            var oMarker = this.repository.GetMarkerDetails(id);

            if (oMarker.GPS_Reference.Trim() != "")
            {
                MapMarker oMM = new MapMarker
                {
                    OSMap10 = oMarker.GPS_Reference,
                    popupText = WalkingStick.MarkerPopup(oMarker, Request.Url.GetLeftPart(System.UriPartial.Authority))
                };
                lstMarkerMarkers.Add(oMM);
                iShowMap = 1;
            }
            else if (oMarker.Hill != null && (oMarker.Hill.Gridref10 != "" || oMarker.Hill.Gridref != ""))
            {
                MapMarker oMM = new MapMarker
                {
                    OSMap10 = WalkingStick.FivePacesEastFromSummit(oMarker.Hill),
                    popupText = WalkingStick.MarkerPopup(oMarker, Request.Url.GetLeftPart(System.UriPartial.Authority))
                };
                lstMarkerMarkers.Add(oMM);
                iShowMap = 1;
            }

            ViewData["ShowMap"] = iShowMap;
            ViewData["MarkerMarkers"] = lstMarkerMarkers;

            return this.View(oMarker);
        }

        /// <summary>
        /// Name: _MarkersInMapBounds
        /// Desc: Given a map bounds defition which is a rectangle of SW and NE points in lat/long format,
        ///          a) query database for markers in these bounds
        ///          b) return the set of markers in 
        ///          
        /// </summary>
        /// <param name="neLat"></param>
        /// <param name="neLng"></param>
        /// <param name="swLat"></param>
        /// <param name="swLng"></param>
        /// <returns></returns>
        public JsonResult _MarkersInMapBounds(string neLat, string neLng, string swLat, string swLng)
        {
            var mapmarkers = "hellow world we are about to do something awesome...";

            float fNeLat, fNeLng, fSwLat, fSwLng;
            try
            {
                fNeLat = float.Parse(neLat);
                fNeLng = float.Parse(neLng);
                fSwLat = float.Parse(swLat);
                fSwLng = float.Parse(swLng);
            }
            catch (Exception e)
            {
                mapmarkers = "Error occurred problem with lat/long format of new map bounds: " + e.Message;
                return Json(mapmarkers, JsonRequestBehavior.AllowGet);
            }

            // Given the new map bounds, get the set of markers which fall within these bounds
            IEnumerable<Marker> IEMarkersWithLocation = this.repository.GetAllMarkersWithLocation();

            List<Marker> markersInMapBounds = WalkingStick.SelectMarkersInMapBounds(IEMarkersWithLocation, fNeLat, fNeLng, fSwLat, fSwLng);


            return Json(mapmarkers, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int id)
        {
            Marker oMarker = this.repository.GetMarkerDetails(id);

            var markerStatii = this.repository.GetAllMarkerStatusOptions().AsEnumerable();

            ViewData["Marker_Status"] = markerStatii;
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;

            return this.View(oMarker);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                Marker oMarker = this.repository.GetMarkerDetails(id);
                this.repository.UpdateMarkerDetails(oMarker, Request.Form);

                return Redirect(formCollection["previousUrl"]);
            }
            catch (Exception)
            {
                return this.View();
            }
        }

    }
}
