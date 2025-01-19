using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GoceTransportApp.Common;
using GoceTransportApp.Services;
using GoceTransportApp.Services.Data.ContactForms;
using GoceTransportApp.Web.ViewModels.ContactForms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoceTransportApp.Controllers
{
    [Authorize]
    public class ContactFormController : Controller
    {
        private readonly IContactFormService contactFormService;

        public ContactFormController(IContactFormService contactFormService)
        {
            this.contactFormService = contactFormService;
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]

        public IActionResult Index()
        {
            var contactForms = contactFormService.GetAllFormsAsync();
            return View(contactForms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContactFormInputModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await contactFormService.CreateAsync(model, userId);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var contactForm = await contactFormService.GetByIdAsync(id);
            if (contactForm == null)
            {
                return NotFound();
            }

            return View(contactForm);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await contactFormService.DeleteFormAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
