using System;

namespace PFSite.Models
{
    public class PointLog
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int Change { get; set; }
        public string Reason { get; set; }
        public DateTime Time { get; set; }

        public static PointLog Create(string studentId, int change, string reason)
        {
            return new PointLog
            {
                StudentId = studentId,
                Change = change,
                Reason = reason,
                Time = DateTime.UtcNow
            };
        }

        public static PointLog CreateBySign(User user, int change)
            => Create(user.StudentId, change, "签到");
    }
}
