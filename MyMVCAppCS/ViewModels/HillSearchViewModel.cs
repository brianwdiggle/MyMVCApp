
namespace MyMVCAppCS.ViewModels
{
    using System.Web.Mvc;

    public class HillSearchViewModel
    {
        public string SearchHillTitle { get; set; }

        public float SearchHeight { get; set; }

        public SelectList SearchHeightGtLtList { get; set; }
        public bool SearchHeightGtLt { get; set; }

        public int NumberOfAscents { get; set; }
        public SelectList NumberOfAscentsGtLtEqList { get; set; }
        public string NumberOfAscentsGtLtEq { get; set; }

        public string FirstClimbedDate { get; set; }
        public SelectList FirstClimbedDateGtLtList { get; set; }
        public string FirstClimbedDateGtLt { get; set; }
    }
}