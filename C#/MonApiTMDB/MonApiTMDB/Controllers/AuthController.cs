using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Nécessaire pour FirstOrDefaultAsync
using MonApiTMDB.Data;
using MonApiTMDB.Models;
using MonApiTMDB.Models.Dtos;
using MonApiTMDB.Services;

// Alias pour éviter le conflit entre votre classe User (BDD) et la propriété User (Controller)
using UserEntity = MonApiTMDB.Models.User; 

namespace MonApiTMDB.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(TokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        // ==========================================
        // 1. INSCRIPTION (Register)
        // ==========================================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // A. Vérifier si Email OU Username existe déjà
            // C'est important de vérifier les deux pour éviter les doublons
            bool exists = await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username);
            
            if (exists)
            {
                return BadRequest("Cet email ou ce nom d'utilisateur est déjà pris.");
            }

            // B. HACHER LE MOT DE PASSE
            // Sécurité : On transforme le mot de passe en chaîne incompréhensible
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // C. Créer l'objet User
            var user = new UserEntity
            {
                Username = request.Username,
                Email = request.Email,
                Password = passwordHash // On stocke le hash !
            };

            // D. Sauvegarder en BD
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // On ne renvoie pas l'objet user complet (sécurité), juste un message
            return Ok(new { Message = "Inscription réussie !" });
        }

        // ==========================================
        // 2. CONNEXION (Login)
        // ==========================================
        [HttpPost("login")] // J'ai renommé en "login" (plus standard), mais vous pouvez garder "Auth"
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // A. Chercher l'utilisateur (par Username ou Email selon votre logique)
            // Ici on cherche par Username comme dans votre code précédent
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null)
            {
                return Unauthorized(new { Message = "Pseudo ou mot de passe incorrect." });
            }

            // B. VÉRIFIER LE MOT DE PASSE (CORRECTION MAJEURE)
            // On ne fait PAS : user.Password == loginRequest.Password
            // On utilise BCrypt pour comparer le mot de passe clair avec le hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Pseudo ou mot de passe incorrect." });
            }
            
            // C. Générer le Token JWT
            var tokenString = _tokenService.GenerateToken(user);
            
            // D. Créer le Cookie (Optionnel si vous utilisez le Header Authorization)
            Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Mettre à false si vous testez en HTTP sans certificat
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return Ok(new { Message = "Connexion réussie.", Token = tokenString });
        }

        // ==========================================
        // 3. VERIFICATION (IsConnected)
        // ==========================================
        [Authorize] 
        [HttpGet("IsConnected")] 
        public IActionResult IsConnected()
        {
            return Ok(new
            {
                Status = "Connecté",
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, 
                Username = User.Identity?.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            }); 
        }
        
        // ==========================================
        // 4. DECONNEXION (Logout)
        // ==========================================
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return Ok(new { Message = "Déconnexion réussie." });
        }
    }
}