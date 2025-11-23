namespace DentalHorizonePRMS.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
