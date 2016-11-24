using System;
using VTS.Core.CrossCutting;

namespace Specific.iOS.Services
{
    public class PlatformException : IPlatformException
    {
        public PlatformException()
            : base()
        { }

        public Type URISyntaxException()
        {
            return typeof(System.Net.WebException);
        }
    }
}
