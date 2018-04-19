using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NgKillerApiCore.Models
{
    public class Agent : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }

        public long? MissionId { get; set; }
        public string TargetId { get; set; }
        public long GameId { get; set; }

        [ForeignKey(nameof(MissionId))]
        public Mission Mission { get; set; }

        public string Status { get; set; }
        public int Life { get; set; }

        [ForeignKey(nameof(TargetId))]
        public Agent Target { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }


        [InverseProperty("Receiver")]
        public List<Request> Requests { get; set; }
        [InverseProperty("Killer")]
        public List<Action> ActionsAsKiller { get; set; }
        [InverseProperty("Target")]
        public List<Action> ActionsAsTarget { get; set; }

        [NotMapped]
        public List<Action> Actions{ 
            get{
                var actions = new List<Action>();
                if(ActionsAsKiller!=null)
                {
                    actions.AddRange(ActionsAsKiller);
                }
                if(ActionsAsTarget!=null)
                {
                    actions.AddRange(ActionsAsTarget);
                }
                return actions.OrderByDescending(a=>a.DateCreation).ToList();
            }
        }
    }
}