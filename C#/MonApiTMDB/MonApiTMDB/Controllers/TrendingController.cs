using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrendingController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public TrendingController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // GET: api/trending
        [HttpGet]
        public async Task<ActionResult<TrendingViewModel>> GetAllTrends()
        {
            try
            {
                // 1. Lancement des tâches en parallèle pour la performance
                var moviesTask = _tmdbService.GetTrendingMoviesAsync();
                var tvTask = _tmdbService.GetTrendingTvShowsAsync();
                var peopleTask = _tmdbService.GetTrendingPeopleAsync(); // Retourne maintenant ActorsResponse

                // 2. Attente de la fin de tous les appels
                await Task.WhenAll(moviesTask, tvTask, peopleTask);

                // 3. Construction de l'objet unique de réponse
                var viewModel = new TrendingViewModel
                {
                    // On récupère la liste .Results, ou une liste vide si null
                    Movies = (await moviesTask)?.Results ?? new List<Movie>(),
                    TvShows = (await tvTask)?.Results ?? new List<TvShow>(),
                    People = (await peopleTask)?.Results ?? new List<Person>()
                };

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                // En production, il vaut mieux logger l'erreur plutôt que de l'afficher
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }
    }
}