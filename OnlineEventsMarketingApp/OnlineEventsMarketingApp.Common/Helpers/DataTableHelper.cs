using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlineEventsMarketingApp.Common.Helpers
{
    public static class DataTableHelper
    {
        public static bool IsHeaderValid(this DataTable table, IEnumerable<string> headers)
        {
            return headers.All(header => table.Columns.Contains(header));
        }
    }
}
