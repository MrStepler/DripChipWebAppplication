using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DripChipWebApplication.Shared
{
    public class AnimalDTO
    {
        public long Id { get; set; }
        public long[]? AnimalTypes { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; }
        public string LifeStatus { get; set; }
        public string ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[]? VisitedLocations { get; set; }
        public string? DeathDateTime { get; set; }
        public AnimalDTO(Animal animal)
        {
            Id = animal.Id;
            Weight = animal.Weight;
            Length = animal.Length;
            Height = animal.Height;
            if (animal.Gender == Animal.gender.FEMALE)
            {
                Gender = "FEMALE";
            }
            if (animal.Gender == Animal.gender.MALE)
            {
                Gender = "MALE";
            }
            if (animal.Gender == Animal.gender.OTHER)
            {
                Gender = "OTHER";
            }
            if(animal.LifeStatus == Animal.lifeStatus.ALIVE)
            {
                LifeStatus = "ALIVE";
            }
            if (animal.LifeStatus == Animal.lifeStatus.DEAD)
            {
                LifeStatus = "DEAD";
            }
            ChipperId= animal.ChipperId;
            ChippingDateTime = animal.ChippingDateTime;
            ChippingLocationId = animal.ChippingLocationId;
            DeathDateTime= animal.DeathDateTime;

        }
        public AnimalDTO() { }
    }
}
