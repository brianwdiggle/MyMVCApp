namespace MyMVCAppCS.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    using MyMVCApp.DAL;

    public class MarkerSearchViewModel
    {
        public MarkerSearchViewModel()
        {
            this.FieldCombinationList = new SelectList(new string[] { "AND", "OR" }.Select(x => new { value = x, text = x }), "value", "text", "AND");
            this.DateFromMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");
            this.DateToMonthList = new SelectList(new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select(x => new { value = x, text = x }), "value", "text", "Jan");

        }

        public string SearchMarkerTitle { get; set; }

        public string SearchMarkerDescription { get; set; }

        public string SearchMarkerHill { get; set; }

        public SelectList FieldCombinationList { get; set; }
        public string FieldCombination { get; set; }

        // Length search
        public SelectList LengthGtLtList { get; set; }
        public string LengthGtLt { get; set; }
        public string SearchLength { get; set; }

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

        public bool MarkerResultsAvailable { get; set; }
        public List<Marker> MarkersFound;


    }
}