using Foundation;
using System;
using System.IO;
using UIKit;

namespace VTS.iOS.Helpers
{
    public static class Utils
    {
        public static DateTime NSDateToDateTime(this NSDate date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }       

        public const int LIST_VACATION = 0;
        public const int VACATION_REQUEST = 1;
        public const int SICK_REQUEST = 2;
        public const int OVERTIME_REQUEST = 3;
        public const int LIVEWOP_REQUEST = 4;
        public const int EXCEPTIONAL_REQUEST = 5;
        public const int EXIT_REQUEST = 6;
        
    }
}
