using System;

namespace NgKillerApiCore.Models
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