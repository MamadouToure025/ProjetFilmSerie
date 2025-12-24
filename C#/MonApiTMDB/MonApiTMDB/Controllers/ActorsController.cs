using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Models;
using MonApiTMDB.Services;

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;

        public ActorsController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // ==========================================
        // 1. RECHERCHER UN ACTEUR
        // GET api/v1/Actors/search?query=Brad
        // ==========================================
        [HttpGet("search")]
        public async Task<ActionResult<List<PersonDetail>>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Veuillez saisir un nom.");

            var response = await _tmdbService.SearchActorsAsync(query);
            return Ok(response?.Results ?? new List<PersonDetail>());
        }

        // ==========================================
        // 2. DÉTAILS D'UN ACTEUR (Avec ses films)
        // GET api/v1/Actors/117642
        // ==========================================
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDetail>> GetActor(int id)
        {
            var actor = await _tmdbService.GetPersonDetailAsync(id);

            if (actor == null)
            {
                return NotFound("Acteur introuvable.");
            }

            return Ok(actor);
        }
    }
}