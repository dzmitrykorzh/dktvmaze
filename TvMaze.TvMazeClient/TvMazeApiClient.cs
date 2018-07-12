using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TvMaze.ConfigSettings;
using TvMaze.Models;
using TvMaze.Interfaces;

namespace TvMaze.TvMazeClient
{
    public class TvMazeApiClient : ITvMazeApiClient
    {
        private const string ShowApiResource = "shows";
        private const string CastApiResource = "cast";
        private const string PageApiParameter = "page";

        private readonly int _limitSleepTime;
        private readonly HttpStatusCode _limitHttpCode;
        private readonly IRestClient _restClient;
        private readonly ILogger _logger;

        public TvMazeApiClient(IRestClient restClient, IOptions<ApiSettings> settings, ILogger<TvMazeApiClient> logger)
        {
            _restClient = restClient;
            _logger = logger;
            _restClient.BaseUrl = new Uri(settings.Value.ApiBaseUrl);
            _limitSleepTime = settings.Value.ApiLimitSleepTimeSeconds;
            _limitHttpCode = settings.Value.ApiLimitHttpCode;
        }

        /// <summary>
        /// Get shows from TvMaze API. 
        /// Paginated with page size 250
        /// </summary>
        /// <param name="page">pagenumber</param>
        /// <returns>list of tv shows</returns>
        public async Task<IList<TvShow>> GetShowsAsync(int page)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page));

            _logger.LogInformation($"Starting downloading page {page} at {DateTime.Now.ToLongTimeString()}");

            var shows = new List<TvShow>();

            var request = new RestRequest(ShowApiResource, Method.GET);
            request.AddQueryParameter(PageApiParameter, page.ToString());

            var limitReached = false;
            do
            {
                limitReached = false;
                var response = await _restClient.ExecuteTaskAsync<List<TvShow>>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    shows.AddRange(await EnrichShowsWithPersonsAsync(response.Data));
                }
                if (response.StatusCode == _limitHttpCode)
                {
                    _logger.LogInformation("show request limit reached");
                    limitReached = true;
                    await Task.Delay(TimeSpan.FromSeconds(_limitSleepTime));
                }
            } while (limitReached);

            _logger.LogInformation($"End downloading page {page} at {DateTime.Now.ToLongTimeString()}, downloaded shows: {shows.Count}");

            return shows;
        }

        /// <summary>
        /// Enrich each show whith persons information
        /// </summary>
        /// <param name="shows">List of shows</param>
        /// <returns>List of shows</returns>
        private async Task<IList<TvShow>> EnrichShowsWithPersonsAsync(IList<TvShow> shows)
        {
            foreach (var tvShow in shows)
            {
                var request = new RestRequest($"{ShowApiResource}/{tvShow.Id}/{CastApiResource}", Method.GET);

                var limitReached = false;
                do
                {
                    limitReached = false;
                    var response = await _restClient.ExecuteTaskAsync<List<Actor>>(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (var actor in response.Data)
                        {
                            tvShow.Cast.Add(actor.Person);
                        }
                    }
                    if (response.StatusCode == _limitHttpCode)
                    {
                        _logger.LogInformation("show request limit reached");
                        limitReached = true;
                        await Task.Delay(TimeSpan.FromSeconds(_limitSleepTime));
                    }
                } while (limitReached);
            }
            return shows;
        }
    }
}