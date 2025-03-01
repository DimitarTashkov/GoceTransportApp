﻿using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Web.ViewModels.ContactForms;
using GoceTransportApp.Web.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IDeletableEntityRepository<Organization> organizationRepository;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IDeletableEntityRepository<Organization> organizationRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.organizationRepository = organizationRepository;
        }

        public async Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(AllUsersSearchFilterViewModel inputModel)
        {
            var query = userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                query = query.Where(x =>
                    x.UserName.Contains(inputModel.SearchQuery) ||
                    x.Email.Contains(inputModel.SearchQuery));
            }

            var totalItems = await query.CountAsync();
            IEnumerable<ApplicationUser> allUsers = await query
                .Skip((inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1)) * inputModel.EntitiesPerPage.Value)
                .Take(inputModel.EntitiesPerPage.Value)
                .Select(x => new ApplicationUser
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName,
                })
                .ToListAsync();

            ICollection<AllUsersViewModel> allUsersViewModel = new List<AllUsersViewModel>();

            foreach (ApplicationUser user in allUsers)
            {
                IEnumerable<string> roles = await this.userManager.GetRolesAsync(user);

                allUsersViewModel.Add(new AllUsersViewModel()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Username = user.UserName,
                    Roles = roles
                });
            }

            return allUsersViewModel;
        }

        public async Task<bool> UserExistsByIdAsync(Guid userId)
        {
            ApplicationUser? user = await this.userManager
                .FindByIdAsync(userId.ToString());

            return user != null;
        }

        public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());
            bool roleExists = await this.roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await this.userManager.IsInRoleAsync(user, roleName);
            if (!alreadyInRole)
            {
                IdentityResult? result = await this.userManager
                    .AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> RemoveUserRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());
            bool roleExists = await this.roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await this.userManager.IsInRoleAsync(user, roleName);
            if (alreadyInRole)
            {
                IdentityResult? result = await this.userManager
                    .RemoveFromRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            IdentityResult? result = await this.userManager
                .DeleteAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasUserCreatedOrganizationAsync(string userId, string organizationId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(organizationId))
            {
                return false;
            }

            return await organizationRepository.AllAsNoTracking()
                .AnyAsync(o => o.Id == Guid.Parse(organizationId) && o.FounderId == userId);
        }

        public async Task<int> GetUsersCountByFilterAsync(AllUsersSearchFilterViewModel inputModel)
        {
            IQueryable<ApplicationUser> allUsersQuery = userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allUsersQuery = allUsersQuery.Where(x =>
                     x.UserName.Contains(inputModel.SearchQuery) ||
                     x.Email.Contains(inputModel.SearchQuery));
            }

            return await allUsersQuery.CountAsync();
        }
    }
}
