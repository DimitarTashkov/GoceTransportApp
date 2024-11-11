using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.StreetConstants;

namespace GoceTransportApp.Web.ViewModels.Streets
{
    public class StreetsCheckBoxItemInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(MinStreetLength)]
        [MaxLength(MaxStreetLength)]
        public string Name { get; set; } = null!;

        public bool IsSelected { get; set; }
    }
}
