using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;
using GoceTransportApp.Data.Models.Enumerations;

public class VehicleServiceTests
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
    public async Task CreateAsync_Should_Add_New_Vehicle()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicleInput = new VehicleInputModel
        {
            Number = "V123",
            Type = "Bus",
            Manufacturer = "Mercedes",
            Model = "Sprinter",
            Capacity = 30,
            FuelConsumption = 12.5,
            Status = VehicleStatus.Available.ToString(),
            OrganizationId = Guid.NewGuid().ToString(),
        };

        // Act
        await vehicleService.CreateAsync(vehicleInput);

        // Assert
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync();
        Assert.NotNull(vehicle);
        Assert.Equal("V123", vehicle.Number);
        Assert.Equal("Bus", vehicle.Type);
    }

    [Fact]
    public async Task GetVehicleForEditAsync_Should_Return_Correct_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V100",
            Type = "Truck",
            Manufacturer = "Volvo",
            Model = "FH16",
            Capacity = 40,
            FuelConsumption = 15.0,
            VehicleStatus = VehicleStatus.Available,
            OrganizationId = Guid.NewGuid(),
        };

        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await vehicleService.GetVehicleForEditAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("V100", result.Number);
        Assert.Equal("Truck", result.Type);
        Assert.Equal("Volvo", result.Manufacturer);
    }

    [Fact]
    public async Task RemoveVehicleAsync_Should_Delete_Vehicle()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V555",
            Type = "Bus",
            Manufacturer = "Scania",
            Model = "Touring HD",
            Capacity = 50,
            FuelConsumption = 20.0,
            VehicleStatus = VehicleStatus.Available,
            OrganizationId = Guid.NewGuid(),
        };

        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        var removeModel = new RemoveVehicleViewModel
        {
            Id = vehicle.Id.ToString(),
        };

        // Act
        var result = await vehicleService.RemoveVehicleAsync(removeModel);

        // Assert
        Assert.True(result);
        var deletedVehicle = await dbContext.Vehicles.FindAsync(vehicle.Id);
        Assert.True(deletedVehicle.IsDeleted);
    }

    [Fact]
    public async Task GetAllVehiclesAsync_Should_Filter_By_Search_Query()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicles = new List<Vehicle>
        {
            new Vehicle { Id = Guid.NewGuid(), Number = "A1", Manufacturer = "Mercedes", Model = "Actros", Type = "Truck", OrganizationId = Guid.NewGuid() },
            new Vehicle { Id = Guid.NewGuid(), Number = "B2", Manufacturer = "Scania", Model = "Touring", Type = "Bus", OrganizationId = Guid.NewGuid() },
            new Vehicle { Id = Guid.NewGuid(), Number = "C3", Manufacturer = "Volvo", Model = "FH16", Type = "Truck", OrganizationId = Guid.NewGuid() },
        };

        await dbContext.Vehicles.AddRangeAsync(vehicles);
        await dbContext.SaveChangesAsync();

        var searchFilter = new AllVehiclesSearchFilterViewModel
        {
            SearchQuery = "Scania",
        };

        // Act
        var result = await vehicleService.GetAllVehiclesAsync(searchFilter);

        // Assert
        Assert.Single(result);
        Assert.Equal("Scania", result.First().Manufacturer);
    }

    [Fact]
    public async Task GetVehiclesCountByFilterAsync_Should_Return_Correct_Count()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        await dbContext.Vehicles.AddRangeAsync(new List<Vehicle>
        {
            new Vehicle { Id = Guid.NewGuid(), Number = "E52CH2", Manufacturer = "Ford", Model = "Transit", Type = "Van", OrganizationId = Guid.NewGuid() },
            new Vehicle { Id = Guid.NewGuid(), Number = "E50CH2", Manufacturer = "Ford", Model = "F150", Type = "Truck", OrganizationId = Guid.NewGuid() },
            new Vehicle { Id = Guid.NewGuid(), Number = "E51CH2", Manufacturer = "Mercedes", Model = "Sprinter", Type = "Van", OrganizationId = Guid.NewGuid() },
        });

        await dbContext.SaveChangesAsync();

        var filterModel = new AllVehiclesSearchFilterViewModel
        {
            SearchQuery = "Ford",
        };

        // Act
        var result = await vehicleService.GetVehiclesCountByFilterAsync(filterModel);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetVehicleForEditAsync_Should_Return_Null_For_NonExistent_Vehicle()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var nonExistentVehicleId = Guid.NewGuid(); 

        // Act
        var result = await vehicleService.GetVehicleForEditAsync(nonExistentVehicleId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EditVehicleAsync_Should_Update_Vehicle_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V200",
            Type = "Truck",
            Manufacturer = "DAF",
            Model = "XF",
            Capacity = 45,
            FuelConsumption = 18.0,
            VehicleStatus = VehicleStatus.Available,
            OrganizationId = Guid.NewGuid(),
        };

        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        var editInput = new EditVehicleInputModel
        {
            Id = vehicle.Id.ToString(),
            Number = "V200 Updated",
            Type = "Truck",
            Manufacturer = "DAF",
            Model = "XF",
            Capacity = 45,
            FuelConsumption = 17.0,
            Status = VehicleStatus.UnderMaintenance,
            OrganizationId = vehicle.OrganizationId.ToString(),
        };

        // Act
        var result = await vehicleService.EditVehicleAsync(editInput);

        // Assert
        Assert.True(result);
        var updatedVehicle = await dbContext.Vehicles.FindAsync(vehicle.Id);
        Assert.Equal("V200 Updated", updatedVehicle.Number);
        Assert.Equal(17.0, updatedVehicle.FuelConsumption);
        Assert.Equal(VehicleStatus.UnderMaintenance, updatedVehicle.VehicleStatus);
    }

    [Fact]
    public async Task GetAllVehiclesAsync_Should_Return_Empty_List_When_No_Vehicles()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var searchFilter = new AllVehiclesSearchFilterViewModel
        {
            SearchQuery = "NonexistentManufacturer",
        };

        // Act
        var result = await vehicleService.GetAllVehiclesAsync(searchFilter);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetVehiclesForOrganizationAsync_Should_Return_Empty_List_When_No_Vehicles_In_Organization()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var nonExistentOrganizationId = Guid.NewGuid().ToString();

        // Act
        var result = await vehicleService.GetVehiclesForOrganizationAsync(nonExistentOrganizationId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task RemoveVehicleAsync_Should_Return_False_For_NonExistent_Vehicle()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var nonExistentVehicleId = Guid.NewGuid().ToString(); // Non-existent vehicle

        var removeModel = new RemoveVehicleViewModel
        {
            Id = nonExistentVehicleId,
        };

        // Act
        var result = await vehicleService.RemoveVehicleAsync(removeModel);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetVehicleDetailsAsync_Should_Return_Correct_Vehicle_Details_For_Valid_Vehicle_Id()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);
        var founderId = Guid.NewGuid().ToString();

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "TransportCo",
            FounderId = founderId,
        };

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V500",
            Type = "Truck",
            Manufacturer = "Scania",
            Model = "R580",
            Capacity = 40,
            FuelConsumption = 20.0,
            VehicleStatus = VehicleStatus.Busy,
            OrganizationId = organization.Id,
        };

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await vehicleService.GetVehicleDetailsAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("V500", result.Number);
        Assert.Equal("Truck", result.Type);
        Assert.Equal("Scania", result.Manufacturer);
        Assert.Equal("R580", result.Model);
        Assert.Equal(40, result.Capacity);
        Assert.Equal(20.0, result.FuelConsumption);
        Assert.Equal(VehicleStatus.Busy.ToString(), result.Status);
        Assert.Equal(organization.Name, result.OrganizationName);
        Assert.Equal(vehicle.OrganizationId.ToString(), result.OrganizationId);
    }

    [Fact]
    public async Task GetVehicleDetailsAsync_Should_Return_Null_For_NonExistent_Vehicle_Id()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var nonExistentVehicleId = Guid.NewGuid(); // Non-existent vehicle ID

        // Act
        var result = await vehicleService.GetVehicleDetailsAsync(nonExistentVehicleId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetVehicleForDeletionAsync_Should_Return_Correct_Vehicle_Data_For_Valid_Vehicle_Id()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V600",
            Type = "Truck",
            Manufacturer = "Volvo",
            Model = "FMX",
            Capacity = 30,
            FuelConsumption = 14.0,
            VehicleStatus = VehicleStatus.Available,
            OrganizationId = Guid.NewGuid(),
        };

        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await vehicleService.GetVehicleForDeletionAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("V600", result.Number);
        Assert.Equal(vehicle.OrganizationId.ToString(), result.OrganizationId);
    }

    [Fact]
    public async Task GetVehicleForDeletionAsync_Should_Return_Null_For_NonExistent_Vehicle_Id()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var vehicleRepository = new EfDeletableEntityRepository<Vehicle>(dbContext);
        var vehicleService = new VehicleService(vehicleRepository);

        var nonExistentVehicleId = Guid.NewGuid(); // Non-existent vehicle ID

        // Act
        var result = await vehicleService.GetVehicleForDeletionAsync(nonExistentVehicleId);

        // Assert
        Assert.Null(result);
    }

}
