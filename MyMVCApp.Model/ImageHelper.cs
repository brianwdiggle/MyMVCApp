
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
           
           // string imagePath = HttpRuntime.AppDomainAppPath + imagePathFromAppRoot.Substring(1, imagePathFromAppRoot.Length - 1).Replace(@"/",@"\");
            string res1 = imagesdirectory.Replace(@"\\", "\\");
            string res2 = imagePathFromAppRoot.Replace("/Content/images/", "");
            string res3 = res2.Replace("/",@"\");

            string imagePath = res1 + res3;
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
         

            int width = imageToResize.Width/ 8;
            int height = imageToResize.Height/ 8;

            var oStringBuilder = new StringBuilder();

            oStringBuilder.Append(@"<a href=""" + strWebAppRoot + imagePathFromAppRoot + @""" target=""_new"">");
            oStringBuilder.Append(@"<img src=""" + strWebAppRoot + imagePathFromAppRoot + @""" width=" + width + " height=" + height + " />");
            oStringBuilder.Append("</a>");

            return oStringBuilder.ToString();
        }
    }
}