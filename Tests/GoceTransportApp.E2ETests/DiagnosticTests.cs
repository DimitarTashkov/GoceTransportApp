using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GoceTransportApp.E2ETests;

[TestFixture]
public class DiagnosticTests : BaseTest
{
    [Test]
    public async Task Diagnose_Login_And_OrgCreate()
    {
        await LoginAsync();

        // 1) Where are we after login?
        TestContext.Progress.WriteLine($"[DIAG] URL after login: {Page.Url}");
        var bodyText = await Page.Locator("body").InnerTextAsync();
        TestContext.Progress.WriteLine($"[DIAG] Body contains 'Log out': {bodyText.Contains("Log out")}");
        TestContext.Progress.WriteLine($"[DIAG] Body contains 'Login': {bodyText.Contains("Log in")}");

        // 2) Navigate to Create
        await Page.GotoAsync($"{BaseUrl}/Organization/Create");
        TestContext.Progress.WriteLine($"[DIAG] URL after nav to Create: {Page.Url}");

        // 3) Check if FounderId hidden field has a value
        var founderInput = Page.Locator("input[name='FounderId']");
        var founderCount = await founderInput.CountAsync();
        TestContext.Progress.WriteLine($"[DIAG] FounderId inputs found: {founderCount}");
        if (founderCount > 0)
        {
            var founderVal = await founderInput.InputValueAsync();
            TestContext.Progress.WriteLine($"[DIAG] FounderId value: '{founderVal}'");
        }

        // 4) Check antiforgery token
        var tokenInput = Page.Locator("input[name='__RequestVerificationToken']");
        var tokenCount = await tokenInput.CountAsync();
        TestContext.Progress.WriteLine($"[DIAG] AntiforgeryToken inputs found: {tokenCount}");

        // 5) Fill and submit
        await Page.FillAsync("#Name", "DiagOrg12345");
        await Page.FillAsync("#Address", "Diag Street 99");
        await Page.FillAsync("#Phone", "+359888999000");

        await Page.Locator(".card-body button[type='submit']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 6) After submit diagnostics
        TestContext.Progress.WriteLine($"[DIAG] URL after submit: {Page.Url}");

        // 7) Capture page title and first 2000 chars of body
        var title = await Page.TitleAsync();
        TestContext.Progress.WriteLine($"[DIAG] Page title: {title}");

        var html = await Page.ContentAsync();
        var snippet = html.Length > 3000 ? html[..3000] : html;
        TestContext.Progress.WriteLine($"[DIAG] HTML snippet:\n{snippet}");

        // 8) Check validation errors
        var valErrors = await Page.Locator(".text-danger").AllInnerTextsAsync();
        TestContext.Progress.WriteLine($"[DIAG] Validation errors count: {valErrors.Count}");
        foreach (var err in valErrors)
            TestContext.Progress.WriteLine($"[DIAG]   >> {err}");

        // 9) Check all cookies
        var cookies = await Page.Context.CookiesAsync();
        foreach (var c in cookies)
            TestContext.Progress.WriteLine($"[DIAG] Cookie: {c.Name} = {c.Value[..Math.Min(30, c.Value.Length)]}...");

        Assert.Pass("Diagnostic test — check Progress output above");
    }
}
