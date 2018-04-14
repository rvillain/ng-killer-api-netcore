using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class AgentsController : ApiController<Agent, string, KillerContext, RequestHub>
    {
        public AgentsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
            if (!context.Agents.Any())
            {
                context.Agents.Add(new Agent {Id = "666", Name = "Un petit agent", Status = "Drunk", GameId = 999});
                context.Games.Add(new Game { Id = 999, Name = "la petite game" });
                //context.Missions.Add(new Mission { Id = 333, Title = "la petite mission" });
                context.SaveChanges();
            }

            Includes.Add(a => a.Game);
            //Includes.Add(a => a.Mission);
        }

        public override IActionResult GetById(string id)
        {
            Agent item = Context.Agents
                .Include(a => a.Game)
                .Include(a => a.Mission)
                .Include(a => a.Target)
                .Include(a => a.Requests)
                .First(g => g.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            //get only non treated request
            item.Requests = item.Requests.Where(r => !r.IsTreated).ToList();
            return new ObjectResult(item);
        }

        [HttpPost("{id}/getForUnmask")]
        public ICollection<Agent> GetForUnmask(string id)
        {
            var agent = Context.Agents.Find(id);
            var agents = Context.Agents.Where(a => a.Id != agent.Id && a.GameId == agent.GameId);

            return agents.ToList();
        }
    }
}