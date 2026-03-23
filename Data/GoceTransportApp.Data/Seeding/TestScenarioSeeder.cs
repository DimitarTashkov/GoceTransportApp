namespace GoceTransportApp.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Models.Enumerations;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Seeds a complete, fully-relational test scenario into a fresh database:
    ///   Users → Cities/Streets → Organizations → Vehicles/Drivers/Routes/Schedules → Ticket/UserTicket
    ///
    /// Seeded accounts (password: Test1234!):
    ///   org1@test.com  — founder of "Express Lines"
    ///   org2@test.com  — founder of "Global Trans"
    ///   org3@test.com  — founder of "Eco Travel"
    ///   passenger@test.com — passenger with one upcoming ticket (Sofia → Plovdiv, 7 days)
    /// </summary>
    public class TestScenarioSeeder : ISeeder
    {
        private const string TestPassword = "Test1234!";
        private const string PlaceholderImage = "/images/no-organization-image.png";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            // ── Guard: run only on an empty database ─────────────────────────────────
            if (await dbContext.Organizations.AnyAsync())
            {
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ── 2.1  Users ────────────────────────────────────────────────────────────
            var owner1    = await EnsureUserAsync(userManager, "org1@test.com",       "Owner",     "One");
            var owner2    = await EnsureUserAsync(userManager, "org2@test.com",       "Owner",     "Two");
            var owner3    = await EnsureUserAsync(userManager, "org3@test.com",       "Owner",     "Three");
            var passenger = await EnsureUserAsync(userManager, "passenger@test.com",  "Test",      "Passenger");

            // ── 2.2  Cities ───────────────────────────────────────────────────────────
            var sofia    = await EnsureCityAsync(dbContext, "Sofia",    "Sofia-grad", "1000");
            var plovdiv  = await EnsureCityAsync(dbContext, "Plovdiv",  "Plovdiv",    "4000");
            var varna    = await EnsureCityAsync(dbContext, "Varna",    "Varna",      "9000");
            await dbContext.SaveChangesAsync(); // persist cities so their IDs are valid for CityStreet FKs

            // ── Streets (one per city, linked via CityStreet) ─────────────────────────
            var sofiaStreet   = await EnsureStreetWithCityAsync(dbContext, "Sofia Bus Terminal",   sofia.Id);
            var plovdivStreet = await EnsureStreetWithCityAsync(dbContext, "Plovdiv Bus Terminal", plovdiv.Id);
            var varnaStreet   = await EnsureStreetWithCityAsync(dbContext, "Varna Bus Terminal",   varna.Id);
            await dbContext.SaveChangesAsync(); // persist streets + city-street links

            // ── 2.3  Organizations ────────────────────────────────────────────────────
            var org1 = new Organization
            {
                Name = "Express Lines",
                Address = "14 Industrial Blvd, Sofia",
                Phone = "02-555-0101",
                FounderId = owner1.Id,
                ImageUrl = PlaceholderImage,
            };

            var org2 = new Organization
            {
                Name = "Global Trans",
                Address = "7 Central Ave, Plovdiv",
                Phone = "032-555-0202",
                FounderId = owner2.Id,
                ImageUrl = PlaceholderImage,
            };

            var org3 = new Organization
            {
                Name = "Eco Travel",
                Address = "22 Freedom St, Varna",
                Phone = "052-555-0303",
                FounderId = owner3.Id,
                ImageUrl = PlaceholderImage,
            };

            await dbContext.Organizations.AddRangeAsync(org1, org2, org3);
            await dbContext.SaveChangesAsync(); // persist orgs; they become the FK for vehicles/drivers/routes

            // ── 2.4  Per-org Resources & Schedules ───────────────────────────────────
            // Org1: Express Lines  — Sofia → Plovdiv  (150 km, 150 min)
            var (_, _, route1, schedule1) = await SeedOrgResourcesAsync(
                dbContext,
                org: org1,
                vehicleNumber: "CB1111AB",
                driverFirst: "Ivan",    driverLast: "Petrov",
                fromCity: sofia,        fromStreet: sofiaStreet,
                toCity:   plovdiv,      toStreet:   plovdivStreet,
                distance: 150,          duration: 150);

            // Org2: Global Trans   — Plovdiv → Varna  (250 km, 240 min)
            await SeedOrgResourcesAsync(
                dbContext,
                org: org2,
                vehicleNumber: "CB2222PB",
                driverFirst: "Georgi",  driverLast: "Stoyanov",
                fromCity: plovdiv,      fromStreet: plovdivStreet,
                toCity:   varna,        toStreet:   varnaStreet,
                distance: 250,          duration: 240);

            // Org3: Eco Travel     — Varna → Sofia    (440 km, 360 min)
            await SeedOrgResourcesAsync(
                dbContext,
                org: org3,
                vehicleNumber: "CB3333VB",
                driverFirst: "Dimitar", driverLast: "Ivanov",
                fromCity: varna,        fromStreet: varnaStreet,
                toCity:   sofia,        toStreet:   sofiaStreet,
                distance: 440,          duration: 360);

            // ── 2.5  Ticket + UserTicket for passenger ────────────────────────────────
            // The ticket is for schedule1 (Sofia → Plovdiv, departure in 7 days).
            // Expiry = departure date — once the trip date passes, the passenger
            // will be able to leave a review for "Express Lines".
            var ticket = new Ticket
            {
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = schedule1.Departure,
                Price = 35.00m,
                RouteId = route1.Id,
                OrganizationId = org1.Id,
                ScheduleId = schedule1.Id,
            };

            await dbContext.Tickets.AddAsync(ticket);
            await dbContext.SaveChangesAsync();

            await dbContext.UsersTickets.AddAsync(new UserTicket
            {
                CustomerId = passenger.Id,
                TicketId = ticket.Id,
                AvailableTickets = 1,
                IsBoarded = false,
            });

            await dbContext.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────────

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string firstName,
            string lastName)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                return existing;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                City = "Sofia",
                CreatedOn = DateTime.UtcNow,
            };

            await userManager.CreateAsync(user, TestPassword);
            return user;
        }

        private static async Task<City> EnsureCityAsync(
            ApplicationDbContext dbContext,
            string name,
            string state,
            string zip)
        {
            var existing = await dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name);
            if (existing != null)
            {
                return existing;
            }

            var city = new City { Name = name, State = state, ZipCode = zip };
            await dbContext.Cities.AddAsync(city);
            return city;
        }

        private static async Task<Street> EnsureStreetWithCityAsync(
            ApplicationDbContext dbContext,
            string name,
            Guid cityId)
        {
            var street = await dbContext.Streets.FirstOrDefaultAsync(s => s.Name == name);
            if (street == null)
            {
                street = new Street { Name = name };
                await dbContext.Streets.AddAsync(street);

                // street.Id is already set by the Street() constructor, so we can
                // add the CityStreet link immediately without an intermediate save.
                await dbContext.CitiesStreets.AddAsync(new CityStreet
                {
                    CityId = cityId,
                    StreetId = street.Id,
                });
            }
            else
            {
                // Street exists — ensure the city link is also present
                bool linkExists = await dbContext.CitiesStreets
                    .AnyAsync(cs => cs.CityId == cityId && cs.StreetId == street.Id);

                if (!linkExists)
                {
                    await dbContext.CitiesStreets.AddAsync(new CityStreet
                    {
                        CityId = cityId,
                        StreetId = street.Id,
                    });
                }
            }

            return street;
        }

        private static async Task<(Vehicle vehicle, Driver driver, Route route, Schedule schedule)>
            SeedOrgResourcesAsync(
                ApplicationDbContext dbContext,
                Organization org,
                string vehicleNumber,
                string driverFirst,
                string driverLast,
                City fromCity,
                Street fromStreet,
                City toCity,
                Street toStreet,
                double distance,
                double duration)
        {
            var vehicle = new Vehicle
            {
                Number = vehicleNumber,
                Type = "Bus",
                Manufacturer = "Mercedes",
                Model = "Travego",
                Capacity = 50,
                FuelConsumption = 18.5,
                VehicleStatus = VehicleStatus.Available,
                OrganizationId = org.Id,
            };

            var driver = new Driver
            {
                FirstName = driverFirst,
                LastName = driverLast,
                Age = 35,
                Experience = DriverExperience.Experienced,
                OrganizationId = org.Id,
            };

            var route = new Route
            {
                FromCityId = fromCity.Id,
                FromStreetId = fromStreet.Id,
                ToCityId = toCity.Id,
                ToStreetId = toStreet.Id,
                Distance = distance,
                Duration = duration,
                OrganizationId = org.Id,
            };

            await dbContext.Vehicles.AddAsync(vehicle);
            await dbContext.Drivers.AddAsync(driver);
            await dbContext.Routes.AddAsync(route);
            await dbContext.SaveChangesAsync(); // IDs needed by Schedule below

            // Schedule departs at 08:00, seven days from today
            var departure = DateTime.UtcNow.AddDays(7).Date.AddHours(8);

            var schedule = new Schedule
            {
                Day = departure.DayOfWeek,
                Departure = departure,
                Arrival = departure.AddMinutes(duration),
                OrganizationId = org.Id,
                VehicleId = vehicle.Id,
                RouteId = route.Id,
            };

            await dbContext.Schedules.AddAsync(schedule);
            await dbContext.SaveChangesAsync();

            return (vehicle, driver, route, schedule);
        }
    }
}
