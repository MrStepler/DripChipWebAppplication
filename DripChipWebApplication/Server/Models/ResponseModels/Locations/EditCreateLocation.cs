using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Server.Models.ResponseModels.Locations
{
    public class EditCreateLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
