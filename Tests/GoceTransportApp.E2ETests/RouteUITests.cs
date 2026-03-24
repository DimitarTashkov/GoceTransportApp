using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class RouteUITests : BaseTest
{
    private string _orgId = null!;

    [SetUp]
    public async Task LoginAndGetOrg()
    {
        await LoginAsync();
        _orgId = await GetOrganizationIdAsync("Express Lines");
    }

    [Test]
    public async Task Can_Create_Route_Successfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Route/Create?organizationId={_orgId}");
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Route/Create", RegexOptions.IgnoreCase));

        // ── From: select city, wait for AJAX streets, select street ──
        await Page.SelectOptionAsync("#fromCity", new SelectOptionValue { Label = "Sofia" });
        // Wait for the AJAX-loaded street dropdown to have options
        await Page.WaitForFunctionAsync(
            "() => { var s = document.getElementById('fromStreet'); return !s.disabled && s.options.length > 1; }",
            null, new() { Timeout = 10_000 });
        await Page.SelectOptionAsync("#fromStreet", new SelectOptionValue { Index = 1 });

        // ── To: select city, wait for AJAX streets, select street ──
        await Page.SelectOptionAsync("#toCity", new SelectOptionValue { Label = "Plovdiv" });
        await Page.WaitForFunctionAsync(
            "() => { var s = document.getElementById('toStreet'); return !s.disabled && s.options.length > 1; }",
            null, new() { Timeout = 10_000 });
        await Page.SelectOptionAsync("#toStreet", new SelectOptionValue { Index = 1 });

        // Distance and duration
        await Page.FillAsync("#Distance", "150");
        await Page.FillAsync("#Duration", "120");

        await SubmitFormAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex(@"/Organization/Details", RegexOptions.IgnoreCase),
            new() { Timeout = 10_000 });
        await AssertSuccessToastAsync();
    }
}
