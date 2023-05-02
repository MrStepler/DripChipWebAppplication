using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Models.ResponseModels.Locations;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace DripChipWebApplication.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class LocationController : Controller
    {
        ILocationService locationService;
        IVisitedLocationService visitedLocationService;
        IAnimalsService animalsService;
        public LocationController(ILocationService locationService, IVisitedLocationService visitedLocationService, IAnimalsService animalsService) 
        {
            this.locationService = locationService;
            this.visitedLocationService = visitedLocationService;
            this.animalsService = animalsService;
        }
        [Route("locations/{pointId}")]
        [HttpGet] //Ready
        public ActionResult<AnimalLocation> GetLocation(long? pointId)
        {
            if (pointId == null || pointId <= 0)
            {
                return StatusCode(400);
            }
            if (locationService.GetLocation((long)pointId) == null)
            {
                return StatusCode(404);
            }
            return Ok(locationService.GetLocation((long)pointId));
        }
        [Route("locations")]
        [HttpPost]//Ready
        public ActionResult<AnimalLocation> AddLocation([FromBody] EditCreateLocation? location)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (location.Latitude == null || location.Longitude == null)
            {
                return StatusCode(400);
            }
            if (!IsValidLocation(location))
            {
                return StatusCode(400);
            }
            if (locationService.GetLocation(location) != null)
            {
                return StatusCode(409);
            }
            return Created("", locationService.AddLocation(location));
        }
        [Route("locations/{pointId}")]
        [HttpPut] //Ready
        public ActionResult<AnimalLocation> EditLocation(long? pointId, [FromBody] EditCreateLocation? location)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (pointId == null || pointId <= 0)
            {
                return StatusCode(400);
            }
            if (location.Latitude == null || location.Longitude == null)
            {
                return StatusCode(400);
            }
            if (!IsValidLocation(location))
            {
                return StatusCode(400);
            }
            if (locationService.GetLocation(location) != null)
            {
                return StatusCode(409);
            }
            if (locationService.GetLocation((long)pointId) == null)
            {
                return StatusCode(404);
            }

            return Ok(locationService.EditLocation((long)pointId, location));
        }
        [Route("locations/{pointId}")]
        [HttpDelete]
        public ActionResult<AnimalLocation> DeleteLocation(long? pointId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (pointId == null || pointId <= 0) 
            {
                return StatusCode(400);
            }
            if (locationService.GetLocation((long)pointId) == null)
            {
                return StatusCode(404);
            }
            if (visitedLocationService.GetListVisistedLocationsByPointId((long)pointId) != null)
            {
                return StatusCode(400);
            }
            if (animalsService.GetAnimalsByPointId((long)pointId) != null )
            {
                return StatusCode(400);
            }

            locationService.DeleteLocation((long)pointId);
            return Ok();
        }
        private bool IsValidLocation(EditCreateLocation location)
        {
            if (location.Latitude > 90 || location.Latitude < -90)
            {
                return false;
            }
            if (location.Longitude > 180 || location.Longitude < -180)
            {
                return false;
            }
            return true;
        }
    }
}
