using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Hubs;

namespace NgKillerApiCore.Controllers
{

    public class MissionsController : ApiController<Mission, long, KillerContext, RequestHub>
    {
        public MissionsController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
            //if (!context.Missions.Any())
            //{
            //    context.Missions.Add(new Mission { Id = 1, Title = "Test", Difficulty = "Easy" });
            //    context.SaveChanges();
            //}
        }
        protected override void UpdateRange(Mission dbItem, Mission item)
        {
            dbItem.Title = item.Title;
            dbItem.Difficulty = item.Difficulty;
        }

        [HttpPost("generics")]
        public ICollection<Mission> GetGenerics()
        {
            var missions = Context.Missions.Where(m=>m.GameId == null);

            return missions.ToList();
        }
    }
}