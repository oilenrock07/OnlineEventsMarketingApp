using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OnlineEventsMarketingApp.Common.Enums;
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class DataSheetService : IDataSheetService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<DataSheet> _dataSheetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DataSheetService(IUnitOfWork unitOfWork, IRepository<DataSheet> dataSheetRepository, IRepository<Tag> tagRepository)
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _dataSheetRepository = dataSheetRepository;
        }

        public void UploadDataSheet(int month, int year, DataTable table)
        {
            if (table == null)
                return;

            //get the tags for the month
            var tags = _tagRepository.Find(x => !x.IsDeleted && x.Month == month && x.Year == year);

            //delete first the content for the month
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            _unitOfWork.ExecuteSqlCommand(string.Format("UPDATE DataSheets SET IsDeleted = 'True' WHERE Date >= '{0}' AND Date < '{1}'", start.ToSqlDate(), end.ToSqlDate()));

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var dataTag = row["Tag"].ToString();
                    var date = row["Rnd"].ToDateTime();
                    var tag = tags.FirstOrDefault(x => string.Equals(x.TagName, dataTag, StringComparison.InvariantCultureIgnoreCase));
                    if (tag == null) //use logic to default the tag
                        tag = tags.FirstOrDefault(x => x.StartDate >= date && x.EndDate <= date);

                    var status = EnumHelper.ParseEnum<DataStatus>(row["Status"].ToString());
                    var data = new DataSheet
                    {
                        DIS = row["DIS"].ToInt(),
                        TE = row["TE"].ToInt(),
                        TM = row["TM"].ToString(),
                        Area = row["DIS"].ToString(),
                        InHouse = row["InHouse"].ToString(),
                        Rnd = row["Rnd"].ToString(),
                        Date = date,
                        NewUsers = row["New Users"].ToInt(),
                        ExistingUsers = row["Existing Users"].ToInt(),
                        Status = (byte)status,
                        NoOfPatients = row["No Of Patients"].ToInt(),
                        TagId = tag == null ? default(int) : tag.TagId
                    };

                    _dataSheetRepository.Add(data);
                }                    
            }

            _unitOfWork.Commit();
        }

        public void UploadDataSheet(int month, int year, IEnumerable<DataSheet> datasheet)
        {
            //delete first the content for the month
            _dataSheetRepository.ExecuteSqlCommand("UPDATE DataSheet SET IsDeleted = 1 WHERE Date BETWEEN {0} AND {1}", month, year);

            foreach (var data in datasheet)
                _dataSheetRepository.Add(data);

            _unitOfWork.Commit();
        }
    }
}
