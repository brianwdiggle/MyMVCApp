using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;

    public class WalkSearchResults
    {
        public List<Walk> WalksFound { get; set; }

        public string SearchSummary { get; set; }
    }
}