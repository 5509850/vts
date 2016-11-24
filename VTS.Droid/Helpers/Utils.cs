using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views.InputMethods;
using System;
using System.IO;

namespace VTS.Droid.Helpers
{
    public static class Utils
    {
        public static void HideSoftKeyboard(Activity activity)
        {
            var inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, 0);
        }

        public static Bitmap DecodeBitmapFromStream(Context context, Android.Net.Uri url)
        {
            using (Stream stream = context.ContentResolver.OpenInputStream(url))
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InJustDecodeBounds = false;
                Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options);
                return bitmap;
            }
        }

        public static byte[] GetByteByURI(Context context, Android.Net.Uri uri)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var bitmap = DecodeBitmapFromStream(context, uri);
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return bitmapData;
            }
        }        
    }
}