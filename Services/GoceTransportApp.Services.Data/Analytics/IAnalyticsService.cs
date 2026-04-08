namespace GoceTransportApp.Services.Data.Analytics
{
    using System;
    using System.Threading.Tasks;
    using GoceTransportApp.Web.ViewModels.Analytics;

    public interface IAnalyticsService
    {
        Task<OrganizationAnalyticsViewModel> GetOrganizationAnalyticsAsync(Guid organizationId);
    }
}
