using BazaMuzyczna.Models;

namespace BazaMuzyczna.DataTransferObjects
{
    public class PlaybackDTO
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public TrackTitleDTO? Track { get; set; }
        public int Quantity { get; set; }
    }
}
