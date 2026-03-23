namespace GoceTransportApp.Services.Data.Reviews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GoceTransportApp.Data.Common.Repositories;
    using GoceTransportApp.Data.Models;
    using GoceTransportApp.Services.Data.Base;
    using GoceTransportApp.Web.ViewModels.Organizations;

    using Microsoft.EntityFrameworkCore;

    public class ReviewService : BaseService, IReviewService
    {
        private readonly IRepository<Review> reviewRepository;
        private readonly IRepository<UserTicket> userTicketRepository;

        public ReviewService(
            IRepository<Review> reviewRepository,
            IRepository<UserTicket> userTicketRepository)
        {
            this.reviewRepository = reviewRepository;
            this.userTicketRepository = userTicketRepository;
        }

        public async Task<bool> AddReviewAsync(string userId, string organizationId, int rating, string? comment)
        {
            if (!Guid.TryParse(organizationId, out Guid orgGuid))
            {
                return false;
            }

            bool hasTraveled = await this.userTicketRepository
                .GetAllAttached()
                .Include(ut => ut.Ticket)
                .AnyAsync(ut => ut.CustomerId == userId
                             && ut.Ticket.OrganizationId == orgGuid
                             && ut.Ticket.ExpiryDate < DateTime.UtcNow);

            if (!hasTraveled)
            {
                return false;
            }

            bool alreadyReviewed = await this.reviewRepository
                .GetAllAttached()
                .AnyAsync(r => r.PassengerId == userId && r.OrganizationId == orgGuid);

            if (alreadyReviewed)
            {
                return false;
            }

            var review = new Review
            {
                OrganizationId = orgGuid,
                PassengerId = userId,
                Rating = rating,
                Comment = comment,
            };

            await this.reviewRepository.AddAsync(review);
            await this.reviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ReviewViewModel>> GetReviewsForOrganizationAsync(string organizationId)
        {
            if (!Guid.TryParse(organizationId, out Guid orgGuid))
            {
                return Enumerable.Empty<ReviewViewModel>();
            }

            return await this.reviewRepository
                .GetAllAttached()
                .Where(r => r.OrganizationId == orgGuid)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new ReviewViewModel
                {
                    PassengerName = r.Passenger.UserName ?? "Anonymous",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedOn = r.CreatedOn,
                })
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(string organizationId)
        {
            if (!Guid.TryParse(organizationId, out Guid orgGuid))
            {
                return 0;
            }

            var ratings = await this.reviewRepository
                .GetAllAttached()
                .Where(r => r.OrganizationId == orgGuid)
                .Select(r => r.Rating)
                .ToListAsync();

            if (!ratings.Any())
            {
                return 0;
            }

            return Math.Round(ratings.Average(), 1);
        }
    }
}
