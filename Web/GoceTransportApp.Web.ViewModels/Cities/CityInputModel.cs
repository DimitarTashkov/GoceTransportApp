using System.ComponentModel.DataAnnotations;

using static GoceTransportApp.Common.EntityValidationConstants.CityConstants;

namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class CityInputModel
    {
        [Required(ErrorMessage = "CityNameRequired")]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength, ErrorMessage = "StringLengthField")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "CityStateRequired")]
        [StringLength(MaxStateLength, MinimumLength = MinStateLength, ErrorMessage = "StringLengthField")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "CityZipRequired")]
        [StringLength(MaxZipCodeLength, MinimumLength = MinZipCodeLength, ErrorMessage = "StringLengthField")]
        public string ZipCode { get; set; } = null!;
    }
}
