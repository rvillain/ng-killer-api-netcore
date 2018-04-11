using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{

    public class MissionsController : ApiController<Mission, long, KillerContext>
    {
        public MissionsController(KillerContext context) : base(context)
        {
            if (!context.Missions.Any())
            {
                context.Missions.Add(new Mission { Id = 1, Title = "Test", Difficulty = "Easy" });
                context.SaveChanges();
            }
        }
    }
}