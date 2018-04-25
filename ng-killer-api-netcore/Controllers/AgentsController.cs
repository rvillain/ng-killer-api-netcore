using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using NgKillerApiCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Gestion dezs agents
    /// </summary>
    [Route("api/[controller]")]
    public class AgentsController : ApiController<Agent, string, KillerContext, RequestHub>
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        public AgentsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
            Includes.Add(a => a.Game);
            //Includes.Add(a => a.Mission);
        }

        /// <summary>
        /// Rejoindre une partie
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public override Agent Create([FromBody]Agent agent)
        {
            var createdAgent = base.Create(agent);
            SendToAll(agent.GameId.ToString(), Constantes.REQUEST_METHOD_NAME, new Request{
                GameId = createdAgent.GameId,
                EmitterId = createdAgent.Id,
                Emitter = createdAgent,
                Type = Constantes.REQUEST_TYPE_NEW_AGENT
            });
            return createdAgent;
        }

        /// <summary>
        /// Infos de l'agent 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IActionResult GetById(string id)
        {
            Agent item = Context.Agents.AsNoTracking()
                .Include(a => a.Game)
                .Include(a => a.Mission)
                .Include(a => a.Target)
                .Include(a => a.Requests)
                .Include(a => a.ActionsAsKiller)
                .Include(a => a.ActionsAsTarget)
                .FirstOrDefault(a => a.Id.Equals(id));
            if (item == null)
            {
                return NotFound();
            }
            //get only non treated request
            item.Requests = item.Requests.Where(r => !r.IsTreated).OrderByDescending(r => r.DateCreation).ToList();
            return new ObjectResult(item);
        }

        /// <summary>
        /// Liste des agent démasquables
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/getForUnmask")]
        public ICollection<Agent> GetForUnmask(string id)
        {
            var agent = Context.Agents.Find(id);
            var agents = Context.Agents.AsNoTracking().Where(a => a.Id != agent.Id && a.GameId == agent.GameId);

            return agents.ToList();
        }

        /// <summary>
        /// Ajouter un device pour push notification
        /// </summary>
        /// <param name="id"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost("{id}/addDevice")]
        public void AddDevice(string id, [FromBody]Device device)
        {
            var agent = this.Context.Agents.Find(id);
            var existingDevice = this.Context.Devices.FirstOrDefault(d=>d.Name==id);
            if(existingDevice!=null){
                this.Context.Remove(existingDevice);
            }
            device.Name = agent.Id;
            this.Context.Devices.Add(device);
            this.Context.SaveChanges();
            return;
        }
    }
}