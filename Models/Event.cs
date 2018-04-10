

using System;
using System.Collections.Generic;

namespace ng_killer_api_netcore.Models
{
    public class Event
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public Agent Emitter { get; set; }
        public Agent Receiver { get; set; }
        public DateTime DateCreation{ get; set; }
    }
}