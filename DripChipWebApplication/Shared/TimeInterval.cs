using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Shared
{
    public class TimeInterval
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ssZ}", ApplyFormatInEditMode = true)]
        public DateTime? startDateTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ssZ}", ApplyFormatInEditMode = true)]
        public DateTime? endDateTime { get; set; }
    }
}
