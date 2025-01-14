namespace BazaMuzyczna.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Album> Albums { get; set; }
        public ICollection<Playback> Playbacks { get; set; }
    }
}
