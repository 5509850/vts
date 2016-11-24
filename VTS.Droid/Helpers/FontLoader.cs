using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace VTS.Droid.Helpers
{
    public static class FontLoader
    {
        public static Typeface GetFontNormal(Context context)
        {            
            Typeface tf;
            try
            {
                tf = Typeface.CreateFromAsset(context.Assets, "Oswald-Regular.ttf");
            }
            catch
            {
                return Typeface.Default;
            }
            return tf;
        }

        public static Typeface GetFontBold(Context context)
        {
            Typeface tf;
            try
            {
                tf = Typeface.CreateFromAsset(context.Assets, "Oswald-Bold.ttf");
            }
            catch
            {
                return Typeface.Default;
            }
            return tf;
        }

        public static Typeface GetFontLight(Context context)
        {
            Typeface tf;
            try
            {
                tf = Typeface.CreateFromAsset(context.Assets, "Oswald-Light.ttf");
            }
            catch
            {
                return Typeface.Default;
            }
            return tf;
        }
    }
}
