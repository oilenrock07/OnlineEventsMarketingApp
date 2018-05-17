using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineEventsMarketingApp.Interfaces;
using OnlineEventsMarketingApp.Models;

namespace OnlineEventsMarketingApp.BusinessLogic
{
    public class PaginationService : IPaginationService
    {
        public IEnumerable<T> TakePaginationModel<T>(IEnumerable<T> list, IPaginationModel pagination) where T : class 
        {
            list = (pagination.CurrentPage > 0 ? list.Skip((pagination.CurrentPage - 1) * pagination.ItemsPerPage).Take(pagination.ItemsPerPage) : list);
            return list;
        }

        public IPaginationModel GetPaginationModel(HttpRequestBase request, int itemCount, int itemsPerPage = 0, string pageName = "")
        {
            var page = Convert.ToInt32(request.QueryString["Page"] ?? "1");

            if (itemsPerPage == 0)
            {
                itemsPerPage = 50;
            }

            return new PaginationModel
            {
                PageName = pageName,
                CurrentPage = page == 0 ? 1 : page,
                TotalPages = Convert.ToInt32(Math.Ceiling((decimal)itemCount / itemsPerPage)),
                TotalItems = itemCount,
                DefaultItemsPerPage = itemsPerPage,
                ItemsPerPage = itemsPerPage,
                
            };
        }
    }
}
