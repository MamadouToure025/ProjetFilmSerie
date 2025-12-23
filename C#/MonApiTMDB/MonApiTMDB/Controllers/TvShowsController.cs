using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;
using MonApiTMDB.Models.Dtos; // <--- C'est cette ligne qui fait disparaître le rouge
namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TvShowsController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public TvShowsController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [HttpGet("tendances")]
        public async Task<ActionResult<TvShowResponse>> GetTrendingTvShows()
        {
            try 
            { 
                return Ok(await _tmdbService.GetTrendingTvShowsAsync()); 
            } 
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        }
        
        // --- NOUVEL ENDPOINT : RECHERCHE ---
        [HttpGet("rechercher/{query}")]
        public async Task<ActionResult<TvShowResponse>> SearchTvShows(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("La recherche ne peut pas être vide.");
            }

            try
            {
                var result = await _tmdbService.SearchTvShowAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
        // ==============================================================
// DÉCOUVRIR (Discover) AVEC FILTRES
// URL : GET api/TvShows/discover?genreId=18&year=2023
// ==============================================================
        [HttpGet("decouvrir")]
        public async Task<ActionResult<TvShowResponse>> DiscoverTvShows(
            [FromQuery] int? genreId,  // Optionnel : ID du genre (ex: 18 pour Drame)
            [FromQuery] int? year,     // Optionnel : Année (ex: 2023)
            [FromQuery] int page = 1)  // Optionnel : Page (défaut 1)
        {
            try
            {
                var result = await _tmdbService.DiscoverTvShowsAsync(genreId, year, "fr-FR", page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }  
        
        // ==============================================================
// SÉRIES DIFFUSÉES AUJOURD'HUI
// URL : GET api/TvShows/today
// ==============================================================
        [HttpGet("aujourd-hui")]
        public async Task<ActionResult<TvShowResponse>> GetAiringToday([FromQuery] int page = 1)
        {
            try
            {
                var result = await _tmdbService.GetAiringTodayTvShowsAsync("fr-FR", page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        } 
        
        // ==============================================================
// SÉRIES POPULAIRES
// URL : GET api/TvShows/popular
// ==============================================================
        [HttpGet("populaire")]
        public async Task<ActionResult<TvShowResponse>> GetPopular([FromQuery] int page = 1)
        {
            try
            {
                var result = await _tmdbService.GetPopularTvShowsAsync("fr-FR", page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
// GET: api/TvShows/1399 (Pour Game of Thrones)
        [HttpGet("{id}")]
        public async Task<ActionResult<TvShowDetail>> GetTvShowDetail(int id)
        {
            try
            {
                var result = await _tmdbService.GetTvShowDetailAsync(id);
        
                if (result == null) return NotFound("Série introuvable.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // ==============================================================
// DÉTAILS COMPLETS PAR NOM
// URL : GET api/TvShows/details/name/Game of Thrones
// ==============================================================
        [HttpGet("details/name/{name}")]
        public async Task<ActionResult<TvShowDetail>> GetTvShowDetailsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Le nom est vide.");

            try
            {
                // ETAPE 1 : On cherche l'ID via le nom
                // (On utilise la méthode de recherche que nous avons créée plus tôt)
                var searchResult = await _tmdbService.SearchTvShowAsync(name);

                // Si on ne trouve rien
                if (searchResult == null || searchResult.Results == null || !searchResult.Results.Any())
                {
                    return NotFound($"Aucune série trouvée pour : {name}");
                }

                // ETAPE 2 : On prend le premier résultat (le plus probable)
                var firstSeries = searchResult.Results.First();
                int seriesId = firstSeries.Id;

                // ETAPE 3 : On appelle la méthode de détails avec l'ID trouvé
                var details = await _tmdbService.GetTvShowDetailAsync(seriesId);

                if (details == null) return NotFound("Impossible de récupérer les détails.");

                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        
        
        // ==============================================================
// OBTENIR LES ACTEURS D'UNE SÉRIE (PAR ID)
// URL : GET api/TvShows/1399/credits
// ==============================================================
        [HttpGet("{id:int}/credits")]
        public async Task<ActionResult<TvShowCredits>> GetTvShowCredits(int id)
        {
            try
            {
                var credits = await _tmdbService.GetTvShowCreditsAsync(id);
                if (credits == null) return NotFound();
                return Ok(credits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

// ==============================================================
// OBTENIR LES ACTEURS D'UNE SÉRIE (PAR NOM)
// URL : GET api/TvShows/credits/name/Game of Thrones
// ==============================================================
        [HttpGet("credits/name/{name}")]
        public async Task<ActionResult<TvShowCredits>> GetCreditsByTvShowName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Nom requis.");

            try
            {
                // 1. On cherche la série
                var searchResult = await _tmdbService.SearchTvShowAsync(name);
        
                // 2. On prend la première
                var show = searchResult?.Results?.FirstOrDefault();

                if (show == null) return NotFound($"Série '{name}' introuvable.");

                // 3. On récupère les crédits via l'ID trouvé
                var credits = await _tmdbService.GetTvShowCreditsAsync(show.Id);

                return Ok(new 
                { 
                    serie_name = show.Name, 
                    serie_id = show.Id,
                    cast = credits?.Cast.Take(10), // On affiche juste le top 10 pour pas surcharger
                    crew = credits?.Crew.Take(5)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        
        //-----------------------------------------------------------
        
       // ----------------------------------------------------------
        // 1. NOTER UNE SÉRIE (PAR NOM)
        // POST: api/TvShows/rate/name/Lupin
        // ----------------------------------------------------------
        [Authorize]
        [HttpPost("rate/name/{name}")]
        public async Task<ActionResult<TmdbStatusResponse>> RateTvShowByName(string name, [FromBody] RatingDto rating)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Nom requis.");
            if (rating.Value < 0.5 || rating.Value > 10) return BadRequest("Note invalide (0.5 - 10).");

            try
            {
                // Etape 1 : Recherche ID
                var search = await _tmdbService.SearchTvShowAsync(name);
                if (search?.Results == null || !search.Results.Any()) 
                    return NotFound($"Série '{name}' introuvable.");

                var show = search.Results.First();

                // Etape 2 : Noter
                var result = await _tmdbService.RateTvShowAsync(show.Id, rating.Value, rating.GuestSessionId);

                if (result != null && (result.StatusCode == 1 || result.StatusCode == 12))
                    return Ok(new { message = "Série notée !", serie = show.Name, result });

                return BadRequest(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------------------------------------------------
        // 2. SUPPRIMER NOTE SÉRIE (PAR NOM)
        // DELETE: api/TvShows/rate/name/Lupin?guestSessionId=...
        // ----------------------------------------------------------
        [Authorize]
        [HttpDelete("rate/name/{name}")]
        public async Task<ActionResult<TmdbStatusResponse>> DeleteTvShowRatingByName(string name, [FromQuery] string guestSessionId)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Nom requis.");
            if (string.IsNullOrWhiteSpace(guestSessionId)) return BadRequest("Session ID requis.");

            try
            {
                // Etape 1 : Recherche ID
                var search = await _tmdbService.SearchTvShowAsync(name);
                if (search?.Results == null || !search.Results.Any()) 
                    return NotFound($"Série '{name}' introuvable.");

                var show = search.Results.First();

                // Etape 2 : Supprimer
                var result = await _tmdbService.DeleteTvShowRatingAsync(show.Id, guestSessionId);

                if (result != null && result.StatusCode == 13)
                    return Ok(new { message = "Note supprimée !", serie = show.Name, result });

                return BadRequest(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
} 
    
