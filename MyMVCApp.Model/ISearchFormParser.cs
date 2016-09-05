
using System.Collections.Generic;


namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;

    public interface ISearchFormParser
    {
        List<SearchTerm> ParseSearchForm(NameValueCollection formCollection);
    }
}