using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class MissionsController : Controller
    {
        private const string GetMissionRouteName = "GetMission";
        private readonly KillerContext _context;


        public MissionsController(KillerContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<Mission> GetAll()
        {
            return _context.Missions.ToList();
        }

        [HttpGet("{id}", Name = GetMissionRouteName)]
        public IActionResult Get(string id)
        {
            return new ObjectResult(_context.Missions.Find(id));
        }

        [HttpPost]
        public IActionResult Create([FromBody]Mission mission)
        {
            if (mission == null)
            {
                return BadRequest();
            }
            // TODO valider les données de l'agent
            _context.Missions.Add(mission);
            _context.SaveChanges();
            return CreatedAtRoute(GetMissionRouteName, new {id = mission.Id}, mission);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]Mission mission)
        {
            if (mission == null || mission.Id != id)
            {
                return BadRequest();
            }

            var missionDb = _context.Missions.FirstOrDefault(t => t.Id == id);
            if (missionDb == null)
            {
                return NotFound();
            }

            //TODO affectation des champs modifiables

            _context.Missions.Update(missionDb);
            _context.SaveChanges();
            return new NoContentResult();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var missionDb = _context.Missions.FirstOrDefault(t => t.Id == id);
            if (missionDb == null)
            {
                return NotFound();
            }
            _context.Missions.Remove(missionDb);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}