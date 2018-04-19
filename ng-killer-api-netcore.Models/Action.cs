using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgKillerApiCore.Models
{
    public class Action : IEntity<long>
    {
        public Action(){
            DateCreation = DateTime.Now;
        }
        public long Id { get; set; }
        public string Type { get; set; }
        public DateTime DateCreation{ get; set; }

        public string KillerName { get; set; }
        public string TargetName { get; set; }
        public string MissionTitle { get; set; }


        public long GameId { get; set; }
        public string KillerId { get; set; }
        public string TargetId { get; set; }
        public long MissionId { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }
        [ForeignKey(nameof(KillerId))]
        public Agent Killer { get; set; }
        [ForeignKey(nameof(TargetId))]
        public Agent Target { get; set; }

        [ForeignKey(nameof(MissionId))]
        public Mission Mission { get; set; }
    }
}