using System.Linq;
using System.Web.Mvc;

namespace MyMVCAppCS.Controllers
{
    using MyMVCApp.DAL;
    using MyMVCAppCS.Models;

    public class HillAscentController : Controller
    {
        private readonly IWalkingRepository repository;

        private const int HILLASCENTS_PAGE_SIZE = 50;
        private const int MAX_PAGINATION_LINKS = 10;

        public HillAscentController()
        {
            this.repository = new SqlWalkingRepository(SessionSingleton.Current.ConnectionString);
        }

        /// <summary>
        /// Display a list of paginated list of hill ascents
        /// </summary>
        /// <param name="orderBy">Field to order by - Date/Hill/Metres/Walk</param>
        /// <param name="page">Current page number in list</param>
        /// <returns></returns>
        public ActionResult Index(string orderBy, int page= 1) 
        {
            IQueryable<HillAscent> iqHillAscents;

            // ---Use the walking repository to get a list of all the hill ascents----
            // ---Set up the ordering of the hill ascents------
            switch (orderBy)
            {
                case "DateAsc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderBy(ascent =>ascent.AscentDate).ThenBy(ascent =>ascent.AscentID);
                    this.ViewData["OrderAscDesc"] = "Asc";
                    this.ViewData["OrderBy"] = "Date";
                    break;
                case "DateDesc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderByDescending(ascent =>ascent.AscentDate).ThenByDescending(ascent =>ascent.AscentID);
                    this.ViewData["OrderBy"] = "Date";
                    this.ViewData["OrderAscDesc"] = "Desc";
                    break;
                case "HillAsc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderBy(ascent =>ascent.Hill.Hillname);
                    this.ViewData["OrderBy"] = "Hill";
                    this.ViewData["OrderAscDesc"] = "Asc";
                    break;
                case "HillDesc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderByDescending(ascent =>ascent.Hill.Hillname);
                    this.ViewData["OrderBy"] = "Hill";
                    this.ViewData["OrderAscDesc"] = "Desc";
                    break;
                case "MetresAsc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderBy(ascent =>ascent.Hill.Metres);
                    this.ViewData["OrderBy"] = "Metres";
                    this.ViewData["OrderAscDesc"] = "Asc";
                    break;
                case "MetresDesc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderByDescending(ascent =>ascent.Hill.Metres);
                    this.ViewData["OrderBy"] = "Metres";
                    this.ViewData["OrderAscDesc"] = "Desc";
                    break;
                case "WalkAsc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderBy(ascent =>ascent.Walk.WalkTitle);
                    this.ViewData["OrderBy"] = "Walk";
                    this.ViewData["OrderAscDesc"] = "Asc";
                    break;
                case "WalkDesc":
                    iqHillAscents = this.repository.GetAllHillAscents().OrderByDescending(ascent =>ascent.Walk.WalkTitle);
                    this.ViewData["OrderBy"] = "Walk";
                    this.ViewData["OrderAscDesc"] = "Desc";
                    break;
                default:
                    this.ViewData["OrderBy"] = "Date";
                    this.ViewData["OrderAscDesc"] = "Asc";
                    iqHillAscents = this.repository.GetAllHillAscents().OrderBy(ascent =>ascent.AscentDate).ThenBy(ascent =>ascent.AscentID);
                    break;
            }

            // ----Create a paginated list of the walks----------------
            var iqPaginatedAscents = new PaginatedListMVC<HillAscent>(iqHillAscents,page, HILLASCENTS_PAGE_SIZE, Url.Action("Index","HillAscent", new {OrderBy = ViewData["OrderBy"] + ViewData["OrderAscDesc"].ToString()}),MAX_PAGINATION_LINKS,"");
            return View(iqPaginatedAscents);
        }
    }
}
