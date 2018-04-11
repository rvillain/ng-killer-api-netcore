namespace NgKillerApiCore.Models
{
    public class Agent : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        

        public string Status { get; set; }

        public long MissionId { get; set; }
        public Mission Mission { get; set; }

        public long GameId { get; set; }
        public Game Game { get; set; }
    }
}