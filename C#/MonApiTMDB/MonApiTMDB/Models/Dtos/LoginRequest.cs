namespace MonApiTMDB.Models.Dtos;

public class LoginRequest
{
    // On garde uniquement ce que l'utilisateur tape dans le formulaire
    public string Username { get; set; } 
    public string Password { get; set; }
}