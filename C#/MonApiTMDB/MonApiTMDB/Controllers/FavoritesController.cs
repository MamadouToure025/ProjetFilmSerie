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
    [Authorize] // Il faut être connecté pour gérer ses favoris
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITmdbService _tmdbService;

        public FavoritesController(AppDbContext context, ITmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // ==============================================================
        // AJOUTER OU RETIRER (TOGGLE)
        // C'est ici que la magie "Récupérer puis Ajouter" opère
        // ==============================================================
        [HttpPost("toggle")]
        public async Task<ActionResult> ToggleFavorite([FromBody] FavoriteDto request)
        {
            // 1. Qui est connecté ? (On récupère l'ID depuis le Token JWT)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Conversion String -> Int (car votre User.Id est un int)
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Utilisateur non identifié.");
            }

            // 2. Validation simple
            if (request.MediaType != "movie" && request.MediaType != "tv")
                return BadRequest("Le type doit être 'movie' ou 'tv'.");

            // 3. Vérifier si le favori est DÉJÀ en base
            var existingFav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MediaId == request.MediaId && f.MediaType == request.MediaType);

            if (existingFav != null)
            {
                // --- CAS A : IL EXISTE -> ON LE SUPPRIME ---
                _context.Favorites.Remove(existingFav);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Retiré des favoris", isFavorite = false });
            }
            else
            {
                // --- CAS B : IL N'EXISTE PAS -> ON L'AJOUTE ---
                
                string title = "Inconnu";
                string poster = "";

                // ÉTAPE CLÉ : On va chercher les infos chez TMDB
                if (request.MediaType == "movie")
                {
                    var movie = await _tmdbService.GetMovieDetailAsync(request.MediaId);
                    if (movie == null) return NotFound("Film introuvable sur TMDB.");
                    
                    title = movie.Title;
                    poster = movie.PosterPath;
                }
                else // tv
                {
                    var show = await _tmdbService.GetTvShowDetailAsync(request.MediaId);
                    if (show == null) return NotFound("Série introuvable sur TMDB.");
                    
                    title = show.Name;
                    poster = show.PosterPath;
                }

                // On crée l'objet Favori avec les données récupérées
                var newFav = new Favorite
                {
                    UserId = userId,
                    MediaId = request.MediaId,
                    MediaType = request.MediaType,
                    Title = title,       // Stocké pour affichage rapide
                    PosterPath = poster, // Stocké pour affichage rapide
                    AddedAt = DateTime.UtcNow
                };

                // On sauvegarde en base
                _context.Favorites.Add(newFav);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ajouté aux favoris", isFavorite = true, title = title });
            }
        }

        // ==============================================================
        // LISTER MES FAVORIS
        // ==============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetMyFavorites()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();

            return Ok(favorites);
        }

        // ==============================================================
        // VÉRIFIER SI DÉJÀ FAVORI (Pour colorer le cœur)
        // ==============================================================
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