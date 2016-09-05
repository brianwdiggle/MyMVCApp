using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Text;

    public class ImageSearchSummary : ISearchSummary
    {
        public string SummariseRequestedSearch(List<SearchTerm> searchTerms)
        {
            StringBuilder oStringBuilder = new StringBuilder();

            oStringBuilder.AppendLine("<p><strong>You searched for:</strong><br/>");

            foreach (SearchTerm searchTerm in searchTerms)
            {
                switch (searchTerm.SearchType)
                {
                    case SearchTermSelection.ImageCaption:
                        oStringBuilder.AppendLine(
                            "Image caption contains "
                            + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.ImageName:
                        oStringBuilder.AppendLine(
                            "Image name contains "
                            + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.MarkerName:
                        oStringBuilder.AppendLine(
                            "Image name contains "
                            + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                }
            }
            return oStringBuilder.ToString();
        }
    }
}