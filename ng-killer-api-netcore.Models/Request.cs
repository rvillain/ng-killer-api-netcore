using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgKillerApiCore.Models
{
    public class Request : IEntity<long>
    {
        public Request(){
            DateCreation = DateTime.Now;
        }
        public long Id { get; set; }
        public string Type { get; set; }
        public DateTime DateCreation{ get; set; }
        public string Data { get; set; }
        public bool IsTreated { get; set; }

        public string EmitterId { get; set; }
        public string ReceiverId { get; set; }
        public long GameId { get; set; }
        public long? ParentRequestId { get; set; }

        [ForeignKey(nameof(EmitterId))]
        public Agent Emitter { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public Agent Receiver { get; set; }
        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }
        [ForeignKey(nameof(ParentRequestId))]
        public Request ParentRequest { get; set; }
    }
}