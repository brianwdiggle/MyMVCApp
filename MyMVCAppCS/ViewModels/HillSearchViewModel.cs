
namespace MyMVCAppCS.ViewModels
{
    using DotNetOpenAuth.Messaging;
    using MyMVCApp.DAL;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class HillSearchViewModel
    {

        public HillSearchViewModel(IQueryable<Class> hillClasses, IQueryable<Area> hillAreas)
        {
            this.HillClassList = this.hillClassesAsSelectList(hillClasses);
            this.HillAreaList = this.hillAreasAsSelectList(hillAreas);
            //---Populate the drop down lists-------------------
            //this.FieldCombinationList = new SelectList(new string[] { "AND", "OR" }.Select(x => new { value = x, text = x }), "value", "text", "AND");
            this.FirstClimbedDateFromMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.FirstClimbedDateToMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.SearchHeightGtLtList = new SelectList(new string[] { ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", ">");
            this.NumberOfAscentsGtLtEqList = new SelectList(new string[] { ">", "<", "=" }.Select(x => new { value = x, text = x }), "value", "text", ">");
        }

        // Form fields-------------

        // Hill name
        public string SearchHillName { get; set; }

        // Height (metres)
        public float SearchHeight { get; set; }
        public SelectList SearchHeightGtLtList { get; set; }
        public bool SearchHeightGtLt { get; set; }

        // Number of ascents
        public int NumberOfAscents { get; set; }
        public SelectList NumberOfAscentsGtLtEqList { get; set; }
        public string NumberOfAscentsGtLtEq { get; set; }

        // First climbed date from
        public string FirstClimbedDateFromDay { get; set; }
        public SelectList FirstClimbedDateFromMonthList { get; set; }
        public string FirstClimbedDateFromMonth { get; set; }
        public string FirstClimbedDateFromYear { get; set; }

        // First climbed date from
        public string FirstClimbedDateToDay { get; set; }
        public SelectList FirstClimbedDateToMonthList { get; set; }
        public string FirstClimbedDateToMonth { get; set; }
        public string FirstClimbedDateToYear { get; set; }

        //Hill area
        public string HillArea { get; set; }
        public SelectList HillAreaList { get; set; }

        //Hill Class
        public string HillClass { get; set; }
        public SelectList HillClassList { get; set; }

        // Show climbed, unclimbed, all
        public string ShowOption {  get; set; }

        // Results below
        public string SearchSummary { get; set; }
        public bool HillResultsAvailable { get; set; }

        public IEnumerable<Hill> HillsFound;

        private SelectList hillClassesAsSelectList(IQueryable<Class> hillClasses)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var hillClass in hillClasses)
            {
                if (hillClass.ClassType != null)
                {
                    SelectListItem newItem = new SelectListItem { Text = hillClass.Classname + "(" + hillClass.ClassType + ")", Value = hillClass.Classref };
                    items.Add(newItem);
                }
                else
                {
                    SelectListItem newItem = new SelectListItem { Text = hillClass.Classname, Value = hillClass.Classref };
                    items.Add(newItem);
                }
            }

            SelectList hillClassesSL = new SelectList(items, "Value", "Text", 1);

            return hillClassesSL;
        }

        private SelectList hillAreasAsSelectList(IQueryable<Area> hillAreas)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var hillArea in hillAreas)
            {
                SelectListItem newItem = new SelectListItem { Text = hillArea.Areaname, Value = hillArea.Arearef };
                items.Add(newItem);
            }

            SelectList hillAreasSL = new SelectList(items, "Value", "Text", 1);

            return hillAreasSL;
        }
    }
}