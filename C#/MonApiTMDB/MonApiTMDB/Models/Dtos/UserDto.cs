namespace MonApiTMDB.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; } // Pour voir si c'est un Admin ou User
        
    }
}