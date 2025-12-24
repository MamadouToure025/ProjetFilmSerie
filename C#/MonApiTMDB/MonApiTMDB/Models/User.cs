namespace MonApiTMDB.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } // <--- Vérifiez que cette ligne existe
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; // <--- Et celle-ci
    }
}