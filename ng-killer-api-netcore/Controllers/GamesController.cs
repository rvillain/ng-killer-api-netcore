using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : ApiController<Game, long, KillerContext>
    {
        public GamesController(KillerContext context) : base(context)
        {
        }

        protected override void UpdateRange(Game dbItem, Game item)
        {
            dbItem.Status = item.Status;
        }
    }
}