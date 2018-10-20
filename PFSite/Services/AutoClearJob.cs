using Microsoft.Extensions.Logging;
using PFSite.Repositories;
using Pomelo.AspNetCore.TimedJob;
using System.Threading.Tasks;

namespace PFSite.Services
{
    /// <summary>
    /// 自动清人
    /// </summary>
    public class AutoClearJob : Job
    {
        private readonly ILogger _logger;

        public AutoClearJob(ILogger<AutoClearJob> logger)
        {
            _logger = logger;
            _logger.LogDebug("Use the AutoClearJob");
        }

        // 每天定时清人
        [Invoke(Begin = "2018-01-01", Interval = 1000 * 60 * 60 * 24)]
        // 注入仓储对象
        public async Task Clear(RecordRepository recordRepository)
        {
            _logger.LogDebug("Invoke the AutoClearJob");

            await recordRepository.ClearAllUnSignOutAsync();
        }
    }
}
