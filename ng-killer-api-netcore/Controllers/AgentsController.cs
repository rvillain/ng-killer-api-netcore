using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class AgentsController : ApiController<Agent, string, KillerContext>
    {
        public AgentsController(KillerContext context) : base(context)
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
            Agent item = Context.Set<Agent>()
                .Include(a => a.Game)
                .Include(a => a.Mission)
                .Include(a => a.Target)
                .First(g => g.Id == id);
            if (item == null)
            {
                return NotFound();
            }
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