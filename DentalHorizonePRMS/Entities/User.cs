namespace DentalHorizonePRMS.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Admin";
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
    }
}
