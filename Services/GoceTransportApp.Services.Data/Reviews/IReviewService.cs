namespace GoceTransportApp.Services.Data.Reviews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using GoceTransportApp.Web.ViewModels.Organizations;

    public interface IReviewService
    {
        Task<bool> AddReviewAsync(string userId, string organizationId, int rating, string? comment);

        Task<IEnumerable<ReviewViewModel>> GetReviewsForOrganizationAsync(string organizationId);

        Task<double> GetAverageRatingAsync(string organizationId);
    }
}
