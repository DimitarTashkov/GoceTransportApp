using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GoceTransportApp.Common;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services;
using GoceTransportApp.Services.Data.ContactForms;
using GoceTransportApp.Services.Messaging;
using GoceTransportApp.Web.Hubs;
using GoceTransportApp.Web.ViewModels.ContactForms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

using static GoceTransportApp.Common.GlobalConstants;
using static GoceTransportApp.Common.ResultMessages.ContactFormMessages;
using static GoceTransportApp.Common.ResultMessages.GeneralMessages;

namespace GoceTransportApp.Controllers
{
    [Authorize]
    public class ContactFormController : Controller
    {
        private readonly IContactFormService contactFormService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly IEmailSender emailSender;
        private readonly EmailSettings emailSettings;

        public ContactFormController(
            IContactFormService contactFormService,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            IEmailSender emailSender,
            IOptions<EmailSettings> emailSettings)
        {
            this.contactFormService = contactFormService;
            this.userManager = userManager;
            this.hubContext = hubContext;
            this.emailSender = emailSender;
            this.emailSettings = emailSettings.Value;
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
        [AllowAnonymous]
        public IActionResult Create()
        {
            string userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            ContactFormInputModel model = new ContactFormInputModel();
            model.UserId = userId;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(ContactFormInputModel model)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                model.UserId = null;
            }

            if (ModelState.IsValid)
            {
                await contactFormService.CreateAsync(model);

                var admins = await userManager.GetUsersInRoleAsync(GlobalConstants.AdministratorRoleName);
                var adminIds = admins.Select(a => a.Id).ToList();
                if (adminIds.Count > 0)
                {
                    await hubContext.Clients.Users(adminIds)
                        .SendAsync(SignalRMethods.ReceiveSystemAlert, NewContactFormAlert);
                }

                TempData[nameof(SuccessMessage)] = ContactFormWasSumbitted;

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Reply(Guid id)
        {
            var contactForm = await contactFormService.GetFormForReplyAsync(id);
            if (contactForm == null)
            {
                return NotFound();
            }

            return View(contactForm);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Reply(ContactFormReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var original = await contactFormService.GetFormForReplyAsync(model.Id);
                if (original == null)
                {
                    return NotFound();
                }

                original.ReplyText = model.ReplyText;
                return View(original);
            }

            var contactForm = await contactFormService.GetFormForReplyAsync(model.Id);
            if (contactForm == null)
            {
                return NotFound();
            }

            string htmlContent = $@"
                <h2>Re: {contactForm.Title}</h2>
                <p>{model.ReplyText}</p>
                <hr />
                <p><strong>Original message:</strong></p>
                <p>{contactForm.Message}</p>";

            await emailSender.SendEmailAsync(
                emailSettings.SenderEmail,
                emailSettings.SenderName,
                contactForm.Email,
                $"Re: {contactForm.Title}",
                htmlContent);

            await contactFormService.ReplyAsync(model.Id, model.ReplyText);

            TempData[nameof(SuccessMessage)] = ContactFormReplySuccess;

            return RedirectToAction(nameof(Index));
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
