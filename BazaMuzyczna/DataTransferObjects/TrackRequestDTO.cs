namespace BazaMuzyczna.DataTransferObjects
{
    public class TrackRequestDTO
    {
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public int AlbumId { get; set; }
        public int GenreId { get; set; }

    }
}
