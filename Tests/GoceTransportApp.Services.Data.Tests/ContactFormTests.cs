using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Services;
using GoceTransportApp.Web.ViewModels.ContactForms;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ContactFormServiceTests
{
    private readonly ApplicationDbContext dbContext;
    private readonly EfDeletableEntityRepository<ContactForm> contactFormRepository;
    private readonly ContactFormService contactFormService;

    public ContactFormServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        this.dbContext = new ApplicationDbContext(options);
        this.contactFormRepository = new EfDeletableEntityRepository<ContactForm>(this.dbContext);
        this.contactFormService = new ContactFormService(this.contactFormRepository);

        this.dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewContactForm()
    {
        // Arrange
        var model = new ContactFormInputModel
        {
            UserId = "user123",
            Email = "test@example.com",
            Title = "Test Title",
            Message = "Test Message"
        };

        // Act
        await contactFormService.CreateAsync(model);
        var forms = await dbContext.ContactForms.ToListAsync();

        // Assert
        Assert.Single(forms);
        Assert.Equal(model.Email, forms[0].Email);
        Assert.Equal(model.Title, forms[0].Title);
    }

    [Fact]
    public async Task GetAllFormsAsync_ShouldReturnCorrectData()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "user@example.com"
        };

        await dbContext.Users.AddAsync(user);

        await dbContext.ContactForms.AddRangeAsync(new List<ContactForm>
        {
            new ContactForm { Id = Guid.NewGuid(), Title = "First", Message = "Message One", Email = "one@example.com", DateSubmitted = DateTime.UtcNow, UserId = user.Id },
            new ContactForm { Id = Guid.NewGuid(), Title = "Second", Message = "Message Two", Email = "two@example.com", DateSubmitted = DateTime.UtcNow, UserId = user.Id },
        });
        await dbContext.SaveChangesAsync();

        var filter = new AllFormsSearchFilterViewModel { CurrentPage = 1, EntitiesPerPage = 10 };

        // Act
        var result = await contactFormService.GetAllFormsAsync(filter);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectContactForm()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "user@example.com"
        };

        await dbContext.Users.AddAsync(user);

        var contactForm = new ContactForm
        {
            Id = Guid.NewGuid(),
            Title = "Unique Title",
            Email = "unique@example.com",
            Message = "Test Message",
            DateSubmitted = DateTime.UtcNow,
            UserId = user.Id
        };

        await dbContext.ContactForms.AddAsync(contactForm);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await contactFormService.GetByIdAsync(contactForm.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contactForm.Title, result.Title);
        Assert.Equal(contactForm.Email, result.Email);
    }

    [Fact]
    public async Task DeleteFormAsync_ShouldRemoveContactForm()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var contactForm = new ContactForm
        {
            Id = Guid.NewGuid(),
            Title = "To Delete",
            Email = "delete@example.com",
            Message = "Delete me",
            UserId = userId
        };

        await dbContext.ContactForms.AddAsync(contactForm);
        await dbContext.SaveChangesAsync();

        // Act
        await contactFormService.DeleteFormAsync(contactForm.Id);
        var result = await dbContext.ContactForms.FindAsync(contactForm.Id);

        // Assert
        Assert.True(result.IsDeleted);
    }

    [Fact]
    public async Task GetFormDetailsByIdAsync_ShouldReturnDetails()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "user@example.com"
        };

        await dbContext.Users.AddAsync(user);

        var contactForm = new ContactForm
        {
            Id = Guid.NewGuid(),
            Title = "Detail Test",
            Email = "detail@example.com",
            Message = "Detailed message",
            DateSubmitted = DateTime.UtcNow,
            UserId = user.Id
        };

        await dbContext.ContactForms.AddAsync(contactForm);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await contactFormService.GetFormDetailsByIdAsync(contactForm.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contactForm.Title, result.Title);
        Assert.Equal(contactForm.Message, result.Message);
    }

    [Fact]
    public async Task GetFormsCountByFilterAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "user@example.com"
        };

        await dbContext.Users.AddAsync(user); await dbContext.ContactForms.AddRangeAsync(new List<ContactForm>
        {
            new ContactForm { Id = Guid.NewGuid(), Title = "First", Message = "Message One", Email = "one@example.com", UserId = user.Id },
            new ContactForm { Id = Guid.NewGuid(), Title = "Second", Message = "Message Two", Email = "two@example.com", UserId = user.Id },
            new ContactForm { Id = Guid.NewGuid(), Title = "Search Me", Message = "Filtered message", Email = "filter@example.com", UserId = user.Id }
        });

        await dbContext.SaveChangesAsync();

        var filter = new AllFormsSearchFilterViewModel { SearchQuery = "Search", CurrentPage = 1, EntitiesPerPage = 10 };

        // Act
        var count = await contactFormService.GetFormsCountByFilterAsync(filter);

        // Assert
        Assert.Equal(1, count);
    }
}
