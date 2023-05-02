using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Models.ResponseModels;
using DripChipWebApplication.Server.Models.ResponseModels.Animal;
using DripChipWebApplication.Server.Models.ResponseModels.AnimalType;
using DripChipWebApplication.Server.Models.ResponseModels.Locations;
using DripChipWebApplication.Server.Services;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static DripChipWebApplication.Server.Models.Animal;

namespace DripChipWebApplication.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class AnimalController : Controller
    {
        IAnimalsService animalService;
        IAnimalTypesService animalTypesService;
        IAccountService accountService;
        ILocationService locationService;
        IVisitedLocationService visitedLocationService;
        public AnimalController(IAnimalsService animalService, 
            IAnimalTypesService animalTypesService, 
            IAccountService accountService, 
            ILocationService locationService,
            IVisitedLocationService visitedLocationService)
        {
            this.animalService = animalService;
            this.animalTypesService = animalTypesService;
            this.accountService = accountService;
            this.locationService = locationService;
            this.visitedLocationService = visitedLocationService;
        }
        //=================================================================================================
        // ANIMAL BLOCK
        //=================================================================================================
        [Route("animals")]
        [HttpPost]
        public ActionResult<AnimalDTO> ChipAnimal(CreateAnimalDTO createdAnimal) //I mean ready
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (!VerifyAddingAnimal(createdAnimal))
            {
                return StatusCode(400);
            }
            if (accountService.GetAccount((int)createdAnimal.ChipperId) == null)
            {
                return StatusCode(404);
            }
            foreach (long typeId in createdAnimal.AnimalTypes)
            {
                if (animalTypesService.GetType(typeId) == null)
                {
                    return StatusCode(404);
                }
            }
            if (locationService.GetLocation((long)createdAnimal.ChippingLocationId) == null)
            {
                return StatusCode(404);
            }
            var duplicates = createdAnimal.AnimalTypes.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();        
            if (duplicates.Count() > 0)
            {
                return StatusCode(409);
            }
            AnimalDTO animalDTO = new AnimalDTO(animalService.ChipAnimal(createdAnimal));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs(animalDTO.Id);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId(animalDTO.Id); 
            return Created("", animalDTO);
        }
        [Route("animals/search")]
        [HttpGet]
        public ActionResult<Animal> SearchAnimal([FromQuery] TimeInterval interval, [FromQuery] int? chipperId, [FromQuery] long? chippingLocationId, [FromQuery] string? lifeStatus, [FromQuery] string? gender, [FromQuery] int? from = 0, [FromQuery] int? size = 10)
        {

            if (from < 0)
            {
                return StatusCode(400);
            }
            if (size <= 0)
            {
                return StatusCode(400);
            }
            if (from == null)
            {
                from = 0;
            }
            if (size == null)
            {
                size = 10;
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if (!VerifySearchinData(chipperId, chippingLocationId, gender, lifeStatus))
            {
                return StatusCode(400);
            }

            List<AnimalDTO> animals = new List<AnimalDTO>();
            if (animalService.SearchAnimal(interval.startDateTime, interval.endDateTime, chipperId, chippingLocationId, lifeStatus, gender, (int)from, (int)size) != null)
            {
                foreach (Animal animal in animalService.SearchAnimal(interval.startDateTime, interval.endDateTime, chipperId, chippingLocationId, lifeStatus, gender, (int)from, (int)size))
                {
                    AnimalDTO animalDTO = new AnimalDTO(animal);
                    animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs(animal.Id);
                    animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId(animal.Id);
                    animals.Add(animalDTO); 
                }
            }

            return Ok(animals);
        }
        [Route("animals/{animalId}")]
        [HttpPut] //READY
        public ActionResult<AnimalDTO> ChipAnimal(long? animalId, [FromBody] EditableAnimalDTO editableAnimal)
        {
            
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (!VerifyEditAnimal(editableAnimal))
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEAD && editableAnimal.LifeStatus == "ALIVE")
            {
                return StatusCode(400);
            }
            if (accountService.GetAccount((int)editableAnimal.ChipperId) == null)
            {
                return StatusCode(404);
            }
            if (locationService.GetLocation((long)editableAnimal.ChippingLocationId) == null)
            {
                return StatusCode(404);
            }
            if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId) != null)
            {
                if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId)[0].LocationPointId == editableAnimal.ChippingLocationId)
                {
                    return StatusCode(400);
                }
            }
            
            AnimalDTO animalDTO = new AnimalDTO(animalService.EditAnimal((long)animalId, editableAnimal));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs((long)animalId);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId((long)animalId);
            return Ok(animalDTO);
        }
        [Route("animals/{animalId}")]
        [HttpDelete] //READY
        public ActionResult<AnimalDTO> DeleteAnimal(long? animalId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId) != null)
            {
                if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId)[visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId).Count()-1].LocationPointId != animalService.GetAnimalById((long)animalId).ChippingLocationId)
                {
                    return StatusCode(400);
                }
            }
            animalService.ChipAwayAnimal((long)animalId);
            return Ok();
        }
        private bool VerifyEditAnimal(EditableAnimalDTO editableAnimal)
        {

            if (editableAnimal.Weight == null || editableAnimal.Weight <= 0)
            {
                return false;
            }
            if (editableAnimal.Length == null || editableAnimal.Length <= 0)
            {
                return false;
            }
            if (editableAnimal.Height == null || editableAnimal.Height <= 0)
            {
                return false;
            }
            if (editableAnimal.Gender == null || editableAnimal.Gender != "MALE" && editableAnimal.Gender != "FEMALE" && editableAnimal.Gender != "OTHER")
            {
                return false;
            }
            if (editableAnimal.LifeStatus == null || editableAnimal.LifeStatus != "ALIVE" && editableAnimal.LifeStatus != "DEAD" )
            {
                return false;
            }
            if (editableAnimal.ChipperId == null || editableAnimal.ChipperId <= 0)
            {
                return false;
            }
            if (editableAnimal.ChippingLocationId == null || editableAnimal.ChippingLocationId <= 0)
            {
                return false;
            }
            return true;
        }
        private bool VerifyAddingAnimal(CreateAnimalDTO createdAnimal)
        {
            if (createdAnimal.AnimalTypes == null || createdAnimal.AnimalTypes.Count() <= 0)
            {
                return false;
            }
            if (createdAnimal.AnimalTypes.Count() <= 0)
            {
                return false;
            }
            else
            {
                foreach (long idType in createdAnimal.AnimalTypes)
                {
                    if (idType == null || idType <= 0)
                    {
                        return false;
                    }
                }
            }
            if (createdAnimal.Weight == null || createdAnimal.Weight <= 0)
            {
                return false;
            }
            if (createdAnimal.Length == null || createdAnimal.Length <= 0)
            {
                return false;
            }
            if (createdAnimal.Height == null || createdAnimal.Height <= 0)
            {
                return false;
            }
            if (createdAnimal.Gender == null || (createdAnimal.Gender != "MALE" && createdAnimal.Gender != "FEMALE" && createdAnimal.Gender != "OTHER" ))
            {
                return false;
            }
            if (createdAnimal.ChipperId == null || createdAnimal.ChipperId <= 0)
            {
                return false;
            }
            if (createdAnimal.ChippingLocationId == null || createdAnimal.ChippingLocationId <= 0)
            {
                return false;
            }
            return true;
        }
        private bool VerifySearchinData(int? ChipperId, long? ChippingLocationId, string? Gender, string lifeStatus)
        {
            if (ChipperId != null)
            {
                if (ChipperId <= 0)
                {
                    return false;
                }
            }
            if (ChippingLocationId != null)
            {
                if (ChippingLocationId <= 0)
                {
                    return false;
                }
            }
            if (Gender != null)
            {
                if (Gender != "MALE" && Gender != "FEMALE" && Gender != "OTHER")
                {
                    return false;
                }
            }
            if (lifeStatus != null)
            {
                if (lifeStatus != "ALIVE" && lifeStatus != "DEAD")
                {
                    return false;
                }
            }

            return true;
        }

        [Route("animals/{animalId}")]
        [HttpGet]
        public ActionResult<AnimalDTO> GetAnimal(long? animalId)
        {
            if (animalId == null || animalId <= 0) 
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            AnimalDTO animalDTO = new AnimalDTO(animalService.GetAnimalById((long)animalId));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs((long)animalId);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId((long)animalId);
            return Ok(animalDTO);
           
        }
        //=================================================================================================
        // ANIMAL THAT HAVE TYPE BLOCK
        //=================================================================================================
        [Route("animals/{animalId}/types/{typeId}")]
        [HttpPost] //READY
        public ActionResult<AnimalDTO> AddTypeToAnimal(long? animalId, long? typeId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (typeId == null || typeId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)typeId) == null)
            {
                return StatusCode(404);
            }
            ////if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEATH)
            ////{
            ////    return StatusCode(400);
            ////}
            if (animalTypesService.GetListTypesOfAnimal((long)animalId) != null)
            {
                List<AnimalType> typesThatHaveAnimal = animalTypesService.GetListTypesOfAnimal((long)animalId);
                if (typesThatHaveAnimal.Any(x => x.Id == typeId))
                {
                    return StatusCode(409);
                }
            }

            AnimalDTO animalDTO = new AnimalDTO(animalTypesService.AddTypeToAnimal((long)animalId, (long)typeId));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs(animalDTO.Id);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId(animalDTO.Id);
            return Created("", animalDTO);
        }
        [Route("animals/{animalId}/types")]
        [HttpPut] //Ready
        public ActionResult<AnimalDTO> EditTypeOfAnimal(long? animalId, SwitchableTypes switchableTypes)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (switchableTypes.OldTypeId == null || switchableTypes.OldTypeId <= 0)
            {
                return StatusCode(400);
            }
            if (switchableTypes.NewTypeId == null || switchableTypes.NewTypeId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)switchableTypes.OldTypeId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)switchableTypes.NewTypeId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)switchableTypes.OldTypeId).AnimalId != animalId)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)switchableTypes.NewTypeId).AnimalId != null)
            {
                return StatusCode(409);
            }
            ////if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEATH)
            ////{
            ////    return StatusCode(400);
            ////}

            AnimalDTO animalDTO = new AnimalDTO(animalTypesService.EditTypeOfAnimal((long)animalId, switchableTypes));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs(animalDTO.Id);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId(animalDTO.Id);
            return Ok(animalDTO);
        }
        [Route("animals/{animalId}/types/{typeId}")]
        [HttpDelete] //Ready
        public ActionResult<AnimalDTO> EditTypeOfAnimal(long? animalId, long? typeId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (typeId == null || typeId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)typeId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType((long)typeId).AnimalId != animalId)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetListTypesOfAnimal((long)animalId).Count() <= 1)
            {
                return StatusCode(400);
            }
            ////if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEATH)
            ////{
            ////    return StatusCode(400);
            ////}
            
            AnimalDTO animalDTO = new AnimalDTO(animalTypesService.DeleteTypeOfAnimal((long)animalId, (long)typeId));
            animalDTO.VisitedLocations = visitedLocationService.GetVisitedLocationsIDs(animalDTO.Id);
            animalDTO.AnimalTypes = animalTypesService.GetTypesByAnimalId(animalDTO.Id);
            return Ok(animalDTO);
        }
        //=================================================================================================
        // ANIMAL TYPE BLOCK
        //=================================================================================================

        [Route("animals/types")]
        [HttpPost]
        public ActionResult<AnimalTypeDTO> AddAnimalType([FromBody] AddableType addableType) 
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }

            if (string.IsNullOrWhiteSpace(addableType.type))
            {
                return StatusCode(400);
            }
            if (animalTypesService.GetType(addableType.type) != null)
            {
                return StatusCode(409);
            }
            AnimalTypeDTO animalTypeDTO = new AnimalTypeDTO(animalTypesService.AddType(addableType.type));
            return Created("", animalTypeDTO);

        }
        [Route("animals/types/{typeId}")]
        [HttpGet] //READY
        public ActionResult<AnimalTypeDTO> GetAnimalType(long? typeId)
        {
            if (typeId == null || typeId <= 0)
            {
                return StatusCode(400);
            }
            if (animalTypesService.GetType((long)typeId) == null)
            {
                return StatusCode(404);
            }
            AnimalTypeDTO animalTypeDTO = new AnimalTypeDTO(animalTypesService.GetType((long)typeId));
            return Ok(animalTypeDTO);

        }

        [Route("animals/types/{typeId}")]
        [HttpPut]
        public ActionResult<AnimalTypeDTO> EditAnimalType(long? typeId,[FromBody] AddableType addableType) 
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (typeId == null || typeId <= 0)
            {
                return StatusCode(400);
            }
            if (string.IsNullOrWhiteSpace(addableType.type))
            {
                return StatusCode(400);
            }
            if (animalTypesService.GetType((long)typeId) == null)
            {
                return StatusCode(404);
            }
            if (animalTypesService.GetType(addableType.type) != null)
            {
                return StatusCode(409);
            }
            AnimalTypeDTO animalTypeDTO = new AnimalTypeDTO(animalTypesService.EditType((long)typeId, addableType.type));
            return Ok(animalTypeDTO);

        }
        [Route("animals/types/{typeId}")]
        [HttpDelete]
        public ActionResult<AnimalType> DeleteAnimalType(long? typeId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (typeId == null || typeId <= 0)
            {
                return StatusCode(400);
            }
            if (animalTypesService.GetType((long)typeId) == null)
            {
                return StatusCode(404);
            }
            if (animalService.ExistAnimalWithType((long)typeId)) 
            {
                return StatusCode(400);
            }
            animalTypesService.DeleteType((long)typeId);
            return Ok();

        }
        //=================================================================================================
        // VISITED LOCATION BLOCK
        //=================================================================================================
        [Route("animals/{animalId}/locations")]
        [HttpGet] //READY
        public ActionResult<VisitedLocationDTO> GetVistedLocations(long? animalId, [FromQuery] TimeInterval? timeInterval, [FromQuery] int? from = 0, [FromQuery] int? size = 10)
        {
            
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (from <0)
            {
                return StatusCode(400);
            }
            if (size <= 0)
            {
                return StatusCode(400);
            }
            if (from == null)
            {
                from = 0;
            }
            if (size == null)
            {
                size = 10;
            }
            List<VisitedLocationDTO> visitedLocationDTOs = new List<VisitedLocationDTO>();
            if (animalService.GetVisitedLocation((long)animalId, timeInterval.startDateTime, timeInterval.endDateTime, (int)from, (int)size) != null)
            {
                foreach (AnimalVisitedLocation visitedLocation in animalService.GetVisitedLocation((long)animalId, timeInterval.startDateTime, timeInterval.endDateTime, (int)from, (int)size))
                {
                    VisitedLocationDTO visitedLocationDTO = new VisitedLocationDTO(visitedLocation);
                    visitedLocationDTOs.Add(visitedLocationDTO);
                }
            }
            return Ok(visitedLocationDTOs);
        }
        [Route("animals/{animalId}/locations/{pointId}")]
        [HttpPost] //Ready
        public ActionResult<VisitedLocationDTO> AddVisitedLocation(long? animalId, long? pointId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (pointId == null || pointId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (locationService.GetLocation((long)pointId) == null)
            {
                return StatusCode(404);
            }
            if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEAD)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId).ChippingLocationId == pointId && visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId) == null)
            {
                return StatusCode(400);
            }
            if (visitedLocationService.GetVisitedLocationsIDs((long)animalId) != null)
            {
                foreach (long visitedId in visitedLocationService.GetVisitedLocationsIDs((long)animalId))
                {
                    if (pointId == visitedLocationService.GetVisitedLocationsById(visitedId).LocationPointId)
                    {
                        return StatusCode(400);
                    }
                }
            }
            
            
            VisitedLocationDTO visitedLocationDTO = new VisitedLocationDTO(visitedLocationService.VisitLocation((long)animalId, (long)pointId));
            return Created("",visitedLocationDTO);
        }
        [Route("animals/{animalId}/locations")]
        [HttpPut]//READY
        public ActionResult<VisitedLocationDTO> EditVisitedLocation(long? animalId, [FromBody] VisitedLocationAndLocationId visitedLocationAndLocationId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (visitedLocationAndLocationId.LocationPointId == null || visitedLocationAndLocationId.LocationPointId <= 0)
            {
                return StatusCode(400);
            }
            if (visitedLocationAndLocationId.VisitedLocationPointId == null || visitedLocationAndLocationId.VisitedLocationPointId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (locationService.GetLocation((long)visitedLocationAndLocationId.LocationPointId) == null)
            {
                return StatusCode(404);
            }
            //if (animalService.GetAnimalById((long)animalId).LifeStatus == lifeStatus.DEATH)
            //{
            //    return StatusCode(400);
            //}
            if (!ThisIndexesLocationsExist(animalId, visitedLocationAndLocationId.VisitedLocationPointId, visitedLocationAndLocationId.LocationPointId))
            {
                return StatusCode(404);
            }
            foreach (long visitedId in visitedLocationService.GetVisitedLocationsIDs((long)animalId))
            {
                if (visitedLocationAndLocationId.LocationPointId == visitedLocationService.GetVisitedLocationsById(visitedId).LocationPointId)
                {
                    return StatusCode(400);
                }
            }
            if (animalService.GetAnimalById((long)animalId).ChippingLocationId == visitedLocationAndLocationId.LocationPointId && visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId)[0].Id == visitedLocationAndLocationId.VisitedLocationPointId)
            {
                return StatusCode(400);
            }

            VisitedLocationDTO visitedLocationDTO = new VisitedLocationDTO(visitedLocationService.EditVisitedLocation((long)visitedLocationAndLocationId.VisitedLocationPointId, (long)visitedLocationAndLocationId.LocationPointId));
            return Ok(visitedLocationDTO);
        }
        [Route("animals/{animalId}/locations/{visitedPointId}")]
        [HttpDelete] //READY
        public ActionResult<VisitedLocationDTO> DeleteVisitedLocation(long? animalId, long? visitedPointId)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (animalId == null || animalId <= 0)
            {
                return StatusCode(400);
            }
            if (visitedPointId == null || visitedPointId <= 0)
            {
                return StatusCode(400);
            }
            if (animalService.GetAnimalById((long)animalId) == null)
            {
                return StatusCode(404);
            }
            if (!ThisIndexesLocationsExist(animalId, visitedPointId, null))
            {
                return StatusCode(404);
            }
            visitedLocationService.DeleteVisitedLocation((long)animalId, (long)visitedPointId);
            if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId) != null)
            {
                if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId)[0].LocationPointId == animalService.GetAnimalById((long)animalId).ChippingLocationId)
                {
                    visitedLocationService.DeleteVisitedLocation((long)animalId, visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId)[0].Id);
                }
            }
            return Ok();
        }
        private bool ThisIndexesLocationsExist(long? animalId, long? visitedLocationPointId , long? locationPointId)
        {
            if (visitedLocationService.GetVisitedLocationsIDs((long)animalId) == null)
            {
                return false;
            }
            if (visitedLocationService.GetListVisistedLocationsOfAnimal((long)animalId).Where(x => x.Id == visitedLocationPointId) == null)
            {
                return false;
            }
            if (visitedLocationService.GetVisitedLocationsById((long)visitedLocationPointId) == null)
            {
                return false;
            }
            if (locationPointId != null)
            {
                if (locationService.GetLocation((long)locationPointId) == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
