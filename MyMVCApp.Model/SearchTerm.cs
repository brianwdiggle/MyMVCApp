using System;


namespace MyMVCAppCS.Models
{

    // Todo: refactor to use dynamics?

    public class SearchTerm
    {
        public SearchTerm(int intval, SearchTermSelection walkSelectionType, SearchOperator searchOperator)
        {
            this.IntVal = intval;
            this.SearchType = walkSelectionType;
            this.SearchOperator = searchOperator;
        }

        public SearchTerm(string[] stringVals, SearchTermSelection walkSelectionType)
        {
            this.StringVals = stringVals;
            this.SearchType = walkSelectionType;
            this.SearchOperator = SearchOperator.Contains;
        }

        public SearchTerm(double doubleVal, SearchTermSelection walkSelectionType, SearchOperator searchOperator)
        {
            this.DoubleVal = doubleVal;
            this.SearchType = walkSelectionType;
            this.SearchOperator = searchOperator;
        }

        public SearchTerm(DateTime datetimeVal, SearchTermSelection walkSelectionType, SearchOperator searchOperator)
        {
            this.DateTimeVal = datetimeVal;
            this.SearchType = walkSelectionType;
            this.SearchOperator = searchOperator;
        }

        public SearchTermSelection SearchType { get; set; }

        public int IntVal { get; set; }

        public double DoubleVal { get; set; }

        public string[] StringVals { get; set; }

        public DateTime DateTimeVal { get; set; }

        public SearchOperator SearchOperator { get; set; }
    }
}