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
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITmdbService _tmdbService;

        public FavoritesController(AppDbContext context, ITmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // ==========================================
        // 1. AJOUTER (Avec détails complets)
        // ==========================================
        [HttpPost("toggle")]
        public async Task<ActionResult> ToggleFavorite([FromBody] FavoriteDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MediaId == request.MediaId && f.MediaType == request.MediaType);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Retiré des favoris", isFavorite = false });
            }
            else
            {
                // Préparation des données à stocker
                var newFav = new Favorite
                {
                    UserId = userId,
                    MediaId = request.MediaId,
                    MediaType = request.MediaType,
                    AddedAt = DateTime.UtcNow
                };

                // On récupère TOUT chez TMDB
                if (request.MediaType == "movie")
                {
                    var m = await _tmdbService.GetMovieDetailAsync(request.MediaId);
                    if (m == null) return NotFound("Film introuvable.");
                    
                    newFav.Title = m.Title;
                    newFav.PosterPath = m.PosterPath;
                    newFav.Overview = m.Overview;           // <--- Nouveau
                    newFav.BackdropPath = m.BackdropPath;   // <--- Nouveau
                    newFav.VoteAverage = m.VoteAverage;     // <--- Nouveau
                    newFav.ReleaseDate = m.ReleaseDate;     // <--- Nouveau
                }
                else // tv
                {
                    var s = await _tmdbService.GetTvShowDetailAsync(request.MediaId);
                    if (s == null) return NotFound("Série introuvable.");

                    newFav.Title = s.Name;
                    newFav.PosterPath = s.PosterPath;
                    newFav.Overview = s.Overview;
                    newFav.BackdropPath = s.BackdropPath;
                    newFav.VoteAverage = s.VoteAverage;
                    newFav.ReleaseDate = s.FirstAirDate;
                }

                _context.Favorites.Add(newFav);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ajouté aux favoris", isFavorite = true, title = newFav.Title });
            }
        }

        // ==========================================
        // 2. LISTE PAGINÉE (Format TMDB Exact)
        // URL : GET api/v1/Favorites?page=1&type=movie
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> GetMyFavorites([FromQuery] int page = 1, [FromQuery] string type = "all")
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            int pageSize = 20; // Standard TMDB

            // 1. Filtrer la requête (User + Type optionnel)
            var query = _context.Favorites.Where(f => f.UserId == userId);
            
            if (type == "movie") query = query.Where(f => f.MediaType == "movie");
            else if (type == "tv") query = query.Where(f => f.MediaType == "tv");

            // 2. Compter le total pour la pagination
            int totalResults = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            // 3. Récupérer la page demandée
            var results = await query
                .OrderByDescending(f => f.AddedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. Construire la réponse JSON exacte
            var response = new
            {
                page = page,
                results = results, // Contient maintenant Overview, Backdrop, etc.
                total_pages = totalPages,
                total_results = totalResults
            };

            return Ok(response);
        }
    }
}