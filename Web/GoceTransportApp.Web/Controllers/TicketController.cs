using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Web.ViewModels.Schedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Tickets;

using static GoceTransportApp.Common.ErrorMessages.TicketMessages;
using System.Collections.Generic;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketController(ITicketService ticketService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.ticketService = ticketService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(AllTicketsSearchFilterViewModel inputModel)
        {
            IEnumerable<TicketDataViewModel> allTickets =
            await ticketService.GetAllTicketsAsync(inputModel);

            int allTicketsCount = await ticketService.GetTicketsCountByFilterAsync(inputModel);

            AllTicketsSearchFilterViewModel viewModel = new AllTicketsSearchFilterViewModel
            {
                Tickets = allTickets,
                SearchQuery = inputModel.SearchQuery,
                PriceFrom = inputModel.PriceFrom,
                PriceTo = inputModel.PriceTo,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allTicketsCount / inputModel.EntitiesPerPage.Value)
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            TicketInputModel model = new TicketInputModel();
            model.OrganizationId = organizationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await ticketService.CreateAsync(model);

            return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string organizationId)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            EditTicketInputModel? formModel = await this.ticketService
                .GetTicketForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTicketInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isUpdated = await this.ticketService
                .EditTicketAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(TicketEditFailed), TicketEditFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id, string organizationId)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            RemoveTicketViewModel? model = await ticketService
                .GetTicketForDeletionAsync(ticketGuid);

            if (model == null)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveTicketViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.ticketService
                .RemoveTicketAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(TicketDeleteFailed), TicketDeleteFailed);

                return this.View(formModel);
            }

            return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id, string organizationId)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);
            if (!isIdValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            TicketDetailsViewModel? model = await ticketService
                .GetTicketDetailsAsync(ticketGuid);

            if (model == null)
            {
                TempData[nameof(InvalidTicketDetails)] = InvalidTicketDetails;

                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Purchase(string? id, string organizationId)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            TicketDetailsViewModel model = await ticketService.GetTicketDetailsAsync(ticketGuid);

            if (model == null)
            {
                TempData[nameof(InvalidTicketDetails)] = InvalidTicketDetails;
                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            model.QuantityToBuy = 1;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(TicketDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });

            }

            if (model.QuantityToBuy <= 0)
            {
                ModelState.AddModelError("InvalidQuantity", "Please select a valid number of tickets.");
                return View(model);
            }

            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(model.Id, ref ticketGuid);

            if (!isIdValid)
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            var purchaseSuccess = await ticketService.BuyTicketsAsync(ticketGuid, model.QuantityToBuy);

            if (!purchaseSuccess)
            {
                ModelState.AddModelError(nameof(TicketPurchaseFailed), TicketPurchaseFailed);
                return View(model);
            }

            TempData[nameof(TicketPurchaseSuccess)] = TicketPurchaseSuccess;
            return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
        }
    }
}
