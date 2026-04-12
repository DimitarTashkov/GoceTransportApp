using GoceTransportApp.Services.Data.Notifications;
using GoceTransportApp.Services.Data.Schedules;
using Microsoft.EntityFrameworkCore;
using GoceTransportApp.Services.Data.Tickets;
using GoceTransportApp.Services.Messaging;
using GoceTransportApp.Web.ViewModels.Schedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Tickets;

using static GoceTransportApp.Common.GlobalConstants;
using static GoceTransportApp.Common.GlobalConstants.RateLimitPolicies;
using static GoceTransportApp.Common.GlobalConstants.SignalRMethods;
using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.ResultMessages.TicketMessages;
using System.Collections.Generic;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Mail;
using GoceTransportApp.Services.Data.Routes;
using System.Linq;
using Microsoft.Extensions.Configuration;
using GoceTransportApp.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly IScheduleService scheduleService;
        private readonly IRouteService routeService;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationService notificationService;
        private readonly IDeletableEntityRepository<Organization> organizationRepository;

        public TicketController(
            ITicketService ticketService,
            IDeletableEntityRepository<Organization> organizationRepository,
            IScheduleService scheduleService,
            IRouteService routeService,
            IEmailSender emailSender,
            IConfiguration configuration,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
            : base(organizationRepository)
        {
            this.ticketService = ticketService;
            this.organizationRepository = organizationRepository;
            this.scheduleService = scheduleService;
            this.routeService = routeService;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.hubContext = hubContext;
            this.notificationService = notificationService;
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
                FilterDate = inputModel.FilterDate,
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

            if (!await this.HasUserCreatedOrganizationAsync(userId, organizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("UserOrganizations", "Organization");
            }

            var routes = await routeService.GetRoutesForOrganizationAsync(organizationId);
            var schedules = await scheduleService.GetSchedulesForOrganizationAsync(organizationId);

            var model = new TicketInputModel
            {
                OrganizationId = organizationId,
                Routes = routes.ToList(),
                Schedules = schedules.ToList(),
                IssuedDate = DateTime.Now.Date,
                ExpiryDate = DateTime.Now.Date
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                model.Routes = (await routeService.GetRoutesForOrganizationAsync(model.OrganizationId)).ToList();
                model.Schedules = (await scheduleService.GetSchedulesForOrganizationAsync(model.OrganizationId)).ToList();

                return View(model);
            }

            bool isCreated = await ticketService.CreateAsync(model);

            if (!isCreated)
            {
                ModelState.AddModelError(string.Empty, ScheduleCapacityExceeded);
                model.Routes = (await routeService.GetRoutesForOrganizationAsync(model.OrganizationId)).ToList();
                model.Schedules = (await scheduleService.GetSchedulesForOrganizationAsync(model.OrganizationId)).ToList();
                return View(model);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            // Send ticket-issued confirmation to the org owner
            await this.SendTicketCreatedEmailAsync(userId, model);

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
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            formModel.Routes = await routeService.GetRoutesForOrganizationAsync(organizationId);
            formModel.Schedules = await scheduleService.GetSchedulesForOrganizationAsync(organizationId);

            return this.View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTicketInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = formModel.OrganizationId });
            }

            if (!ModelState.IsValid)
            {
                formModel.Routes = await routeService.GetRoutesForOrganizationAsync(formModel.OrganizationId);
                formModel.Schedules = await scheduleService.GetSchedulesForOrganizationAsync(formModel.OrganizationId);
                return this.View(formModel);
            }

            bool isUpdated = await this.ticketService
                .EditTicketAsync(formModel);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

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
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, model.OrganizationId) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveTicketViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.OrganizationId) && !User.IsInRole(AdministratorRoleName))
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
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

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
                TempData[nameof(FailMessage)] = FailMessage;

                return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyTickets()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            MyTicketsViewModel model = await this.ticketService.GetMyTicketsAsync(userId);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string ticketId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid ticketGuid = Guid.Empty;
            bool isValid = IsGuidValid(ticketId, ref ticketGuid);

            if (!isValid)
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return RedirectToAction(nameof(MyTickets));
            }

            // Load ticket details before cancellation for the email
            TicketDetailsViewModel ticketDetails = await this.ticketService.GetTicketDetailsAsync(ticketGuid);

            bool isCancelled = await this.ticketService.CancelTicketAsync(userId, ticketGuid);

            if (!isCancelled)
            {
                TempData[nameof(FailMessage)] = TicketCancelFail;
            }
            else
            {
                TempData[nameof(SuccessMessage)] = TicketCancelSuccess;
                await this.SendCancellationEmailAsync(userId, ticketDetails);
            }

            return RedirectToAction(nameof(MyTickets));
        }

        [HttpPost]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting(RateLimitPolicies.Purchase)]
        public async Task<IActionResult> Purchase(string? id, string organizationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid ticketGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref ticketGuid);
            if (!isIdValid)
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return RedirectToAction("Details", "Organization", new { id = organizationId });
            }

            bool success = await this.ticketService.PurchaseTicketAsync(userId, ticketGuid);
            if (!success)
            {
                TempData[nameof(FailMessage)] = TicketPurchaseFail;
            }
            else
            {
                TempData[nameof(SuccessMessage)] = TicketPurchaseSuccess;

                var details = await this.ticketService.GetTicketDetailsAsync(ticketGuid);
                if (details != null)
                {
                    // Store purchase details in TempData so the redirected page can show
                    // the toast after the new SignalR connection is established.
                    // (Sending directly here causes a race condition â€” the message arrives
                    //  on the old connection that is being torn down during navigation.)
                    TempData[TempDataKeys.PurchaseFrom] = details.FromCity;
                    TempData[TempDataKeys.PurchaseTo] = details.ToCity;
                    TempData[TempDataKeys.PurchaseOrg] = details.OrganizationName;

                    var org = await this.organizationRepository.AllAsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id.ToString() == details.OrganizationId);

                    await this.notificationService.CreateAsync(
                        userId,
                        string.Format(TicketPurchaseNotification, details.FromCity, details.ToCity),
                        organizationFounderId: org?.FounderId);
                    await this.hubContext.Clients.User(userId)
                        .SendAsync(ReceiveNotification);

                    DateTime? departureDateTime = await this.ticketService.GetTicketDepartureDateTimeAsync(ticketGuid);
                    if (departureDateTime.HasValue)
                    {
                        TimeSpan delay = departureDateTime.Value.AddMinutes(-30) - DateTime.Now;
                        string capturedUserId = userId;
                        string capturedFrom = details.FromCity;
                        string capturedTo = details.ToCity;
                        _ = Task.Run(async () =>
                        {
                            if (delay > TimeSpan.Zero)
                                await Task.Delay(delay);
                            await this.hubContext.Clients.User(capturedUserId)
                                .SendAsync(SignalRMethods.ReceiveDepartureReminder, capturedFrom, capturedTo);
                        });
                    }
                }
            }

            return RedirectToAction(nameof(MyTickets));
        }

        //[HttpGet]
        //public async Task<IActionResult> Purchase_OLD(string? id, string organizationId)
        //{
        //    Guid ticketGuid = Guid.Empty;
        //    bool isIdValid = IsGuidValid(id, ref ticketGuid);

        //    if (!isIdValid)
        //    {
        //        return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
        //    }

        //    TicketDetailsViewModel model = await ticketService.GetTicketDetailsAsync(ticketGuid);

        //    if (model == null)
        //    {
        //        TempData[nameof(FailMessage)] = FailMessage;
        //        return RedirectToAction("Tickets", "Organization", new { organizationId = organizationId });
        //    }

        //    model.QuantityToBuy = 1;
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Purchase(TicketDetailsViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });

        //    }

        //    if (model.QuantityToBuy <= 0)
        //    {
        //        ModelState.AddModelError("InvalidQuantity", "Please select a valid number of tickets.");
        //        return View(model);
        //    }

        //    Guid ticketGuid = Guid.Empty;
        //    bool isIdValid = IsGuidValid(model.Id, ref ticketGuid);

        //    if (!isIdValid)
        //    {
        //        return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
        //    }

        //    var purchaseSuccess = await ticketService.BuyTicketsAsync(ticketGuid, model.QuantityToBuy);

        //    if (!purchaseSuccess)
        //    {
        //        ModelState.AddModelError(nameof(FailMessage), FailMessage);
        //        return View(model);
        //    }

        //    TempData[nameof(TicketPurchaseSuccess)] = TicketPurchaseSuccess;
        //    return RedirectToAction("Tickets", "Organization", new { organizationId = model.OrganizationId });
        //}

        private async Task SendTicketCreatedEmailAsync(string userId, TicketInputModel model)
        {
            try
            {
                string userEmail = User.FindFirstValue(ClaimTypes.Email);
                string userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? "User";
                string senderEmail = this.configuration["EmailSettings:SenderEmail"] ?? "noreply@gocetransport.com";
                string senderName = this.configuration["EmailSettings:SenderName"] ?? "GoceTransport";

                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    return;
                }

                string html = EmailTemplates.GetTicketConfirmationEmail(
                    recipientName: userName,
                    fromCity: "ï¿½",
                    toCity: "ï¿½",
                    departureDate: model.IssuedDate.ToString("dd MMM yyyy"),
                    departureTime: "ï¿½",
                    arrivalTime: "ï¿½",
                    organizationName: model.OrganizationId,
                    ticketId: "Newly created",
                    price: model.Price.ToString("F2"));

                await this.emailSender.SendEmailAsync(
                    senderEmail,
                    senderName,
                    userEmail,
                    "Ticket Successfully Issued ï¿½ GoceTransport",
                    html);
            }
            catch
            {
                // Email failure must never break the user flow
            }
        }

        private async Task SendCancellationEmailAsync(string userId, TicketDetailsViewModel ticket)
        {
            try
            {
                if (ticket == null)
                {
                    return;
                }

                string userEmail = User.FindFirstValue(ClaimTypes.Email);
                string userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? "User";
                string senderEmail = this.configuration["EmailSettings:SenderEmail"] ?? "noreply@gocetransport.com";
                string senderName = this.configuration["EmailSettings:SenderName"] ?? "GoceTransport";

                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    return;
                }

                string html = EmailTemplates.GetTicketCancellationEmail(
                    recipientName: userName,
                    fromCity: ticket.FromCity,
                    toCity: ticket.ToCity,
                    departureDate: ticket.IssuedDate,
                    organizationName: ticket.OrganizationName,
                    ticketId: ticket.Id);

                await this.emailSender.SendEmailAsync(
                    senderEmail,
                    senderName,
                    userEmail,
                    "Ticket Cancellation Confirmed ï¿½ GoceTransport",
                    html);
            }
            catch
            {
                // Email failure must never break the user flow
            }
        }
    }
}
