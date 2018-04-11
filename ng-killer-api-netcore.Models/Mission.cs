namespace NgKillerApiCore.Models
{
    public class Mission
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }
        public Agent Agent { get; set; }
    }
}