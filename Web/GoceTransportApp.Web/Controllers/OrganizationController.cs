using GoceTransportApp.Data.Models.Enumerations;
using GoceTransportApp.Services.Data.Analytics;
using GoceTransportApp.Services.Data.Notifications;
using GoceTransportApp.Services.Data.Organizations;
using GoceTransportApp.Services.Data.Reviews;
using GoceTransportApp.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using GoceTransportApp.Services.Data.Vehicles;
using GoceTransportApp.Web.ViewModels.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GoceTransportApp.Web.ViewModels.Organizations;

using static GoceTransportApp.Common.GlobalConstants;
using static GoceTransportApp.Common.GlobalConstants.SignalRMethods;
using static GoceTransportApp.Common.ResultMessages.GeneralMessages;
using static GoceTransportApp.Common.ResultMessages.OrganizationMessages;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GoceTransportApp.Web.ViewModels.Routes;
using GoceTransportApp.Web.ViewModels.Schedules;
using GoceTransportApp.Web.ViewModels.Tickets;
using GoceTransportApp.Web.ViewModels.Drivers;
using System.Net.Mail;
using GoceTransportApp.Services.Data.Routes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoceTransportApp.Web.Controllers
{
    [Authorize]
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService organizationService;
        private readonly IReviewService reviewService;
        private readonly INotificationService notificationService;
        private readonly IAnalyticsService analyticsService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<Organization> organizationRepository;
        private readonly IHubContext<NotificationHub> hubContext;

        public OrganizationController(
            IOrganizationService organizationService,
            IReviewService reviewService,
            INotificationService notificationService,
            IAnalyticsService analyticsService,
            IDeletableEntityRepository<Organization> organizationRepository,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
            : base(organizationRepository)
        {
            this.organizationService = organizationService;
            this.reviewService = reviewService;
            this.notificationService = notificationService;
            this.analyticsService = analyticsService;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            this.organizationRepository = organizationRepository;
            this.hubContext = hubContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(AllOrganizationsSearchFilterViewModel inputModel)
        {
            IEnumerable<OrganizationDataViewModel> allOrganizations = await organizationService.GetAllOrganizationsAsync(inputModel);

            int allOrganizationsCount = await organizationService.GetOrganizationsCountByFilterAsync(inputModel);

            AllOrganizationsSearchFilterViewModel viewModel = new AllOrganizationsSearchFilterViewModel
            {
                Organizations = allOrganizations,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allOrganizationsCount / inputModel.EntitiesPerPage.Value)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UserOrganizations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return this.RedirectToAction(nameof(Index));
            }

            var userOrganizations = await organizationService.GetUserOrganizationsAsync(userId);

            return View(userOrganizations);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Membership limit check
            var user = await userManager.FindByIdAsync(userId);
            int existingCount = await organizationRepository.AllAsNoTracking()
                .CountAsync(o => o.FounderId == userId);

            if (user != null && existingCount >= (int)user.MembershipTier)
            {
                TempData[TempDataKeys.UpgradeReason] = $"You have reached the limit of {(int)user.MembershipTier} organization(s) on the {user.MembershipTier} plan.";
                return RedirectToAction(nameof(Upgrade));
            }

            OrganizationInputModel model = new OrganizationInputModel();
            model.FounderId = userId;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Upgrade()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Analytics(string? id)
        {
            Guid orgGuid = Guid.Empty;
            if (!IsGuidValid(id, ref orgGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isOwner = await HasUserCreatedOrganizationAsync(userId, id);
            bool isAdmin = User.IsInRole(AdministratorRoleName);

            if (!isOwner && !isAdmin)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var currentUser = await userManager.FindByIdAsync(userId);
            var tier = currentUser?.MembershipTier ?? MembershipTier.Free;

            // Check if org is on trial → treat as Pro for analytics access
            bool onTrial = await IsOrgOnTrialAsync(id!);
            bool hasExtended = isAdmin || onTrial || tier >= MembershipTier.Starter;
            bool hasFull     = isAdmin || onTrial || tier >= MembershipTier.Pro;

            ViewBag.HasExtended = hasExtended;
            ViewBag.HasFull     = hasFull;
            ViewBag.Tier        = tier.ToString();

            var model = await analyticsService.GetOrganizationAnalyticsAsync(orgGuid);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAnalytics(string? id)
        {
            Guid orgGuid = Guid.Empty;
            if (!IsGuidValid(id, ref orgGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isOwner  = await HasUserCreatedOrganizationAsync(userId, id);
            bool isAdmin  = User.IsInRole(AdministratorRoleName);
            if (!isOwner && !isAdmin)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var currentUser = await userManager.FindByIdAsync(userId);
            var tier = currentUser?.MembershipTier ?? MembershipTier.Free;
            bool onTrial = await IsOrgOnTrialAsync(id!);
            if (!isAdmin && !onTrial && tier < MembershipTier.Pro)
            {
                TempData[TempDataKeys.UpgradeReason] = "CSV export is available on the Pro plan or higher.";
                return RedirectToAction(nameof(Upgrade));
            }

            var data = await analyticsService.GetOrganizationAnalyticsAsync(orgGuid);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Organization,\"{data.OrganizationName}\"");
            sb.AppendLine($"Total Routes,{data.TotalRoutes}");
            sb.AppendLine($"Total Schedules,{data.TotalSchedules}");
            sb.AppendLine($"Total Vehicles,{data.TotalVehicles}");
            sb.AppendLine($"Total Drivers,{data.TotalDrivers}");
            sb.AppendLine($"Total Tickets,{data.TotalTickets}");
            sb.AppendLine($"Total Revenue (EUR),{data.TotalRevenue:F2}");
            sb.AppendLine($"Boarded Passengers,{data.TotalBoardedPassengers}");
            sb.AppendLine($"Average Rating,{data.AverageRating:F1}");
            sb.AppendLine($"Total Reviews,{data.TotalReviews}");
            sb.AppendLine($"Followers,{data.TotalFollowers}");
            sb.AppendLine($"Total Route Distance (km),{data.TotalRouteDistanceKm}");
            sb.AppendLine($"Average Route Distance (km),{data.AverageRouteDistanceKm}");
            sb.AppendLine();
            sb.AppendLine("Top Routes");
            sb.AppendLine("Route,Tickets,Revenue (EUR)");
            foreach (var r in data.TopRoutes)
            {
                sb.AppendLine($"\"{r.RouteLabel}\",{r.TicketCount},{r.Revenue:F2}");
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            string filename = $"analytics-{data.OrganizationName.Replace(" ", "_")}-{DateTime.UtcNow:yyyyMMdd}.csv";
            return File(bytes, "text/csv", filename);
        }

        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Create(OrganizationInputModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            int existingCount = await organizationRepository.AllAsNoTracking()
                .CountAsync(o => o.FounderId == userId);

            if (user != null && existingCount >= (int)user.MembershipTier)
            {
                TempData[TempDataKeys.UpgradeReason] = $"You have reached the limit of {(int)user.MembershipTier} organization(s) on the {user.MembershipTier} plan.";
                return RedirectToAction(nameof(Upgrade));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imageUrl = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                if (model.Image.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.Image), ImageTooLarge);
                    return View(model);
                }

                string ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp")
                {
                    ModelState.AddModelError(nameof(model.Image), InvalidImageFormat);
                    return View(model);
                }

                string webRoot = string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : webHostEnvironment.WebRootPath;

                string uploadsFolder = Path.Combine(webRoot, "images", "organizations");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + ext;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }
                imageUrl = $"/images/organizations/{uniqueFileName}";
            }

            Guid newId = await organizationService.CreateAsync(model, imageUrl ?? "/images/no-organization-image.png");
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return RedirectToAction(nameof(Details), new { id = newId.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {

            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            EditOrganizationInputModel? formModel = await this.organizationService
                .GetOrganizationForEditAsync(organizationGuid);

            if (formModel == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            return this.View(formModel);
        }

        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Edit(EditOrganizationInputModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.Id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            string? imageUrl = formModel.ExistingImageUrl;
            if (formModel.Image != null && formModel.Image.Length > 0)
            {
                if (formModel.Image.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(formModel.Image), ImageTooLarge);
                    return this.View(formModel);
                }

                string ext = Path.GetExtension(formModel.Image.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp")
                {
                    ModelState.AddModelError(nameof(formModel.Image), InvalidImageFormat);
                    return this.View(formModel);
                }

                string webRoot = string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : webHostEnvironment.WebRootPath;

                string uploadsFolder = Path.Combine(webRoot, "images", "organizations");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + ext;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await formModel.Image.CopyToAsync(fileStream);
                }
                imageUrl = $"/images/organizations/{uniqueFileName}";
            }

            bool isUpdated = await this.organizationService
                .EditOrganizationAsync(formModel, imageUrl);

            if (!isUpdated)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return this.RedirectToAction(nameof(Details), new { id = formModel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            RemoveOrganizationViewModel? model = await organizationService
                .GetOrganizationForDeletionAsync(organizationGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;
                return this.RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RemoveOrganizationViewModel formModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await this.HasUserCreatedOrganizationAsync(userId, formModel.Id) && !User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return this.View(formModel);
            }

            bool isDeleted = await this.organizationService
                .RemoveOrganizationAsync(formModel);

            if (!isDeleted)
            {
                ModelState.AddModelError(nameof(FailMessage), FailMessage);

                return this.View(formModel);
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return this.RedirectToAction(nameof(UserOrganizations));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? id)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(id, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            OrganizationDetailsViewModel? model = await organizationService
                .GetOrganizationDetailsAsync(organizationGuid);

            if (model == null)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            model.Reviews = await this.reviewService.GetReviewsForOrganizationAsync(id!);
            model.AverageRating = await this.reviewService.GetAverageRatingAsync(id!);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                model.IsFavorite = await this.organizationService.IsOrganizationFavoriteAsync(userId, id!);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(string organizationId, int rating, string? comment)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool success = await this.reviewService.AddReviewAsync(userId, organizationId, rating, comment);

            if (!success)
            {
                TempData[TempDataKeys.ErrorMessage] = ReviewFail;
            }
            else
            {
                TempData[nameof(SuccessMessage)] = ReviewSuccess;

                var org = await this.organizationRepository.AllAsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id.ToString() == organizationId);
                if (org != null)
                {
                    await this.hubContext.Clients.User(org.FounderId)
                        .SendAsync(ReceiveNewReview, org.Name, rating);

                    await this.notificationService.CreateAsync(
                        org.FounderId,
                        string.Format(NewReviewNotification, org.Name),
                        $"/Organization/Details/{organizationId}",
                        org.FounderId);

                    await this.hubContext.Clients.User(org.FounderId)
                        .SendAsync(ReceiveNotification);
                }
            }

            return this.RedirectToAction(nameof(Details), new { id = organizationId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Routes(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var routes = await organizationService.GetRoutesByOrganizationId(organizationGuid);

            var viewModel = new RoutesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Routes = routes
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Drivers(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var drivers = await organizationService.GetDriversByOrganizationId(organizationGuid);

            var viewModel = new DriversForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Drivers = drivers
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Vehicles(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var vehicles = await organizationService.GetVehiclesByOrganizationId(organizationGuid);

            var viewModel = new VehiclesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Vehicles = vehicles
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Tickets(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var tickets = await organizationService.GetTicketsByOrganizationId(organizationGuid);
            var viewModel = new TicketsForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Tickets = tickets
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Schedules(string organizationId)
        {
            Guid organizationGuid = Guid.Empty;
            bool isIdValid = IsGuidValid(organizationId, ref organizationGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var schedules = await organizationService.GetSchedulesByOrganizationId(organizationGuid);
            var viewModel = new SchedulesForOrganizationViewModel
            {
                OrganizationId = organizationId,
                Schedules = schedules
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            bool isFavorited = await this.organizationService.ToggleFavoriteAsync(userId, id);

            if (isFavorited && Guid.TryParse(id, out Guid orgGuid))
            {
                var org = await this.organizationRepository.AllAsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == orgGuid);
                if (org != null)
                {
                    await this.hubContext.Clients.User(userId)
                        .SendAsync(ReceiveFavoriteAdded, org.Name);

                    await this.notificationService.CreateAsync(
                        userId,
                        string.Format(FavoriteAddedNotification, org.Name),
                        organizationFounderId: org.FounderId);
                    await this.hubContext.Clients.User(userId)
                        .SendAsync(ReceiveNotification);
                }
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorites = await organizationService.GetFavoriteOrganizationsByUserIdAsync(userId);

            return View(favorites);
        }
    }
}
