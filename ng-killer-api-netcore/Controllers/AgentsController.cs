using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        }

        protected override void UpdateRange(Agent dbItem, Agent item)
        {
            dbItem.Name = item.Name;
            dbItem.Photo = item.Photo;
            dbItem.Status = item.Status;
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