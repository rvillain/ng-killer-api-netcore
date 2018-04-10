

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ng_killer_api_netcore.Models
{
    public class KillerContext : DbContext
    {
        public KillerContext(DbContextOptions<KillerContext> options)
            : base(options)
        {
        }

        public DbSet<Action> Actions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Game> Missions { get; set; }
        public DbSet<Event> Events { get; set; }

    }
}