using Microsoft.EntityFrameworkCore;
using MonApiTMDB.Models;

namespace MonApiTMDB.Data
{
    // On repasse en DbContext classique pour éviter le conflit avec votre AuthController
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}