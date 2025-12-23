using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonApiTMDB.Data;
using MonApiTMDB.Models.Dtos;
using MonApiTMDB.Services;
// Alias pour éviter le conflit entre votre User BDD et le User système
using UserEntity = MonApiTMDB.Models.User; 

namespace MonApiTMDB.Controllers;

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
    [HttpPost("Register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // 1. Vérifier si l'utilisateur existe déjà (par email ou username)
        if (_context.Users.Any(u => u.Email == request.Email || u.Username == request.Username))
        {
            return BadRequest("Cet email ou ce pseudo est déjà utilisé.");
        }

        // 2. Créer le nouvel utilisateur
        var newUser = new UserEntity
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password, // ⚠️ Note: En production, il faudrait hacher le mot de passe ici !
            Role = "User" // Par défaut, tout le monde est simple "User"
        };

        // 3. Sauvegarder dans la base de données
        _context.Users.Add(newUser);
        _context.SaveChanges();

        return Ok(new { Message = "Inscription réussie ! Vous pouvez maintenant vous connecter." });
    }

    // ==========================================
    // 2. CONNEXION (Login)
    // ==========================================
    [HttpPost("Auth")]
    [AllowAnonymous]
    public IActionResult Auth([FromBody] LoginRequest loginRequest)
    {
        // Chercher l'utilisateur dans la BDD
        var user = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username);

        // Vérifier le mot de passe
        if (user == null || user.Password != loginRequest.Password)
        {
            return Unauthorized(new { Message = "Pseudo ou mot de passe incorrect." });
        }
        
        // Générer le Token
        var tokenString = _tokenService.GenerateToken(user);
        
        // Créer le Cookie
        Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
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