using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Services.Data.ContactForms;
using GoceTransportApp.Web.ViewModels.ContactForms;
using Microsoft.EntityFrameworkCore;

namespace GoceTransportApp.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly IDeletableEntityRepository<ContactForm> contactFormRepository;

        public ContactFormService(IDeletableEntityRepository<ContactForm> contactFormRepository)
        {
            this.contactFormRepository = contactFormRepository;
        }

        public async Task CreateAsync(ContactFormInputModel model, string userId)
        {
            var contactForm = new ContactForm
            {
                UserId = userId,
                Email = model.Email,
                Title = model.Title,
                Message = model.Message,
                DateSubmitted = DateTime.UtcNow
            };

            await contactFormRepository.AddAsync(contactForm);
            await contactFormRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactFormDataViewModel>> GetAll()
        {
            return await contactFormRepository
                .AllAsNoTracking()
                .Select(x => new ContactFormDataViewModel
                {
                    Id = x.Id,
                    Username = x.User.UserName,
                    Title = x.Title,
                    DateSubmitted = x.DateSubmitted,
                })
                .ToListAsync();
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

        public async Task DeleteAsync(Guid id)
        {
            var contactForm = await contactFormRepository.GetByIdAsync(id);

            if (contactForm != null)
            {
                await contactFormRepository.DeleteAsync(contactForm);
                await contactFormRepository.SaveChangesAsync();
            }
        }
    }
}
