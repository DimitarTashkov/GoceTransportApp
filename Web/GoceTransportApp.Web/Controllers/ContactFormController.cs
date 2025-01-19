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
    //TODO: Implement searching, pagination and to display latest forms from top to bottom
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

        public async Task<IActionResult> Index()
        {
            var contactForms = await contactFormService.GetAllFormsAsync();
            return View(contactForms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ContactFormInputModel model = new ContactFormInputModel();
            model.UserId = userId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContactFormInputModel model)
        {
            if (ModelState.IsValid)
            {
                await contactFormService.CreateAsync(model);
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

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var contactForm = await contactFormService.GetFormDetailsByIdAsync(id);
            if (contactForm == null)
            {
                return NotFound();
            }

            return View(contactForm);
        }
    }
}
