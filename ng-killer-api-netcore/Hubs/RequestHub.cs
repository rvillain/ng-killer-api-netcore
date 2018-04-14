using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Hubs
{
    public class RequestHub: Hub
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.AddAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveAsync(Context.ConnectionId, roomName);
        }
    }
}
