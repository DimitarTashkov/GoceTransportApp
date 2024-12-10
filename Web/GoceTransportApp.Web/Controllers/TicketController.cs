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

namespace GoceTransportApp.Web.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketController(ITicketService ticketService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.ticketService = ticketService;
        }

        [HttpGet]
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
        public async Task<IActionResult> Create()
        {
            TicketInputModel model = new TicketInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await ticketService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid scheduleGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref scheduleGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            EditTicketInputModel? formModel = await this.ticketService
                .GetTicketForEditAsync(scheduleGuid);

            if (formModel == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTicketInputModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            RemoveTicketViewModel? model = await ticketService
                .GetTicketForDeletionAsync(ticketGuid);

            if (model == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveTicketViewModel formModel)
        {
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

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            TicketDetailsViewModel? model = await ticketService
                .GetTicketDetailsAsync(ticketGuid);

            if (model == null)
            {
                TempData[nameof(InvalidTicketDetails)] = InvalidTicketDetails;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Purchase(string? id)
        {
            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);

            if (!isIdValid)
            {
                return RedirectToAction(nameof(Index));
            }

            TicketDetailsViewModel model = await ticketService.GetTicketDetailsAsync(ticketGuid);

            if (model == null)
            {
                TempData[nameof(InvalidTicketDetails)] = InvalidTicketDetails;
                return RedirectToAction(nameof(Index));
            }

            // Add a property to the model to store the number of tickets the user wants to buy
            model.QuantityToBuy = 1; // Default to 1 ticket
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(TicketDetailsViewModel model)
        {
            if (model.QuantityToBuy <= 0)
            {
                ModelState.AddModelError("", "Please select a valid number of tickets.");
                return View(model);
            }

            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(model.Id, ref ticketGuid);

            if (!isIdValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var purchaseSuccess = await ticketService.BuyTicketsAsync(ticketGuid, model.QuantityToBuy);

            if (!purchaseSuccess)
            {
                ModelState.AddModelError(nameof(TicketPurchaseFailed), TicketPurchaseFailed);
                return View(model);
            }

            TempData[nameof(TicketPurchaseSuccess)] = TicketPurchaseSuccess;
            return RedirectToAction(nameof(Index));
        }
    }
}
