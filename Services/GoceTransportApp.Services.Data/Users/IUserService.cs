using GoceTransportApp.Web.ViewModels.ContactForms;
using GoceTransportApp.Web.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.Users
{
    public interface IUserService
    {
        Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(AllUsersSearchFilterViewModel inputModel);

        Task<bool> UserExistsByIdAsync(Guid userId);

        Task<bool> AssignUserToRoleAsync(Guid userId, string roleName);

        Task<bool> RemoveUserRoleAsync(Guid userId, string roleName);

        Task<bool> DeleteUserAsync(Guid userId);

        Task<bool> HasUserCreatedOrganizationAsync(string userId, string organizationId);

        public Task<int> GetUsersCountByFilterAsync(AllUsersSearchFilterViewModel inputModel);

    }
}
