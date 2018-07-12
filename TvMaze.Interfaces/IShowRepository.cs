using System.Collections.Generic;
using System.Threading.Tasks;
using TvMaze.Models;

namespace TvMaze.Interfaces
{
    public interface IShowRepository
    {
        Task AddShowsAsync(IList<TvShow> shows);

        Task<IList<TvShow>> GetShowsAsync(int pageNumber, int pageSize);

        Task<long> GetLastShowId();
    }
}
