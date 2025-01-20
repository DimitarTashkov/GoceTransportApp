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
    public class CityInputModel
    {
        [Required(ErrorMessage = CityNameRequiredMessage)]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = CityStateRequiredMessage)]
        [MaxLength(MaxStateLength)]
        [MinLength(MinStateLength)]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = CityZipRequiredMessage)]
        [MinLength(MinZipCodeLength)]
        [MaxLength(MaxZipCodeLength)]
        public string ZipCode { get; set; } = null!;
    }
}
