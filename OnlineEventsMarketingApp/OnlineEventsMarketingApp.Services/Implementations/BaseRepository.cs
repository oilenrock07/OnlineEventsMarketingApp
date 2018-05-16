using OnlineEventsMarketingApp.Entities.Contexts;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public abstract class BaseRepository
    {
        public OnlineEventsContext Context { get; set; }

        protected BaseRepository(IDatabaseFactory databaseFactory)
        {
            if (Context == null && databaseFactory != null)
                Context = databaseFactory.GetContext();
        }
    }
}
