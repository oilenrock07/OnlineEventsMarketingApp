using System.Collections.Generic;
using System.Data;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IDataSheetService
    {
        void UploadDataSheet(int month, int year, DataTable table);
        void UploadDataSheet(int month, int year, IEnumerable<DataSheet> datasheet);
    }
}
