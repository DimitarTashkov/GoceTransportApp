using GoceTransportApp.Web.ViewModels.ContactForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Services.Data.ContactForms
{
    public interface IContactFormService
    {
        Task CreateAsync(ContactFormInputModel model, string userId);

        Task <IEnumerable<ContactFormDataViewModel>> GetAllFormsAsync();

        Task<ContactFormDeleteViewModel> GetByIdAsync(Guid id);

        Task DeleteFormAsync(Guid id);

        Task<ContactFormDetailsViewModel> GetFormDetailsByIdAsync(Guid id);

    }
}
