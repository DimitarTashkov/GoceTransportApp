using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Data.Seeding
{
    public class DataSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            // Prevent multiple seedings
            if (await dbContext.Cities.AnyAsync()) return;

            var random = new Random();

            // Seed Cities
            var cityNames = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix",
        "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose",
        "Austin", "Jacksonville", "Fort Worth", "Columbus", "San Francisco",
        "Charlotte", "Indianapolis", "Seattle", "Denver", "Washington D.C." };

            var cities = cityNames.Select(name => new City
            {
                Id = Guid.NewGuid(),
                Name = name,
                State = "USA",
                ZipCode = $"{10000 + random.Next(1, 9000)}"
            }).ToList();

            await dbContext.Cities.AddRangeAsync(cities);

            // Seed Streets
            var streetNames = new[] { "Main Street", "Broadway", "Fifth Avenue", "Elm Street", "Sunset Boulevard",
        "Park Avenue", "Maple Street", "Cedar Road", "Lakeview Drive", "Oak Lane",
        "Pine Street", "River Road", "Washington Boulevard", "Hilltop Avenue", "Ocean Drive",
        "Lexington Avenue", "Mulholland Drive", "Beacon Street", "Willow Lane", "Chestnut Street" };

            var streets = streetNames.Select(name => new Street { Id = Guid.NewGuid(), Name = name }).ToList();
            await dbContext.Streets.AddRangeAsync(streets);

            // Seed CityStreet (Many-to-Many Relationship)
            var cityStreetRelations = new List<CityStreet>();
            var streetIds = streets.Select(s => s.Id).ToList();
            foreach (var city in cities)
            {
                var selectedStreets = streetIds.OrderBy(_ => Guid.NewGuid()).Take(3); // Each city gets 3 random streets
                foreach (var streetId in selectedStreets)
                {
                    cityStreetRelations.Add(new CityStreet
                    {
                        CityId = city.Id,
                        StreetId = streetId
                    });
                }
            }
            await dbContext.CitiesStreets.AddRangeAsync(cityStreetRelations);

            // Seed Users & Organizations
            var users = new List<ApplicationUser>();
            var organizations = new List<Organization>();

            var userData = new (string FirstName, string LastName, string Email, string OrgName)[]
            {
        ("John", "Doe", "john.doe@email.com", "City Express"),
        ("Alice", "Smith", "alice.smith@email.com", "FastTrack Logistics"),
        ("Robert", "Johnson", "robert.johnson@email.com", "Metro Transit"),
        ("Sophia", "Martinez", "sophia.martinez@email.com", "Skyline Bus Service"),
        ("Michael", "Brown", "michael.brown@email.com", "RapidRide Transport")
            };

            foreach (var data in userData)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = data.Email,
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    City = "New York"
                };

                users.Add(user);

                organizations.Add(new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = data.OrgName,
                    FounderId = user.Id,
                    Address = "123 Business Road",
                    Phone = $"555-{random.Next(1000, 9999)}"
                });
            }

            await dbContext.Users.AddRangeAsync(users);
            await dbContext.Organizations.AddRangeAsync(organizations);

            // Seed Routes, Vehicles, Drivers, Schedules & Tickets
            var routes = new List<Route>();
            var vehicles = new List<Vehicle>();
            var drivers = new List<Driver>();
            var schedules = new List<Schedule>();
            var tickets = new List<Ticket>();

            var routePairs = new (string From, string To)[]
            {
        ("New York", "Los Angeles"), ("Chicago", "Houston"),
        ("Philadelphia", "San Antonio"), ("San Diego", "Dallas"), ("San Francisco", "Seattle")
            };

            var vehicleNames = new[] { "Blue Bird", "Silver Comet", "MetroRunner", "RapidGo", "SkyHigh" };
            var driverNames = new (string FirstName, string LastName)[]
            {
        ("James", "Anderson"), ("Emily", "Clark"), ("Daniel", "Miller"),
        ("Olivia", "Taylor"), ("William", "Harris")
            };

            var cityDict = cities.ToDictionary(c => c.Name, c => c.Id);

            foreach (var org in organizations)
            {
                for (int i = 0; i < 5; i++)
                {
                    var (fromCity, toCity) = routePairs[i % routePairs.Length];
                    var fromCityId = cityDict[fromCity];
                    var toCityId = cityDict[toCity];
                    var fromStreetId = streetIds[random.Next(streetIds.Count)];
                    var toStreetId = streetIds[random.Next(streetIds.Count)];

                    var route = new Route
                    {
                        Id = Guid.NewGuid(),
                        FromCityId = fromCityId,
                        ToCityId = toCityId,
                        FromStreetId = fromStreetId,
                        ToStreetId = toStreetId,
                        Distance = random.Next(50, 500),
                        Duration = random.Next(1, 8),
                        OrganizationId = org.Id
                    };
                    routes.Add(route);

                    var vehicleName = vehicleNames[i].Substring(0, Math.Min(6, vehicleNames[i].Length)); 

                    var vehicle = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Number = $"{vehicleName}-{random.Next(100, 999)}",
                        Type = "Bus",
                        Manufacturer = "Mercedes",
                        Model = "Sprinter",
                        Capacity = random.Next(20, 50),
                        FuelConsumption = random.Next(5, 15),
                        VehicleStatus = VehicleStatus.Available,
                        OrganizationId = org.Id
                    };
                    vehicles.Add(vehicle);

                    var (driverFirst, driverLast) = driverNames[i];
                    var driver = new Driver
                    {
                        Id = Guid.NewGuid(),
                        FirstName = driverFirst,
                        LastName = driverLast,
                        Age = random.Next(25, 60),
                        Experience = (DriverExperience)random.Next(0, 3),
                        OrganizationId = org.Id
                    };
                    drivers.Add(driver);

                    var schedule = new Schedule
                    {
                        Id = Guid.NewGuid(),
                        Day = (DayOfWeek)random.Next(0, 6),
                        Departure = DateTime.Now.AddHours(random.Next(1, 12)),
                        Arrival = DateTime.Now.AddHours(random.Next(12, 24)),
                        VehicleId = vehicle.Id,
                        RouteId = route.Id,
                        OrganizationId = org.Id
                    };
                    schedules.Add(schedule);

                    var ticket = new Ticket
                    {
                        Id = Guid.NewGuid(),
                        IssuedDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        Price = random.Next(10, 100),
                        RouteId = route.Id,
                        OrganizationId = org.Id,
                        ScheduleId = schedule.Id
                    };
                    tickets.Add(ticket);
                }
            }

            await dbContext.Routes.AddRangeAsync(routes);
            await dbContext.Vehicles.AddRangeAsync(vehicles);
            await dbContext.Drivers.AddRangeAsync(drivers);
            await dbContext.Schedules.AddRangeAsync(schedules);
            await dbContext.Tickets.AddRangeAsync(tickets);

            // Save all changes to the database
            await dbContext.SaveChangesAsync();
        }

    }
}
