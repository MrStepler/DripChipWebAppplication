using DripChipWebApplication.Server.Models;

namespace DripChipWebApplication.Server.Services.ServiceInterfaces
{
    public interface IVisitedLocationService
    {
        AnimalVisitedLocation VisitLocation(long animalId, long pointId);
        long[]? GetVisitedLocationsIDs(long animalId);
        AnimalVisitedLocation? GetVisitedLocationsById(long visitedPointId);
        AnimalVisitedLocation EditVisitedLocation(long visitedPointId, long newPointId);
        List<AnimalVisitedLocation>? GetListVisistedLocationsOfAnimal(long animalId);
        List<AnimalVisitedLocation>? GetListVisistedLocationsByPointId(long pointId);
        void DeleteVisitedLocation(long animalId, long visitedPointId);
    }
}
