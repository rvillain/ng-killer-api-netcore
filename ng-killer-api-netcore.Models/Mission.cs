
namespace NgKillerApiCore.Models
{
    public class Mission : IEntity<long>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }

        public long? GameId { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }
        public bool IsUsed { get; set; }
    }
}