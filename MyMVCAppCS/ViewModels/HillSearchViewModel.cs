
namespace MyMVCAppCS.ViewModels
{
    using MyMVCApp.DAL;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    public class HillSearchViewModel
    {

        public HillSearchViewModel()
        {

        }

        public HillSearchViewModel(IQueryable<Class> hillClasses, IQueryable<Area> hillAreas)
        {
            this.HillClassList = this.hillClassesAsSelectList(hillClasses);
            this.HillAreaList = this.hillAreasAsSelectList(hillAreas);
            //---Populate the drop down lists-------------------
            //this.FieldCombinationList = new SelectList(new string[] { "AND", "OR" }.Select(x => new { value = x, text = x }), "value", "text", "AND");
            this.FirstClimbedDateFromMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.FirstClimbedDateToMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.SearchHeightGtLtList = new SelectList(new string[] { "", ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", "");
            this.NumberOfAscentsGtLtEqList = new SelectList(new string[] { "", ">", "<", "=" }.Select(x => new { value = x, text = x }), "value", "text", "");
            this.OrderByList = new SelectList(new string[] { "Name", "Height" }.Select(x => new { value = x, text = x }), "value", "text", "");
            this.OrderByAscDescList = new SelectList(new string[] { "Asc", "Desc" }.Select(x => new { value = x, text = x }), "value", "text", "");
        }

        // Form fields-------------

        // Hill name

        public string SearchHillName { get; set; }

        // Height (metres)
        [Range(0, 8850)]
        public float? SearchHeight { get; set; }
        public SelectList SearchHeightGtLtList { get; set; }
        public bool? SearchHeightGtLt { get; set; }

        // Number of ascents
        [Range(0,100)]
        public int? NumberOfAscents { get; set; }
        public SelectList NumberOfAscentsGtLtEqList { get; set; }
        public string NumberOfAscentsGtLtEq { get; set; }

        // First climbed date from
        [StringLength(2, ErrorMessage = "Day must be no more than 2 characters long")]
        [Range(0,31, ErrorMessage = "Day must be numeric value between 0 and 31")]
        public string FirstClimbedDateFromDay { get; set; }
        public SelectList FirstClimbedDateFromMonthList { get; set; }
        public string FirstClimbedDateFromMonth { get; set; }

        [Range(1968, 2968, ErrorMessage = "First climbed year cannot be before I was born :-)")]
        public string FirstClimbedDateFromYear { get; set; }

        // First climbed date from
        [StringLength(2, ErrorMessage = "Day must be no more than 2 characters long")]
        [Range(0, 31, ErrorMessage = "Day must be numeric value between 0 and 31")]
        public string FirstClimbedDateToDay { get; set; }
        public SelectList FirstClimbedDateToMonthList { get; set; }
        public string FirstClimbedDateToMonth { get; set; }

        [Range(1968, 2968, ErrorMessage = "First climbed year cannot be before I was born :-)")]
        public string FirstClimbedDateToYear { get; set; }

        //Hill area
        public string HillArea { get; set; }
        public SelectList HillAreaList { get; set; }

        //Hill Class
        public string HillClass { get; set; }
        public SelectList HillClassList { get; set; }

        // Show climbed, unclimbed, all
        public string ShowOption {  get; set; }

        // Order by
        public string OrderBy { get; set; }
        public SelectList OrderByList { get; set; }


        // Order by Asc/Desc
        public string OrderByAscDesc { get; set; }
        public SelectList OrderByAscDescList { get; set; }

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

            SelectList hillClassesSL = new SelectList(items, "Value", "Text");

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