using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvMaze.ConfigSettings;
using TvMaze.Interfaces;
using TvMaze.Models;

namespace TvMaze.DataAccess
{
    public class ShowRepository : IShowRepository
    {
        private readonly string _idColumn;
        private readonly IShowContext _context;
        private readonly ILogger _logger;

        public ShowRepository(IShowContext showContext, IOptions<RepositorySettings> settings, ILogger<ShowRepository> logger)
        {
            _context = showContext;
            _logger = logger;
            _idColumn = settings.Value.IdColumn;
        }

        public async Task AddShowsAsync(IList<TvShow> shows)
        {
            var models = new WriteModel<TvShow>[shows.Count];

            //ReplaceOneModel with property IsUpsert set to true to upsert whole documents
            for (var i = 0; i < shows.Count; i++)
            {
                models[i] = new ReplaceOneModel<TvShow>(new BsonDocument(_idColumn, shows[i].Id), shows[i]) { IsUpsert = true };
            };

            try
            {
                await _context.Shows.BulkWriteAsync(models);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<IList<TvShow>> GetShowsAsync(int pageNumber, int pageSize)
        {
            IList<TvShow> shows = new List<TvShow>();
            try
            {
                shows = await _context.Shows.Find(_ => true).Skip(pageNumber * pageSize).Limit(pageSize).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return shows;
        }

        public async Task<long> GetLastShowId()
        {
            TvShow lastShow;
            try
            {
                lastShow = await _context.Shows.Find(new BsonDocument()).Sort(new BsonDocument(_idColumn, -1)).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }

            return lastShow?.Id ?? 0;
        }
    }
}
