using GoceTransportApp.Web.ViewModels.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Web.ViewModels.Cities
{
    public class CitiesDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string State { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        public IEnumerable<StreetsDataViewModel> Streets { get; set; }
        = new List<StreetsDataViewModel>();
    }
}
