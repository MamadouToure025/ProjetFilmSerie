using System.ComponentModel.DataAnnotations;

namespace MonApiTMDB.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Votre ID est un int
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}