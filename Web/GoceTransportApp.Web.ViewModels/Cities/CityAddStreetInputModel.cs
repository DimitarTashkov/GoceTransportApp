using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.CityConstants;
using static GoceTransportApp.Common.ResultMessages.CityMessages;


namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class CityAddStreetInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = CityNameRequiredMessage)]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        public List<StreetCheckBoxItemInputModel> Streets { get; set; }
        = new List<StreetCheckBoxItemInputModel>();
    }
}
