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
    [Route("api/v1/[controller]")] // J'ai harmonisé la route avec votre AuthController
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

        [HttpPost("toggle")]
        public async Task<ActionResult> ToggleFavorite([FromBody] FavoriteDto request)
        {
            // 1. Récupérer l'ID (Format string dans le token -> int pour la BDD)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("ID utilisateur invalide.");
            }

            if (request.MediaType != "movie" && request.MediaType != "tv")
                return BadRequest("MediaType invalide.");

            // 2. Vérifier si existe
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
                // 3. Récupérer infos TMDB
                string title = "Inconnu";
                string poster = "";

                if (request.MediaType == "movie")
                {
                    var movie = await _tmdbService.GetMovieDetailAsync(request.MediaId);
                    if (movie != null) { title = movie.Title; poster = movie.PosterPath; }
                }
                else
                {
                    var show = await _tmdbService.GetTvShowDetailAsync(request.MediaId);
                    if (show != null) { title = show.Name; poster = show.PosterPath; }
                }

                var newFav = new Favorite
                {
                    UserId = userId, // C'est maintenant un int compatible
                    MediaId = request.MediaId,
                    MediaType = request.MediaType,
                    Title = title,
                    PosterPath = poster
                };

                _context.Favorites.Add(newFav);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ajouté aux favoris", isFavorite = true, title = title });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetMyFavorites()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
            
            var list = await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();

            return Ok(list);
        }
        
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckFavorite([FromQuery] int mediaId, [FromQuery] string mediaType)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Ok(false);

            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.MediaId == mediaId && f.MediaType == mediaType);
            
            return Ok(exists);
        }
    }
}