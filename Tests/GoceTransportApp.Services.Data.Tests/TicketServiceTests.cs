using GoceTransportApp.Data;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Web.ViewModels.Tickets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace GoceTransportApp.Services.Data.Tests
{
    public class TicketServiceTests
    {
        private readonly ApplicationDbContext databaseContext;
        private readonly TicketService ticketService;

        public TicketServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();

            var ticketRepository = new EfDeletableEntityRepository<Ticket>(databaseContext);
            var userTicketRepository = new EfDeletableEntityRepository<UserTicket>(databaseContext);

            ticketService = new TicketService(ticketRepository, userTicketRepository);
        }

        public void Dispose()
        {
            databaseContext.Database.EnsureDeleted();
            databaseContext.Dispose();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTicketToDatabase()
        {
            // Arrange
            var inputModel = new TicketInputModel
            {
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 50,
                OrganizationId = Guid.NewGuid().ToString(),
                RouteId = Guid.NewGuid().ToString(),
                ScheduleId = Guid.NewGuid().ToString()
            };

            // Act
            await ticketService.CreateAsync(inputModel);

            // Assert
            var ticketCount = await databaseContext.Tickets.CountAsync();
            Assert.Equal(1, ticketCount);
        }

        [Fact]
        public async Task EditTicketAsync_ShouldUpdateExistingTicket()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 100,
                RouteId = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow
            };

            databaseContext.Tickets.Add(ticket);
            await databaseContext.SaveChangesAsync();

            var editModel = new EditTicketInputModel
            {
                Id = ticket.Id.ToString(),
                IssuedDate = DateTime.UtcNow.AddDays(2),
                ExpiryDate = DateTime.UtcNow.AddDays(3),
                Price = 150,
                RouteId = ticket.RouteId.ToString(),
                ScheduleId = ticket.ScheduleId.ToString(),
                OrganizationId = ticket.OrganizationId.ToString(),
                TicketsUsers = new HashSet<UserTicket>(),
            };

            // Act
            var result = await ticketService.EditTicketAsync(editModel);

            // Assert
            Assert.True(result);

            var updatedTicket = await databaseContext.Tickets.FindAsync(ticket.Id);
            Assert.Equal(150, updatedTicket.Price);
        }

        [Fact]
        public async Task GetTicketForEditAsync_ShouldReturnCorrectTicket()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 75,
                RouteId = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
            };

            databaseContext.Tickets.Add(ticket);
            await databaseContext.SaveChangesAsync();

            // Act
            var result = await ticketService.GetTicketForEditAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id.ToString(), result.Id);
            Assert.Equal(ticket.Price, result.Price);
        }

        [Fact]
        public async Task RemoveTicketAsync_ShouldDeleteTicket()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 50
            };

            databaseContext.Tickets.Add(ticket);
            await databaseContext.SaveChangesAsync();

            var inputModel = new RemoveTicketViewModel { Id = ticket.Id.ToString() };

            // Act
            var result = await ticketService.RemoveTicketAsync(inputModel);

            // Assert
            Assert.True(result);
            var ticketExists = await databaseContext.Tickets.AnyAsync(t => t.Id == ticket.Id);
            Assert.False(ticketExists);
        }

        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnFilteredTickets()
        {
            // Arrange
            var streetA = new Street { Id = Guid.NewGuid(), Name = "CityA"};
            var streetB = new Street { Id = Guid.NewGuid(), Name = "CityB"};
            databaseContext.Streets.AddRange(streetA, streetB);
            await databaseContext.SaveChangesAsync();

            var cityA = new City { Id = Guid.NewGuid(), Name = "CityA", State = "CityA", ZipCode = "14151" };
            var cityB = new City { Id = Guid.NewGuid(), Name = "CityB", State = "CityB", ZipCode = "51413" };
            databaseContext.Cities.AddRange(cityA, cityB);
            await databaseContext.SaveChangesAsync();

            var founder = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Email = "user@example.com"
            };

            databaseContext.Users.Add(founder);
            await databaseContext.SaveChangesAsync();

            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Test Org",
                FounderId = founder.Id
            };
            databaseContext.Organizations.Add(organization);
            await databaseContext.SaveChangesAsync();

            var route = new Route { Id = Guid.NewGuid(), FromCity = cityA, ToCity = cityB, FromStreet = streetA, ToStreet = streetB, Distance = 120, Duration = 100, OrganizationId = organization.Id };
            databaseContext.Routes.Add(route);
            await databaseContext.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 100,
                Route = route,
                ScheduleId = Guid.NewGuid(),
                OrganizationId = organization.Id
            };

            databaseContext.Tickets.Add(ticket);
            await databaseContext.SaveChangesAsync();

            var filterModel = new AllTicketsSearchFilterViewModel
            {
                SearchQuery = "CityA",
                EntitiesPerPage = 10,
                CurrentPage = 1
            };

            // Act
            var result = await ticketService.GetAllTicketsAsync(filterModel);

            // Assert
            Assert.Single(result);
            Assert.Equal("CityA", result.First().FromCity);
        }

        [Fact]
        public async Task EditTicketAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            // Arrange
            var editModel = new EditTicketInputModel
            {
                Id = Guid.NewGuid().ToString(), // Random non-existent ID
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Price = 100,
                RouteId = Guid.NewGuid().ToString(),
                ScheduleId = Guid.NewGuid().ToString(),
                OrganizationId = Guid.NewGuid().ToString(),
                TicketsUsers = new HashSet<UserTicket>()
            };

            // Act
            var result = await ticketService.EditTicketAsync(editModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveTicketAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            // Arrange
            var inputModel = new RemoveTicketViewModel { Id = Guid.NewGuid().ToString() };

            // Act
            var result = await ticketService.RemoveTicketAsync(inputModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTicketForEditAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            // Act
            var result = await ticketService.GetTicketForEditAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnEmptyList_WhenNoTicketsExist()
        {
            // Act
            var result = await ticketService.GetAllTicketsAsync(new AllTicketsSearchFilterViewModel());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task RemoveTicketAsync_ShouldReturnFalse_WhenTicketIsAlreadyDeleted()
        {
            // Arrange
            var ticket = await CreateTicketAsync();
            databaseContext.Tickets.Remove(ticket);
            await databaseContext.SaveChangesAsync();

            var inputModel = new RemoveTicketViewModel { Id = ticket.Id.ToString() };

            // Act
            var result = await ticketService.RemoveTicketAsync(inputModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTicketForEditAsync_ShouldReturnCorrectTicket_WhenTicketExists()
        {
            // Arrange
            var ticket = await CreateTicketAsync();

            // Act
            var result = await ticketService.GetTicketForEditAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id.ToString(), result.Id);
            Assert.Equal(ticket.Price, result.Price);
        }

        [Fact]
        public async Task GetTicketForDeletionAsync_ShouldReturnCorrectTicket_WhenTicketExists()
        {
            // Arrange
            var ticket = await CreateTicketAsync();

            // Act
            var result = await ticketService.GetTicketForDeletionAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id.ToString(), result.Id);
        }

        [Fact]
        public async Task GetTicketForDeletionAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            // Act
            var result = await ticketService.GetTicketForDeletionAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTicketDetailsAsync_ShouldReturnCorrectDetails_WhenTicketExists()
        {
            // Arrange
            var ticket = await CreateTicketAsync();

            // Act
            var result = await ticketService.GetTicketDetailsAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id.ToString(), result.Id);
            Assert.Equal(ticket.Price.ToString(), result.Price);
            Assert.Equal(ticket.Route.FromCity.Name, result.FromCity);
            Assert.Equal(ticket.Route.ToCity.Name, result.ToCity);
            Assert.Equal(ticket.Organization.Name, result.OrganizationName);
        }

        [Fact]
        public async Task GetTicketDetailsAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            // Act
            var result = await ticketService.GetTicketDetailsAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        private async Task<Ticket> CreateTicketAsync(decimal price = 50, DateTime? expiryDate = null)
        {
            var fromCity = new City { Id = Guid.NewGuid(), Name = "CityA", State = "CityA", ZipCode = "14141" };
            var toCity = new City { Id = Guid.NewGuid(), Name = "CityB", State = "CityB", ZipCode = "15616" };
            var fromStreet = new Street { Id = Guid.NewGuid(), Name = "Main St" };
            var toStreet = new Street { Id = Guid.NewGuid(), Name = "Second St" };
            var founder = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Email = "testUser@gmail.com",
            };

            var organization = new Organization { Id = Guid.NewGuid(), Name = "Test Organization", FounderId = founder.Id };


            var route = new Route
            {
                Id = Guid.NewGuid(),
                FromCity = fromCity,
                ToCity = toCity,
                FromStreet = fromStreet,
                ToStreet = toStreet,
                OrganizationId = organization.Id,
            };

            var timeTable = new Schedule
            {
                Id = Guid.NewGuid(),
                Day = DayOfWeek.Monday,
                Arrival = DateTime.UtcNow.AddHours(2),
                Departure = DateTime.UtcNow.AddHours(1),
                Route = route,
                OrganizationId = organization.Id
            };

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = expiryDate ?? DateTime.UtcNow.AddDays(1),
                Price = price,
                Route = route,
                TimeTable = timeTable,
                Organization = organization
            };

            // Add and save all entities to ensure they exist in the DB
            databaseContext.Cities.AddRange(fromCity, toCity);
            databaseContext.Streets.AddRange(fromStreet, toStreet);
            databaseContext.Routes.Add(route);
            databaseContext.Schedules.Add(timeTable);
            databaseContext.Organizations.Add(organization);
            databaseContext.Tickets.Add(ticket);

            await databaseContext.SaveChangesAsync();
            return ticket;
        }

    }
}
