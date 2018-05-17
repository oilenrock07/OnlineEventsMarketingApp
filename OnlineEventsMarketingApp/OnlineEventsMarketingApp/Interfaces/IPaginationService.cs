using System.Collections.Generic;
using System.Web;

namespace OnlineEventsMarketingApp.Interfaces
{
    public interface IPaginationService
    {
        IPaginationModel GetPaginationModel(HttpRequestBase request, int itemCount, int itemsPerPage = 0, string pageName = "");
        IEnumerable<T> TakePaginationModel<T>(IEnumerable<T> list, IPaginationModel pagination) where T : class;
    }
}
