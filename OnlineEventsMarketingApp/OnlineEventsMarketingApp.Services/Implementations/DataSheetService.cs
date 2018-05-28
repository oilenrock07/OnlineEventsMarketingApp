﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OnlineEventsMarketingApp.Common.Enums;
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
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
            var tags = _tagRepository.Find(x => !x.IsDeleted && x.Month == month && x.Year == year).ToList();

            //delete first the content for the month
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var date = row["Date"].ToDateTime();
                    if (!(date >= start && date < end)) continue;

                    var dataTag = row["Tag"].ToString();                    
                    var tag = tags.FirstOrDefault(x => string.Equals(x.TagName, dataTag, StringComparison.InvariantCultureIgnoreCase));
                    if (tag == null) //use logic to default the tag
                        tag = tags.FirstOrDefault(x => date >= x.StartDate && date <= x.EndDate);

                    var status = EnumHelper.ParseEnum<DataStatus>(row["Status"].ToString());
                    var data = new DataSheet
                    {
                        DIS = row["DIS"].ToInt(),
                        TE = row["TE"].ToInt(),
                        TM = row["TM"].ToString(),
                        Area = row["Area"].ToString(),
                        InHouse = row["InHouse"].ToString(),
                        Rnd = row["RND"].ToString(),
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

            _unitOfWork.ExecuteSqlCommand(string.Format("DELETE FROM DataSheets WHERE Date >= '{0}' AND Date < '{1}'", start.ToSqlDate(), end.ToSqlDate()));
            _unitOfWork.Commit();
        }

        public IEnumerable<DataSheet> GetDataSheet(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            return _dataSheetRepository.Find(x => x.Date >= startDate && x.Date < endDate).ToList();
        }

        public IEnumerable<WeeklyReportDTO> GetWeeklyReport(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var weeklyReport = (from data in _dataSheetRepository.GetAll()
                                join tag in _tagRepository.GetAll() on data.TagId equals tag.TagId
                                where data.Date >= startDate && data.Date < endDate && data.Status == 1 && !tag.IsDeleted
                                group new { data, tag } by new { data.DIS, data.TE, data.TM, data.InHouse, data.TagId, tag.TagName } into g
                                select new
                                {
                                    g.Key.DIS,
                                    g.Key.TE,
                                    g.Key.TM,
                                    g.Key.InHouse,
                                    g.Key.TagId,
                                    g.Key.TagName,
                                    Count = g.Count()
                                }).ToList();

            var result = weeklyReport.GroupBy(x => new
            {
                x.DIS,
                x.TE,
                x.TM,
                x.InHouse
            }).Select(x => new WeeklyReportDTO
            {
                DIS = x.Key.DIS,
                TE = x.Key.TE,
                TM = x.Key.TM,
                InHouse = x.Key.InHouse,
                TagReport = x.Select(y => new TagReportDTO
                {
                    TagId = y.TagId.Value,
                    TagCounts = y.Count,
                    TagName = y.TagName
                })
            });

            return result;
        }

        public IEnumerable<WeeklyInhouseSummaryDTO> GetWeeklyInHouseSummary(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var weeklyReport = (from data in _dataSheetRepository.GetAll()
                                join tag in _tagRepository.GetAll() on data.TagId equals tag.TagId
                                where data.Date >= startDate && data.Date < endDate && data.Status == 1 && !tag.IsDeleted
                                group new { data, tag } by new { data.InHouse, data.TagId, tag.TagName } into g
                                select new WeeklyInhouseSummaryDTO
                                {
                                    InHouse = g.Key.InHouse,
                                    TagId = g.Key.TagId.Value,
                                    TagName = g.Key.TagName,
                                    Count = g.Count()
                                }).ToList();

            return weeklyReport;
        }
    }
}
