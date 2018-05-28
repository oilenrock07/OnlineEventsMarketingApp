﻿using System.Collections.Generic;
using System.Data;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IDataSheetService
    {
        void UploadDataSheet(int month, int year, DataTable table);
        IEnumerable<DataSheet> GetDataSheet(int month, int year);
        IEnumerable<WeeklyReportDTO> GetWeeklyReport(int month, int year);
        IEnumerable<WeeklyInhouseSummaryDTO> GetWeeklyInHouseSummary(int month, int year);
    }
}
