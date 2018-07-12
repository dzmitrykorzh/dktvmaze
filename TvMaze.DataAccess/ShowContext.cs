using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;
using TvMaze.ConfigSettings;
using TvMaze.Interfaces;
using TvMaze.Models;

namespace TvMaze.DataAccess
{
    public class ShowContext : IShowContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;
        
        public ShowContext(IOptions<DbSettings> settings)
        {
            _collectionName = settings.Value.CollectionName;
            MongoClientSettings clientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
            clientSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var client = new MongoClient(clientSettings);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<TvShow> Shows => _database.GetCollection<TvShow>(_collectionName);
        
    }
}
