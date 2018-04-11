using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class AgentsController : Controller
    {
        private const string GetAgentRouteName = "GetAgent";
        private readonly KillerContext _context;


        public AgentsController(KillerContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<Agent> GetAll()
        {
            return _context.Agents.ToList();
        }

        [HttpGet("{id}", Name = GetAgentRouteName)]
        public IActionResult Get(string id)
        {
            return new ObjectResult(_context.Agents.Find(id));
        }

        [HttpPost]
        public IActionResult Create([FromBody]Agent agent)
        {
            if (agent == null)
            {
                return BadRequest();
            }
            // TODO valider les données de l'agent
            _context.Agents.Add(agent);
            _context.SaveChanges();
            return CreatedAtRoute(GetAgentRouteName, new {id = agent.Id}, agent);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Agent agent)
        {
            if (agent == null || agent.Id != id)
            {
                return BadRequest();
            }

            var agentDb = _context.Agents.FirstOrDefault(t => t.Id == id);
            if (agentDb == null)
            {
                return NotFound();
            }

            agentDb.Status = agent.Status;

            _context.Agents.Update(agentDb);
            _context.SaveChanges();
            return new NoContentResult();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var agentDb = _context.Agents.FirstOrDefault(t => t.Id == id);
            if (agentDb == null)
            {
                return NotFound();
            }
            _context.Agents.Remove(agentDb);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}