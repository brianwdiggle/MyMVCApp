using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;

    public class ImageSearchResults
    {
        public List<Walk_AssociatedFile> ImagesFound { get; set; }

        public string SearchSummary { get; set; }
    }
}