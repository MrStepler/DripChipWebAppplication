using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DripChipWebApplication.Shared
{
    public class AnimalVisitedLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string DateTimeOfVisitLocationPoint { get; set; }
        [Required]
        public long LocationPointId { get; set; }

        [AllowNull]
        public long? animalID { get; set; }
        [ForeignKey("animalID")]
        [AllowNull]
        public Animal? Animal { get; set; }
    }
}
