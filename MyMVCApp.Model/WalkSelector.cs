using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;
    //todo: refactor to use factory with class for each search type
    public class WalkSelector
    {
        public List<Walk> SelectWalks(SearchTerm searchTerm, List<Walk> sourceWalks)
        {
            List<Walk> selectedWalks = new List<Walk>();

            switch (searchTerm.SearchType)
            {
                case SearchTermSelection.HillAscended:
                    selectedWalks = (from walk in sourceWalks
                                     where
                                         walk.HillAscents.Any(
                                             ascent => ascent.Hill.Hillname.Contains(searchTerm.StringVals[0]))
                                     select walk).ToList();
                    break;
                case SearchTermSelection.Description:
                      selectedWalks= sourceWalks.FindAll(walk => walk.WalkDescription.ToLower().StartsWith(searchTerm.StringVals[0] + " ") ||
                                         walk.WalkDescription.ToLower().EndsWith(" " + searchTerm.StringVals[0]) ||
                                         walk.WalkDescription.ToLower().Contains(" " + searchTerm.StringVals[0] + " "));

                    for (int iSearchWord=1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++ )
                    {
                        List<Walk> additionalSearchTermWalks = sourceWalks.FindAll(walk => walk.WalkDescription.ToLower().StartsWith(searchTerm.StringVals[iSearchWord] + " ") ||
                                         walk.WalkDescription.ToLower().EndsWith(" " + searchTerm.StringVals[iSearchWord]) ||
                                         walk.WalkDescription.ToLower().Contains(" " + searchTerm.StringVals[iSearchWord] + " "));
                        
                        selectedWalks = selectedWalks.Intersect(additionalSearchTermWalks).ToList();
                    }
                    break;
                case SearchTermSelection.ImageCaption:
                    selectedWalks = (from walk in sourceWalks
                                     where
                                        walk.Walk_AssociatedFiles.Any(
                                            was => was.Walk_AssociatedFile_Caption != null && (was.Walk_AssociatedFile_Caption.ToLower().Contains(" " + searchTerm.StringVals[0] + " ") ||
                                                                                               was.Walk_AssociatedFile_Caption.ToLower().StartsWith(searchTerm.StringVals[0]) ||
                                                                                               was.Walk_AssociatedFile_Caption.ToLower().EndsWith(searchTerm.StringVals[0])))
                                    select walk).ToList();

                    for (int iSearchWord = 1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++)
                    {
                        List<Walk> additionalSearchTermWalks = (from walk in sourceWalks
                                                                where
                                                                   walk.Walk_AssociatedFiles.Any(
                                                                       was => was.Walk_AssociatedFile_Caption != null && (was.Walk_AssociatedFile_Caption.ToLower().Contains(" " + searchTerm.StringVals[iSearchWord] + " ") ||
                                                                                                                          was.Walk_AssociatedFile_Caption.ToLower().StartsWith(searchTerm.StringVals[iSearchWord]) ||
                                                                                                                          was.Walk_AssociatedFile_Caption.ToLower().EndsWith(searchTerm.StringVals[iSearchWord])))
                                                                select walk).ToList();
                        selectedWalks = selectedWalks.Intersect(additionalSearchTermWalks).ToList();
                    }
                    break;
                case SearchTermSelection.MetresAscent:
                    if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.MetresOfAscent > searchTerm.IntVal);
                    }else
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.MetresOfAscent < searchTerm.IntVal);              
                    }
                    break;
                case SearchTermSelection.OverallSpeed:
                    if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.WalkAverageSpeedKmh > searchTerm.DoubleVal);
                    }
                    else
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.WalkAverageSpeedKmh < searchTerm.DoubleVal);
                    }
                    break;
                case SearchTermSelection.Title:
                     selectedWalks= sourceWalks.FindAll(walk => walk.WalkTitle.ToLower().StartsWith(searchTerm.StringVals[0] + " ") ||
                                         walk.WalkTitle.ToLower().EndsWith(" " + searchTerm.StringVals[0]) ||
                                         walk.WalkTitle.ToLower().Contains(" " + searchTerm.StringVals[0] + " "));

                    for (int iSearchWord=1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++ )
                    {
                        List <Walk> additionalSearchTermWalks = sourceWalks.FindAll(walk => walk.WalkTitle.ToLower().StartsWith(searchTerm.StringVals[iSearchWord] + " ") ||
                                         walk.WalkTitle.ToLower().EndsWith(" " + searchTerm.StringVals[iSearchWord]) ||
                                         walk.WalkTitle.ToLower().Contains(" " + searchTerm.StringVals[iSearchWord] + " "));
                        
                        selectedWalks = selectedWalks.Intersect(additionalSearchTermWalks).ToList();
                    }
                    break;
                case SearchTermSelection.WalkDateFrom:
                    selectedWalks = sourceWalks.FindAll(walk => walk.WalkDate > searchTerm.DateTimeVal);
                    break;
                case SearchTermSelection.WalkDateTo:
                    selectedWalks = sourceWalks.FindAll(walk => walk.WalkDate < searchTerm.DateTimeVal);
                    break;
                case SearchTermSelection.WalkDuration:
                    int totalTime = searchTerm.DateTimeVal.Hour * 60 + searchTerm.DateTimeVal.Minute;
                    if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.WalkTotalTime > totalTime);
                    }else
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.WalkTotalTime < totalTime);
                    }
   
                    break;
                case SearchTermSelection.WalkLength:
                    if (searchTerm.SearchOperator == SearchOperator.GreaterThan)
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.CartographicLength > searchTerm.DoubleVal);
                    }
                    else
                    {
                        selectedWalks = sourceWalks.FindAll(walk => walk.CartographicLength < searchTerm.DoubleVal);                        
                    }
                    break;
            }

            return selectedWalks;
        }
    }
}