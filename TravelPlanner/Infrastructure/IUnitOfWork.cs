using System.Threading.Tasks;

namespace TravelPlanner.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}