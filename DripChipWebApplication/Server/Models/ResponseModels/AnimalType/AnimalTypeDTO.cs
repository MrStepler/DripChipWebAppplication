using System.ComponentModel.DataAnnotations;
using DripChipWebApplication.Server.Models;
namespace DripChipWebApplication.Server.Models.ResponseModels.AnimalType
{
    public class AnimalTypeDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public AnimalTypeDTO(Models.AnimalType animalType) 
        {
            Id = animalType.Id;
            Type = animalType.Type;
        }
    }
}
