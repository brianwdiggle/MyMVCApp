using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Text;

    public static class SearchTermGenerator
    {
        public static string[] ReturnSearchTerms(string input)
        {
            // Todo: Add security stuff
            return
                input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(word => word.Trim().ToLower()).
                    ToArray();
        }

        public static string ReturnSearchTermSummary(string[] searchTerms, string termOperator)
        {
            StringBuilder oStringBuilder  = new StringBuilder();

            for (int iTermCounter = 0; iTermCounter < searchTerms.Length; iTermCounter++ )
            {
                if (iTermCounter > 0)
                {
                    oStringBuilder.Append(" " + termOperator + " ");       
                }
               
                 oStringBuilder.Append("'<em>" + searchTerms[iTermCounter] + "</em>'");
            }
            return oStringBuilder.ToString();
        }
    }
}