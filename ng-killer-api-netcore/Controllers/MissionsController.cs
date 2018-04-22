using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Hubs;
using Microsoft.EntityFrameworkCore;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Gestion des missions
    /// </summary>
    [Route("api/[controller]")]
    public class MissionsController : ApiController<Mission, long, KillerContext, RequestHub>
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        public MissionsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
        }

        /// <summary>
        /// Liste des missions génériques
        /// </summary>
        /// <returns></returns>
        [HttpGet("generics")]
        public ICollection<Mission> GetGenerics()
        {
            var missions = Context.Missions.AsNoTracking().Where(m=>m.GameId == null);

            return missions.ToList();
        }

        /// <summary>
        /// Importer des missions
        /// </summary>
        /// <returns></returns>
        [HttpPost("import")]
        public void Import([FromBody]List<Mission> missions)
        {
            foreach(var mission in missions.Where(m=>m.Title != null))
            {
                var exist = Context.Missions.Any(m => m.Title.ToLower() == mission.Title.ToLower() && m.GameId == null);
                if (!exist && mission.Title.Count()>=5)
                {
                    mission.GameId = null;
                    Context.Add(mission);
                }
            }
            Context.SaveChanges();
        }
    }
}