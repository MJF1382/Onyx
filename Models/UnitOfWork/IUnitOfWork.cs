using Onyx.Models.Repositories;

namespace Onyx.Models.UnitOfWork
{
    public interface IUnitOfWork<TEntity> where TEntity : class
    {
        IRepository<TEntity> Repository { get; }
        Task<bool> Save();
    }
}
