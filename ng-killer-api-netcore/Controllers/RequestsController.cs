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
    [Route("api/[controller]")]
    public class RequestsController : ApiController<Request, long, KillerContext, RequestHub>
    {
        public RequestsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
        }

        [HttpPost("push")]
        [EnableCors("AllowAll")]
        public Request Push([FromBody]Request req)
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
                        this.ChangeMission(req.EmitterId);
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_KILL:
                        var killerToUpdate = Context.Agents.Include(a=>a.Mission).First(a=>a.TargetId == req.EmitterId);
                        req.ReceiverId = killerToUpdate.Id;
                        this.Kill(req.EmitterId, killerToUpdate);
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_UNMASK:
                        this.Unmask(req.EmitterId);
                        break;
                    case Constantes.REQUEST_TYPE_SUICIDE:
                        this.Suicide(req.EmitterId);
                        break;
                    case Constantes.REQUEST_TYPE_ASK_UNMASK:
                        this.AskUnmask(req.EmitterId, req.ReceiverId);
                        break;
                }

                if (req.ParentRequestId > 0)
                {
                    var treatedRequest = Context.Requests.Find(req.ParentRequestId);
                    treatedRequest.IsTreated = true;
                    Context.Update(treatedRequest);
                }
                Context.Add(req);
                Context.SaveChanges();
                this.SendToAll(req.GameId.ToString(), "Request", req);
            }
            catch(Exception)
            {
                
            }

            
            return req;
        }



        private bool AskUnmask(string emitterId, string receiverId)
        {
            var emitterToUpdate = Context.Agents.Find(emitterId);
            var receiverToUpdate = Context.Agents.Find(receiverId);

            var aliveAgents = Context.Agents.Count(a => a.Status == "alive" && a.GameId == emitterToUpdate.GameId);
            if (aliveAgents > 5)
            {
                if(receiverToUpdate.TargetId == emitterToUpdate.Id)
                {
                    return true;
                }
                else
                {
                    this.WrongKiller(emitterToUpdate);
                }
            }
            return false;
            //emit action error
        }

        private void ChangeMission(string agentId)
        {
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agentId);
            var missions = this.Context.Missions.Where(m => m.GameId == agentToUpdate.Game.Id && !m.IsUsed).ToList();
            if (missions.Count > 0)
            {
                var mission = missions.First();
                mission.IsUsed = true;
                this.Context.Update(mission);
                agentToUpdate.MissionId = mission.Id;
                agentToUpdate.Life--;
                this.Context.Update(agentToUpdate);
            }
        }
        private void Suicide(string agentId)
        {
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agentId);
            agentToUpdate.Life = 0;
            agentToUpdate.Status = "dead";
            var killer = this.Context.Agents.First(a => a.TargetId == agentId);
            killer.TargetId = agentToUpdate.TargetId;
            Models.Action action = new Models.Action()
            {
                GameId = agentToUpdate.GameId,
                KillerId = agentToUpdate.Id,
                Type = Constantes.ACTTION_TYPE_SUICIDE
            };
            this.Context.Actions.Add(action);

            this.Context.SaveChanges();
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
                KillerId = killerToUpdate.Id,
                Type = Constantes.ACTTION_TYPE_KILL,
                MissionId = mission.Id,
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
                KillerId = target.Id,
                Type = Constantes.ACTTION_TYPE_UNMASK
            };

            this.Context.Agents.UpdateRange(new List<Agent> { victimToUpdate, killer, target });
            this.Context.Actions.Add(action);
        }

        private void WrongKiller(Agent agentToUpdate)
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
                    TargetId = killer.Id,
                    KillerId = agentToUpdate.Id,
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
                    KillerId = agentToUpdate.Id,
                    Type = Constantes.ACTTION_TYPE_WRONG_KILLER
                };

                this.Context.Agents.UpdateRange(new List<Agent> { agentToUpdate });
                this.Context.Actions.Add(action);
            }
        }
    }
}