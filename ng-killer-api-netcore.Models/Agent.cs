namespace ng_killer_api_netcore.Models
{
    public class Agent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public Mission Mission { get; set; }
        public Game Game { get; set; }

        public string Status { get; set; }
    }
}