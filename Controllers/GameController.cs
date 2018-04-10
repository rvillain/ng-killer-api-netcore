using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ng_killer_api_netcore.Models;
using System.Linq;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly KillerContext _context;

        public GamesController(KillerContext context)
        {
            _context = context;

            if (_context.Games.Count() == 0)
            {
                _context.Games.Add(new Game { Name = "G1" });
                _context.SaveChanges();
            }
        }
        [HttpGet]
        public IEnumerable<Game> GetAll()
        {
            return _context.Games.ToList();
        }

        [HttpGet("{id}", Name = "GetGame")]
        public IActionResult GetById(long id)
        {
            var item = _context.Games.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
        [HttpPost]
        public IActionResult Create([FromBody] Game item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.Games.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetGame", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Game item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var game = _context.Games.FirstOrDefault(t => t.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            game.Status = item.Status;

            _context.Games.Update(game);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.Games.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            _context.Games.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}