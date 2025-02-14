using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Data.Repositories;
using System.Linq;

public class RouteServiceTests
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
    public async Task GetAllRoutesAsync_Should_Return_Correct_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var city1 = new City { Id = Guid.NewGuid(), Name = "CityA", State = "CityA", ZipCode = "14114" };
        var city2 = new City { Id = Guid.NewGuid(), Name = "CityB", State = "CityB", ZipCode = "14113" };
        var street1 = new Street { Id = Guid.NewGuid(), Name = "Street1" };
        var street2 = new Street { Id = Guid.NewGuid(), Name = "Street2" };

        await dbContext.Cities.AddRangeAsync(city1, city2);
        await dbContext.Streets.AddRangeAsync(street1, street2);

        var route1 = new Route
        {
            Id = Guid.NewGuid(),
            FromCityId = city1.Id,
            ToCityId = city2.Id,
            FromStreetId = street1.Id,
            ToStreetId = street2.Id,
            Distance = 50,
            Duration = 60
        };

        var route2 = new Route
        {
            Id = Guid.NewGuid(),
            FromCityId = city2.Id,
            ToCityId = city1.Id,
            FromStreetId = street2.Id,
            ToStreetId = street1.Id,
            Distance = 70,
            Duration = 80
        };

        await dbContext.Routes.AddRangeAsync(route1, route2);
        await dbContext.SaveChangesAsync();

        var inputModel = new AllRoutesSearchFilterViewModel
        {
            SearchQuery = "CityA",
            CurrentPage = 1,
            EntitiesPerPage = 10
        };

        // Act
        var result = await routeService.GetAllRoutesAsync(inputModel);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, r => r.DepartingCity == "CityA");
    }
    [Fact]
    public async Task CreateAsync_Should_Add_Route()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var inputModel = new RouteInputModel
        {
            Duration = 60,
            Distance = 15.5,
            FromCityId = Guid.NewGuid().ToString(),
            ToCityId = Guid.NewGuid().ToString(),
            FromStreetId = Guid.NewGuid().ToString(),
            ToStreetId = Guid.NewGuid().ToString(),
            OrganizationId = Guid.NewGuid().ToString()
        };

        // Act
        await routeService.CreateAsync(inputModel);
        var route = await dbContext.Routes.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(route);
        Assert.Equal(60, route.Duration);
        Assert.Equal(15.5, route.Distance);
    }

    [Fact]
    public async Task DeleteRouteAsync_Should_Return_True_When_Route_Exists()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 10,
            Duration = 30
        };

        await dbContext.Routes.AddAsync(route);
        await dbContext.SaveChangesAsync();

        var inputModel = new RemoveRouteViewModel { Id = route.Id.ToString() };

        // Act
        var result = await routeService.DeleteRouteAsync(inputModel);

        // Assert
        Assert.True(result);
        Assert.Null(await dbContext.Routes.FindAsync(route.Id));
    }

    [Fact]
    public async Task GetRouteForEditAsync_Should_Return_Correct_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 100,
            Duration = 120
        };

        await dbContext.Routes.AddAsync(route);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await routeService.GetRouteForEditAsync(route.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("100", result.Distance.ToString());
    }

    [Fact]
    public async Task GetRouteForDeletionAsync_Should_Return_Correct_Data()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        // Create cities
        var city1 = new City { Id = Guid.NewGuid(), Name = "City A", State = "City A", ZipCode = "24151" };
        var city2 = new City { Id = Guid.NewGuid(), Name = "City B", State = "City B", ZipCode = "22351" };

        await dbContext.Cities.AddRangeAsync(city1, city2);
        await dbContext.SaveChangesAsync();

        // Create streets
        var street1 = new Street { Id = Guid.NewGuid(), Name = "Street A" };
        var street2 = new Street { Id = Guid.NewGuid(), Name = "Street B" };

        await dbContext.Streets.AddRangeAsync(street1, street2);
        await dbContext.SaveChangesAsync();

        // Create route
        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 100,
            Duration = 120,
            FromCityId = city1.Id,
            ToCityId = city2.Id,
            FromStreetId = street1.Id,
            ToStreetId = street2.Id
        };

        await dbContext.Routes.AddAsync(route);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await routeService.GetRouteForDeletionAsync(route.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(route.Id.ToString(), result.Id);
        Assert.Equal("City A", result.DepartingCity);
        Assert.Equal("City B", result.ArrivingCity);
        Assert.Equal("Street A", result.DepartingStreet);
        Assert.Equal("Street B", result.ArrivingStreet);
    }

    [Fact]
    public async Task GetRoutesForOrganizationAsync_Should_Return_Correct_Routes()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var organizationId = Guid.NewGuid();

        // Create cities
        var city1 = new City { Id = Guid.NewGuid(), Name = "City A", State = "City A", ZipCode = "24151" };
        var city2 = new City { Id = Guid.NewGuid(), Name = "City B", State = "City B", ZipCode = "22351" };

        await dbContext.Cities.AddRangeAsync(city1, city2);
        await dbContext.SaveChangesAsync(); 

        // Create streets
        var street1 = new Street { Id = Guid.NewGuid(), Name = "Street A" };
        var street2 = new Street { Id = Guid.NewGuid(), Name = "Street B" };

        await dbContext.Streets.AddRangeAsync(street1, street2);
        await dbContext.SaveChangesAsync();

        // Create route
        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 100,
            Duration = 120,
            FromCityId = city1.Id,
            ToCityId = city2.Id,
            FromStreetId = street1.Id,
            ToStreetId = street2.Id,
            OrganizationId = organizationId
        };

        await dbContext.Routes.AddAsync(route);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await routeService.GetRoutesForOrganizationAsync(organizationId.ToString());

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(route.Id.ToString(), result.First().Value);
    }

    [Fact]
    public async Task DeleteRouteAsync_Should_Return_False_When_Route_Does_Not_Exist()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var inputModel = new RemoveRouteViewModel { Id = Guid.NewGuid().ToString() };

        // Act
        var result = await routeService.DeleteRouteAsync(inputModel);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EditRouteAsync_Should_Return_True_When_Route_Is_Updated()
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var routeRepository = new EfDeletableEntityRepository<Route>(dbContext);
        var streetRepository = new EfDeletableEntityRepository<Street>(dbContext);
        var cityRepository = new EfDeletableEntityRepository<City>(dbContext);
        var routeService = new RouteService(routeRepository, streetRepository, cityRepository);

        var route = new Route
        {
            Id = Guid.NewGuid(),
            Distance = 10,
            Duration = 30
        };

        await dbContext.Routes.AddAsync(route);
        await dbContext.SaveChangesAsync();

        var editModel = new EditRouteInputModel
        {
            Id = route.Id.ToString(),
            Distance = 20,
            Duration = 45,
            FromCityId = Guid.NewGuid().ToString(),
            ToCityId = Guid.NewGuid().ToString(),
            FromStreetId = Guid.NewGuid().ToString(),
            ToStreetId = Guid.NewGuid().ToString(),
            OrganizationId = Guid.NewGuid().ToString()
        };

        // Act
        var result = await routeService.EditRouteAsync(editModel);
        var updatedRoute = await dbContext.Routes.FindAsync(route.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(20, updatedRoute.Distance);
        Assert.Equal(45, updatedRoute.Duration);
    }
}
