using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Data;
using GoceTransportApp.Services.Data.Organizations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoceTransportApp.Web.ViewModels.Organizations;
using Xunit;

namespace GoceTransportApp.Services.Data.Tests
{
    public class OrganizationServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizationService organizationService;

        public OrganizationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            dbContext = new ApplicationDbContext(options);

            var organizationRepository = new EfDeletableEntityRepository<Organization>(dbContext);
            var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
            var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
            var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
            var ticketRepository = new EfDeletableEntityRepository<Ticket>(dbContext);
            var scheduleRepository = new EfDeletableEntityRepository<Schedule>(dbContext);

            organizationService = new OrganizationService(
                organizationRepository, routeRepository, driverRepository,
                vehicleRepository, ticketRepository, scheduleRepository);
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddOrganization()
        {
            // Arrange
            var inputModel = new OrganizationInputModel
            {
                Name = "Test Organization",
                Address = "123 Test Street",
                Phone = "123-456-7890",
                FounderId = "founder-1"
            };

            // Act
            await organizationService.CreateAsync(inputModel);
            var organization = await dbContext.Organizations.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(organization);
            Assert.Equal(inputModel.Name, organization.Name);
            Assert.Equal(inputModel.Address, organization.Address);
        }

        [Fact]
        public async Task EditOrganizationAsync_ShouldUpdateOrganization_WhenExists()
        {
            // Arrange
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Address = "Old Address",
                Phone = "111-111-1111",
                FounderId = "founder-1"
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            var inputModel = new EditOrganizationInputModel
            {
                Id = organization.Id.ToString(),
                Name = "New Name",
                Address = "New Address",
                Phone = "999-999-9999",
                FounderId = "founder-1"
            };

            // Act
            var result = await organizationService.EditOrganizationAsync(inputModel);
            var updatedOrganization = await dbContext.Organizations.FindAsync(organization.Id);

            // Assert
            Assert.True(result);
            Assert.Equal("New Name", updatedOrganization.Name);
            Assert.Equal("New Address", updatedOrganization.Address);
            Assert.Equal("999-999-9999", updatedOrganization.Phone);
        }

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnFilteredOrganizations()
        {
            // Arrange
            var organizations = new List<Organization>
    {
        new Organization { Id = Guid.NewGuid(), Name = "Alpha", Founder = new ApplicationUser { UserName = "User1" } },
        new Organization { Id = Guid.NewGuid(), Name = "Beta", Founder = new ApplicationUser { UserName = "User2" } },
        new Organization { Id = Guid.NewGuid(), Name = "Gamma", Founder = new ApplicationUser { UserName = "User3" } },
    };

            dbContext.Organizations.AddRange(organizations);
            await dbContext.SaveChangesAsync();

            var inputModel = new AllOrganizationsSearchFilterViewModel
            {
                SearchQuery = "Alpha",
                EntitiesPerPage = 2,
                CurrentPage = 1
            };

            // Act
            var result = await organizationService.GetAllOrganizationsAsync(inputModel);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, o => o.Name == "Alpha");
        }

        [Fact]
        public async Task GetOrganizationDetailsAsync_ShouldReturnCorrectDetails_WhenOrganizationExists()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Address = "Test Address",
                Phone = "1234567890",
                FounderId = "founder-1",
                Founder = new ApplicationUser { UserName = "FounderUser" }
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await organizationService.GetOrganizationDetailsAsync(organizationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(organizationId.ToString(), result.Id);
            Assert.Equal("Test Org", result.Name);
            Assert.Equal("FounderUser", result.Founder);
        }

        [Fact]
        public async Task RemoveOrganizationAsync_ShouldDeleteOrganization_WhenExists()
        {
            // Arrange
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Test Organization",
                FounderId = "founder-1",
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            var inputModel = new RemoveOrganizationViewModel
            {
                Id = organization.Id.ToString()
            };

            // Act
            var result = await organizationService.RemoveOrganizationAsync(inputModel);
            var deletedOrganization = await dbContext.Organizations.FindAsync(organization.Id);

            // Assert
            Assert.True(result);
            Assert.True(deletedOrganization.IsDeleted);
        }

        [Fact]
        public async Task EditOrganizationAsync_ShouldReturnFalse_WhenOrganizationDoesNotExist()
        {
            // Arrange
            var inputModel = new EditOrganizationInputModel
            {
                Id = Guid.NewGuid().ToString(), // Non-existent ID
                Name = "Updated Name",
                Address = "Updated Address",
                Phone = "999-999-9999",
                FounderId = "founder-1"
            };

            // Act
            var result = await organizationService.EditOrganizationAsync(inputModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetOrganizationDetailsAsync_ShouldReturnNull_WhenOrganizationDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await organizationService.GetOrganizationDetailsAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveOrganizationAsync_ShouldReturnFalse_WhenOrganizationDoesNotExist()
        {
            // Arrange
            var inputModel = new RemoveOrganizationViewModel
            {
                Id = Guid.NewGuid().ToString() // Non-existent ID
            };

            // Act
            var result = await organizationService.RemoveOrganizationAsync(inputModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnEmptyList_WhenNoOrganizationsExist()
        {
            // Arrange
            var inputModel = new AllOrganizationsSearchFilterViewModel
            {
                SearchQuery = null,
                EntitiesPerPage = 5,
                CurrentPage = 1
            };

            // Act
            var result = await organizationService.GetAllOrganizationsAsync(inputModel);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnOrganizations_WhenSearchingByFounderName()
        {
            // Arrange
            var founder = new ApplicationUser { Id = "founder-1", UserName = "Founder123" };
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Alpha Org",
                Address = "123 Main St",
                FounderId = founder.Id,
                Founder = founder
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            var inputModel = new AllOrganizationsSearchFilterViewModel
            {
                SearchQuery = "Founder123", // Search by founder name
                EntitiesPerPage = 10,
                CurrentPage = 1
            };

            // Act
            var result = await organizationService.GetAllOrganizationsAsync(inputModel);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, o => o.Founder == "Founder123");
        }

        [Fact]
        public async Task GetOrganizationForEditAsync_ShouldReturnOrganization_WhenOrganizationExists()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Organization",
                Address = "123 Main St",
                Phone = "123-456-7890",
                FounderId = "founder-1"
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await organizationService.GetOrganizationForEditAsync(organizationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(organizationId.ToString(), result.Id);
            Assert.Equal("Test Organization", result.Name);
            Assert.Equal("123 Main St", result.Address);
            Assert.Equal("123-456-7890", result.Phone);
        }

        [Fact]
        public async Task GetOrganizationForEditAsync_ShouldReturnNull_WhenOrganizationDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await organizationService.GetOrganizationForEditAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrganizationForDeletionAsync_ShouldReturnOrganization_WhenOrganizationExists()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var founder = new ApplicationUser { Id = "founder-1", UserName = "FounderUser" };
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Delete Me Org",
                Address = "456 Another St",
                FounderId = founder.Id,
                Founder = founder
            };

            dbContext.Organizations.Add(organization);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await organizationService.GetOrganizationForDeletionAsync(organizationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(organizationId.ToString(), result.Id);
            Assert.Equal("Delete Me Org", result.Name);
            Assert.Equal("456 Another St", result.Address);
            Assert.Equal("FounderUser", result.FounderName);
        }

        [Fact]
        public async Task GetOrganizationForDeletionAsync_ShouldReturnNull_WhenOrganizationDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await organizationService.GetOrganizationForDeletionAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

    }
}
