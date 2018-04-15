﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Transactions;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class AgentsController : ApiController<Agent, string, KillerContext, RequestHub>
    {
        public AgentsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
            Includes.Add(a => a.Game);
            //Includes.Add(a => a.Mission);
        }

        public override Agent Create([FromBody]Agent item)
        {
            using (var transaction = new TransactionScope())
            {
                var agent = base.Create(item);
                SendToAll(item.GameId.ToString(), "Request", new Request{
                    GameId = agent.GameId,
                    EmitterId = agent.Id,
                    Emitter = agent,
                    Type = Constantes.REQUEST_TYPE_NEW_AGENT
                });
                transaction.Complete();
                return agent;
            }
        }
        public override IActionResult GetById(string id)
        {
            Agent item = Context.Agents
                .Include(a => a.Game)
                .Include(a => a.Mission)
                .Include(a => a.Target)
                .Include(a => a.Requests)
                .FirstOrDefault(a => a.Id.Equals(id));
            if (item == null)
            {
                return NotFound();
            }
            //get only non treated request
            item.Requests = item.Requests.Where(r => !r.IsTreated).ToList();
            return new ObjectResult(item);
        }

        [HttpGet("{id}/getForUnmask")]
        public ICollection<Agent> GetForUnmask(string id)
        {
            var agent = Context.Agents.Find(id);
            var agents = Context.Agents.Where(a => a.Id != agent.Id && a.GameId == agent.GameId);

            return agents.ToList();
        }
    }
}