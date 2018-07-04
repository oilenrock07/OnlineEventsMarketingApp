using System;
using System.Data;
using ClosedXML.Excel;
using OnlineEventsMarketingApp.Interfaces;

namespace OnlineEventsMarketingApp.Models.Reports
{
    public class ExportDataSourceBase : IExportDataSource
    {
        public DataTable Table { get; set; }
        public Action<IXLWorksheet> Action { get; set; }
    }
}