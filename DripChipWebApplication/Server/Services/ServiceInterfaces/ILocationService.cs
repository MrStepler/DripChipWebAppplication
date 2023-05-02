using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Models.ResponseModels.Locations;

namespace DripChipWebApplication.Server.Services.ServiceInterfaces
{
    public interface ILocationService
    {
        AnimalLocation? GetLocation(long pointId);
        AnimalLocation? GetLocation(EditCreateLocation location);
        AnimalLocation AddLocation(EditCreateLocation location);
        AnimalLocation EditLocation(long pointid, EditCreateLocation location);
        void DeleteLocation(long pointId);
    }
}
