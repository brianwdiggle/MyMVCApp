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
            return this.View();
        }

        public ActionResult HillSearch()
        {
            return this.View();
        }

        public ActionResult AscentSearch()
        {
            return this.View();
        }
    }
}
