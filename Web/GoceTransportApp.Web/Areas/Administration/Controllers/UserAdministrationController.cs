namespace GoceTransportApp.Web.Areas.Administration.Controllers
{
    using GoceTransportApp.Common;
    using GoceTransportApp.Services.Data.Users;
    using GoceTransportApp.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using GoceTransportApp.Web.ViewModels.Users;
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Web.ViewModels.ContactForms;
    using GoceTransportApp.Services;
    using static GoceTransportApp.Common.ResultMessages.GeneralMessages;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministratorArea)]
    public class UserAdministrationController : BaseController
    {
        private readonly IUserService userService;

        public UserAdministrationController(IUserService userService, IDeletableEntityRepository<Organization> organizationRepository)
            : base(organizationRepository)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllUsersSearchFilterViewModel inputModel)
        {
            IEnumerable<AllUsersViewModel> allUsers = await userService.GetAllUsersAsync(inputModel);

            int allRoutesCount = await userService.GetUsersCountByFilterAsync(inputModel);

            AllUsersSearchFilterViewModel viewModel = new AllUsersSearchFilterViewModel
            {
                Users = allUsers,
                SearchQuery = inputModel.SearchQuery,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)allRoutesCount / inputModel.EntitiesPerPage.Value)
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;
            if (!this.IsGuidValid(userId, ref userGuid))
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool userExists = await this.userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            bool assignResult = await this.userService
                .AssignUserToRoleAsync(userGuid, role);
            if (!assignResult)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return this.RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;
            if (!this.IsGuidValid(userId, ref userGuid))
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool userExists = await this.userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            bool removeResult = await this.userService
                .RemoveUserRoleAsync(userGuid, role);
            if (!removeResult)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }
            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return this.RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            Guid userGuid = Guid.Empty;
            if (!this.IsGuidValid(userId, ref userGuid))
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool userExists = await this.userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            bool removeResult = await this.userService
                .DeleteUserAsync(userGuid);
            if (!removeResult)
            {
                TempData[nameof(FailMessage)] = FailMessage;

                return this.RedirectToAction(nameof(Index));
            }

            TempData[nameof(SuccessMessage)] = SuccessMessage;

            return this.RedirectToAction(nameof(Index));
        }
    }
}
