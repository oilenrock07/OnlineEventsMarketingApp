using System.Collections.Generic;
using System.Linq;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class TagService : BaseRepository, ITagService
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagService(IDatabaseFactory databaseFactory, IRepository<Tag> tagRepository)
            : base(databaseFactory)
        {
            _tagRepository = tagRepository;
        }

        public IEnumerable<Tag> GetTagsByMonth(int month)
        {
            return _tagRepository.Find(x => x.Month == month).ToList();
        }
    }
}
