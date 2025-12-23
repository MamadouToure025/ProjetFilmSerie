using Microsoft.EntityFrameworkCore;
using MonApiTMDB.Models;

namespace MonApiTMDB.Data
{
    // Pas de IdentityDbContext ici, juste DbContext
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}