using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using OnlineEventsMarketingApp.Models.Reports;

namespace OnlineEventsMarketingApp.Helpers
{
    public class ReportHelper
    {
        public GridView PopulateMonthlyReportGridView(IEnumerable<MonthlyReportData> data)
        {
            var gridView = new GridView();
            gridView.RowCreated += OnRowCreated;
            gridView.DataSource = GenerateDataTable(data);
            gridView.DataBind();
            return gridView;
        }

        public DataTable GenerateDataTable(IEnumerable<MonthlyReportData> data, string name = "")
        {
            var dt = new DataTable(name);
            var tgtConsultation = new DataColumn()
            {
                ColumnName = "TGT Consultation",
                DataType = typeof(int)
            };
            var tgtNU = new DataColumn()
            {
                ColumnName = "TGT NU",
                DataType = typeof(int)
            };
            dt.Columns.Add("Month", typeof(string));
            dt.Columns.Add("# Of Runs", typeof(int));
            dt.Columns.Add(tgtConsultation);
            dt.Columns.Add("ACT Consultation", typeof(int));
            dt.Columns.Add("ACT vs TGT % Consultation", typeof(string));
            dt.Columns.Add(tgtNU);
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

            return dt;
        }

        private void OnRowCreated(object sender, GridViewRowEventArgs gridViewRowEventArgs)
        {
            if (gridViewRowEventArgs.Row.RowType == DataControlRowType.Header) // If header created
            {
                var gridview = (GridView)sender;

                // Creating a Row
                GridViewRow HeaderRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                //Adding Year Column
                TableCell HeaderCell = new TableCell();
                HeaderCell.Text = "";
                HeaderCell.ColumnSpan = 2;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding Period Column
                HeaderCell = new TableCell();
                HeaderCell.Text = "Consultation";
                HeaderCell.ColumnSpan = 3;
                HeaderCell.HorizontalAlign = HorizontalAlign.Center;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding Audited By Column
                HeaderCell = new TableCell();
                HeaderCell.Text = "NU";
                HeaderCell.ColumnSpan = 3;
                HeaderCell.HorizontalAlign = HorizontalAlign.Center;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding the Row at the 0th position (first row) in the Grid
                gridview.Controls[0].Controls.AddAt(0, HeaderRow);
            }
        }
    }
}