using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class CitiesAddStreetInputModel
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
