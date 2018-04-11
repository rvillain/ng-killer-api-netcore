using System;

namespace NgKillerApiCore.Models
{
    public class Request
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public Agent Emitter { get; set; }
        public Agent Receiver { get; set; }
        public DateTime DateCreation{ get; set; }
    }
}