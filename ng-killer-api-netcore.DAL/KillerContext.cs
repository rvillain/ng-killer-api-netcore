using System;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.DAL
{
    public class KillerContext : DbContext
    {
        public KillerContext(DbContextOptions<KillerContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            // Migration
            //optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Killer;Integrated Security=True");
            //optionsBuilder.UseSqlServer("Data Source=MOONSERV;Initial Catalog=Killer;User ID=sa;Password=Quantipro69");
            optionsBuilder.UseSqlServer("Data Source=DISCOVERY;Initial Catalog=Killer;User ID=sa;Password=Quantipro69");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Game>().HasMany(_ => _.Agents).WithOne(_ => _.Game);
            //modelBuilder.Entity<Game>().HasMany(_ => _.Missions).WithOne(_ => _.Game);
            //modelBuilder.Entity<Game>().HasMany(_ => _.Actions).WithOne(_ => _.Game);
        }

        public DbSet<Models.Action> Actions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Request> Requests { get; set; }

        public void SaveChanges(Request req)
        {
            throw new NotImplementedException();
        }
    }
}