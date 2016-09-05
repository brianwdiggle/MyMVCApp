

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;
    using System.Collections.Generic;

    using MyMVCApp.DAL;

    public class WalkSearchEngine
    {
        private readonly WalkSelector walkSelector;

        private readonly ISearchFormParser walkSearchFormParser;

        private readonly ISearchSummary walkSearchSummary;

        public WalkSearchEngine(WalkSelector walkSelector)
        {
            this.walkSelector = walkSelector;
            this.walkSearchFormParser = new WalkSearchFormParser();
            this.walkSearchSummary = new WalkSearchSummary();
        }

        public WalkSearchResults PerformSearch(List<Walk> sourceWalks, NameValueCollection formCollection)
        {
            List<SearchTerm> searchTerms = this.walkSearchFormParser.ParseSearchForm(formCollection);
            string searchSummary = this.walkSearchSummary.SummariseRequestedSearch(searchTerms);

            List<Walk> resultWalks = sourceWalks;

            foreach( SearchTerm searchTerm in searchTerms)
            {
                resultWalks = this.walkSelector.SelectWalks(searchTerm, resultWalks);
            }
            
            var searchResults = new WalkSearchResults { WalksFound = resultWalks, SearchSummary = searchSummary };

            return searchResults;
        }
    }
}