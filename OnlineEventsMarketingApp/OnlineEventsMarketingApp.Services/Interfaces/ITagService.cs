using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTagsByMonth(int month);
    }
}
