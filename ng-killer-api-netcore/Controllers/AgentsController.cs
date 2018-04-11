using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class AgentsController : ApiController<Agent, string, KillerContext>
    {
        public AgentsController(KillerContext context) : base(context)
        {
        }

        protected override void UpdateRange(Agent dbItem, Agent item)
        {
            dbItem.Name = item.Name;
            dbItem.Photo = item.Photo;
            dbItem.Status = item.Status;
        }
    }
}