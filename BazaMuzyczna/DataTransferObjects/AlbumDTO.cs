namespace BazaMuzyczna.DataTransferObjects
{
    public class AlbumDTO
    {
        public string Name { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public UserDTO User { get; set; }
        public List<TrackDTO> Tracks { get; set; }
        
    }
}
