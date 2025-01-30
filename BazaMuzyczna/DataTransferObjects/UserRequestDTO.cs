namespace BazaMuzyczna.DataTransferObjects
{
    public class UserRequestDTO
    {
        public string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
