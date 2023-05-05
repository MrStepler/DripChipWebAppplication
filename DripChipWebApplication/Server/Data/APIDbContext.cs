using Microsoft.EntityFrameworkCore;
using DripChipWebApplication.Server.Models;
using Microsoft.AspNetCore.Connections;

namespace DripChipWebApplication.Server.Data
{
    public class APIDbContext : DbContext
    {
 
       
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalLocation> Locations { get; set; }
        public DbSet<AnimalVisitedLocation> VisitedLocations { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public APIDbContext(DbContextOptions<APIDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbFilePath = Path.Combine(Environment.CurrentDirectory, "ApiDB.db");
            optionsBuilder.UseSqlite($"Data Source = {dbFilePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .HasKey(p => new { p.Id });
            modelBuilder.Entity<Animal>()
            .HasKey(p => new { p.Id });
            modelBuilder.Entity<AnimalType>()
            .HasKey(p => new { p.Id });
            modelBuilder.Entity<AnimalLocation>()
            .HasKey(p => new { p.Id });
            modelBuilder.Entity<AnimalVisitedLocation>()
            .HasKey(p => new { p.Id });
        }
    }
}
