using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DripChipWebApplication.Server.Models
{
    public class AnimalType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Type { get; set; }
        [AllowNull]
        public long? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        [AllowNull]
        public Animal? Animal { get; set; }

    }
}
