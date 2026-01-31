using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing; // For ImageFormat
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace MyMVCApp.ImagePreparer
{
    public class ImageResizer
    {

        public ImageResizer(string sourceFilePath, string targetPath)
        {
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetPath;
        }

        public ImageResizer(string sourceFilePath, string targetPath, string fileNamePrefix)
        {
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetPath;
            ImageNamePrefix = fileNamePrefix;
        }

        public ImageResizer(string sourceFilePath, string targetPath, string fileNamePrefix, short shMaxWidth, short shMaxHeight)
        {
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetPath;
            ImageNamePrefix = fileNamePrefix;
            CanvasWidthPixels = shMaxWidth;
            CanvasHeightPixels = shMaxHeight;
        }

        public string WalkTitle { get; set; } = string.Empty;

        public string SourceFilePath { get; set; } = string.Empty;

        public string TargetFilePath { get; set; } = string.Empty;

        public string ImageNamePrefix { get; set; } = string.Empty;

        public short CanvasWidthPixels { get; set; } = 0;

        public short CanvasHeightPixels { get; set; } = 0;


        public bool ResizeImagesInDirectory()
        {

            int iImageNumber = 1;
            string strTargetFileFullPath = "";

            Console.WriteLine("Resizing images in source directory [" + SourceFilePath + "]");

            if (this.SourceFilePath == string.Empty || this.TargetFilePath == string.Empty || this.ImageNamePrefix == string.Empty)
            {
                Console.WriteLine("image resize: class variables not set");
                return false;
            }
            string pattern = @"/\.JPG$/";
            Regex regex = new Regex(pattern);

            var imageFiles = Directory.EnumerateFiles(SourceFilePath, "*.*", SearchOption.AllDirectories)
                                     .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                 s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));

            foreach (var sourceFilePath in imageFiles)
            {
                strTargetFileFullPath = this.TargetFilePath + this.ImageNamePrefix + "_" + iImageNumber.ToString() + ".jpg";
                Console.WriteLine("resizing " + sourceFilePath + " to " + strTargetFileFullPath);

                bool bResult = ResizeImageFromFile(sourceFilePath, strTargetFileFullPath, iImageNumber);

                iImageNumber++;
            }


            return true;
        }



        public bool ResizeImageFromFile(string sourceFullPath, string targetFullPath, int iImageNum)
        {
            // Load the original image
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(sourceFullPath);

            // If exif property for orientation indicates the image should be rotated, do this.
            // (portrait images from iphones are stored natively as landscape)
            if (Array.IndexOf(originalImage.PropertyIdList, 274) > -1)
            {
                var orientation = (int)originalImage.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        originalImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        originalImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        originalImage.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                originalImage.RemovePropertyItem(274);
            }

            double ratioX = (double)CanvasWidthPixels / originalImage.Width;
            double ratioY = (double)CanvasHeightPixels / originalImage.Height;
            double ratio = Math.Min(ratioX, ratioY); // Use the smaller ratio to ensure the image fits within both dimensions

            int newWidth = (int)(originalImage.Size.Width * ratio);
            int newHeight = (int)(originalImage.Size.Height * ratio);

            // Size size = new Size(newWidth, newHeight);

            Bitmap sizedImage = ResizeImageInMemory(originalImage, newWidth, newHeight, iImageNum);

            // Save the new image
            // Save as JPEG with a specific quality (optional)
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L); // 90% quality
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg"); // Helper method to get codec info

            sizedImage.Save(targetFullPath, jpegCodec, encoderParameters);
       
            originalImage.Dispose();
            sizedImage.Dispose();

            return true;
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImageInMemory(System.Drawing.Image image, int width, int height, int iImageNo)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);


            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;


                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            // Copy the exif data from the original to preserve it.
            foreach (var id in image.PropertyIdList)
            {
                PropertyItem pi = image.GetPropertyItem(id);
                if (pi.Id == 305)  // Image software name exif property
                {
                    pi.Value = Encoding.UTF8.GetBytes("Brians Walking Web Site (c)2025");
                    pi.Len = pi.Value.Length;
                }
                else if (pi.Id == 306)
                {
                    pi.Value = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                    pi.Len = pi.Value.Length;

                }
                else if (pi.Id == 36868) // datetime image digitised
                {

                    pi.Id = 270; // repurposing this property as image title
                    pi.Value = Encoding.UTF8.GetBytes(WalkTitle + " image " + iImageNo.ToString() + " .");
                    pi.Len = pi.Value.Length;
                }
                destImage.SetPropertyItem(pi);

            }

            return destImage;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                if (codec.MimeType == mimeType)
                    return codec;

            return null;
        }

 

    }


}
