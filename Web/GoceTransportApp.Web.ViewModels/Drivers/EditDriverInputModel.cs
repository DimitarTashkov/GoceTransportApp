using GoceTransportApp.Data.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.DriverConstants;
using static GoceTransportApp.Common.ResultMessages.DriverMessages;

namespace GoceTransportApp.Web.ViewModels.Drivers
{
    public class EditDriverInputModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = DriverFirstNameRequired)]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = DriverLastNameRequired)]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; } = null!;

        public int? Age { get; set; }

        [Required(ErrorMessage = DriverExperienceRequired)]
        public DriverExperience DrivingExperience { get; set; }

        [Required]
        public string OrganizationId { get; set; } = null!;
    }
}
