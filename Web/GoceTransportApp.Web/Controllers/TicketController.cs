using GoceTransportApp.Services.Data.Schedules;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Web.ViewModels.Schedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Tickets;

using static GoceTransportApp.Common.ErrorMessages.TicketMessages;

namespace GoceTransportApp.Web.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await ticketService.GetAllTickets();

            return View(model);
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
                .GetTicketForEdit(scheduleGuid);

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
                .GetTicketForDeletion(ticketGuid);

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
                .TicketDetails(ticketGuid);

            if (model == null)
            {
                TempData[nameof(InvalidTicketDetails)] = InvalidTicketDetails;

                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
