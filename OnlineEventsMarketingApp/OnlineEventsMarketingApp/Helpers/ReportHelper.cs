using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClosedXML.Excel;
using OnlineEventsMarketingApp.Models.Reports;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Helpers
{
    public class ReportHelper
    {
        public ExportDataSourceBase GenerateMonthlyDataTable(IEnumerable<MonthlyReportData> data, string name = "")
        {
            var dt = new DataTable(name);

            dt.Columns.Add("Month", typeof(string));
            dt.Columns.Add("# Of Runs", typeof(int));
            dt.Columns.Add("TGT Consultation", typeof(int));
            dt.Columns.Add("ACT Consultation", typeof(int));
            dt.Columns.Add("ACT vs TGT % Consultation", typeof(string));
            dt.Columns.Add("TGT NU", typeof(int));
            dt.Columns.Add("ACT NU", typeof(int));
            dt.Columns.Add("ACT vs TGT % NU", typeof(string));

            foreach (var item in data)
            {
                var row = dt.NewRow();
                row["Month"] = item.MonthName;
                row["# Of Runs"] = item.NoOfRuns;
                row["TGT Consultation"] = item.ConsultationTGT;
                row["ACT Consultation"] = item.ConsultationACT;
                row["ACT vs TGT % Consultation"] = item.ConsultationACTVsTGT;
                row["TGT NU"] = item.NUTGT;
                row["ACT NU"] = item.NUACT;
                row["ACT vs TGT % NU"] = item.NUACTVsTGT;

                dt.Rows.Add(row);
            }

            Action<IXLWorksheet> action = workSheet =>
            {
                workSheet.Row(1).InsertRowsAbove(1);
                workSheet.Range("A1:B1").Row(1).Merge();
                workSheet.Range("C1:E1").Row(1).Merge();
                workSheet.Range("F1:H1").Row(1).Merge();
                workSheet.Cell(1, 1).Value = "";
                workSheet.Cell(1, 3).Value = "Consultation";
                workSheet.Cell(1, 6).Value = "NU";
            };

            return new ExportDataSourceBase
            {
                Action = action,
                Table = dt
            };
        }

        public ExportDataSourceBase GenerateWeeklyDataTable(IEnumerable<WeeklyReportDTO> data, IEnumerable<Tag> tags)
        {
            var dt = new DataTable("Weekly Tags Run");

            dt.Columns.Add("DIS", typeof(int));
            dt.Columns.Add("TE", typeof(int));
            dt.Columns.Add("TM", typeof(string));
            dt.Columns.Add("InHouse", typeof(string));

            foreach (var item in tags)
                dt.Columns.Add(item.TagName, typeof(string)); 

            dt.Columns.Add("Total", typeof(int));
            dt.Columns.Add("Variance", typeof(int));


            foreach (var item in data)
            {
                var row = dt.NewRow();
                row["DIS"] = item.DIS;
                row["TE"] = item.TE;
                row["TM"] = item.TM;
                row["InHouse"] = item.InHouse;

                foreach (var tag in tags)
                    row[tag.TagName] = item.GetTagCount(tag.TagId);

                row["Total"] = item.Total;
                row["Variance"] = item.Variance;

                dt.Rows.Add(row);
            }

            return new ExportDataSourceBase
            {
                Table = dt,
                Action = null
            };
        }

        public ExportDataSourceBase GenerateWeeklySummaryDataTable(IEnumerable<WeeklyInhouseSummaryDTO> data, IEnumerable<Tag> tags)
        {
            var dt = new DataTable("Summary");

            //add columns
            dt.Columns.Add("Type", typeof(string));

            foreach (var item in tags)
                dt.Columns.Add(item.TagName, typeof(string));

            dt.Columns.Add("Total", typeof(int));

            //add data
            foreach (var item in data.GroupBy(x => x.InHouse))
            {
                var row = dt.NewRow();

                row["Type"] = item.Key;
                foreach (var tag in tags)
                {
                    row[tag.TagName] = GetWeeklyInhouseCount(data, tag.TagId, item.Key);
                }
                row["Total"] = GetWeeklyInhouseTotal(data, item.Key);
                dt.Rows.Add(row);
            }

            DataRow overAllTotalRow = dt.NewRow(); 
            overAllTotalRow["Type"] = "Overall Total";
            foreach (var tag in tags)
            {
                overAllTotalRow[tag.TagName] = GetWeeklyInhouseOverAllTotal(data, tag.TagId);
            }
            overAllTotalRow["Total"] = GetWeeklyInhouseTotalCount(data);
            dt.Rows.Add(overAllTotalRow);

            //add action
            Action<IXLWorksheet> action = workSheet =>
            {
                workSheet.Cell(1, 1).SetValue(" ");
            };

            return new ExportDataSourceBase
            {
                Table = dt,
                Action = action
            };
        }

        private int GetWeeklyInhouseCount(IEnumerable<WeeklyInhouseSummaryDTO> data, int tagId, string inhouse)
        {
            if (data != null)
            {
                var tag = data.FirstOrDefault(x => x.TagId == tagId && x.InHouse == inhouse);
                if (tag != null)
                    return tag.Count;
            }

            return 0;
        }

        private int GetWeeklyInhouseTotal(IEnumerable<WeeklyInhouseSummaryDTO> data, string inhouse)
        {
            if (data != null)
                return data.Where(x => x.InHouse == inhouse).Sum(x => x.Count);

            return 0;
        }

        private int GetWeeklyInhouseOverAllTotal(IEnumerable<WeeklyInhouseSummaryDTO> data, int tagId)
        {
            if (data != null)
                return data.Where(x => x.TagId == tagId).Sum(x => x.Count);

            return 0;
        }

        private int GetWeeklyInhouseTotalCount(IEnumerable<WeeklyInhouseSummaryDTO> data)
        {
            if (data != null)
                return data.Sum(x => x.Count);

            return 0;
        }
    }
}