using System.Collections.Generic;
using System.Data;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IDataSheetService
    {
        void UploadDataSheet(int month, int year, DataTable table);
        IEnumerable<WeeklyReportDTO> GetWeeklyReport(int month, int year);
    }
}
