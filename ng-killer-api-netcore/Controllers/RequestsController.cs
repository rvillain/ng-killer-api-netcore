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

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class RequestsController : ApiController<Request, long, KillerContext>
    {
        protected MessageHandler _messageHandler { get; set; }
        public RequestsController(KillerContext context, MessageHandler messageHandler) : base(context)
        {
            this._messageHandler = messageHandler;
        }

        [HttpPost("push")]
        [EnableCors("AllowAll")]
        public async Task<Request> Push([FromBody]Request req)
        {
            if (req == null)
            {
                throw new Exception();
            }


            try
            {
                Context.Add(req);

                switch (req.Type)
                {
                    case Constantes.REQUEST_TYPE_CHANGE_MISSION:
                        this.ChangeMission(req.Emitter.Id);
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_KILL:
                        this.Kill(req.Emitter);
                        break;
                    case Constantes.REQUEST_TYPE_CONFIRM_UNMASK:
                        this.Unmask(req.Emitter);
                        break;
                    case Constantes.REQUEST_TYPE_SUICIDE:
                        this.Suicide(req.Emitter);
                        break;
                    case Constantes.REQUEST_TYPE_ASK_UNMASK:
                        this.AskUnmask(req.Emitter.Id, req.Receiver.Name);
                        break;
                }


                Context.SaveChanges();
                await this._messageHandler.SendMessageToAllAsync(new WebSocketManager.Common.Message
                {
                    Data = JsonConvert.SerializeObject(req),
                    MessageType = WebSocketManager.Common.MessageType.Text
                });
            }
            catch(Exception ex)
            {

            }

            
            return req;
        }

        protected override void UpdateRange(Request dbItem, Request item)
        {
            throw new System.NotImplementedException();
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
        private void Suicide(Agent agent)
        {
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agent.Id);
            agentToUpdate.Life = 0;
            agentToUpdate.Status = "dead";
            var killer = this.Context.Agents.First(a => a.TargetId == agent.Id);
            killer.TargetId = agentToUpdate.TargetId;
            Models.Action action = new Models.Action
            {
                GameId = agent.GameId,
                KillerId = agent.Id,
                Type = Constantes.ACTTION_TYPE_SUICIDE
            };
            this.Context.Actions.Add(action);

            this.Context.SaveChanges();
        }

        private void Kill(Agent victim)
        {
            var killerToUpdate = this.Context.Agents.First(a => a.TargetId == victim.Id);
            var mission = killerToUpdate.Mission;
            var victimToUpdate = this.Context.Agents.First(a => a.Id == victim.Id);
            var newTarget = this.Context.Agents.First(a => a.Id == victimToUpdate.Id);

            killerToUpdate.MissionId = victimToUpdate.MissionId;
            killerToUpdate.TargetId = newTarget.Id;
            if (killerToUpdate.Life < 5)
                killerToUpdate.Life++;

            victimToUpdate.Life = 0;
            victimToUpdate.Status = "dead";
            victimToUpdate.MissionId = null;
            victimToUpdate.TargetId = null;

            var action = new Models.Action
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

        private void Unmask(Agent victim)
        {
            // Killer => Victim => Target
            //Target unmask victim
            var victimToUpdate = this.Context.Agents.First(a => a.Id == victim.Id);
            var target = this.Context.Agents.First(a => a.Id == victim.TargetId);
            var killer = this.Context.Agents.First(a => a.TargetId == victim.Id);

            killer.TargetId = target.Id;
            if (target.Life < 5)
                target.Life++;

            victimToUpdate.Life = 0;
            victimToUpdate.Status = "dead";
            victimToUpdate.MissionId = null;
            victimToUpdate.TargetId = null;

            var action = new Models.Action
            {
                GameId = victimToUpdate.GameId,
                TargetId = victimToUpdate.Id,
                KillerId = target.Id,
                Type = Constantes.ACTTION_TYPE_UNMASK
            };

            this.Context.Agents.UpdateRange(new List<Agent> { victimToUpdate, killer, target });
            this.Context.Actions.Add(action);
        }

        private void WrongKiller(Agent agent)
        {
            //socket.emit("wrong-killer", agent);
            var agentToUpdate = this.Context.Agents.First(a => a.Id == agent.Id);
            agentToUpdate.Life--;
            if (agent.Life <= 0)
            {
                agent.Status = "dead";
                var killer = this.Context.Agents.First(a => a.TargetId == agent.Id);
                killer.TargetId = agentToUpdate.TargetId;
                agentToUpdate.TargetId = null;
                agentToUpdate.MissionId = null;
                var action = new Models.Action
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
                var action = new Models.Action
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