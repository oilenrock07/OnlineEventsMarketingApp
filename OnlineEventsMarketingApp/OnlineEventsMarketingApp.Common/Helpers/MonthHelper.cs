using System;
using System.Collections.Generic;

namespace OnlineEventsMarketingApp.Common.Helpers
{
    public static class MonthYearHelper
    {
        public static IEnumerable<Tuple<int, string>> _months = new List<Tuple<int, string>>
        {
            new Tuple<int, string>(1, "January"),
            new Tuple<int, string>(2, "February"),
            new Tuple<int, string>(3, "March"),
            new Tuple<int, string>(4, "April"),
            new Tuple<int, string>(5, "May"),
            new Tuple<int, string>(6, "June"),
            new Tuple<int, string>(7, "July"),
            new Tuple<int, string>(8, "August"),
            new Tuple<int, string>(9, "September"),
            new Tuple<int, string>(10, "October"),
            new Tuple<int, string>(11, "November"),
            new Tuple<int, string>(12, "December")
        };

        public static IEnumerable<Tuple<int, string>> GetMonths()
        {
            return _months;
        }

        public static IEnumerable<int> GetYears()
        {
            var yearNow = DateTime.Now.Year;
            var years = new List<int>();

            for (var a = yearNow - 5; a <= yearNow + 5; a++)
                years.Add(a);

            return years;
        }
    }
}
