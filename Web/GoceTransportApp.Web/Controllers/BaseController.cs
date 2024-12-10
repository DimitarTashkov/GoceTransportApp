namespace GoceTransportApp.Web.Controllers
{
    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.Organizations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;

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
    }
}
