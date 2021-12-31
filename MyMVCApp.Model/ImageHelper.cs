
namespace MyMVCApp.Model
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Web;

    public static class ImageHelper
    {
        public static string BuildImgTag(string imagesdirectory, string imagePathFromAppRoot, string strWebAppRoot)
        {

            string strCleanUpContentImages;
     
            if (imagePathFromAppRoot.StartsWith("/Content/images/"))
            {
                string res2 = imagePathFromAppRoot.Replace("/Content/images/", "");
                strCleanUpContentImages = res2.Replace("/", @"\");
            } else if (imagePathFromAppRoot.StartsWith("Content/images/"))
            {
                string res2 = imagePathFromAppRoot.Replace("Content/images/", "");
                strCleanUpContentImages = res2.Replace("/", @"\");
            } else
            {
                strCleanUpContentImages = imagePathFromAppRoot.Replace("/", @"\");
            }
 
            string imagePath = imagesdirectory + strCleanUpContentImages;

            Image imageToResize;
            // Create image.
            try
            {
                 imageToResize = Image.FromFile(imagePath);
            }
            catch (FileNotFoundException)
            {
                return "Could not find image at [" + imagePath + "]";
            }
         
            var oStringBuilder = new StringBuilder();

            oStringBuilder.Append(@"<a href=""" + strWebAppRoot + imagePathFromAppRoot + @""" target=""_new"">");
            oStringBuilder.Append(@"<img src=""" + strWebAppRoot + imagePathFromAppRoot + @""" width=""100"" />");
            oStringBuilder.Append("</a>");

            return oStringBuilder.ToString();
        }
    }
}