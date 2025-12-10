using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet("populaires")]
        public async Task<ActionResult<TmdbResponse>> GetPopularMovies()
        {
            try { return Ok(await _tmdbService.GetPopularMoviesAsync()); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("decouvrir")]
        public async Task<ActionResult<TmdbResponse>> DiscoverMovies([FromQuery] int? genreId, [FromQuery] int? year, [FromQuery] int? companyId, [FromQuery] int page = 1)
        {
            try { return Ok(await _tmdbService.DiscoverMoviesAsync(genreId, year, companyId, page)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("actuellement-en-salle")]
        public async Task<ActionResult<TmdbResponse>> GetNowPlayingMovies([FromQuery] int page = 1)
        {
            try { return Ok(await _tmdbService.GetNowPlayingMoviesAsync("fr-FR", page)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
        [HttpGet("mieux-notes")]
        public async Task<ActionResult<TmdbResponse>> GetTopRatedMovies([FromQuery] int page = 1)
    {
            return Ok(await _tmdbService.GetTopRatedMoviesAsync("fr-FR", page));
    } 
        
        // Dans MoviesController.cs

        [HttpGet("tendances")]
        public async Task<ActionResult<TmdbResponse>> GetTrendingMovies()
        {
            try 
            { 
                return Ok(await _tmdbService.GetTrendingMoviesAsync()); 
            } 
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        }
        
      [HttpGet("{id:int}/credits")]
      public async Task<ActionResult<MovieCredits>> GetMovieCredits(int id)
      {
          try
          {
              var credits = await _tmdbService.GetMovieCreditsAsync(id);
              return credits != null ? Ok(credits) : NotFound();
          }
          catch (Exception ex)
          {
              return StatusCode(500, ex.Message);
          }
      }
        
      // Recherche le premier film correspondant au nom et renvoie directement ses acteurs
      [HttpGet("recherche-credits")]
      public async Task<ActionResult<MovieCredits>> GetCreditsByMovieName([FromQuery] string query)
      {
          // 1. On cherche le film
          var searchResult = await _tmdbService.SearchMovieAsync(query);
    
          // 2. On prend le premier résultat
          var firstMovie = searchResult?.Results?.FirstOrDefault();
    
          if (firstMovie == null)
          {
              return NotFound($"Aucun film trouvé pour '{query}'");
          }

          // 3. On utilise l'ID de ce film pour récupérer les crédits
          var credits = await _tmdbService.GetMovieCreditsAsync(firstMovie.Id);
    
          return Ok(credits);
      }
      
      

        [HttpGet("recherche")]
        public async Task<ActionResult<TmdbResponse>> SearchMovies([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Le nom est obligatoire.");
            try { return Ok(await _tmdbService.SearchMovieAsync(query, page)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // CORRECTION ICI : {id:int}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Movie>> GetMovieDetail(int id)
        {
            try { 
                var movie = await _tmdbService.GetMovieDetailAsync(id);
                return movie != null ? Ok(movie) : NotFound();
            } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("session-invitee")]
        public async Task<ActionResult<string>> CreateGuestSession()
        {
            try { return Ok(new { guestSessionId = await _tmdbService.CreateGuestSessionAsync() }); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // CORRECTION ICI : {id:int}
        [HttpPost("{id:int}/notation")]
        public async Task<ActionResult<TmdbStatusResponse>> RateMovie(int id, [FromQuery] string guestSessionId, [FromBody] double rating)
        {
            try { return Ok(await _tmdbService.RateMovieAsync(id, rating, guestSessionId)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // CORRECTION ICI : {id:int}
        [HttpDelete("{id:int}/notation")]
        public async Task<ActionResult<TmdbStatusResponse>> DeleteRating(int id, [FromQuery] string guestSessionId)
        {
            try { return Ok(await _tmdbService.DeleteMovieRatingAsync(id, guestSessionId)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}