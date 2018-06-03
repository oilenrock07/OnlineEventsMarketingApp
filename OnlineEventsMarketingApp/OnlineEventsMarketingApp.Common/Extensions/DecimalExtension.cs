using System;
using System.Runtime.Remoting;

namespace OnlineEventsMarketingApp.Common.Extensions
{
    public static class DecimalExtension
    {
        public static string FormatAmount(this decimal amount)
        {
            return String.Format("Php {0}", amount.ToString("####,###,##0.00"));
        }

        public static string FormatDecimal(this decimal value)
        {
            return String.Format("{0}%", Math.Round(value*100));
        }
    }
}
