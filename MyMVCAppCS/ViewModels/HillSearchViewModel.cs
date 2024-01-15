
namespace MyMVCAppCS.ViewModels
{
    using MyMVCApp.DAL;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class HillSearchViewModel
    {

        public HillSearchViewModel()
        {
            //---Populate the drop down lists-------------------
            //this.FieldCombinationList = new SelectList(new string[] { "AND", "OR" }.Select(x => new { value = x, text = x }), "value", "text", "AND");
            //this.DateFromMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            //this.DateToMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
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
        public string[] ShowRadioGroup = new[] { "All", "Unclimbed", "Climbed" };

        // Results below
        public string SearchSummary { get; set; }
        public bool HillResultsAvailable { get; set; }

        public IEnumerable<Hill> HillsFound;

    }
}