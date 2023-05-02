using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Shared
{
    public class EditCreateLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
