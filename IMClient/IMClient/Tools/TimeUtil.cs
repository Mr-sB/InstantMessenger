using System;

namespace IMClient.Tools
{
    public static class TimeUtil
    {
        private static int? mTimeDifferenceHour;

        public static int TimeDifferenceHour
        {
            get
            {
                if (!mTimeDifferenceHour.HasValue)
                    mTimeDifferenceHour = int.Parse(DateTime.Now.ToString("%z"));
                return mTimeDifferenceHour.Value;
            }
        }

        public static DateTime ToNowTime(this DateTime utcTime)
        {
            return utcTime.AddHours(TimeDifferenceHour);
        }
    }
}