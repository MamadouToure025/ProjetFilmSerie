using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonApiTMDB.Data;
using MonApiTMDB.Models;
using MonApiTMDB.Models.Dtos;
using MonApiTMDB.Services;
using System.Security.Claims;

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class WatchLaterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITmdbService _tmdbService;

        public WatchLaterController(AppDbContext context, ITmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // ==========================================
        // 1. AJOUTER / RETIRER (TOGGLE)
        // POST api/v1/WatchLater/toggle
        // Body: { "mediaId": 123, "mediaType": "movie" }
        // ==========================================
        [HttpPost("toggle")]
        public async Task<ActionResult> ToggleWatchLater([FromBody] WatchLaterDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // Vérifie si déjà dans la liste
            var existing = await _context.WatchLaters
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MediaId == request.MediaId && w.MediaType == request.MediaType);

            if (existing != null)
            {
                // RETIRER
                _context.WatchLaters.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Retiré de la liste 'À voir plus tard'.", isInWatchLater = false });
            }
            else
            {
                // AJOUTER
                var newItem = new WatchLater
                {
                    UserId = userId,
                    MediaId = request.MediaId,
                    MediaType = request.MediaType,
                    AddedAt = DateTime.UtcNow
                };

                // Récupération des infos TMDB pour le cache
                if (request.MediaType == "movie")
                {
                    var m = await _tmdbService.GetMovieDetailAsync(request.MediaId);
                    if (m != null)
                    {
                        newItem.Title = m.Title;
                        newItem.Overview = m.Overview;
                        newItem.PosterPath = m.PosterPath;
                        newItem.BackdropPath = m.BackdropPath;
                        newItem.VoteAverage = m.VoteAverage;
                        newItem.ReleaseDate = m.ReleaseDate;
                    }
                }
                else // tv
                {
                    var s = await _tmdbService.GetTvShowDetailAsync(request.MediaId);
                    if (s != null)
                    {
                        newItem.Title = s.Name;
                        newItem.Overview = s.Overview;
                        newItem.PosterPath = s.PosterPath;
                        newItem.BackdropPath = s.BackdropPath;
                        newItem.VoteAverage = s.VoteAverage;
                        newItem.ReleaseDate = s.FirstAirDate;
                    }
                }

                _context.WatchLaters.Add(newItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Ajouté à 'À voir plus tard'.", isInWatchLater = true });
            }
        }

        // ==========================================
        // 2. LISTER (GET) - AVEC FORMAT JSON ADAPTÉ
        // GET api/v1/WatchLater?page=1&type=movie
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> GetWatchLaterList([FromQuery] int page = 1, [FromQuery] string type = "all")
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            int pageSize = 20;
            var query = _context.WatchLaters.Where(w => w.UserId == userId);

            if (!string.IsNullOrEmpty(type) && type != "all")
            {
                query = query.Where(w => w.MediaType == type);
            }

            int totalResults = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            var list = await query
                .OrderByDescending(w => w.AddedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Construction de la réponse JSON dynamique
            object finalResults;

            if (type == "tv")
            {
                // Format SÉRIE
                finalResults = list.Select(w => new
                {
                    id = w.MediaId,
                    name = w.Title, // Stocké dans Title mais c'est le Name
                    original_name = w.Title,
                    overview = w.Overview,
                    poster_path = w.PosterPath,
                    backdrop_path = w.BackdropPath,
                    vote_average = w.VoteAverage,
                    first_air_date = w.ReleaseDate, // Stocké dans ReleaseDate
                    origin_country = new List<string> { "US" }
                });
            }
            else
            {
                // Format FILM (Défaut)
                finalResults = list.Select(w => new
                {
                    id = w.MediaId,
                    title = w.Title,
                    original_title = w.Title,
                    overview = w.Overview,
                    poster_path = w.PosterPath,
                    backdrop_path = w.BackdropPath,
                    vote_average = w.VoteAverage,
                    release_date = w.ReleaseDate
                });
            }

            return Ok(new
            {
                page = page,
                results = finalResults,
                total_pages = totalPages,
                total_results = totalResults
            });
        }
        
        // ==========================================
        // 3. VÉRIFIER SI PRÉSENT (Pour colorer le bouton)
        // GET api/v1/WatchLater/check?mediaId=123&mediaType=movie
        // ==========================================
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckWatchLater([FromQuery] int mediaId, [FromQuery] string mediaType)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Ok(false);

            var exists = await _context.WatchLaters
                .AnyAsync(w => w.UserId == userId && w.MediaId == mediaId && w.MediaType == mediaType);

            return Ok(exists);
        }
    }
}