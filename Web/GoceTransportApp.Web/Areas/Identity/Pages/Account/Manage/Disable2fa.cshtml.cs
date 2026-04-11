#nullable disable

namespace GoceTransportApp.Web.Areas.Identity.Pages.Account.Manage
{
    using System;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public Disable2faModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!await userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException("Cannot disable 2FA as it's not currently enabled.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException("Unexpected error disabling 2FA.");
            }

            StatusMessage = "2FA has been disabled. You can re-enable 2FA when you setup an authenticator app.";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
