using Microsoft.EntityFrameworkCore;
using PFSite.Data;
using PFSite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PFSite.Repositories
{
    public class UserRepository
    {
        private ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        public async Task BindUser(User user)
        {
            _dbContext.Add(user);

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="githubId">GitHub Id</param>
        public async Task<bool> HasUserAsync(string githubId)
        {
            return await _dbContext.Users.AnyAsync(u => u.GitHubId == githubId);
        }

        /// <summary>
        /// 根据GitHub Id查找用户
        /// </summary>
        /// <param name="githubId">GitHub Id</param>
        public async Task<User> FindWithAsync(string githubId)
        {
            return await (from u in _dbContext.Users
                          where u.GitHubId == githubId
                          select u).SingleOrDefaultAsync();
        }

        /// <summary>
        /// 判断是否已经签到
        /// </summary>
        /// <param name="studentId">学号</param>
        public async Task<bool> IsSignedAsync(string studentId)
        {
            return await _dbContext.Records.AsNoTracking()
                .AnyAsync(r => r.SignOutTime == null && r.StudentId == studentId);
        }
    }
}
