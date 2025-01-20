using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.ContactForms;
using GoceTransportApp.Web.ViewModels.ContactForms;
using GoceTransportApp.Web.ViewModels.Routes;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GoceTransportApp.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly IDeletableEntityRepository<ContactForm> contactFormRepository;

        public ContactFormService(IDeletableEntityRepository<ContactForm> contactFormRepository)
        {
            this.contactFormRepository = contactFormRepository;
        }

        public async Task CreateAsync(ContactFormInputModel model)
        {
            var contactForm = new ContactForm
            {
                UserId = model.UserId,
                Email = model.Email,
                Title = model.Title,
                Message = model.Message,
                DateSubmitted = DateTime.UtcNow
            };

            await contactFormRepository.AddAsync(contactForm);
            await contactFormRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactFormDataViewModel>> GetAllFormsAsync(AllFormsSearchFilterViewModel inputModel)
        {
            var query = contactFormRepository.AllAsNoTracking();

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                query = query.Where(x =>
                    x.Title.Contains(inputModel.SearchQuery) ||
                    x.Message.Contains(inputModel.SearchQuery) ||
                    x.User.UserName.Contains(inputModel.SearchQuery) ||
                    x.Email.Contains(inputModel.SearchQuery));
            }

            var totalItems = await query.CountAsync();

            var forms = await query
                .OrderByDescending(x => x.DateSubmitted)
                .Skip((inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1)) * inputModel.EntitiesPerPage.Value)
                .Take(inputModel.EntitiesPerPage.Value)
                .Select(x => new ContactFormDataViewModel
                {
                    Id = x.Id,
                    Username = x.User.UserName,
                    Title = x.Title,
                    DateSubmitted = x.DateSubmitted,
                })
                .ToListAsync();

            return forms;
        }

        public async Task<ContactFormDeleteViewModel> GetByIdAsync(Guid id)
        {
            var contactForm = await contactFormRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ContactFormDeleteViewModel
                {
                    Id = x.Id,
                    Username = x.User.UserName,
                    Email = x.Email,
                    Title = x.Title,
                })
                .FirstOrDefaultAsync();

            return contactForm;
        }

        public async Task DeleteFormAsync(Guid id)
        {
            var contactForm = await contactFormRepository.GetByIdAsync(id);

            if (contactForm != null)
            {
                await contactFormRepository.DeleteAsync(contactForm);
                await contactFormRepository.SaveChangesAsync();
            }
        }

        public async Task<ContactFormDetailsViewModel> GetFormDetailsByIdAsync(Guid id)
        {
            var contactForm = await contactFormRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ContactFormDetailsViewModel
                {
                    Id = x.Id,
                    Username = x.User.UserName, 
                    Email = x.Email,
                    Title = x.Title,
                    Message = x.Message,
                    DateSubmitted = x.DateSubmitted,
                })
                .FirstOrDefaultAsync();

            return contactForm;
        }

        public async Task<int> GetFormsCountByFilterAsync(AllFormsSearchFilterViewModel inputModel)
        {
            IQueryable<ContactForm> allFormsQuery = contactFormRepository
                .AllAsNoTracking();

            if (!string.IsNullOrEmpty(inputModel.SearchQuery))
            {
                allFormsQuery = allFormsQuery.Where(x =>
                     x.Title.Contains(inputModel.SearchQuery) ||
                     x.Message.Contains(inputModel.SearchQuery) ||
                     x.User.UserName.Contains(inputModel.SearchQuery) ||
                     x.Email.Contains(inputModel.SearchQuery));
            }



            return await allFormsQuery.CountAsync();
        }
    }
}
