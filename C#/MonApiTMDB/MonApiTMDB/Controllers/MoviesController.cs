using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public MoviesController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // --- 1. Films Populaires ---
        [HttpGet("popular")]
        public async Task<ActionResult<TmdbResponse>> GetPopularMovies()
        {
            try
            {
                var response = await _tmdbService.GetPopularMoviesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 2. Recherche Avancée (Discover) ---
        [HttpGet("discover")]
        public async Task<ActionResult<TmdbResponse>> DiscoverMovies(
            [FromQuery] int? genreId, 
            [FromQuery] int? year, 
            [FromQuery] int? companyId,
            [FromQuery] int page = 1)
        {
            try
            {
                var response = await _tmdbService.DiscoverMoviesAsync(genreId, year, companyId, page);
                if (response == null || response.Results == null || !response.Results.Any()) return NoContent();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 3. Films en Salle ---
        [HttpGet("now-playing")]
        public async Task<ActionResult<TmdbResponse>> GetNowPlayingMovies([FromQuery] int page = 1)
        {
            try
            {
                var response = await _tmdbService.GetNowPlayingMoviesAsync("fr-FR", page);
                if (response == null || response.Results == null || !response.Results.Any()) return NoContent();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 4. Recherche par Nom ---
        [HttpGet("search")]
        public async Task<ActionResult<TmdbResponse>> SearchMovies([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Le nom du film (query) est obligatoire.");
            try
            {
                var result = await _tmdbService.SearchMovieAsync(query, page);
                if (result == null || result.Results == null || !result.Results.Any()) return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 5. Détails (ID) --- 
        // 👇 CORRECTION CRUCIALE ICI : {id:int}
        [HttpGet("{id:int}")] 
        public async Task<ActionResult<Movie>> GetMovieDetail(int id)
        {
            try
            {
                var movie = await _tmdbService.GetMovieDetailAsync(id);
                if (movie == null) return NotFound($"Film ID {id} introuvable.");
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 6. Auth Guest Session ---
        [HttpGet("guest-session")]
        public async Task<ActionResult<string>> CreateGuestSession()
        {
            try 
            {
                var sessionId = await _tmdbService.CreateGuestSessionAsync();
                return Ok(new { guestSessionId = sessionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 7. Noter un film ---
        // 👇 CORRECTION CRUCIALE ICI : {id:int}
        [HttpPost("{id:int}/rating")] 
        public async Task<ActionResult<TmdbStatusResponse>> RateMovie(int id, [FromQuery] string guestSessionId, [FromBody] double rating)
        {
            if (string.IsNullOrEmpty(guestSessionId)) return BadRequest("Un guestSessionId est obligatoire.");
            if (rating < 0.5 || rating > 10.0) return BadRequest("La note doit être comprise entre 0.5 et 10.0");

            try
            {
                var result = await _tmdbService.RateMovieAsync(id, rating, guestSessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 8. Supprimer note ---
        // 👇 CORRECTION CRUCIALE ICI : {id:int}
        [HttpDelete("{id:int}/rating")] 
        public async Task<ActionResult<TmdbStatusResponse>> DeleteRating(int id, [FromQuery] string guestSessionId)
        {
            if (string.IsNullOrEmpty(guestSessionId)) return BadRequest("Le guestSessionId est obligatoire.");

            try
            {
                var result = await _tmdbService.DeleteMovieRatingAsync(id, guestSessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}