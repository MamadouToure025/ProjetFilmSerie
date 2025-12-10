using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;
        public ActorsController(ITmdbService tmdbService) { _tmdbService = tmdbService; }

        [HttpGet("recherche")]
        public async Task<ActionResult<ActorsResponse>> Search([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Nom obligatoire.");
            try { return Ok(await _tmdbService.SearchPersonAsync(query, page)); } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // CORRECTION ICI : {id:int}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonDetail>> GetPerson(int id)
        {
            try {
                var p = await _tmdbService.GetPersonDetailAsync(id);
                return p != null ? Ok(p) : NotFound();
            } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}