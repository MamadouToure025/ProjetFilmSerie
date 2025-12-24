using System.ComponentModel.DataAnnotations;

namespace MonApiTMDB.Models.Dtos;

public class RegisterRequest
{
    // 1. Règle : Format standard d'email
    [Required(ErrorMessage = "L'email est requis.")]
    [EmailAddress(ErrorMessage = "L'adresse mail doit respecter le format standard.")]
    public string Email { get; set; }

    // 2. Règle : Pas de caractères spéciaux (Uniquement Lettres et Chiffres)
    [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", 
        ErrorMessage = "Le nom d'utilisateur ne peut contenir de caractères spéciaux.")]
    public string Username { get; set; }

    // 3. Règle : 1 Majuscule, 1 Minuscule, 1 Chiffre (et min 6 caractères par sécurité)
    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
        ErrorMessage = "Le mot de passe doit au moins contenir 1 majuscule, 1 minuscule et 1 chiffre.")]
    public string Password { get; set; }

    // Bonus : Confirmation du mot de passe
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; }
}