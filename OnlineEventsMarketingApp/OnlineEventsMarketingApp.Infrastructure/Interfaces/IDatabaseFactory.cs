using OnlineEventsMarketingApp.Entities.Contexts;

namespace OnlineEventsMarketingApp.Infrastructure.Interfaces
{
    public interface IDatabaseFactory
    {
        OnlineEventsContext GetContext();
    }
}
