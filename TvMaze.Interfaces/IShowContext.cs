using MongoDB.Driver;
using TvMaze.Models;

namespace TvMaze.Interfaces
{
    public interface IShowContext
    {
        IMongoCollection<TvShow> Shows { get; }
    }
}
