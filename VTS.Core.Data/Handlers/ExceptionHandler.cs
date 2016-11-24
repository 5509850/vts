using System;

namespace VTS.Core.Data.Hundlers
{
    public class ExceptionHandler : Exception
    {
        public ExceptionHandler()
            : base()
        {

            this.Data.Add("message", "notFoundServer");
            throw this;
        }

        public ExceptionHandler(string message)
            : base(message)
        { }
    }
}

