using System;
using System.Collections.Generic;
using System.Data;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IDataSheetService
    {
        bool HasDataSheet(int year, int month);
        void UploadDataSheet(int month, int year, DataTable table);
        void UploadNewUserMTDDataSheet(int year, DataTable table);
        void ClearDataSheet(DateTime start, DateTime end);
        IEnumerable<DataSheet> GetDataSheet(int month, int year);
        IEnumerable<MonthlyRunsCountDTO> GetMonthlyRunsCount(int year);
        IEnumerable<WeeklyReportDTO> GetWeeklyReport(int month, int year);
        IEnumerable<WeeklyInhouseSummaryDTO> GetWeeklyInHouseSummary(int month, int year);
        IEnumerable<NewUserMTDDTO> GetMonthlyNewUserReport(int year);
        IEnumerable<NewUserMTDDTO> GetMonthlyNewUserReport(int year, int month);
        IEnumerable<MonthlyConsultationACTDTO> GetMonthlyConsultationReport(int year);
    }
}
