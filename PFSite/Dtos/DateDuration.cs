using System;
using System.ComponentModel.DataAnnotations;

namespace PFSite.Dtos
{
    public class DateDuration
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
