using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NgKillerApiCore
{
    public class SocketManager
    {
        private KillerContext killerContext;
        public SocketManager(KillerContext _killerContext)
        {
            this.killerContext = _killerContext;
        }
        public async Task ManageSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var str = System.Text.Encoding.Default.GetString(buffer);
            var req = JsonConvert.DeserializeObject<Request>(str);

            switch (req.Type)
            {
                case Constantes.REQUEST_TYPE_JOIN_ROOM:
                    break;
                default:
                    killerContext.Requests.Add(req);
                    killerContext.SaveChanges();
                    break;

            }

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public void ChangeMission(Agent agent)
        {
            var missions = killerContext.Missions.Where(m => m.GameId == agent.Game.Id && !m.IsUsed).ToList();
            if(missions.Count > 0)
            {
                var mission = missions.First();
                mission.IsUsed = true;
                killerContext.Update(mission);
                var agentToUpdate = killerContext.Agents.First(a => a.Id == agent.Id);
                agentToUpdate.MissionId = mission.Id;
                agentToUpdate.Life--;
                killerContext.Update(agentToUpdate);
            }
        }
        public void Suicide(Agent agent)
        {
            var agentToUpdate = killerContext.Agents.First(a => a.Id == agent.Id);
            agentToUpdate.Life = 0;
            agentToUpdate.Status = "dead";
            var killer = killerContext.Agents.First(a=>a.TargetId == agent.Id);
            killer.TargetId = agentToUpdate.TargetId;
            Models.Action action = new Models.Action
            {
                GameId = agent.GameId,
                KillerId = agent.Id,
                Type = Constantes.ACTTION_TYPE_SUICIDE
            };
            killerContext.Actions.Add(action);

            killerContext.SaveChanges();
        }

        public void Kill(Agent victim)
        {
            var killerToUpdate = killerContext.Agents.First(a => a.TargetId == victim.Id);
            var mission = killerToUpdate.Mission;
            var victimToUpdate = killerContext.Agents.First(a => a.Id == victim.Id);
            var newTarget = killerContext.Agents.First(a => a.Id == victimToUpdate.Id);

            killerToUpdate.MissionId = victimToUpdate.MissionId;
            killerToUpdate.TargetId = newTarget.Id;
            if(killerToUpdate.Life<5)
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

            killerContext.Agents.UpdateRange(new List<Agent> { killerToUpdate, victimToUpdate});
            killerContext.Actions.Add(action);
        }

        public void Unmask(Agent victim)
        {
            // Killer => Victim => Target
            //Target unmask victim
            var victimToUpdate = killerContext.Agents.First(a => a.Id == victim.Id);
            var target = killerContext.Agents.First(a => a.Id == victim.TargetId);
            var killer = killerContext.Agents.First(a => a.TargetId == victim.Id);

            killer.TargetId = target.Id;
            if(target.Life<5)
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

            killerContext.Agents.UpdateRange(new List<Agent> { victimToUpdate, killer, target });
            killerContext.Actions.Add(action);
        }

        public void WrongKiller(Agent agent)
        {
            //socket.emit("wrong-killer", agent);
            var agentToUpdate = killerContext.Agents.First(a => a.Id == agent.Id);
            agentToUpdate.Life--;
            if (agent.Life <= 0)
            {
                agent.Status = "dead";
                var killer = killerContext.Agents.First(a => a.TargetId == agent.Id);
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

                killerContext.Agents.UpdateRange(new List<Agent> { agentToUpdate, killer });
                killerContext.Actions.Add(action);
            }
            else
            {
                var action = new Models.Action
                {
                    GameId = agentToUpdate.GameId,
                    KillerId = agentToUpdate.Id,
                    Type = Constantes.ACTTION_TYPE_WRONG_KILLER
                };

                killerContext.Agents.UpdateRange(new List<Agent> { agentToUpdate });
                killerContext.Actions.Add(action);
            }
        }

        //public void StartTribunal(socket, agents)
        //{
        //    var newTribunal = new Tribunal();
        //    newTribunal.killer = agents.killer._id;
        //    newTribunal.target = agents.target._id;
        //    newTribunal.game = public void getRoom(socket);
        //    Tribunal.create(newTribunal, (err, t) => {
        //    updateTribunal(t, socket);
        //    //Init period: 1 minute
        //    setTimeout(function() {
        //        t.status = "started";
        //        updateTribunal(t, socket);
        //    }, 6000)
        //  });
        //}

    }
}
