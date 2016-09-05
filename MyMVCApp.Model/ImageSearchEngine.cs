

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;
    using System.Collections.Generic;

    using MyMVCApp.DAL;

    public class ImageSearchEngine
    {
        private readonly ImageSelector imageSelector;

        private readonly ISearchFormParser imageSearchFormParser;

        private readonly ISearchSummary imageSearchSummary;

        public ImageSearchEngine(ImageSelector imageSelector)
        {
            this.imageSelector = imageSelector;
            this.imageSearchFormParser = new ImageSearchFormParser();
            this.imageSearchSummary = new ImageSearchSummary();
        }

        public ImageSearchResults PerformSearch(List<Walk_AssociatedFile> sourceImages, NameValueCollection formCollection)
        {
            List<SearchTerm> searchTerms = this.imageSearchFormParser.ParseSearchForm(formCollection);

            string searchSummary = this.imageSearchSummary.SummariseRequestedSearch(searchTerms);

            List<Walk_AssociatedFile> resultImages = sourceImages;

            foreach(SearchTerm searchTerm in searchTerms)
            {
                resultImages = this.imageSelector.SelectImages(searchTerm, resultImages);
            }

            var searchResults = new ImageSearchResults { ImagesFound = resultImages, SearchSummary = searchSummary };

            return searchResults;
        }
    }
}