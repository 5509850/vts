using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace VTS.iOS.Helpers
{
    public static class FontLoader
    {
        private static UIFont defaultNormalFont;
        private static UIFont defaultBoldFont;
        private static UIFont defaultLightlFont;
        private static UIFont defaultSmallFont;
        
        public static UIFont GetFontNormal()
        {
            if (defaultNormalFont == null)
            {
                try
                {
                    defaultNormalFont = UIFont.FromName("Oswald Regular", 15.0f);
                }
                catch
                {
                    return UIFont.FromName("Al Nile", 15.0f); ;
                }
            }
                
            return defaultNormalFont;
        }


        public static UIFont GetFontBold()
        {
            if (defaultBoldFont == null)
            {
                try
                {
                    defaultBoldFont = UIFont.FromName("Oswald", 16.0f);                
                }
                catch
                {
                    return UIFont.FromName("Al Nile", 16.0f);
                }
            }

            return defaultBoldFont;
        }

        public static UIFont GetFontLight()
        {
            if (defaultLightlFont == null)
            {
                try
                {
                    defaultLightlFont = UIFont.FromName("Oswald", 12.0f);
                }
                catch
                {
                    return UIFont.FromName("Al Nile", 12.0f); ;
                }
            }

            return defaultLightlFont;
        }

        public static UIFont GetFontSmall()
        {
            if (defaultSmallFont == null)
            {
                try
                {
                    defaultSmallFont = UIFont.FromName("Oswald", 10.0f);
                }
                catch
                {
                    return UIFont.FromName("Al Nile", 10.0f); ;
                }
            }

            return defaultSmallFont;
        }
    }
}
