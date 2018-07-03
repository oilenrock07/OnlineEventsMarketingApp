using System;
using System.Collections.Generic;
using System.Data;
using ClosedXML.Excel;
using OnlineEventsMarketingApp.Models.Reports;

namespace OnlineEventsMarketingApp.Helpers
{
    public class ReportHelper
    {
        public MonthlyReportExportDataSource GenerateMonthlyDataTable(IEnumerable<MonthlyReportData> data, string name = "")
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

            return new MonthlyReportExportDataSource
            {
                Action = action,
                Table = dt
            };
        }        
    }
}