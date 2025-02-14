using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using GoceTransportApp.Data.Repositories;

public class StreetServiceTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var databaseContext = new ApplicationDbContext(options);
        await databaseContext.Database.EnsureCreatedAsync();
        return databaseContext;
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Street()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var repository = new EfDeletableEntityRepository<Street>(dbContext);
        var streetService = new StreetService(repository);

        var inputModel = new StreetInputModel { Street = "Test Street" };

        // Act
        await streetService.CreateAsync(inputModel);
        var street = await dbContext.Streets.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(street);
        Assert.Equal("Test Street", street.Name);
    }

    [Fact]
    public async Task DeleteStreetAsync_Should_Return_True_When_Street_Exists()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var repository = new EfDeletableEntityRepository<Street>(dbContext);
        var streetService = new StreetService(repository);

        var street = new Street { Id = Guid.NewGuid(), Name = "Test Street" };
        await dbContext.Streets.AddAsync(street);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await streetService.DeleteStreetAsync(street.Id);
        var deletedStreet = await dbContext.Streets.FindAsync(street.Id);


        // Assert
        Assert.True(result);
        Assert.True(deletedStreet.IsDeleted);
    }

    [Fact]
    public async Task DeleteStreetAsync_Should_Return_False_When_Street_Does_Not_Exist()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var repository = new EfDeletableEntityRepository<Street>(dbContext);
        var streetService = new StreetService(repository);

        // Act
        var result = await streetService.DeleteStreetAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EditStreetAsync_Should_Return_True_When_Street_Is_Updated()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var repository = new EfDeletableEntityRepository<Street>(dbContext);
        var streetService = new StreetService(repository);

        var street = new Street { Id = Guid.NewGuid(), Name = "Old Street" };
        await dbContext.Streets.AddAsync(street);
        await dbContext.SaveChangesAsync();

        var editModel = new EditStreetInputModel { Id = street.Id.ToString(), Street = "New Street" };

        // Act
        var result = await streetService.EditStreetAsync(editModel);
        var updatedStreet = await dbContext.Streets.FindAsync(street.Id);

        // Assert
        Assert.True(result);
        Assert.Equal("New Street", updatedStreet.Name);
    }
}
