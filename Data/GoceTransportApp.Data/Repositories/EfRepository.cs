namespace GoceTransportApp.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Repositories;

    using Microsoft.EntityFrameworkCore;

    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public EfRepository(ApplicationDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.DbSet = this.Context.Set<TEntity>();
        }

        protected DbSet<TEntity> DbSet { get; set; }

        protected ApplicationDbContext Context { get; set; }

        public virtual IEnumerable<TEntity> GetAll() => this.DbSet.ToArray();

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() => await this.DbSet.ToArrayAsync();

        public virtual IQueryable<TEntity> GetAllAttached() => this.DbSet.AsQueryable();

        public virtual IQueryable<TEntity> AllAsNoTracking() => this.DbSet.AsNoTracking();

        public virtual Task AddAsync(TEntity entity) => this.DbSet.AddAsync(entity).AsTask();

        public virtual void Update(TEntity entity)
        {
            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity) => this.DbSet.Remove(entity);

        public Task<int> SaveChangesAsync() => this.Context.SaveChangesAsync();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TEntity GetById(Guid id)
        {
            TEntity entity = this.DbSet
                .Find(id);

            return entity;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            TEntity entity = await this.DbSet
                .FindAsync(id);

            return entity;
        }

        public TEntity FirstOrDefault(Func<TEntity, bool> predicate)
        {
            TEntity entity = this.DbSet
                .FirstOrDefault(predicate);

            return entity;
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entity = await this.DbSet
                .FirstOrDefaultAsync(predicate);

            return entity;
        }

        public void Add(TEntity item)
        {
            throw new NotImplementedException();
        }

        public void AddRange(TEntity[] items)
        {
            this.DbSet.AddRange(items);
            this.Context.SaveChanges();
        }

        public async Task AddRangeAsync(TEntity[] items)
        {
            await this.DbSet.AddRangeAsync(items);
            await this.Context.SaveChangesAsync();
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            this.DbSet.Remove(entity);
            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(TEntity item)
        {
            try
            {
                var entry = this.Context.Entry(item);
                if (entry.State == EntityState.Detached)
                {
                    this.DbSet.Attach(item);
                }

                entry.State = EntityState.Modified;
                await this.Context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context?.Dispose();
            }
        }
    }
}
