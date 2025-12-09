using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public CollectionsController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // 1. RECHERCHE
        [HttpGet("search")]
        public async Task<ActionResult<CollectionResponse>> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Le paramètre 'query' est obligatoire.");
            try
            {
                var result = await _tmdbService.SearchCollectionAsync(query, page);
                if (result == null || result.Results.Count == 0) return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 2. DÉTAILS PAR ID
        // 👇 CORRECTION CRUCIALE ICI : {id:int}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CollectionDetail>> GetCollectionDetails(int id)
        {
            try
            {
                var collection = await _tmdbService.GetCollectionDetailsAsync(id);
                if (collection == null) return NotFound($"La collection avec l'ID {id} est introuvable.");
                return Ok(collection);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}