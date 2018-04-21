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

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<Models.Action> Actions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}