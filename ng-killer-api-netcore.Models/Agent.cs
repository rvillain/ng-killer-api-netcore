using System.ComponentModel.DataAnnotations.Schema;

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
    }
}