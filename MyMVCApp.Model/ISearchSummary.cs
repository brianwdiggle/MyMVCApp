
using System.Collections.Generic;


namespace MyMVCAppCS.Models
{
    public interface ISearchSummary
    {
        string SummariseRequestedSearch(List<SearchTerm> searchTerms);
    }
}