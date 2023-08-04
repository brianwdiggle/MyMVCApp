namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;
    using System.Collections.Generic;

    using MyMVCApp.DAL;

    public class MarkerSearchEngine
    {
        private readonly MarkerSelector markerSelector;

        private readonly ISearchFormParser markerSearchFormParser;

        private readonly ISearchSummary markerSearchSummary;

        public MarkerSearchEngine(MarkerSelector markerSelector)
        {
            this.markerSelector = markerSelector;
            this.markerSearchFormParser = new MarkerSearchFormParser();
            this.markerSearchSummary = new MarkerSearchSummary();
        }

        public MarkerSearchResults PerformSearch(List<Marker> sourceMarkers, NameValueCollection formCollection)
        {
            List<SearchTerm> searchTerms = this.markerSearchFormParser.ParseSearchForm(formCollection);
            string searchSummary = this.markerSearchSummary.SummariseRequestedSearch(searchTerms);

            List<Marker> resultMarkers = sourceMarkers;

            foreach (SearchTerm searchTerm in searchTerms)
            {
                resultMarkers = this.markerSelector.SelectMarkers(searchTerm, resultMarkers);
            }

            var searchResults = new MarkerSearchResults { MarkersFound = resultMarkers, SearchSummary = searchSummary };

            return searchResults;
        }
    }
}