using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using MyMVCApp.DAL;

    public class ImageSelector
    {
        public List<Walk_AssociatedFile> SelectImages(SearchTerm searchTerm, List<Walk_AssociatedFile> sourceImages)
        {
            var selectedImages = new List<Walk_AssociatedFile>();

            switch (searchTerm.SearchType)
            {
                case SearchTermSelection.ImageCaption:
                      selectedImages= sourceImages.FindAll(image => image.Walk_AssociatedFile_Caption.ToLower().StartsWith(searchTerm.StringVals[0] + " ") ||
                                         image.Walk_AssociatedFile_Caption.ToLower().EndsWith(" " + searchTerm.StringVals[0]) ||
                                         image.Walk_AssociatedFile_Caption.ToLower().Contains(" " + searchTerm.StringVals[0] + " "));

                    for (int iSearchWord=1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++ )
                    {
                        List<Walk_AssociatedFile> additionalSearchTermImages =
                            sourceImages.FindAll(
                                image =>
                                image.Walk_AssociatedFile_Caption.ToLower().StartsWith(
                                    searchTerm.StringVals[iSearchWord] + " ")
                                ||
                                image.Walk_AssociatedFile_Caption.ToLower().EndsWith(
                                    " " + searchTerm.StringVals[iSearchWord])
                                ||
                                image.Walk_AssociatedFile_Caption.ToLower().Contains(
                                    " " + searchTerm.StringVals[iSearchWord] + " "));
                        
                        selectedImages = selectedImages.Intersect(additionalSearchTermImages).ToList();
                    }
                    break;
                case SearchTermSelection.ImageName:
                    selectedImages = sourceImages.FindAll(image => image.Walk_AssociatedFile_Name.ToLower().Contains(searchTerm.StringVals[0]));

                    for (int iSearchWord = 1; iSearchWord < searchTerm.StringVals.Length; iSearchWord++)
                    {
                        List<Walk_AssociatedFile> additionalSearchTermImages = sourceImages.FindAll(image => image.Walk_AssociatedFile_Name.ToLower().Contains(searchTerm.StringVals[iSearchWord]));

                        selectedImages = selectedImages.Intersect(additionalSearchTermImages).ToList();
                    }
                    break;           
            }
            return selectedImages;
        }
    }
}