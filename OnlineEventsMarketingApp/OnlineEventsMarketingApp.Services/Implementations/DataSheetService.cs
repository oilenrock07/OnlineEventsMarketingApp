using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using OnlineEventsMarketingApp.Common.Constants;
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
        private readonly IRepository<NewUserMTD> _newUserMtdRepository;
        private readonly IRepository<DataSheet> _dataSheetRepository;
        private readonly IUnitOfWork _unitOfWork;        

        public DataSheetService(IUnitOfWork unitOfWork, IRepository<DataSheet> dataSheetRepository, IRepository<Tag> tagRepository, IRepository<NewUserMTD> newUserMtdRepository)
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _dataSheetRepository = dataSheetRepository;
            _newUserMtdRepository = newUserMtdRepository;
        }

        public void UploadDataSheet(int month, int year, DataTable table)
        {
            if (table == null)
                return;

            //get the tags for the month
            var yearMonthList = from row in table.AsEnumerable()
                                where row["Date"].ToString() != ""
                                select new {row["Date"].ToDateTime().Month, row["Date"].ToDateTime().Year};

            var months = yearMonthList.Select(x => x.Month).Distinct();
            var years = yearMonthList.Select(x => x.Year).Distinct();

            var tags = _tagRepository.Find(x => !x.IsDeleted && months.Contains(x.Month)  && years.Contains(x.Year)).ToList();

            var monthsDoesNotExists = months.Except(tags.Select(x => x.Month).Distinct());
            if (monthsDoesNotExists.Any())
            {
                var monthNames = monthsDoesNotExists.Select(x => new DateTime(year, x, 1).ToString("MMMM"));
                throw new Exception(String.Format("The following months does not have assigned tags: {0}", String.Join(",",monthNames)));
            }
                

            //delete first the content for the month
            var start = new DateTime(year, month, 1);
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var date = row["Date"].ToDateTime();

                    if (!IsDataSheetValid(row)) continue;

                    var tag = tags.FirstOrDefault(x => date >= x.StartDate && date <= x.EndDate);

                    var data = new DataSheet
                    {
                        DIS = row["DISTID"].ToInt(),
                        TE = row["TERRID"].ToInt(),
                        TM = row["TM"].ToString(),
                        Area = row["AREA"].ToString(),
                        InHouse = row["In House"].ToString().ToUpper(),
                        Rnd = row["RND"].ToString(),
                        Date = date == DateTime.MinValue ? default(DateTime?) : date,
                        TempDate = date == DateTime.MinValue ? start : default(DateTime?),
                        NewUsers = row["New User"].ToInt(),
                        ExistingUsers = row["Existing User"].ToInt(),
                        Status = row["STATUS"].ToString().ToUpper(),
                        NoOfPatients = row["# of PATIENTS"].ToInt(),
                        TagId = tag == null ? default(int) : tag.TagId
                    };

                    _dataSheetRepository.Add(data);
                }
            }

            ClearDataSheet(months, years);
            _unitOfWork.Commit();
        }

        public void UploadNewUserMTDDataSheet(int year, DataTable table)
        {
            if (table == null)
                return;

            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);

            var result = (from row in table.AsEnumerable()
                         where row.Field<string>("BRAND") == Constants.BRAND_NEPROVANILA && row.Field<string>("FIRST USE") == "YES" &&
                               row["Date"].ToDateTime() >= startDate && row["Date"].ToDateTime() < endDate
                          select new NewUserMTD
                          {
                              Month = row["Date"].ToDateTime().Month,
                              Year = row["Date"].ToDateTime().Year,
                              TMCode = row["TM CODE"].ToInt()
                          });

            if (result.Any())
            {
                foreach (var item in result)
                    _newUserMtdRepository.Add(item);
            }

            _unitOfWork.ExecuteSqlCommand(string.Format("DELETE FROM NewUserMTDs WHERE year = '{0}'", year));
            _unitOfWork.Commit();
        }

        public IEnumerable<DataSheet> GetDataSheet(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            return _dataSheetRepository.Find(x => (x.Date >= startDate && x.Date < endDate) || (x.Date == null && x.TempDate >= startDate && x.TempDate < endDate)).ToList();
        }

        public IEnumerable<MonthlyConsultationACTDTO> GetMonthlyConsultationReport(int year)
        {
            var report = _dataSheetRepository.Find(x => x.Date != null && x.Date.Value.Year == year && x.Status == "RUN").GroupBy(x => new { x.Date.Value.Month, x.InHouse})
                .Select(x => new MonthlyConsultationACTDTO
                {
                    Inhouse = x.Key.InHouse,
                    Month = x.Key.Month,
                    ACT = x.Sum(y => y.NewUsers + y.ExistingUsers)
                }).ToList();

            return report;
        }

        public IEnumerable<NewUserMTDDTO> GetMonthlyNewUserReport(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);

            var query = _newUserMtdRepository.Find(x => x.Year == year);
            return GetMonthlyNewUserReport(startDate, endDate, query);
        }

        public IEnumerable<NewUserMTDDTO> GetMonthlyNewUserReport(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var query = _newUserMtdRepository.Find(x => x.Year == year && x.Month == month);
            return GetMonthlyNewUserReport(startDate, endDate, query);
        }

        private IEnumerable<NewUserMTDDTO> GetMonthlyNewUserReport(DateTime startDate, DateTime endDate, IQueryable<NewUserMTD> query)
        {
            var datasheet = (from sheet in _dataSheetRepository.Find(x => x.Date != null && x.Date >= startDate && x.Date < endDate && x.Status == "RUN" && 
                             x.TagId != 0 && (x.InHouse == Constants.INHOUSE || x.InHouse == Constants.ONLINE))
                            group sheet by new { sheet.InHouse, sheet.TE, sheet.Date.Value.Month} into g
                            select new
                            {
                                g.Key.InHouse,
                                g.Key.TE,
                                g.Key.Month
                            });
            
            return (from newUser in query
                    join data in datasheet on newUser.TMCode equals data.TE
                    where newUser.Month == data.Month
                    group newUser by new { newUser.Month, newUser.Year, data.InHouse } into g
                    select new NewUserMTDDTO
                    {
                        InHouse = g.Key.InHouse,
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        ActualCount = g.Count()
                    }).ToList();
        }

        public IEnumerable<MonthlyRunsCountDTO> GetMonthlyRunsCount(int year)
        {
            return _dataSheetRepository.Find(x => x.Date != null && x.Date.Value.Year == year && x.Status == "RUN")
                   .GroupBy(x => new { x.Date.Value.Month, x.InHouse})
                   .Select(x => new MonthlyRunsCountDTO
                   {
                       InHouse = x.Key.InHouse,
                       Month = x.Key.Month,
                       Count = x.Count()
                   }).ToList();
        }

        public IEnumerable<WeeklyReportDTO> GetWeeklyReport(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var weeklyReport = (from data in _dataSheetRepository.GetAll()
                                join tag in _tagRepository.GetAll() on data.TagId equals tag.TagId
                                where data.Date >= startDate && data.Date < endDate && data.Status == "RUN" && !tag.IsDeleted
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
                                where data.Date >= startDate && data.Date < endDate && data.Status == "RUN" && !tag.IsDeleted
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

        public void ClearDataSheet(DateTime start, DateTime end)
        {
            _unitOfWork.ExecuteSqlCommand(string.Format("DELETE FROM DataSheets WHERE Date >= '{0}' AND Date < '{1}' OR (TempDate >= '{0}' AND TempDate < '{1}')", start.ToSqlDate(), end.ToSqlDate()));
        }

        public void ClearDataSheet(IEnumerable<int> months, IEnumerable<int> years)
        {
            var strMonths = String.Join(",", months);
            var strYear = String.Join(",", years);
            _unitOfWork.ExecuteSqlCommand(string.Format("DELETE FROM DataSheets WHERE MONTH(Date) IN ({0}) AND YEAR(Date) IN ({1}) OR (MONTH(TempDate) IN ({0}) AND YEAR(TempDate) IN ({1}))", strMonths, strYear));
        }

        public bool HasDataSheet(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            return _dataSheetRepository.Find(x => (x.Date >= start && x.Date < end) || (x.Date == null && x.TempDate >= start && x.TempDate < end)).Any();
        }

        #region Private Methods

        private bool IsDataSheetValid(DataRow row)
        {
            var valid = false;
            foreach (var header in Constants.DATASHEET_HEADERS)
            {
                if (row[header].ToString() != "")
                {
                    valid = true;
                    break;
                }
            }
            return valid;
        }
        #endregion

    }
}
