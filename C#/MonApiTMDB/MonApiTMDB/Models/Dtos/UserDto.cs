namespace MonApiTMDB.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        
        // Ces deux lignes sont obligatoires pour enlever le rouge à gauche
        public string Username { get; set; } 
        public string Role { get; set; }

        public string Email { get; set; }
    }
}