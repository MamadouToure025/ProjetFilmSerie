using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonApiTMDB.Data;
using MonApiTMDB.Models;       // <--- INDISPENSABLE pour "User"
using MonApiTMDB.Models.Dtos;  // <--- INDISPENSABLE pour "CreateAdminDto"
using System.Security.Claims;namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    // SÉCURITÉ MAXIMALE : Seul un Admin peut entrer ici
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. LISTER TOUS LES UTILISATEURS
        // GET api/v1/Users
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            // On récupère tout le monde depuis la BDD
            var users = await _context.Users.ToListAsync();

            // On transforme en DTO (pour cacher les mots de passe)
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });

            return Ok(userDtos);
        }
// ==========================================
        // 4. CRÉER UN NOUVEL ADMIN (Réservé aux Admins)
        // POST api/v1/Users/admin
        // ==========================================
        [HttpPost("admin")]
        [Authorize(Roles = "Admin")] // <--- SÉCURITÉ CRITIQUE
        public async Task<ActionResult> CreateAdmin([FromBody] CreateAdminDto request)
        {
            // 1. Vérifier si l'utilisateur ou l'email existe déjà
            if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            {
                return BadRequest("Ce nom d'utilisateur ou cet email est déjà utilisé.");
            }

            // 2. Création du nouvel Admin
            var newAdmin = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password, // Pensez à hacher le mot de passe dans un vrai projet
                Role = "Admin" // <--- ON FORCE LE RÔLE ADMIN ICI
            };

            // 3. Sauvegarde
            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"L'administrateur {newAdmin.Username} a été créé avec succès." });
        }
// ==========================================
        // 1. MODIFIER SON PROPRE PROFIL (Pour tout le monde)
        // PUT api/v1/Users/profile
        // ==========================================
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UserUpdateDto request)
        {
            // 1. Identifier l'utilisateur connecté via son Token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // 2. Récupérer l'utilisateur en BDD
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Utilisateur introuvable.");

            // 3. Mise à jour des champs (si fournis)
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
               
                if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != userId))
                {
                    return BadRequest("Ce nom d'utilisateur est déjà pris.");
                }
                user.Username = request.Username;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                // Note : Idéalement, il faudrait hacher le mot de passe ici
                user.Password = request.Password;
            }

            // 4. Sauvegarder
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil mis à jour avec succès.", username = user.Username });
        }

    // ==========================================
        // 2. SUPPRIMER UN UTILISATEUR
        // DELETE api/v1/Users/5
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            // 1. Chercher l'utilisateur à supprimer
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete == null) return NotFound("Utilisateur introuvable.");

            // 2. SÉCURITÉ : Qui est connecté ?
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(currentUserIdString, out int currentUserId))
            {
                // On empêche l'admin de se supprimer lui-même !
                if (currentUserId == id)
                {
                    return BadRequest("Vous ne pouvez pas supprimer votre propre compte administrateur.");
                }
            }

            // 3. Suppression
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"L'utilisateur {userToDelete.Username} a été supprimé avec succès." });
        }
    }
}