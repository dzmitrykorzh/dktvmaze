using System.Collections.Generic;
using System.Threading.Tasks;
using TvMaze.Models;

namespace TvMaze.Interfaces
{
    public interface ITvMazeApiClient
    {
        Task<IList<TvShow>> GetShowsAsync(int page);
    }
}
