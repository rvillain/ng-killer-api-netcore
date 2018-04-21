using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Hubs;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Gestion des missions
    /// </summary>
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
        [HttpPost("generics")]
        public ICollection<Mission> GetGenerics()
        {
            var missions = Context.Missions.Where(m=>m.GameId == null);

            return missions.ToList();
        }
    }
}