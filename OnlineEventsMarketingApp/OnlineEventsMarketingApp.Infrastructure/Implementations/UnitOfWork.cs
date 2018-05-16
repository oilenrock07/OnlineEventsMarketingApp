using OnlineEventsMarketingApp.Infrastructure.Interfaces;

namespace OnlineEventsMarketingApp.Infrastructure.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory _databaseFactory;

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public void Commit()
        {
            _databaseFactory.GetContext().SaveChanges();
        }
    }
}
