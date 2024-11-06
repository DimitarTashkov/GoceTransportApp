namespace GoceTransportApp.Data.Common.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity GetById(Guid id);

        Task<TEntity> GetByIdAsync(Guid id);

        TEntity FirstOrDefault(Func<TEntity, bool> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        IQueryable<TEntity> GetAllAttached();

        IQueryable<TEntity> AllAsNoTracking();

        void Add(TEntity item);

        Task AddAsync(TEntity item);

        void AddRange(TEntity[] items);

        Task AddRangeAsync(TEntity[] items);

        Task<bool> DeleteAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity item);

        Task<int> SaveChangesAsync();
    }
}
