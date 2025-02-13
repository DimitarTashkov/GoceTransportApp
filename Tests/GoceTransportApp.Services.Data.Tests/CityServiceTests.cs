using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Web.ViewModels.Cities;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CityServiceTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        return dbContext;
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCity()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var input = new CityInputModel
        {
            Name = "New City",
            State = "Test State",
            ZipCode = "12345"
        };

        await cityService.CreateAsync(input);
        var city = await dbContext.Cities.FirstOrDefaultAsync();

        Assert.NotNull(city);
        Assert.Equal(input.Name, city.Name);
        Assert.Equal(input.State, city.State);
        Assert.Equal(input.ZipCode, city.ZipCode);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldDeleteCity()
    {
        var dbContext = await GetDatabaseContext();
        var city = new City { Id = Guid.NewGuid(), Name = "Delete City", State = "Test State", ZipCode = "12345" };
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.DeleteCityAsync(city.Id);
        Assert.True(result);

        var deletedCity = await dbContext.Cities.FindAsync(city.Id);
        Assert.True(deletedCity.IsDeleted);
    }

    [Fact]
    public async Task EditCityAsync_ShouldUpdateCity()
    {
        var dbContext = await GetDatabaseContext();
        var city = new City { Id = Guid.NewGuid(), Name = "Old Name", State = "Old State", ZipCode = "00000" };
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var inputModel = new EditCityInputModel
        {
            Id = city.Id.ToString(),
            Name = "New Name",
            State = "New State",
            ZipCode = "99999"
        };

        var result = await cityService.EditCityAsync(inputModel);
        Assert.True(result);

        var updatedCity = await dbContext.Cities.FindAsync(city.Id);
        Assert.Equal("New Name", updatedCity.Name);
        Assert.Equal("New State", updatedCity.State);
        Assert.Equal("99999", updatedCity.ZipCode);
    }

    [Fact]
    public async Task GetAllCitiesAsync_ShouldReturnCities()
    {
        var dbContext = await GetDatabaseContext();
        var cities = new List<City>
        {
            new City { Id = Guid.NewGuid(), Name = "City 1", State = "State 1", ZipCode = "11111" },
            new City { Id = Guid.NewGuid(), Name = "City 2", State = "State 2", ZipCode = "22222" }
        };

        await dbContext.Cities.AddRangeAsync(cities);
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetAllCitiesAsync(new AllCitiesSearchFilterViewModel());
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCityDetailsAsync_ShouldReturnDetails()
    {
        var dbContext = await GetDatabaseContext();
        var city = new City { Id = Guid.NewGuid(), Name = "Detail City", State = "State X", ZipCode = "55555" };
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetCityDetailsAsync(city.Id);
        Assert.NotNull(result);
        Assert.Equal(city.Name, result.Name);
        Assert.Equal(city.State, result.State);
        Assert.Equal(city.ZipCode, result.ZipCode);
    }

    [Fact]
    public async Task GetCitiesCountByFilterAsync_ShouldReturnCount()
    {
        var dbContext = await GetDatabaseContext();
        await dbContext.Cities.AddAsync(new City { Id = Guid.NewGuid(), Name = "Filter City", State = "State X", ZipCode = "66666" });
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var count = await cityService.GetCitiesCountByFilterAsync(new AllCitiesSearchFilterViewModel());
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetAllCitiesForDropDownsAsync_ShouldReturnCities()
    {
        var dbContext = await GetDatabaseContext();
        await dbContext.Cities.AddAsync(new City { Id = Guid.NewGuid(), Name = "Dropdown City", State = "State Z", ZipCode = "77777" });
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetAllCitiesForDropDownsAsync();
        Assert.Single(result);
    }
    [Fact]
    public async Task AddStreetToCityAsync_ShouldReturnFalse_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.AddStreetToCityAsync(Guid.NewGuid(), new CityAddStreetInputModel());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldReturnFalse_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.DeleteCityAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task EditCityAsync_ShouldReturnFalse_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.EditCityAsync(new EditCityInputModel
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Non-Existent City",
            State = "Nowhere",
            ZipCode = "00000"
        });

        Assert.False(result);
    }

    [Fact]
    public async Task GetCityDetailsAsync_ShouldReturnNull_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetCityDetailsAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllCitiesAsync_ShouldReturnEmptyList_WhenNoCitiesExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetAllCitiesAsync(new AllCitiesSearchFilterViewModel());

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddStreetToCityAsync_ShouldNotAddDuplicateStreet()
    {
        var dbContext = await GetDatabaseContext();
        var city = new City { Id = Guid.NewGuid(), Name = "City", State = "State", ZipCode = "12345" };
        var street = new Street { Id = Guid.NewGuid(), Name = "Main Street" };
        var cityStreet = new CityStreet { CityId = city.Id, StreetId = street.Id };

        await dbContext.Cities.AddAsync(city);
        await dbContext.Streets.AddAsync(street);
        await dbContext.CitiesStreets.AddAsync(cityStreet);
        await dbContext.SaveChangesAsync();

        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var model = new CityAddStreetInputModel
        {
            Streets = new List<StreetCheckBoxItemInputModel>
            {
                new StreetCheckBoxItemInputModel { Id = street.Id.ToString(), IsSelected = true }
            }
        };

        var result = await cityService.AddStreetToCityAsync(city.Id, model);

        Assert.True(result);

        var streetsInCity = await dbContext.CitiesStreets.Where(cs => cs.CityId == city.Id).ToListAsync();
        Assert.Single(streetsInCity);
    }

    [Fact]
    public async Task GetCityDetailsByNameAsync_ShouldReturnNull_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetCityDetailsByNameAsync("NonExistentCity");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCityForEditAsync_ShouldReturnNull_WhenCityDoesNotExist()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var result = await cityService.GetCityForEditAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCitiesCountByFilterAsync_ShouldReturnZero_WhenNoCitiesMatchFilter()
    {
        var dbContext = await GetDatabaseContext();
        var cityService = new CityService(
            new EfDeletableEntityRepository<City>(dbContext),
            new EfDeletableEntityRepository<Street>(dbContext),
            new EfDeletableEntityRepository<CityStreet>(dbContext));

        var count = await cityService.GetCitiesCountByFilterAsync(new AllCitiesSearchFilterViewModel { SearchQuery = "NonExistentCity" });

        Assert.Equal(0, count);
    }
}
