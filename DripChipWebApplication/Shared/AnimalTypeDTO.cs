using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Shared
{
    public class AnimalTypeDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public AnimalTypeDTO(AnimalType animalType) 
        {
            Id = animalType.Id;
            Type = animalType.Type;
        }
        public AnimalTypeDTO() { }
    }
}
