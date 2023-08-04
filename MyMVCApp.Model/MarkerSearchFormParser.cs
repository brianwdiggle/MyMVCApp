using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;

    public class MarkerSearchFormParser : ISearchFormParser
    {

        public List<SearchTerm> ParseSearchForm(NameValueCollection formCollection)
        {
            var searchTerms = new List<SearchTerm>();

            if (!string.IsNullOrEmpty(formCollection["SearchMarkerTitle"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchMarkerTitle"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.Title));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchMarkerDescription"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchMarkerDescription"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.Description));
            }

    

            if (!string.IsNullOrEmpty(formCollection["SearchMarkerHill"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchMarkerHill"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.MetresAscent));
            }

    
            if (!string.IsNullOrEmpty(formCollection["DateFromDay"]) && !string.IsNullOrEmpty(formCollection["DateFromYear"]))
            {
                string stringDate = formCollection["DateFromDay"] + " " + formCollection["DateFromMonth"] + " " + formCollection["DateFromYear"];
                DateTime fromDate = DateTime.Parse(stringDate);
                SearchOperator searchOperator = SearchOperator.GreaterThan;
                searchTerms.Add(new SearchTerm(fromDate, SearchTermSelection.MarkerDateLeftFrom, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["DateToDay"]) && !string.IsNullOrEmpty(formCollection["DateToYear"]))
            {
                string stringDate = formCollection["DateToDay"] + " " + formCollection["DateToMonth"] + " " + formCollection["DateToYear"];
                DateTime toDate = DateTime.Parse(stringDate);
                SearchOperator searchOperator = SearchOperator.LessThan;
                searchTerms.Add(new SearchTerm(toDate, SearchTermSelection.MarkerDateLeftTo, searchOperator));
            }

            return searchTerms;
        }


    }
}