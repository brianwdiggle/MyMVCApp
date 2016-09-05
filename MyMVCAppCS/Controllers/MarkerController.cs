using System.Linq;
using System.Web.Mvc;

namespace MyMVCAppCS.Controllers
{
    using System;

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
            var iqPaginatedMarkers = new PaginatedListMVC<Marker>(iqMarkers, page, MARKERS_PAGE_SIZE, Url.Action("Index", "Marker", new { OrderBy = ViewData["OrderBy"] + ViewData["OrderAscDesc"].ToString() }), MAX_PAGINATION_LINKS, "");

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

            var oMarker = this.repository.GetMarkerDetails(id);

            return this.View(oMarker);
        }

        public ActionResult Edit(int id)
        {
            Marker oMarker = this.repository.GetMarkerDetails(id);

            var markerStatii = this.repository.GetAllMarkerStatusOptions().AsEnumerable();

            ViewData["Marker_Status"] = markerStatii;

            return this.View(oMarker);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                Marker oMarker = this.repository.GetMarkerDetails(id);
                this.repository.UpdateMarkerDetails(oMarker, Request.Form);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return this.View();
            }
        }

    }
}
