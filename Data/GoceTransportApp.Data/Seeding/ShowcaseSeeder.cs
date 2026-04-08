namespace GoceTransportApp.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Models.Enumerations;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Seeds 15+ Bulgarian cities and a showcase Pro-tier organization
    /// ("Балкан Експрес") so visitors can see what a paid plan looks like:
    ///   – multiple routes with intermediate stops and GPS coordinates
    ///   – daily + weekday recurring schedules
    ///   – several vehicles and drivers
    /// </summary>
    public class ShowcaseSeeder : ISeeder
    {
        private const string ShowcaseOrgName  = "Балкан Експрес";
        private const string PlaceholderImage = "/images/no-organization-image.png";
        private const string AdminEmail       = "mitkoadmin@gmail.com";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            // Guard: run only once
            if (await dbContext.Organizations.AnyAsync(o => o.Name == ShowcaseOrgName))
            {
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ── 1. Ensure admin user has Pro tier ────────────────────────────────────
            var admin = await userManager.FindByEmailAsync(AdminEmail);
            if (admin == null)
            {
                return; // AdminSeeder must run first
            }

            if (admin.MembershipTier != MembershipTier.Pro)
            {
                admin.MembershipTier = MembershipTier.Pro;
                await userManager.UpdateAsync(admin);
            }

            // ── 2. Seed Bulgarian cities ─────────────────────────────────────────────
            var sofia         = await EnsureCityAsync(dbContext, "София",          "Sofia-grad", "1000");
            var plovdiv       = await EnsureCityAsync(dbContext, "Пловдив",        "Plovdiv",    "4000");
            var varna         = await EnsureCityAsync(dbContext, "Варна",          "Varna",      "9000");
            var burgas        = await EnsureCityAsync(dbContext, "Бургас",         "Burgas",     "8000");
            var staraZagora   = await EnsureCityAsync(dbContext, "Стара Загора",   "Stara Zagora","6000");
            var ruse          = await EnsureCityAsync(dbContext, "Русе",           "Ruse",       "7000");
            var pleven        = await EnsureCityAsync(dbContext, "Плевен",         "Pleven",     "5800");
            var velikoTarnovo = await EnsureCityAsync(dbContext, "Велико Търново", "Veliko Tarnovo","5000");
            var blagoevgrad   = await EnsureCityAsync(dbContext, "Благоевград",   "Blagoevgrad","2700");
            var shumen        = await EnsureCityAsync(dbContext, "Шумен",          "Shumen",     "9700");
            var dobrich       = await EnsureCityAsync(dbContext, "Добрич",         "Dobrich",    "9300");
            var haskovo       = await EnsureCityAsync(dbContext, "Хасково",        "Haskovo",    "6300");
            var pazardzhik    = await EnsureCityAsync(dbContext, "Пазарджик",      "Pazardzhik", "4400");
            var kardzhali     = await EnsureCityAsync(dbContext, "Кърджали",       "Kardzhali",  "6600");
            var sliven        = await EnsureCityAsync(dbContext, "Сливен",         "Sliven",     "8800");
            var gabrovo       = await EnsureCityAsync(dbContext, "Габрово",        "Gabrovo",    "5300");
            var pernik        = await EnsureCityAsync(dbContext, "Перник",         "Pernik",     "2300");

            await dbContext.SaveChangesAsync();

            // ── 3. Streets (bus terminals for the three main showcase routes) ─────────
            var sofiaBus       = await EnsureStreetWithCityAsync(dbContext, "Централна Автогара",     sofia.Id);
            var plovdivBus     = await EnsureStreetWithCityAsync(dbContext, "Автогара Пловдив",       plovdiv.Id);
            var varnaBus       = await EnsureStreetWithCityAsync(dbContext, "Автогара Варна",         varna.Id);
            var burgasBus      = await EnsureStreetWithCityAsync(dbContext, "Автогара Бургас",        burgas.Id);
            var staraZagoraBus = await EnsureStreetWithCityAsync(dbContext, "Автогара Стара Загора",  staraZagora.Id);
            var shumenBus      = await EnsureStreetWithCityAsync(dbContext, "Автогара Шумен",         shumen.Id);

            // Extra city streets (no routes needed — just ensure the cities have them)
            await EnsureStreetWithCityAsync(dbContext, "Автогара Русе",             ruse.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Плевен",           pleven.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Велико Търново",   velikoTarnovo.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Благоевград",      blagoevgrad.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Добрич",           dobrich.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Хасково",          haskovo.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Пазарджик",        pazardzhik.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Кърджали",         kardzhali.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Сливен",           sliven.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Габрово",          gabrovo.Id);
            await EnsureStreetWithCityAsync(dbContext, "Автогара Перник",           pernik.Id);

            await dbContext.SaveChangesAsync();

            // ── 4. Showcase organization ─────────────────────────────────────────────
            var org = new Organization
            {
                Name      = ShowcaseOrgName,
                Address   = "бул. Витоша 1, София",
                Phone     = "02-988-7777",
                FounderId = admin.Id,
                ImageUrl  = PlaceholderImage,
                IsOnTrial = false,
                CreatedOn = DateTime.UtcNow,
            };

            await dbContext.Organizations.AddAsync(org);
            await dbContext.SaveChangesAsync();

            // ── 5. Vehicles ───────────────────────────────────────────────────────────
            var bus1 = CreateVehicle("СА1001АА", "Автобус", "Setra",    "S 516 HDH",  52, 26.0, org.Id);
            var bus2 = CreateVehicle("СА2002АА", "Автобус", "Neoplan",  "Cityliner",  48, 24.5, org.Id);
            var bus3 = CreateVehicle("СА3003АА", "Автобус", "Mercedes", "Tourismo",   50, 27.0, org.Id);

            await dbContext.Vehicles.AddRangeAsync(bus1, bus2, bus3);

            // ── 6. Drivers ────────────────────────────────────────────────────────────
            var driver1 = CreateDriver("Александър", "Богданов",  42, DriverExperience.Experienced, org.Id);
            var driver2 = CreateDriver("Стефан",     "Маринов",   38, DriverExperience.Experienced, org.Id);
            var driver3 = CreateDriver("Красимир",   "Тодоров",   35, DriverExperience.Skilled,     org.Id);
            var driver4 = CreateDriver("Николай",    "Иванов",    29, DriverExperience.Beginner,    org.Id);

            await dbContext.Drivers.AddRangeAsync(driver1, driver2, driver3, driver4);
            await dbContext.SaveChangesAsync();

            // ── 7. Routes with intermediate stops ────────────────────────────────────

            // Route A: София → Пловдив  (147 km, 150 min)
            var routeSofiaPlovdiv = new Route
            {
                FromCityId     = sofia.Id,
                FromStreetId   = sofiaBus.Id,
                ToCityId       = plovdiv.Id,
                ToStreetId     = plovdivBus.Id,
                Distance       = 147,
                Duration       = 150,
                OrganizationId = org.Id,
            };

            // Route B: Пловдив → Варна  (296 km, 240 min)
            var routePlovdivVarna = new Route
            {
                FromCityId     = plovdiv.Id,
                FromStreetId   = plovdivBus.Id,
                ToCityId       = varna.Id,
                ToStreetId     = varnaBus.Id,
                Distance       = 296,
                Duration       = 240,
                OrganizationId = org.Id,
            };

            // Route C: Стара Загора → Бургас  (116 km, 95 min)
            var routeStaraZagoraBurgas = new Route
            {
                FromCityId     = staraZagora.Id,
                FromStreetId   = staraZagoraBus.Id,
                ToCityId       = burgas.Id,
                ToStreetId     = burgasBus.Id,
                Distance       = 116,
                Duration       = 95,
                OrganizationId = org.Id,
            };

            await dbContext.Routes.AddRangeAsync(routeSofiaPlovdiv, routePlovdivVarna, routeStaraZagoraBurgas);
            await dbContext.SaveChangesAsync();

            // Route A stops — Sofia → Ihtiman → Plovdiv
            var stopsA = new List<RouteStop>
            {
                new RouteStop
                {
                    Name          = "Ихтиман",
                    Order         = 1,
                    ArrivalTime   = new TimeSpan(0, 40, 0),
                    DepartureTime = new TimeSpan(0, 45, 0),
                    Latitude      = 42.4339,
                    Longitude     = 23.8160,
                    RouteId       = routeSofiaPlovdiv.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
                new RouteStop
                {
                    Name          = "Вакарел",
                    Order         = 2,
                    ArrivalTime   = new TimeSpan(0, 55, 0),
                    DepartureTime = new TimeSpan(1, 0, 0),
                    Latitude      = 42.5420,
                    Longitude     = 23.7050,
                    RouteId       = routeSofiaPlovdiv.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
                new RouteStop
                {
                    Name          = "Пазарджик",
                    Order         = 3,
                    ArrivalTime   = new TimeSpan(2, 0, 0),
                    DepartureTime = new TimeSpan(2, 5, 0),
                    Latitude      = 42.1928,
                    Longitude     = 24.3337,
                    RouteId       = routeSofiaPlovdiv.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
            };

            // Route B stops — Plovdiv → Karlovo → Sliven → Shumen → Varna
            var stopsB = new List<RouteStop>
            {
                new RouteStop
                {
                    Name          = "Карлово",
                    Order         = 1,
                    ArrivalTime   = new TimeSpan(0, 50, 0),
                    DepartureTime = new TimeSpan(0, 55, 0),
                    Latitude      = 42.6431,
                    Longitude     = 24.8082,
                    RouteId       = routePlovdivVarna.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
                new RouteStop
                {
                    Name          = "Сливен",
                    Order         = 2,
                    ArrivalTime   = new TimeSpan(1, 50, 0),
                    DepartureTime = new TimeSpan(1, 55, 0),
                    Latitude      = 42.6817,
                    Longitude     = 26.3228,
                    RouteId       = routePlovdivVarna.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
                new RouteStop
                {
                    Name          = "Шумен",
                    Order         = 3,
                    ArrivalTime   = new TimeSpan(3, 0, 0),
                    DepartureTime = new TimeSpan(3, 5, 0),
                    Latitude      = 43.2706,
                    Longitude     = 26.9262,
                    RouteId       = routePlovdivVarna.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
            };

            // Route C stops — Stara Zagora → Aytos → Burgas
            var stopsC = new List<RouteStop>
            {
                new RouteStop
                {
                    Name          = "Нова Загора",
                    Order         = 1,
                    ArrivalTime   = new TimeSpan(0, 30, 0),
                    DepartureTime = new TimeSpan(0, 33, 0),
                    Latitude      = 42.4897,
                    Longitude     = 26.0117,
                    RouteId       = routeStaraZagoraBurgas.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
                new RouteStop
                {
                    Name          = "Айтос",
                    Order         = 2,
                    ArrivalTime   = new TimeSpan(1, 15, 0),
                    DepartureTime = new TimeSpan(1, 18, 0),
                    Latitude      = 42.7055,
                    Longitude     = 27.2524,
                    RouteId       = routeStaraZagoraBurgas.Id,
                    CreatedOn     = DateTime.UtcNow,
                },
            };

            await dbContext.RouteStops.AddRangeAsync(stopsA);
            await dbContext.RouteStops.AddRangeAsync(stopsB);
            await dbContext.RouteStops.AddRangeAsync(stopsC);
            await dbContext.SaveChangesAsync();

            // ── 8. Schedules ─────────────────────────────────────────────────────────
            // Sofia → Plovdiv: Daily 07:00 → 09:30
            var depSP1 = Today(7, 0);
            await dbContext.Schedules.AddAsync(new Schedule
            {
                Day               = DayOfWeek.Monday,
                Departure         = depSP1,
                Arrival           = depSP1.AddMinutes(150),
                RecurrencePattern = RecurrencePattern.Daily,
                OrganizationId    = org.Id,
                VehicleId         = bus1.Id,
                RouteId           = routeSofiaPlovdiv.Id,
            });

            // Sofia → Plovdiv: Daily 14:00 → 16:30
            var depSP2 = Today(14, 0);
            await dbContext.Schedules.AddAsync(new Schedule
            {
                Day               = DayOfWeek.Monday,
                Departure         = depSP2,
                Arrival           = depSP2.AddMinutes(150),
                RecurrencePattern = RecurrencePattern.Daily,
                OrganizationId    = org.Id,
                VehicleId         = bus2.Id,
                RouteId           = routeSofiaPlovdiv.Id,
            });

            // Plovdiv → Varna: Weekdays 06:00 → 10:00
            var depPV = Today(6, 0);
            await dbContext.Schedules.AddAsync(new Schedule
            {
                Day               = DayOfWeek.Monday,
                Departure         = depPV,
                Arrival           = depPV.AddMinutes(240),
                RecurrencePattern = RecurrencePattern.Weekdays,
                OrganizationId    = org.Id,
                VehicleId         = bus3.Id,
                RouteId           = routePlovdivVarna.Id,
            });

            // Stara Zagora → Burgas: Weekends 08:00 → 09:35
            var depSZB = Today(8, 0);
            await dbContext.Schedules.AddAsync(new Schedule
            {
                Day               = DayOfWeek.Saturday,
                Departure         = depSZB,
                Arrival           = depSZB.AddMinutes(95),
                RecurrencePattern = RecurrencePattern.Weekends,
                OrganizationId    = org.Id,
                VehicleId         = bus1.Id,
                RouteId           = routeStaraZagoraBurgas.Id,
            });

            // Stara Zagora → Burgas: Specific day (Sunday) 16:00 → 17:35
            var depSZB2 = Today(16, 0);
            await dbContext.Schedules.AddAsync(new Schedule
            {
                Day               = DayOfWeek.Sunday,
                Departure         = depSZB2,
                Arrival           = depSZB2.AddMinutes(95),
                RecurrencePattern = RecurrencePattern.SpecificDay,
                OrganizationId    = org.Id,
                VehicleId         = bus2.Id,
                RouteId           = routeStaraZagoraBurgas.Id,
            });

            await dbContext.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────────

        private static DateTime Today(int hour, int minute)
            => DateTime.UtcNow.Date.AddHours(hour).AddMinutes(minute);

        private static Vehicle CreateVehicle(
            string number, string type, string manufacturer, string model,
            int capacity, double fuel, Guid orgId) => new Vehicle
        {
            Number         = number,
            Type           = type,
            Manufacturer   = manufacturer,
            Model          = model,
            Capacity       = capacity,
            FuelConsumption = fuel,
            VehicleStatus  = VehicleStatus.Available,
            OrganizationId = orgId,
        };

        private static Driver CreateDriver(
            string first, string last, int age,
            DriverExperience experience, Guid orgId) => new Driver
        {
            FirstName      = first,
            LastName       = last,
            Age            = age,
            Experience     = experience,
            OrganizationId = orgId,
        };

        private static async Task<City> EnsureCityAsync(
            ApplicationDbContext dbContext,
            string name, string state, string zip)
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
            string name, Guid cityId)
        {
            var street = await dbContext.Streets.FirstOrDefaultAsync(s => s.Name == name);
            if (street == null)
            {
                street = new Street { Name = name };
                await dbContext.Streets.AddAsync(street);

                await dbContext.CitiesStreets.AddAsync(new CityStreet
                {
                    CityId   = cityId,
                    StreetId = street.Id,
                });
            }
            else
            {
                bool linkExists = await dbContext.CitiesStreets
                    .AnyAsync(cs => cs.CityId == cityId && cs.StreetId == street.Id);

                if (!linkExists)
                {
                    await dbContext.CitiesStreets.AddAsync(new CityStreet
                    {
                        CityId   = cityId,
                        StreetId = street.Id,
                    });
                }
            }

            return street;
        }
    }
}
