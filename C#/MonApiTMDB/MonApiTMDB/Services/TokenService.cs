using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MonApiTMDB.Models; // <--- Indispensable pour reconnaître la classe 'User'
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MonApiTMDB.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // CHANGEMENT MAJEUR : On prend l'objet 'User' (de la BDD) en paramètre
    public string GenerateToken(User user)
    {
        // 1. Récupérer les infos de config
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKeyString = jwtSettings["SecretKey"];
        
        if (string.IsNullOrEmpty(secretKeyString)) throw new Exception("La clé JWT n'est pas configurée dans appsettings.json !");

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
        
        // 2. Créer les Claims à partir des VRAIES données de la BDD
        var claims = new List<Claim>
        {
            // L'ID unique de l'utilisateur (utile pour les requêtes futures)
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            
            // Le pseudo
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            
            // L'email (si vous en avez besoin)
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            
            // LE ROLE : On prend celui qui est stocké en base (Admin ou User)
            new Claim(ClaimTypes.Role, user.Role), 
            
            // Identifiant unique du token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 3. Signature et Création
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // Durée de validité (par défaut 60 min)
        var expirationMinutes = double.TryParse(jwtSettings["ExpirationInMinutes"], out var min) ? min : 60;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}