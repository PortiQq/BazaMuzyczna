﻿namespace BazaMuzyczna.DataTransferObjects
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<AlbumNameDTO>? Albums { get; set; }
    }
}
