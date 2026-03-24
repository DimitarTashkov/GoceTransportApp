using System.Text.RegularExpressions;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class OrganizationUITests : BaseTest
{
    [SetUp]
    public async Task Login() => await LoginAsync();

    [Test]
    public async Task Can_Create_Organization_Successfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Organization/Create");
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Organization/Create", RegexOptions.IgnoreCase));

        string orgName = $"E2E Org {Guid.NewGuid():N}"[..20];
        await Page.FillAsync("#Name", orgName);
        await Page.FillAsync("#Address", "E2E Test Street 42, Sofia");
        await Page.FillAsync("#Phone", "+359888000111");

        await SubmitFormAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex(@"/Organization/Details", RegexOptions.IgnoreCase),
            new() { Timeout = 10_000 });
        await AssertSuccessToastAsync();
    }

    [Test]
    public async Task Create_Organization_WithEmptyName_ShowsValidationError()
    {
        await Page.GotoAsync($"{BaseUrl}/Organization/Create");

        // Leave Name empty, fill only Address
        await Page.FillAsync("#Address", "Some Street 1, Sofia");

        await SubmitFormAsync();

        // Should stay on Create page — client-side validation fires
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Organization/Create", RegexOptions.IgnoreCase));
        await Expect(Page.Locator("span[data-valmsg-for='Name']")).Not.ToBeEmptyAsync();
    }
}
