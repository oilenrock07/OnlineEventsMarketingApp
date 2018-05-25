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

        public void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _databaseFactory.GetContext().Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
