namespace GoceTransportApp.Data
{
    using System.Linq;

    using GoceTransportApp.Data.Common.Models;
    using GoceTransportApp.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal static class EntityIndexesConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // IDeletableEntity.IsDeleted index
            var deletableEntityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                modelBuilder.Entity(deletableEntityType.ClrType).HasIndex(nameof(IDeletableEntity.IsDeleted));
            }

            // Performance indexes for search-heavy queries
            modelBuilder.Entity<Schedule>()
                .HasIndex(s => s.Departure);

            modelBuilder.Entity<Schedule>()
                .HasIndex(s => s.OrganizationId);

            modelBuilder.Entity<Route>()
                .HasIndex(r => new { r.FromCityId, r.ToCityId });
        }
    }
}
