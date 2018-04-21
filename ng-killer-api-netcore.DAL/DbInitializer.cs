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

            context.Agents.Add(new Agent {  Name = "Un petit agent", Status = defaultAgentStatus, GameId = game.Id });
            context.Agents.Add(new Agent {  Name = "Un petit agent 2", Status = defaultAgentStatus, GameId = game.Id });
            context.Agents.Add(new Agent {  Name = "Un petit agent 3", Status = defaultAgentStatus, GameId = game.Id });
            context.Agents.Add(new Agent {  Name = "Un petit agent 4", Status = defaultAgentStatus, GameId = game.Id });
            context.Agents.Add(new Agent {  Name = "Un petit agent 5", Status = defaultAgentStatus, GameId = game.Id });
            context.Agents.Add(new Agent {  Name = "Un petit agent 6", Status = defaultAgentStatus, GameId = game.Id });

            context.Missions.Add(new Mission { Title = "Une petite mission", GameId = game.Id });
            context.Missions.Add(new Mission { Title = "Une petite mission 2", GameId = game.Id });
            context.Missions.Add(new Mission { Title = "Une petite mission 3", GameId = game.Id });
            context.Missions.Add(new Mission { Title = "Une petite mission 4", GameId = game.Id });
            context.Missions.Add(new Mission { Title = "Une petite mission 5", GameId = game.Id });
            context.Missions.Add(new Mission { Title = "Une petite mission 6", GameId = game.Id });
            context.SaveChanges();
        }
    }
}
