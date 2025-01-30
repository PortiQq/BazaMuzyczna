using BazaMuzyczna.Models;

namespace BazaMuzyczna.DataTransferObjects
{
    public class TrackDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public int AlbumId { get; set; }
        public AlbumNameDTO? Album { get; set; }
        public GenreNameDTO? Genre { get; set; }
    }
}
