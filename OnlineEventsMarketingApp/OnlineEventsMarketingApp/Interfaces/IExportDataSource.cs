using System;
using System.Data;
using ClosedXML.Excel;

namespace OnlineEventsMarketingApp.Interfaces
{
    public interface IExportDataSource
    {
        DataTable Table { get; set; }
        Action<IXLWorksheet> Action { get; set; }
    }
}
