using NgKillerApiCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NgKillerApiCore.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(KillerContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Games.Any())
            {
                return;   // DB has been seeded
            }
            string defaultAgentStatus = "alive";
            string defaultGameStatus = "created";

            var game = new Game { Name = "la petite game", Status = defaultGameStatus };
            context.Games.Add(game);
            context.SaveChanges();

            for (int i = 1; i <= 20; i++)
            {
                context.Agents.Add(new Agent { Name = "Agent " + i, Status = defaultAgentStatus, GameId = game.Id });

                context.Missions.Add(new Mission { Title = "Une petite mission " + i, GameId = game.Id });
            }

            context.SaveChanges();
        }
    }
}
