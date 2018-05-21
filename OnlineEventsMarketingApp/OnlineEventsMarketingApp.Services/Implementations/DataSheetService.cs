using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class DataSheetService : IDataSheetService
    {
        private readonly IRepository<DataSheet> _dataSheetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DataSheetService(IUnitOfWork unitOfWork, IRepository<DataSheet> dataSheetRepository)
        {
            _unitOfWork = unitOfWork;
            _dataSheetRepository = dataSheetRepository;
        }

        public void UploadDataSheet(int month, int year, IEnumerable<DataSheet> datasheet)
        {
            //delete first the content for the month
            _dataSheetRepository.ExecuteSqlCommand("UPDATE DataSheet SET IsDeleted = 1WHERE Date BETWEEN {0} AND {1}", month, year);

            foreach (var data in datasheet)
                _dataSheetRepository.Add(data);

            _unitOfWork.Commit();
        }
    }
}
