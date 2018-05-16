using OnlineEventsMarketingApp.Entities.Contexts;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;

namespace OnlineEventsMarketingApp.Infrastructure.Implementations
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private OnlineEventsContext _context;

        public DatabaseFactory()
        {
        }

        public DatabaseFactory(OnlineEventsContext context)
        {
            _context = context;
        }

        public virtual OnlineEventsContext GetContext()
        {
            if (_context != null) return _context;

            _context = new OnlineEventsContext();
            return _context;
        }
    }
}
