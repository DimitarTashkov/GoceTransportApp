namespace GoceTransportApp.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Models;

    public class OrganizationSeeder : ISeeder
    {
        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }

        //public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        //{
        //    if (dbContext.Organizations.Any())
        //    {
        //        return;
        //    }

        //    var founder = dbContext.Users.FirstOrDefault();

        //    if (founder == null)
        //    {
        //        return;
        //    }

        //    const string defaultImage = "/images/no-organization-image.png";

        //    var organizations = new[]
        //    {
        //        new Organization
        //        {
        //            Name = "Global Trans",
        //            Address = "14 Industrial Boulevard, Sofia",
        //            Phone = "02-555-0101",
        //            FounderId = founder.Id,
        //            ImageUrl = defaultImage,
        //            CreatedOn = DateTime.UtcNow,
        //        },
        //        new Organization
        //        {
        //            Name = "City Express",
        //            Address = "7 Central Avenue, Plovdiv",
        //            Phone = "032-555-0202",
        //            FounderId = founder.Id,
        //            ImageUrl = defaultImage,
        //            CreatedOn = DateTime.UtcNow,
        //        },
        //        new Organization
        //        {
        //            Name = "Balkan Routes",
        //            Address = "22 Freedom Street, Varna",
        //            Phone = "052-555-0303",
        //            FounderId = founder.Id,
        //            ImageUrl = defaultImage,
        //            CreatedOn = DateTime.UtcNow,
        //        },
        //    };

        //    await dbContext.Organizations.AddRangeAsync(organizations);
        //}
    }
}
