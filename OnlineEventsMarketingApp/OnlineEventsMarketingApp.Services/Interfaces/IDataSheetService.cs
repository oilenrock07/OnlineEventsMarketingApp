using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IDataSheetService
    {
        void UploadDataSheet(int month, int year, IEnumerable<DataSheet> datasheet);
    }
}
