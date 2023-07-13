using Onyx.Models.Repositories;

namespace Onyx.Models.UnitOfWork
{
    public interface IUnitOfWork<TEntity> : IDisposable where TEntity : class
    {
        IRepository<TEntity> Repository { get; }
        Task<bool> Save();
    }
}
