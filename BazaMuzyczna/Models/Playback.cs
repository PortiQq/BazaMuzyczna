namespace BazaMuzyczna.Models
{
    public class Playback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public int Quantity { get; set; }
    }
}
