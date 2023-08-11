using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Text;

    public class MarkerSearchSummary : ISearchSummary
    {
        public string SummariseRequestedSearch(List<SearchTerm> searchTerms)
        {
            StringBuilder oStringBuilder = new StringBuilder();

            oStringBuilder.AppendLine("<p><strong>You searched for: </strong>");
            foreach (SearchTerm searchTerm in searchTerms)
            {
                switch (searchTerm.SearchType)
                {
                    case SearchTermSelection.MetresAscent:
                        oStringBuilder.AppendLine("Name of hill on which marker was left contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.Description:
                        oStringBuilder.AppendLine("marker description contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.MarkerName:
                        oStringBuilder.AppendLine("Marker title contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.MarkerDateLeftFrom:
                        oStringBuilder.AppendLine("Marker left after <em>" + searchTerm.DateTimeVal.ToString("dd MMMM yyyy") + "</em><br/>");
                        break;
                    case SearchTermSelection.MarkerDateLeftTo:
                        oStringBuilder.AppendLine("Marker left before <em>" + searchTerm.DateTimeVal.ToString("dd MMMM yyyy") + "</em><br/>");
                        break;
                }
            }
            return oStringBuilder.ToString();
        }
    }
}