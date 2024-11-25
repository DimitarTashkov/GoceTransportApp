using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Streets;
using GoceTransportApp.Web.ViewModels.Streets;
using Microsoft.AspNetCore.Mvc;

namespace GoceTransportApp.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreetsController : ControllerBase
    {
        private readonly ICityService cityService;

        public StreetsController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        [HttpGet("GetStreetsByCity/{cityId?}")]
        [ProducesResponseType(typeof(IEnumerable<StreetDataViewModel>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStreetsByCity(string cityId)
        {
            if (string.IsNullOrEmpty(cityId))
            {
                return BadRequest("City ID cannot be null or empty.");
            }

            Guid cityGuid = Guid.Empty;
            if (!this.IsGuidValid(cityId, ref cityGuid))
            {
                return this.BadRequest();
            }

            IEnumerable<StreetDataViewModel> streets = await cityService.GetAllStreetsInCity(cityGuid);
            return Ok(streets);
        }
        protected bool IsGuidValid(string? id, ref Guid parsedGuid)
        {
            // Non-existing parameter in the URL
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            // Invalid parameter in the URL
            bool isGuidValid = Guid.TryParse(id, out parsedGuid);
            if (!isGuidValid)
            {
                return false;
            }

            return true;
        }
    }

}
