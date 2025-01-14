using Microsoft.EntityFrameworkCore;

namespace BazaMuzyczna.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }
        public DbSet<User> User { get; set; }
        public DbSet<Track> Track { get; set; }
        public DbSet<Album> Album { get; set; }
        public DbSet<Playback> Playback { get; set; }
        public DbSet<Genre> Genre { get; set; }

    }
}
