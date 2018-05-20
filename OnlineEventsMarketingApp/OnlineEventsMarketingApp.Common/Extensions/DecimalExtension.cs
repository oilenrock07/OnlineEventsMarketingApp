using System;

namespace OnlineEventsMarketingApp.Common.Extensions
{
    public static class DecimalExtension
    {
        public static string FormatAmount(this decimal amount)
        {
            return String.Format("Php {0}", amount.ToString("####,###,##0.00"));
        }
    }
}
