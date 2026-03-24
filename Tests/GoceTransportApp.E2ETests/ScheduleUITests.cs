using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class ScheduleUITests : BaseTest
{
    private string _orgId = null!;

    [SetUp]
    public async Task LoginAndGetOrg()
    {
        await LoginAsync();
        _orgId = await GetOrganizationIdAsync("Express Lines");
    }

    [Test]
    public async Task Can_Create_Schedule_Successfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Schedule/Create?organizationId={_orgId}");
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Schedule/Create", RegexOptions.IgnoreCase));

        // Day — hardcoded <option value="Monday">Monday</option>
        await Page.SelectOptionAsync("#Day", "Monday");

        // Time inputs (type="time" — format HH:mm)
        await Page.FillAsync("#Departure", "08:00");
        await Page.FillAsync("#Arrival", "10:30");

        // Vehicle & Route dropdowns populated by the server (seeded data)
        await Page.SelectOptionAsync("#VehicleId", new SelectOptionValue { Index = 1 });
        await Page.SelectOptionAsync("#RouteId", new SelectOptionValue { Index = 1 });

        await SubmitFormAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex(@"/Organization/Details", RegexOptions.IgnoreCase),
            new() { Timeout = 10_000 });
        await AssertSuccessToastAsync();
    }
}
