
namespace MyMVCAppCS.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    using MyMVCApp.DAL;

    public class WalkSearchViewModel
    {
        public WalkSearchViewModel()
        {
            this.FieldCombinationList = new SelectList(new string[] { "AND", "OR"}.Select(x => new { value=x, text=x}), "value", "text", "AND") ;
            this.LengthGtLtList = new SelectList(new string[] { ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", ">");
            this.AscentGtLtList = new SelectList(new string[] { ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", ">");
            this.DurationGtLtList = new SelectList(new string[] { ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", ">");
            this.OverallSpeedGtLtList = new SelectList(new string[] { ">", "<" }.Select(x => new { value = x, text = x }), "value", "text", ">");
            this.DateFromMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.DateToMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
 
        }

        public string SearchTitle { get; set; }

        public string SearchWalkDescription { get; set; }

        public string SearchImageCaptions { get; set; }

        public string SearchHillAscended { get; set; }

        public SelectList FieldCombinationList { get; set; }
        public string FieldCombination { get; set; }

        // Length search
        public SelectList LengthGtLtList { get; set; }
        public string LengthGtLt { get; set; }
        public string SearchLength { get; set; }

        // Ascent search
        public SelectList AscentGtLtList { get; set; }
        public string AscentGtLt { get; set; }
        public string SearchAscent { get; set; }

        // Duration search
        public SelectList DurationGtLtList { get; set; }
        public string DurationGtLt { get; set; }
        public string SearchDurationHours { get; set; }
        public string SearchDurationMins { get; set; }

        // Overall speed search
        public SelectList OverallSpeedGtLtList { get; set; }
        public string OverallSpeedGtLt { get; set; }
        public string SearchOverallSpeed { get; set; }

        // Date search
        public string DateFromDay { get; set; }
        public SelectList DateFromMonthList { get; set; }
        public string DateFromMonth { get; set; }
        public string DateFromYear { get; set; }

        public string DateToDay { get; set; }
        public SelectList DateToMonthList { get; set; }
        public string DateToMonth { get; set; }
        public string DateToYear { get; set; }

     
        // Results below

        public string SearchSummary { get; set; }

        public bool WalkResultsAvailable { get; set; }
        public List<Walk> WalksFound;

       
    }
}