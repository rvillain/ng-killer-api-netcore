using Newtonsoft.Json;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;

namespace NgKillerApiCore
{
    public class MessageHandler : WebSocketHandler
    {
        private SocketManager _socketManager;
        private KillerContext _killerContext;

        public MessageHandler(WebSocketConnectionManager webSocketConnectionManager, SocketManager socketManager, KillerContext killerContext) : base(webSocketConnectionManager)
        {
            _socketManager = socketManager;
            _killerContext = killerContext;
        }
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} is now connected"
            };

            await SendMessageToAllAsync(message);
        }

        //public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string json)
        //{
        //    try
        //    {

        //        var request = JsonConvert.DeserializeObject<Request>(json);

        //        if (request != null)
        //        {
        //            switch (request.Type)
        //            {
        //                case Constantes.REQUEST_TYPE_JOIN_ROOM:
        //                    break;
        //                default:
        //                    _killerContext.Requests.Add(request);
        //                    _killerContext.SaveChanges();
        //                    break;

        //            }

        //            var message = new Message()
        //            {
        //                MessageType = MessageType.Text,
        //                Data = json
        //            };

        //            await SendMessageToAllAsync(message);
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            await base.OnDisconnected(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} disconnected"
            };
            await SendMessageToAllAsync(message);
        }
    }
}
