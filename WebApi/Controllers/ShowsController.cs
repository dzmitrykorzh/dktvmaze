using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TvMaze.Interfaces;
using TvMaze.Models;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ShowsController : Controller
    {
        private readonly IShowRepository _showRepository;
        private readonly ILogger _logger;

        public ShowsController(IShowRepository showRepository, ILogger<ShowsController> logger)
        {
            _showRepository = showRepository;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of tv shows with casts
        /// </summary>
        /// <param name="page">page number(starts from 0)</param>
        /// <param name="size">shows per page (250 by default)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int page, [FromQuery]int size = 250)
        {
            if (page < 0) return BadRequest(new ArgumentOutOfRangeException(nameof(page)));
            if (size <= 0) return BadRequest(new ArgumentOutOfRangeException(nameof(size)));

            _logger.LogInformation($"Get shows. Page {page}, size {size}");
            var shows = await GetShowsAsync(page, size);

            if (shows == null || !shows.Any())
                return NotFound();

            return Ok(shows);
        }

        private async Task<IEnumerable<TvShow>> GetShowsAsync(int page, int size)
        {
            var shows = await _showRepository.GetShowsAsync(page, size);

            return shows.OrderBy(s => s.Id)
                .Select(s => new TvShow()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Cast = s.Cast.OrderByDescending(c => c.Birthday).ToList()
                })
                .ToList();
        }
    }
}
