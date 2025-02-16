namespace GoceTransportApp.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Models;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Models.Enumerations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using static GoceTransportApp.Common.GlobalConstants;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Street> Streets { get; set; }

        public DbSet<CityStreet> CitiesStreets { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<UserTicket> UsersTickets { get; set; }

        public DbSet<ContactForm> ContactForms { get; set; }


        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set Vehichle's Number to unique constraint
            builder.Entity<Vehicle>().HasIndex(v => v.Number).IsUnique();

            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            this.ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            this.SeedAll(builder);
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Applies configurations
        private void ConfigureUserIdentityRelations(ModelBuilder builder)
             => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }

        private void SeedAll(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApplicationUser>().HasData(
            //    new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@example.com", FirstName = "Alice", LastName = "Smith", ProfilePictureUrl = DefaultProfileImageUrl, City = "Gotse Delchev", CreatedOn = DateTime.Now },
            //    new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@example.com", FirstName = "Bob", LastName = "Johnson", ProfilePictureUrl = DefaultProfileImageUrl, City = "Mosomishte", CreatedOn = DateTime.Now },
            //    new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user3", Email = "user3@example.com", FirstName = "Carol", LastName = "Taylor", ProfilePictureUrl = DefaultProfileImageUrl, City = "Borovo", CreatedOn = DateTime.Now },
            //    new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user4", Email = "user4@example.com", FirstName = "David", LastName = "Brown", ProfilePictureUrl = DefaultProfileImageUrl, City = "Lqski", CreatedOn = DateTime.Now },
            //    new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user5", Email = "user5@example.com", FirstName = "Eve", LastName = "Davis", ProfilePictureUrl = DefaultProfileImageUrl, City = "Mosomishte", CreatedOn = DateTime.Now }
            //);

            //modelBuilder.Entity<City>().HasData(
            //    new City { Id = Guid.NewGuid(), Name = "Springfield", State = "Illinois", ZipCode = "62704" },
            //    new City { Id = Guid.NewGuid(), Name = "Shelbyville", State = "Illinois", ZipCode = "62705" },
            //    new City { Id = Guid.NewGuid(), Name = "Ogdenville", State = "Indiana", ZipCode = "46123" },
            //    new City { Id = Guid.NewGuid(), Name = "North Haverbrook", State = "Indiana", ZipCode = "46124" },
            //    new City { Id = Guid.NewGuid(), Name = "Capitol City", State = "Illinois", ZipCode = "62706" }
            //);

            //modelBuilder.Entity<Driver>().HasData(
            //    new Driver { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Age = 30, AvatarUrl = DefaultProfileImageUrl, Experience = DriverExperience.None, OrganizationId = Guid.Parse("6b040313-77f4-49b0-b3d3-b25de95eb408") },
            //    new Driver { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Age = 28, AvatarUrl = DefaultProfileImageUrl, Experience = DriverExperience.Beginner, OrganizationId = Guid.Parse("00591d7b-5bf5-475b-ac85-4a2467481881") },
            //    new Driver { Id = Guid.NewGuid(), FirstName = "Mike", LastName = "Ross", Age = 35, AvatarUrl = DefaultProfileImageUrl, Experience = DriverExperience.Skilled, OrganizationId = Guid.Parse("c40f52d6-d2cc-4379-8acf-782558dba84c") },
            //    new Driver { Id = Guid.NewGuid(), FirstName = "Rachel", LastName = "Zane", Age = 29, AvatarUrl = DefaultProfileImageUrl, Experience = DriverExperience.Experienced, OrganizationId = Guid.Parse("a982c9b9-c1bc-4a5b-a89a-4c53a701c526") },
            //    new Driver { Id = Guid.NewGuid(), FirstName = "Harvey", LastName = "Specter", Age = 40, AvatarUrl = DefaultProfileImageUrl, Experience = DriverExperience.Experienced, OrganizationId = Guid.Parse("4b5ab8a3-7604-447a-a603-f8e97e45fb61") }
            //);

            //modelBuilder.Entity<Street>().HasData(
            //    new Street { Id = Guid.NewGuid(), Name = "Qntra" },
            //    new Street { Id = Guid.NewGuid(), Name = "Tsaritsa Joanna" },
            //    new Street { Id = Guid.NewGuid(), Name = "Dimitar Talev" },
            //    new Street { Id = Guid.NewGuid(), Name = "Polkovnik Drangov" },
            //    new Street { Id = Guid.NewGuid(), Name = "Marica" }
            //);

            //modelBuilder.Entity<Route>().HasData(
            //    new Route { Id = Guid.NewGuid(), FromCityId = Guid.Parse("A2326550-D8B7-48CD-AFAB-370F702FEBB5"), FromStreetId = Guid.Parse("5BA431EF-D79A-4AD6-BCC6-9C9F382FAEC7"), ToCityId = Guid.Parse("0BB97157-9F48-4019-9D57-4F8F08F56EA4"), ToStreetId = Guid.Parse("C42A1B2E-AFBE-4F81-8D68-AAAC514F87AF"), Duration = 1.5, Distance = 100.0, OrganizationId = Guid.Parse("4b5ab8a3-7604-447a-a603-f8e97e45fb61") },
            //    new Route { Id = Guid.NewGuid(), FromCityId = Guid.Parse("0BB97157-9F48-4019-9D57-4F8F08F56EA4"), FromStreetId = Guid.Parse("C42A1B2E-AFBE-4F81-8D68-AAAC514F87AF"), ToCityId = Guid.Parse("CD658FE2-309D-4DE5-8EAE-5AF9BD2DD637"), ToStreetId = Guid.Parse("3B46A044-4BB2-4AD1-86F0-B198719E9137"), Duration = 2.0, Distance = 150.0, OrganizationId = Guid.Parse("a982c9b9-c1bc-4a5b-a89a-4c53a701c526") },
            //    new Route { Id = Guid.NewGuid(), FromCityId = Guid.Parse("CD658FE2-309D-4DE5-8EAE-5AF9BD2DD637"), FromStreetId = Guid.Parse("3B46A044-4BB2-4AD1-86F0-B198719E9137"), ToCityId = Guid.Parse("EBF4BE4C-B944-4A60-8820-CE9ADD91C913"), ToStreetId = Guid.Parse("48546194-3244-4C88-B55B-B3F1A10CFAE4"), Duration = 0.75, Distance = 50.0, OrganizationId = Guid.Parse("c40f52d6-d2cc-4379-8acf-782558dba84c") },
            //    new Route { Id = Guid.NewGuid(), FromCityId = Guid.Parse("EBF4BE4C-B944-4A60-8820-CE9ADD91C913"), FromStreetId = Guid.Parse("48546194-3244-4C88-B55B-B3F1A10CFAE4"), ToCityId = Guid.Parse("6EE1013B-9EF3-4ABD-9F91-EE39A1BDC3BF"), ToStreetId = Guid.Parse("A64D591D-D8CF-488D-AD9A-DFB3A6962450"), Duration = 3.0, Distance = 200.0, OrganizationId = Guid.Parse("00591d7b-5bf5-475b-ac85-4a2467481881") },
            //    new Route { Id = Guid.NewGuid(), FromCityId = Guid.Parse("6EE1013B-9EF3-4ABD-9F91-EE39A1BDC3BF"), FromStreetId = Guid.Parse("A64D591D-D8CF-488D-AD9A-DFB3A6962450"), ToCityId = Guid.Parse("A2326550-D8B7-48CD-AFAB-370F702FEBB5"), ToStreetId = Guid.Parse("5BA431EF-D79A-4AD6-BCC6-9C9F382FAEC7"), Duration = 1.0, Distance = 75.0, OrganizationId = Guid.Parse("6b040313-77f4-49b0-b3d3-b25de95eb408") }
            //);

            //modelBuilder.Entity<Schedule>().HasData(
            //    new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Monday, Departure = DateTime.Now.AddHours(1), Arrival = DateTime.Now.AddHours(2.5), OrganizationId = Guid.Parse("4b5ab8a3-7604-447a-a603-f8e97e45fb61"), VehicleId = Guid.NewGuid(), RouteId = Guid.Parse("") },
            //    new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Tuesday, Departure = DateTime.Now.AddHours(2), Arrival = DateTime.Now.AddHours(4), OrganizationId = Guid.Parse(""), VehicleId = Guid.NewGuid(), RouteId = Guid.Parse("") },
            //    new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Wednesday, Departure = DateTime.Now.AddHours(3), Arrival = DateTime.Now.AddHours(5), OrganizationId = Guid.Parse(""), VehicleId = Guid.NewGuid(), RouteId = Guid.Parse("") },
            //    new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Thursday, Departure = DateTime.Now.AddHours(4), Arrival = DateTime.Now.AddHours(6), OrganizationId = Guid.Parse(""), VehicleId = Guid.NewGuid(), RouteId = Guid.Parse("") },
            //    new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Friday, Departure = DateTime.Now.AddHours(5), Arrival = DateTime.Now.AddHours(7), OrganizationId = Guid.Parse(""), VehicleId = Guid.NewGuid(), RouteId = Guid.Parse("") }
            //);

            //modelBuilder.Entity<Vehicle>().HasData(
            //    new Vehicle { Id = Guid.NewGuid(), Number = "V001", Type = "Bus", Manufacturer = "Mercedes", Model = "Sprinter", Capacity = 20, FuelConsumption = 15.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.Parse("4b5ab8a3-7604-447a-a603-f8e97e45fb61") },
            //    new Vehicle { Id = Guid.NewGuid(), Number = "V002", Type = "Van", Manufacturer = "Ford", Model = "Transit", Capacity = 15, FuelConsumption = 12.0, VehicleStatus = VehicleStatus.Busy, OrganizationId = Guid.Parse("a982c9b9-c1bc-4a5b-a89a-4c53a701c526") },
            //    new Vehicle { Id = Guid.NewGuid(), Number = "V003", Type = "Truck", Manufacturer = "Volvo", Model = "FM", Capacity = 30, FuelConsumption = 25.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.Parse("c40f52d6-d2cc-4379-8acf-782558dba84c") },
            //    new Vehicle { Id = Guid.NewGuid(), Number = "V004", Type = "Car", Manufacturer = "Toyota", Model = "Corolla", Capacity = 5, FuelConsumption = 8.0, VehicleStatus = VehicleStatus.UnderMaintenance, OrganizationId = Guid.Parse("00591d7b-5bf5-475b-ac85-4a2467481881") },
            //    new Vehicle { Id = Guid.NewGuid(), Number = "V005", Type = "Minibus", Manufacturer = "Volkswagen", Model = "Crafter", Capacity = 10, FuelConsumption = 10.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.Parse("6b040313-77f4-49b0-b3d3-b25de95eb408") }
            //);

            //modelBuilder.Entity<Ticket>().HasData(
            //    new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-2), ExpiryDate = DateTime.Now.AddMonths(1), Price = 15.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
            //    new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-1), ExpiryDate = DateTime.Now.AddMonths(1), Price = 20.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
            //    new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(1), Price = 18.5m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
            //    new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-3), ExpiryDate = DateTime.Now.AddMonths(1), Price = 22.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
            //    new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-4), ExpiryDate = DateTime.Now.AddMonths(1), Price = 12.5m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() }
            //);

            //modelBuilder.Entity<UserTicket>().HasData(
            //    new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 1 },
            //    new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 2 },
            //    new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 3 },
            //    new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 1 },
            //    new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 2 }
            //);

            //modelBuilder.Entity<CityStreet>().HasData(
            //    new CityStreet { CityId = Guid.Parse("A2326550-D8B7-48CD-AFAB-370F702FEBB5"), StreetId = Guid.Parse("5BA431EF-D79A-4AD6-BCC6-9C9F382FAEC7") },
            //    new CityStreet { CityId = Guid.Parse("0BB97157-9F48-4019-9D57-4F8F08F56EA4"), StreetId = Guid.Parse("C42A1B2E-AFBE-4F81-8D68-AAAC514F87AF") },
            //    new CityStreet { CityId = Guid.Parse("CD658FE2-309D-4DE5-8EAE-5AF9BD2DD637"), StreetId = Guid.Parse("3B46A044-4BB2-4AD1-86F0-B198719E9137") },
            //    new CityStreet { CityId = Guid.Parse("EBF4BE4C-B944-4A60-8820-CE9ADD91C913"), StreetId = Guid.Parse("48546194-3244-4C88-B55B-B3F1A10CFAE4") },
            //    new CityStreet { CityId = Guid.Parse("6EE1013B-9EF3-4ABD-9F91-EE39A1BDC3BF"), StreetId = Guid.Parse("A64D591D-D8CF-488D-AD9A-DFB3A6962450") }
            //);

            //// Additional Entries for Related Tables
            //// Replace all Guid.NewGuid() with valid references to entities already seeded.

            // Organization (Example only, since not explicitly detailed in the provided classes)
        //    modelBuilder.Entity<Organization>().HasData(
        //        new Organization
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = "City Transport Co.",
        //            Address = "123 Main St, Springfield",
        //            FounderId = "4b5ab8a3-7604-447a-a603-f8e97e45fb61",
        //            Phone = "+1 555-1234",
        //            CreatedOn = DateTime.Now
        //        },
        //new Organization
        //{
        //    Id = Guid.NewGuid(),
        //    Name = "Express Logistics",
        //    Address = "456 Elm St, Shelbyville",
        //    FounderId = "a982c9b9-c1bc-4a5b-a89a-4c53a701c526",
        //    Phone = "+1 555-5678",
        //    CreatedOn = DateTime.Now
        //},
        //new Organization
        //{
        //    Id = Guid.NewGuid(),
        //    Name = "Safe Travels",
        //    Address = "789 Oak St, Ogdenville",
        //    FounderId = "c40f52d6-d2cc-4379-8acf-782558dba84c",
        //    Phone = "+1 555-9012",
        //    CreatedOn = DateTime.Now
        //},
        //new Organization
        //{
        //    Id = Guid.NewGuid(),
        //    Name = "Urban Express",
        //    Address = "321 Pine St, North Haverbrook",
        //    FounderId = "00591d7b-5bf5-475b-ac85-4a2467481881",
        //    Phone = "+1 555-3456",
        //    CreatedOn = DateTime.Now
        //},
        //new Organization
        //{
        //    Id = Guid.NewGuid(),
        //    Name = "Comfy Rides",
        //    Address = "654 Maple St, Capitol City",
        //    FounderId = "6b040313-77f4-49b0-b3d3-b25de95eb408",
        //    Phone = "+1 555-7890",
        //    CreatedOn = DateTime.Now
        //}
        //    );

        }
    }
}
