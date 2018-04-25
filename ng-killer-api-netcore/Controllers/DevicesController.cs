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
    public class DevicesController : ApiController<Device, int, KillerContext, RequestHub>
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        public DevicesController(KillerContext context, IHubContext<RequestHub> hubContext) : base(context, hubContext)
        {
        }

    }
}