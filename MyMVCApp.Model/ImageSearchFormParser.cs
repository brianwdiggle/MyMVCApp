
using System.Collections.Generic;

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;

    public class ImageSearchFormParser :  ISearchFormParser
    {
        public List<SearchTerm> ParseSearchForm(NameValueCollection formCollection)
        {
            var searchTerms = new List<SearchTerm>();

            if (!string.IsNullOrEmpty(formCollection["SearchImageCaption"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchImageCaption"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.ImageCaption));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchImageName"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchImageName"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.ImageName));
            }

            if (!string.IsNullOrEmpty(formCollection["SearchMarkerName"]))
            {
                string[] searchWords = SearchTermGenerator.ReturnSearchTerms(formCollection["SearchMarkerName"]);
                searchTerms.Add(new SearchTerm(searchWords, SearchTermSelection.MarkerName));
            }

            return searchTerms;
        }
    }
}