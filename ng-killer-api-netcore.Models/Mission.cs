using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgKillerApiCore.Models
{
    public class Mission : IEntity<long>
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }
        public string AgentId { get; set; }

        [ForeignKey(nameof(AgentId))]
        public Agent Agent { get; set; }
    }
}