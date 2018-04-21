using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Hubs
{
    /// <summary>
    /// Hub des requêtes des joueurs
    /// </summary>
    public class RequestHub: Hub
    {
        /// <summary>
        /// Rejoindre la room de la partie
        /// </summary>
        /// <param name="roomName">id du game</param>
        /// <returns></returns>
        public Task JoinRoom(string roomName)
        {
            return Groups.AddAsync(Context.ConnectionId, roomName);
        }

        /// <summary>
        /// Quitter la room
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveAsync(Context.ConnectionId, roomName);
        }
    }
}
