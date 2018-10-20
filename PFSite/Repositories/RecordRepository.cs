using Microsoft.EntityFrameworkCore;
using PFSite.Data;
using PFSite.Dtos;
using PFSite.Extensions;
using PFSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFSite.Repositories
{
    public class RecordRepository
    {
        private readonly ApplicationDbContext _dbContext;
        // 超时未签退所扣除的积分
        private const int timeoutDeductedPoints = -4;

        public RecordRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="studentId">学号</param>
        /// <param name="name">姓名</param>
        public async Task SignInAsync(string studentId, string name)
        {
            // 创建签到
            Record record = Record.SignIn(studentId, name);

            // 保存到数据库
            _dbContext.Add(record);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 签退
        /// </summary>
        /// <param name="studentId">学号</param>
        public async Task SignOutAsync(string studentId)
        {
            // 查找签到记录
            Record record = await (from r in _dbContext.Records
                                   where r.StudentId == studentId
                                   && r.SignOutTime == null
                                   select r).FirstAsync();
            // 签退
            record.SignOut();

            // 保存到数据库
            _dbContext.Update(record);

            // 清算积分
            if (record.Duration > TimeSpan.FromHours(2))
            {
                User user = await _dbContext.Users.FindAsync(record.StudentId);
                int pointChange = record.Duration?.Hours ?? 0;
                user.Points += pointChange;
                PointLog log = PointLog.CreateBySign(user, pointChange);

                _dbContext.Add(log);
                _dbContext.Update(user);
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 清理所有未签退用户
        /// </summary>
        public async Task ClearAllUnSignOutAsync()
        {
            // 查找所有未签到的人员
            var records = await (from r in _dbContext.Records
                                 where r.SignOutTime == null
                                 select r).ToListAsync();

            // 为所有人签退
            records.ForEach(r => r.SignOut());

            // 记录未签到人员的学号
            var studentIds = records
                .Select(r => r.StudentId)
                .Distinct().ToList();

            // 扣除积分
            List<User> users = await _dbContext.Users
                .Where(u => studentIds.Contains(u.StudentId)).ToListAsync();
            users.ForEach(u => u.Points += timeoutDeductedPoints);
            List<PointLog> logs = users.Select(
                u => PointLog.Create(
                    u.StudentId, timeoutDeductedPoints, "超时清退")).ToList();

            // 更新数据库
            _dbContext.UpdateRange(records);
            _dbContext.AddRange(logs);
            _dbContext.UpdateRange(users);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 所有人本周的签到计时/降序
        /// </summary>
        public async Task<List<RecordDuration>> ReportAllAsync()
        {
            // 默认报告时间段为本周
            DateTime begin = DateTime.Today.StartOfWeek().ToUniversalTime();
            DateTime end   = begin.AddDays(7);

            // 读取本周内所有签到记录
            var records = await (from r in _dbContext.Records.AsNoTracking()
                                 where r.SignOutTime != null
                                 && r.SignOutTime >= begin
                                 && r.SignInTime < end
                                 select r).ToListAsync();

            // 统计签到记录
            var result = records.GroupBy(r => new { r.StudentId, r.Name })
                            .Select(g => new RecordDuration
                            {
                                StudentId = g.Key.StudentId,
                                Name = g.Key.Name,
                                Duration = g.Sum(r => r.Duration ?? TimeSpan.Zero)
                            })
                            .OrderByDescending(rd => rd.Duration)
                            .ToList();

            return result;
        }

        public async Task<List<DateDuration>> ReportWithAsync(string studentId)
        {
            // 默认报告时间段为本周
            DateTime begin = DateTime.Today.StartOfWeek().ToUniversalTime();
            DateTime end = begin.AddDays(7);
            // 已知(end - begin).Days 必为 7
            List<DateTime> dates = Enumerable.Range(0, 7)
                .Select(i => begin.AddDays(i)).ToList();

            // 读取该用户本周内所有签到记录
            var records = await (from r in _dbContext.Records.AsNoTracking()
                                 where r.SignOutTime != null
                                 && r.SignOutTime >= begin
                                 && r.SignInTime < end
                                 && r.StudentId == studentId
                                 select r).ToListAsync();

            // 统计签到记录
            var result = (from d in dates
                          let durtion = (from r in records
                                         where r.SignInTime >= d
                                         && r.SignInTime <= d.AddDays(1)
                                         select r).Sum(r => r.Duration)
                          select new DateDuration
                          {
                              Date = d,
                              Duration = durtion
                          }).ToList();

            return result;
        }
    }
}
