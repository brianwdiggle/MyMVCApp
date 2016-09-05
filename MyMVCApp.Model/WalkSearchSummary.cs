using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Text;

    public class WalkSearchSummary : ISearchSummary
    {
        public string SummariseRequestedSearch(List<SearchTerm> searchTerms)
        {
            StringBuilder oStringBuilder = new StringBuilder();

            oStringBuilder.AppendLine("<p><strong>You searched for:</strong><br/>");
            foreach (SearchTerm searchTerm in searchTerms)
            {
                switch (searchTerm.SearchType)
                {
                    case SearchTermSelection.HillAscended:
                        oStringBuilder.AppendLine("Hill ascended contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.Description:
                        oStringBuilder.AppendLine("walk description contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.ImageCaption:
                        oStringBuilder.AppendLine("Walk has image caption which contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.MetresAscent:
                        if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                        {
                            oStringBuilder.AppendLine("Ascent at least <em>" + searchTerm.IntVal.ToString() + "</em> metres<br/>");
                        }
                        else
                        {
                            oStringBuilder.AppendLine("Ascent less than <em>" + searchTerm.IntVal.ToString() + "</em> metres<br/>");                           
                        }
  
                        break;
                    case SearchTermSelection.OverallSpeed:
                        if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                        {
                            oStringBuilder.AppendLine("Overall speed at least <em>" + searchTerm.DoubleVal.ToString() + "</em> km/h<br/>");
                        }
                        else
                        {
                            oStringBuilder.AppendLine("Overall speed than <em>" + searchTerm.DoubleVal.ToString() + "</em> km/h<br/>");
                        }

                        break;
                    case SearchTermSelection.Title:
                        oStringBuilder.AppendLine("Walk title contains " + SearchTermGenerator.ReturnSearchTermSummary(searchTerm.StringVals, "AND") + "<br/>");
                        break;
                    case SearchTermSelection.WalkDateFrom:
                        oStringBuilder.AppendLine("Walk date after <em>" + searchTerm.DateTimeVal.ToString("dd MMMM yyyy") + "</em><br/>");
                        break;
                    case SearchTermSelection.WalkDateTo:
                        oStringBuilder.AppendLine("Walk date before <em>" + searchTerm.DateTimeVal.ToString("dd MMMM yyyy") + "</em><br/>");
                        break;

                    case SearchTermSelection.WalkDuration:
                        if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                        {
                            oStringBuilder.AppendLine("Duration at least <em>" + searchTerm.DateTimeVal.Hour.ToString()+ " hr " + searchTerm.DateTimeVal.Minute.ToString() + " min</em><br/>");
                        }
                        else
                        {
                            oStringBuilder.AppendLine("Duration less than <em>" + searchTerm.DateTimeVal.Hour.ToString()+ " hr " + searchTerm.DateTimeVal.Minute.ToString() + " min</em><br/>");
                        }

                        break;
                   case SearchTermSelection.WalkLength:
                        if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                        {
                            oStringBuilder.AppendLine("Length at least <em>" + searchTerm.DoubleVal.ToString() + "</em> km<br/>");
                        }
                        else
                        {
                            oStringBuilder.AppendLine("Length less than <em>" + searchTerm.DoubleVal.ToString() + "</em> km<br/>");
                        }

                        break;
                }
            }
            return oStringBuilder.ToString();
        }
    }
}