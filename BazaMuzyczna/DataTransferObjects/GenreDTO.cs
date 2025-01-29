namespace BazaMuzyczna.DataTransferObjects
{
    public class GenreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TrackTitleDTO>? Tracks { get; set; }
    }
}
