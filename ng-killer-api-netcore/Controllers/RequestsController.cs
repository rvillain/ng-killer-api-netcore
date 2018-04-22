using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Contrôleur des requêtes émisent par les joueurs
    /// </summary>
    [Route("api/[controller]")]
    public class RequestsController : ApiController<Request, long, KillerContext, RequestHub>
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        public RequestsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
        }

        /// <summary>
        /// Envoyer une nouvelle requête
        /// </summary>
        /// <param name="req">
        /// Types:
        /// "agent-update";
        /// "change-mission";
        /// "suicide";
        /// "new-action";
        /// "new-agent";
        /// "game-status";
        /// "action-error";
        /// "tribunal-status";
        /// </param>
        /// <returns></returns>
        [HttpPost("push")]
        [EnableCors("AllowAll")]
        public IActionResult Push([FromBody]Request req)
        {
            if (req == null)
            {
                throw new Exception();
            }
            try
            {
                switch (req.Type)
                {
                    case Constantes.REQUEST_TYPE_CHANGE_MISSION:
                        if (!this.ChangeMission(req.EmitterId))
                        {
                            return Ok(req);
                        }
                        req.IsTreated = true;
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_KILL:
                        var killerToUpdate = Context.Agents.Include(a => a.Mission).First(a => a.TargetId == req.EmitterId);
                        req.ReceiverId = killerToUpdate.Id;
                        this.Kill(req.EmitterId, killerToUpdate);
                        req.IsTreated = true;
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_UNMASK:
                        this.Unmask(req.EmitterId);
                        req.IsTreated = true;
                        break;
                    case Constantes.REQUEST_TYPE_SUICIDE:
                        this.Suicide(req.EmitterId);
                        req.IsTreated = true;
                        break;
                    case Constantes.REQUEST_TYPE_ASK_UNMASK:
                        if (CheckFinal(req.GameId))
                        {
                            throw new Exception(string.Format("Impossible de démasquer lorsqu'il ne reste que {0} joueurs", Constantes.FINAL_NB));
                        }
                        if (!this.AskUnmask(req.EmitterId, req.ReceiverId))
                        {
                            Context.SaveChanges();
                            this.SendToAll(req.GameId.ToString(), Constantes.REQUEST_METHOD_NAME, new Request()
                            {
                                Type = Constantes.REQUEST_TYPE_WRONG_KILLER,
                                GameId = req.GameId,
                                IsTreated = true
                            });
                            throw new Exception("Non, ce n'est pas votre killer");
                        }
                        break;
                }

                if (req.ParentRequestId > 0)
                {
                    var treatedRequest = Context.Requests.AsNoTracking().First(r=>r.Id == req.ParentRequestId);
                    treatedRequest.IsTreated = true;
                    Context.Update(treatedRequest);
                }
                Context.Add(req);
                CheckGameEnd(req.GameId);
                Context.SaveChanges();

                req.Game = null;
                req.Emitter = null;
                req.Receiver = null;
                req.ParentRequest = null;
                this.SendToAll(req.GameId.ToString(), Constantes.REQUEST_METHOD_NAME, req);
            }
            catch (Exception ex)
            {
                return StatusCode(406, ex.Message);
            }


            return Ok(req);
        }


        
        private bool AskUnmask(string emitterId, string receiverId)
        {
            var emitterToUpdate = Context.Agents.Find(emitterId);
            var receiverToUpdate = Context.Agents.Find(receiverId);

            var aliveAgents = Context.Agents.Count(a => a.Status == "alive" && a.GameId == emitterToUpdate.GameId);
            if (aliveAgents > Constantes.FINAL_NB)
            {
                if (receiverToUpdate.TargetId == emitterToUpdate.Id)
                {
                    return true;
                }
                else
                {
                    this.WrongKiller(emitterToUpdate, receiverId);
                }
            }
            return false;
        }

        private bool ChangeMission(string agentId)
        {
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agentId);
            if(agentToUpdate.Life == 1)
            {
                throw new Exception("Vous n'avez pas assez de points de vie");
            }
            var missions = this.Context.Missions.AsNoTracking().Where(m => m.GameId == agentToUpdate.GameId && !m.IsUsed).ToList();
            if (missions.Count > 0)
            {
                var mission = missions.First();
                mission.IsUsed = true;
                this.Context.Update(mission);
                agentToUpdate.MissionId = mission.Id;
                agentToUpdate.Life--;
                this.Context.Update(agentToUpdate);
                return true;
            }
            else
            {
                throw new Exception("Il n'y a plus aucune mission disponible");
            }
        }
        private void Suicide(string agentId)
        {
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agentId);
            agentToUpdate.Life = 0;
            agentToUpdate.Status = Constantes.AGENT_STATUS_DEAD;
            var killer = this.Context.Agents.First(a => a.TargetId == agentId);
            killer.TargetId = agentToUpdate.TargetId;
            Context.Update(agentToUpdate);
            Context.Update(killer);
            Models.Action action = new Models.Action()
            {
                GameId = agentToUpdate.GameId,
                KillerId = agentToUpdate.Id,
                KillerName = agentToUpdate.Name,
                Type = Constantes.ACTTION_TYPE_SUICIDE
            };
            this.Context.Actions.Add(action);
            
            this.SendToAll(agentToUpdate.GameId.ToString(), Constantes.REQUEST_METHOD_NAME, new Request(){
                Type = Constantes.REQUEST_TYPE_AGENT_UPDATE,
                ReceiverId = killer.Id,
                IsTreated = true
            });
        }

        private void Kill(string victimId, Agent killerToUpdate)
        {
            var mission = killerToUpdate.Mission;
            var victimToUpdate = this.Context.Agents.First(a => a.Id == victimId);
            var newTarget = this.Context.Agents.First(a => a.Id == victimToUpdate.TargetId);

            killerToUpdate.MissionId = victimToUpdate.MissionId;
            killerToUpdate.TargetId = newTarget.Id;
            if (killerToUpdate.Life < 5)
                killerToUpdate.Life++;

            victimToUpdate.Life = 0;
            victimToUpdate.Status = "dead";
            victimToUpdate.MissionId = null;
            victimToUpdate.TargetId = null;

            var action = new Models.Action()
            {
                GameId = killerToUpdate.GameId,
                TargetId = victimToUpdate.Id,
                TargetName = victimToUpdate.Name,
                KillerId = killerToUpdate.Id,
                KillerName = killerToUpdate.Name,
                Type = Constantes.ACTTION_TYPE_KILL,
                MissionId = mission.Id,
                MissionTitle = mission.Title
            };

            this.Context.Agents.UpdateRange(new List<Agent> { killerToUpdate, victimToUpdate });
            this.Context.Actions.Add(action);
        }

        private void Unmask(string victimId)
        {
            // Killer => Victim => Target
            //Target unmask victim
            var victimToUpdate = this.Context.Agents.First(a => a.Id == victimId);
            var target = this.Context.Agents.First(a => a.Id == victimToUpdate.TargetId);
            var killer = this.Context.Agents.First(a => a.TargetId == victimId);

            killer.TargetId = target.Id;
            if (target.Life < 5)
                target.Life++;

            victimToUpdate.Life = 0;
            victimToUpdate.Status = "dead";
            victimToUpdate.MissionId = null;
            victimToUpdate.TargetId = null;

            var action = new Models.Action()
            {
                GameId = victimToUpdate.GameId,
                TargetId = victimToUpdate.Id,
                TargetName = victimToUpdate.Name,
                KillerId = target.Id,
                KillerName = target.Name,
                Type = Constantes.ACTTION_TYPE_UNMASK
            };

            this.Context.Agents.UpdateRange(new List<Agent> { victimToUpdate, killer, target });
            this.Context.Actions.Add(action);
        }

        private void WrongKiller(Agent agentToUpdate, string blufferId)
        {
            agentToUpdate.Life--;
            if (agentToUpdate.Life <= 0)
            {
                agentToUpdate.Status = "dead";
                var killer = this.Context.Agents.First(a => a.TargetId == agentToUpdate.Id);
                killer.TargetId = agentToUpdate.TargetId;
                agentToUpdate.TargetId = null;
                agentToUpdate.MissionId = null;
                var action = new Models.Action()
                {
                    GameId = agentToUpdate.GameId,
                    TargetId = blufferId,
                    KillerId = agentToUpdate.Id,
                    KillerName = agentToUpdate.Name,
                    Type = Constantes.ACTTION_TYPE_ERROR_DEATH
                };

                this.Context.Agents.UpdateRange(new List<Agent> { agentToUpdate, killer });
                this.Context.Actions.Add(action);
            }
            else
            {
                var action = new Models.Action()
                {
                    GameId = agentToUpdate.GameId,
                    TargetId = blufferId,
                    KillerId = agentToUpdate.Id,
                    KillerName = agentToUpdate.Name,
                    Type = Constantes.ACTTION_TYPE_WRONG_KILLER
                };

                this.Context.Agents.UpdateRange(new List<Agent> { agentToUpdate });
                this.Context.Actions.Add(action);
            }
        }

        private void CheckGameEnd(long GameId)
        {
            var game = Context.Games.Include(g => g.Agents).First(g => g.Id == GameId);
            if (game.Agents.Count(a => a.Status == Constantes.AGENT_STATUS_ALIVE) == 1 && game.Status == Constantes.GAME_STATUS_STARTED)
            {
                game.Status = Constantes.GAME_STATUS_FINISHED;
                var lastAgent = game.Agents.First(a=>a.Status == Constantes.AGENT_STATUS_ALIVE);
                Models.Action action = new Models.Action()
                {
                    GameId = GameId,
                    Type = Constantes.ACTTION_TYPE_END,
                    KillerId = lastAgent.Id,
                    KillerName = lastAgent.Name
                };
                this.Context.Actions.Add(action);
                Context.Update(game);
            }
        }

        private bool CheckFinal(long GameId)
        {
            var game = Context.Games.Include(g => g.Agents).First(g => g.Id == GameId);
            if (game.Agents.Count(a => a.Status == Constantes.AGENT_STATUS_ALIVE) <= Constantes.FINAL_NB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}