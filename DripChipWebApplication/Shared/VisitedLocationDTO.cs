using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Shared
{
    public class VisitedLocationDTO
    {
        public long Id { get; set; }

        public string DateTimeOfVisitLocationPoint { get; set; }
        
        public long LocationPointId { get; set; }
        public VisitedLocationDTO(AnimalVisitedLocation animalVisitedLocation) 
        {
            Id = animalVisitedLocation.Id;
            DateTimeOfVisitLocationPoint = animalVisitedLocation.DateTimeOfVisitLocationPoint;
            LocationPointId = animalVisitedLocation.LocationPointId;
        }
    }
}
