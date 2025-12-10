using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

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
    }
}