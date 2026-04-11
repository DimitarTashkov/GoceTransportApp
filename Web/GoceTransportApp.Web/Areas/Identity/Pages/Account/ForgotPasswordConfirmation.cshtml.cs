#nullable disable

namespace CinemaApp.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [AllowAnonymous]
    public class ForgotPasswordConfirmationModel : PageModel
    {
        public IActionResult OnGet() => Page();
    }
}
