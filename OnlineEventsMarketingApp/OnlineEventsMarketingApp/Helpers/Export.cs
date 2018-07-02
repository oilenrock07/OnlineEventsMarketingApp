using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
    }
}