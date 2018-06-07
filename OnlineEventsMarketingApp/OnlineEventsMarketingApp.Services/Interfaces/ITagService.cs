using System;
using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTags(int year, int month);
        IEnumerable<Tag> GetTags(IEnumerable<DateTime> dates);
        bool HasTags(int year, int month);
    }
}
