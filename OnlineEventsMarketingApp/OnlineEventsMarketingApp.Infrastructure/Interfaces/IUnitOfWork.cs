
namespace OnlineEventsMarketingApp.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        void Commit();
        void ExecuteSqlCommand(string command, params object[] parameters);
    }
}
