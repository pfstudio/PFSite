using System;

namespace PFSite.Dtos
{
    public class RecordDuration
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
