using System;
using VTS.Core.CrossCutting;


namespace Specific.Droid
{
    public class PlatformException : IPlatformException
    {
        public PlatformException() : base() { }
        public Type URISyntaxException()
        {
            return typeof(Java.Net.URISyntaxException);
        }
    }
}
