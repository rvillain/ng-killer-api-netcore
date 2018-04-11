using System;

namespace NgKillerApiCore.Models
{
    public class Action : IEntity<long>
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public DateTime DateCreation{ get; set; }

        public Agent Killer { get; set; }
        public Agent Target { get; set; }
        public Game Game { get; set; }
        public Mission Mission { get; set; }
    }
}