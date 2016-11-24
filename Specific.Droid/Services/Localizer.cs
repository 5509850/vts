using System;
using VTS.Core.CrossCutting;

namespace Specific.Droid
{
    public class Localizer : ILocalizer
    {
        public string GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            string netLanguage = string.Empty;
            try
            {
                netLanguage = androidLocale.ToString().Split('_', '1')[0];
            }
            catch (Exception)
            {
                netLanguage = "en";
            }
            return netLanguage;
        }
    }
}