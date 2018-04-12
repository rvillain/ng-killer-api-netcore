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
        public SocketManager()
        {
        }
        public async Task ManageSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var str = System.Text.Encoding.Default.GetString(buffer);
            var req = JsonConvert.DeserializeObject<Request>(str);

            //switch (req.Type)
            //{
            //    case Constantes.REQUEST_TYPE_JOIN_ROOM:
            //        break;
            //    default:
            //        killerContext.Requests.Add(req);
            //        killerContext.SaveChanges();
            //        break;

            //}

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
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
