using System;

namespace OnlineEventsMarketingApp.Common.Extensions
{
    public static class StringExtension
    {
        public static int ToInt(this object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime ToDateTime(this object value)
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static string ToSqlDate(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }
    }
}
