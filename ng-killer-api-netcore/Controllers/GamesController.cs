using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : ApiController<Game, long, KillerContext, RequestHub>
    {
        public GamesController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
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

        public override IActionResult GetById(long id)
        {
            Game item = Context.Set<Game>()
                .Include(g => g.Missions)
                .Include(g => g.Agents)
                .Include(g => g.Actions)
                .First(g => g.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        private ICollection<T> Shuffle<T>(ICollection<T> o)
        {
            Random rnd = new Random();
            return o.OrderBy(x => rnd.Next()).ToList();
        }

        [HttpPost("{id}/start")]
        public Game StartAGame(long id)
        {
            Game game = Context.Set<Game>()
                .Include(g => g.Missions)
                .Include(g => g.Agents)
                .Include(g => g.Actions)
                .First(g => g.Id == id);

            var randomOrderedAgents = Shuffle<Agent>(game.Agents);
            var randomOrderedMissions = Shuffle<Mission>(game.Missions);

            for (var i = 0; i < randomOrderedAgents.Count; i++)
            {
                var sourceAgent = randomOrderedAgents.ElementAt(i);
                var targetAgent = randomOrderedAgents.ElementAt((i == (game.Agents.Count - 1)) ? 0 : (i + 1));

                var mission = randomOrderedMissions.ElementAt(i);

                sourceAgent.TargetId = targetAgent.Id;
                sourceAgent.MissionId = mission.Id;
                sourceAgent.Status = "alive";
                sourceAgent.Life = 3;

                Context.Agents.Update(sourceAgent);

                //mission.agentId
                mission.IsUsed = true;
                Context.Missions.Update(mission);
            }

            game.Status = "started";
            Context.Games.Update(game);

            var action = new Models.Action
            {
                GameId = game.Id,
                Type = Constantes.ACTTION_TYPE_GAME_STARTED
            };
            Context.Actions.Add(action);

            Context.SaveChanges();
            this.SendToAll(game.Id.ToString(),"Request", new Request
            {
                Type = Constantes.REQUEST_TYPE_GAME_STATUS
            });
            return game;
        }
        [HttpPost("{id}/reinit")]
        public Game ReinitAGame(long id)
        {
            Game game = Context.Set<Game>()
                .Include(g => g.Missions)
                .Include(g => g.Agents)
                .Include(g => g.Actions)
                .First(g => g.Id == id);

            game.Status = "created";

            foreach (var agent in game.Agents)
            {
                agent.Status = "alive";
                agent.MissionId = null;
                agent.TargetId = null;
                agent.Life = 3;
            }
            foreach (var mission in game.Missions)
            {
                mission.IsUsed = false;
            }

            Context.Actions.RemoveRange(game.Actions);
            Context.Games.Update(game);


            Context.SaveChanges();
            return game;
        }

        //[HttpPost("{id}/missions")]
        //exports.add_missions = function(req, res)
        //{
        //    var gameId = req.params.gameId;
        //    var missions = req.body.missions;
        //    missions.forEach(function(mission) {
        //        var new_mission = new Mission();
        //        new_mission.title = mission.title;
        //        new_mission.difficulty = mission.difficulty;
        //        new_mission.game = gameId;
        //        new_mission.save(function(err, game) { });
        //    }, this);
        //    exports.read_a_game(req, res);
        //}
    }
}