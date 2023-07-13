using Onyx.Models.Database;
using Onyx.Models.Repositories;

namespace Onyx.Models.UnitOfWork
{
    public class UnitOfWork<TEntity> : IUnitOfWork<TEntity> where TEntity : class
    {
        private readonly OnyxDBContext _context;

        public UnitOfWork(OnyxDBContext context)
        {
            _context = context;
        }

        public IRepository<TEntity> Repository => new Repository<TEntity>(_context);

        public async Task<bool> SaveAsync()
        {
            return Convert.ToBoolean(await _context.SaveChangesAsync());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
