using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TvMaze.ConfigSettings;
using TvMaze.Interfaces;

namespace TvMaze.ScraperService
{
    public class Scraper : IScraperService
    {
        private readonly int _apiPageSize;
        private readonly IShowRepository _repo;
        private readonly ITvMazeApiClient _apiClient;
        private readonly ILogger _logger;

        public Scraper(IShowRepository repo, ITvMazeApiClient apiClient, IOptions<ScraperSettings> settings, ILogger<Scraper> logger)
        {
            _apiPageSize = settings.Value.ApiPageSize;
            _repo = repo;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task ScrapShowsAsync()
        {
            var lastShowId = await _repo.GetLastShowId();
            var pageToLoad = GetPageNumber(lastShowId + 1);

            var showsToLoadExist = true;
            while (showsToLoadExist)
            {
                var shows = await _apiClient.GetShowsAsync(pageToLoad);
                if (shows.Count > 0)
                {
                    pageToLoad += 1;
                    _logger.LogInformation($"Starting insert page {pageToLoad} at {DateTime.Now.ToLongTimeString()}");
                    await _repo.AddShowsAsync(shows);
                    _logger.LogInformation($"End insert page {pageToLoad} at {DateTime.Now.ToLongTimeString()}");
                }
                else
                {
                    showsToLoadExist = false;
                }
            }
        }

        private int GetPageNumber(long showId)
        {
            return (int)Math.Floor((double)showId / (double)_apiPageSize);
        }
    }
}
