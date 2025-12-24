using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;     // Pour TvSeasonDetail
using MonApiTMDB.Services;   // Pour ITmdbService

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TvSeasonsController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public TvSeasonsController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // GET api/v1/TvSeasons/1399/1
        [HttpGet("{tvId}/{seasonNumber}")]
        public async Task<ActionResult<TvSeasonDetail>> GetSeasonDetail(int tvId, int seasonNumber)
        {
            var season = await _tmdbService.GetTvSeasonAsync(tvId, seasonNumber);

            if (season == null)
            {
                return NotFound($"La saison {seasonNumber} de la série {tvId} est introuvable.");
            }

            return Ok(season);
        }
    }
}