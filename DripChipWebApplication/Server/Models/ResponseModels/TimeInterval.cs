using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DripChipWebApplication.Server.Models.ResponseModels
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
