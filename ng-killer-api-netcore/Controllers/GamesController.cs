using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : ApiController<Game, long, KillerContext>
    {
        public GamesController(KillerContext context) : base(context)
        {
            Includes.Add(g => g.Agents);

            if (!context.Agents.Any())
            {
                context.Agents.Add(new Agent { Id = "666", Name = "Un petit agent", Status = "Drunk", GameId = 999 });
                context.Games.Add(new Game { Id = 999, Name = "la petite game" });
                //context.Missions.Add(new Mission { Id = 333, Title = "la petite mission" });
                context.SaveChanges();
            }
        }
    }
}