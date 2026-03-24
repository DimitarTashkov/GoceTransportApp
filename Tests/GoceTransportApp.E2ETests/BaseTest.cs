using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace GoceTransportApp.E2ETests;

/// <summary>
/// Base class for all E2E tests.
/// Handles browser setup, cookie consent, login, and reusable helpers.
/// </summary>
public class BaseTest : PageTest
{
    protected const string BaseUrl = "https://localhost:5001";

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true,
        ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
    };

    /// <summary>
    /// Pre-sets the ASP.NET Core cookie consent cookie so authentication
    /// and antiforgery cookies are stored correctly.
    /// </summary>
    protected async Task GrantCookieConsentAsync()
    {
        await Page.Context.AddCookiesAsync(new[]
        {
            new Cookie
            {
                Name = ".AspNet.Consent",
                Value = "yes",
                Domain = "localhost",
                Path = "/",
                Secure = true,
                Expires = (float)DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds(),
            }
        });
    }

    /// <summary>
    /// Logs in with specified credentials. Defaults to seeded org owner org1@test.com.
    /// </summary>
    protected async Task LoginAsync(string email = "org1@test.com", string password = "Test1234!")
    {
        await GrantCookieConsentAsync();
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Page.FillAsync("#Input_Email", email);
        await Page.FillAsync("#Input_Password", password);
        await Page.ClickAsync("#login-submit");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Navigates to UserOrganizations and returns the org GUID matching the given name.
    /// Falls back to the first org if name is null.
    /// </summary>
    protected async Task<string> GetOrganizationIdAsync(string? orgName = null)
    {
        await Page.GotoAsync($"{BaseUrl}/Organization/UserOrganizations");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        ILocator card;
        if (orgName != null)
        {
            // Find the card whose title matches the org name
            card = Page.Locator(".card", new() { HasText = orgName }).First;
        }
        else
        {
            card = Page.Locator(".card").First;
        }

        var link = card.Locator("a[href*='/Organization/Details']");
        var href = await link.GetAttributeAsync("href");
        var uri = new Uri(new Uri(BaseUrl), href!);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["id"]!;
    }

    /// <summary>
    /// Clicks the primary submit button inside the card body form.
    /// Scoped to .card-body to avoid hitting the navbar Logout button.
    /// </summary>
    protected async Task SubmitFormAsync()
    {
        await Page.Locator(".card-body button[type='submit']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Asserts the success toast notification is visible.
    /// </summary>
    protected async Task AssertSuccessToastAsync()
    {
        await Expect(Page.Locator(".toast.text-bg-success"))
            .ToBeVisibleAsync(new() { Timeout = 5000 });
    }
}
