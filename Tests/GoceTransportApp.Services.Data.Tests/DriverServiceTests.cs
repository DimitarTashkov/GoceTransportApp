using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Services.Data.Drivers;
using GoceTransportApp.Web.ViewModels.Drivers;
using GoceTransportApp.Data.Models.Enumerations;

public class DriverServiceTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        var databaseContext = new ApplicationDbContext(options);
        await databaseContext.Database.EnsureCreatedAsync(); // Ensure schema is created

        return databaseContext;
    }

    [Fact]
    public async Task CreateAsync_Should_Add_New_Driver()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var driverInput = new DriverInputModel
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            DrivingExperience = DriverExperience.Experienced.ToString(),
            OrganizationId = Guid.NewGuid().ToString(),
        };

        // Act
        await driverService.CreateAsync(driverInput);

        // Assert
        var driver = await dbContext.Drivers.FirstOrDefaultAsync();
        Assert.NotNull(driver);
        Assert.Equal("John", driver.FirstName);
        Assert.Equal("Doe", driver.LastName);
    }

    [Fact]
    public async Task EditDriverAsync_Should_Update_Driver_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            FirstName = "OldFirstName",
            LastName = "OldLastName",
            Age = 35,
            Experience = DriverExperience.Beginner,
            OrganizationId = Guid.NewGuid(),
        };
        await dbContext.Drivers.AddAsync(driver);
        await dbContext.SaveChangesAsync();

        var editInput = new EditDriverInputModel
        {
            Id = driver.Id.ToString(),
            FirstName = "NewFirstName",
            LastName = "NewLastName",
            Age = 36,
            DrivingExperience = DriverExperience.Skilled,
            OrganizationId = driver.OrganizationId.ToString(),
        };

        // Act
        var result = await driverService.EditDriverAsync(editInput);

        // Assert
        Assert.True(result);
        var updatedDriver = await dbContext.Drivers.FindAsync(driver.Id);
        Assert.Equal("NewFirstName", updatedDriver.FirstName);
        Assert.Equal("NewLastName", updatedDriver.LastName);
    }

    [Fact]
    public async Task RemoveDriverAsync_Should_Delete_Driver()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            FirstName = "DeleteFirstName",
            LastName = "DeleteLastName",
            Age = 40,
            Experience = DriverExperience.Experienced,
            OrganizationId = Guid.NewGuid(),
        };
        await dbContext.Drivers.AddAsync(driver);
        await dbContext.SaveChangesAsync();

        var removeModel = new RemoveDriverViewModel
        {
            Id = driver.Id.ToString(),
        };

        // Act
        var result = await driverService.RemoveDriverAsync(removeModel);

        // Assert
        Assert.True(result);
        var deletedDriver = await dbContext.Drivers.FindAsync(driver.Id);
        Assert.True(deletedDriver.IsDeleted); // Assuming the driver is soft-deleted
    }    

    [Fact]
    public async Task GetDriverDetailsAsync_Should_Return_Null_For_NonExistent_Driver()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var nonExistentDriverId = Guid.NewGuid(); // Non-existent driver ID

        // Act
        var result = await driverService.GetDriverDetailsAsync(nonExistentDriverId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EditDriverAsync_Should_Return_False_If_Driver_Not_Exists()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var editInput = new EditDriverInputModel
        {
            Id = Guid.NewGuid().ToString(), // Non-existent driver ID
            FirstName = "UpdatedName",
            LastName = "UpdatedLast",
            Age = 35,
            DrivingExperience = DriverExperience.Skilled,
            OrganizationId = Guid.NewGuid().ToString(),
        };

        // Act
        var result = await driverService.EditDriverAsync(editInput);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveDriverAsync_Should_Return_False_If_Driver_Not_Exists()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var removeModel = new RemoveDriverViewModel
        {
            Id = Guid.NewGuid().ToString(), // Non-existent driver ID
        };

        // Act
        var result = await driverService.RemoveDriverAsync(removeModel);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetDriverDetailsAsync_Should_Not_Return_Soft_Deleted_Driver()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var driverRepository = new EfDeletableEntityRepository<Driver>(dbContext);
        var driverService = new DriverService(driverRepository);

        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            FirstName = "DeletedDriver",
            LastName = "Test",
            Age = 45,
            Experience = DriverExperience.Experienced,
            OrganizationId = Guid.NewGuid(),
            IsDeleted = true,
        };
        await dbContext.Drivers.AddAsync(driver);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await driverService.GetDriverDetailsAsync(driver.Id);

        // Assert
        Assert.Null(result);
    }


}
