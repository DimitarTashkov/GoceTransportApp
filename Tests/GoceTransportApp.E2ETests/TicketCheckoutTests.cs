using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class TicketCheckoutTests : BaseTest
{
    [Test]
    public async Task OrgOwner_Can_Create_Ticket()
    {
        await LoginAsync(); // org1@test.com
        var orgId = await GetOrganizationIdAsync("Express Lines");

        await Page.GotoAsync($"{BaseUrl}/Ticket/Create?organizationId={orgId}");
        await Expect(Page).ToHaveURLAsync(new Regex(@"/Ticket/Create", RegexOptions.IgnoreCase));

        // Dates (type="date" — format yyyy-MM-dd)
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var nextWeek = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        await Page.FillAsync("#IssuedDate", today);
        await Page.FillAsync("#ExpiryDate", nextWeek);

        await Page.FillAsync("#Price", "25");

        // Route & Schedule dropdowns (seeded data)
        await Page.SelectOptionAsync("#RouteId", new SelectOptionValue { Index = 1 });
        await Page.SelectOptionAsync("#ScheduleId", new SelectOptionValue { Index = 1 });

        await SubmitFormAsync();

        // Redirects to /Organization/Tickets?organizationId=...
        await Expect(Page).ToHaveURLAsync(
            new Regex(@"/Organization/Tickets", RegexOptions.IgnoreCase),
            new() { Timeout = 10_000 });
        await AssertSuccessToastAsync();
    }

    [Test]
    public async Task Passenger_Can_See_Upcoming_Ticket_In_MyTickets()
    {
        await LoginAsync("passenger@test.com"); // seeded passenger with 1 ticket

        await Page.GotoAsync($"{BaseUrl}/Ticket/MyTickets");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // The seeded ticket: Express Lines, Sofia → Plovdiv, 35 лв.
        await Expect(Page.Locator("text=Sofia")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Plovdiv")).ToBeVisibleAsync();
        await Expect(Page.Locator("text=Express Lines")).ToBeVisibleAsync();
    }
}
