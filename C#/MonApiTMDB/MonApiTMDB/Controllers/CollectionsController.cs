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
        public CollectionsController(ITmdbService tmdbService) { _tmdbService = tmdbService; }

        [HttpGet("recherche")]
        public async Task<ActionResult<CollectionResponse>> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query obligatoire.");
            try { return Ok(await _tmdbService.SearchCollectionAsync(query, page)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // CORRECTION ICI : {id:int}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CollectionDetail>> GetCollectionDetails(int id)
        {
            try {
                var c = await _tmdbService.GetCollectionDetailsAsync(id);
                return c != null ? Ok(c) : NotFound();
            } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}