
using System.Web.Mvc;
using System.Web.UI;
using System.Linq;

namespace MyMVCAppCS.Controllers
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Configuration;

    using MyMVCApp.DAL;
    using MyMVCApp.Model;

    using MyMVCAppCS.Models;
    using MyMVCAppCS.ViewModels;

#region "Page Actions"

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class WalksController : Controller
    {
   
        private readonly IWalkingRepository repository;

        public WalksController()
        {
            this.repository = new SqlWalkingRepository(SessionSingleton.Current.ConnectionString);
        }

        //
        // GET: /Walks/
        public ActionResult Index()
        {
            ViewBag.MVCVersion = typeof(Controller).Assembly.GetName().Version;
  
            return View();
        }

        public ActionResult WalkingAreasByCountry(string strCountryCode, string strAreaType = "")
        {
            string strCountryName = WalkingStick.CountryNameFromCountryCode(strCountryCode);
            string strAreaTypeName = "";
            ViewData["CountryName"] = strCountryName;

            IQueryable<Area> iqWalkingAreas;

            if ( strAreaType.Equals(""))
            {
                iqWalkingAreas = this.repository.GetAllWalkingAreas(strCountryCode);
            }
            else
            {
                iqWalkingAreas = this.repository.GetAllWalkingAreas(strCountryCode, strAreaType);
                strAreaTypeName = repository.GetWalkAreaTypeNameFromType(strAreaType);
            }
            ViewData["AreaTypeName"] = strAreaTypeName;

            return this.View(iqWalkingAreas);

        }


        public ActionResult HillClasses()
        {
            var iqHillClasses =
                this.repository.GetAllHillClassifications().OrderBy(classification => classification.Classname);

            return this.View(iqHillClasses);
        }

        /// <summary>
        /// Show a list of hills by areas
        /// </summary>
        /// <param name="id">ID of area show</param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult HillsByArea(string id, string orderBy = "NameAsc", int page = 1)
        {
           // var IQHillsInWalkingArea = this.repository.FindHillsByArea(id);

            IQueryable<Hill> iqHillsInWalkingArea;

            if ((orderBy == "NameAsc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderBy(hill => hill.Hillname);
                ViewData["OrderBy"] = "Name";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "NameDesc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderByDescending(hill => hill.Hillname);
                ViewData["OrderBy"] = "Name";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "MetresAsc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderBy(hill => hill.Metres);
                ViewData["OrderBy"] = "Metres";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "MetresDesc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderByDescending(hill => hill.Metres);
                ViewData["OrderBy"] = "Metres";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "FirstAscentDesc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderByDescending(hill => hill.FirstClimbedDate);
                ViewData["OrderBy"] = "FirstAscent";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "FirstAscentAsc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderBy(hill => hill.FirstClimbedDate);
                ViewData["OrderBy"] = "FirstAscent";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((orderBy == "NumberAscentDesc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderByDescending(hill => hill.NumberOfAscents);
                ViewData["OrderBy"] = "NumberAscent";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((orderBy == "NumberAscentAsc"))
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id).OrderBy(hill => hill.NumberOfAscents);
                ViewData["OrderBy"] = "NumberAscent";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else
            {
                iqHillsInWalkingArea = repository.FindHillsByArea(id);
                ViewData["OrderBy"] = "Name";
                ViewData["OrderAscDesc"] = "Asc";
            }

            int pageSize = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_PAGE_SIZE"]);
            int maxPageLinks = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_MAX_PAGE_LINKS"]);
            string strAreaName = this.repository.GetWalkAreaNameFromAreaRef(id);

            ViewData["AreaName"] = strAreaName;

            // ----Create a paginated list of hills----------------
            PaginatedListMVC<Hill> iqPaginatedHills = new PaginatedListMVC<Hill>(iqHillsInWalkingArea,
                                                                                page,
                                                                                pageSize,
                                                                                Url.RouteUrl("Default", new { action="HillsByArea", controller="Walks"}),
                                                                                maxPageLinks,
                                                                                "?OrderBy" + orderBy);

            // -----Pass the paginated list of hills to the view. The view expects a paginated list as its model-----
            return View(iqPaginatedHills);
        }

        public ActionResult HillDetails(int id)
        {
            var oHillDetails = this.repository.GetHillDetails(id);

            var oHillAscents = this.repository.GetHillAscents(id).OrderBy(hill => hill.AscentDate);

            ViewData["HillAscents"] = oHillAscents.AsEnumerable().ToList();
            return this.View(oHillDetails);

        }

   // -------------------------------------------------------------------------------------
    //  Function: HillInClassification
    //  URL     : /Walks/HillsInClassification/Classref/[Pagenumber]
    //  Descr   : Return a list of hills with classification as specified by id parameter
    //            optional page parameter provides pagination.
    // --------------------------------------------------------------------------------------
    public ActionResult HillsInClassification(string id, string orderBy="NameAsc", int page=1) 
    {
    
        if ((id == null)) 
        {
            ViewData["HillClassName"] = "All Hill Classes";
        }
        else 
        {
            ViewData["HillClassName"] = repository.GetHillClassificationName(id);
        }
        // -----Get the full hill classification name and pass it to the view---------------
        // ---Use the walking repository to get a list of all the hills in the specified classification----
        IQueryable<Hill> IQHillsInClassificaton;
      
        if ((orderBy == "NameAsc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderBy(hill =>hill.Hillname);
            ViewData["OrderBy"] = "Name";
            ViewData["OrderAscDesc"] = "Asc";
        }
        else if ((orderBy == "NameDesc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderByDescending(hill => hill.Hillname);
            ViewData["OrderBy"] = "Name";
            ViewData["OrderAscDesc"] = "Desc";
        }
        else if ((orderBy == "MetresAsc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderBy(hill => hill.Metres);
            ViewData["OrderBy"] = "Metres";
            ViewData["OrderAscDesc"] = "Asc";
        }
        else if ((orderBy == "MetresDesc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderByDescending(hill => hill.Metres);
            ViewData["OrderBy"] = "Metres";
            ViewData["OrderAscDesc"] = "Desc";
        }
        else if ((orderBy == "FirstAscentDesc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderByDescending(hill => hill.FirstClimbedDate);
            ViewData["OrderBy"] = "FirstAscent";
            ViewData["OrderAscDesc"] = "Desc";
        }
        else if ((orderBy == "FirstAscentAsc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderBy(hill => hill.FirstClimbedDate);
            ViewData["OrderBy"] = "FirstAscent";
            ViewData["OrderAscDesc"] = "Asc";
        }
        else if ((orderBy == "NumberAscentDesc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderByDescending(hill => hill.NumberOfAscents);
            ViewData["OrderBy"] = "NumberAscent";
            ViewData["OrderAscDesc"] = "Desc";
        }
        else if ((orderBy == "NumberAscentAsc")) {
            IQHillsInClassificaton = repository.GetHillsByClassification(id).OrderBy(hill => hill.NumberOfAscents);
            ViewData["OrderBy"] = "NumberAscent";
            ViewData["OrderAscDesc"] = "Asc";
        }
        else {
            IQHillsInClassificaton = repository.GetHillsByClassification(id);
            ViewData["OrderBy"] = "Name";
            ViewData["OrderAscDesc"] = "Asc";
        }
        object iNumClimbed = IQHillsInClassificaton.Count(hill => hill.NumberOfAscents > 0);

        ViewData["NumberClimbed"] = iNumClimbed;

        int pageSize = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_PAGE_SIZE"]);
        int maxPageLinks = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_MAX_PAGE_LINKS"]);

        // ----Create a paginated list of hills----------------
        PaginatedListMVC<Hill> iqPaginatedHills = new PaginatedListMVC<Hill>(IQHillsInClassificaton, 
                                                                            page, 
                                                                            pageSize,
                                                                            Url.RouteUrl("Default", new { action="HillsInClassification", controller="Walks"}),
                                                                            maxPageLinks,
                                                                            "?OrderBy=" + orderBy); 
     
     
        // -----Pass the paginated list of hills to the view. The view expects a paginated list as its model-----
        return View(iqPaginatedHills);
    }


        // -------------------------------------------------------------------------------------
        //  Function: WalksByDate
        //  URL     : /Walks/WalksByDate/OrderBy/{page}
        //  Descr   : Return a list of walks by date, order as per the OrderBy parameter
        // --------------------------------------------------------------------------------------
        public ActionResult WalksByDate(string OrderBy, int page=1) {
            
            IOrderedQueryable<Walk> iqWalks;
   
            // ---Use the walking repository to get a list of all the walks----
            // ---Set up the ordering of the walks ------
            if ((OrderBy == "DateAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkDate);
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "DateDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.WalkDate);
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "TitleAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkTitle);
                ViewData["OrderBy"] = "Title";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "TitleDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.WalkTitle);
                ViewData["OrderBy"] = "Title";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "AreaAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkAreaName);
                ViewData["OrderBy"] = "Area";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "AreaDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.WalkAreaName);
                ViewData["OrderBy"] = "Area";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "LengthAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.CartographicLength);
                ViewData["OrderBy"] = "Length";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "LengthDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.CartographicLength);
                ViewData["OrderBy"] = "Length";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "AscentAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.MetresOfAscent);
                ViewData["OrderBy"] = "Ascent";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "AscentDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.MetresOfAscent);
                ViewData["OrderBy"] = "Ascent";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "TotalTimeAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkTotalTime);
                ViewData["OrderBy"] = "TotalTime";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "TotalTimeDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.WalkTotalTime);
                ViewData["OrderBy"] = "TotalTime";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "MovAvgAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.MovingAverageKmh);
                ViewData["OrderBy"] = "MovAvg";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "MovAvgDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.MovingAverageKmh);
                ViewData["OrderBy"] = "MovAvg";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else if ((OrderBy == "OvlAvgAsc")) {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkAverageSpeedKmh);
                ViewData["OrderBy"] = "OvlAvg";
                ViewData["OrderAscDesc"] = "Asc";
            }
            else if ((OrderBy == "OvlAvgDesc")) {
                iqWalks = repository.FindAllWalks().OrderByDescending(walk =>walk.WalkAverageSpeedKmh);
                ViewData["OrderBy"] = "OvlAvg";
                ViewData["OrderAscDesc"] = "Desc";
            }
            else {
                iqWalks = repository.FindAllWalks().OrderBy(walk =>walk.WalkDate);
                ViewData["OrderBy"] = "Date";
                ViewData["OrderAscDesc"] = "Desc";
            }

            int pageSize = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_PAGE_SIZE"]);
            int maxPageLinks = Int32.Parse(WebConfigurationManager.AppSettings["PAGINATION_MAX_PAGE_LINKS"]);

            ViewData["page"] = page;
            ViewData["pagesize"] = pageSize;
            ViewData["StartWalkNumber"] = ((page-1) * pageSize) + 1;

            // ----Create a paginated list of the walks----------------
            var IQPaginatedWalks = new PaginatedListMVC<Walk>(iqWalks, page, pageSize, Url.Action("WalksByDate", "Walks", new {OrderBy = ViewData["OrderBy"].ToString() + ViewData["OrderAscDesc"].ToString()}), maxPageLinks, "");

            // -----Pass the paginated list of walks to the view. The view expects a paginated list as its model-----
            return View(IQPaginatedWalks);
        }


        public ActionResult Details(int id)
        {
            Walk oWalk = this.repository.GetWalkDetails(id);
            string strTotalTime = WalkingStick.ConvertTotalTimeToString(oWalk.WalkTotalTime, false);

            ViewData["TotalTime"] = strTotalTime;

            return this.View(oWalk);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int id)
        {
            Walk oWalk = this.repository.GetWalkDetails(id);
            var oAssociatedFileTypes = this.repository.GetAssociatedFileTypes();
            var oAuxilliaryFiles = this.repository.GetWalkAuxilliaryFiles(id);

            var oWalkTypes = this.repository.GetWalkTypes();
            var oWalkMarkers = this.repository.GetMarkersCreatedOnWalk(id);

            ViewBag.WalkTypes = new SelectList(oWalkTypes, "WalkTypeString", "WalkTypeString", oWalk.WalkType);
            ViewBag.Associated_File_Types = new SelectList(oAssociatedFileTypes, "Walk_AssociatedFile_Type1", "Walk_AssociatedFile_Type1");
            ViewData["Model"] = oWalk;
            ViewBag.Auxilliary_Files = oAuxilliaryFiles.AsEnumerable().ToList();
            ViewData["WalkMarkersAlreadyCreated"] = oWalkMarkers;

            if (oWalk.WalkTotalTime != null)
            {
                ViewBag.WalkHours = (int)oWalk.WalkTotalTime / 60;
                ViewBag.WalkMinutes = (int)oWalk.WalkTotalTime % 60;
            }
            else
            {
                ViewBag.WalkHours = 0;
                ViewBag.WalkMinutes = 0;
            }
  
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;

            return this.View(oWalk);
        }

        // -------------------------------------------------------------------------------------
        //  Function: Create  (GET)
        //  URL     : /Walks/Create
        //  Descr   : Create Walk form
        // --------------------------------------------------------------------------------------
        public ActionResult Create()
        {
            Walk oWalk = new Walk();
            var oAssociatedFileTypes = this.repository.GetAssociatedFileTypes();
            var oWalkTypes = this.repository.GetWalkTypes();

            ViewData["WalkTypes"] = new SelectList(oWalkTypes, "WalkTypeString", "WalkTypeString");
            ViewData["Associated_File_Types"] = new SelectList(oAssociatedFileTypes, "Walk_AssociatedFile_Type1", "Walk_AssociatedFile_Type1").ToList();
            ViewData["Model"] = oWalk;

            return this.View();

        }


        // -------------------------------------------------------------------------------------
        //  Function: Create  (POST)
        //  URL     : /Walks/Create
        //  Descr   : Create Walk form
        // --------------------------------------------------------------------------------------
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Walk walk)
        {
            // ----This should really be done by a customised Model Binder-----------
            Walk oWalk = new Walk();
            WalkingStick.FillWalkFromFormVariables(ref oWalk, Request.Form);
            int iWalkID = this.repository.AddWalk(oWalk);

            // ---Add hill ascents-----------------
            List<HillAscent> arHillAscents = WalkingStick.FillHillAscentsFromFormVariables(iWalkID, this.Request.Form);
            repository.AddWalkSummitsVisited(arHillAscents);

            // ---Add the associated files-----
            List<Walk_AssociatedFile> arAssociatedFiles = WalkingStick.FillHillAssociatedFilesFromFormVariables(
                iWalkID,
                this.Request.Form,
                this.Server.MapPath("/"));
            repository.AddWalkAssociatedFiles(arAssociatedFiles);

            // ---update any markers created by ajax call with walk id, and add any marker observations----------------
            repository.AssociateMarkersWithWalk(Request.Form, iWalkID);
            if ((walk.HillAscents.Count > 0))
            {
                return RedirectToAction("HillsByArea", new { id = oWalk.Area.Arearef.Trim(), page = 1 });
            }

            return RedirectToAction("WalksByDate", new { OrderBy = "DateDesc" });
        }


        // -------------------------------------------------------------------------------------
        //  Function: CreateAuto  (GET)
        //  URL     : /Walks/CreateAuto
        //  Descr   : Create Walk form
        // --------------------------------------------------------------------------------------
        public ActionResult CreateAuto()
        {
            Walk oWalk = new Walk();
            var oAssociatedFileTypes = this.repository.GetAssociatedFileTypes();
            var oWalkTypes = this.repository.GetWalkTypes();

            ViewData["WalkTypes"] = new SelectList(oWalkTypes, "WalkTypeString", "WalkTypeString");
            ViewData["Associated_File_Types"] = new SelectList(oAssociatedFileTypes, "Walk_AssociatedFile_Type1", "Walk_AssociatedFile_Type1").ToList();
            ViewData["Model"] = oWalk;

            return this.View();

        }


        // -------------------------------------------------------------------------------------
        //  Function: CreateAuto  (POST)
        //  URL     : /Walks/CreateAuto
        //  Descr   : Create Walk form
        // --------------------------------------------------------------------------------------
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateAuto(Walk walk)
        {
            // ----This should really be done by a customised Model Binder-----------
            Walk oWalk = new Walk();
            WalkingStick.FillWalkFromFormVariables(ref oWalk, Request.Form);
            int iWalkID = this.repository.AddWalk(oWalk);

            // ---Add hill ascents-----------------
            List<HillAscent> arHillAscents = WalkingStick.FillHillAscentsFromFormVariables(iWalkID, this.Request.Form);
            repository.AddWalkSummitsVisited(arHillAscents);

            // ---Add the associated files-----
            // --TODO - this needs to change to pull files from disk rather than form variable
            List<Walk_AssociatedFile> arWalkAssociatedFiles = WalkingStick.FillWalkAssociatedFilesByExaminingDirectory(
                iWalkID,
                this.Request.Form,
                this.Server.MapPath("~/Content/images/").Replace("\\", "/"));
            repository.AddWalkAssociatedFiles(arWalkAssociatedFiles);

            //---Redirect to the newly created walk to continue editing
            return RedirectToAction("Edit", new { id = iWalkID });
        }



        // -------------------------------------------------------------------------------------
        //  Function: Edit  (POST)
        //  URL     : /Walks/Edit/Id
        //  Descr   : 1. Get the existing id into a walk object
        //            2. Based on the form variables, update the walk object and submit chnages
        // --------------------------------------------------------------------------------------
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, FormCollection formValues)
        {
            Walk oWalk = this.repository.GetWalkDetails(id);
            repository.UpdateWalkDetails(oWalk, Request.Form, Server.MapPath("/"));

            // ---update any markers created by ajax call with walk id, and add any marker observations----------------
            // iRetval = _repository.AssociateMarkersWithWalk(Request.Form, iWalkID)

            return Redirect(formValues["previousUrl"]);

        }

#endregion

#region "AJAX actions"

        // -----------------------------------------------------------------------------------------------------
        //  CreateMarker
        //  Server side of AJAX call, using JSON as data format, to insert a new marker.
        // -------------------------------------
        public JsonResult CreateMarker(string mtitle, string mdesc, string mdate, int mhillid = 0, string mgps = "", int mwalkid = 0)
        {

            var oNewMarker = new Marker { MarkerTitle = mtitle, Location_Description = mdesc };
            try
            {
                oNewMarker.DateLeft = DateTime.Parse((mdate + " 00:00:00"));
            }
            catch (Exception)
            {
                oNewMarker.DateLeft = DateTime.Now;
            }

            oNewMarker.GPS_Reference = mgps;
            if ((mhillid != 0))
            {
                oNewMarker.Hillnumber = (short)mhillid;
            }

            if ((mwalkid != 0))
            {
                oNewMarker.WalkID = mwalkid;
            }

            oNewMarker.Status = "Left - Not yet found again";
            repository.CreateMarker(oNewMarker);
            // ----Have to explicitly allow GET requests otherwise there is no callback------------------------

            return Json(new { markerid = oNewMarker.MarkerID }, JsonRequestBehavior.AllowGet);
        }


        // ------------------------------------------------------------------------------------------------
        //  MarkerSuggestions
        //  Used as the AJAX server side for an autocomplete function on a client side textbox
        // -----------------------
        public JsonResult MarkerSuggestions(string term)
        {

            var markeroptions = new List<AutocompleteSuggestionOption>();
            var IQMarkers = repository.FindMarkersByNameLike(term);

            foreach (Marker item in IQMarkers)
            {
                markeroptions.Add(new AutocompleteSuggestionOption
                {
                    label = WalkingStick.FormatMarkerAsLine(item) + ("|" + (item.MarkerID.ToString().Trim())),
                    value = WalkingStick.FormatMarkerAsLine(item) + ("|" + (item.MarkerID.ToString().Trim()))
                });

            }

            return Json(markeroptions, JsonRequestBehavior.AllowGet);
        }


        // ------------------------------------------------------------------------------------------------
        //  WalkSuggestions
        //  Used as the AJAX server side for an autocomplete function on a client side textbox - Create Marker
        // -----------------------
        public JsonResult WalkSuggestions(string term)
        {
            var IQWalks = repository.FindWalksByTitleLike(term);

            var suggestedWalks = new List<AutocompleteSuggestionOption>();
            foreach (Walk oWalk in IQWalks)
            {
                var walkTitle = WalkingStick.FormatWalkAsLine(oWalk);
                suggestedWalks.Add(new AutocompleteSuggestionOption { label = walkTitle, value = walkTitle + " | " + oWalk.WalkID });
            }
            return Json(suggestedWalks, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckFileInWebrootJSON(string imagepath)
        {
            string strPathToRoot = Server.MapPath("~/Content/images/");
            string strFullPathToFile = strPathToRoot + imagepath;

            bool bIsInPath = System.IO.File.Exists(strFullPathToFile);

            var oRes = new { isinpath = bIsInPath.ToString() };

            return Json(oRes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateAutoCheckFiles(string reldir)
        {
            string strPathToRoot = Server.MapPath("~/Content/images/");
            string strFullPathToNamePrefix = strPathToRoot + reldir;
            int iLoc = strFullPathToNamePrefix.LastIndexOf('\\');
            string strFullPathToDir = strFullPathToNamePrefix.Substring(0, iLoc);
            
            bool bIsInPath = System.IO.Directory.Exists(strFullPathToDir);
            if (!bIsInPath)
            {
                var oRes = new { error = "The directory could not be found or does not exist in the website root" };
                return Json(oRes, JsonRequestBehavior.AllowGet);
            }

            //-----Are there any files in the directory matching the specified name prefix?----

            string[] filesindir;

            try
            {
                filesindir = Directory.GetFiles(strFullPathToDir, strFullPathToNamePrefix.Substring(iLoc + 1, strFullPathToNamePrefix.Length - iLoc - 1) + "*");
            }
            catch (Exception e)
            {
                var oRes = new { error = "An error occurred when checking directory" + e.Message };
                return Json(oRes, JsonRequestBehavior.AllowGet);
            }
            
             if (filesindir.Length==0)
            {
                var oRes = new { error = "No files matching the name prefix were found in the directory." };
                return Json(oRes, JsonRequestBehavior.AllowGet);
            }else
            {
                var oRes = new { error = "" };
                return Json(oRes, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Called from Create walk and Edit walk jQuery UI autocomplete widget
        /// </summary>
        /// <param name="term">at least two characters of the walk area</param>
        /// <param name="options"></param>
        /// <returns>JSON formatted results</returns>
        public JsonResult WalkAreaSuggestions(string term, string options="")
        {
            var IQWalkAreas = this.repository.FindWalkAreasByNameLike(term);

            var areaoptions = new List<AutocompleteSuggestionOption>();

            foreach (Area item in IQWalkAreas)
            {
                areaoptions.Add(new AutocompleteSuggestionOption { label = WalkingStick.FormatWalkAreaAsLine(item), value = WalkingStick.FormatWalkAreaAsLine(item) + " | " + item.Arearef });
            }

            var myjson=  Json(areaoptions, JsonRequestBehavior.AllowGet);

            return myjson;
        }


        // ------------------------------------------------------------------------------------------------
        //  CheckImages
        //  Used as the AJAX server side to check that images are present in specified directory
        // -----------------------
        public JsonResult CheckImages(string imagepath, string options="") 
        {
            string strAtWork = "False";
            if (SessionSingleton.Current.UsageLocation == WalkingConstants.AT_WORK)
            {
                strAtWork = "True";
            }
    
            imagepath = imagepath.Replace("\\", "/");
            int iLoc;

            try {
                iLoc = imagepath.LastIndexOf("/", StringComparison.Ordinal);
            }
            catch (Exception) {
                iLoc = 0;
            }

            string strPath = imagepath.Substring(0, iLoc);
        

            string strRootPath = Server.MapPath("~/Content/images/").Replace("\\", "/");

            // -----Check that the path specified is valid------------------------
            if (!WalkingStick.DetermineIfDirectoryExists(strRootPath + strPath)) 
            {
                ViewData["checkresults"] = "{\"Error\" | \"Directory Not Found.\"}";
                return Json(new { Error = "Directory [" + strRootPath + strPath + "] Not Found." },JsonRequestBehavior.AllowGet);
            }


            // ----Now check that images are found in this directory------------------
            var oResults = WalkingStick.CheckFilesInDirectory(VirtualPathUtility.ToAbsolute("~/"),strPath, imagepath.Substring((iLoc + 1), ((imagepath.Length - iLoc) 
                                - 1)), ref strRootPath, bool.Parse(strAtWork));
       
            return Json(oResults, JsonRequestBehavior.AllowGet);
        }


        public JsonResult HillSuggestions(string term, string areaid="")
        {
            var hillsuggestions = new List<AutocompleteSuggestionOption>();

            IQueryable<Hill> IQHillsAboveHeight;

            if (areaid.Equals("") && areaid.Length <2)
            {
                IQHillsAboveHeight = this.repository.FindHillsByNameLike(term);
            }
            else
            {
                IQHillsAboveHeight = this.repository.FindHillsInAreaByNameLike(term, areaid);
            }

            foreach (Hill item in IQHillsAboveHeight)
            {
                var optionlabel = WalkingStick.FormatHillSummaryAsLine(item);
                var optionvalue = optionlabel + "|" + item.Hillnumber;
   
                hillsuggestions.Add(new AutocompleteSuggestionOption { label = optionlabel, value = optionvalue});
            }

            return Json(hillsuggestions, JsonRequestBehavior.AllowGet);
        }

#endregion

    }
}
