using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;

    public class WalkSearchFormParser : ISearchFormParser
    {

        public List<SearchTerm> ParseSearchForm(NameValueCollection formCollection)
        {
            var searchTerms = new List<SearchTerm>();

            if (!string.IsNullOrEmpty(formCollection["SearchTitle"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchTitle"]);      
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.Title));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchWalkDescription"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchWalkDescription"]);      
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.Description));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchImageCaptions"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchImageCaptions"]);      
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.ImageCaption));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchHillAscended"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchHillAscended"]);     
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.HillAscended));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchLength"]))
            {
                double lengthVal = double.Parse(formCollection["SearchLength"]);
                SearchOperator searchOperator;
                if (formCollection["LengthGtLt"].Equals(">"))
                {
                    searchOperator = SearchOperator.GreaterThan;
                }
                else
                {
                    searchOperator = SearchOperator.LessThan;
                }
                searchTerms.Add(new SearchTerm(lengthVal, SearchTermSelection.WalkLength, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchAscent"]))
            {
                int ascentVal = Int32.Parse(formCollection["SearchAscent"]);
                SearchOperator searchOperator;
                if (formCollection["AscentGtLt"].Equals(">"))
                {
                    searchOperator = SearchOperator.GreaterThan;
                }
                else
                {
                    searchOperator = SearchOperator.LessThan;
                }
                searchTerms.Add(new SearchTerm(ascentVal, SearchTermSelection.MetresAscent, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchOverallSpeed"]))
            {
                double speedVal = double.Parse(formCollection["SearchOverallSpeed"]);
                SearchOperator searchOperator;
                if (formCollection["OverallSpeedGtLt"].Equals(">"))
                {
                    searchOperator = SearchOperator.GreaterThan;
                }
                else
                {
                    searchOperator = SearchOperator.LessThan;
                }
                searchTerms.Add(new SearchTerm(speedVal, SearchTermSelection.OverallSpeed, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["DateFromDay"]) && !string.IsNullOrEmpty(formCollection["DateFromYear"]))
            {
                string stringDate = formCollection["DateFromDay"] + " " + formCollection["DateFromMonth"] + " " + formCollection["DateFromYear"];
                DateTime fromDate = DateTime.Parse(stringDate);
                SearchOperator searchOperator = SearchOperator.GreaterThan;
                searchTerms.Add(new SearchTerm(fromDate, SearchTermSelection.WalkDateFrom, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["DateToDay"]) && !string.IsNullOrEmpty(formCollection["DateToYear"]))
            {
                string stringDate = formCollection["DateToDay"] + " " + formCollection["DateToMonth"] + " " + formCollection["DateToYear"];
                DateTime toDate = DateTime.Parse(stringDate);
                SearchOperator searchOperator = SearchOperator.LessThan;
                searchTerms.Add(new SearchTerm(toDate, SearchTermSelection.WalkDateTo, searchOperator));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchDurationHours"]) && !string.IsNullOrEmpty(formCollection["SearchDurationMins"]))
            {
                int durationHour = Int32.Parse(formCollection["SearchDurationHours"]);
                int durationMins = Int32.Parse(formCollection["SearchDurationMins"]);

                DateTime walkDuration = new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day, durationHour, durationMins,0 );

                SearchOperator searchOperator;
                if (formCollection["DurationGtLt"].Equals(">"))
                {
                    searchOperator = SearchOperator.GreaterThan;
                }
                else
                {
                    searchOperator = SearchOperator.LessThan;
                }
                searchTerms.Add(new SearchTerm(walkDuration, SearchTermSelection.WalkDuration, searchOperator));
            }

            return searchTerms;
        }

       
    }
}