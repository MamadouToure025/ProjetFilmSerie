using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [Route("api/[controller]")] // URL: /api/genres
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public GenresController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        /// <summary>
        /// Récupère la liste officielle des genres de films (Action, Comédie, etc.)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            try
            {
                var genres = await _tmdbService.GetGenresAsync(); // Par défaut en fr-FR
                return Ok(genres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des genres : {ex.Message}");
            }
        }

        // --- ENDPOINT : Films actuellement au cinéma ---
        // URL : GET /api/Movies/now-playing
        [HttpGet("joues-en-salle")]
        public async Task<ActionResult<TmdbResponse>> GetNowPlayingMovies([FromQuery] int page = 1)
        {
            try
            {
                // On force "fr-FR" ici, ou on pourrait le passer en paramètre
                var response = await _tmdbService.GetNowPlayingMoviesAsync("fr-FR", page);

                if (response == null || response.Results == null || !response.Results.Any())
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des films en salle : {ex.Message}");
            }
        }

        /// <summary>
        /// Recherche un acteur ou une personnalité.
        /// Exemple : GET /api/People/search?query=Tom Hanks
        /// </summary>
        [HttpGet("recherche-acteur")]
        public async Task<ActionResult<ActorsResponse>> Search([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Le paramètre 'query' est obligatoire.");
            }

            try
            {
                var result = await _tmdbService.SearchPersonAsync(query, page);

                if (result == null || result.Results.Count == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la recherche : {ex.Message}");
            }
        }
    }
}