using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonApiTMDB.Data;
using MonApiTMDB.Models;
using MonApiTMDB.Models.Dtos; // INDISPENSABLE pour RatingDto et RatingDtoUser
using MonApiTMDB.Services;
using System.Security.Claims;

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITmdbService _tmdbService;

        public RatingsController(AppDbContext context, ITmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // ==========================================
        // 1. AJOUTER / MODIFIER UNE NOTE
        // ==========================================
        [HttpPost]
        public async Task<ActionResult> AddOrUpdateRating([FromBody] RatingDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var existing = await _context.RatingUsers
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MediaId == request.MediaId && r.MediaType == request.MediaType);

            if (existing != null)
            {
                // Mise à jour
                existing.RatingValue = request.Value;
                existing.RatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Note mise à jour.", rating = request.Value });
            }
            else
            {
                // Création
                var newRating = new RatingUser
                {
                    UserId = userId,
                    MediaId = request.MediaId,
                    MediaType = request.MediaType,
                    RatingValue = request.Value,
                    RatedAt = DateTime.UtcNow
                };

                // Récupération des infos TMDB pour le cache local
                if (request.MediaType == "movie")
                {
                    var m = await _tmdbService.GetMovieDetailAsync(request.MediaId);
                    if (m != null)
                    {
                        newRating.Title = m.Title;
                        newRating.Overview = m.Overview;
                        newRating.PosterPath = m.PosterPath;
                        newRating.BackdropPath = m.BackdropPath;
                        newRating.VoteAverage = m.VoteAverage;
                        newRating.ReleaseDate = m.ReleaseDate;
                    }
                }
                else // tv
                {
                    var s = await _tmdbService.GetTvShowDetailAsync(request.MediaId);
                    if (s != null)
                    {
                        newRating.Title = s.Name;
                        newRating.Overview = s.Overview;
                        newRating.PosterPath = s.PosterPath;
                        newRating.BackdropPath = s.BackdropPath;
                        newRating.VoteAverage = s.VoteAverage;
                        newRating.ReleaseDate = s.FirstAirDate;
                    }
                }

                _context.RatingUsers.Add(newRating);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Film noté.", rating = request.Value });
            }
        }

       // GET api/v1/Ratings?page=1&type=tv
        [HttpGet]
        public async Task<ActionResult> GetRatedMovies([FromQuery] int page = 1, [FromQuery] string type = "movie")
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            int pageSize = 20;

            // 1. On prépare la requête BDD
            var query = _context.RatingUsers.Where(r => r.UserId == userId);

            // On filtre selon le type demandé (movie ou tv)
            if (!string.IsNullOrEmpty(type) && type != "all")
            {
                query = query.Where(r => r.MediaType == type);
            }

            // 2. Pagination
            int totalResults = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            // 3. Récupération des données brutes
            var ratingsList = await query
                .OrderByDescending(r => r.RatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. TRANSFORMATION INTELLIGENTE (C'est ici que ça change)
            object finalResults;

            if (type == "tv")
            {
                // Si c'est "tv", on utilise le DTO avec "Name" et "FirstAirDate"
                finalResults = ratingsList.Select(r => new RatingDtoTvUser
                {
                    Id = r.MediaId,
                    Name = r.Title,                 // Title de la BDD va dans Name
                    OriginalName = r.Title,
                    Overview = r.Overview,
                    PosterPath = r.PosterPath,
                    BackdropPath = r.BackdropPath,
                    VoteAverage = r.VoteAverage,
                    FirstAirDate = r.ReleaseDate,   // ReleaseDate de la BDD va dans FirstAirDate
                    Rating = r.RatingValue
                });
            }
            else
            {
                // Si c'est "movie", on garde le DTO classique avec "Title" et "ReleaseDate"
                finalResults = ratingsList.Select(r => new RatingDtoUser
                {
                    Id = r.MediaId,
                    Title = r.Title,
                    OriginalTitle = r.Title,
                    Overview = r.Overview,
                    PosterPath = r.PosterPath,
                    BackdropPath = r.BackdropPath,
                    VoteAverage = r.VoteAverage,
                    ReleaseDate = r.ReleaseDate,
                    Rating = r.RatingValue
                });
            }

            return Ok(new
            {
                page = page,
                results = finalResults, // Contiendra soit des Films, soit des Séries
                total_pages = totalPages,
                total_results = totalResults
            });
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}