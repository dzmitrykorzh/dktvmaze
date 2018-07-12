using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TvMaze.ConfigSettings;
using TvMaze.Interfaces;

namespace WebApi.HostedService
{
    public class ScraperBackgroundRunner : BackgroundService
    {
        private readonly IScraperService _scraper;
        private readonly int _taskRestartHours; 

        public ScraperBackgroundRunner(IScraperService scraper, IOptions<BackgroundTaskSettings> settings)
        {
            _scraper = scraper;
            _taskRestartHours = settings.Value.TaskRestartHours;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _scraper.ScrapShowsAsync();
                await Task.Delay(TimeSpan.FromHours(_taskRestartHours), stoppingToken);
            }
        }
    }
}
