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

        public DbSet<Action> Actions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Request> Requests { get; set; }

    }
}