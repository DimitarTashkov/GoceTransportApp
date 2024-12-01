namespace GoceTransportApp.Data.Common.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using GoceTransportApp.Data.Common.Models;

    public interface IDeletableEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllAsNoTrackingWithDeleted();

        Task<bool> HardDelete(TEntity entity);

        Task<bool> Undelete(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);
    }
}
