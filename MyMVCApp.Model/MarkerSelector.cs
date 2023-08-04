using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;
    //todo: refactor to use factory with class for each search type
    public class MarkerSelector
    {
        public List<Marker> SelectMarkers(SearchTerm searchTerm, List<Marker> sourceMarkers)
        {
            List<Marker> selectedMarkers = new List<Marker>();

            switch (searchTerm.SearchType)
            {
                case SearchTermSelection.Description:
                    selectedMarkers = sourceMarkers.FindAll(marker => marker.Location_Description.ToLower().StartsWith(searchTerm.StringVals[0] + " ") ||
                                        marker.Location_Description.ToLower().EndsWith(" " + searchTerm.StringVals[0]) ||
                                        marker.Location_Description.ToLower().Contains(" " + searchTerm.StringVals[0] + " "));

                    for (int iSearchWord = 1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++)
                    {
                        List<Marker> additionalSearchTermMarkers = sourceMarkers.FindAll(marker => marker.Location_Description.ToLower().StartsWith(searchTerm.StringVals[iSearchWord] + " ") ||
                                         marker.Location_Description.ToLower().EndsWith(" " + searchTerm.StringVals[iSearchWord]) ||
                                         marker.Location_Description.ToLower().Contains(" " + searchTerm.StringVals[iSearchWord] + " "));

                        selectedMarkers = selectedMarkers.Intersect(additionalSearchTermMarkers).ToList();
                    }
                    break;
                case SearchTermSelection.Title:
                    selectedMarkers = sourceMarkers.FindAll(marker => marker.MarkerTitle.ToLower().StartsWith(searchTerm.StringVals[0] + " ") ||
                                         marker.MarkerTitle.ToLower().EndsWith(" " + searchTerm.StringVals[0]) ||
                                         marker.MarkerTitle.ToLower().Contains(" " + searchTerm.StringVals[0] + " "));

                    for (int iSearchWord = 1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++)
                    {
                        List<Marker> additionalSearchTermWalks = sourceMarkers.FindAll(marker => marker.MarkerTitle.ToLower().StartsWith(searchTerm.StringVals[iSearchWord] + " ") ||
                                        marker.MarkerTitle.ToLower().EndsWith(" " + searchTerm.StringVals[iSearchWord]) ||
                                        marker.MarkerTitle.ToLower().Contains(" " + searchTerm.StringVals[iSearchWord] + " "));

                        selectedMarkers = selectedMarkers.Intersect(additionalSearchTermWalks).ToList();
                    }
                    break;
                case SearchTermSelection.MarkerDateLeftFrom:
                    selectedMarkers = sourceMarkers.FindAll(marker => marker.DateLeft > searchTerm.DateTimeVal);
                    break;
                case SearchTermSelection.MarkerDateLeftTo:
                    selectedMarkers = sourceMarkers.FindAll(marker => marker.DateLeft < searchTerm.DateTimeVal);
                    break;
            }

            return selectedMarkers;
        }
    }
}