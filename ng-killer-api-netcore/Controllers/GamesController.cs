using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using System;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Contrôleur des parties
    /// </summary>
    [Route("api/[controller]")]
    public class GamesController : ApiController<Game, long, KillerContext, RequestHub>
    {
        /// <summary>
        /// Contrôleur des parties - Constructeur
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        public GamesController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
            Includes.Add(g => g.Agents);
        }

        /// <summary>
        /// Récupère la partie avec les missions, les agents et les actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IActionResult GetById(long id)
        {
            Game item = Context.Set<Game>().AsNoTracking()
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

        /// <summary>
        /// Import des missions génériques
        /// </summary>
        /// <param name="id"></param>
        /// <param name="difficulty"></param>
        [HttpPost("{id}/importmissions")]
        public void ImportMissions(long id,[FromBody] string difficulty)
        {
            var missions = Context.Missions.AsNoTracking().Where(m => m.Difficulty == difficulty && m.GameId == null).ToList();

            foreach(var mission in missions)
            {
                var newMission = new Mission
                {
                    Title = mission.Title,
                    Difficulty = difficulty,
                    GameId = id,
                    IsUsed = false
                };
                Context.Add(newMission);
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// Démarer une partie.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/start")]
        public Game StartAGame(long id)
        {
            Game game = Context.Set<Game>().AsNoTracking()
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
                sourceAgent.Status = Constantes.AGENT_STATUS_ALIVE;
                sourceAgent.Life = Constantes.AGENT_LIFE_AT_START;

                Context.Agents.Update(sourceAgent);

                //mission.agentId
                mission.IsUsed = true;
                Context.Missions.Update(mission);
            }

            game.Status = Constantes.GAME_STATUS_STARTED;
            Context.Games.Update(game);

            var action = new Models.Action()
            {
                GameId = game.Id,
                Type = Constantes.ACTTION_TYPE_GAME_STARTED
            };
            Context.Actions.Add(action);

            Context.SaveChanges();
            this.SendToAll(game.Id.ToString(),Constantes.REQUEST_METHOD_NAME, new Request
            {
                GameId = game.Id,
                Type = Constantes.REQUEST_TYPE_GAME_STATUS
            });
            return game;
        }

        /// <summary>
        /// Réinitialiser la partie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/reinit")]
        public Game ReinitAGame(long id)
        {
            Game game = Context.Set<Game>().AsNoTracking()
                .Include(g => g.Missions)
                .Include(g => g.Agents)
                .Include(g => g.Actions)
                .Include(g => g.Requests)
                .First(g => g.Id == id);

            game.Status = Constantes.GAME_STATUS_CREATED;

            foreach (var agent in game.Agents)
            {
                agent.Status = Constantes.AGENT_STATUS_ALIVE;
                agent.MissionId = null;
                agent.TargetId = null;
                agent.Life = Constantes.AGENT_LIFE_AT_START;

                Context.Update(agent);
            }
            foreach (var mission in game.Missions)
            {
                mission.IsUsed = false;

                Context.Update(mission);
            }

            Context.Actions.RemoveRange(game.Actions);
            Context.Requests.RemoveRange(game.Requests);
            Context.Games.Update(game);


            Context.SaveChanges();
            this.SendToAll(game.Id.ToString(), Constantes.REQUEST_METHOD_NAME, new Request
            {
                GameId = game.Id,
                Type = Constantes.REQUEST_TYPE_GAME_STATUS
            });
            return game;
        }
    }
}