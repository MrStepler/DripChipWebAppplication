using DripChipWebApplication.Server.Data;
using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Models.ResponseModels.Animal;
using DripChipWebApplication.Server.Models.ResponseModels.Locations;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Globalization;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DripChipWebApplication.Server.Services
{
    public class AnimalsService : IAnimalsService
    {
        IDbContextFactory<APIDbContext> contextFactory;
        IAnimalTypesService animalTypesService;
        IVisitedLocationService visitedLocationService;
        public AnimalsService(IDbContextFactory<APIDbContext> contextFactory, IAnimalTypesService animalTypesService, IVisitedLocationService visitedLocationService) 
        {
            this.contextFactory = contextFactory;
            this.animalTypesService= animalTypesService;
            this.visitedLocationService = visitedLocationService;
        }
        public Animal? GetAnimalById(long id)
        {
            var dbContext = contextFactory.CreateDbContext();
            return dbContext.Animals.FirstOrDefault(a => a.Id == id);
        }

        public Animal[]? SearchAnimal(DateTime? startDateTime, DateTime? endDateTime, int? chipperId, long? chippingLocationId, string? lifeStatus, string? gender, int from, int size)
        {
            var dbContext = contextFactory.CreateDbContext();
            var searchedAnimals = dbContext.Animals.AsQueryable();
            if (startDateTime != null)
            {
                 searchedAnimals = searchedAnimals.Where(x => DateTime.Parse(x.ChippingDateTime,CultureInfo.InvariantCulture) > startDateTime);
            }
            if (endDateTime != null)
            {
                searchedAnimals = searchedAnimals.Where(x => DateTime.Parse(x.ChippingDateTime, CultureInfo.InvariantCulture) < endDateTime);
            }
            if (chipperId != null)
            {
                searchedAnimals = searchedAnimals.Where(x => x.ChipperId == chipperId);
            }
            if (chippingLocationId != null)
            {
                searchedAnimals = searchedAnimals.Where(x => x.ChippingLocationId == chippingLocationId);
            }
            if (lifeStatus != null)
            {
                searchedAnimals = searchedAnimals.Where(x => x.LifeStatus == LifeStatusConverter(lifeStatus));
            }
            if (gender != null)
            {
                searchedAnimals = searchedAnimals.Where(x => x.Gender == GenderConverter(gender));
            }
            if (searchedAnimals.Count() == 0)
            {
                return null;
            }
            
            return searchedAnimals.OrderBy(x => x.Id).Skip(from).Take(size).ToArray();
        }
        public AnimalVisitedLocation[]? GetVisitedLocation(long animalId, DateTime? startDateTime, DateTime? endDateTime, int from, int size)
        {
            var dbContext = contextFactory.CreateDbContext();
            var LocationIndexes = dbContext.VisitedLocations.Where(x => x.Animal.Id == animalId);
            if (startDateTime != null)
            {
                LocationIndexes = LocationIndexes.Where(x => DateTime.Parse(x.DateTimeOfVisitLocationPoint, CultureInfo.InvariantCulture) > startDateTime);
            }
            if (endDateTime != null)
            {
                LocationIndexes = LocationIndexes.Where(x => DateTime.Parse(x.DateTimeOfVisitLocationPoint, CultureInfo.InvariantCulture) < endDateTime);
            }
            return LocationIndexes.Skip(from).Take(size).ToArray();

        }

        public bool ExistAnimalWithType(long typeId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var animalsWithType = dbContext.AnimalTypes.Where(x => x.Id == typeId).Where(x =>x.Animal != null);

            if (animalsWithType.Count() == 0)
            {
                return false;
            }
            return true;
        }
        public List<Animal>? GetAnimalsByPointId(long pointId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (dbContext.Animals.Any(x => x.ChippingLocationId == pointId))
            {
                return null;
            }
            return dbContext.Animals.Where(x=>x.ChippingLocationId == pointId).ToList();
        }

        public Animal ChipAnimal(CreateAnimalDTO createdAnimal) // MAYBE READY
        {
            using var dbContext = contextFactory.CreateDbContext();
            Animal animal = new Animal();
            animal.Weight = (float)createdAnimal.Weight;
            animal.Height = (float)createdAnimal.Height;
            animal.Length = (float)createdAnimal.Length;
            animal.Gender = GenderConverter(createdAnimal.Gender);
            animal.ChipperId = (int)createdAnimal.ChipperId;
            animal.ChippingLocationId = (long)createdAnimal.ChippingLocationId;
            animal.LifeStatus = Animal.lifeStatus.ALIVE;
            animal.ChippingDateTime = DateTime.Now.ToString("O");
            animal.DeathDateTime = null;
            animal.VisitedLocations = new List<AnimalVisitedLocation>();
            foreach (long typeAnimalInd in createdAnimal.AnimalTypes)
            {
                var typeAnimal = dbContext.AnimalTypes.Find(typeAnimalInd);
                typeAnimal.Animal = animal;
            }
            animal.AnimalTypes = new List<AnimalType>();
            dbContext.Animals.Add(animal);
            dbContext.SaveChanges();
            return animal;
        }
        public Animal EditAnimal(long animalId, EditableAnimalDTO editableAnimalDTO)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var editableAnimal = dbContext.Animals.Find(animalId);
            editableAnimal.Weight = (float)editableAnimalDTO.Weight;
            editableAnimal.Height = (float)editableAnimalDTO.Height;
            editableAnimal.Length = (float)editableAnimalDTO.Length;
            editableAnimal.Gender = GenderConverter(editableAnimalDTO.Gender);
            if (editableAnimalDTO.LifeStatus == "DEAD" && editableAnimal.LifeStatus != Animal.lifeStatus.DEAD)
            {
                editableAnimal.DeathDateTime = DateTime.Now.ToString("O");
            }
            editableAnimal.LifeStatus = LifeStatusConverter(editableAnimalDTO.LifeStatus);
            editableAnimal.ChipperId = (int)editableAnimalDTO.ChipperId;
            editableAnimal.ChippingLocationId = (long)editableAnimalDTO.ChippingLocationId;
            dbContext.SaveChanges();
            return editableAnimal;
        }
        public void ChipAwayAnimal(long animalId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var deletableAnimal = dbContext.Animals.Find(animalId);
            foreach (var animalType in dbContext.AnimalTypes.Where(x=>x.AnimalId == animalId))
            {
                dbContext.Entry(animalType).State = EntityState.Modified;
            }
            if (visitedLocationService.GetListVisistedLocationsOfAnimal(animalId) != null)
            {
                foreach (var visitedLocation in visitedLocationService.GetListVisistedLocationsOfAnimal(animalId))
                {
                    dbContext.VisitedLocations.Remove(visitedLocation);
                }
            }
            
            dbContext.Animals.Remove(deletableAnimal);
            dbContext.SaveChanges();
        }
        private Animal.gender GenderConverter(string gernderString)
        {
            if (gernderString == "FEMALE")
            {
                return Animal.gender.FEMALE;
            }
            else if (gernderString == "MALE")
            {
                return Animal.gender.MALE;
            }
            else
            {
                return Animal.gender.OTHER;
            }

        }
        private Animal.lifeStatus LifeStatusConverter(string lifeStatusString)
        {
            if (lifeStatusString == "ALIVE")
            {
                return Animal.lifeStatus.ALIVE;
            }
            else 
            {
                return Animal.lifeStatus.DEAD;
            }
        }
    }
}
