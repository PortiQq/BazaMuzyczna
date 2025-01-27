namespace BazaMuzyczna.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly ReleaseDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
