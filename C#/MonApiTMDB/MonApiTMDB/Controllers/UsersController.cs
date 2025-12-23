using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Important pour .ToListAsync()
using MonApiTMDB.Models.Dtos;

// Assurez-vous d'importer votre classe User personnalisée si vous en avez une (ex: ApplicationUser)
// Sinon, utilisez IdentityUser
using Microsoft.AspNetCore.Identity; 

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // <--- SÉCURITÉ : Seuls les Admins passent ici
    public class UsersController : ControllerBase
    {
        // Remplacez IdentityUser par votre classe (ex: ApplicationUser) si vous l'avez étendue
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // ==========================================
        // LISTER TOUS LES UTILISATEURS
        // URL : GET api/Users
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                // 1. Récupérer tous les users de la BDD
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDto>();

                // 2. Transformer chaque User en UserDto
                foreach (var user in users)
                {
                    // On récupère les rôles de cet utilisateur spécifique
                    var roles = await _userManager.GetRolesAsync(user);

                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles
                    });
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        // ==========================================
        // (OPTIONNEL) SUPPRIMER UN USER
        // URL : DELETE api/Users/{id}
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Utilisateur introuvable.");

            // Empêcher de se supprimer soi-même (optionnel mais conseillé)
            if (User.Identity?.Name == user.UserName)
                return BadRequest("Vous ne pouvez pas supprimer votre propre compte admin ici.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest("Erreur lors de la suppression.");

            return Ok(new { message = $"Utilisateur {user.UserName} supprimé." });
        }
    }
}