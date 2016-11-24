using System;

namespace VTS.Core.CrossCutting.Helpers
{
    public static class ConverterHelper
    {
        public static DateTime ConvertMillisecToDateTime(long millsec)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(millsec).ToLocalTime().Date;
        }      

        public static int CalculateDuration(DateTime from, DateTime to)
        {
            if (to < from)
            {
                DateTime temp = new DateTime(from.Year, from.Month, from.Day);
                from = to;
                to = temp;
            }
            return (int)(to.Date - from.Date).TotalDays + 1;
        }

        public static long ConvertDateTimeToMillisec(DateTime date)
        {
            return (long)(date.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}