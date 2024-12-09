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
            // Seed ApplicationUser
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@example.com", FirstName = "Alice", LastName = "Smith", ProfilePictureUrl = "/images/alice.png", City = "Springfield", CreatedOn = DateTime.Now },
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@example.com", FirstName = "Bob", LastName = "Johnson", ProfilePictureUrl = "/images/bob.png", City = "Shelbyville", CreatedOn = DateTime.Now },
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user3", Email = "user3@example.com", FirstName = "Carol", LastName = "Taylor", ProfilePictureUrl = "/images/carol.png", City = "Ogdenville", CreatedOn = DateTime.Now },
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user4", Email = "user4@example.com", FirstName = "David", LastName = "Brown", ProfilePictureUrl = "/images/david.png", City = "Capitol City", CreatedOn = DateTime.Now },
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "user5", Email = "user5@example.com", FirstName = "Eve", LastName = "Davis", ProfilePictureUrl = "/images/eve.png", City = "Springfield", CreatedOn = DateTime.Now }
            );

            // Seed City
            modelBuilder.Entity<City>().HasData(
                new City { Id = Guid.NewGuid(), Name = "Springfield", State = "Illinois", ZipCode = "62704" },
                new City { Id = Guid.NewGuid(), Name = "Shelbyville", State = "Illinois", ZipCode = "62705" },
                new City { Id = Guid.NewGuid(), Name = "Ogdenville", State = "Indiana", ZipCode = "46123" },
                new City { Id = Guid.NewGuid(), Name = "North Haverbrook", State = "Indiana", ZipCode = "46124" },
                new City { Id = Guid.NewGuid(), Name = "Capitol City", State = "Illinois", ZipCode = "62706" }
            );

            // Seed Driver
            modelBuilder.Entity<Driver>().HasData(
                new Driver { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Age = 30, AvatarUrl = "/images/john.png", Experience = DriverExperience.None, OrganizationId = Guid.NewGuid() },
                new Driver { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Age = 28, AvatarUrl = "/images/jane.png", Experience = DriverExperience.Beginner, OrganizationId = Guid.NewGuid() },
                new Driver { Id = Guid.NewGuid(), FirstName = "Mike", LastName = "Ross", Age = 35, AvatarUrl = "/images/mike.png", Experience = DriverExperience.Skilled, OrganizationId = Guid.NewGuid() },
                new Driver { Id = Guid.NewGuid(), FirstName = "Rachel", LastName = "Zane", Age = 29, AvatarUrl = "/images/rachel.png", Experience = DriverExperience.Experienced, OrganizationId = Guid.NewGuid() },
                new Driver { Id = Guid.NewGuid(), FirstName = "Harvey", LastName = "Specter", Age = 40, AvatarUrl = "/images/harvey.png", Experience = DriverExperience.Experienced, OrganizationId = Guid.NewGuid() }
            );

            // Seed Street
            modelBuilder.Entity<Street>().HasData(
                new Street { Id = Guid.NewGuid(), Name = "Main St" },
                new Street { Id = Guid.NewGuid(), Name = "Elm St" },
                new Street { Id = Guid.NewGuid(), Name = "Oak St" },
                new Street { Id = Guid.NewGuid(), Name = "Pine St" },
                new Street { Id = Guid.NewGuid(), Name = "Maple St" }
            );

            // Seed Route
            modelBuilder.Entity<Route>().HasData(
                new Route { Id = Guid.NewGuid(), FromCityId = Guid.NewGuid(), FromStreetId = Guid.NewGuid(), ToCityId = Guid.NewGuid(), ToStreetId = Guid.NewGuid(), Duration = 1.5, Distance = 100.0, OrganizationId = Guid.NewGuid() },
                new Route { Id = Guid.NewGuid(), FromCityId = Guid.NewGuid(), FromStreetId = Guid.NewGuid(), ToCityId = Guid.NewGuid(), ToStreetId = Guid.NewGuid(), Duration = 2.0, Distance = 150.0, OrganizationId = Guid.NewGuid() },
                new Route { Id = Guid.NewGuid(), FromCityId = Guid.NewGuid(), FromStreetId = Guid.NewGuid(), ToCityId = Guid.NewGuid(), ToStreetId = Guid.NewGuid(), Duration = 0.75, Distance = 50.0, OrganizationId = Guid.NewGuid() },
                new Route { Id = Guid.NewGuid(), FromCityId = Guid.NewGuid(), FromStreetId = Guid.NewGuid(), ToCityId = Guid.NewGuid(), ToStreetId = Guid.NewGuid(), Duration = 3.0, Distance = 200.0, OrganizationId = Guid.NewGuid() },
                new Route { Id = Guid.NewGuid(), FromCityId = Guid.NewGuid(), FromStreetId = Guid.NewGuid(), ToCityId = Guid.NewGuid(), ToStreetId = Guid.NewGuid(), Duration = 1.0, Distance = 75.0, OrganizationId = Guid.NewGuid() }
            );

            // Seed Schedule
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Monday, Departure = DateTime.Now.AddHours(1), Arrival = DateTime.Now.AddHours(2.5), OrganizationId = Guid.NewGuid(), VehicleId = Guid.NewGuid(), RouteId = Guid.NewGuid() },
                new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Tuesday, Departure = DateTime.Now.AddHours(2), Arrival = DateTime.Now.AddHours(4), OrganizationId = Guid.NewGuid(), VehicleId = Guid.NewGuid(), RouteId = Guid.NewGuid() },
                new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Wednesday, Departure = DateTime.Now.AddHours(3), Arrival = DateTime.Now.AddHours(5), OrganizationId = Guid.NewGuid(), VehicleId = Guid.NewGuid(), RouteId = Guid.NewGuid() },
                new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Thursday, Departure = DateTime.Now.AddHours(4), Arrival = DateTime.Now.AddHours(6), OrganizationId = Guid.NewGuid(), VehicleId = Guid.NewGuid(), RouteId = Guid.NewGuid() },
                new Schedule { Id = Guid.NewGuid(), Day = DayOfWeek.Friday, Departure = DateTime.Now.AddHours(5), Arrival = DateTime.Now.AddHours(7), OrganizationId = Guid.NewGuid(), VehicleId = Guid.NewGuid(), RouteId = Guid.NewGuid() }
            );

            // Seed Vehicle
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = Guid.NewGuid(), Number = "V001", Type = "Bus", Manufacturer = "Mercedes", Model = "Sprinter", Capacity = 20, FuelConsumption = 15.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.NewGuid() },
                new Vehicle { Id = Guid.NewGuid(), Number = "V002", Type = "Van", Manufacturer = "Ford", Model = "Transit", Capacity = 15, FuelConsumption = 12.0, VehicleStatus = VehicleStatus.Busy, OrganizationId = Guid.NewGuid() },
                new Vehicle { Id = Guid.NewGuid(), Number = "V003", Type = "Truck", Manufacturer = "Volvo", Model = "FM", Capacity = 30, FuelConsumption = 25.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.NewGuid() },
                new Vehicle { Id = Guid.NewGuid(), Number = "V004", Type = "Car", Manufacturer = "Toyota", Model = "Corolla", Capacity = 5, FuelConsumption = 8.0, VehicleStatus = VehicleStatus.UnderMaintenance, OrganizationId = Guid.NewGuid() },
                new Vehicle { Id = Guid.NewGuid(), Number = "V005", Type = "Minibus", Manufacturer = "Volkswagen", Model = "Crafter", Capacity = 10, FuelConsumption = 10.0, VehicleStatus = VehicleStatus.Available, OrganizationId = Guid.NewGuid() }
            );
            // Seed Ticket
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-2), ExpiryDate = DateTime.Now.AddMonths(1), Price = 15.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
                new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-1), ExpiryDate = DateTime.Now.AddMonths(1), Price = 20.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
                new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(1), Price = 18.5m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
                new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-3), ExpiryDate = DateTime.Now.AddMonths(1), Price = 22.0m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() },
                new Ticket { Id = Guid.NewGuid(), IssuedDate = DateTime.Now.AddDays(-4), ExpiryDate = DateTime.Now.AddMonths(1), Price = 12.5m, RouteId = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), ScheduleId = Guid.NewGuid() }
            );

            // Seed UserTicket
            modelBuilder.Entity<UserTicket>().HasData(
                new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 1 },
                new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 2 },
                new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 3 },
                new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 1 },
                new UserTicket { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid().ToString(), TicketId = Guid.NewGuid(), AvailableTickets = 2 }
            );

            // Seed CityStreet
            modelBuilder.Entity<CityStreet>().HasData(
                new CityStreet { Id = Guid.NewGuid(), CityId = Guid.NewGuid(), StreetId = Guid.NewGuid() },
                new CityStreet { Id = Guid.NewGuid(), CityId = Guid.NewGuid(), StreetId = Guid.NewGuid() },
                new CityStreet { Id = Guid.NewGuid(), CityId = Guid.NewGuid(), StreetId = Guid.NewGuid() },
                new CityStreet { Id = Guid.NewGuid(), CityId = Guid.NewGuid(), StreetId = Guid.NewGuid() },
                new CityStreet { Id = Guid.NewGuid(), CityId = Guid.NewGuid(), StreetId = Guid.NewGuid() }
            );

            // Additional Entries for Related Tables
            // Replace all Guid.NewGuid() with valid references to entities already seeded.

            // Organization (Example only, since not explicitly detailed in the provided classes)
            modelBuilder.Entity<Organization>().HasData(
                new Organization { Id = Guid.NewGuid(), Name = "Transport Co.", CreatedOn = DateTime.Now },
                new Organization { Id = Guid.NewGuid(), Name = "Express Travels", CreatedOn = DateTime.Now },
                new Organization { Id = Guid.NewGuid(), Name = "Safe Rides", CreatedOn = DateTime.Now },
                new Organization { Id = Guid.NewGuid(), Name = "Urban Transport", CreatedOn = DateTime.Now },
                new Organization { Id = Guid.NewGuid(), Name = "Comfy Travels", CreatedOn = DateTime.Now }
            );

        }
    }
}
