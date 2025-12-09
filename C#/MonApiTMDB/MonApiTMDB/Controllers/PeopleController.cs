using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public PeopleController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // 1. RECHERCHE PAR NOM
        [HttpGet("search")]
        public async Task<ActionResult<PersonResponse>> Search([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Le nom (query) est obligatoire.");

            try
            {
                var result = await _tmdbService.SearchPersonAsync(query, page);
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
        public async Task<ActionResult<PersonDetail>> GetPerson(int id)
        {
            try
            {
                var person = await _tmdbService.GetPersonDetailAsync(id);
                if (person == null) return NotFound($"Personne introuvable (ID {id}).");
                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}