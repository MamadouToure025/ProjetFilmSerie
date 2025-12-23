using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Models.Dtos;
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

// ==============================================================
        // 1. OBTENIR UN PASS INVITÉ (Nécessaire pour voter)
        // URL : GET api/Movies/guest-session
        // ==============================================================
        [Authorize]
        [HttpGet("guest-session")]
        public async Task<ActionResult<string>> CreateGuestSession()
        {
            try
            {
                var sessionId = await _tmdbService.CreateGuestSessionAsync();
                return Ok(new { guest_session_id = sessionId });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ==============================================================
        // 2. NOTER UN FILM PAR NOM
        // URL : POST api/Movies/rate/name/Superman
        // Body : { "value": 8.5, "guest_session_id": "VOTRE_ID_ICI" }
        // ==============================================================
        [Authorize]
        [HttpPost("rate/name/{name}")]
        public async Task<ActionResult> RateMovieByName(string name, [FromBody] RatingDto rating)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Nom requis.");
            if (string.IsNullOrWhiteSpace(rating.GuestSessionId)) return BadRequest("Guest Session ID requis.");

            try
            {
                // A. On cherche l'ID du film
                var search = await _tmdbService.SearchMovieAsync(name);
                
                // Vérification si résultats existent
                if (search == null || search.Results == null || !search.Results.Any())
                {
                    return NotFound($"Film '{name}' introuvable.");
                }

                var movie = search.Results.First(); // On prend le 1er résultat

                // B. On note
                var result = await _tmdbService.RateMovieAsync(movie.Id, rating.Value, rating.GuestSessionId);

                // C. Analyse réponse
                if (result != null)
                {
                    // Code 1 (Success) ou 12 (Updated)
                    if (result.StatusCode == 1 || result.StatusCode == 12)
                        return Ok(new { message = "Film noté !", film = movie.Title, tmdb_response = result });
                    
                    // Sinon, c'est une erreur TMDB (ex: Session invalide)
                    return BadRequest(result);
                }
                
                return StatusCode(500, "Erreur inconnue TMDB");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ==============================================================
        // 3. SUPPRIMER NOTE PAR NOM
        // URL : DELETE api/Movies/rate/name/Superman?guestSessionId=...
        // ==============================================================
        [Authorize]
        [HttpDelete("rate/name/{name}")]
        public async Task<ActionResult> DeleteMovieRatingByName(string name, [FromQuery] string guestSessionId)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Nom requis.");
            if (string.IsNullOrWhiteSpace(guestSessionId)) return BadRequest("Guest Session ID requis.");

            try
            {
                var search = await _tmdbService.SearchMovieAsync(name);
                if (search == null || search.Results == null || !search.Results.Any())
                    return NotFound($"Film '{name}' introuvable.");

                var movie = search.Results.First();

                var result = await _tmdbService.DeleteMovieRatingAsync(movie.Id, guestSessionId);

                // Code 13 (Deleted)
                if (result != null && result.StatusCode == 13)
                {
                    return Ok(new { message = "Note supprimée !", film = movie.Title, tmdb_response = result });
                }

                return BadRequest(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}