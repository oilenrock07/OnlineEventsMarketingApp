using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using OnlineEventsMarketingApp.Models.Reports;

namespace OnlineEventsMarketingApp.Helpers
{
    public static class Export
    {
        public delegate void GridOnRowCreated(object sender, GridViewRowEventArgs gridViewRowEventArgs);

        public static void ToExcel(HttpResponseBase response, object clientsList, string fileName, GridOnRowCreated onRowCreated = null)
        {
            var grid = new GridView();
            if (onRowCreated != null)
                grid.RowCreated += new GridViewRowEventHandler(onRowCreated);

            grid.DataSource = clientsList;
            grid.DataBind();
            response.ClearContent();
            fileName = String.Format("{0}.xls", fileName);
            response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            response.ContentType = "application/excel";
            using (var sw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    grid.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        public static void ToExcel(HttpResponseBase response, IEnumerable<DataTable> data, string fileName)
        {
            var workBook = new XLWorkbook();

            foreach (var item in data)
            {
                workBook.Worksheets.Add(item);
            }

            response.Clear();
            response.Buffer = true;
            response.Charset = "";
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xlsx", fileName));
            using (var MyMemoryStream = new MemoryStream())
            {
                workBook.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(response.OutputStream);
                response.Flush();
                response.End();
            }
        }
    }
}