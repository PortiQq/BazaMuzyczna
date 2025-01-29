namespace BazaMuzyczna.DataTransferObjects
{
    public class AlbumDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public UserNameDTO User { get; set; }
        public List<TrackDTO>? Tracks { get; set; }
        
    }
}
