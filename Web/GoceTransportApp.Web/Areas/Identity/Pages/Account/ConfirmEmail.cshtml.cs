#nullable disable

namespace CinemaApp.Web.Areas.Identity.Pages.Account
{
    using GoceTransportApp.Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // 1. ЗАЩИТА ОТ "ПРИЗРАЧНИ КЛИКОВЕ" НА АНТИВИРУСНИ БОТОВЕ
            if (user.EmailConfirmed)
            {
                StatusMessage = "Вашият имейл вече е успешно потвърден!";
                return Page();
            }

            // 2. ДЕКОДИРАНЕ И ПРОВЕРКА
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                StatusMessage = "Благодарим ви, имейлът ви е потвърден.";
            }
            else
            {
                // 3. ПОКАЗВАНЕ НА ТОЧНАТА ГРЕШКА
                var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));
                StatusMessage = $"Грешка при потвърждение на имейла: {errorMessages}";
            }

            return Page();
        }
    }
}
