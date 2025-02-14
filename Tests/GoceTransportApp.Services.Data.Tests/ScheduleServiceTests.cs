using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Data;
using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Web.ViewModels.Schedules;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Xunit;

public class ScheduleServiceTests
{
    private readonly ScheduleService scheduleService;
    private readonly ApplicationDbContext dbContext;
    private readonly IDeletableEntityRepository<Schedule> scheduleRepository;

    public ScheduleServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.dbContext = new ApplicationDbContext(options);
        this.scheduleRepository = new EfDeletableEntityRepository<Schedule>(dbContext);
        this.scheduleService = new ScheduleService(scheduleRepository);
    }

    private async Task<(ApplicationUser founder, City fromCity, City toCity, Street street1, Street street2, Organization organization, Route route, Vehicle vehicle)> SetupCommonEntities()
    {
        var founder = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "user@example.com"
        };
        dbContext.Users.Add(founder);
        await dbContext.SaveChangesAsync();

        var fromCity = new City { Id = Guid.NewGuid(), Name = "CityM", State = "StateM", ZipCode = "13414" };
        var toCity = new City { Id = Guid.NewGuid(), Name = "CityN", State = "StateN", ZipCode = "11414" };
        dbContext.Cities.AddRange(fromCity, toCity);
        await dbContext.SaveChangesAsync();

        var street1 = new Street { Id = Guid.NewGuid(), Name = "Street A" };
        var street2 = new Street { Id = Guid.NewGuid(), Name = "Street B" };
        dbContext.Streets.AddRange(street1, street2);
        await dbContext.SaveChangesAsync();

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Test Org",
            FounderId = founder.Id
        };
        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesAsync();

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Number = "V123",
            Manufacturer = "BMW",
            Model = "M5",
            Type = "Combi",
            OrganizationId = organization.Id
        };
        dbContext.Vehicles.Add(vehicle);
        await dbContext.SaveChangesAsync();

        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 100,
            Duration = 2.4,
            FromCity = fromCity,
            ToCity = toCity,
            FromStreet = street1,
            ToStreet = street2,
            OrganizationId = organization.Id
        };
        dbContext.Routes.Add(route);
        await dbContext.SaveChangesAsync();

        return (founder, fromCity, toCity, street1, street2, organization, route, vehicle);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateScheduleSuccessfully()
    {
        // Arrange
        var inputModel = new ScheduleInputModel
        {
            Day = "Monday",
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            OrganizationId = Guid.NewGuid().ToString(),
            RouteId = Guid.NewGuid().ToString(),
            VehicleId = Guid.NewGuid().ToString()
        };

        // Act
        await scheduleService.CreateAsync(inputModel);
        var schedule = await dbContext.Schedules.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(DayOfWeek.Monday, schedule.Day);
        Assert.Equal(inputModel.Departure, schedule.Departure);
        Assert.Equal(inputModel.Arrival, schedule.Arrival);
    }

    [Fact]
    public async Task EditScheduleAsync_ShouldUpdateScheduleSuccessfully()
    {
        // Arrange
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Monday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            OrganizationId = Guid.NewGuid(),
            RouteId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        var inputModel = new EditScheduleInputModel
        {
            Id = schedule.Id.ToString(),
            Day = "Tuesday",
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(3),
            OrganizationId = schedule.OrganizationId.ToString(),
            RouteId = schedule.RouteId.ToString(),
            VehicleId = schedule.VehicleId.ToString()
        };

        // Act
        var result = await scheduleService.EditScheduleAsync(inputModel);
        var updatedSchedule = await dbContext.Schedules.FindAsync(schedule.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedSchedule);
        Assert.Equal(DayOfWeek.Tuesday, updatedSchedule.Day);
        Assert.Equal(inputModel.Arrival, updatedSchedule.Arrival);
    }

    [Fact]
    public async Task GetAllSchedulesAsync_ShouldReturnSchedules()
    {
        // Arrange
        var (founder, fromCity, toCity, street1, street2, organization, route, _) = await SetupCommonEntities();

        dbContext.Schedules.Add(new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Wednesday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            Route = route
        });

        await dbContext.SaveChangesAsync();

        var filter = new AllSchedulesSearchFilterViewModel();

        // Act
        var schedules = await scheduleService.GetAllSchedulesAsync(filter);

        // Assert
        Assert.NotEmpty(schedules);
        Assert.Contains(schedules, s => s.FromCity == "CityM" && s.ToCity == "CityN");
    }

    [Fact]
    public async Task GetScheduleForDeletionAsync_ShouldReturnCorrectSchedule()
    {
        // Arrange
        var (founder, fromCity, toCity, street1, street2, organization, route, _) = await SetupCommonEntities();

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Thursday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            Route = route,
            OrganizationId = organization.Id
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await scheduleService.GetScheduleForDeletionAsync(schedule.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Thursday", result.Day);
        Assert.Equal("CityM", result.FromCity);
        Assert.Equal("CityN", result.ToCity);
    }

    [Fact]
    public async Task RemoveScheduleAsync_ShouldDeleteSchedule()
    {
        // Arrange
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Friday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            OrganizationId = Guid.NewGuid(),
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        var inputModel = new RemoveScheduleViewModel { Id = schedule.Id.ToString() };

        // Act
        var result = await scheduleService.RemoveScheduleAsync(inputModel);
        var deletedSchedule = await dbContext.Schedules.FindAsync(schedule.Id);

        // Assert
        Assert.True(result);
        Assert.True(deletedSchedule.IsDeleted);
    }

    [Fact]
    public async Task GetScheduleDetailsAsync_ShouldReturnCorrectDetails()
    {
        // Arrange
        var (founder, fromCity, toCity, street1, street2, organization, route, vehicle) = await SetupCommonEntities();

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Saturday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            Vehicle = vehicle,
            Organization = organization,
            Route = route
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await scheduleService.GetScheduleDetailsAsync(schedule.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Saturday", result.Day);
        Assert.Equal("CityM", result.FromCity);
        Assert.Equal("CityN", result.ToCity);
        Assert.Equal("Test Org", result.OrganizationName);
        Assert.Equal("V123", result.VehicleNumber);
    }

    [Fact]
    public async Task GetSchedulesCountByFilterAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        dbContext.Schedules.Add(new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Sunday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
        });

        await dbContext.SaveChangesAsync();

        var filter = new AllSchedulesSearchFilterViewModel { DayFilter = DayOfWeek.Sunday };

        // Act
        var count = await scheduleService.GetSchedulesCountByFilterAsync(filter);

        // Assert
        Assert.Equal(1, count);
    }
    [Fact]
    public async Task EditScheduleAsync_ShouldNotUpdateIfScheduleDoesNotExist()
    {
        // Arrange
        var inputModel = new EditScheduleInputModel
        {
            Id = Guid.NewGuid().ToString(),  // Non-existent schedule ID
            Day = "Tuesday",
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(3),
            OrganizationId = Guid.NewGuid().ToString(),
            RouteId = Guid.NewGuid().ToString(),
            VehicleId = Guid.NewGuid().ToString()
        };

        // Act
        var result = await scheduleService.EditScheduleAsync(inputModel);

        // Assert
        Assert.False(result);  // It should return false since the schedule doesn't exist
    }

    [Fact]
    public async Task RemoveScheduleAsync_ShouldReturnFalseForNonExistentSchedule()
    {
        // Arrange
        var inputModel = new RemoveScheduleViewModel { Id = Guid.NewGuid().ToString() };  // Non-existent schedule ID

        // Act
        var result = await scheduleService.RemoveScheduleAsync(inputModel);

        // Assert
        Assert.False(result);  // It should return false if the schedule doesn't exist
    }

    [Fact]
    public async Task GetScheduleDetailsAsync_ShouldReturnNullForNonExistentSchedule()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();  // Non-existent schedule ID

        // Act
        var result = await scheduleService.GetScheduleDetailsAsync(scheduleId);

        // Assert
        Assert.Null(result);  // It should return null since the schedule doesn't exist
    }
    [Fact]
    public async Task GetScheduleForEdit_ShouldReturnCorrectScheduleForValidId()
    {
        // Arrange
        var (founder, fromCity, toCity, street1, street2, organization, route, vehicle) = await SetupCommonEntities();

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Monday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            VehicleId = vehicle.Id,
            Route = route
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await scheduleService.GetScheduleForEditAsync(schedule.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(schedule.Id.ToString(), result.Id);
        Assert.Equal("Monday", result.Day);
        Assert.Equal(schedule.Departure, result.Departure);
        Assert.Equal(schedule.Arrival, result.Arrival);
    }

    [Fact]
    public async Task GetScheduleForEdit_ShouldReturnNullForNonExistentSchedule()
    {
        // Arrange
        var nonExistentScheduleId = Guid.NewGuid();

        // Act
        var result = await scheduleService.GetScheduleForEditAsync(nonExistentScheduleId);

        // Assert
        Assert.Null(result);  // It should return null if the schedule doesn't exist
    }

    [Fact]
    public async Task GetSchedulesFromOrganizations_ShouldReturnSchedulesForValidOrganization()
    {
        // Arrange
        var (founder, fromCity, toCity, street1, street2, organization, route, vehicle) = await SetupCommonEntities();

        var schedule1 = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Monday,
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            VehicleId = vehicle.Id,
            Route = route,
            OrganizationId = organization.Id
        };
        var schedule2 = new Schedule
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Tuesday,
            Departure = DateTime.UtcNow.AddDays(1),
            Arrival = DateTime.UtcNow.AddDays(1).AddHours(2),
            VehicleId = vehicle.Id,
            Route = route,
            OrganizationId = organization.Id
        };

        dbContext.Schedules.AddRange(schedule1, schedule2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await scheduleService.GetSchedulesForOrganizationAsync(organization.Id.ToString());

        // Assert
        Assert.NotEmpty(result); 

    }

    [Fact]
    public async Task GetSchedulesFromOrganizations_ShouldReturnEmptyForOrganizationWithNoSchedules()
    {
        // Arrange
        var organizationWithoutSchedules = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Empty Organization",
            FounderId = Guid.NewGuid().ToString()
        };
        dbContext.Organizations.Add(organizationWithoutSchedules);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await scheduleService.GetSchedulesForOrganizationAsync(organizationWithoutSchedules.Id.ToString());

        // Assert
        Assert.Empty(result);  // It should return an empty list if the organization has no schedules
    }

    [Fact]
    public async Task GetSchedulesFromOrganizations_ShouldReturnEmptyForNonExistentOrganization()
    {
        // Arrange
        var nonExistentOrganizationId = Guid.NewGuid().ToString();

        // Act
        var result = await scheduleService.GetSchedulesForOrganizationAsync(nonExistentOrganizationId);

        // Assert
        Assert.Empty(result);  // It should return an empty list if the organization doesn't exist
    }
}
