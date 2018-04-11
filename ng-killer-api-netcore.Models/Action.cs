

using System;
using System.Collections.Generic;

namespace ng_killer_api_netcore.Models
{
    public class Action
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public Agent Killer { get; set; }
        public Agent Target { get; set; }
        public DateTime DateCreation{ get; set; }
    }
}