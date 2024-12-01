namespace GoceTransportApp.Data.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GoceTransportApp.Data.Common.Models;
    using GoceTransportApp.Data.Common.Repositories;

    using Microsoft.EntityFrameworkCore;

    public class EfDeletableEntityRepository<TEntity> : EfRepository<TEntity>, IDeletableEntityRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        public EfDeletableEntityRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public override IQueryable<TEntity> GetAll() => base.GetAllAttached().Where(x => !x.IsDeleted);

        public override IQueryable<TEntity> AllAsNoTracking() => base.AllAsNoTracking().Where(x => !x.IsDeleted);

        public IQueryable<TEntity> AllWithDeleted() => base.GetAllAttached().IgnoreQueryFilters();

        public IQueryable<TEntity> AllAsNoTrackingWithDeleted() => base.AllAsNoTracking().IgnoreQueryFilters();

        public async Task<bool> HardDelete(TEntity entity) => await base.DeleteAsync(entity);

        public async Task<bool> Undelete(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.DeletedOn = null;
            bool isUndeleted = await this.UpdateAsync(entity);

            return isUndeleted;
        }

        public async override Task<bool> DeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            bool isDeleted = await this.UpdateAsync(entity);

            return isDeleted;
        }
    }
}
