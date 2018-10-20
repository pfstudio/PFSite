using System;

namespace PFSite.Models
{
    public class Record
    {
        public int Id { get; set; }
        // 学号
        public string StudentId { get; set; }
        // 姓名
        public string Name { get; set; }
        // 签到时间
        public DateTime SignInTime { get; set; }
        // 签退时间
        public DateTime? SignOutTime { get; set; }
        // 签到时长
        public TimeSpan? Duration { get; set; }

        // 超时时间
        private const int timeoutHours = 8;

        // 签到
        public static Record SignIn(string studentId, string name)
        {
            return new Record
            {
                StudentId = studentId,
                Name = name,
                SignInTime = DateTime.UtcNow
            };
        }

        // 签退
        public void SignOut()
        {
            SignOutTime = this.IsTimeOut() ?
                SignInTime.AddHours(timeoutHours) : DateTime.UtcNow;
            Duration = SignOutTime - SignInTime;
        }

        // 判断超时
        public bool IsTimeOut()
        {
            return SignInTime.AddHours(timeoutHours) < DateTime.UtcNow;
        }
    }
}
