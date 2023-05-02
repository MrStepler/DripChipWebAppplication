using DripChipWebApplication.Server.Data;
using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DripChipWebApplication.Server.Services
{
    public class VisitedLocationService : IVisitedLocationService
    {
        IDbContextFactory<APIDbContext> contextFactory;
        public VisitedLocationService(IDbContextFactory<APIDbContext> contextFactory) 
        {
            this.contextFactory = contextFactory;
        }

        public long[]? GetVisitedLocationsIDs(long animalId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.VisitedLocations.Any(x => x.animalID == animalId))
            {
                return null;
            }
            return dbContext.VisitedLocations.Where(x => x.animalID == animalId).Select(x => x.Id).ToArray();
        }
        public List<AnimalVisitedLocation>? GetListVisistedLocationsOfAnimal(long animalId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.VisitedLocations.Any(x => x.animalID == animalId))
            {
                return null;
            }
            return dbContext.VisitedLocations.Where(x => x.animalID == animalId).OrderBy(x =>x.Id).ToList();
        }
        public List<AnimalVisitedLocation>? GetListVisistedLocationsByPointId(long pointId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.VisitedLocations.Any(x => x.LocationPointId == pointId))
            {
                return null;
            }
            return dbContext.VisitedLocations.Where(x => x.LocationPointId == pointId).OrderBy(x => x.Id).ToList();
        }
        public AnimalVisitedLocation? GetVisitedLocationsById(long visitedPointId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.VisitedLocations.Any(x => x.Id == visitedPointId))
            {
                return null;
            }
            return dbContext.VisitedLocations.First(x => x.Id == visitedPointId);
        }
        public AnimalVisitedLocation EditVisitedLocation(long visitedPointId, long newPointId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var editableVisitedLocation = dbContext.VisitedLocations.Find(visitedPointId);
            editableVisitedLocation.LocationPointId = newPointId;
            dbContext.SaveChanges();
            return editableVisitedLocation;
        }
        public AnimalVisitedLocation VisitLocation(long animalId, long pointId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            AnimalVisitedLocation visitedLocation = new AnimalVisitedLocation();
            visitedLocation.LocationPointId = pointId;
            visitedLocation.DateTimeOfVisitLocationPoint = DateTime.Now.ToString("O");
            visitedLocation.Animal = dbContext.Animals.First(x => x.Id == animalId);
            dbContext.VisitedLocations.Add(visitedLocation);
            dbContext.SaveChanges();
            return visitedLocation;
        }
        public void DeleteVisitedLocation(long animalId, long visitedPointId) //kdafldjvgdldkdvgnhidlxshgfvlxdck
        {
            using var dbContext = contextFactory.CreateDbContext();
            var deletableVisitedPoint = dbContext.VisitedLocations.FirstOrDefault(x=>x.Id == visitedPointId && x.animalID ==animalId);
            dbContext.VisitedLocations.Remove(deletableVisitedPoint);
            dbContext.SaveChanges();
        }
    }
}
