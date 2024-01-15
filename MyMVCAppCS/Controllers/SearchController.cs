using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyMVCAppCS.Controllers
{
    using System.Web;

    using MyMVCApp.DAL;

    using MyMVCAppCS.Models;
    using MyMVCAppCS.ViewModels;

    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private IWalkingRepository repository;

        public SearchController()
        {
            this.repository = new SqlWalkingRepository(SessionSingleton.Current.ConnectionString);
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult WalkSearch()
        {
            var searchViewModel =new WalkSearchViewModel();
            return View(searchViewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult WalkSearch(WalkSearchViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(searchViewModel);
            }
         
            List<Walk> allWalks = this.repository.FindAllWalks().ToList();

            var walkSearchEngine = new WalkSearchEngine(new WalkSelector());

            WalkSearchResults searchResults = walkSearchEngine.PerformSearch(allWalks, Request.Form);

            searchViewModel.WalksFound = searchResults.WalksFound.ToList();
            searchViewModel.SearchSummary = searchResults.SearchSummary;
            searchViewModel.WalkResultsAvailable = true;

            return this.View(searchViewModel);
        }

        [HttpGet]
        public ActionResult ImageSearch()
        {
            var imageSearchViewModel = new ImageSearchViewModel();
 
            return this.View(imageSearchViewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ImageSearch(ImageSearchViewModel imageSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(imageSearchViewModel);
            }

            var imageSearchEngine = new ImageSearchEngine(new ImageSelector());

            ViewBag.ImagesDirectory = Server.MapPath("~/Content/images/");
            ViewBag.ApplicationRoot = VirtualPathUtility.ToAbsolute("~/");
            List<Walk_AssociatedFile> allImages;
            
            if (Request.Form["SearchImageCaption"].Length > 0 )
            {
                allImages = this.repository.GetAllImagesWithCaptions().ToList();
            } else {
                allImages = this.repository.GetAllImages().ToList();
            }

            ImageSearchResults searchResults = imageSearchEngine.PerformSearch(allImages, Request.Form);

            imageSearchViewModel.ImageResultsAvailable = true;

            imageSearchViewModel.ImagesFound = searchResults.ImagesFound.ToList();
            imageSearchViewModel.SearchSummary = searchResults.SearchSummary;
      
            return this.View(imageSearchViewModel);
        }


        public ActionResult MarkerSearch()
        {
            var searchViewModel = new MarkerSearchViewModel();
            return View(searchViewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult MarkerSearch(MarkerSearchViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(searchViewModel);
            }

            List<Marker> allMarkers = this.repository.FindAllMarkers().ToList();

            var markerSearchEngine = new MarkerSearchEngine(new MarkerSelector());

            MarkerSearchResults searchResults = markerSearchEngine.PerformSearch(allMarkers, Request.Form);

            searchViewModel.MarkersFound = searchResults.MarkersFound.ToList();
            searchViewModel.SearchSummary = searchResults.SearchSummary;
            searchViewModel.MarkerResultsAvailable = true;

            int iShowMap = 0;

            ///----Prepare data about markers to be used on the map
            List<MapMarker> lstMarkerMarkers = new List<MapMarker>();
            foreach (Marker oMarker in searchViewModel.MarkersFound)
            {
                if (oMarker.GPS_Reference.Trim() != "")
                {
                    MapMarker oMM = new MapMarker
                    {
                        OSMap10 = oMarker.GPS_Reference,
                        popupText = WalkingStick.MarkerPopup(oMarker, Request.Url.GetLeftPart(System.UriPartial.Authority))
                    };
                    lstMarkerMarkers.Add(oMM);
                    iShowMap = 1;
                }else if (oMarker.Hill !=null && (oMarker.Hill.Gridref10 !="" || oMarker.Hill.Gridref !=""))
                {
                    MapMarker oMM = new MapMarker
                    {
                        OSMap10 = WalkingStick.FivePacesEastFromSummit(oMarker.Hill),
                        popupText = WalkingStick.MarkerPopup(oMarker, Request.Url.GetLeftPart(System.UriPartial.Authority))
                    };
                    lstMarkerMarkers.Add(oMM);
                    iShowMap = 1;
                }
            }

            ViewData["ShowMap"] = iShowMap;
            ViewData["MarkerMarkers"] = lstMarkerMarkers;


            return this.View(searchViewModel);
        }



        public ActionResult HillSearch()
        {
            var searchViewModel = new HillSearchViewModel();
            return View(searchViewModel);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult HillSearch(HillSearchViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(searchViewModel);
            }

            searchViewModel.HillsFound = this.repository.HillSearch(Request.Form);

            searchViewModel.SearchSummary = "This will be the search summary";
            searchViewModel.HillResultsAvailable = true;

            int iShowMap = 0;

            ///----Prepare data about markers to be used on the map
            List<MapMarker> lstHillMarkers = new List<MapMarker>();
            foreach (Hill oHill in searchViewModel.HillsFound)
            {
                if (oHill.Gridref10.Trim() != "")
                {
                    MapMarker oMM = new MapMarker
                    {
                        OSMap10 = oHill.Gridref10,
                        popupText = WalkingStick.HillPopup(oHill)
                    };
                    lstHillMarkers.Add(oMM);
                    iShowMap = 1;
                }
                else if (oHill != null && ( oHill.Gridref != ""))
                {
                    MapMarker oMM = new MapMarker
                    {
                        OSMap10 = WalkingStick.GridrefToGridRef10(oHill.Gridref),
                        popupText = WalkingStick.HillPopup(oHill)
                    };
                    lstHillMarkers.Add(oMM);
                    iShowMap = 1;
                }
            }

            ViewData["ShowMap"] = iShowMap;
            ViewData["MarkerMarkers"] = lstHillMarkers;

            return this.View(searchViewModel);
        }


        public ActionResult AscentSearch()
        {
            return this.View();
        }
    }
}
