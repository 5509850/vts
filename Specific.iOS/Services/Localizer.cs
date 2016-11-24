using Foundation;
using VTS.Core.CrossCutting;

namespace Specific.iOS.Services
{
    public class Localizer : ILocalizer
    {
        public string GetCurrentCultureInfo()
        {
            var lang = NSLocale.PreferredLanguages[0].Split('_', '1', '-')[0];
            return lang;
        }
    }
}
