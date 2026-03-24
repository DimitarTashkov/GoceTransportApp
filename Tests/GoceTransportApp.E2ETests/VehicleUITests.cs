using System.Text.RegularExpressions;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class VehicleUITests : BaseTest
{
    private string _orgId = null!;

    [SetUp]
    public async Task LoginAndGetOrg()
    {
        await LoginAsync();
        _orgId = await GetOrganizationIdAsync("Express Lines");
    }

    [Test]
    public async Task Can_Create_Vehicle_Successfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Vehicle/Create?organizationId={_orgId}");
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Vehicle/Create", RegexOptions.IgnoreCase));

        // Generate a unique registration number
        string regNum = $"E2E{Guid.NewGuid():N}"[..8].ToUpper();
        await Page.FillAsync("#Number", regNum);
        await Page.FillAsync("#Type", "Minibus");
        await Page.FillAsync("#Manufacturer", "Mercedes");
        await Page.FillAsync("#Model", "Sprinter");
        await Page.FillAsync("#Capacity", "20");
        await Page.FillAsync("#FuelConsumption", "12");

        await SubmitFormAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex(@"/Organization/Details", RegexOptions.IgnoreCase),
            new() { Timeout = 10_000 });
        await AssertSuccessToastAsync();
    }
}
