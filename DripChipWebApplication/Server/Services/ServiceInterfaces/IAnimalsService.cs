using DripChipWebApplication.Server.Models;
using static DripChipWebApplication.Server.Models.Animal;
using System.Drawing;
using DripChipWebApplication.Server.Models.ResponseModels.Animal;

namespace DripChipWebApplication.Server.Services.ServiceInterfaces
{
    public interface IAnimalsService
    {
        Animal? GetAnimalById(long id);
        Animal ChipAnimal(CreateAnimalDTO createdAnimal);
        bool ExistAnimalWithType(long typeId);
        Animal[]? SearchAnimal(DateTime? startDateTime, DateTime? endDateTime, int? chipperId, long? chippingLocationId, string? lifeStatus, string? gender, int from, int size);
        AnimalVisitedLocation[]? GetVisitedLocation(long animalId, DateTime? startDateTime, DateTime? endDateTime, int from, int size);
        Animal EditAnimal(long animalId, EditableAnimalDTO editableAnimalDTO);
        List<Animal>? GetAnimalsByPointId(long pointId);
        void ChipAwayAnimal(long animalId);
    }

}
