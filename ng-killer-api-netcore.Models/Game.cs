using System.Collections.Generic;

namespace NgKillerApiCore.Models
{
    public class Game : IEntity<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public List<Agent> Agents { get; set; }
        public List<Mission> Missions { get; set; }
        //public List<Action> Actions { get; set; }
    }
}