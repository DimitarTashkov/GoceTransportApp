namespace GoceTransportApp.Web.Controllers
{
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Data.Models.Enumerations;
    using GoceTransportApp.Services.Data.Organizations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static GoceTransportApp.Common.GlobalConstants;

    public class BaseController : Controller
    {
        private readonly IDeletableEntityRepository<Organization> organizationRepository;

        public BaseController(IDeletableEntityRepository<Organization> organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        protected bool IsGuidValid(string? id, ref Guid parsedGuid)
        {
            // Non-existing parameter in the URL
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            // Invalid parameter in the URL
            bool isGuidValid = Guid.TryParse(id, out parsedGuid);
            if (!isGuidValid)
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

        protected async Task<List<Guid>> GetUserOrganizationIdsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Guid>();
            }

            return await organizationRepository.AllAsNoTracking()
                .Where(o => o.FounderId == userId)
                .Select(o => o.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Returns the route/schedule limit for the given tier, accounting for
        /// an active Pro trial on the target organization (trial = unlimited).
        /// </summary>
        protected async Task<int> GetEffectiveRouteLimitAsync(MembershipTier tier, string organizationId)
        {
            if (tier == MembershipTier.Pro || tier == MembershipTier.Enterprise)
            {
                return int.MaxValue;
            }

            if (await IsOrgOnTrialAsync(organizationId))
            {
                return int.MaxValue;
            }

            return tier == MembershipTier.Starter ? PlanLimits.StarterRoutes : PlanLimits.FreeRoutes;
        }

        protected async Task<int> GetEffectiveScheduleLimitAsync(MembershipTier tier, string organizationId)
        {
            if (tier == MembershipTier.Pro || tier == MembershipTier.Enterprise)
            {
                return int.MaxValue;
            }

            if (await IsOrgOnTrialAsync(organizationId))
            {
                return int.MaxValue;
            }

            return tier == MembershipTier.Starter ? PlanLimits.StarterSchedules : PlanLimits.FreeSchedules;
        }

        protected async Task<bool> IsOrgOnTrialAsync(string organizationId)
        {
            if (!Guid.TryParse(organizationId, out Guid orgGuid))
            {
                return false;
            }

            return await organizationRepository.AllAsNoTracking()
                .AnyAsync(o => o.Id == orgGuid && o.IsOnTrial && o.TrialExpiresOn > DateTime.UtcNow);
        }
    }
}
