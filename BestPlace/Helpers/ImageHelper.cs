using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BestPlace.Helpers
{
    public class ImageHelper
    {
        public static void SaveJpeg(string path, Bitmap img)
        {
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, (long)1000);

            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public static void CropImage(string imgPath, string path, int width, int height)
        {
            using (Bitmap img = (Bitmap)Image.FromFile(imgPath))
            {
                Rectangle cropArea = new Rectangle(0, 0, 0, 0);
                cropArea.Width = Math.Min(Math.Min(img.Width, img.Height), width);
                cropArea.Height = Math.Min(Math.Min(img.Width, img.Height), height);

                Bitmap bmpCrop = img.Clone(cropArea, img.PixelFormat);

                SaveJpeg(path, bmpCrop);
            };
        }
    }
}