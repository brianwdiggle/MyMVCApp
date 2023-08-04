using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;

    public class MarkerSearchResults
    {
        public List<Marker> MarkersFound { get; set; }

        public string SearchSummary { get; set; }
    }
}