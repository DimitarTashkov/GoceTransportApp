using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GoceTransportApp.Common;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services;
using GoceTransportApp.Services.Data.ContactForms;
using GoceTransportApp.Services.Data.Routes;
using GoceTransportApp.Web.Hubs;
using GoceTransportApp.Web.ViewModels.ContactForms;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.ResultMessages.ContactFormMessages;

namespace GoceTransportApp.Controllers
{
    [Authorize]
    public class ContactFormController : Controller
    {
        private readonly IContactFormService contactFormService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<NotificationHub> hubContext;

        public ContactFormController(
            IContactFormService contactFormService,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            this.contactFormService = contactFormService;
            this.userManager = userManager;
            this.hubContext = hubContext;
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]

        public async Task<IActionResult> Index(AllFormsSearchFilterViewModel inputModel)
        {
            IEnumerable<ContactFormDataViewModel> allForms = await contactFormService.GetAllFormsAsync(inputModel);

            int allRoutesCount = await contactFormService.GetFormsCountByFilterAsync(inputModel);

            AllFormsSearchFilterViewModel viewModel = new AllFormsSearchFilterViewModel
            {
                Forms = allForms,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allRoutesCount / inputModel.EntitiesPerPage.Value)
            };

            return View(viewModel);
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

                var admins = await userManager.GetUsersInRoleAsync(GlobalConstants.AdministratorRoleName);
                var adminIds = admins.Select(a => a.Id).ToList();
                if (adminIds.Count > 0)
                {
                    await hubContext.Clients.Users(adminIds)
                        .SendAsync("ReceiveSystemAlert", "???? ????????? ?? ??????????? ?????!");
                }

                TempData[nameof(SuccessMessage)] = ContactFormWasSumbitted;

                return RedirectToAction("Index", "Home");
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

            TempData[nameof(SuccessMessage)] = SuccessMessage;

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
