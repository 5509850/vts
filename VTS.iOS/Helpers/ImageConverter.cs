using Foundation;
using System;
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace VTS.iOS.Helpers
{
    public static class ImageConverter
    {
        public static UIImage ToImage(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIImage image = null;
            try
            {

                image = new UIImage(NSData.FromArray(data));
                data = null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
            return image;
        }

        public static byte[] GetByteByImg(UIImage originalImage)
        {
            if (originalImage != null)
            {                               
                UIImage resizer = ResizeImage(originalImage, 80, 80);
                NSData dataTempImage = resizer.AsPNG(); //originalImage.AsPNG(); 
                byte[] tempImage = new byte[dataTempImage.Length];
                System.Runtime.InteropServices.Marshal.Copy(dataTempImage.Bytes, tempImage, 0, Convert.ToInt32(tempImage.Length));
                return tempImage;
            }
            return null;
        }

        public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}
