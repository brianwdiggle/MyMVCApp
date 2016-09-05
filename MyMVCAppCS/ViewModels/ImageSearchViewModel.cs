namespace MyMVCAppCS.ViewModels
{
    using System.Collections.Generic;
    using MyMVCApp.DAL;

    public class ImageSearchViewModel
    {
        public string SearchImageCaption { get; set; }

        public string SearchImageName { get; set; }

        public string SearchMarkerName { get; set; }

        // Results below
        public string SearchSummary { get; set; }
        public bool ImageResultsAvailable { get; set; }
        public List<Walk_AssociatedFile> ImagesFound;
    }
}