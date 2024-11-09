﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GoceTransportApp.Common.EntityValidationConstants.StreetConstants;
using static GoceTransportApp.Common.ErrorMessages.StreetsMessages;

namespace GoceTransportApp.Web.ViewModels.Streets
{
    public class StreetsInputModel
    {
        [Required(ErrorMessage = StreetNameRequiredMessage)]
        [MinLength(MinStreetLength)]
        [MaxLength(MaxStreetLength)]
        public string Street { get; set; }
    }
}
